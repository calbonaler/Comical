using System;
using System.Windows.Forms;
using Comical.Core;

namespace Comical
{
	static class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			if (!Properties.Settings.Default.HasUpgraded)
			{
				Properties.Settings.Default.Upgrade();
				Properties.Settings.Default.HasUpgraded = true;
				Properties.Settings.Default.Save();
			}
			if (args.Length >= 2 && args[0].Equals("/view", StringComparison.OrdinalIgnoreCase) && System.IO.File.Exists(args[1]) && new FileHeader(args[1]).CanOpen)
				Application.Run(new ViewerForm(args[1]));
			else
				Application.Run(new EditorForm());
		}
	}
}
