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
		public BookmarksView() { InitializeComponent(); }

		ImageReferenceCollection _images;
		BookmarkCollection _bookmarks;

		public event EventHandler BookmarkSelected
		{
			add { dgvBookmarks.SelectionChanged += value; }
			remove { dgvBookmarks.SelectionChanged -= value; }
		}

		public event EventHandler<BookmarkNavigatedEventArgs> BookmarkNavigated;

		public IEnumerable<Bookmark> SelectedBookmarks => dgvBookmarks.SelectedRows.Cast<DataGridViewRow>().Select(row => _bookmarks[row.Index]);

		public IDisposable BeginAsyncWork()
		{
			dgvBookmarks.ReadOnly = true;
			return new DelegateDisposable(() => dgvBookmarks.ReadOnly = false);
		}

		public void SetImages(ImageReferenceCollection images)
		{
			if (_images != images)
			{
				if (_images != null)
					_images.CollectionChanged -= Images_CollectionChanged;
				_images = images;
				if (images != null)
				{
					images.CollectionChanged += Images_CollectionChanged;
					Images_CollectionChanged(images, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				}
			}
		}

		public void SetBookmarks(BookmarkCollection bookmarks)
		{
			if (_bookmarks != bookmarks)
			{
				if (_bookmarks != null)
					_bookmarks.CollectionChanged -= Bookmarks_CollectionChanged;
				_bookmarks = bookmarks;
				if (bookmarks != null)
				{
					bookmarks.CollectionChanged += Bookmarks_CollectionChanged;
					Bookmarks_CollectionChanged(bookmarks, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				}
			}
		}

		public void AddBookmarks(IEnumerable<int> targetIndexes)
		{
			if (targetIndexes == null)
				throw new ArgumentNullException(nameof(targetIndexes));
			foreach (var targetIndex in targetIndexes)
				_bookmarks.Add(new Bookmark() { Target = targetIndex });
		}

		public void DeleteSelectedBookmarks()
		{
			foreach (var bookmark in SelectedBookmarks.ToArray())
				_bookmarks.Remove(bookmark);
		}

		protected override string GetPersistString() => "BookmarkList";

		protected virtual void OnBookmarkNavigated(BookmarkNavigatedEventArgs e) { BookmarkNavigated?.Invoke(this, e); }

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

		private void dgvBookmarks_CellErrorTextNeeded(object sender, DataGridViewCellErrorTextNeededEventArgs e)
		{
			e.ErrorText = string.Empty;
			if (e.RowIndex < 0 || e.RowIndex >= _bookmarks.Count)
				return;
			if (dgvBookmarks.Columns[e.ColumnIndex] == clmTarget)
			{
				if (_bookmarks[e.RowIndex].Target < _images.Count)
					return;
				e.ErrorText = "ブックマークの対象インデックスは画像数未満である必要があります。";
			}
		}

		private void dgvBookmarks_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex >= 0 && e.RowIndex < _bookmarks.Count)
				e.Value = dgvBookmarks.Columns[e.ColumnIndex] == clmName ? _bookmarks[e.RowIndex].Name : (object)_bookmarks[e.RowIndex].Target;
		}

		private void dgvBookmarks_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex < 0 || e.RowIndex >= _bookmarks.Count)
				return;
			if (dgvBookmarks.Columns[e.ColumnIndex] == clmName)
			{
				_bookmarks[e.RowIndex].Name = e.Value.ToString();
				return;
			}
			int target;
			if (int.TryParse(e.Value.ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out target) && target >= 0)
				_bookmarks[e.RowIndex].Target = target;
		}

		private void dgvBookmarks_QueryActualDestination(object sender, Controls.QueryActualDestinationEventArgs e) { e.Effect = e.Source == dgvBookmarks ? DragDropEffects.Move : DragDropEffects.Link; }

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

		private void Images_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.InvokeIfNeeded(() =>
            {
				RefreshMenuVisibility();
				dgvBookmarks.Invalidate();
			});
		}

		private void dgvBookmarks_SelectionChanged(object sender, EventArgs e) { RefreshMenuVisibility(); }

		private void Bookmarks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.InvokeIfNeeded(() =>
			{
				dgvBookmarks.RowCount = _bookmarks.Count;
				dgvBookmarks.Invalidate();
			});
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

		private void itmRemove_Click(object sender, EventArgs e) { DeleteSelectedBookmarks(); }
	}

	public class BookmarkNavigatedEventArgs : EventArgs
	{
		public BookmarkNavigatedEventArgs(Bookmark bookmark) { Bookmark = bookmark; }

		public Bookmark Bookmark { get; }
	}
}
