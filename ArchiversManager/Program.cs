using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CommonLibrary;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Comical.Archivers.Manager
{
	static class Program
	{
		[STAThread]
		static void Main()
		{
			var args = Environment.GetCommandLineArgs();
			if (args.Length > 2)
			{
				IntPtr ownerHandle = IntPtr.Zero;
				int ow = 0;
				if (int.TryParse(args[1], out ow) && ow != 0)
					ownerHandle = new IntPtr(ow);
				var currentDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				if (args[2].Equals("/update", StringComparison.OrdinalIgnoreCase))
					UpdateAll(ownerHandle, currentDirectory);
				else if (args.Length > 3)
				{
					var set = ArchiversConfiguration.Settings.FirstOrDefault(s => s.DllName.Equals(args[3], StringComparison.OrdinalIgnoreCase));
					if (set != null)
					{
						if (args[2].Equals("/install", StringComparison.OrdinalIgnoreCase))
							Deploy(ownerHandle, set, currentDirectory);
						else if (args[2].Equals("/uninstall", StringComparison.OrdinalIgnoreCase))
							Uninstall(ownerHandle, set, currentDirectory);
					}
				}
			}
		}

		static void Deploy(IntPtr owner, ArchiverSetting set, string directory)
		{
			if (set.DependedArchiver != null && !set.DependedArchiver.Exists)
				Deploy(owner, set.DependedArchiver, directory);
			var task = set.GetDownloadUrls();
			task.Wait();
			var urls = task.Result;
			using (WebClient client = new WebClient())
			using (TaskDialog dialog = new TaskDialog())
			{
				dialog.Caption = Properties.Resources.Install;
				dialog.OwnerWindowHandle = owner;
				dialog.StandardButtons = TaskDialogStandardButtons.Cancel;
				dialog.InstructionText = Properties.Resources.InstallDownloadingDll;
				dialog.ProgressBar = new TaskDialogProgressBar(0, 100, 0);
				dialog.Opened += async (s, ev) =>
				{
					var di = CommonUtils.TempFolder.CreateSubdirectory("Dll");
					string name = Path.Combine(di.FullName, Path.GetFileName(urls[0]));
					for (int i = 0; i < urls.Length; i++)
					{
						var timeout = Task.Delay(5000);
						var download = client.DownloadFileTaskAsync(new Uri(urls[i]), name, new Progress<int>(p => dialog.ProgressBar.Value = p));
						dialog.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.InstallRetryMessage, i + 1);
						if (await Task.WhenAny(download, timeout) == timeout && dialog.ProgressBar.Value == 0)
							client.CancelAsync();
						else
						{
							await download;
							if (download.Status == TaskStatus.RanToCompletion)
							{
								dialog.InstructionText = Properties.Resources.InstallExtractingArchive;
								var needList = new List<string>() { set.BundleDllName, set.DllName };
								var arc = ArchiversConfiguration.FindArchiverToExtract(name);
								if (arc != null)
								{
									using (arc)
										arc.Extract(name, di.FullName, string.Join(" ", needList));
									foreach (var f in needList)
									{
										var file = new FileInfo(Path.Combine(di.FullName, f));
										if (file.Exists)
										{
											file.CopyTo(Path.Combine(directory, f), true);
											file.Delete();
										}
									}
									foreach (var dir in di.EnumerateDirectories())
									{
										foreach (var file in dir.EnumerateFiles("*", SearchOption.AllDirectories))
											file.CopyTo(Path.Combine(directory, file.Name), true);
									}
								}
								else if (set.DependedArchiver == null)
									LzhExtractor.Melt(name, directory, needList.ToArray());
							}
							break;
						}
					}
					di.Delete(true);
					dialog.Close();
				};
				if (dialog.Show() == TaskDialogResult.Cancel)
					client.CancelAsync();
			}
		}

		static void Uninstall(IntPtr owner, ArchiverSetting arcset, string directory)
		{
			string source = "";
			try
			{
				File.Delete(source = arcset.Path);
				var files = Directory.GetFiles(directory, arcset.BundleDllName);
				for (int j = 0; j < files.Length; j++)
					File.Delete(source = files[j]);
			}
			catch (UnauthorizedAccessException)
			{
				using (TaskDialog dialog = new TaskDialog())
				{
					dialog.Caption = Properties.Resources.Uninstall;
					dialog.OwnerWindowHandle = owner;
					dialog.InstructionText = string.Format(CultureInfo.CurrentCulture, Properties.Resources.UninstallCannotRemoveFile, Path.GetFileName(source));
					dialog.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.UninstallSetReadOnlyAttribute, source);
					dialog.Icon = TaskDialogStandardIcon.Error;
					dialog.Show();
				}
			}
			catch (IOException)
			{
				using (TaskDialog dialog = new TaskDialog())
				{
					dialog.Caption = Properties.Resources.Uninstall;
					dialog.OwnerWindowHandle = owner;
					dialog.InstructionText = string.Format(CultureInfo.CurrentCulture, Properties.Resources.UninstallCannotRemoveFile, Path.GetFileName(source));
					dialog.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.UninstallFileInUse, source);
					dialog.Icon = TaskDialogStandardIcon.Error;
					dialog.Show();
				}
			}
		}

		static void UpdateAll(IntPtr owner, string directory)
		{
			var t = Task.WhenAll(ArchiversConfiguration.Settings.Where(x => x.Exists).Select(async x => new { Setting = x, LatestVersionAvailable = await x.IsLatestVersionAvailable() }));
			t.Wait();
			foreach (var set in t.Result)
			{
				if (set.LatestVersionAvailable)
					Deploy(owner, set.Setting, directory);
			}
		}
	}
}
