using System;
using System.Collections.Generic;
using System.Linq;

namespace Comical.Core
{
	public interface INotifyCollectionItemPropertyChanged
	{
		event EventHandler<CollectionItemPropertyChangedEventArgs> CollectionItemPropertyChanged;
	}

	public class CollectionItemPropertyChangedEventArgs : EventArgs
	{
		public CollectionItemPropertyChangedEventArgs(IEnumerable<KeyValuePair<object, string>> propertyNames) { PropertyNames = propertyNames.ToLookup(x => x.Key, x => x.Value); }

		public CollectionItemPropertyChangedEventArgs(params KeyValuePair<object, string>[] propertyNames) : this((IEnumerable<KeyValuePair<object, string>>)propertyNames) { }

		public CollectionItemPropertyChangedEventArgs(object originalSender, string propertyName) : this(new KeyValuePair<object, string>(originalSender, propertyName)) { }

		public ILookup<object, string> PropertyNames { get; }
	}
}
