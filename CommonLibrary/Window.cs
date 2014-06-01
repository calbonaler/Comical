using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace CommonLibrary
{
	public class Window : IWin32Window
	{
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

		public int TextLength { get { return CommonLibrary.NativeMethods.SendMessage(Handle, 0x0E, IntPtr.Zero, IntPtr.Zero).ToInt32() + 1; } }

		public Icon GetIcon(bool large)
		{
			IntPtr ptr = CommonLibrary.NativeMethods.SendMessage(Handle, 0x7F, new IntPtr(large ? 1 : 0), IntPtr.Zero);
			if (ptr == IntPtr.Zero)
				ptr = CommonLibrary.NativeMethods.GetClassLongPtr(Handle, large ? WindowClassMember.Icon : WindowClassMember.SmallIcon);
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
			using (var handle = SafeEnumWindowHandle.Create((hw, lp) =>
			{
				var w = new Window(hw);
				if (predicate(w))
					windows.Add(w);
				return true;
			}))
				NativeMethods.EnumChildWindows(ownerHandle, handle, IntPtr.Zero);
			return windows;
		}

		static Window DescendantInternal(IntPtr ownerHandle, Func<Window, bool> predicate)
		{
			Window window = null;
			using (var handle = SafeEnumWindowHandle.Create((hw, lp) =>
			{
				var w = new Window(hw);
				if (predicate(w))
				{
					window = w;
					return false;
				}
				return true;
			}))
				NativeMethods.EnumChildWindows(ownerHandle, handle, IntPtr.Zero);
			return window;
		}
	}
}
