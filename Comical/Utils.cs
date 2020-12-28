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

		public static Image CreateImage(this Binary binary, Size size)
		{
			using var ms = binary.ToStream();
			using var image = Image.FromStream(ms);
			if (size.IsEmpty)
				size = image.Size;
			var ratio = Math.Min(image.Width * size.Height, size.Width * image.Height);
			Bitmap bitmap = null;
			try { bitmap = new Bitmap(image, ratio / image.Height, ratio / image.Width); }
			catch
			{
				if (bitmap != null)
					bitmap.Dispose();
				throw;
			}
			return bitmap;
		}

		public static Image CreateImage(this Binary binary) => CreateImage(binary, Size.Empty);

		public static System.Drawing.Imaging.ImageCodecInfo GetImageCodecInfo(this Image image) => Array.Find(System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders(), item => item.FormatID == image.RawFormat.Guid);
	}
}
