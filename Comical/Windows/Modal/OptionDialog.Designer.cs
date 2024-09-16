namespace Comical
{
	partial class OptionDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.Windows.Forms.Button OKButton;
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionDialog));
			System.Windows.Forms.Button CancelButton;
			System.Windows.Forms.Label DefaultSavedFileNameLabel;
			System.Windows.Forms.ToolStripMenuItem TitleMenuItem;
			System.Windows.Forms.ToolStripMenuItem AuthorMenuItem;
			System.Windows.Forms.ToolStripMenuItem FormattedDateMenuItem;
			InsertMaskButton = new System.Windows.Forms.Button();
			DefaultSavedFileNameTextBox = new System.Windows.Forms.TextBox();
			InsertMaskContextMenu = new System.Windows.Forms.ContextMenuStrip(components);
			OKButton = new System.Windows.Forms.Button();
			CancelButton = new System.Windows.Forms.Button();
			DefaultSavedFileNameLabel = new System.Windows.Forms.Label();
			TitleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			AuthorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			FormattedDateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			InsertMaskContextMenu.SuspendLayout();
			SuspendLayout();
			// 
			// OKButton
			// 
			resources.ApplyResources(OKButton, "OKButton");
			OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			OKButton.Name = "OKButton";
			OKButton.UseVisualStyleBackColor = true;
			OKButton.Click += OnOKButtonClick;
			// 
			// CancelButton
			// 
			resources.ApplyResources(CancelButton, "CancelButton");
			CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			CancelButton.Name = "CancelButton";
			CancelButton.UseVisualStyleBackColor = true;
			// 
			// DefaultSavedFileNameLabel
			// 
			resources.ApplyResources(DefaultSavedFileNameLabel, "DefaultSavedFileNameLabel");
			DefaultSavedFileNameLabel.Name = "DefaultSavedFileNameLabel";
			// 
			// TitleMenuItem
			// 
			TitleMenuItem.Name = "TitleMenuItem";
			resources.ApplyResources(TitleMenuItem, "TitleMenuItem");
			TitleMenuItem.Tag = "";
			TitleMenuItem.Click += OnInsertMaskMenuItemsClick;
			// 
			// AuthorMenuItem
			// 
			AuthorMenuItem.Name = "AuthorMenuItem";
			resources.ApplyResources(AuthorMenuItem, "AuthorMenuItem");
			AuthorMenuItem.Tag = "";
			AuthorMenuItem.Click += OnInsertMaskMenuItemsClick;
			// 
			// FormattedDateMenuItem
			// 
			FormattedDateMenuItem.Name = "FormattedDateMenuItem";
			resources.ApplyResources(FormattedDateMenuItem, "FormattedDateMenuItem");
			FormattedDateMenuItem.Tag = "";
			FormattedDateMenuItem.Click += OnInsertMaskMenuItemsClick;
			// 
			// InsertMaskButton
			// 
			resources.ApplyResources(InsertMaskButton, "InsertMaskButton");
			InsertMaskButton.Name = "InsertMaskButton";
			InsertMaskButton.UseVisualStyleBackColor = true;
			InsertMaskButton.Click += OnInsertMaskButtonClick;
			// 
			// DefaultSavedFileNameTextBox
			// 
			resources.ApplyResources(DefaultSavedFileNameTextBox, "DefaultSavedFileNameTextBox");
			DefaultSavedFileNameTextBox.Name = "DefaultSavedFileNameTextBox";
			// 
			// InsertMaskContextMenu
			// 
			InsertMaskContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { TitleMenuItem, AuthorMenuItem, FormattedDateMenuItem });
			InsertMaskContextMenu.Name = "conInsertMask";
			InsertMaskContextMenu.ShowImageMargin = false;
			resources.ApplyResources(InsertMaskContextMenu, "InsertMaskContextMenu");
			// 
			// OptionDialog
			// 
			AcceptButton = OKButton;
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = CancelButton;
			Controls.Add(InsertMaskButton);
			Controls.Add(DefaultSavedFileNameTextBox);
			Controls.Add(DefaultSavedFileNameLabel);
			Controls.Add(CancelButton);
			Controls.Add(OKButton);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "OptionDialog";
			ShowIcon = false;
			ShowInTaskbar = false;
			InsertMaskContextMenu.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		System.Windows.Forms.TextBox DefaultSavedFileNameTextBox;
		System.Windows.Forms.Button InsertMaskButton;
		System.Windows.Forms.ContextMenuStrip InsertMaskContextMenu;
	}
}