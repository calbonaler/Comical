using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Comical.Core
{
	static class Utils
	{
		public static void SetProperty<T>(ref T storage, T value, object @this, PropertyChangedEventHandler handler, [CallerMemberName]string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(storage, value))
				return;
			storage = value;
			handler?.Invoke(@this, new PropertyChangedEventArgs(propertyName));
		}

		public static ValueTask<int> ReadExactlyNoThrowAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
			=> stream.ReadAtLeastAsync(buffer, buffer.Length, false, cancellationToken);
	}
}
