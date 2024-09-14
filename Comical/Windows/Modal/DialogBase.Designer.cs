namespace Comical
{
	partial class DialogBase
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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
		private void InitializeComponent()
		{
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogBase));
			lblDescription = new System.Windows.Forms.Label();
			SuspendLayout();
			// 
			// lblDescription
			// 
			resources.ApplyResources(lblDescription, "lblDescription");
			lblDescription.BackColor = System.Drawing.Color.Transparent;
			lblDescription.ForeColor = System.Drawing.Color.FromArgb(0, 51, 153);
			lblDescription.Name = "lblDescription";
			// 
			// DialogBase
			// 
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			Controls.Add(lblDescription);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "DialogBase";
			ShowIcon = false;
			ShowInTaskbar = false;
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private System.Windows.Forms.Label lblDescription;
	}
}