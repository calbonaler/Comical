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
	public partial class OptionDialog : Form
	{
		public OptionDialog() { InitializeComponent(); }

		public string InitialPageName { get; set; }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			chkUsePageView.Checked = Properties.Settings.Default.UsePageView;
			txtDefaultSavedFileName.Text = Properties.Settings.Default.DefaultSavedFileName;
			chkCheckArchiverUpdateWhenStarted.Checked = Properties.Settings.Default.CheckArchiverUpdateWhenStarted;
			if (InitialPageName != null)
				tcContents.SelectTab(InitialPageName);
		}

		void btnOK_Click(object sender, EventArgs e)
		{
			Properties.Settings.Default.UsePageView = chkUsePageView.Checked;
			Properties.Settings.Default.DefaultSavedFileName = txtDefaultSavedFileName.Text;
			Properties.Settings.Default.CheckArchiverUpdateWhenStarted = chkCheckArchiverUpdateWhenStarted.Checked;
			Properties.Settings.Default.Save();
		}

		void btnInsertMask_Click(object sender, EventArgs e) { conInsertMask.Show(btnInsertMask, btnInsertMask.Width, 0); }

		void InsertMaskItem_Click(object sender, EventArgs e)
		{
			var senderMenuItem = sender as ToolStripItem;
			if (senderMenuItem != null)
			{
				txtDefaultSavedFileName.Text = txtDefaultSavedFileName.Text.Insert(txtDefaultSavedFileName.SelectionStart,
					senderMenuItem.Text.Split(new[] { "  " }, StringSplitOptions.RemoveEmptyEntries)[0]);
			}
		}
	}
}
