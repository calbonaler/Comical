using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Comical.Controls
{
	static class NativeMethods
	{
		[DllImport("gdi32.dll")]
		public static extern uint GetPixel(IntPtr hdc, int x, int y);

		public static Color GetPixelColor(IntPtr hdc, int xPos, int yPos)
		{
			uint rgb = GetPixel(hdc, xPos, yPos);
			return Color.FromArgb((int)(rgb & 0xFF), (int)((rgb >> 8) & 0xFF), (int)((rgb >> 16) & 0xFF));
		}
	}
}
