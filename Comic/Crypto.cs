using System;
using System.IO;
using System.Text;

namespace Comical.Core
{
	public static class Crypto
	{
		public static byte[] Encrypt(byte[] src, string password, Encoding enc)
		{
			if (src == null)
				throw new ArgumentNullException("src");
			using (MemoryStream sourceStream = new MemoryStream(src))
			using (MemoryStream targetStream = new MemoryStream())
			{
				Encrypt(sourceStream, targetStream, password, enc, src.Length);
				return targetStream.ToArray();
			}
		}

		public static void Encrypt(Stream readStream, Stream writeStream, string password, Encoding enc, int length)
		{
			if (writeStream == null)
				return;
			if (readStream == null)
				throw new ArgumentNullException("readStream");
			if (enc == null)
				throw new ArgumentNullException("enc");
			if (string.IsNullOrEmpty(password))
				password = "\0";
			byte[] pw = enc.GetBytes(password);
			byte[] temp = new byte[4096 / (pw.Length - 1) * (pw.Length - 1)];
			while (length > 0)
			{
				int bytesRead = readStream.Read(temp, 0, (int)Math.Min(temp.Length, length));
				for (int i = 0; i < bytesRead; i++)
					temp[i] = (byte)(unchecked(temp[i] + pw[i % (pw.Length - 1)]));
				writeStream.Write(temp, 0, bytesRead);
				length -= bytesRead;
			}
		}

		public static byte[] Decrypt(byte[] src, string password, Encoding enc)
		{
			if (src == null)
				throw new ArgumentNullException("src");
			using (MemoryStream srcStream = new MemoryStream(src))
			using (MemoryStream targetStream = new MemoryStream())
			{
				Decrypt(srcStream, targetStream, password, enc, src.Length);
				return targetStream.ToArray();
			}
		}

		public static void Decrypt(Stream readStream, Stream writeStream, string password, Encoding enc, int length)
		{
			if (writeStream == null)
				return;
			if (readStream == null)
				throw new ArgumentNullException("readStream");
			if (enc == null)
				throw new ArgumentNullException("enc");
			if (string.IsNullOrEmpty(password))
				password = "\0";
			byte[] pw = enc.GetBytes(password);
			byte[] temp = new byte[4096 / (pw.Length - 1) * (pw.Length - 1)];
			while (length > 0)
			{
				int bytesRead = readStream.Read(temp, 0, (int)Math.Min(temp.Length, length));
				for (int i = 0; i < bytesRead; i++)
					temp[i] = (byte)(unchecked(temp[i] - pw[i % (pw.Length - 1)]));
				writeStream.Write(temp, 0, bytesRead);
				length -= bytesRead;
			}
		}
	}
}
