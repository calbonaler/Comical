using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Comical.Core;

namespace Comical
{
	public partial class ContentsView : WeifenLuo.WinFormsUI.Docking.DockContent
	{
		public ContentsView(ImageReferenceCollection images)
		{
			InitializeComponent();
			dgvImages.RowTemplate.Height = ThumbnailSize.Height;
			clmViewMode.DataSource = Enum.GetNames(typeof(ImageViewMode));

			_images = images;
			_images.CollectionChanged += Images_CollectionChanged;
			_images.CollectionItemPropertyChanged += Images_CollectionItemPropertyChanged;
			Images_CollectionChanged(_images, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		readonly ImageReferenceCollection _images;
		static readonly Size ThumbnailSize = new(118, 118);

		IEnumerable<Viewer> Viewers => DockPanel?.Contents?.OfType<Viewer>() ?? [];

		Viewer? ActiveViewer => DockPanel?.ActiveDocument as Viewer;

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

		public event EventHandler<FileDroppedEventArgs>? FileDropped;

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

		public IEnumerable<int> SelectedIndices => dgvImages.SelectedRows.Cast<DataGridViewRow>().Select(r => r.Index);

		void Images_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => this.InvokeIfNeeded(() =>
		{
			dgvImages.RowCount = _images.Count;
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				foreach (var viewer in Viewers.ToArray())
					viewer.Close();
			}
			dgvImages.Invalidate();
		});

		void Images_CollectionItemPropertyChanged(object? sender, CollectionItemPropertyChangedEventArgs e) => this.InvokeIfNeeded(() =>
		{
			foreach (var group in e.PropertyNames)
			{
				Debug.Assert(group.Key != null, "sender of ImageReference's PropertyChanged event must not be null");
				var index = _images.IndexOf((ImageReference)group.Key);
				if (index >= 0)
					dgvImages.UpdateCellValue(1, index);
			}
		});

		protected override string GetPersistString() => "ImageList";

		public void SelectSingleImage(int index)
		{
			for (var i = 0; i < _images.Count; i++)
				dgvImages.Rows[i].Selected = i == index;
			if (index >= 0 && index < _images.Count)
				dgvImages.FirstDisplayedScrollingRowIndex = index;
		}

		public void OpenFirstSelectedImage()
		{
			if (dgvImages.SelectedRows.Count > 0)
			{
				var content = DockPanel.ActiveContent;
				var firstSelectedIndex = SelectedIndices.Min();
				var viewer = new Viewer
				{
					Text = firstSelectedIndex.ToString(CultureInfo.CurrentCulture),
					Image = _images[firstSelectedIndex].Data
				};
				viewer.Show(DockPanel);
				content.DockHandler.Activate();
			}
		}

		public void DeleteSelectedImages()
		{
			foreach (var x in SelectedIndices.OrderByDescending(x => x).ToArray())
				_images.RemoveAt(x);
		}

		public void SetSelectedImagesViewModes(bool startAtLeft)
		{
			var start = SelectedIndices.Last();
			var count = SelectedIndices.First() - start + 1;
			if (count < 0)
				count = 0;
			else if (count > _images.Count - start)
				count = _images.Count - start;
			for (var i = 0; i < count; i++)
				_images[i + start].ViewMode = i % 2 == (startAtLeft ? 0 : 1) ? ImageViewMode.Left : ImageViewMode.Right;
		}

		public void InvertSelectedImagesViewModes()
		{
			foreach (var i in SelectedIndices)
			{
				var image = _images[i];
				if (image.ViewMode == ImageViewMode.Left)
					image.ViewMode = ImageViewMode.Right;
				else if (image.ViewMode == ImageViewMode.Right)
					image.ViewMode = ImageViewMode.Left;
			}
		}

		void dgvImages_SelectionChanged(object? sender, EventArgs e)
		{
			var count = dgvImages.SelectedRows.Count;
			if (ActiveViewer != null && count == 1)
			{
				var firstSelectedIndex = SelectedIndices.Min();
				ActiveViewer.Text = firstSelectedIndex.ToString(CultureInfo.CurrentCulture);
				try { ActiveViewer.Image = _images[firstSelectedIndex].Data; }
				catch (ArgumentException) { }
			}
			itmOpen.Visible = sepImage1.Visible = count == 1;
			itmAddToBookmark.Visible = sepImage2.Visible =
				itmExport.Visible = itmExtract.Visible = sepImage3.Visible =
				itmStartViewModeSettingLeft.Visible = itmStartViewModeSettingRight.Visible = sepImage4.Visible =
				itmDelete.Visible = count > 0;
		}

		void dgvImages_DragEnter(object? sender, DragEventArgs e)
		{
			Debug.Assert(e.Data != null, "I think this never happens.");
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		void dgvImages_DragDrop(object? sender, DragEventArgs e)
		{
			Debug.Assert(e.Data != null, "I think this never happens.");
			if (e.Data.GetDataPresent(DataFormats.FileDrop) && FileDropped != null)
			{
				var fileNames = (string[]?)e.Data.GetData(DataFormats.FileDrop);
				Debug.Assert(fileNames != null, "I think this never happens.");
				FileDropped(this, new FileDroppedEventArgs(fileNames, e.KeyState, e.X, e.Y));
			}
		}

		void dgvImages_RowMoving(object? sender, Controls.RowMovingEventArgs e)
		{
			if (e.Source == dgvImages)
				_images.MoveRange(e.SourceRows[0].Index, e.SourceRows.Count, e.Destination);
		}

		void dgvImages_QueryRowDragDropEffect(object? sender, Controls.QueryRowDragDropEffectEventArgs e) => e.Effect = e.Source == dgvImages ? DragDropEffects.Move : DragDropEffects.None;

		void dgvImages_CellDoubleClick(object? sender, DataGridViewCellEventArgs e) => OpenFirstSelectedImage();

		void dgvImages_CellValueNeeded(object? sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex < 0 || e.RowIndex >= _images.Count)
				return;
			if (dgvImages.Columns[e.ColumnIndex] == clmViewMode)
				e.Value = _images[e.RowIndex].ViewMode.ToString();
			else if (dgvImages.Columns[e.ColumnIndex] == clmImage)
				e.Value = _images[e.RowIndex].Data;
		}

		void dgvImages_CellValuePushed(object? sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex >= 0 && e.RowIndex < _images.Count && dgvImages.Columns[e.ColumnIndex] == clmViewMode && e.Value != null && Enum.TryParse(e.Value.ToString(), out ImageViewMode mode))
				_images[e.RowIndex].ViewMode = mode;
		}

