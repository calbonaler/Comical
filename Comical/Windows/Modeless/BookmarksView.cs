using Comical.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Comical
{
	public partial class BookmarksView : WeifenLuo.WinFormsUI.Docking.DockContent
	{
		public BookmarksView(ImageReferenceCollection images, BookmarkCollection bookmarks)
		{
			InitializeComponent();

			_images = images;
			_images.CollectionChanged += OnImagesCollectionChanged;
			OnImagesCollectionChanged(_images, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

			_bookmarks = bookmarks;
			_bookmarks.CollectionChanged += OnBookmarksCollectionChanged;
			OnBookmarksCollectionChanged(_bookmarks, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		readonly ImageReferenceCollection _images;
		readonly BookmarkCollection _bookmarks;

		public event EventHandler BookmarkSelected
		{
			add => BookmarksDataGridView.SelectionChanged += value;
			remove => BookmarksDataGridView.SelectionChanged -= value;
		}

		public event EventHandler<BookmarkNavigatedEventArgs>? BookmarkNavigated;

		public IEnumerable<int> SelectedIndices => BookmarksDataGridView.SelectedRows.Cast<DataGridViewRow>().Select(row => row.Index);

		public IDisposable BeginAsyncWork()
		{
			BookmarksDataGridView.ReadOnly = true;
			return new DelegateDisposable(() => BookmarksDataGridView.ReadOnly = false);
		}

		public void DeleteSelectedBookmarks()
		{
			foreach (var index in SelectedIndices.OrderByDescending(x => x).ToArray())
				_bookmarks.RemoveAt(index);
		}

		protected override string GetPersistString() => "BookmarkList";

		protected virtual void OnBookmarkNavigated(BookmarkNavigatedEventArgs e) => BookmarkNavigated?.Invoke(this, e);

		void RefreshMenuVisibility()
		{
			var count = SelectedIndices.Count();
			SelectTargetMenuItem.Visible = count == 1;
			BookmarkMenuSeparator1.Visible = count == 1;
			CreateNewMenuItem.Visible = _images.Count > 0;
			InsertAboveMenuItem.Visible = count == 1;
			InsertBelowMenuItem.Visible = count == 1;
			BookmarkMenuSeparator2.Visible = count > 0;
			DeleteMenuItem.Visible = count > 0;
		}

		void OnBookmarksDataGridViewCellErrorTextNeeded(object? sender, DataGridViewCellErrorTextNeededEventArgs e)
		{
			e.ErrorText = string.Empty;
			if (e.RowIndex < 0 || e.RowIndex >= _bookmarks.Count)
				return;
			if (BookmarksDataGridView.Columns[e.ColumnIndex] == TargetDataGridViewColumn)
			{
				if (_bookmarks[e.RowIndex].Target < _images.Count)
					return;
				e.ErrorText = Properties.Resources.InvalidBookmarkIndex;
			}
		}

		void OnBookmarksDataGridViewCellValueNeeded(object? sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex >= 0 && e.RowIndex < _bookmarks.Count)
				e.Value = BookmarksDataGridView.Columns[e.ColumnIndex] == NameDataGridViewColumn ? _bookmarks[e.RowIndex].Name : _bookmarks[e.RowIndex].Target;
		}

		void OnBookmarksDataGridViewCellValuePushed(object? sender, DataGridViewCellValueEventArgs e)
		{
			if (e.RowIndex < 0 || e.RowIndex >= _bookmarks.Count || e.Value is not string value)
				return;
			if (BookmarksDataGridView.Columns[e.ColumnIndex] == NameDataGridViewColumn)
			{
				_bookmarks[e.RowIndex].Name = value;
				return;
			}
			if (int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.CurrentCulture, out var target) && target >= 0)
				_bookmarks[e.RowIndex].Target = target;
		}

		void OnBookmarksDataGridViewQueryRowDragDropEffect(object? sender, Controls.QueryRowDragDropEffectEventArgs e) => e.Effect = e.Source == BookmarksDataGridView ? DragDropEffects.Move : DragDropEffects.Link;

		void OnBookmarksDataGridViewRowMoving(object? sender, Controls.RowMovingEventArgs e)
		{
			if (e.Source == BookmarksDataGridView)
				_bookmarks.Move(e.SourceRows[0].Index, e.Destination);
			else
			{
				e.Cancel = true;
				_bookmarks.Insert(e.Destination, new Bookmark() { Target = e.SourceRows[0].Index });
			}
		}

		void OnImagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => this.InvokeIfNeeded(() =>
		{
			RefreshMenuVisibility();
			BookmarksDataGridView.Invalidate();
		});

		void OnBookmarksDataGridViewSelectionChanged(object? sender, EventArgs e) => RefreshMenuVisibility();

		void OnBookmarksCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => this.InvokeIfNeeded(() =>
		{
			BookmarksDataGridView.RowCount = _bookmarks.Count;
			BookmarksDataGridView.Invalidate();
		});

		void OnBookmarksDataGridViewCellDoubleClick(object? sender, DataGridViewCellEventArgs e) => OnBookmarkNavigated(new BookmarkNavigatedEventArgs(_bookmarks[e.RowIndex]));

		int rowIndex = -1;

		void OnBookmarksDataGridViewUserDeletingRow(object? sender, DataGridViewRowCancelEventArgs e)
		{
			Debug.Assert(e.Row != null, "I think this never happens.");
			rowIndex = e.Row.Index;
		}

		void OnBookmarksDataGridViewUserDeletedRow(object? sender, DataGridViewRowEventArgs e)
		{
			if (rowIndex >= 0 && rowIndex < _bookmarks.Count)
				_bookmarks.RemoveAt(rowIndex);
		}

		void OnSelectTargetMenuItemClick(object? sender, EventArgs e) => OnBookmarkNavigated(new BookmarkNavigatedEventArgs(_bookmarks[BookmarksDataGridView.SelectedRows[0].Index]));

		void OnCreateNewMenuItemClick(object? sender, EventArgs e) => _bookmarks.Add(new Bookmark());

		void OnInsertAboveMenuItemClick(object? sender, EventArgs e) => _bookmarks.Insert(BookmarksDataGridView.SelectedRows[0].Index, new Bookmark());

		void OnInsertBelowMenuItemClick(object? sender, EventArgs e) => _bookmarks.Insert(BookmarksDataGridView.SelectedRows[0].Index + 1, new Bookmark());

		void OnDeleteMenuItemClick(object? sender, EventArgs e) => DeleteSelectedBookmarks();
	}

	public class BookmarkNavigatedEventArgs(Bookmark bookmark) : EventArgs
	{
		public Bookmark Bookmark { get; } = bookmark;
	}
}
