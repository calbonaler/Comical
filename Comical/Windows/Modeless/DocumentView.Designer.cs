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
			if (disposing)
			{
				_comic.PropertyChanged -= Comic_PropertyChanged;
				_comic.Images.CollectionChanged -= ComicImageCollection_CollectionChanged;
				if (components != null)
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
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(DocumentView));
			System.Windows.Forms.TableLayoutPanel tplPanel1;
			System.Windows.Forms.Label lblBindingSide;
			System.Windows.Forms.Label lblCultureDependingPublished;
			System.Windows.Forms.Label lblTitle;
			System.Windows.Forms.Label lblPublished;
			System.Windows.Forms.Label lblAuthor;
			System.Windows.Forms.Button btnSearchOnBrowser;
			System.Windows.Forms.TableLayoutPanel tplPanel2;
			System.Windows.Forms.Button btnEdit;
			cmbBindingSide = new System.Windows.Forms.ComboBox();
			txtCultureDependingPublished = new System.Windows.Forms.TextBox();
			txtTitle = new System.Windows.Forms.TextBox();
			cmbAuthor = new System.Windows.Forms.ComboBox();
			dtpPublished = new System.Windows.Forms.DateTimePicker();
			lblThumbnail = new System.Windows.Forms.Label();
			preThumbnail = new Controls.Previewer();
			btnUpdate = new System.Windows.Forms.Button();
			lblSize = new System.Windows.Forms.Label();
			numThumbnailIndex = new System.Windows.Forms.NumericUpDown();
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
			((System.ComponentModel.ISupportInitialize)splMain).BeginInit();
			splMain.Panel1.SuspendLayout();
			splMain.Panel2.SuspendLayout();
			splMain.SuspendLayout();
			tplPanel1.SuspendLayout();
			tplPanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)numThumbnailIndex).BeginInit();
			SuspendLayout();
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
			tplPanel1.Controls.Add(cmbBindingSide, 0, 9);
			tplPanel1.Controls.Add(lblBindingSide, 0, 8);
			tplPanel1.Controls.Add(txtCultureDependingPublished, 0, 7);
			tplPanel1.Controls.Add(lblCultureDependingPublished, 0, 6);
			tplPanel1.Controls.Add(lblTitle, 0, 0);
			tplPanel1.Controls.Add(txtTitle, 0, 1);
			tplPanel1.Controls.Add(lblPublished, 0, 4);
			tplPanel1.Controls.Add(lblAuthor, 0, 2);
			tplPanel1.Controls.Add(btnSearchOnBrowser, 1, 5);
			tplPanel1.Controls.Add(cmbAuthor, 0, 3);
			tplPanel1.Controls.Add(dtpPublished, 0, 5);
			tplPanel1.Name = "tplPanel1";
			// 
			// cmbBindingSide
			// 
			resources.ApplyResources(cmbBindingSide, "cmbBindingSide");
			tplPanel1.SetColumnSpan(cmbBindingSide, 2);
			cmbBindingSide.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			cmbBindingSide.FormattingEnabled = true;
			cmbBindingSide.Items.AddRange(new object[] { resources.GetString("cmbBindingSide.Items"), resources.GetString("cmbBindingSide.Items1"), resources.GetString("cmbBindingSide.Items2") });
			cmbBindingSide.Name = "cmbBindingSide";
			cmbBindingSide.SelectedIndexChanged += cmbBindingSide_SelectedIndexChanged;
			// 
			// lblBindingSide
			// 
			resources.ApplyResources(lblBindingSide, "lblBindingSide");
			tplPanel1.SetColumnSpan(lblBindingSide, 2);
			lblBindingSide.Name = "lblBindingSide";
			// 
			// txtCultureDependingPublished
			// 
			resources.ApplyResources(txtCultureDependingPublished, "txtCultureDependingPublished");
			tplPanel1.SetColumnSpan(txtCultureDependingPublished, 2);
			txtCultureDependingPublished.Name = "txtCultureDependingPublished";
			txtCultureDependingPublished.TextChanged += txtCultureDependingPublished_TextChanged;
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
			resources.ApplyResources(txtTitle, "txtTitle");
			tplPanel1.SetColumnSpan(txtTitle, 2);
			txtTitle.Name = "txtTitle";
			txtTitle.TextChanged += txtTitle_TextChanged;
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
			btnSearchOnBrowser.Click += btnSearchOnBrowser_Click;
			// 
			// cmbAuthor
			// 
			resources.ApplyResources(cmbAuthor, "cmbAuthor");
			tplPanel1.SetColumnSpan(cmbAuthor, 2);
			cmbAuthor.DropDownHeight = 256;
			cmbAuthor.FormattingEnabled = true;
			cmbAuthor.Name = "cmbAuthor";
			cmbAuthor.TextChanged += cmbAuthor_TextChanged;
			// 
			// dtpPublished
			// 
			resources.ApplyResources(dtpPublished, "dtpPublished");
			dtpPublished.Checked = false;
			dtpPublished.Name = "dtpPublished";
			dtpPublished.ShowCheckBox = true;
			dtpPublished.ValueChanged += dtpPublished_ValueChanged;
			// 
			// tplPanel2
			// 
			resources.ApplyResources(tplPanel2, "tplPanel2");
			tplPanel2.Controls.Add(lblThumbnail, 0, 0);
			tplPanel2.Controls.Add(preThumbnail, 0, 2);
			tplPanel2.Controls.Add(btnUpdate, 1, 1);
			tplPanel2.Controls.Add(lblSize, 0, 3);
			tplPanel2.Controls.Add(numThumbnailIndex, 0, 1);
			tplPanel2.Controls.Add(btnEdit, 1, 3);
			tplPanel2.Name = "tplPanel2";
			// 
			// lblThumbnail
			// 
			resources.ApplyResources(lblThumbnail, "lblThumbnail");
			tplPanel2.SetColumnSpan(lblThumbnail, 2);
			lblThumbnail.Name = "lblThumbnail";
			// 
			// preThumbnail
			// 
			resources.ApplyResources(preThumbnail, "preThumbnail");
			tplPanel2.SetColumnSpan(preThumbnail, 2);
			preThumbnail.Name = "preThumbnail";
			preThumbnail.StretchMode = Comical.Controls.PreviewerStretchMode.Uniform;
			// 
			// btnUpdate
			// 
			resources.ApplyResources(btnUpdate, "btnUpdate");
			btnUpdate.Name = "btnUpdate";
			btnUpdate.UseVisualStyleBackColor = true;
			btnUpdate.Click += btnUpdate_Click;
			// 
			// lblSize
			// 
			resources.ApplyResources(lblSize, "lblSize");
			lblSize.Name = "lblSize";
			// 
			// numThumbnailIndex
			// 
			resources.ApplyResources(numThumbnailIndex, "numThumbnailIndex");
			numThumbnailIndex.Name = "numThumbnailIndex";
			// 
			// btnEdit
			// 
			resources.ApplyResources(btnEdit, "btnEdit");
			btnEdit.Name = "btnEdit";
			btnEdit.UseVisualStyleBackColor = true;
			btnEdit.Click += btnEdit_Click;
			// 
			// DocumentView
			// 
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			Controls.Add(splMain);
			HideOnClose = true;
			Name = "DocumentView";
			splMain.Panel1.ResumeLayout(false);
			splMain.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splMain).EndInit();
			splMain.ResumeLayout(false);
			tplPanel1.ResumeLayout(false);
			tplPanel1.PerformLayout();
			tplPanel2.ResumeLayout(false);
			tplPanel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)numThumbnailIndex).EndInit();
			ResumeLayout(false);
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