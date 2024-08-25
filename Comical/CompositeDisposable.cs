using Comical.Core;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Comical
{
	sealed class CompositeDisposable : IDisposable, IEnumerable<IDisposable>
	{
		readonly List<IDisposable> _disposables = [];

		public void Add(IDisposable item) => _disposables.Add(item);

		public void Add(Action action) => _disposables.Add(new DelegateDisposable(action));

		public void Dispose()
		{
			foreach (var item in _disposables)
				item.Dispose();
		}

		public IEnumerator<IDisposable> GetEnumerator() => _disposables.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
