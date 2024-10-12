using System;
using System.IO;
using System.Threading.Tasks;

namespace Comical.Core
{
	public class Binary
	{
		Binary(byte[] data) => _data = data;

		readonly byte[] _data;

		public int Length => _data.Length;

		public Task WriteToAsync(Stream stream) => stream.WriteAsync(_data, 0, _data.Length);

		public MemoryStream ToStream() => new(_data, false);

		public static Binary FromMemoryStream(MemoryStream memoryStream) => new(memoryStream.ToArray());

		public static async Task<Binary?> TryFromStreamAsync(Stream stream, int bytesToRead, ReadOnlyMemory<byte> leadingBytes)
		{
			var array = new byte[leadingBytes.Length + bytesToRead];
			leadingBytes.CopyTo(array);
			var bytesRead = await stream.ReadAtLeastAsync(array.AsMemory(leadingBytes.Length), bytesToRead, false).ConfigureAwait(false);
			return bytesRead == bytesToRead ? new Binary(array) : null;
		}

		public static async Task<Binary> FromStreamAsync(Stream stream, int bytesToRead, bool throwOnEndOfStream)
		{
			var array = new byte[bytesToRead];
			await stream.ReadAtLeastAsync(array, bytesToRead, throwOnEndOfStream).ConfigureAwait(false);
			return new Binary(array);
		}

		public static async Task<Binary> FromFileAsync(string path)
		{
			using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.None);
			return fileStream.Length > int.MaxValue ? throw new IOException(Properties.Resources.FileTooLong2GB) :
				await FromStreamAsync(fileStream, (int)fileStream.Length, true).ConfigureAwait(false);
		}
	}
}
