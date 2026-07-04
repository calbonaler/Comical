using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Comical.Controls;

public partial class ScrollBaredControl : Control
{
	public ScrollBaredControl()
	{
		ScrollBars = new ScrollProperties(this);
		SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
	}

	static readonly object s_mouseHorizontalWheelEvent = new();
	static readonly object s_scrollChangedEvent = new();

	public event MouseEventHandler? MouseHorizontalWheel
	{
		add => Events.AddHandler(s_mouseHorizontalWheelEvent, value);
		remove => Events.RemoveHandler(s_mouseHorizontalWheelEvent, value);
	}
	public event EventHandler? ScrollChanged
	{
		add => Events.AddHandler(s_scrollChangedEvent, value);
		remove => Events.RemoveHandler(s_scrollChangedEvent, value);
	}

	protected override CreateParams CreateParams
	{
		get
		{
			const int WS_HSCROLL = 0x100000;
			const int WS_VSCROLL = 0x200000;
			var param = base.CreateParams;
			param.Style |= WS_HSCROLL | WS_VSCROLL;
			return param;
		}
	}
	public ScrollProperties ScrollBars { get; }

	protected override void OnResize(EventArgs e)
	{
		ScrollBars.ViewportSize = ClientSize;
		base.OnResize(e);
	}
	protected override bool IsInputKey(Keys keyData) => keyData.HasFlag(Keys.Left) || keyData.HasFlag(Keys.Up) || keyData.HasFlag(Keys.Right) || keyData.HasFlag(Keys.Down) || keyData.HasFlag(Keys.Enter) || base.IsInputKey(keyData);
	protected override void OnMouseWheel(MouseEventArgs e)
	{
		if (ScrollBars.IsYOverflow && ModifierKeys == Keys.None)
		{
			ScrollBars.ScrollBySeparately(new Size(-e.Delta, -e.Delta), true);
			OnScrollChanged(EventArgs.Empty);
		}
		else if (ScrollBars.IsXOverflow && (ModifierKeys & ~Keys.Shift) == Keys.None)
		{
			ScrollBars.ScrollBySeparately(new Size(-e.Delta, -e.Delta), false);
			OnScrollChanged(EventArgs.Empty);
		}
		base.OnMouseWheel(e);
	}
	protected virtual void OnMouseHorizontalWheel(MouseEventArgs e)
	{
		if (ScrollBars.IsXOverflow && ModifierKeys == Keys.None)
		{
			ScrollBars.ScrollBySeparately(new Size(e.Delta, e.Delta), false);
			OnScrollChanged(EventArgs.Empty);
		}
		((MouseEventHandler?)Events[s_mouseHorizontalWheelEvent])?.Invoke(this, e);
	}
	protected virtual void OnScrollChanged(EventArgs e) => ((EventHandler?)Events[s_scrollChangedEvent])?.Invoke(this, e);
	protected override void WndProc(ref Message m)
	{
		if (ScrollBars.ProcessScrollMessages(ref m))
			OnScrollChanged(EventArgs.Empty);
		else if (m.Msg == 0x20E) // WM_MOUSEHWHEEL
		{
			var pt = PointToClient(new Point(unchecked((short)(m.LParam & 0xFFFF)), unchecked((short)(m.LParam >> 16 & 0xFFFF))));
			var e = new HandledMouseEventArgs(MouseButtons.None, 0, pt.X, pt.Y, unchecked((short)(m.WParam >> 16 & 0xFFFF)));
			OnMouseHorizontalWheel(e);
			m.Result = e.Handled ? 1 : 0;
			if (!e.Handled)
				DefWndProc(ref m);
		}
		else
		{
			base.WndProc(ref m);
		}
	}

	public partial class ScrollProperties(Control owner)
	{
		readonly Control _owner = owner;
		Size _contentSize;
		Size _viewportSize;
		Point _position;

		public Size ContentSize
		{
			get => _contentSize;
			set
			{
				_contentSize = value;
				Position = _position;
			}
		}
		public Size ViewportSize
		{
			get => _viewportSize;
			set
			{
				_viewportSize = value;
				Position = _position;
			}
		}
		public Point Position
		{
			get => _position;
			set
			{
				SetPositionNoApply(value);
				Apply(true, true);
			}
		}
		void SetPositionNoApply(Point value) => _position = Clamp(value, default, Maximum);

