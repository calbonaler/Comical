using System;
using System.IO;
using System.Threading.Tasks;

namespace Comical.Core
{
	public class Binary(byte[] data)
	{
		readonly byte[] _data = data ?? throw new ArgumentNullException(nameof(data));

		public int Length => _data.Length;

		public Task WriteToAsync(Stream stream) => stream.WriteAsync(_data, 0, _data.Length);

		public MemoryStream ToStream() => new(_data, false);

		public static async Task<Binary> FromFileAsync(string path)
		{
			using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.None);
			if (fileStream.Length > int.MaxValue)
				throw new IOException(Properties.Resources.FileTooLong2GB);
			var array = new byte[(int)fileStream.Length];
			await fileStream.ReadExactlyAsync(array).ConfigureAwait(false);
			return new Binary(array);
		}
	}
}
