using System;
using System.Windows.Forms;

namespace Comical
{
	static class Utils
	{
		public static void InvokeIfNeeded(this Control control, Action action)
		{
			if (control.InvokeRequired)
				control.Invoke(action);
			else
				action();
		}
	}
}
