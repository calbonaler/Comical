﻿using System;

namespace Comical.Core
{
	public sealed class DelegateDisposable : IDisposable
	{
		public DelegateDisposable(Action action) { _action = action; }

		Action _action;

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
