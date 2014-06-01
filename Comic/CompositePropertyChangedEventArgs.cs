using System;
using System.Collections.Generic;

namespace Comical.Core
{
	public class CompositePropertyChangedEventArgs : EventArgs
	{
		public CompositePropertyChangedEventArgs(Dictionary<object, string> propertyNames) { PropertyNames = propertyNames; }

		public Dictionary<object, string> PropertyNames { get; private set; }
	}
}
