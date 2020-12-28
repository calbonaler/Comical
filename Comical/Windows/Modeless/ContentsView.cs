using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Comical.Core;

namespace Comical
{
	public partial class ContentsView : WeifenLuo.WinFormsUI.Docking.DockContent
	{
		public ContentsView()
		{
			InitializeComponent();
			dgvImages.RowTemplate.Height = ThumbnailSize.Height;
			clmViewMode.DataSource = Enum.GetNames(typeof(ImageViewMode));
		}

		ImageReferenceCollection _images;
		static readonly Size ThumbnailSize = new Size(118, 118);

		Viewer DefaultViewer => DockPanel?.Contents?.OfType<Viewer>()?.FirstOrDefault(v => v.Pane.IsActiveDocumentPane);

		public event EventHandler ImageReferenceSelected
		{
			add => dgvImages.SelectionChanged += value;
			remove => dgvImages.SelectionChanged -= value;
		}

		public event EventHandler ExportRequested
		{
			add => itmExport.Click += value;
			remove => itmExport.Click -= value;
		}

		public event EventHandler ExtractRequested
		{
			add => itmExtract.Click += value;
			remove => itmExport.Click -= value;
		}

		public event EventHandler BookmarkRequested
		{
			add => itmAddToBookmark.Click += value;
			remove => itmAddToBookmark.Click -= value;
		}

		public event EventHandler<FileDroppedEventArgs> FileDropped;

		public IDisposable BeginAsyncWork()
		{
			dgvImages.ReadOnly = true;
			dgvImages.Refresh();
			return new DelegateDisposable(() =>
			{
				dgvImages.ReadOnly = false;
				dgvImages.Refresh();
			});
		}

		public IEnumerable<int> SelectedIndicies => dgvImages.SelectedRows.Cast<DataGridViewRow>().Select(r => r.Index);

		public IEnumerable<ImageReference> SortedSelectedImages => SelectedIndicies.OrderBy(x => x).Select(x => _images[x]);

		public void SetImages(ImageReferenceCollection value)
		{
			if (_images == value)
				return;
			if (_images != null)
			{
				_images.CollectionChanged -= Images_CollectionChanged;
				_images.CollectionItemPropertyChanged -= Images_CollectionItemPropertyChanged;
			}
			_images = value;
			if (value != null)
			{
				value.CollectionChanged += Images_CollectionChanged;
				value.CollectionItemPropertyChanged += Images_CollectionItemPropertyChanged;
				Images_CollectionChanged(value, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		public void AddImages(IEnumerable<ImageReference> images)
		{
			if (images == null)
				throw new ArgumentNullException(nameof(images));
			using (_images.EnterUnnotifiedSection())
			{
				foreach (var image in images)
					_images.Add(image);
			}
		}

		void Images_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) => this.InvokeIfNeeded(() =>
		{
			dgvImages.RowCount = _images.Count;
			if (_images.Count == 0 && DefaultViewer != null)
				DefaultViewer.Image = null;
			dgvImages.Invalidate();
		});

		void Images_CollectionItemPropertyChanged(object sender, CollectionItemPropertyChangedEventArgs e) => this.InvokeIfNeeded(() =>
		{
		    foreach (var group in e.PropertyNames)
		    {
		  	  var index = _images.IndexOf((ImageReference)group.Key);
		  	  if (index >= 0)
		  		  dgvImages.UpdateCellValue(1, index);
		    }
		});

		protected override string GetPersistString() => "ImageList";

		public int FirstSelectedRowIndex
		{
			get => dgvImages.SelectedRows.Count > 0 ? SelectedIndicies.Min() : -1;
			set
			{
				for (var i = 0; i < _images.Count; i++)
					dgvImages.Rows[i].Selected = i == value;
				if (value >= 0 && value < _images.Count)
					dgvImages.FirstDisplayedScrollingRowIndex = value;
			}
		}

		public void OpenFirstSelectedImage()
		{
			if (FirstSelectedRowIndex >= 0)
			{
				var content = DockPanel.ActiveContent;
				var viewer = new Viewer();
				viewer.Text = FirstSelectedRowIndex.ToString(CultureInfo.CurrentCulture);
				viewer.Image = _images[FirstSelectedRowIndex].Data;
				viewer.Show(DockPanel);
				content.DockHandler.Activate();
			}
		}

		public void DeleteSelectedImages()
		{
			foreach (var x in SortedSelectedImages.ToArray())
				_images.Remove(x);
		}

		public void SetViewModes(bool startAtLeft)
		{
			var start = SelectedIndicies.Last();
			var count = SelectedIndicies.First() - start + 1;
			if (count < 0)
				count = 0;
			else if (count > _images.Count - start)
				count = _images.Count - start;
			for (var i = 0; i < count; i++)
				_images[i + start].ViewMode = i % 2 == (startAtLeft ? 0 : 1) ? ImageViewMode.Left : ImageViewMode.Right;
		}

		public void InvertViewMode()
		{
			foreach (var image in SortedSelectedImages)
			{
				if (image.ViewMode == ImageViewMode.Left)
					image.ViewMode = ImageViewMode.Right;
				else if (image.ViewMode == ImageViewMode.Right)
					image.ViewMode = ImageViewMode.Left;
			}
		}

		void dgvImages_SelectionChanged(object sender, EventArgs e)
		{
			var count = dgvImages.SelectedRows.Count;
			if (DefaultViewer != null && count == 1)
			{
				DefaultViewer.Text = FirstSelectedRowIndex.ToString(CultureInfo.CurrentCulture);
				try { DefaultViewer.Image = _images[FirstSelectedRowIndex].Data; }
				catch (ArgumentException) { }
			}
			itmOpen.Visible = sepImage1.Visible = count == 1;
			itmAddToBookmark.Visible = sepImage2.Visible =
				itmExport.Visible = itmExtract.Visible = sepImage3.Visible =
				itmStartViewModeSettingLeft.Visible = itmStartViewModeSettingRight.Visible = sepImage4.Visible =
				itmDelete.Visible = count > 0;
		}

		void dgvImages_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		void dgvImages_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop) && FileDropped != null)
				FileDropped(this, new FileDroppedEventArgs(e.Data.GetData(DataFormats.FileDrop) as string[], e.KeyState, e.X, e.Y));
		}

