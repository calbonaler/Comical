using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Comical.Controls
{
	public class DraggableDataGridView : DataGridView
	{
		bool _mouseDownOnSelectedCell = false;
		bool _allowUserToDragRows = false;
		Point? _origin;
		int _dragOverCalled = 0;
		int _hitRowIndex = -1;
		readonly Pen _insertionPen = new(Color.Black, 2.0F);

		protected override void Dispose(bool disposing)
		{
			_insertionPen.Dispose();
			base.Dispose(disposing);
		}

		int ScrollArea => FirstDisplayedScrollingRowIndex >= 0 ? Rows[FirstDisplayedScrollingRowIndex].Height / 2 : 48;

		/// <summary>ユーザーによる行のドラッグが開始するときに発生します。</summary>
		[Category("アクション")]
		[Description("ユーザーによる行のドラッグが開始するときに発生します。")]
		public event EventHandler<RowDragStartingEventArgs>? RowDragStarting;

		/// <summary>ユーザーによってドラッグされた行を受け入れるときに発生します。</summary>
		[Category("アクション")]
		[Description("ユーザーによってドラッグされた行を受け入れるときに発生します。")]
		public event EventHandler<RowDroppedEventArgs>? RowDropped;

		/// <summary>行のドラッグ・ドロップ効果を確認するときに発生します。</summary>
		[Category("アクション")]
		[Description("行のドラッグ・ドロップ効果を確認するときに発生します。")]
		public event EventHandler<QueryRowDragDropEffectEventArgs>? QueryRowDragDropEffect;

		/// <summary>ユーザーが行をドラッグできるかどうかを示す値を取得または設定します。</summary>
		[Category("動作")]
		[Description("ユーザーが行をドラッグできるかどうかを示します。")]
		[DefaultValue(false)]
		public bool AllowUserToDragRows
		{
			get => _allowUserToDragRows;
			set
			{
				_allowUserToDragRows = value;
				if (value)
					AllowDrop = true;
			}
		}

		/// <summary>複数の行をドラッグできるかどうかを示す値を取得または設定します。</summary>
		[Category("動作")]
		[Description("複数の行をドラッグできるかどうかを示します。")]
		[DefaultValue(false)]
		public bool MultiDrag { get; set; }

		public void SelectRowRange(int start, int count)
		{
			if (SelectionMode is not DataGridViewSelectionMode.FullRowSelect and not DataGridViewSelectionMode.RowHeaderSelect || !MultiSelect && count > 1)
				return;
			ClearSelection();
			foreach (var i in Enumerable.Range(start, count))
				SetSelectedRowCore(i, true);
		}

		int HitRowIndex
		{
			get => _hitRowIndex;
			set
			{
				if (_hitRowIndex != value)
				{
					if (_hitRowIndex >= 0 && _hitRowIndex < RowCount)
						InvalidateRow(_hitRowIndex);
					if (value >= 0 && value < RowCount)
						InvalidateRow(value);
					_hitRowIndex = value;
				}
			}
		}

		void AddFirstDisplayedScrollingRowIndex(int addend)
		{
			if (RowCount == 0)
				return;
			FirstDisplayedScrollingRowIndex = Math.Clamp(FirstDisplayedScrollingRowIndex + addend, 0, RowCount);
		}

		static DataGridViewDraggedRowSet? DataObjectToDraggedRowSet(IDataObject? dataObject) => dataObject != null && dataObject.GetDataPresent(typeof(DataGridViewDraggedRowSet)) ? dataObject.GetData(typeof(DataGridViewDraggedRowSet)) as DataGridViewDraggedRowSet : null;

		DragHitTestInfo DragHitTest(DataGridViewDraggedRowSet set, Point point)
		{
			var pt = PointToClient(point);
			var index = HitTest(pt.X, pt.Y).RowIndex;
			if (index < 0)
			{
				if (RowCount == 0)
					index = 0;
				else if (pt.Y > GetRowDisplayRectangle(RowCount - 1, false).Bottom)
					index = RowCount;
				else
					return DragHitTestInfo.Nowhere;
			}
			if (index < RowCount)
			{
				var rect = GetRowDisplayRectangle(index, false);
				if (pt.Y >= rect.Y + rect.Height / 2)
					index++;
			}
			var ev = new QueryRowDragDropEffectEventArgs(set, index, DragDropEffects.Move);
			OnQueryRowDragDropEffect(ev);
			return ev.Effect == DragDropEffects.None ? DragHitTestInfo.Nowhere : new DragHitTestInfo(ev.Effect | DragDropEffects.Scroll, index);
		}

		int IncrementDragOverCalled(int value)
		{
			var callMax = ScrollArea / 2;
			_dragOverCalled += value;
			var c = _dragOverCalled / callMax;
			_dragOverCalled %= callMax;
			return c;
		}

		protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
		{
			if (e != null && e.ColumnIndex >= 0 && e.RowIndex >= 0 && this[e.ColumnIndex, e.RowIndex].Selected)
			{
				_mouseDownOnSelectedCell = true;
				return;
			}
			if (e != null && e.ColumnIndex >= 0 && e.RowIndex >= 0 && e.Button == MouseButtons.Right)
				ClearSelection(e.ColumnIndex, e.RowIndex, true);
			else
				base.OnCellMouseDown(e);
		}

		protected override void OnCellMouseUp(DataGridViewCellMouseEventArgs e)
		{
			if (e != null && e.ColumnIndex >= 0 && e.RowIndex >= 0 && this[e.ColumnIndex, e.RowIndex].Selected && _mouseDownOnSelectedCell)
			{
				_mouseDownOnSelectedCell = false;
				if (e.Button == MouseButtons.Left)
				{
					ClearSelection(e.ColumnIndex, e.RowIndex, true);
					if (!IsCurrentCellDirty)
						CurrentCell = this[e.ColumnIndex, e.RowIndex];
					BeginEdit(false);
				}
			}
			base.OnCellMouseUp(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e != null)
				_origin = e.Location;
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e != null && AllowUserToDragRows && e.Button == MouseButtons.Left && _origin != null &&
				SelectionMode == DataGridViewSelectionMode.FullRowSelect &&
				(Math.Abs(_origin.Value.X - e.X) > SystemInformation.DragSize.Width / 2 || Math.Abs(_origin.Value.Y - e.Y) > SystemInformation.DragSize.Height / 2))
			{
				if (TryGetRangeIfIndicesAreContiguous(SelectedRows.Cast<DataGridViewRow>().Select(x => x.Index)) is { } range &&
					(MultiDrag ? range.Count > 0 : range.Count == 1))
				{
					var ev = new RowDragStartingEventArgs(range.Start, range.Count, DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move | DragDropEffects.Scroll);
					OnRowDragStarting(ev);
					if (ev.Items != null)
						DoDragDrop(new DataGridViewDraggedRowSet(range.Start, ev.Items), ev.AllowedEffects);
				}
				_origin = null;
			}
			base.OnMouseMove(e);
		}

		protected override void OnDragEnter(DragEventArgs drgevent)
		{
			if (DataObjectToDraggedRowSet(drgevent.Data) is { } set)
				drgevent.Effect = DragHitTest(set, new Point(drgevent.X, drgevent.Y)).Effect;
			else
				base.OnDragEnter(drgevent);
		}

		protected override void OnDragOver(DragEventArgs drgevent)
		{
			if (DataObjectToDraggedRowSet(drgevent.Data) is not { } set)
			{
				base.OnDragOver(drgevent);
				return;
			}
			var pt = PointToClient(new Point(drgevent.X, drgevent.Y));
			var diffTop = ScrollArea - pt.Y;
			var diffBottom = pt.Y - Height + ScrollArea;
			if (diffTop >= 0)
				AddFirstDisplayedScrollingRowIndex(-IncrementDragOverCalled(diffTop));
			if (diffBottom >= 0)
				AddFirstDisplayedScrollingRowIndex(IncrementDragOverCalled(diffBottom));
			var info = DragHitTest(set, new Point(drgevent.X, drgevent.Y));
			drgevent.Effect = info.Effect;
			HitRowIndex = info.HitIndex;
		}

		protected override void OnDragLeave(EventArgs e)
		{
			HitRowIndex = -1;
			base.OnDragLeave(e);
		}

		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			if (HitRowIndex < 0 || DataObjectToDraggedRowSet(drgevent.Data) is not { } set)
			{
				base.OnDragDrop(drgevent);
				return;
			}
			var info = DragHitTest(set, new Point(drgevent.X, drgevent.Y));
			drgevent.Effect = info.Effect;
			if (info.Effect == DragDropEffects.None)
				return;
			OnRowDropped(new RowDroppedEventArgs(set, info.HitIndex));
			HitRowIndex = -1;
		}

		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs qcdevent)
		{
			if ((qcdevent.KeyState & 2) > 0 || (qcdevent.KeyState & 16) > 0)
				qcdevent.Action = DragAction.Cancel;
			base.OnQueryContinueDrag(qcdevent);
		}

		protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
		{
			if (e != null && e.RowIndex == HitRowIndex)
				e.Graphics.DrawLine(_insertionPen, e.RowBounds.Left, e.RowBounds.Top + 1, e.RowBounds.Right, e.RowBounds.Top + 1);
			base.OnRowPostPaint(e);
		}

		/// <summary><see cref="RowDragStarting"/> イベントを発生させます。</summary>
		protected virtual void OnRowDragStarting(RowDragStartingEventArgs e) => RowDragStarting?.Invoke(this, e);

		/// <summary><see cref="RowDropped"/> イベントを発生させます。</summary>
		protected virtual void OnRowDropped(RowDroppedEventArgs e) => RowDropped?.Invoke(this, e);

		/// <summary><see cref="QueryRowDragDropEffect"/> イベントを発生させます。</summary>
		protected virtual void OnQueryRowDragDropEffect(QueryRowDragDropEffectEventArgs e) => QueryRowDragDropEffect?.Invoke(this, e);

		static (int Start, int Count)? TryGetRangeIfIndicesAreContiguous(IEnumerable<int> indices)
		{
			var inclusiveMin = int.MaxValue;
			var exclusiveMax = 0;
			var count = 0;
			foreach (var i in indices)
			{
				inclusiveMin = Math.Clamp(i, 0, inclusiveMin);
				exclusiveMax = Math.Max(exclusiveMax, i + 1);
				count++;
			}
			return count == 0 ? (0, count) : count == exclusiveMax - inclusiveMin ? new(inclusiveMin, count) : null;
		}

		readonly record struct DragHitTestInfo(DragDropEffects Effect, int HitIndex)
		{
			public static readonly DragHitTestInfo Nowhere = new(DragDropEffects.None, -1);
		}
	}

	public class DataGridViewDraggedRowSet(int startIndex, IReadOnlyList<object> items)
	{
		public int StartIndex { get; } = startIndex;

		public int EndIndex => StartIndex + Items.Count;

		public IReadOnlyList<object> Items { get; } = items;
	}

	public class RowDragStartingEventArgs(int startIndex, int count, DragDropEffects allowedEffects) : EventArgs
	{
		public int StartIndex { get; } = startIndex;

		public int Count { get; } = count;

		public DragDropEffects AllowedEffects { get; set; } = allowedEffects;

		public IReadOnlyList<object>? Items { get; private set; }

		public void SetItems(IEnumerable items)
		{
			Items = items.Cast<object>().ToArray();
			if (Items.Count != Count)
				throw new ArgumentException($"Must have same number of items as {nameof(Count)}", nameof(items));
		}
	}

	public class RowDroppedEventArgs(DataGridViewDraggedRowSet rows, int index) : EventArgs
	{
		public DataGridViewDraggedRowSet RowSet { get; } = rows;

		public int Index { get; } = index;

		public int MoveInListIndex => Index - (Index > RowSet.EndIndex ? RowSet.Items.Count : 0);
	}

	public class QueryRowDragDropEffectEventArgs(DataGridViewDraggedRowSet set, int index, DragDropEffects effects) : EventArgs
	{
		public DataGridViewDraggedRowSet RowSet { get; } = set;

		public int Index { get; } = index;

		public bool MovesIntoMovingRows => Index >= RowSet.StartIndex && Index <= RowSet.EndIndex;

		public DragDropEffects Effect { get; set; } = effects;
	}
}
