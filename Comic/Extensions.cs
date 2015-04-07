using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Threading;

namespace Comical.Core
{
	public static class Extensions
	{
		public static void SendIfNeeded(this SynchronizationContext context, Action action)
		{
			if (action == null)
				throw new ArgumentNullException("action");
			if (context == SynchronizationContext.Current || context == null)
				action();
			else
				context.Send(_ => action(), null);
		}

		public static string ToString(this int value, int maximum) { return value.ToString(new string('0', maximum >= 1 ? (int)Math.Log10(maximum) + 1 : 1), CultureInfo.CurrentCulture); }

		public static Image Resize(this Image src, Size size)
		{
			if (src == null)
				throw new ArgumentNullException("src");
			Bitmap bitmap = null;
			Bitmap tmp = null;
			try
			{
				if (size.IsEmpty)
					tmp = new Bitmap(src);
				else
				{
					var x = Math.Min(src.Width * size.Height, size.Width * src.Height);
					tmp = new Bitmap(src, x / src.Height, x / src.Width);
				}
				bitmap = tmp;
				tmp = null;
			}
			finally
			{
				if (tmp != null)
					tmp.Dispose();
			}
			return bitmap;
		}

		public static void Raise<T>(this PropertyChangedEventHandler handler, object @this, SynchronizationContext context, System.Linq.Expressions.Expression<Func<T>> property)
		{
			var memberExp = property.Body as System.Linq.Expressions.MemberExpression;
			if (memberExp != null && handler != null)
				context.SendIfNeeded(() => handler(@this, new PropertyChangedEventArgs(memberExp.Member.Name)));
		}

		public static void Raise(this PropertyChangedEventHandler handler, object @this, SynchronizationContext context, [System.Runtime.CompilerServices.CallerMemberName]string propertyName = "")
		{
			if (handler != null)
				context.SendIfNeeded(() => handler(@this, new PropertyChangedEventArgs(propertyName)));
		}
	}
}
