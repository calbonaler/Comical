using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Comical.Archivers;
using CommonLibrary;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Comical
{
	public partial class ManageArchiverDialog : Form
	{
		public ManageArchiverDialog()
		{
			InitializeComponent();
		}

		void StartListeningToExited(Process process)
		{
			if (process != null)
			{
				EventHandler exited = (s, ev) => RefreshArchiverList();
				process.EnableRaisingEvents = true;
				process.SynchronizingObject = this;
				process.Exited += exited;
				if (process.HasExited)
				{
					process.Exited -= exited;
					exited(null, null);
				}
			}
		}

		void RefreshArchiverList()
		{
			lvArchivers.Items.Clear();
			foreach (var set in ArchiversConfiguration.Settings)
			{
				var item = lvArchivers.Items.Add(new ListViewItem(set.DllName));
				item.ImageKey = set.Exists ? "installed" : "uninstalled";
				item.SubItems[0].Name = "clmName";
				using (var arc = set.CreateArchiver())
					item.SubItems.Add(arc != null ? arc.Version.ToString() : "").Name = "clmVersion";
			}
			clmVersion.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
			clmName.Width = lvArchivers.ClientSize.Width - clmVersion.Width;
			lvArchivers_SelectedIndexChanged(null, null);
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			lvArchivers.SetWindow("Explorer");
			RefreshArchiverList();
			btnInstall.UseElevationIcon(true);
			btnUninstall.UseElevationIcon(true);
			btnUpdate.UseElevationIcon(true);
		}

		private void lvArchivers_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lvArchivers.SelectedIndices.Count > 0)
				btnInstall.Enabled = !(btnUninstall.Enabled = ArchiversConfiguration.Settings[lvArchivers.SelectedIndices[0]].Exists);
			else
				btnInstall.Enabled = btnUninstall.Enabled = false;
		}

		private void btnInstall_Click(object sender, EventArgs e)
		{
			StartListeningToExited(ArchiversConfiguration.Install(Handle, ArchiversConfiguration.Settings[lvArchivers.SelectedIndices[0]]));
		}

		private void btnUninstall_Click(object sender, EventArgs e)
		{
			var set = ArchiversConfiguration.Settings[lvArchivers.SelectedIndices[0]];
			var users = set.GetDependingArchivers().Where(x => x.Exists).Select(x => x.DllName);
			if (users.Any())
			{
				using (TaskDialog dialog = new TaskDialog())
				{
					dialog.Cancelable = true;
					dialog.Caption = Properties.Resources.Uninstall;
					dialog.Icon = TaskDialogStandardIcon.Error;
					dialog.InstructionText = string.Format(CultureInfo.CurrentCulture, Properties.Resources.CannotUninstallWithDependencies, set.DllName);
					dialog.OwnerWindowHandle = Handle;
					dialog.StandardButtons = TaskDialogStandardButtons.Close;
					dialog.StartupLocation = TaskDialogStartupLocation.CenterOwner;
					dialog.Text = Properties.Resources.Archivers_HasDependencyDialog_DetailsHeader + "\r\n" + string.Join("\r\n", users);
					dialog.Show();
				}
			}
			else
				StartListeningToExited(ArchiversConfiguration.Uninstall(Handle, set));
		}

		private async void btnUpdate_Click(object sender, EventArgs e)
		{
			bool notNeedUpdate = true;
			foreach (var set in ArchiversConfiguration.Settings.Where(x => x.Exists))
			{
				if (await set.IsLatestVersionAvailableAsync())
				{
					notNeedUpdate = false;
					break;
				}
			}
			if (notNeedUpdate)
			{
				using (TaskDialog dialog = new TaskDialog())
				{
					dialog.Cancelable = true;
					dialog.Caption = Properties.Resources.Update;
					dialog.Icon = TaskDialogStandardIcon.Information;
					dialog.InstructionText = Properties.Resources.AllArchiversAreLatest;
					dialog.OwnerWindowHandle = Handle;
					dialog.StartupLocation = TaskDialogStartupLocation.CenterOwner;
					dialog.StandardButtons = TaskDialogStandardButtons.Close;
					dialog.Show();
				}
			}
			else
				StartListeningToExited(ArchiversConfiguration.UpdateAll(Handle));
		}

		private void btnClose_Click(object sender, EventArgs e) { Close(); }

		public Action<IWin32Window> AfterClosed { get; private set; }

		private void llConfigureAutomaticUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			AfterClosed = form =>
			{
				using (OptionDialog dialog = new OptionDialog())
				{
					dialog.InitialPageName = "tpArchivers";
					dialog.ShowDialog(form);
				}
			};
			Close();
		}
	}
}
