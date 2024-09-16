namespace Comical
{
	partial class AboutDialog
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
			System.Windows.Forms.Label LibrariesLabel;
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
			lblVersionHeader = new System.Windows.Forms.Label();
			CopyrightLabel = new System.Windows.Forms.Label();
			LibrariesTextBox = new System.Windows.Forms.TextBox();
			MainPanel = new System.Windows.Forms.TableLayoutPanel();
			LibrariesLabel = new System.Windows.Forms.Label();
			MainPanel.SuspendLayout();
			SuspendLayout();
			// 
			// LibrariesLabel
			// 
			resources.ApplyResources(LibrariesLabel, "LibrariesLabel");
			LibrariesLabel.BackColor = System.Drawing.Color.Transparent;
			LibrariesLabel.Name = "LibrariesLabel";
			LibrariesLabel.Click += OnClick;
			// 
			// lblVersionHeader
			// 
			resources.ApplyResources(lblVersionHeader, "lblVersionHeader");
			lblVersionHeader.BackColor = System.Drawing.Color.Transparent;
			lblVersionHeader.ForeColor = System.Drawing.Color.White;
			lblVersionHeader.Name = "lblVersionHeader";
			lblVersionHeader.Click += OnClick;
			// 
			// CopyrightLabel
			// 
			resources.ApplyResources(CopyrightLabel, "CopyrightLabel");
			CopyrightLabel.BackColor = System.Drawing.Color.Transparent;
			CopyrightLabel.Name = "CopyrightLabel";
			// 
			// LibrariesTextBox
			// 
			resources.ApplyResources(LibrariesTextBox, "LibrariesTextBox");
			LibrariesTextBox.BackColor = System.Drawing.SystemColors.Window;
			LibrariesTextBox.Name = "LibrariesTextBox";
			LibrariesTextBox.ReadOnly = true;
			// 
			// MainPanel
			// 
			resources.ApplyResources(MainPanel, "MainPanel");
			MainPanel.BackColor = System.Drawing.Color.White;
			MainPanel.Controls.Add(LibrariesLabel, 0, 1);
			MainPanel.Controls.Add(CopyrightLabel, 0, 0);
			MainPanel.Controls.Add(LibrariesTextBox, 0, 2);
			MainPanel.Name = "MainPanel";
			MainPanel.Click += OnClick;
			// 
			// AboutDialog
			// 
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			Controls.Add(lblVersionHeader);
			Controls.Add(MainPanel);
			DoubleBuffered = true;
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "AboutDialog";
			ShowIcon = false;
			ShowInTaskbar = false;
			Click += OnClick;
			MainPanel.ResumeLayout(false);
			MainPanel.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		System.Windows.Forms.Label lblVersionHeader;
		private System.Windows.Forms.Label CopyrightLabel;
		private System.Windows.Forms.TextBox LibrariesTextBox;
		private System.Windows.Forms.TableLayoutPanel MainPanel;
	}
}