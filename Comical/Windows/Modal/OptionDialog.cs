using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Comical;

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
		DefaultSavedFileNameTextBox.Text = Properties.Settings.Default.DefaultSavedFileName;
	}

	void OnOKButtonClick(object? sender, EventArgs e)
	{
		Properties.Settings.Default.DefaultSavedFileName = DefaultSavedFileNameTextBox.Text;
		Properties.Settings.Default.Save();
	}

	void OnInsertMaskButtonClick(object? sender, EventArgs e) => InsertMaskContextMenu.Show(InsertMaskButton, 0, InsertMaskButton.Height);

	void OnInsertMaskMenuItemsClick(object? sender, EventArgs e)
	{
		var senderMenuItem = (ToolStripItem?)sender;
		Debug.Assert(senderMenuItem != null && senderMenuItem.Text != null);
		var insertedText = senderMenuItem.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
		var oldSelectionStart = DefaultSavedFileNameTextBox.SelectionStart;
		DefaultSavedFileNameTextBox.Text = DefaultSavedFileNameTextBox.Text.Insert(oldSelectionStart, insertedText);
		DefaultSavedFileNameTextBox.SelectionStart = oldSelectionStart + insertedText.Length;
		DefaultSavedFileNameTextBox.Focus();
	}
}
