using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Comical
{
	public sealed class Window : IWin32Window
	{
		static class NativeMethods
		{
			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool EnumChildWindows(IntPtr hWndParent, SafeFunctionPointer lpEnumFunc, IntPtr lParam);

			[DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern int GetClassName(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

			[DllImport("user32.dll", CharSet = CharSet.Unicode)]
			public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

			[DllImport("user32.dll")]
			[return: MarshalAs(UnmanagedType.Bool)]
			public static extern bool GetWindowInfo(IntPtr hWnd, WindowInfo pwi);

			[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetClassLong")]
			static extern int GetClassLongPtr32(IntPtr hWnd, [MarshalAs(UnmanagedType.I4)]WindowClassMember info);

			[DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetClassLongPtr")]
			static extern IntPtr GetClassLongPtr64(IntPtr hWnd, [MarshalAs(UnmanagedType.I4)]WindowClassMember info);

			public static IntPtr GetClassLongPtr(IntPtr hWnd, WindowClassMember info)
			{
				if (IntPtr.Size == 4)
					return new IntPtr(GetClassLongPtr32(hWnd, info));
				else if (IntPtr.Size == 8)
					return GetClassLongPtr64(hWnd, info);
				else
					throw new PlatformNotSupportedException();
			}
		}

		sealed class SafeFunctionPointer : SafeHandle
		{
			public SafeFunctionPointer(Delegate d) : base(IntPtr.Zero, true)
			{
				_delegate = d;
				handle = Marshal.GetFunctionPointerForDelegate(_delegate);
			}

			Delegate _delegate;

			public override bool IsInvalid { get { return handle == IntPtr.Zero; } }

			protected override bool ReleaseHandle()
			{
				_delegate = null;
				return true;
			}
		}

		enum WindowClassMember
		{
			MenuName = -8,
			BackgroundBrush = -10,
			Cursor = -12,
			Icon = -14,
			Module = -16,
			ExtraWindowMemorySize = -18,
			ExtraClassMemorySize = -20,
			WindowProcedure = -24,
			Style = -26,
			Atom = -32,
			SmallIcon = -34,
		}

		delegate bool EnumWindowProc(IntPtr hWnd, IntPtr lParam);

		public Window(IntPtr handle) { Handle = handle; }

		public IntPtr Handle { get; private set; }

		public string ClassName
		{
			get
			{
				StringBuilder sb = new StringBuilder(2048);
				if (NativeMethods.GetClassName(Handle, sb, sb.Capacity) == 0)
					Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
				return sb.ToString();
			}
		}

		public string Text
		{
			get
			{
				IntPtr ptr = IntPtr.Zero;
				try
				{
					ptr = Marshal.StringToCoTaskMemUni(new string('\0', TextLength + 1));
					NativeMethods.SendMessage(Handle, 0x0D, new IntPtr(TextLength + 1), ptr);
					return Marshal.PtrToStringUni(ptr);
				}
				finally
				{
					if (ptr != IntPtr.Zero)
						Marshal.FreeCoTaskMem(ptr);
				}
			}
		}

		public int TextLength { get { return NativeMethods.SendMessage(Handle, 0x0E, IntPtr.Zero, IntPtr.Zero).ToInt32() + 1; } }

		public Icon GetIcon(bool large)
		{
			IntPtr ptr = NativeMethods.SendMessage(Handle, 0x7F, new IntPtr(large ? 1 : 0), IntPtr.Zero);
			if (ptr == IntPtr.Zero)
				ptr = NativeMethods.GetClassLongPtr(Handle, large ? WindowClassMember.Icon : WindowClassMember.SmallIcon);
			if (ptr != IntPtr.Zero)
				return Icon.FromHandle(ptr);
			else
				return null;
		}

		public WindowInfo Information
		{
			get
			{
				WindowInfo info = new WindowInfo();
				NativeMethods.GetWindowInfo(Handle, info);
				return info;
			}
		}

		public WindowStyles Style { get { return Information.Style; } }

		public override string ToString() { return Text; }

		public IEnumerable<Window> Descendants() { return Descendants(w => true); }

		public IEnumerable<Window> Descendants(Func<Window, bool> predicate) { return DescendantsInternal(Handle, predicate); }

		public Window Descendant(Func<Window, bool> predicate) { return DescendantInternal(Handle, predicate); }

		public static IEnumerable<Window> TopLevels() { return TopLevels(w => true); }

		public static IEnumerable<Window> TopLevels(Func<Window, bool> predicate) { return DescendantsInternal(IntPtr.Zero, predicate); }

		public static Window TopLevel(Func<Window, bool> predicate) { return DescendantInternal(IntPtr.Zero, predicate); }

		static IEnumerable<Window> DescendantsInternal(IntPtr ownerHandle, Func<Window, bool> predicate)
		{
			List<Window> windows = new List<Window>();
			using (var ptr = new SafeFunctionPointer(new EnumWindowProc((hw, lp) =>
			{
				var w = new Window(hw);
				if (predicate(w))
					windows.Add(w);
				return true;
			})))
				NativeMethods.EnumChildWindows(ownerHandle, ptr, IntPtr.Zero);
			return windows;
		}

		static Window DescendantInternal(IntPtr ownerHandle, Func<Window, bool> predicate)
		{
			Window window = null;
			using (var ptr = new SafeFunctionPointer(new EnumWindowProc((hw, lp) =>
			{
				var w = new Window(hw);
				if (predicate(w))
				{
					window = w;
					return false;
				}
				return true;
			})))
				NativeMethods.EnumChildWindows(ownerHandle, ptr, IntPtr.Zero);
			return window;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	public sealed class WindowInfo
	{
		[StructLayout(LayoutKind.Sequential)]
		struct NativeRect
		{
			int left;
			int top;
			int right;
			int bottom;
			public Rectangle ToRectangle() { return Rectangle.FromLTRB(left, top, right, bottom); }
		}

		uint cbSize;
		NativeRect rcWindow;
		NativeRect rcClient;
		WindowStyles dwStyle;
		ExtendedWindowStyles dwExStyle;
		uint dwWindowStatus;
		uint cxWindowBorders;
		uint cyWindowBorders;
		ushort atomWindowType;
		ushort wCreatorVersion;
		public WindowInfo() { cbSize = (uint)Marshal.SizeOf(typeof(WindowInfo)); }
		public Rectangle WindowRectangle { get { return rcWindow.ToRectangle(); } }
		public Rectangle ClientRectangle { get { return rcClient.ToRectangle(); } }
		public WindowStyles Style { get { return dwStyle; } }
		public ExtendedWindowStyles ExtendedStyles { get { return dwExStyle; } }
		public bool IsActive { get { return dwWindowStatus == 1; } }
		public Size BorderSize { get { return new Size((int)cxWindowBorders, (int)cyWindowBorders); } }
		public int CreatorVersion { get { return wCreatorVersion; } }
	}

	public enum WindowStyles
	{
		None = 0x00000000,
		TabStop = 0x00010000,
		Group = 0x00020000,
		SizeBox = 0x00040000,
		SystemMenu = 0x00080000,
		HorizontalScroll = 0x00100000,
		VerticalScroll = 0x00200000,
		DlagFrame = 0x00400000,
		Border = 0x00800000,
		Maximize = 0x01000000,
		ClipChildren = 0x02000000,
		ClipSiblings = 0x04000000,
		Disabled = 0x08000000,
		Visible = 0x10000000,
		Minimize = 0x20000000,
		Child = 0x40000000,
		Popup = unchecked((int)0x80000000),
		MaximizeBox = TabStop,
		MinimizeBox = Group,
		Caption = Border | DlagFrame,
		PopupWindow = Popup | Border | SystemMenu,
		OverlapedWindow = None | Caption | SystemMenu | SizeBox | MinimizeBox | MaximizeBox,
	}

	[Flags]
	public enum ExtendedWindowStyles
	{
		None = 0x00000000,
		AcceptFiles = 0x00000010,
		ApplicationWindow = 0x00040000,
		BorderSunken = 0x00000200,
		DoubleBuffer = 0x02000000,
		HelpButton = 0x00000400,
		HasChildWindows = 0x00010000,
		BorderDouble = 0x00000001,
		LayeredWindow = 0x00080000,
		RtlLayout = 0x00400000,
		LeftScrollBarAlignment = 0x00004000,
		MdiChild = 0x00000040,
		Unactivatable = 0x08000000,
		DisableChildrenInheritance = 0x00100000,
		DisableNotificationToParent = 0x00000004,
		OverlappedWindow = BorderRaised | BorderSunken,
		PaletteWindow = BorderRaised | ToolWindow | TopMost,
		RghtAlignment = 0x00001000,
		RtlReading = 0x00002000,
		Border3D = 0x00020000,
		ToolWindow = 0x00000080,
		TopMost = 0x00000008,
		Transparent = 0x00000020,
		BorderRaised = 0x00000100,
	}
}
