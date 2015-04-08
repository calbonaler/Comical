using System;
using System.IO;
using System.Text;

namespace Comical.Core
{
	public static class Crypto
	{
		public static byte[] Transform(byte[] src, string password, Encoding enc, bool decrypt)
		{
			if (src == null)
				throw new ArgumentNullException("src");
			using (MemoryStream source = new MemoryStream(src, false))
			using (MemoryStream destination = new MemoryStream())
			{
				Transform(source, destination, password, enc, src.Length, decrypt);
				return destination.ToArray();
			}
		}

		public static void Transform(Stream readStream, Stream writeStream, string password, Encoding enc, int length, bool decrypt)
		{
			if (readStream == null)
				throw new ArgumentNullException("readStream");
			if (writeStream == null)
				throw new ArgumentNullException("writeStream");
			if (enc == null)
				throw new ArgumentNullException("enc");
			if (string.IsNullOrEmpty(password))
				password = "\0";
			byte[] pw = enc.GetBytes(password);
			byte[] temp = new byte[4096 / (pw.Length - 1) * (pw.Length - 1)];
			while (length > 0)
			{
				int bytesRead = readStream.Read(temp, 0, (int)Math.Min(temp.Length, length));
				if (bytesRead <= 0)
					return;
				for (int i = 0; i < bytesRead; i++)
				{
					byte p = pw[i % (pw.Length - 1)];
					temp[i] = (byte)(unchecked(temp[i] + (decrypt ? -p : p)));
				}
				writeStream.Write(temp, 0, bytesRead);
				length -= bytesRead;
			}
		}
	}
}
