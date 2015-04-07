using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Forms;
using Comical.Core;

namespace Comical
{
	public partial class BookmarksView : WeifenLuo.WinFormsUI.Docking.DockContent
	{
		public BookmarksView()
		{
			InitializeComponent();
			dgvBookmarks.Font = new System.Drawing.Font("Meiryo", 9);
		}

		ImageReferenceCollection _images;
		BookmarkCollection _bookmarks;

		public event EventHandler BookmarkSelected
		{
			add { dgvBookmarks.SelectionChanged += value; }
			remove { dgvBookmarks.SelectionChanged -= value; }
		}

		public event EventHandler<BookmarkNavigatedEventArgs> BookmarkNavigated;

		public IEnumerable<Bookmark> SelectedBookmarks { get { return dgvBookmarks.SelectedRows.Cast<DataGridViewRow>().Select(row => _bookmarks[row.Index]); } }

		public bool ReadOnly
		{
			get { return dgvBookmarks.ReadOnly; }
			set { dgvBookmarks.ReadOnly = value; }
		}

		public void SetImages(ImageReferenceCollection images)
		{
			if (_images != images)
			{
				if (_images != null)
					_images.CollectionChanged -= Images_CollectionChanged;
				this._images = images;
				if (images != null)
				{
					images.CollectionChanged += Images_CollectionChanged;
					Images_CollectionChanged(images, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				}
			}
		}

		public void SetBookmarks(BookmarkCollection bookmarks)
		{
			if (this._bookmarks != bookmarks)
			{
				if (this._bookmarks != null)
					this._bookmarks.CollectionChanged -= Bookmarks_CollectionChanged;
				this._bookmarks = bookmarks;
				if (bookmarks != null)
				{
					bookmarks.CollectionChanged += Bookmarks_CollectionChanged;
					Bookmarks_CollectionChanged(bookmarks, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				}
			}
		}

		protected override string GetPersistString() { return "BookmarkList"; }

		protected virtual void OnBookmarkNavigated(BookmarkNavigatedEventArgs e)
		{
			if (BookmarkNavigated != null)
				BookmarkNavigated(this, e);
		}

		private void RefreshMenuVisibility()
		{
			var count = SelectedBookmarks.Count();
			itmSelectTarget.Visible = count == 1;
			sepBookmark1.Visible = count == 1;
			itmAdd.Visible = _images.Count > 0;
			itmInsertAbove.Visible = count == 1;
			itmInsertBelow.Visible = count == 1;
			sepBookmark2.Visible = count > 0;
			itmRemove.Visible = count > 0;
		}

		private void dgvBookmarks_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			if (e.RowIndex < 0 || e.RowIndex >= _bookmarks.Count)
				return;
			if (string.IsNullOrEmpty(e.FormattedValue.ToString()))
				e.Cancel = true;
			else if (dgvBookmarks.Columns[e.ColumnIndex].Name == "clmTarget")
			{
				int ui = 0;
				if (int.TryParse(e.FormattedValue.ToString(), out ui) && ui >= 0 && ui < _images.Count)
					return;
				e.Cancel = true;
			}
		}

		private void dgvBookmarks_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex >= 0 && e.RowIndex < _bookmarks.Count)
				e.Value = dgvBookmarks.Columns[e.ColumnIndex].Name == "clmName" ? (object)_bookmarks[e.RowIndex].Name : (object)_bookmarks[e.RowIndex].Target;
		}

		private void dgvBookmarks_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex >= 0 && e.RowIndex < _bookmarks.Count)
			{
				Bookmark it = _bookmarks[e.RowIndex];
				if (dgvBookmarks.Columns[e.ColumnIndex].Name == "clmName")
					it.Name = e.Value.ToString();
				else
					it.Target = int.Parse(e.Value.ToString(), System.Globalization.CultureInfo.CurrentCulture);
				_bookmarks[e.RowIndex] = it;
			}
		}

		private void dgvBookmarks_QueryActualDestination(object sender, Controls.QueryActualDestinationEventArgs e)
		{
			if (e.Source == dgvBookmarks)
				e.Effect = DragDropEffects.Move;
			else
				e.Effect = DragDropEffects.Link;
			if (e.Source.Name == "dgvTrashBox")
				e.ActualDestination = -1;
		}

		private void dgvBookmarks_RowMoving(object sender, Controls.RowMovingEventArgs e)
		{
			if (e.Source.Name == "dgvImages")
			{
				e.Cancel = true;
				_bookmarks.Insert(e.Destination, new Bookmark() { Target = e.SourceRows[0].Index });
			}
			else if (e.Source == dgvBookmarks)
				_bookmarks.Move(e.SourceRows[0].Index, e.Destination);
		}

		private void Images_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) { RefreshMenuVisibility(); }

		private void dgvBookmarks_SelectionChanged(object sender, EventArgs e) { RefreshMenuVisibility(); }

		private void Bookmarks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			dgvBookmarks.RowCount = _bookmarks.Count;
			dgvBookmarks.Invalidate();
		}

		private void dgvBookmarks_CellDoubleClick(object sender, DataGridViewCellEventArgs e) { OnBookmarkNavigated(new BookmarkNavigatedEventArgs(_bookmarks[e.RowIndex])); }

		int rowIndex = -1;

		private void dgvBookmarks_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) { rowIndex = e.Row.Index; }

		private void dgvBookmarks_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
		{
			if (rowIndex >= 0 && rowIndex < _bookmarks.Count)
				_bookmarks.RemoveAt(rowIndex);
		}

		private void itmSelectTarget_Click(object sender, EventArgs e) { OnBookmarkNavigated(new BookmarkNavigatedEventArgs(_bookmarks[dgvBookmarks.SelectedRows[0].Index])); }

		private void itmAdd_Click(object sender, EventArgs e) { _bookmarks.Add(new Bookmark()); }

		private void itmInsertAbove_Click(object sender, EventArgs e) { _bookmarks.Insert(dgvBookmarks.SelectedRows[0].Index, new Bookmark()); }

		private void itmInsertBelow_Click(object sender, EventArgs e) { _bookmarks.Insert(dgvBookmarks.SelectedRows[0].Index + 1, new Bookmark()); }

		private void itmRemove_Click(object sender, EventArgs e)
		{
			foreach (var bookmark in SelectedBookmarks.ToArray())
				_bookmarks.Remove(bookmark);
		}
	}

	public class BookmarkNavigatedEventArgs : EventArgs
	{
		public BookmarkNavigatedEventArgs(Bookmark bookmark) { this.Bookmark = bookmark; }

		public Bookmark Bookmark { get; private set; }
	}
}
