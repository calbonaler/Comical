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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
			this.lblVersionHeader = new System.Windows.Forms.Label();
			this.lblCopyright = new System.Windows.Forms.Label();
			this.txtLibraries = new System.Windows.Forms.TextBox();
			lblLibraries = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblLibraries
			// 
			resources.ApplyResources(lblLibraries, "lblLibraries");
			lblLibraries.BackColor = System.Drawing.Color.Transparent;
			lblLibraries.Name = "lblLibraries";
			lblLibraries.Click += new System.EventHandler(this.SplashScreen_Click);
			// 
			// lblVersionHeader
			// 
			resources.ApplyResources(this.lblVersionHeader, "lblVersionHeader");
			this.lblVersionHeader.BackColor = System.Drawing.Color.Transparent;
			this.lblVersionHeader.ForeColor = System.Drawing.Color.White;
			this.lblVersionHeader.Name = "lblVersionHeader";
			this.lblVersionHeader.Click += new System.EventHandler(this.SplashScreen_Click);
			// 
			// lblCopyright
			// 
			resources.ApplyResources(this.lblCopyright, "lblCopyright");
			this.lblCopyright.BackColor = System.Drawing.Color.Transparent;
			this.lblCopyright.Name = "lblCopyright";
			// 
			// txtLibraries
			// 
			resources.ApplyResources(this.txtLibraries, "txtLibraries");
			this.txtLibraries.BackColor = System.Drawing.SystemColors.Window;
			this.txtLibraries.Name = "txtLibraries";
			this.txtLibraries.ReadOnly = true;
			// 
			// AboutDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.txtLibraries);
			this.Controls.Add(this.lblCopyright);
			this.Controls.Add(lblLibraries);
			this.Controls.Add(this.lblVersionHeader);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Click += new System.EventHandler(this.SplashScreen_Click);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		System.Windows.Forms.Label lblVersionHeader;
		private System.Windows.Forms.Label lblCopyright;
		private System.Windows.Forms.TextBox txtLibraries;
	}
}