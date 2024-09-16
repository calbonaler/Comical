using System;
using System.Collections.Generic;
using System.Linq;

namespace Comical.Core
{
	public interface INotifyCollectionItemPropertyChanged
	{
		event EventHandler<CollectionItemPropertyChangedEventArgs>? CollectionItemPropertyChanged;
	}

	public class CollectionItemPropertyChangedEventArgs(object? item, string? propertyName) : EventArgs
	{
		public object? Item { get; } = item;
		public string? PropertyName { get; } = propertyName;
	}
}
