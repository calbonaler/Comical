using System;
using System.Drawing;
using System.Windows.Forms;
using Comical.Core;

namespace Comical
{
	static class Utils
	{
		public static void InvokeIfNeeded(this Control control, Action action)
		{
			if (control.InvokeRequired)
				control.Invoke(action);
			else
				action();
		}

		public static Image GetImage(this ImageReference ir, Size size)
		{
			using (var ms = ir.OpenImageStream())
			using (var image = Image.FromStream(ms))
			{
				Bitmap bitmap = null;
				Bitmap tmp = null;
				try
				{
					if (size.IsEmpty)
						tmp = new Bitmap(image);
					else
					{
						var x = Math.Min(image.Width * size.Height, size.Width * image.Height);
						tmp = new Bitmap(image, x / image.Height, x / image.Width);
					}
					bitmap = tmp;
					tmp = null;
				}
				finally
				{
					if (tmp != null)
						tmp.Dispose();
				}
				return bitmap;
			}
		}

		public static Image GetImage(this ImageReference ir) => GetImage(ir, Size.Empty);

		public static System.Drawing.Imaging.ImageCodecInfo GetImageCodecInfo(this Image image)
		{
			return Array.Find(System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders(), item => item.FormatID == image.RawFormat.Guid);
        }
	}
}
