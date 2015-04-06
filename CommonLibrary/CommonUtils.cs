﻿using System;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommonLibrary
{
	public static class CommonUtils
	{
		public static async Task<string> GetHtml(Uri url, CancellationToken token)
		{
			using (var client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
			using (var response = await client.GetAsync(url, token))
			{
				var bytes = await response.Content.ReadAsByteArrayAsync();
				var charset = response.Content.Headers.ContentType.CharSet;
				var infos = EncodingDetector.DetectEncoding(bytes, IncomingDataTypes.Html, !string.IsNullOrEmpty(charset) ? Encoding.GetEncoding(charset) : null);
				return infos.Length == 0 ? "" : infos[0].Encoding.GetString(bytes);
			}
		}

		public static void SetWindow(this Control control, string appName)
		{
			if (control == null)
				throw new ArgumentNullException("control");
			Marshal.ThrowExceptionForHR(NativeMethods.SetWindowTheme(control.Handle, appName, null));
		}

		public static void ExtendFrameIntoClientArea(this Form form, bool fullWindow)
		{
			if (form == null)
				throw new ArgumentNullException("form");
			Margins m = new Margins(fullWindow);
			Marshal.ThrowExceptionForHR(NativeMethods.DwmExtendFrameIntoClientArea(form.Handle, ref m));
		}

		public static void SendIfNeeded(this SynchronizationContext context, Action action)
		{
			if (action == null)
				throw new ArgumentNullException("action");
			if (context == SynchronizationContext.Current)
				action();
			else
				context.Send(_ => action(), null);
		}

		public static string ToString(this int value, int maximum) { return value.ToString(new string('0', maximum >= 1 ? (int)Math.Log10(maximum) + 1 : 1), CultureInfo.CurrentCulture); }

		public static Image Resize(this Image src, Size size)
		{
			if (src == null)
				throw new ArgumentNullException("src");
			Bitmap bitmap = null;
			Bitmap tmp = null;
			try
			{
				if (size.IsEmpty)
					tmp = new Bitmap(src);
				else
				{
					var x = Math.Min(src.Width * size.Height, size.Width * src.Height);
					tmp = new Bitmap(src, x / src.Height, x / src.Width);
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
}
