using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Comical.Core;

public class Binary
{
	Binary(byte[] data) => _data = data;

	readonly byte[] _data;

	public int Length => _data.Length;

	public bool IsBitmap => Bitmap.TryGetLength(_data, out var bitmapLength) && bitmapLength == _data.Length;

	public Task WriteToAsync(Stream stream) => stream.WriteAsync(_data, 0, _data.Length);

	public MemoryStream ToStream() => new(_data, false);

	public static Binary FromMemoryStream(MemoryStream memoryStream) => new(memoryStream.ToArray());

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

	internal static Task<Binary?> TryReadBitmapAsync(Stream stream, ReadOnlySpan<byte> leadingBytes)
	{
		static (byte[]? DataBuffer, int BytesFilled) TryReadHeader(Stream stream, ReadOnlySpan<byte> leadingBytes)
		{
			var writableHeader = leadingBytes.Length < Bitmap.MinimumLength ? stackalloc byte[Bitmap.MinimumLength] : default;
			if (!writableHeader.IsEmpty)
			{
				leadingBytes.CopyTo(writableHeader);
				if (!stream.TryReadExactly(writableHeader[leadingBytes.Length..Bitmap.MinimumLength]))
					return (null, 0);
			}
			var headerBytes = !writableHeader.IsEmpty ? writableHeader[..Bitmap.MinimumLength] : leadingBytes;
			if (!Bitmap.TryGetLength(headerBytes, out var bitmapLength))
				return (null, 0);
			var bitmapData = new byte[bitmapLength];
			headerBytes.CopyTo(bitmapData);
			var bytesFilled = Math.Min(headerBytes.Length, bitmapLength);
			return (bitmapData, bytesFilled);
		}
		static async Task<Binary?> TryReadDataAsync(Stream stream, byte[] bitmapData, int bytesFilled)
			=> await stream.TryReadExactlyAsync(bitmapData.AsMemory(bytesFilled)).ConfigureAwait(false) ? new Binary(bitmapData) : null;
		var (bitmapData, bytesFilled) = TryReadHeader(stream, leadingBytes);
		return bitmapData != null ? TryReadDataAsync(stream, bitmapData, bytesFilled) : Task.FromResult<Binary?>(null);
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	struct Bitmap
	{
		[InlineArray(2)]
		struct IdentifierArray { byte _value; }

		IdentifierArray Identifier;
		public uint Length;

		public static int MinimumLength => Unsafe.SizeOf<Bitmap>();
		public static bool TryGetLength(ReadOnlySpan<byte> data, out int value)
		{
			if (data.Length < MinimumLength)
			{
				value = default;
				return false;
			}
			var header = MemoryMarshal.Read<Bitmap>(data);
			if (!"BM"u8.SequenceEqual(header.Identifier))
			{
				value = default;
				return false;
			}
			value = (int)header.Length;
			return true;
		}
	}
}
