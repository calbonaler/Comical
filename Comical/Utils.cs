using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Comical
{
	static class Utils
	{
		static class NativeMethods
		{
			[DllImport("DwmApi.dll")]
			public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref NativeMargins m);
		}

		[StructLayout(LayoutKind.Sequential)]
		struct NativeMargins
		{
			int LeftWidth;
			int RightWidth;
			int TopHeight;
			int BottomHeight;
			public NativeMargins(bool fullWindow) { LeftWidth = RightWidth = TopHeight = BottomHeight = (fullWindow ? -1 : 0); }
		};

		public static void ExtendFrameIntoClientArea(this Form form, bool fullWindow)
		{
			if (form == null)
				throw new ArgumentNullException("form");
			NativeMargins m = new NativeMargins(fullWindow);
			Marshal.ThrowExceptionForHR(NativeMethods.DwmExtendFrameIntoClientArea(form.Handle, ref m));
		}

		public static void LoadFromXml(this WeifenLuo.WinFormsUI.Docking.DockPanel panel, string xml, params WeifenLuo.WinFormsUI.Docking.IDockContent[] contents)
		{
			if (panel == null)
				throw new ArgumentNullException("panel");
			if (string.IsNullOrEmpty(xml))
				return;
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(xml)))
				panel.LoadFromXml(ms, persistString => Array.Find(contents, x => string.Equals(persistString, x.DockHandler.GetPersistStringCallback(), StringComparison.Ordinal)));
		}

		public static IEnumerable<T> SplitEnumValue<T>(T value) where T : struct
		{
			if (!value.Equals(Enum.ToObject(typeof(T), 0)))
			{
				foreach (var item in (T[])Enum.GetValues(typeof(T)))
				{
					if (!item.Equals(Enum.ToObject(typeof(T), 0)) && ((Enum)(object)value).HasFlag((Enum)(object)item))
						yield return item;
				}
			}
		}
	}
}
