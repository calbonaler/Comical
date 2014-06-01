namespace Comical
{
	partial class GoToIndexView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GoToIndexView));
			this.lblIndex = new System.Windows.Forms.Label();
			this.numIndex = new System.Windows.Forms.NumericUpDown();
			this.btnOK = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numIndex)).BeginInit();
			this.SuspendLayout();
			// 
			// lblIndex
			// 
			resources.ApplyResources(this.lblIndex, "lblIndex");
			this.lblIndex.Name = "lblIndex";
			// 
			// numIndex
			// 
			resources.ApplyResources(this.numIndex, "numIndex");
			this.numIndex.Name = "numIndex";
			// 
			// btnOK
			// 
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			this.btnOK.UseVisualStyleBackColor = true;
			// 
			// GoToIndexView
			// 
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.numIndex);
			this.Controls.Add(this.lblIndex);
			this.HideOnClose = true;
			this.Name = "GoToIndexView";
			((System.ComponentModel.ISupportInitialize)(this.numIndex)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		System.Windows.Forms.Label lblIndex;
		System.Windows.Forms.NumericUpDown numIndex;
		System.Windows.Forms.Button btnOK;
	}
}