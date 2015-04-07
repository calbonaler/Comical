﻿using System;
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
			dgvImages.Font = new System.Drawing.Font("Meiryo", 9);
		}

		Comic _comic;
		static readonly Size ThumbnailSize = new Size(118, 118);

		Viewer DefaultViewer { get { return DockPanel != null ? DockPanel.Contents.OfType<Viewer>().FirstOrDefault(v => v.Pane.IsActiveDocumentPane) : null; } }

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

		public event EventHandler<FileDroppedEventArgs> FileDropped;

		public bool ReadOnly
		{
			get { return dgvImages.ReadOnly; }
			set { dgvImages.ReadOnly = value; }
		}

		public IEnumerable<int> SelectedIndicies { get { return dgvImages.SelectedRows.Cast<DataGridViewRow>().Select(r => r.Index); } }

		public void SetComic(Comic comic)
		{
			if (this._comic != comic)
			{
				if (this._comic != null)
				{
					this._comic.Images.CollectionChanged -= Images_CollectionChanged;
					this._comic.Images.CollectionItemPropertyChanged -= Images_CollectionItemPropertyChanged;
					this._comic.PropertyChanged -= Comic_PropertyChanged;
				}
				this._comic = comic;
				if (comic != null)
				{
					comic.Images.CollectionChanged += Images_CollectionChanged;
					comic.Images.CollectionItemPropertyChanged += Images_CollectionItemPropertyChanged;
					comic.PropertyChanged += Comic_PropertyChanged;
					Images_CollectionChanged(comic.Images, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				}
			}
		}

		private void Comic_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsBusy")
				dgvImages.Refresh();
		}

		public void SelectAll() { dgvImages.SelectAll(); }

		public void InvertSelections()
		{
			foreach (DataGridViewRow row in dgvImages.Rows)
				row.Selected = !row.Selected;
		}

		void Images_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			dgvImages.RowCount = _comic.Images.Count;
			if (_comic.Images.Count == 0 && DefaultViewer != null)
				DefaultViewer.Image = null;
			dgvImages.Invalidate();
		}

		void Images_CollectionItemPropertyChanged(object sender, CompositePropertyChangedEventArgs<ImageReference> e)
		{
			foreach (var group in e.PropertyNames)
			{
				int index = _comic.Images.IndexOf(group.Key);
				if (index >= 0)
					dgvImages.UpdateCellValue(1, index);
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			dgvImages.RowTemplate.Height = ThumbnailSize.Height;
			dgvImages.Columns["clmImage"].Width = ThumbnailSize.Width;
			(dgvImages.Columns["clmViewMode"] as DataGridViewComboBoxColumn).DataSource = Enum.GetNames(typeof(ImageViewMode));
		}

		protected override string GetPersistString() { return "ImageList"; }

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
				for (int i = 0; i < _comic.Images.Count; i++)
					dgvImages.Rows[i].Selected = i == value;
				if (value >= 0 && value < _comic.Images.Count)
					dgvImages.FirstDisplayedScrollingRowIndex = value;
			}
		}

		public void OpenFirstSelectedImage()
		{
			if (FirstSelectedRowIndex >= 0)
			{
				Viewer viewer = new Viewer();
				viewer.Text = FirstSelectedRowIndex.ToString(_comic.Images.Count);
				viewer.Image = _comic.Images[FirstSelectedRowIndex].GetImage();
				viewer.Show(DockPanel);
			}
		}

		public void DeleteSelectedImages()
		{
			foreach (var x in SelectedIndicies.OrderBy(x => x).Select(x => _comic.Images[x]).ToArray())
				_comic.Images.Remove(x);
		}

		public void BookmarkSelectedImages()
		{
			foreach (var index in SelectedIndicies.OrderBy(x => x))
				_comic.Bookmarks.Insert(_comic.Bookmarks.Count, new Bookmark("ページ", index));
		}

		private void dgvImages_SelectionChanged(object sender, System.EventArgs e)
		{
			int count = dgvImages.SelectedRows.Count;
			if (DefaultViewer != null)
			{
				DefaultViewer.Description = count < 1 ? Properties.Resources.ViewerDescription_NoSelection :
					string.Format(CultureInfo.CurrentCulture, Properties.Resources.ViewerDescription_Selection, count);
				if (count == 1)
				{
					DefaultViewer.Text = FirstSelectedRowIndex.ToString(_comic.Images.Count);
					try { DefaultViewer.Image = _comic.Images[FirstSelectedRowIndex].GetImage(); }
					catch (ArgumentException) { }
				}
			}
			itmOpen.Visible = count == 1;
			itmAddToBookmark.Visible = itmDelete.Visible = itmExport.Visible = itmExtract.Visible = sepImage1.Visible = sepImage2.Visible = count > 0;
		}

		private void dgvImages_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
				e.Effect = DragDropEffects.Copy;
		}

		private void dgvImages_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop) && FileDropped != null)
				FileDropped(this, new FileDroppedEventArgs(e.Data.GetData(DataFormats.FileDrop) as string[], e.KeyState, e.X, e.Y));
		}

		private void dgvImages_RowMoving(object sender, Controls.RowMovingEventArgs e)
		{
			if (e.Source == dgvImages)
				_comic.Images.MoveRange(e.SourceRows[0].Index, e.SourceRows.Count, e.Destination);
		}

		private void dgvImages_QueryActualDestination(object sender, Controls.QueryActualDestinationEventArgs e)
		{
			e.Effect = DragDropEffects.Move;
			if (e.Source.Name == "dgvBookmarks")
				e.ActualDestination = -1;
		}

		private void dgvImages_CellDoubleClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e) { OpenFirstSelectedImage(); }

		private void dgvImages_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex >= 0 && e.RowIndex < _comic.Images.Count)
			{
				switch (dgvImages.Columns[e.ColumnIndex].Name)
				{
					case "clmViewMode":
						e.Value = _comic.Images[e.RowIndex].ViewMode.ToString();
						break;
					case "clmImage":
						e.Value = _comic.IsBusy ? null : _comic.Images[e.RowIndex].GetImage(ThumbnailSize);
						break;
				}
			}
		}

		private void dgvImages_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex >= 0 && e.RowIndex < _comic.Images.Count && dgvImages.Columns[e.ColumnIndex].Name == "clmViewMode" && e.Value != null)
			{
				ImageViewMode mode;
				if (Enum.TryParse(e.Value.ToString(), out mode))
					_comic.Images[e.RowIndex].ViewMode = mode;
			}
		}

		int rowIndex = -1;

		private void dgvImages_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) { rowIndex = e.Row.Index; }

		private void dgvImages_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
		{
			if (rowIndex >= 0)
			{
				_comic.Images.RemoveAt(rowIndex);
				rowIndex = -1;
			}
		}

		private void itmOpen_Click(object sender, EventArgs e) { OpenFirstSelectedImage(); }

		private void itmDelete_Click(object sender, EventArgs e) { DeleteSelectedImages(); }

		private void itmBookmark_Click(object sender, EventArgs e) { BookmarkSelectedImages(); }
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

		public IEnumerable<string> FileNames { get; private set; }

		public bool MouseLeft { get { return (_keyState & 1) != 0; } }

		public bool MouseRight { get { return (_keyState & 2) != 0; } }

		public bool MouseMiddle { get { return (_keyState & 16) != 0; } }

		public bool Shift { get { return (_keyState & 4) != 0; } }

		public bool Control { get { return (_keyState & 8) != 0; } }

		public bool Alt { get { return (_keyState & 32) != 0; } }

		public int X { get; private set; }

		public int Y { get; private set; }
	}
}
