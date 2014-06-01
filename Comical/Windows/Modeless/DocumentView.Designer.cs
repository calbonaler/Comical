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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentView));
			this.splMain = new System.Windows.Forms.SplitContainer();
			this.lblCultureDependingDateOfPublication = new System.Windows.Forms.Label();
			this.txtCultureDependingDateOfPublication = new System.Windows.Forms.TextBox();
			this.lblTitle = new System.Windows.Forms.Label();
			this.txtTitle = new System.Windows.Forms.TextBox();
			this.lblAuthor = new System.Windows.Forms.Label();
			this.lblDateOfPublication = new System.Windows.Forms.Label();
			this.lblPageTurningDirection = new System.Windows.Forms.Label();
			this.cmbAuthor = new System.Windows.Forms.ComboBox();
			this.cmbPageTurningDirection = new System.Windows.Forms.ComboBox();
			this.btnSearchOnBrowser = new System.Windows.Forms.Button();
			this.dtpDateOfPublication = new System.Windows.Forms.DateTimePicker();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.numThumbnailIndex = new System.Windows.Forms.NumericUpDown();
			this.preThumbnail = new Comical.Controls.Previewer();
			this.lblThumbnail = new System.Windows.Forms.Label();
			this.lblSize = new System.Windows.Forms.Label();
			this.btnEdit = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.splMain)).BeginInit();
			this.splMain.Panel1.SuspendLayout();
			this.splMain.Panel2.SuspendLayout();
			this.splMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numThumbnailIndex)).BeginInit();
			this.SuspendLayout();
			// 
			// splMain
			// 
			resources.ApplyResources(this.splMain, "splMain");
			this.splMain.Name = "splMain";
			// 
			// splMain.Panel1
			// 
			resources.ApplyResources(this.splMain.Panel1, "splMain.Panel1");
			this.splMain.Panel1.Controls.Add(this.lblCultureDependingDateOfPublication);
			this.splMain.Panel1.Controls.Add(this.txtCultureDependingDateOfPublication);
			this.splMain.Panel1.Controls.Add(this.lblTitle);
			this.splMain.Panel1.Controls.Add(this.txtTitle);
			this.splMain.Panel1.Controls.Add(this.lblAuthor);
			this.splMain.Panel1.Controls.Add(this.lblDateOfPublication);
			this.splMain.Panel1.Controls.Add(this.lblPageTurningDirection);
			this.splMain.Panel1.Controls.Add(this.cmbAuthor);
			this.splMain.Panel1.Controls.Add(this.cmbPageTurningDirection);
			this.splMain.Panel1.Controls.Add(this.btnSearchOnBrowser);
			this.splMain.Panel1.Controls.Add(this.dtpDateOfPublication);
			// 
			// splMain.Panel2
			// 
			resources.ApplyResources(this.splMain.Panel2, "splMain.Panel2");
			this.splMain.Panel2.Controls.Add(this.btnUpdate);
			this.splMain.Panel2.Controls.Add(this.numThumbnailIndex);
			this.splMain.Panel2.Controls.Add(this.preThumbnail);
			this.splMain.Panel2.Controls.Add(this.lblThumbnail);
			this.splMain.Panel2.Controls.Add(this.lblSize);
			this.splMain.Panel2.Controls.Add(this.btnEdit);
			// 
			// lblCultureDependingDateOfPublication
			// 
			resources.ApplyResources(this.lblCultureDependingDateOfPublication, "lblCultureDependingDateOfPublication");
			this.lblCultureDependingDateOfPublication.Name = "lblCultureDependingDateOfPublication";
			// 
			// txtCultureDependingDateOfPublication
			// 
			resources.ApplyResources(this.txtCultureDependingDateOfPublication, "txtCultureDependingDateOfPublication");
			this.txtCultureDependingDateOfPublication.Name = "txtCultureDependingDateOfPublication";
			this.txtCultureDependingDateOfPublication.TextChanged += new System.EventHandler(this.txtCultureDependingDateOfIssue_TextChanged);
			// 
			// lblTitle
			// 
			resources.ApplyResources(this.lblTitle, "lblTitle");
			this.lblTitle.Name = "lblTitle";
			// 
			// txtTitle
			// 
			resources.ApplyResources(this.txtTitle, "txtTitle");
			this.txtTitle.Name = "txtTitle";
			this.txtTitle.TextChanged += new System.EventHandler(this.txtTitle_TextChanged);
			// 
			// lblAuthor
			// 
			resources.ApplyResources(this.lblAuthor, "lblAuthor");
			this.lblAuthor.Name = "lblAuthor";
			// 
			// lblDateOfPublication
			// 
			resources.ApplyResources(this.lblDateOfPublication, "lblDateOfPublication");
			this.lblDateOfPublication.Name = "lblDateOfPublication";
			// 
			// lblPageTurningDirection
			// 
			resources.ApplyResources(this.lblPageTurningDirection, "lblPageTurningDirection");
			this.lblPageTurningDirection.Name = "lblPageTurningDirection";
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
			resources.ApplyResources(this.btnSearchOnBrowser, "btnSearchOnBrowser");
			this.btnSearchOnBrowser.Name = "btnSearchOnBrowser";
			this.btnSearchOnBrowser.UseVisualStyleBackColor = true;
			this.btnSearchOnBrowser.Click += new System.EventHandler(this.btnSearchOnBrowser_Click);
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
			resources.ApplyResources(this.btnEdit, "btnEdit");
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.UseVisualStyleBackColor = true;
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// DocumentView
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.splMain);
			this.HideOnClose = true;
			this.Name = "DocumentView";
			this.splMain.Panel1.ResumeLayout(false);
			this.splMain.Panel1.PerformLayout();
			this.splMain.Panel2.ResumeLayout(false);
			this.splMain.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splMain)).EndInit();
			this.splMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numThumbnailIndex)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		System.Windows.Forms.TextBox txtTitle;
		System.Windows.Forms.ComboBox cmbAuthor;
		System.Windows.Forms.DateTimePicker dtpDateOfPublication;
		System.Windows.Forms.Label lblSize;
		private System.Windows.Forms.ComboBox cmbPageTurningDirection;
		private System.Windows.Forms.SplitContainer splMain;
		private System.Windows.Forms.Label lblThumbnail;
		private Controls.Previewer preThumbnail;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Label lblAuthor;
		private System.Windows.Forms.Label lblDateOfPublication;
		private System.Windows.Forms.Button btnEdit;
		private System.Windows.Forms.Button btnSearchOnBrowser;
		private System.Windows.Forms.Label lblPageTurningDirection;
		private System.Windows.Forms.NumericUpDown numThumbnailIndex;
		private System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.Label lblCultureDependingDateOfPublication;
		private System.Windows.Forms.TextBox txtCultureDependingDateOfPublication;
	}
}