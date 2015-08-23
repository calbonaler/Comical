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
			dgvImages.Font = new Font("Meiryo", 9);
		}

		ImageReferenceCollection _images;
		static readonly Size ThumbnailSize = new Size(118, 118);

		Viewer DefaultViewer => DockPanel?.Contents?.OfType<Viewer>()?.FirstOrDefault(v => v.Pane.IsActiveDocumentPane);

		public event EventHandler ImageReferenceSelected
		{
			add { dgvImages.SelectionChanged += value; }
			remove { dgvImages.SelectionChanged -= value; }
		}

		public event EventHandler ExportRequested
		{
			add { itmExport.Click += value; }
			remove { itmExport.Click -= value; }
		}

		public event EventHandler ExtractRequested
		{
			add { itmExtract.Click += value; }
			remove { itmExport.Click -= value; }
		}

		public event EventHandler BookmarkRequested
		{
			add { itmAddToBookmark.Click += value; }
			remove { itmAddToBookmark.Click -= value; }
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

		public void SelectAll() { dgvImages.SelectAll(); }

		public void InvertSelections()
		{
			foreach (DataGridViewRow row in dgvImages.Rows)
				row.Selected = !row.Selected;
		}

		void Images_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.InvokeIfNeeded(() =>
			{
				dgvImages.RowCount = _images.Count;
				if (_images.Count == 0 && DefaultViewer != null)
					DefaultViewer.Image = null;
				dgvImages.Invalidate();
			});
		}

		void Images_CollectionItemPropertyChanged(object sender, CompositePropertyChangedEventArgs<ImageReference> e)
		{
			this.InvokeIfNeeded(() =>
			{
				foreach (var group in e.PropertyNames)
				{
					int index = _images.IndexOf(group.Key);
					if (index >= 0)
						dgvImages.UpdateCellValue(1, index);
				}
			});
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			dgvImages.RowTemplate.Height = ThumbnailSize.Height;
			clmImage.Width = ThumbnailSize.Width;
			clmViewMode.DataSource = Enum.GetNames(typeof(ImageViewMode));
		}

		protected override string GetPersistString() => "ImageList";

		public int FirstSelectedRowIndex
		{
			get
			{
				if (dgvImages.SelectedRows.Count > 0)
					return SelectedIndicies.Min();
				else
					return -1;
			}
			set
			{
				for (int i = 0; i < _images.Count; i++)
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
				Viewer viewer = new Viewer();
				viewer.Text = FirstSelectedRowIndex.ToString(CultureInfo.CurrentCulture);
				viewer.Image = _images[FirstSelectedRowIndex].GetImage();
				viewer.Show(DockPanel);
				content.DockHandler.Activate();
			}
		}

		public void DeleteSelectedImages()
		{
			foreach (var x in SelectedIndicies.OrderBy(x => x).Select(x => _images[x]).ToArray())
				_images.Remove(x);
		}

		private void dgvImages_SelectionChanged(object sender, EventArgs e)
		{
			int count = dgvImages.SelectedRows.Count;
			if (DefaultViewer != null)
			{
				DefaultViewer.Description = count < 1 ? Properties.Resources.ViewerDescription_NoSelection :
					string.Format(CultureInfo.CurrentCulture, Properties.Resources.ViewerDescription_Selection, count);
				if (count == 1)
				{
					DefaultViewer.Text = FirstSelectedRowIndex.ToString(CultureInfo.CurrentCulture);
					try { DefaultViewer.Image = _images[FirstSelectedRowIndex].GetImage(); }
					catch (ArgumentException) { }
				}
			}
			itmOpen.Visible = count == 1;
			itmAddToBookmark.Visible = itmDelete.Visible = itmExport.Visible = itmExtract.Visible = sepImage1.Visible = sepImage2.Visible = count > 0;
		}

		private void dgvImages_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void dgvImages_DragDrop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop) && FileDropped != null)
				FileDropped(this, new FileDroppedEventArgs(e.Data.GetData(DataFormats.FileDrop) as string[], e.KeyState, e.X, e.Y));
		}

		private void dgvImages_RowMoving(object sender, Controls.RowMovingEventArgs e)
		{
			if (e.Source == dgvImages)
				_images.MoveRange(e.SourceRows[0].Index, e.SourceRows.Count, e.Destination);
		}

		private void dgvImages_QueryActualDestination(object sender, Controls.QueryActualDestinationEventArgs e)
		{
			e.Effect = DragDropEffects.Move;
			if (e.Source.Name == "dgvBookmarks")
				e.ActualDestination = -1;
		}

		private void dgvImages_CellDoubleClick(object sender, DataGridViewCellEventArgs e) { OpenFirstSelectedImage(); }

		private void dgvImages_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex < 0 || e.RowIndex >= _images.Count)
				return;
			if (dgvImages.Columns[e.ColumnIndex] == clmViewMode)
				e.Value = _images[e.RowIndex].ViewMode.ToString();
			else if (dgvImages.Columns[e.ColumnIndex] == clmImage)
				e.Value = _images[e.RowIndex].GetImage(ThumbnailSize);
		}

		private void dgvImages_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex >= 0 && e.RowIndex < _images.Count && dgvImages.Columns[e.ColumnIndex] == clmViewMode && e.Value != null)
			{
				ImageViewMode mode;
				if (Enum.TryParse(e.Value.ToString(), out mode))
					_images[e.RowIndex].ViewMode = mode;
			}
		}

		int rowIndex = -1;

		private void dgvImages_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) { rowIndex = e.Row.Index; }

		private void dgvImages_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
		{
			if (rowIndex >= 0)
			{
				_images.RemoveAt(rowIndex);
				rowIndex = -1;
			}
		}

		private void itmOpen_Click(object sender, EventArgs e) { OpenFirstSelectedImage(); }

		private void itmDelete_Click(object sender, EventArgs e) { DeleteSelectedImages(); }
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

		int _keyState;

		public IEnumerable<string> FileNames { get; }

		public bool MouseLeft => (_keyState & 1) != 0;

		public bool MouseRight => (_keyState & 2) != 0;

		public bool MouseMiddle =>  (_keyState & 16) != 0;

		public bool Shift => (_keyState & 4) != 0;

		public bool Control => (_keyState & 8) != 0;

		public bool Alt => (_keyState & 32) != 0;

		public int X { get; }

		public int Y { get; }
	}
}
