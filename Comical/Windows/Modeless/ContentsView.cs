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
			ImagesDataGridView.RowTemplate.Height = ThumbnailSize.Height;
			ViewModeDataGridViewColumn.DataSource = Enum.GetNames(typeof(ImageViewMode));

			_images = images;
			_images.CollectionChanged += OnImagesCollectionChanged;
			_images.CollectionItemPropertyChanged += OnImagesCollectionItemPropertyChanged;
			OnImagesCollectionChanged(_images, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		readonly ImageReferenceCollection _images;
		readonly ThumbnailCache _thumbnailCache = new();
		static readonly Size ThumbnailSize = new(118, 118);

		IEnumerable<Viewer> Viewers => DockPanel?.Contents?.OfType<Viewer>() ?? [];

		Viewer? ActiveViewer => DockPanel?.ActiveDocument as Viewer;

		public event EventHandler ImageReferenceSelected
		{
			add => ImagesDataGridView.SelectionChanged += value;
			remove => ImagesDataGridView.SelectionChanged -= value;
		}

		public event EventHandler ExportRequested
		{
			add => ExportMenuItem.Click += value;
			remove => ExportMenuItem.Click -= value;
		}

		public event EventHandler ExtractRequested
		{
			add => ExtractMenuItem.Click += value;
			remove => ExportMenuItem.Click -= value;
		}

		public event EventHandler BookmarkRequested
		{
			add => AddToBookmarkMenuItem.Click += value;
			remove => AddToBookmarkMenuItem.Click -= value;
		}

		public event EventHandler<FileDroppedEventArgs>? FileDropped;

		public IDisposable BeginAsyncWork()
		{
			ImagesDataGridView.ReadOnly = true;
			ImagesDataGridView.Refresh();
			return new DelegateDisposable(() =>
			{
				ImagesDataGridView.ReadOnly = false;
				ImagesDataGridView.Refresh();
			});
		}

		public IEnumerable<int> SelectedIndices => ImagesDataGridView.SelectedRows.Cast<DataGridViewRow>().Select(r => r.Index);

		void OnImagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => this.InvokeIfNeeded(() =>
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Remove:
					Debug.Assert(e.OldItems is not null);
					foreach (ImageReference item in e.OldItems)
						_thumbnailCache.Remove(item.Data);
					break;
				case NotifyCollectionChangedAction.Reset:
					_thumbnailCache.Clear();
					break;
			}
			ImagesDataGridView.RowCount = _images.Count;
			if (e.Action == NotifyCollectionChangedAction.Reset)
			{
				foreach (var viewer in Viewers.ToArray())
					viewer.Close();
			}
			ImagesDataGridView.Invalidate();
		});

		void OnImagesCollectionItemPropertyChanged(object? sender, CollectionItemPropertyChangedEventArgs e) => this.InvokeIfNeeded(() =>
		{
			Debug.Assert(e.Item != null, "sender of ImageReference's PropertyChanged event must not be null");
			var index = _images.IndexOf((ImageReference)e.Item);
			if (index >= 0)
				ImagesDataGridView.UpdateCellValue(1, index);
		});

		protected override string GetPersistString() => "ImageList";

		public void SelectSingleImage(int index)
		{
			for (var i = 0; i < _images.Count; i++)
				ImagesDataGridView.Rows[i].Selected = i == index;
			if (index >= 0 && index < _images.Count)
				ImagesDataGridView.FirstDisplayedScrollingRowIndex = index;
		}

		public void OpenFirstSelectedImage()
		{
			if (ImagesDataGridView.SelectedRows.Count > 0)
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

		void OnImagesDataGridViewSelectionChanged(object? sender, EventArgs e)
		{
			var count = ImagesDataGridView.SelectedRows.Count;
			if (ActiveViewer != null && count == 1)
			{
				var firstSelectedIndex = SelectedIndices.Min();
				ActiveViewer.Text = firstSelectedIndex.ToString(CultureInfo.CurrentCulture);
				try { ActiveViewer.Image = _images[firstSelectedIndex].Data; }
				catch (ArgumentException) { }
			}
			OpenMenuItem.Visible = ImageMenuSeparator1.Visible = count == 1;
			AddToBookmarkMenuItem.Visible = ImageMenuSeparator2.Visible =
				ExportMenuItem.Visible = ExtractMenuItem.Visible = ImageMenuSeparator3.Visible =
				StartViewModeSettingLeftMenuItem.Visible = StartViewModeSettingRightMenuItem.Visible = ImageMenuSeparator4.Visible =
				DeleteMenuItem.Visible = count > 0;
		}

		void OnImagesDataGridViewDragEnter(object? sender, DragEventArgs e)
		{
			Debug.Assert(e.Data != null, "I think this never happens.");
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		void OnImagesDataGridViewDragDrop(object? sender, DragEventArgs e)
		{
			Debug.Assert(e.Data != null, "I think this never happens.");
			if (e.Data.GetDataPresent(DataFormats.FileDrop) && FileDropped != null)
			{
				var fileNames = (string[]?)e.Data.GetData(DataFormats.FileDrop);
				Debug.Assert(fileNames != null, "I think this never happens.");
				FileDropped(this, new FileDroppedEventArgs(fileNames, e.KeyState, e.X, e.Y));
			}
		}

		void OnImagesDataGridViewRowMoving(object? sender, Controls.RowMovingEventArgs e)
		{
			if (e.Source == ImagesDataGridView)
				_images.MoveRange(e.SourceRows[0].Index, e.SourceRows.Count, e.Destination);
		}

		void OnImagesDataGridViewQueryRowDragDropEffect(object? sender, Controls.QueryRowDragDropEffectEventArgs e) => e.Effect = e.Source == ImagesDataGridView ? DragDropEffects.Move : DragDropEffects.None;

		void OnImagesDataGridViewCellDoubleClick(object? sender, DataGridViewCellEventArgs e) => OpenFirstSelectedImage();

		void OnImagesDataGridViewCellValueNeeded(object? sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex < 0 || e.RowIndex >= _images.Count)
				return;
			if (ImagesDataGridView.Columns[e.ColumnIndex] == ViewModeDataGridViewColumn)
				e.Value = _images[e.RowIndex].ViewMode.ToString();
			else if (ImagesDataGridView.Columns[e.ColumnIndex] == ImageDataGridViewColumn)
				e.Value = _thumbnailCache.Get(_images[e.RowIndex].Data);
		}

		void OnImagesDataGridViewCellValuePushed(object? sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex >= 0 && e.RowIndex < _images.Count && ImagesDataGridView.Columns[e.ColumnIndex] == ViewModeDataGridViewColumn && e.Value != null && Enum.TryParse(e.Value.ToString(), out ImageViewMode mode))
				_images[e.RowIndex].ViewMode = mode;
		}

		int rowIndex = -1;

		void OnImagesDataGridViewUserDeletingRow(object? sender, DataGridViewRowCancelEventArgs e)
		{
			Debug.Assert(e.Row != null, "I think this never happens.");
			rowIndex = e.Row.Index;
		}

		void OnImagesDataGridViewUserDeletedRow(object? sender, DataGridViewRowEventArgs e)
		{
			if (rowIndex >= 0)
			{
				_images.RemoveAt(rowIndex);
				rowIndex = -1;
			}
		}

		void OnOpenMenuItemClick(object? sender, EventArgs e) => OpenFirstSelectedImage();

		void OnDeleteMenuItemClick(object? sender, EventArgs e) => DeleteSelectedImages();

		void OnStartViewModeSettingLeftMenuItemClick(object? sender, EventArgs e) => SetSelectedImagesViewModes(true);

		void OnStartViewModeSettingRightMenuItemClick(object? sender, EventArgs e) => SetSelectedImagesViewModes(false);

		class ThumbnailCache : IDisposable
		{
			readonly Dictionary<Binary, Image> _cache = [];

			public Image Get(Binary binary)
			{
				if (!_cache.TryGetValue(binary, out var image))
				{
					using var tmpImage = binary.ToImage();
					_cache.Add(binary, image = new Bitmap(tmpImage, Utils.ScaleSize(tmpImage.Size, ThumbnailSize)));
				}
				return image;
			}

			public void Remove(Binary binary)
			{
				if (_cache.Remove(binary, out var image))
					image.Dispose();
			}

			public void Clear()
			{
				foreach (var (_, value) in _cache)
					value.Dispose();
				_cache.Clear();
			}

			public void Dispose() => Clear();
		}
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
