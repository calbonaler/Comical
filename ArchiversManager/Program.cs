using System;
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
			if (args.Length <= 2)
				return;
			IntPtr ownerHandle = IntPtr.Zero;
			int ow = 0;
			if (int.TryParse(args[1], out ow) && ow != 0)
				ownerHandle = new IntPtr(ow);
			if (args[2].Equals("/update", StringComparison.OrdinalIgnoreCase))
				UpdateAll(ownerHandle);
			else if (args.Length > 3)
			{
				var set = ArchiversConfiguration.Settings.FirstOrDefault(s => s.DllName.Equals(args[3], StringComparison.OrdinalIgnoreCase));
				if (set != null)
				{
					if (args[2].Equals("/install", StringComparison.OrdinalIgnoreCase))
						Deploy(ownerHandle, set);
					else if (args[2].Equals("/uninstall", StringComparison.OrdinalIgnoreCase))
						Uninstall(ownerHandle, set);
				}
			}
		}
		
		static readonly string DeployedDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

		static bool Deploy(IntPtr owner, ArchiverSetting set)
		{
			bool innerResult = false;
			if (set.DependedArchiver != null && !set.DependedArchiver.Exists)
				innerResult = Deploy(owner, set.DependedArchiver);
			var task = set.GetAvailableArchiverInfoAsync();
			task.Wait();
			using (var arc = set.CreateArchiver())
			{
				if (arc == null || task.Result == null || arc.Version >= task.Result.AvailableVersion)
					return innerResult || false;
			}
			var urls = task.Result.Urls;
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
					try
					{
						string archiveFileName = Path.Combine(di.FullName, Path.GetFileName(urls[0].AbsolutePath));
						for (int i = 0; i < urls.Count; i++)
						{
							var timeout = Task.Delay(5000);
							var download = client.DownloadFileTaskAsync(urls[i], archiveFileName, new Progress<int>(p => dialog.ProgressBar.Value = p));
							dialog.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.InstallRetryMessage, i + 1);
							if (await Task.WhenAny(download, timeout) == timeout && dialog.ProgressBar.Value == 0)
								client.CancelAsync();
							else
							{
								await download;
								if (download.IsFaulted)
									throw download.Exception;
								break;
							}
						}
						dialog.InstructionText = Properties.Resources.InstallExtractingArchive;
						var needList = new string[] { set.BundleDllName, set.DllName };
						if (LzhExtractor.Melt(archiveFileName, DeployedDirectory, needList))
							return;
						using (var arc = ArchiversConfiguration.FindArchiverToExtract(archiveFileName))
							arc.Extract(archiveFileName, di.FullName, string.Join(" ", needList.Where(x => !string.IsNullOrEmpty(x))));
						File.Delete(archiveFileName);
						foreach (var file in di.EnumerateFiles("*", SearchOption.AllDirectories))
							file.CopyTo(Path.Combine(DeployedDirectory, file.Name), true);
					}
					finally
					{
						di.Delete(true);
						dialog.Close();
					}
				};
				if (dialog.Show() == TaskDialogResult.Cancel)
					client.CancelAsync();
				return true;
			}
		}

		static void Uninstall(IntPtr owner, ArchiverSetting arcset)
		{
			string source = "";
			try
			{
				File.Delete(source = arcset.Path);
				var files = Directory.GetFiles(DeployedDirectory, arcset.BundleDllName);
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

		static void UpdateAll(IntPtr owner)
		{
			bool deployed = false;
			foreach (var set in ArchiversConfiguration.Settings.Where(x => x.Exists))
				deployed = deployed || Deploy(owner, set);
			if (deployed)
				return;
			using (TaskDialog dialog = new TaskDialog())
			{
				dialog.Cancelable = true;
				dialog.Caption = Properties.Resources.Update;
				dialog.Icon = TaskDialogStandardIcon.Information;
				dialog.InstructionText = Properties.Resources.AllArchiversAreLatest;
				dialog.OwnerWindowHandle = owner;
				dialog.StartupLocation = TaskDialogStartupLocation.CenterOwner;
				dialog.StandardButtons = TaskDialogStandardButtons.Close;
				dialog.Show();
			}
		}
	}
}
