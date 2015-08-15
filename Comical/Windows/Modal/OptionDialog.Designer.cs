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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionDialog));
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnInsertMask = new System.Windows.Forms.Button();
			this.txtDefaultSavedFileName = new System.Windows.Forms.TextBox();
			this.lblDefaultSavedFileName = new System.Windows.Forms.Label();
			this.conInsertMask = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.itmTitle = new System.Windows.Forms.ToolStripMenuItem();
			this.itmAuthor = new System.Windows.Forms.ToolStripMenuItem();
			this.itmFormattedDate = new System.Windows.Forms.ToolStripMenuItem();
			this.conInsertMask.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnInsertMask
			// 
			resources.ApplyResources(this.btnInsertMask, "btnInsertMask");
			this.btnInsertMask.Name = "btnInsertMask";
			this.btnInsertMask.UseVisualStyleBackColor = true;
			this.btnInsertMask.Click += new System.EventHandler(this.btnInsertMask_Click);
			// 
			// txtDefaultSavedFileName
			// 
			resources.ApplyResources(this.txtDefaultSavedFileName, "txtDefaultSavedFileName");
			this.txtDefaultSavedFileName.Name = "txtDefaultSavedFileName";
			// 
			// lblDefaultSavedFileName
			// 
			resources.ApplyResources(this.lblDefaultSavedFileName, "lblDefaultSavedFileName");
			this.lblDefaultSavedFileName.Name = "lblDefaultSavedFileName";
			// 
			// conInsertMask
			// 
			this.conInsertMask.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmTitle,
            this.itmAuthor,
            this.itmFormattedDate});
			this.conInsertMask.Name = "conInsertMask";
			this.conInsertMask.ShowImageMargin = false;
			resources.ApplyResources(this.conInsertMask, "conInsertMask");
			// 
			// itmTitle
			// 
			this.itmTitle.Name = "itmTitle";
			resources.ApplyResources(this.itmTitle, "itmTitle");
			this.itmTitle.Tag = "";
			this.itmTitle.Click += new System.EventHandler(this.InsertMaskItem_Click);
			// 
			// itmAuthor
			// 
			this.itmAuthor.Name = "itmAuthor";
			resources.ApplyResources(this.itmAuthor, "itmAuthor");
			this.itmAuthor.Tag = "";
			this.itmAuthor.Click += new System.EventHandler(this.InsertMaskItem_Click);
			// 
			// itmFormattedDate
			// 
			this.itmFormattedDate.Name = "itmFormattedDate";
			resources.ApplyResources(this.itmFormattedDate, "itmFormattedDate");
			this.itmFormattedDate.Tag = "";
			this.itmFormattedDate.Click += new System.EventHandler(this.InsertMaskItem_Click);
			// 
			// OptionDialog
			// 
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnInsertMask);
			this.Controls.Add(this.txtDefaultSavedFileName);
			this.Controls.Add(this.lblDefaultSavedFileName);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.conInsertMask.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		System.Windows.Forms.TextBox txtDefaultSavedFileName;
		System.Windows.Forms.Label lblDefaultSavedFileName;
		System.Windows.Forms.Button btnInsertMask;
		System.Windows.Forms.ContextMenuStrip conInsertMask;
		System.Windows.Forms.ToolStripMenuItem itmTitle;
		System.Windows.Forms.ToolStripMenuItem itmAuthor;
		System.Windows.Forms.ToolStripMenuItem itmFormattedDate;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
	}
}