		int rowIndex = -1;

		void dgvImages_UserDeletingRow(object? sender, DataGridViewRowCancelEventArgs e)
		{
			Debug.Assert(e.Row != null, "I think this never happens.");
			rowIndex = e.Row.Index;
		}

		void dgvImages_UserDeletedRow(object? sender, DataGridViewRowEventArgs e)
		{
			if (rowIndex >= 0)
			{
				_images.RemoveAt(rowIndex);
				rowIndex = -1;
			}
		}

		void itmOpen_Click(object? sender, EventArgs e) => OpenFirstSelectedImage();

		void itmDelete_Click(object? sender, EventArgs e) => DeleteSelectedImages();

		void itmStartViewModeSettingLeft_Click(object? sender, EventArgs e) => SetSelectedImagesViewModes(true);

		void itmStartViewModeSettingRight_Click(object? sender, EventArgs e) => SetSelectedImagesViewModes(false);
	}

	public class FileDroppedEventArgs(IEnumerable<string> fileNames, int keyState, int x, int y) : EventArgs
	{
		readonly int _keyState = keyState;

		public IEnumerable<string> FileNames { get; } = fileNames;

		public bool MouseLeft => (_keyState & 1) != 0;

		public bool MouseRight => (_keyState & 2) != 0;

		public bool MouseMiddle => (_keyState & 16) != 0;

		public bool Shift => (_keyState & 4) != 0;

		public bool Control => (_keyState & 8) != 0;

		public bool Alt => (_keyState & 32) != 0;

		public int X { get; } = x;

		public int Y { get; } = y;
	}
}
