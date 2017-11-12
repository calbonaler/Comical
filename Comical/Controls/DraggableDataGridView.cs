using System;
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
		bool _allowUserToMoveRows = false;
		Point? _origin;
		int _dragOverCalled = 0;
		int _hitRowIndex = -1;
		Pen _insertionPen = new Pen(Color.Black, 2.0F);

		protected override void Dispose(bool disposing)
		{
			if (_insertionPen != null)
			{
				_insertionPen.Dispose();
				_insertionPen = null;
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

		/// <summary>行のドラッグ・ドロップ効果を確認するときに発生します。</summary>
		[Category("アクション")]
		[Description("行のドラッグ・ドロップ効果を確認するときに発生します。")]
		public event EventHandler<QueryRowDragDropEffectEventArgs> QueryRowDragDropEffect;

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

		DragHitTestInfo DragHitTest(IDataObject data, Point point)
		{
			var obj = (DataGridViewMovedRows)data.GetData(typeof(DataGridViewMovedRows));
			var ev = new QueryRowDragDropEffectEventArgs(DragDropEffects.Move, obj.Source);
			OnQueryRowDragDropEffect(ev);
			if (ev.Effect == DragDropEffects.None)
				return DragHitTestInfo.Nowhere;
			var pt = PointToClient(point);
			int index = HitTest(pt.X, pt.Y).RowIndex;
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
			int actualDest = index;
			if (obj.Source == this)
			{
				var rowIndex = obj.SourceRows.Min(x => x.Index);
				if (index > rowIndex + obj.SourceRows.Length)
					actualDest = index - obj.SourceRows.Length;
				else if (index >= rowIndex)
					return DragHitTestInfo.Nowhere;
			}
			return new DragHitTestInfo(ev.Effect | DragDropEffects.Scroll, index, actualDest);
		}

		int IncrementDragOverCalled(int value)
		{
			int callMax = ScrollArea / 2;
			_dragOverCalled += value;
			int c = _dragOverCalled / callMax;
			_dragOverCalled = _dragOverCalled % callMax;
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
			if (e != null && AllowUserToMoveRows && e.Button == MouseButtons.Left && _origin != null &&
				SelectionMode == DataGridViewSelectionMode.FullRowSelect &&
				(MultiDrag ? SelectedRows.Count > 0 : SelectedRows.Count == 1) &&
				(Math.Abs(_origin.Value.X - e.X) > SystemInformation.DragSize.Width / 2 || Math.Abs(_origin.Value.Y - e.Y) > SystemInformation.DragSize.Height / 2))
			{
				DoDragDrop(new DataGridViewMovedRows(SelectedRows, this), DragDropEffects.Copy | DragDropEffects.Link | DragDropEffects.Move | DragDropEffects.Scroll);
				_origin = null;
			}
			base.OnMouseMove(e);
		}

		protected override void OnDragEnter(DragEventArgs drgevent)
		{
			if (drgevent != null && drgevent.Data.GetDataPresent(typeof(DataGridViewMovedRows)))
				drgevent.Effect = DragHitTest(drgevent.Data, new Point(drgevent.X, drgevent.Y)).Effect;
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
			if (diffBottom >= 0)
				FirstDisplayedScrollingRowIndexUnchecked += IncrementDragOverCalled(diffBottom);
			var info = DragHitTest(drgevent.Data, new Point(drgevent.X, drgevent.Y));
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
			if (drgevent == null || !drgevent.Data.GetDataPresent(typeof(DataGridViewMovedRows)) || HitRowIndex < 0)
			{
				base.OnDragDrop(drgevent);
				return;
			}
			var info = DragHitTest(drgevent.Data, new Point(drgevent.X, drgevent.Y));
			drgevent.Effect = info.Effect;
			if (info.Effect == DragDropEffects.None)
				return;
			var dgdo = (DataGridViewMovedRows)drgevent.Data.GetData(typeof(DataGridViewMovedRows));
			var ev = new RowMovingEventArgs(dgdo.Source, dgdo.SourceRows, info.ActualDestination);
			OnRowMoving(ev);
			if (!ev.Cancel)
			{
				foreach (var row in dgdo.SourceRows)
					dgdo.Source.Rows.Remove(row);
				Rows.InsertRange(info.ActualDestination, dgdo.SourceRows);
				ClearSelection();
				foreach (var i in Enumerable.Range(info.ActualDestination, dgdo.SourceRows.Length))
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
				e.Graphics.DrawLine(_insertionPen, e.RowBounds.Left, e.RowBounds.Top + 1, e.RowBounds.Right, e.RowBounds.Top + 1);
			base.OnRowPostPaint(e);
		}

		/// <summary><see cref="RowMoving"/> イベントを発生させます。</summary>
		protected virtual void OnRowMoving(RowMovingEventArgs e) => RowMoving?.Invoke(this, e);

		/// <summary><see cref="RowMoved"/> イベントを発生させます。</summary>
		protected virtual void OnRowMoved(EventArgs e) => RowMoved?.Invoke(this, e);

		/// <summary><see cref="QueryRowDragDropEffect"/> イベントを発生させます。</summary>
		protected virtual void OnQueryRowDragDropEffect(QueryRowDragDropEffectEventArgs e) => QueryRowDragDropEffect?.Invoke(this, e);

		struct DragHitTestInfo
		{
			public DragHitTestInfo(DragDropEffects effect, int hitIndex, int actualDestination)
			{
				Effect = effect;
				HitIndex = hitIndex;
				ActualDestination = actualDestination;
			}

			public static readonly DragHitTestInfo Nowhere = new DragHitTestInfo(DragDropEffects.None, -1, -1);

			public DragDropEffects Effect { get; }

			public int HitIndex { get; }

			public int ActualDestination { get; }
		}

		class DataGridViewMovedRows
		{
			public DataGridViewMovedRows(DataGridViewSelectedRowCollection rows, DataGridView source)
			{
				if (rows == null)
					throw new ArgumentNullException(nameof(rows));
				SourceRows = rows.Cast<DataGridViewRow>().OrderBy(x => x.Index).ToArray();
				Source = source;
			}

			public DataGridViewRow[] SourceRows { get; }

			public DataGridView Source { get; }
		}
	}

	public class RowMovingEventArgs : CancelEventArgs
	{
		public RowMovingEventArgs(DataGridView source, DataGridViewRow[] sourceRows, int dest)
		{
			Source = source;
			SourceRows = new ReadOnlyCollection<DataGridViewRow>(sourceRows);
			Destination = dest;
		}

		public int Destination { get; }

		public DataGridView Source { get; }

		public ReadOnlyCollection<DataGridViewRow> SourceRows { get; }
	}

	public class QueryRowDragDropEffectEventArgs : EventArgs
	{
		public QueryRowDragDropEffectEventArgs(DragDropEffects effects, DataGridView source)
		{
			Effect = effects;
			Source = source;
		}

		public DragDropEffects Effect { get; set; }

		public DataGridView Source { get; }
	}
}
