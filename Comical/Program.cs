using System;
using System.IO;
using System.Windows.Forms;
using Comical.Core;

namespace Comical;

static class Program
{
	/// <summary>
	/// アプリケーションのメイン エントリ ポイントです。
	/// </summary>
	[STAThread]
	static void Main(string[] args)
	{
		ApplicationConfiguration.Initialize();
		if (args.Length >= 2 && args[0].Equals("/view", StringComparison.OrdinalIgnoreCase) && File.Exists(args[1]) && FileHeader.LoadAsync(args[1]).Result != null)
			Application.Run(new ViewerForm(args[1]));
		else
			Application.Run(new EditorForm());
	}
}
