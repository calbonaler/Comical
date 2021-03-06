﻿using System;
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
		bool mouseDownOnSelectedCell = false;
		bool _allowUserToMoveRows = false;
		Point origin;
		int dragOverCalled = 0;
		int _hitRowIndex = -1;
		Pen insertionPen = new Pen(Color.Black, 2.0F);

		protected override void Dispose(bool disposing)
		{
			if (insertionPen != null)
			{
				insertionPen.Dispose();
				insertionPen = null;
			}
			base.Dispose(disposing);
		}

		int ScrollArea => FirstDisplayedScrollingRowIndex >= 0 ? Rows[FirstDisplayedScrollingRowIndex].Height / 2 : 48;

		/// <summary>ユーザーによってドラッグされた行を受け入れる直前に発生します。</summary>
		[Category("アクション")]
		[Description("ユーザーによってドラッグされた行を受け入れる直前に発生します。")]
		public event EventHandler<RowMovingEventArgs> RowMoving;

		/// <summary>ユーザーによってドラッグされた行がドロップされたときに発生します。</summary>
		[Category("アクション")]
		[Description("ユーザーによってドラッグされた行がドロップされたときに発生します。")]
		public event EventHandler RowMoved;

		/// <summary>行が実際に挿入される位置を確認するときに発生します。</summary>
		[Category("アクション")]
		[Description("行が実際に挿入される位置を確認するときに発生します。")]
		public event EventHandler<QueryActualDestinationEventArgs> QueryActualDestination;

		/// <summary>選択された項目の個数を取得します。</summary>
		protected int SelectedItemCount => SelectionMode == DataGridViewSelectionMode.FullColumnSelect ? SelectedColumns.Count :
			(SelectionMode == DataGridViewSelectionMode.FullRowSelect ? SelectedRows.Count : SelectedCells.Count);

		/// <summary>ユーザーが行をドラッグで移動できるかどうかを示す値を取得または設定します。</summary>
		[Category("動作")]
		[Description("ユーザーが行をドラッグで移動できるかどうかを示します。")]
		[DefaultValue(false)]
		public bool AllowUserToMoveRows
		{
			get { return _allowUserToMoveRows; }
			set
			{
				_allowUserToMoveRows = value;
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
			get { return _hitRowIndex; }
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

		int FirstDisplayedScrollingRowIndexUnchecked
		{
			get { return FirstDisplayedScrollingRowIndex; }
			set
			{
				if (RowCount == 0)
					return;
				if (value < 0)
					value = 0;
				else if (value >= RowCount)
					value = RowCount - 1;
				FirstDisplayedScrollingRowIndex = value;
			}
		}

		DragHitTestInfo GetDragHitTestInfo(DragEventArgs de)
		{
			Point pt = PointToClient(new Point(de.X, de.Y));
			int index = HitTest(pt.X, pt.Y).RowIndex;
			if (index < 0)
			{
				if (RowCount == 0)
					index = 0;
				else if (pt.Y > GetRowDisplayRectangle(RowCount - 1, false).Bottom)
					index = RowCount;
			}
			var obj = de.Data.GetData(typeof(DataGridViewMovedRows)) as DataGridViewMovedRows;
			int actualdest = index;
			if (obj.Source == this)
			{
				var rowIndex = obj.SourceRows.Aggregate(int.MaxValue, (work, next) => work > next.Index ? next.Index : work);
				if (index > rowIndex + obj.SourceRows.Count)
					actualdest = index - obj.SourceRows.Count;
				else if (index >= rowIndex)
					actualdest = -1;
			}
			QueryActualDestinationEventArgs ev = new QueryActualDestinationEventArgs(actualdest, DragDropEffects.Move, obj.Source);
			OnQueryActualDestination(ev);
			if (ev.ActualDestination < 0)
				de.Effect = DragDropEffects.None;
			else
				de.Effect = ev.Effect;
			return new DragHitTestInfo(index, ev.ActualDestination);
		}

		int IncrementDragOverCalled(int value)
		{
			int callMax = ScrollArea / 2;
			dragOverCalled += value;
			int c = dragOverCalled / callMax;
			dragOverCalled = dragOverCalled % callMax;
			return c;
		}

		protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
		{
			if (e != null && e.ColumnIndex >= 0 && e.RowIndex >= 0 && this[e.ColumnIndex, e.RowIndex].Selected)
			{
				mouseDownOnSelectedCell = true;
				return;
			}
			if (e != null && e.ColumnIndex >= 0 && e.RowIndex >= 0 && e.Button == MouseButtons.Right)
				ClearSelection(e.ColumnIndex, e.RowIndex, true);
			else
				base.OnCellMouseDown(e);
		}

		protected override void OnCellMouseUp(DataGridViewCellMouseEventArgs e)
		{
			if (e != null && e.ColumnIndex >= 0 && e.RowIndex >= 0 && this[e.ColumnIndex, e.RowIndex].Selected && mouseDownOnSelectedCell)
			{
				mouseDownOnSelectedCell = false;
				if (e.Button == MouseButtons.Left)
				{
					ClearSelection(e.ColumnIndex, e.RowIndex, true);
					if (!IsCurrentCellDirty)
						CurrentCell = this[e.ColumnIndex, e.RowIndex];
					BeginEdit(true);
				}
			}
			base.OnCellMouseUp(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (e != null)
				origin = e.Location;
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (e != null && AllowUserToMoveRows && e.Button == MouseButtons.Left &&
				SelectionMode == DataGridViewSelectionMode.FullRowSelect &&
				(MultiDrag && SelectedItemCount > 0 || !MultiDrag && SelectedItemCount == 1) &&
				(Math.Abs(origin.X - e.X) > SystemInformation.DragSize.Width / 2 ||
				Math.Abs(origin.Y - e.Y) > SystemInformation.DragSize.Height / 2))
				DoDragDrop(new DataGridViewMovedRows(SelectedRows, this),
					DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move | DragDropEffects.None | DragDropEffects.Scroll);
			base.OnMouseMove(e);
		}

		protected override void OnDragEnter(DragEventArgs drgevent)
		{
			if (drgevent != null && drgevent.Data.GetDataPresent(typeof(DataGridViewMovedRows)))
				drgevent.Effect = DragDropEffects.Move | DragDropEffects.Scroll;
			else
				base.OnDragEnter(drgevent);
		}

		protected override void OnDragOver(DragEventArgs drgevent)
		{
			if (drgevent == null || !drgevent.Data.GetDataPresent(typeof(DataGridViewMovedRows)))
			{
				base.OnDragOver(drgevent);
				return;
			}
			var pt = PointToClient(new Point(drgevent.X, drgevent.Y));
			int diffTop = ScrollArea - pt.Y;
			int diffBottom = pt.Y - Height + ScrollArea;
			if (diffTop >= 0)
				FirstDisplayedScrollingRowIndexUnchecked -= IncrementDragOverCalled(diffTop);
			else if (diffBottom >= 0)
				FirstDisplayedScrollingRowIndexUnchecked += IncrementDragOverCalled(diffBottom);
			else
			{
				var info = GetDragHitTestInfo(drgevent);
				HitRowIndex = info.ActualDestination >= 0 ? info.HitIndex : -1;
			}
		}

		protected override void OnDragLeave(EventArgs e)
		{
			HitRowIndex = -1;
			base.OnDragLeave(e);
		}

		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			if (drgevent == null || !drgevent.Data.GetDataPresent(typeof(DataGridViewMovedRows)) || HitRowIndex < 0)
			{
				base.OnDragDrop(drgevent);
				return;
			}
			int newIndex = GetDragHitTestInfo(drgevent).ActualDestination;
			if (newIndex < 0)
				return;
			var dgdo = (DataGridViewMovedRows)drgevent.Data.GetData(typeof(DataGridViewMovedRows));
			RowMovingEventArgs ev = new RowMovingEventArgs(dgdo.Source, dgdo.SourceRows, dgdo.SetRow, newIndex);
			OnRowMoving(ev);
			if (!ev.Cancel)
			{
				foreach (var row in dgdo.SourceRows)
					dgdo.Source.Rows.Remove(row);
				Rows.InsertRange(newIndex, dgdo.GetModifiedRows());
				ClearSelection();
				for (int i = newIndex; i < newIndex + dgdo.SourceRows.Count; i++)
					SetSelectedRowCore(i, true);
				OnRowMoved(EventArgs.Empty);
			}
			HitRowIndex = -1;
		}

		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs qcdevent)
		{
			if (qcdevent != null && ((qcdevent.KeyState & 2) > 0 || (qcdevent.KeyState & 16) > 0))
				qcdevent.Action = DragAction.Cancel;
			base.OnQueryContinueDrag(qcdevent);
		}

		protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
		{
			if (e != null && e.RowIndex == HitRowIndex)
				e.Graphics.DrawLine(insertionPen, e.RowBounds.Left, e.RowBounds.Top + 1, e.RowBounds.Right, e.RowBounds.Top + 1);
			base.OnRowPostPaint(e);
		}

		/// <summary><see cref="RowMoving"/> イベントを発生させます。</summary>
		protected virtual void OnRowMoving(RowMovingEventArgs e) { RowMoving?.Invoke(this, e); }

		/// <summary><see cref="RowMoved"/> イベントを発生させます。</summary>
		protected virtual void OnRowMoved(EventArgs e) { RowMoved?.Invoke(this, e); }

		/// <summary><see cref="QueryActualDestination"/> イベントを発生させます。</summary>
		protected virtual void OnQueryActualDestination(QueryActualDestinationEventArgs e) { QueryActualDestination?.Invoke(this, e); }

		struct DragHitTestInfo
		{
			public DragHitTestInfo(int hitIndex, int actualDestination)
			{
				HitIndex = hitIndex;
				ActualDestination = actualDestination;
			}

			public int HitIndex { get; }

			public int ActualDestination { get; }
		}

		class DataGridViewMovedRows
		{
			public DataGridViewMovedRows(DataGridViewSelectedRowCollection rows, DataGridView source)
			{
				if (rows == null)
					throw new ArgumentNullException(nameof(rows));
				modifiedRows = rows.Cast<DataGridViewRow>().OrderBy(x => x.Index).ToArray();
				SourceRows = new ReadOnlyCollection<DataGridViewRow>((DataGridViewRow[])modifiedRows.Clone());
				Source = source;
			}

			DataGridViewRow[] modifiedRows;

			public ReadOnlyCollection<DataGridViewRow> SourceRows { get; }

			public void SetRow(int index, DataGridViewRow row) { modifiedRows[index] = row; }

			public DataGridViewRow[] GetModifiedRows() => (DataGridViewRow[])modifiedRows.Clone();

			public DataGridView Source { get; }
		}
	}

	public class RowMovingEventArgs : CancelEventArgs
	{
		public RowMovingEventArgs(DataGridView source, ReadOnlyCollection<DataGridViewRow> sourceRows, Action<int, DataGridViewRow> setRow, int dest)
		{
			Source = source;
			SourceRows = sourceRows;
			this.setRow = setRow;
			Destination = dest;
		}

		Action<int, DataGridViewRow> setRow = (a, b) => { };

		public int Destination { get; }

		public DataGridView Source { get; }

		public ReadOnlyCollection<DataGridViewRow> SourceRows { get; }

		public void SetRow(int index, DataGridViewRow row) { setRow(index, row); }
	}

	public class QueryActualDestinationEventArgs : EventArgs
	{
		public QueryActualDestinationEventArgs(int actualDest, DragDropEffects effects, DataGridView source)
		{
			ActualDestination = actualDest;
			Effect = effects;
			Source = source;
		}

		public int ActualDestination { get; set; }

		public DragDropEffects Effect { get; set; }

		public DataGridView Source { get; }
	}
}
