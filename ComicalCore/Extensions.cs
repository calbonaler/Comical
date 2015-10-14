using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Comical.Core
{
	/// <summary>既存のクラスを拡張するユーティリティ メソッドを提供します。</summary>
	static class Extensions
	{
		/// <summary>指定されたインスタンスおよびプロパティ名を使用して、<see cref="INotifyPropertyChanged.PropertyChanged"/> イベントを発生させます。</summary>
		/// <param name="handler"><see cref="INotifyPropertyChanged.PropertyChanged"/> イベントを発生させるデリゲートを指定します。</param>
		/// <param name="this"><see cref="INotifyPropertyChanged.PropertyChanged"/> イベントを発生させるインスタンスを指定します。</param>
		/// <param name="propertyName">変更されたプロパティの名前を指定します。省略した場合はこのメソッドが呼び出されたプロパティの名前が使用されます。</param>
		public static void Raise(this PropertyChangedEventHandler handler, object @this, [CallerMemberName]string propertyName = "") { handler?.Invoke(@this, new PropertyChangedEventArgs(propertyName)); }
	}
}
