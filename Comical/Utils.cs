using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using Comical.Core;

namespace Comical;

static class Utils
{
	public static void InvokeIfNeeded(this Control control, Action action)
	{
		if (control.InvokeRequired)
			control.Invoke(action);
		else
			action();
	}

	/// <summary>バイナリを <see cref="Image"/> に変換する。</summary>
	/// <param name="binary"><see cref="Image"/> に変換するバイナリ情報。</param>
	/// <returns>変換後の <see cref="Image"/>。Save メソッドは使用できない。</returns>
	public static Image ToImage(this Binary binary)
	{
		using var ms = binary.ToStream();
		return Image.FromStream(ms);
	}

	public static Size ScaleSize(Size original, Size containing)
	{
		if (original.Width <= containing.Width && original.Height <= containing.Height)
			return original;
		var ratio = Math.Min(original.Width * containing.Height, containing.Width * original.Height);
		return new Size(ratio / original.Height, ratio / original.Width);
	}

	public static Binary ToBinary(this Image image, ImageFormat format)
	{
		using var ms = new MemoryStream();
		image.Save(ms, format);
		return Binary.FromMemoryStream(ms);
	}

	public static Binary EnsureBitmap(this Binary binary)
	{
		if (binary.IsBitmap) return binary;
		using var image = binary.ToImage();
		using var bitmap = new Bitmap(image);
		return bitmap.ToBinary(ImageFormat.Bmp);
	}

	public static ImageCodecInfo? GetImageCodecInfo(this Image image) => Array.Find(ImageCodecInfo.GetImageDecoders(), item => item.FormatID == image.RawFormat.Guid);
}
