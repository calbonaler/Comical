namespace Comical
{
	partial class DocumentView
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		void InitializeComponent()
		{
			System.Windows.Forms.SplitContainer splMain;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentView));
			System.Windows.Forms.Label lblCultureDependingDateOfPublication;
			System.Windows.Forms.Label lblTitle;
			System.Windows.Forms.Label lblAuthor;
			System.Windows.Forms.Label lblDateOfPublication;
			System.Windows.Forms.Label lblPageTurningDirection;
			System.Windows.Forms.Button btnSearchOnBrowser;
			System.Windows.Forms.Button btnEdit;
			this.txtCultureDependingDateOfPublication = new System.Windows.Forms.TextBox();
			this.txtTitle = new System.Windows.Forms.TextBox();
			this.cmbAuthor = new System.Windows.Forms.ComboBox();
			this.cmbPageTurningDirection = new System.Windows.Forms.ComboBox();
			this.dtpDateOfPublication = new System.Windows.Forms.DateTimePicker();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.numThumbnailIndex = new System.Windows.Forms.NumericUpDown();
			this.preThumbnail = new Comical.Controls.Previewer();
			this.lblThumbnail = new System.Windows.Forms.Label();
			this.lblSize = new System.Windows.Forms.Label();
			splMain = new System.Windows.Forms.SplitContainer();
			lblCultureDependingDateOfPublication = new System.Windows.Forms.Label();
			lblTitle = new System.Windows.Forms.Label();
			lblAuthor = new System.Windows.Forms.Label();
			lblDateOfPublication = new System.Windows.Forms.Label();
			lblPageTurningDirection = new System.Windows.Forms.Label();
			btnSearchOnBrowser = new System.Windows.Forms.Button();
			btnEdit = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(splMain)).BeginInit();
			splMain.Panel1.SuspendLayout();
			splMain.Panel2.SuspendLayout();
			splMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numThumbnailIndex)).BeginInit();
			this.SuspendLayout();
			// 
			// splMain
			// 
			resources.ApplyResources(splMain, "splMain");
			splMain.Name = "splMain";
			// 
			// splMain.Panel1
			// 
			splMain.Panel1.Controls.Add(lblCultureDependingDateOfPublication);
			splMain.Panel1.Controls.Add(this.txtCultureDependingDateOfPublication);
			splMain.Panel1.Controls.Add(lblTitle);
			splMain.Panel1.Controls.Add(this.txtTitle);
			splMain.Panel1.Controls.Add(lblAuthor);
			splMain.Panel1.Controls.Add(lblDateOfPublication);
			splMain.Panel1.Controls.Add(lblPageTurningDirection);
			splMain.Panel1.Controls.Add(this.cmbAuthor);
			splMain.Panel1.Controls.Add(this.cmbPageTurningDirection);
			splMain.Panel1.Controls.Add(btnSearchOnBrowser);
			splMain.Panel1.Controls.Add(this.dtpDateOfPublication);
			// 
			// splMain.Panel2
			// 
			splMain.Panel2.Controls.Add(this.btnUpdate);
			splMain.Panel2.Controls.Add(this.numThumbnailIndex);
			splMain.Panel2.Controls.Add(this.preThumbnail);
			splMain.Panel2.Controls.Add(this.lblThumbnail);
			splMain.Panel2.Controls.Add(this.lblSize);
			splMain.Panel2.Controls.Add(btnEdit);
			// 
			// lblCultureDependingDateOfPublication
			// 
			resources.ApplyResources(lblCultureDependingDateOfPublication, "lblCultureDependingDateOfPublication");
			lblCultureDependingDateOfPublication.Name = "lblCultureDependingDateOfPublication";
			// 
			// txtCultureDependingDateOfPublication
			// 
			resources.ApplyResources(this.txtCultureDependingDateOfPublication, "txtCultureDependingDateOfPublication");
			this.txtCultureDependingDateOfPublication.Name = "txtCultureDependingDateOfPublication";
			this.txtCultureDependingDateOfPublication.TextChanged += new System.EventHandler(this.txtCultureDependingDateOfIssue_TextChanged);
			// 
			// lblTitle
			// 
			resources.ApplyResources(lblTitle, "lblTitle");
			lblTitle.Name = "lblTitle";
			// 
			// txtTitle
			// 
			resources.ApplyResources(this.txtTitle, "txtTitle");
			this.txtTitle.Name = "txtTitle";
			this.txtTitle.TextChanged += new System.EventHandler(this.txtTitle_TextChanged);
			// 
			// lblAuthor
			// 
			resources.ApplyResources(lblAuthor, "lblAuthor");
			lblAuthor.Name = "lblAuthor";
			// 
			// lblDateOfPublication
			// 
			resources.ApplyResources(lblDateOfPublication, "lblDateOfPublication");
			lblDateOfPublication.Name = "lblDateOfPublication";
			// 
			// lblPageTurningDirection
			// 
			resources.ApplyResources(lblPageTurningDirection, "lblPageTurningDirection");
			lblPageTurningDirection.Name = "lblPageTurningDirection";
			// 
			// cmbAuthor
			// 
			resources.ApplyResources(this.cmbAuthor, "cmbAuthor");
			this.cmbAuthor.DropDownHeight = 256;
			this.cmbAuthor.FormattingEnabled = true;
			this.cmbAuthor.Name = "cmbAuthor";
			this.cmbAuthor.TextChanged += new System.EventHandler(this.cmbAuthor_TextChanged);
			// 
			// cmbPageTurningDirection
			// 
			resources.ApplyResources(this.cmbPageTurningDirection, "cmbPageTurningDirection");
			this.cmbPageTurningDirection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbPageTurningDirection.FormattingEnabled = true;
			this.cmbPageTurningDirection.Items.AddRange(new object[] {
            resources.GetString("cmbPageTurningDirection.Items"),
            resources.GetString("cmbPageTurningDirection.Items1")});
			this.cmbPageTurningDirection.Name = "cmbPageTurningDirection";
			this.cmbPageTurningDirection.SelectedIndexChanged += new System.EventHandler(this.cmbPageTurningDirection_SelectedIndexChanged);
			// 
			// btnSearchOnBrowser
			// 
			resources.ApplyResources(btnSearchOnBrowser, "btnSearchOnBrowser");
			btnSearchOnBrowser.Name = "btnSearchOnBrowser";
			btnSearchOnBrowser.UseVisualStyleBackColor = true;
			btnSearchOnBrowser.Click += new System.EventHandler(this.btnSearchOnBrowser_Click);
			// 
			// dtpDateOfPublication
			// 
			resources.ApplyResources(this.dtpDateOfPublication, "dtpDateOfPublication");
			this.dtpDateOfPublication.Checked = false;
			this.dtpDateOfPublication.Name = "dtpDateOfPublication";
			this.dtpDateOfPublication.ShowCheckBox = true;
			this.dtpDateOfPublication.ValueChanged += new System.EventHandler(this.dtpDateOfIssue_ValueChanged);
			// 
			// btnUpdate
			// 
			resources.ApplyResources(this.btnUpdate, "btnUpdate");
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.UseVisualStyleBackColor = true;
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			// 
			// numThumbnailIndex
			// 
			resources.ApplyResources(this.numThumbnailIndex, "numThumbnailIndex");
			this.numThumbnailIndex.Name = "numThumbnailIndex";
			// 
			// preThumbnail
			// 
			resources.ApplyResources(this.preThumbnail, "preThumbnail");
			this.preThumbnail.Description = null;
			this.preThumbnail.Image = null;
			this.preThumbnail.Name = "preThumbnail";
			this.preThumbnail.StretchMode = Comical.Controls.PreviewerStretchMode.Uniform;
			// 
			// lblThumbnail
			// 
			resources.ApplyResources(this.lblThumbnail, "lblThumbnail");
			this.lblThumbnail.Name = "lblThumbnail";
			// 
			// lblSize
			// 
			resources.ApplyResources(this.lblSize, "lblSize");
			this.lblSize.Name = "lblSize";
			// 
			// btnEdit
			// 
			resources.ApplyResources(btnEdit, "btnEdit");
			btnEdit.Name = "btnEdit";
			btnEdit.UseVisualStyleBackColor = true;
			btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// DocumentView
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(splMain);
			this.HideOnClose = true;
			this.Name = "DocumentView";
			splMain.Panel1.ResumeLayout(false);
			splMain.Panel1.PerformLayout();
			splMain.Panel2.ResumeLayout(false);
			splMain.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(splMain)).EndInit();
			splMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numThumbnailIndex)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		System.Windows.Forms.TextBox txtTitle;
		System.Windows.Forms.ComboBox cmbAuthor;
		System.Windows.Forms.DateTimePicker dtpDateOfPublication;
		System.Windows.Forms.Label lblSize;
		private System.Windows.Forms.ComboBox cmbPageTurningDirection;
		private System.Windows.Forms.Label lblThumbnail;
		private Controls.Previewer preThumbnail;
		private System.Windows.Forms.NumericUpDown numThumbnailIndex;
		private System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.TextBox txtCultureDependingDateOfPublication;
	}
}