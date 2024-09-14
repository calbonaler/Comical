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
			System.Windows.Forms.Label lblLibraries;
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
			lblVersionHeader = new System.Windows.Forms.Label();
			lblCopyright = new System.Windows.Forms.Label();
			txtLibraries = new System.Windows.Forms.TextBox();
			tlpMain = new System.Windows.Forms.TableLayoutPanel();
			lblLibraries = new System.Windows.Forms.Label();
			tlpMain.SuspendLayout();
			SuspendLayout();
			// 
			// lblLibraries
			// 
			resources.ApplyResources(lblLibraries, "lblLibraries");
			lblLibraries.BackColor = System.Drawing.Color.Transparent;
			lblLibraries.Name = "lblLibraries";
			lblLibraries.Click += SplashScreen_Click;
			// 
			// lblVersionHeader
			// 
			resources.ApplyResources(lblVersionHeader, "lblVersionHeader");
			lblVersionHeader.BackColor = System.Drawing.Color.Transparent;
			lblVersionHeader.ForeColor = System.Drawing.Color.White;
			lblVersionHeader.Name = "lblVersionHeader";
			lblVersionHeader.Click += SplashScreen_Click;
			// 
			// lblCopyright
			// 
			resources.ApplyResources(lblCopyright, "lblCopyright");
			lblCopyright.BackColor = System.Drawing.Color.Transparent;
			lblCopyright.Name = "lblCopyright";
			// 
			// txtLibraries
			// 
			resources.ApplyResources(txtLibraries, "txtLibraries");
			txtLibraries.BackColor = System.Drawing.SystemColors.Window;
			txtLibraries.Name = "txtLibraries";
			txtLibraries.ReadOnly = true;
			// 
			// tlpMain
			// 
			resources.ApplyResources(tlpMain, "tlpMain");
			tlpMain.BackColor = System.Drawing.Color.White;
			tlpMain.Controls.Add(lblLibraries, 0, 1);
			tlpMain.Controls.Add(lblCopyright, 0, 0);
			tlpMain.Controls.Add(txtLibraries, 0, 2);
			tlpMain.Name = "tlpMain";
			tlpMain.Click += SplashScreen_Click;
			// 
			// AboutDialog
			// 
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			Controls.Add(lblVersionHeader);
			Controls.Add(tlpMain);
			DoubleBuffered = true;
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "AboutDialog";
			ShowIcon = false;
			ShowInTaskbar = false;
			Click += SplashScreen_Click;
			tlpMain.ResumeLayout(false);
			tlpMain.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		System.Windows.Forms.Label lblVersionHeader;
		private System.Windows.Forms.Label lblCopyright;
		private System.Windows.Forms.TextBox txtLibraries;
		private System.Windows.Forms.TableLayoutPanel tlpMain;
	}
}