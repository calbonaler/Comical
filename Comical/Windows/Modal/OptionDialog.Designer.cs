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
			System.Windows.Forms.Button btnOK;
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionDialog));
			System.Windows.Forms.Button btnCancel;
			System.Windows.Forms.Label lblDefaultSavedFileName;
			System.Windows.Forms.ToolStripMenuItem itmTitle;
			System.Windows.Forms.ToolStripMenuItem itmAuthor;
			System.Windows.Forms.ToolStripMenuItem itmFormattedDate;
			btnInsertMask = new System.Windows.Forms.Button();
			txtDefaultSavedFileName = new System.Windows.Forms.TextBox();
			conInsertMask = new System.Windows.Forms.ContextMenuStrip(components);
			btnOK = new System.Windows.Forms.Button();
			btnCancel = new System.Windows.Forms.Button();
			lblDefaultSavedFileName = new System.Windows.Forms.Label();
			itmTitle = new System.Windows.Forms.ToolStripMenuItem();
			itmAuthor = new System.Windows.Forms.ToolStripMenuItem();
			itmFormattedDate = new System.Windows.Forms.ToolStripMenuItem();
			conInsertMask.SuspendLayout();
			SuspendLayout();
			// 
			// btnOK
			// 
			resources.ApplyResources(btnOK, "btnOK");
			btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			btnOK.Name = "btnOK";
			btnOK.UseVisualStyleBackColor = true;
			btnOK.Click += btnOK_Click;
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
			itmTitle.Click += InsertMaskItem_Click;
			// 
			// itmAuthor
			// 
			itmAuthor.Name = "itmAuthor";
			resources.ApplyResources(itmAuthor, "itmAuthor");
			itmAuthor.Tag = "";
			itmAuthor.Click += InsertMaskItem_Click;
			// 
			// itmFormattedDate
			// 
			itmFormattedDate.Name = "itmFormattedDate";
			resources.ApplyResources(itmFormattedDate, "itmFormattedDate");
			itmFormattedDate.Tag = "";
			itmFormattedDate.Click += InsertMaskItem_Click;
			// 
			// btnInsertMask
			// 
			resources.ApplyResources(btnInsertMask, "btnInsertMask");
			btnInsertMask.Name = "btnInsertMask";
			btnInsertMask.UseVisualStyleBackColor = true;
			btnInsertMask.Click += btnInsertMask_Click;
			// 
			// txtDefaultSavedFileName
			// 
			resources.ApplyResources(txtDefaultSavedFileName, "txtDefaultSavedFileName");
			txtDefaultSavedFileName.Name = "txtDefaultSavedFileName";
			// 
			// conInsertMask
			// 
			conInsertMask.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { itmTitle, itmAuthor, itmFormattedDate });
			conInsertMask.Name = "conInsertMask";
			conInsertMask.ShowImageMargin = false;
			resources.ApplyResources(conInsertMask, "conInsertMask");
			// 
			// OptionDialog
			// 
			AcceptButton = btnOK;
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			CancelButton = btnCancel;
			Controls.Add(btnInsertMask);
			Controls.Add(txtDefaultSavedFileName);
			Controls.Add(lblDefaultSavedFileName);
			Controls.Add(btnCancel);
			Controls.Add(btnOK);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "OptionDialog";
			ShowIcon = false;
			ShowInTaskbar = false;
			conInsertMask.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		System.Windows.Forms.TextBox txtDefaultSavedFileName;
		System.Windows.Forms.Button btnInsertMask;
		System.Windows.Forms.ContextMenuStrip conInsertMask;
	}
}