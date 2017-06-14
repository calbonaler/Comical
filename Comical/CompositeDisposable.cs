using Comical.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comical
{
	sealed class CompositeDisposable : IDisposable, IEnumerable<IDisposable>
	{
		List<IDisposable> _disposables = new List<IDisposable>();

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
