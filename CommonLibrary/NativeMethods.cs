using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace CommonLibrary
{
	static class NativeMethods
	{
		[DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
		public static extern int SetWindowTheme(IntPtr windowHandle, string appName, string idList);

		[DllImport("DwmApi.dll")]
		public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref Margins m);

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool EnumChildWindows(IntPtr hWndParent, SafeEnumWindowHandle lpEnumFunc, IntPtr lParam);

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

	delegate bool EnumWindowProc(IntPtr hWnd, IntPtr lParam);

	class SafeEnumWindowHandle : SafeHandle
	{
		SafeEnumWindowHandle() : base(IntPtr.Zero, true) { }

		EnumWindowProc procedure;

		public static SafeEnumWindowHandle Create(EnumWindowProc proc)
		{
			SafeEnumWindowHandle handle = null;
			SafeEnumWindowHandle tmp = null;
			try
			{
				tmp = new SafeEnumWindowHandle();
				tmp.procedure = proc;
				tmp.handle = Marshal.GetFunctionPointerForDelegate(tmp.procedure);
				handle = tmp;
				tmp = null;
			}
			finally
			{
				if (tmp != null)
					tmp.Dispose();
			}
			return handle;
		}

		public override bool IsInvalid { get { return handle == IntPtr.Zero; } }

		protected override bool ReleaseHandle()
		{
			procedure = null;
			return true;
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	struct Margins
	{
		public int LeftWidth;
		public int RightWidth;
		public int TopHeight;
		public int BottomHeight;
		public Margins(bool fullWindow) { LeftWidth = RightWidth = TopHeight = BottomHeight = (fullWindow ? -1 : 0); }
	};

	[StructLayout(LayoutKind.Sequential)]
	struct NativeRect
	{
		int left;
		int top;
		int right;
		int bottom;
		public Rectangle ToRectangle() { return Rectangle.FromLTRB(left, top, right, bottom); }
	}

	[StructLayout(LayoutKind.Sequential)]
	public class WindowInfo
	{
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
}
