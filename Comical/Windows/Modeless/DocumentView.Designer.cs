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
			System.Windows.Forms.TableLayoutPanel tplPanel1;
			System.Windows.Forms.Label lblBindingSide;
			System.Windows.Forms.Label lblCultureDependingPublished;
			System.Windows.Forms.Label lblTitle;
			System.Windows.Forms.Label lblPublished;
			System.Windows.Forms.Label lblAuthor;
			System.Windows.Forms.Button btnSearchOnBrowser;
			System.Windows.Forms.TableLayoutPanel tplPanel2;
			System.Windows.Forms.Button btnEdit;
			this.cmbBindingSide = new System.Windows.Forms.ComboBox();
			this.txtCultureDependingPublished = new System.Windows.Forms.TextBox();
			this.txtTitle = new System.Windows.Forms.TextBox();
			this.cmbAuthor = new System.Windows.Forms.ComboBox();
			this.dtpPublished = new System.Windows.Forms.DateTimePicker();
			this.lblThumbnail = new System.Windows.Forms.Label();
			this.preThumbnail = new Comical.Controls.Previewer();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.lblSize = new System.Windows.Forms.Label();
			this.numThumbnailIndex = new System.Windows.Forms.NumericUpDown();
			splMain = new System.Windows.Forms.SplitContainer();
			tplPanel1 = new System.Windows.Forms.TableLayoutPanel();
			lblBindingSide = new System.Windows.Forms.Label();
			lblCultureDependingPublished = new System.Windows.Forms.Label();
			lblTitle = new System.Windows.Forms.Label();
			lblPublished = new System.Windows.Forms.Label();
			lblAuthor = new System.Windows.Forms.Label();
			btnSearchOnBrowser = new System.Windows.Forms.Button();
			tplPanel2 = new System.Windows.Forms.TableLayoutPanel();
			btnEdit = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(splMain)).BeginInit();
			splMain.Panel1.SuspendLayout();
			splMain.Panel2.SuspendLayout();
			splMain.SuspendLayout();
			tplPanel1.SuspendLayout();
			tplPanel2.SuspendLayout();
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
			resources.ApplyResources(splMain.Panel1, "splMain.Panel1");
			splMain.Panel1.Controls.Add(tplPanel1);
			// 
			// splMain.Panel2
			// 
			resources.ApplyResources(splMain.Panel2, "splMain.Panel2");
			splMain.Panel2.Controls.Add(tplPanel2);
			// 
			// tplPanel1
			// 
			resources.ApplyResources(tplPanel1, "tplPanel1");
			tplPanel1.Controls.Add(this.cmbBindingSide, 0, 9);
			tplPanel1.Controls.Add(lblBindingSide, 0, 8);
			tplPanel1.Controls.Add(this.txtCultureDependingPublished, 0, 7);
			tplPanel1.Controls.Add(lblCultureDependingPublished, 0, 6);
			tplPanel1.Controls.Add(lblTitle, 0, 0);
			tplPanel1.Controls.Add(this.txtTitle, 0, 1);
			tplPanel1.Controls.Add(lblPublished, 0, 4);
			tplPanel1.Controls.Add(lblAuthor, 0, 2);
			tplPanel1.Controls.Add(btnSearchOnBrowser, 1, 5);
			tplPanel1.Controls.Add(this.cmbAuthor, 0, 3);
			tplPanel1.Controls.Add(this.dtpPublished, 0, 5);
			tplPanel1.Name = "tplPanel1";
			// 
			// cmbBindingSide
			// 
			resources.ApplyResources(this.cmbBindingSide, "cmbBindingSide");
			tplPanel1.SetColumnSpan(this.cmbBindingSide, 2);
			this.cmbBindingSide.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbBindingSide.FormattingEnabled = true;
			this.cmbBindingSide.Items.AddRange(new object[] {
            resources.GetString("cmbBindingSide.Items"),
            resources.GetString("cmbBindingSide.Items1"),
            resources.GetString("cmbBindingSide.Items2")});
			this.cmbBindingSide.Name = "cmbBindingSide";
			this.cmbBindingSide.SelectedIndexChanged += new System.EventHandler(this.cmbBindingSide_SelectedIndexChanged);
			// 
			// lblBindingSide
			// 
			resources.ApplyResources(lblBindingSide, "lblBindingSide");
			tplPanel1.SetColumnSpan(lblBindingSide, 2);
			lblBindingSide.Name = "lblBindingSide";
			// 
			// txtCultureDependingPublished
			// 
			resources.ApplyResources(this.txtCultureDependingPublished, "txtCultureDependingPublished");
			tplPanel1.SetColumnSpan(this.txtCultureDependingPublished, 2);
			this.txtCultureDependingPublished.Name = "txtCultureDependingPublished";
			this.txtCultureDependingPublished.TextChanged += new System.EventHandler(this.txtCultureDependingPublished_TextChanged);
			// 
			// lblCultureDependingPublished
			// 
			resources.ApplyResources(lblCultureDependingPublished, "lblCultureDependingPublished");
			tplPanel1.SetColumnSpan(lblCultureDependingPublished, 2);
			lblCultureDependingPublished.Name = "lblCultureDependingPublished";
			// 
			// lblTitle
			// 
			resources.ApplyResources(lblTitle, "lblTitle");
			tplPanel1.SetColumnSpan(lblTitle, 2);
			lblTitle.Name = "lblTitle";
			// 
			// txtTitle
			// 
			resources.ApplyResources(this.txtTitle, "txtTitle");
			tplPanel1.SetColumnSpan(this.txtTitle, 2);
			this.txtTitle.Name = "txtTitle";
			this.txtTitle.TextChanged += new System.EventHandler(this.txtTitle_TextChanged);
			// 
			// lblPublished
			// 
			resources.ApplyResources(lblPublished, "lblPublished");
			tplPanel1.SetColumnSpan(lblPublished, 2);
			lblPublished.Name = "lblPublished";
			// 
			// lblAuthor
			// 
			resources.ApplyResources(lblAuthor, "lblAuthor");
			tplPanel1.SetColumnSpan(lblAuthor, 2);
			lblAuthor.Name = "lblAuthor";
			// 
			// btnSearchOnBrowser
			// 
			resources.ApplyResources(btnSearchOnBrowser, "btnSearchOnBrowser");
			btnSearchOnBrowser.Name = "btnSearchOnBrowser";
			btnSearchOnBrowser.UseVisualStyleBackColor = true;
			btnSearchOnBrowser.Click += new System.EventHandler(this.btnSearchOnBrowser_Click);
			// 
			// cmbAuthor
			// 
			resources.ApplyResources(this.cmbAuthor, "cmbAuthor");
			tplPanel1.SetColumnSpan(this.cmbAuthor, 2);
			this.cmbAuthor.DropDownHeight = 256;
			this.cmbAuthor.FormattingEnabled = true;
			this.cmbAuthor.Name = "cmbAuthor";
			this.cmbAuthor.TextChanged += new System.EventHandler(this.cmbAuthor_TextChanged);
			// 
			// dtpPublished
			// 
			resources.ApplyResources(this.dtpPublished, "dtpPublished");
			this.dtpPublished.Checked = false;
			this.dtpPublished.Name = "dtpPublished";
			this.dtpPublished.ShowCheckBox = true;
			this.dtpPublished.ValueChanged += new System.EventHandler(this.dtpPublished_ValueChanged);
			// 
			// tplPanel2
			// 
			resources.ApplyResources(tplPanel2, "tplPanel2");
			tplPanel2.Controls.Add(this.lblThumbnail, 0, 0);
			tplPanel2.Controls.Add(this.preThumbnail, 0, 2);
			tplPanel2.Controls.Add(this.btnUpdate, 1, 1);
			tplPanel2.Controls.Add(this.lblSize, 0, 3);
			tplPanel2.Controls.Add(this.numThumbnailIndex, 0, 1);
			tplPanel2.Controls.Add(btnEdit, 1, 3);
			tplPanel2.Name = "tplPanel2";
			// 
			// lblThumbnail
			// 
			resources.ApplyResources(this.lblThumbnail, "lblThumbnail");
			this.lblThumbnail.Name = "lblThumbnail";
			// 
			// preThumbnail
			// 
			resources.ApplyResources(this.preThumbnail, "preThumbnail");
			tplPanel2.SetColumnSpan(this.preThumbnail, 2);
			this.preThumbnail.Image = null;
			this.preThumbnail.Name = "preThumbnail";
			this.preThumbnail.StretchMode = Comical.Controls.PreviewerStretchMode.Uniform;
			// 
			// btnUpdate
			// 
			resources.ApplyResources(this.btnUpdate, "btnUpdate");
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.UseVisualStyleBackColor = true;
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			// 
			// lblSize
			// 
			resources.ApplyResources(this.lblSize, "lblSize");
			this.lblSize.Name = "lblSize";
			// 
			// numThumbnailIndex
			// 
			resources.ApplyResources(this.numThumbnailIndex, "numThumbnailIndex");
			this.numThumbnailIndex.Name = "numThumbnailIndex";
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
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(splMain);
			this.HideOnClose = true;
			this.Name = "DocumentView";
			splMain.Panel1.ResumeLayout(false);
			splMain.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(splMain)).EndInit();
			splMain.ResumeLayout(false);
			tplPanel1.ResumeLayout(false);
			tplPanel1.PerformLayout();
			tplPanel2.ResumeLayout(false);
			tplPanel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numThumbnailIndex)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		System.Windows.Forms.TextBox txtTitle;
		System.Windows.Forms.ComboBox cmbAuthor;
		System.Windows.Forms.DateTimePicker dtpPublished;
		System.Windows.Forms.Label lblSize;
		private System.Windows.Forms.ComboBox cmbBindingSide;
		private System.Windows.Forms.Label lblThumbnail;
		private Controls.Previewer preThumbnail;
		private System.Windows.Forms.NumericUpDown numThumbnailIndex;
		private System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.TextBox txtCultureDependingPublished;
	}
}