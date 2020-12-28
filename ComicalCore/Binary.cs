using System;
using System.IO;
using System.Threading.Tasks;

namespace Comical.Core
{
	public class Binary
	{
		public Binary(byte[] data) => _data = data ?? throw new ArgumentNullException(nameof(data));

		readonly byte[] _data;

		public int Length => _data.Length;

		public Task WriteToAsync(Stream stream) => stream.WriteAsync(_data, 0, _data.Length);

		public MemoryStream ToStream() => new MemoryStream(_data, false);

		public static async Task<Binary> FromFileAsync(string path)
		{
			using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.None);
			var offset = 0;
			if (fileStream.Length > int.MaxValue)
				throw new IOException(Properties.Resources.FileTooLong2GB);
			var bytesToRead = (int)fileStream.Length;
			var array = new byte[bytesToRead];
			while (bytesToRead > 0)
			{
				var bytesRead = await fileStream.ReadAsync(array, offset, bytesToRead).ConfigureAwait(false);
				if (bytesRead == 0)
					throw new EndOfStreamException();
				offset += bytesRead;
				bytesToRead -= bytesRead;
			}
			return new Binary(array);
		}
	}
}