		void dgvImages_RowMoving(object sender, Controls.RowMovingEventArgs e)
		{
			if (e.Source == dgvImages)
				_images.MoveRange(e.SourceRows[0].Index, e.SourceRows.Count, e.Destination);
		}

		void dgvImages_QueryRowDragDropEffect(object sender, Controls.QueryRowDragDropEffectEventArgs e) => e.Effect = e.Source == dgvImages ? DragDropEffects.Move : DragDropEffects.None;

		void dgvImages_CellDoubleClick(object sender, DataGridViewCellEventArgs e) => OpenFirstSelectedImage();

		void dgvImages_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex < 0 || e.RowIndex >= _images.Count)
				return;
			if (dgvImages.Columns[e.ColumnIndex] == clmViewMode)
				e.Value = _images[e.RowIndex].ViewMode.ToString();
			else if (dgvImages.Columns[e.ColumnIndex] == clmImage)
				e.Value = _images[e.RowIndex].Data;
		}

		void dgvImages_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex >= 0 && e.RowIndex < _images.Count && dgvImages.Columns[e.ColumnIndex] == clmViewMode && e.Value != null && Enum.TryParse(e.Value.ToString(), out ImageViewMode mode))
				_images[e.RowIndex].ViewMode = mode;
		}

		int rowIndex = -1;

		void dgvImages_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) => rowIndex = e.Row.Index;

		void dgvImages_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
		{
			if (rowIndex >= 0)
			{
				_images.RemoveAt(rowIndex);
				rowIndex = -1;
			}
		}

		void itmOpen_Click(object sender, EventArgs e) => OpenFirstSelectedImage();

		void itmDelete_Click(object sender, EventArgs e) => DeleteSelectedImages();

		void itmStartViewModeSettingLeft_Click(object sender, EventArgs e) => SetViewModes(true);

		void itmStartViewModeSettingRight_Click(object sender, EventArgs e) => SetViewModes(false);
	}

	public class FileDroppedEventArgs : EventArgs
	{
		public FileDroppedEventArgs(IEnumerable<string> fileNames, int keyState, int x, int y)
		{
			FileNames = fileNames;
			_keyState = keyState;
			X = x;
			Y = y;
		}

		readonly int _keyState;

		public IEnumerable<string> FileNames { get; }

		public bool MouseLeft => (_keyState & 1) != 0;

		public bool MouseRight => (_keyState & 2) != 0;

		public bool MouseMiddle => (_keyState & 16) != 0;

		public bool Shift => (_keyState & 4) != 0;

		public bool Control => (_keyState & 8) != 0;

		public bool Alt => (_keyState & 32) != 0;

		public int X { get; }

		public int Y { get; }
	}
}
