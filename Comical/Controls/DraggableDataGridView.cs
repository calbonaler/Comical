using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
			var ev = new QueryRowDragDropEffectEventArgs(DragDropEffects.Move, set.Source);
			OnQueryRowDragDropEffect(ev);
			if (ev.Effect == DragDropEffects.None)
				return DragHitTestInfo.Nowhere;
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
			var actualDest = index;
			if (set.Source == this)
			{
				var rowIndex = set.Rows.Min(x => x.Index);
				if (index > rowIndex + set.Rows.Count)
					actualDest = index - set.Rows.Count;
				else if (index >= rowIndex)
					return DragHitTestInfo.Nowhere;
			}
			return new DragHitTestInfo(ev.Effect | DragDropEffects.Scroll, index, actualDest);
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
				(MultiDrag ? SelectedRows.Count > 0 : SelectedRows.Count == 1) &&
				(Math.Abs(_origin.Value.X - e.X) > SystemInformation.DragSize.Width / 2 || Math.Abs(_origin.Value.Y - e.Y) > SystemInformation.DragSize.Height / 2))
			{
				DoDragDrop(new DataGridViewDraggedRowSet(SelectedRows, this), DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move | DragDropEffects.Scroll);
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
			OnRowDropped(new RowDroppedEventArgs(set, info.ActualDestination));
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

		/// <summary><see cref="RowDropped"/> イベントを発生させます。</summary>
		protected virtual void OnRowDropped(RowDroppedEventArgs e) => RowDropped?.Invoke(this, e);

		/// <summary><see cref="QueryRowDragDropEffect"/> イベントを発生させます。</summary>
		protected virtual void OnQueryRowDragDropEffect(QueryRowDragDropEffectEventArgs e) => QueryRowDragDropEffect?.Invoke(this, e);

		readonly record struct DragHitTestInfo(DragDropEffects Effect, int HitIndex, int ActualDestination)
		{
			public static readonly DragHitTestInfo Nowhere = new(DragDropEffects.None, -1, -1);
		}
	}

	public class DataGridViewDraggedRowSet(DataGridViewSelectedRowCollection rows, DataGridView source)
	{
		public ReadOnlyCollection<DataGridViewRow> Rows { get; } = rows.Cast<DataGridViewRow>().OrderBy(x => x.Index).ToArray().AsReadOnly();

		public DataGridView Source { get; } = source;
	}

	public class RowDroppedEventArgs(DataGridViewDraggedRowSet rowSet, int dest) : EventArgs
	{
		public int Destination { get; } = dest;

		public DataGridViewDraggedRowSet RowSet { get; } = rowSet;

		public void SelectDroppedRows(DataGridView dataGridView)
		{
			dataGridView.ClearSelection();
			foreach (var i in Enumerable.Range(Destination, RowSet.Rows.Count))
				dataGridView.Rows[i].Selected = true;
		}
	}

	public class QueryRowDragDropEffectEventArgs(DragDropEffects effects, DataGridView source) : EventArgs
	{
		public DragDropEffects Effect { get; set; } = effects;

		public DataGridView Source { get; } = source;
	}
}
