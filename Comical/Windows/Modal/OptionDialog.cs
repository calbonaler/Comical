using System;
using System.Windows.Forms;

namespace Comical
{
	public partial class OptionDialog : Form
	{
		public OptionDialog() { InitializeComponent(); }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			txtDefaultSavedFileName.Text = Properties.Settings.Default.DefaultSavedFileName;
		}

		void btnOK_Click(object sender, EventArgs e)
		{
			Properties.Settings.Default.DefaultSavedFileName = txtDefaultSavedFileName.Text;
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
