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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
			this.lblCopyright2 = new System.Windows.Forms.Label();
			this.lblCopyrightInformation = new System.Windows.Forms.Label();
			this.lblCopyright = new System.Windows.Forms.Label();
			this.lblVersionHeader = new System.Windows.Forms.Label();
			this.lblAllRightsReserved = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblCopyright2
			// 
			this.lblCopyright2.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.lblCopyright2, "lblCopyright2");
			this.lblCopyright2.Name = "lblCopyright2";
			this.lblCopyright2.Click += new System.EventHandler(this.SplashScreen_Click);
			// 
			// lblCopyrightInformation
			// 
			resources.ApplyResources(this.lblCopyrightInformation, "lblCopyrightInformation");
			this.lblCopyrightInformation.BackColor = System.Drawing.Color.Transparent;
			this.lblCopyrightInformation.Name = "lblCopyrightInformation";
			this.lblCopyrightInformation.Click += new System.EventHandler(this.SplashScreen_Click);
			// 
			// lblCopyright
			// 
			this.lblCopyright.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.lblCopyright, "lblCopyright");
			this.lblCopyright.Name = "lblCopyright";
			this.lblCopyright.Click += new System.EventHandler(this.SplashScreen_Click);
			// 
			// lblVersionHeader
			// 
			this.lblVersionHeader.BackColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.lblVersionHeader, "lblVersionHeader");
			this.lblVersionHeader.Name = "lblVersionHeader";
			this.lblVersionHeader.Click += new System.EventHandler(this.SplashScreen_Click);
			// 
			// lblAllRightsReserved
			// 
			resources.ApplyResources(this.lblAllRightsReserved, "lblAllRightsReserved");
			this.lblAllRightsReserved.BackColor = System.Drawing.Color.Transparent;
			this.lblAllRightsReserved.Name = "lblAllRightsReserved";
			// 
			// AboutDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblAllRightsReserved);
			this.Controls.Add(this.lblCopyrightInformation);
			this.Controls.Add(this.lblCopyright2);
			this.Controls.Add(this.lblCopyright);
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
		private System.Windows.Forms.Label lblCopyright2;
		private System.Windows.Forms.Label lblCopyrightInformation;
		private System.Windows.Forms.Label lblAllRightsReserved;
	}
}