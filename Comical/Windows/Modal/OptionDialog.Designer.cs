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
			System.Windows.Forms.Button btnOK;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionDialog));
			System.Windows.Forms.Button btnCancel;
			System.Windows.Forms.Label lblDefaultSavedFileName;
			System.Windows.Forms.ToolStripMenuItem itmTitle;
			System.Windows.Forms.ToolStripMenuItem itmAuthor;
			System.Windows.Forms.ToolStripMenuItem itmFormattedDate;
			this.btnInsertMask = new System.Windows.Forms.Button();
			this.txtDefaultSavedFileName = new System.Windows.Forms.TextBox();
			this.conInsertMask = new System.Windows.Forms.ContextMenuStrip(this.components);
			btnOK = new System.Windows.Forms.Button();
			btnCancel = new System.Windows.Forms.Button();
			lblDefaultSavedFileName = new System.Windows.Forms.Label();
			itmTitle = new System.Windows.Forms.ToolStripMenuItem();
			itmAuthor = new System.Windows.Forms.ToolStripMenuItem();
			itmFormattedDate = new System.Windows.Forms.ToolStripMenuItem();
			this.conInsertMask.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			resources.ApplyResources(btnOK, "btnOK");
			btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			btnOK.Name = "btnOK";
			btnOK.UseVisualStyleBackColor = true;
			btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			resources.ApplyResources(btnCancel, "btnCancel");
			btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			btnCancel.Name = "btnCancel";
			btnCancel.UseVisualStyleBackColor = true;
			// 
			// lblDefaultSavedFileName
			// 
			resources.ApplyResources(lblDefaultSavedFileName, "lblDefaultSavedFileName");
			lblDefaultSavedFileName.Name = "lblDefaultSavedFileName";
			// 
			// itmTitle
			// 
			itmTitle.Name = "itmTitle";
			resources.ApplyResources(itmTitle, "itmTitle");
			itmTitle.Tag = "";
			itmTitle.Click += new System.EventHandler(this.InsertMaskItem_Click);
			// 
			// itmAuthor
			// 
			itmAuthor.Name = "itmAuthor";
			resources.ApplyResources(itmAuthor, "itmAuthor");
			itmAuthor.Tag = "";
			itmAuthor.Click += new System.EventHandler(this.InsertMaskItem_Click);
			// 
			// itmFormattedDate
			// 
			itmFormattedDate.Name = "itmFormattedDate";
			resources.ApplyResources(itmFormattedDate, "itmFormattedDate");
			itmFormattedDate.Tag = "";
			itmFormattedDate.Click += new System.EventHandler(this.InsertMaskItem_Click);
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
			// conInsertMask
			// 
			this.conInsertMask.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            itmTitle,
            itmAuthor,
            itmFormattedDate});
			this.conInsertMask.Name = "conInsertMask";
			this.conInsertMask.ShowImageMargin = false;
			resources.ApplyResources(this.conInsertMask, "conInsertMask");
			// 
			// OptionDialog
			// 
			this.AcceptButton = btnOK;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Font = System.Drawing.SystemFonts.MessageBoxFont;
			this.CancelButton = btnCancel;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.btnInsertMask);
			this.Controls.Add(this.txtDefaultSavedFileName);
			this.Controls.Add(lblDefaultSavedFileName);
			this.Controls.Add(btnCancel);
			this.Controls.Add(btnOK);
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
		System.Windows.Forms.Button btnInsertMask;
		System.Windows.Forms.ContextMenuStrip conInsertMask;
	}
}