using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Comical.Core
{
	public static class Crypto
	{
		public static byte[] Transform(byte[] src, string password, Encoding enc, bool decrypt)
		{
			if (src == null)
				throw new ArgumentNullException("src");
			if (enc == null)
				throw new ArgumentNullException("enc");
			byte[] dest = new byte[src.Length];
			TransformInternal(dest, src, enc.GetBytes(string.IsNullOrEmpty(password) ? "\0" : password), src.Length, decrypt);
			return dest;
		}

		public static async Task Transform(Stream readStream, Stream writeStream, string password, Encoding enc, int length, bool decrypt)
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
			byte[] temp = new byte[65536 / (pw.Length - 1) * (pw.Length - 1)];
			while (length > 0)
			{
				int bytesRead = await readStream.ReadAsync(temp, 0, (int)Math.Min(temp.Length, length)).ConfigureAwait(false);
				if (bytesRead <= 0)
					return;
				TransformInternal(temp, temp, pw, bytesRead, decrypt);
				writeStream.Write(temp, 0, bytesRead);
				length -= bytesRead;
			}
		}

		static void TransformInternal(byte[] dest, byte[] src, byte[] password, int length, bool decrypt)
		{
			for (int i = 0; i < length; i++)
			{
				byte p = password[i % (password.Length - 1)];
				dest[i] = (byte)(unchecked(src[i] + (decrypt ? -p : p)));
			}
		}
	}
}
