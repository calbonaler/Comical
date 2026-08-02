using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Comical.Core;

static class Utils
{
	public static void SetProperty<T>(ref T storage, T value, object @this, PropertyChangedEventHandler? handler, [CallerMemberName] string propertyName = "")
	{
		if (EqualityComparer<T>.Default.Equals(storage, value))
			return;
		storage = value;
		handler?.Invoke(@this, new PropertyChangedEventArgs(propertyName));
	}

	public static int TryReadExactlyWithBytesRead(this Stream stream, Span<byte> buffer) => stream.ReadAtLeast(buffer, buffer.Length, false);

	public static ValueTask<int> TryReadExactlyWithBytesReadAsync(this Stream stream, Memory<byte> buffer) => stream.ReadAtLeastAsync(buffer, buffer.Length, false);

	public static bool TryReadExactly(this Stream stream, Span<byte> buffer)
	{
		var bytesRead = stream.TryReadExactlyWithBytesRead(buffer);
		return bytesRead == buffer.Length;
	}

	public static async ValueTask<bool> TryReadExactlyAsync(this Stream stream, Memory<byte> buffer)
	{
		var bytesRead = await stream.TryReadExactlyWithBytesReadAsync(buffer).ConfigureAwait(false);
		return bytesRead == buffer.Length;
	}
}
