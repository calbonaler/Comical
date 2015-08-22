using System;
using System.ComponentModel;
using System.Threading;

namespace Comical.Core
{
	public static class Extensions
	{
		internal static void SendIfNeeded(this SynchronizationContext context, Action action)
		{
			if (context == SynchronizationContext.Current || context == null)
				action();
			else
				context.Send(_ => action(), null);
		}

		public static string ToString<T>(this T value, T exclusiveMax, IFormatProvider provider) where T : IFormattable
		{
			var exMax = Convert.ToDouble(exclusiveMax, provider);
			return value.ToString(new string('0', exMax > 1 ? (int)Math.Log10(exMax - 1) + 1 : 1), provider);
		}
		
		internal static void Raise(this PropertyChangedEventHandler handler, object @this, SynchronizationContext context, [System.Runtime.CompilerServices.CallerMemberName]string propertyName = "")
		{
			if (handler != null)
				context.SendIfNeeded(() => handler(@this, new PropertyChangedEventArgs(propertyName)));
		}
	}
}