		public void ScrollBySeparately(Size value, bool vertical)
		{
			SetPositionNoApply(_position + (vertical ? new Size(0, value.Height) : new Size(value.Width, 0)));
			Apply(!vertical, vertical);
		}
		public void ScrollToSeparately(Point value, bool vertical)
		{
			SetPositionNoApply(vertical ? new Point(_position.X, value.Y) : new Point(value.X, _position.Y));
			Apply(!vertical, vertical);
		}
		public void ScrollByLine(bool decrease, bool vertical) => ScrollBySeparately(decrease ? new Size(-1, -1) : new Size(1, 1), vertical);
		public void ScrollByViewport(bool decrease, bool vertical) => ScrollBySeparately(decrease ? Size.Empty - ViewportSize : ViewportSize, vertical);
		public void ScrollToMinimum(bool vertical) => ScrollToSeparately(default, vertical);
		public void ScrollToMaximum(bool vertical) => ScrollToSeparately(Maximum, vertical);
		public void ScrollToTrackPosition(bool vertical)
		{
			var scrollInfo = new ScrollInfo(ScrollInfoMask.TrackPosition);
			if (!NativeMethods.GetScrollInfo(_owner.Handle, vertical ? ScrollBarKind.Vertical : ScrollBarKind.Horizontal, ref scrollInfo))
				Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
			SetPositionNoApply(vertical ? new(_position.X, scrollInfo.TrackPosition) : new(scrollInfo.TrackPosition, _position.Y));
			Apply(!vertical, vertical);
		}

		public Point Maximum => Max((Point)(ContentSize - ViewportSize), default);
		public bool IsOverflow => IsXOverflow || IsYOverflow;
		public bool IsXOverflow => ContentSize.Width > ViewportSize.Width;
		public bool IsYOverflow => ContentSize.Height > ViewportSize.Height;
		void Apply(bool horizontal, bool vertical)
		{
			if (horizontal)
				NativeMethods.SetScrollInfo(_owner.Handle, ScrollBarKind.Horizontal, new ScrollInfo(ScrollInfoMask.Range | ScrollInfoMask.Page | ScrollInfoMask.Position)
				{
					Minimum = 0,
					Maximum = ContentSize.Width,
					Page = ViewportSize.Width,
					Position = Position.X,
				}, true);
			if (vertical)
				NativeMethods.SetScrollInfo(_owner.Handle, ScrollBarKind.Vertical, new ScrollInfo(ScrollInfoMask.Range | ScrollInfoMask.Page | ScrollInfoMask.Position)
				{
					Minimum = 0,
					Maximum = ContentSize.Height,
					Page = ViewportSize.Height,
					Position = Position.Y,
				}, true);
		}

		public bool ProcessScrollMessages(ref Message m)
		{
			const int WM_HSCROLL = 0x114;
			const int WM_VSCROLL = 0x115;
			switch (m.Msg)
			{
				case WM_HSCROLL:
					OnWmScroll(ref m, false);
					return true;
				case WM_VSCROLL:
					OnWmScroll(ref m, true);
					return true;
				default:
					return false;
			}
		}
		void OnWmScroll(ref Message m, bool vertical)
		{
			switch ((ScrollEventType)(m.WParam & 0xffff))
			{
				case ScrollEventType.SmallDecrement:
					ScrollByLine(true, vertical);
					break;
				case ScrollEventType.SmallIncrement:
					ScrollByLine(false, vertical);
					break;
				case ScrollEventType.LargeDecrement:
					ScrollByViewport(true, vertical);
					break;
				case ScrollEventType.LargeIncrement:
					ScrollByViewport(false, vertical);
					break;
				case ScrollEventType.ThumbPosition:
				case ScrollEventType.ThumbTrack:
					ScrollToTrackPosition(vertical);
					break;
				case ScrollEventType.First:
					ScrollToMinimum(vertical);
					break;
				case ScrollEventType.Last:
					ScrollToMaximum(vertical);
					break;
			}
		}

		static Point Clamp(Point value, Point min, Point max) => new(Math.Clamp(value.X, min.X, max.X), Math.Clamp(value.Y, min.Y, max.Y));
		static Point Max(Point value1, Point value2) => new(Math.Max(value1.X, value2.X), Math.Max(value1.Y, value2.Y));

		static partial class NativeMethods
		{
			[LibraryImport("user32.dll")]
			public static partial int SetScrollInfo(nint hwnd, ScrollBarKind nBar, in ScrollInfo lpsi, [MarshalAs(UnmanagedType.Bool)] bool redraw);
			[LibraryImport("user32.dll", SetLastError = true)]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static partial bool GetScrollInfo(nint hwnd, ScrollBarKind nBar, ref ScrollInfo lpsi);
		}

		struct ScrollInfo(ScrollInfoMask mask)
		{
			public int StructureSize = Marshal.SizeOf<ScrollInfo>();
			public ScrollInfoMask Mask = mask;
			public int Minimum;
			public int Maximum;
			public int Page;
			public int Position;
			public int TrackPosition;
		}

		enum ScrollBarKind
		{
			Horizontal = 0,
			Vertical = 1,
			Control = 2,
		}

		[Flags]
		enum ScrollInfoMask : uint
		{
			Range = 0x01,
			Page = 0x02,
			Position = 0x04,
			DisableNoScroll = 0x08,
			TrackPosition = 0x10,
			All = Range | Page | Position | TrackPosition,
		}
	}
}
