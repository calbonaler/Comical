using System;
using System.Collections.Generic;
using System.Linq;

namespace Comical.Core
{
	public class CompositePropertyChangedEventArgs<T> : EventArgs
	{
		public CompositePropertyChangedEventArgs(IEnumerable<KeyValuePair<T, string>> propertyNames) { PropertyNames = propertyNames.ToLookup(x => x.Key, x => x.Value); }

		public ILookup<T, string> PropertyNames { get; private set; }
	}
}
