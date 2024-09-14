using System;

namespace Comical.Core
{
	public sealed class DelegateDisposable(Action? action) : IDisposable
	{
		Action? _action = action;

		public void Dispose()
		{
			if (_action != null)
			{
				_action();
				_action = null;
			}
		}
	}
}
