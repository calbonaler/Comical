using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Comical
{
	public partial class OptionDialog : Form
	{
		public OptionDialog()
		{
			InitializeComponent();
			Font = SystemFonts.MessageBoxFont;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			txtDefaultSavedFileName.Text = Properties.Settings.Default.DefaultSavedFileName;
		}

		void btnOK_Click(object? sender, EventArgs e)
		{
			Properties.Settings.Default.DefaultSavedFileName = txtDefaultSavedFileName.Text;
			Properties.Settings.Default.Save();
		}

		void btnInsertMask_Click(object? sender, EventArgs e) => conInsertMask.Show(btnInsertMask, 0, btnInsertMask.Height);

		void InsertMaskItem_Click(object? sender, EventArgs e)
		{
			var senderMenuItem = (ToolStripItem?)sender;
			Debug.Assert(senderMenuItem != null && senderMenuItem.Text != null);
			var insertedText = senderMenuItem.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
			var oldSelectionStart = txtDefaultSavedFileName.SelectionStart;
			txtDefaultSavedFileName.Text = txtDefaultSavedFileName.Text.Insert(oldSelectionStart, insertedText);
			txtDefaultSavedFileName.SelectionStart = oldSelectionStart + insertedText.Length;
			txtDefaultSavedFileName.Focus();
		}
	}
}
