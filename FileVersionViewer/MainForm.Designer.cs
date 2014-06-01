namespace FileVersionViewer
{
	partial class MainForm
	{
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.lblFolder = new System.Windows.Forms.Label();
			this.txtFolder = new System.Windows.Forms.TextBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.lblCicFiles = new System.Windows.Forms.Label();
			this.lvCicFiles = new System.Windows.Forms.ListView();
			this.clmFileName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.clmVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.btnView = new System.Windows.Forms.Button();
			this.btnOutput = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lblFolder
			// 
			resources.ApplyResources(this.lblFolder, "lblFolder");
			this.lblFolder.Name = "lblFolder";
			// 
			// txtFolder
			// 
			resources.ApplyResources(this.txtFolder, "txtFolder");
			this.txtFolder.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.txtFolder.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
			this.txtFolder.Name = "txtFolder";
			this.txtFolder.TextChanged += new System.EventHandler(this.txtFolder_TextChanged);
			// 
			// btnBrowse
			// 
			resources.ApplyResources(this.btnBrowse, "btnBrowse");
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// lblCicFiles
			// 
			resources.ApplyResources(this.lblCicFiles, "lblCicFiles");
			this.lblCicFiles.Name = "lblCicFiles";
			// 
			// lvCicFiles
			// 
			resources.ApplyResources(this.lvCicFiles, "lvCicFiles");
			this.lvCicFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmFileName,
            this.clmVersion});
			this.lvCicFiles.FullRowSelect = true;
			this.lvCicFiles.Name = "lvCicFiles";
			this.lvCicFiles.UseCompatibleStateImageBehavior = false;
			this.lvCicFiles.View = System.Windows.Forms.View.Details;
			// 
			// clmFileName
			// 
			resources.ApplyResources(this.clmFileName, "clmFileName");
			// 
			// clmVersion
			// 
			resources.ApplyResources(this.clmVersion, "clmVersion");
			// 
			// btnView
			// 
			resources.ApplyResources(this.btnView, "btnView");
			this.btnView.Name = "btnView";
			this.btnView.UseVisualStyleBackColor = true;
			this.btnView.Click += new System.EventHandler(this.btnView_Click);
			// 
			// btnOutput
			// 
			resources.ApplyResources(this.btnOutput, "btnOutput");
			this.btnOutput.Name = "btnOutput";
			this.btnOutput.UseVisualStyleBackColor = true;
			this.btnOutput.Click += new System.EventHandler(this.btnOutput_Click);
			// 
			// MainForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lvCicFiles);
			this.Controls.Add(this.lblCicFiles);
			this.Controls.Add(this.btnOutput);
			this.Controls.Add(this.btnView);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.txtFolder);
			this.Controls.Add(this.lblFolder);
			this.Name = "MainForm";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label lblFolder;
		private System.Windows.Forms.TextBox txtFolder;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.Label lblCicFiles;
		private System.Windows.Forms.ListView lvCicFiles;
		private System.Windows.Forms.ColumnHeader clmFileName;
		private System.Windows.Forms.ColumnHeader clmVersion;
		private System.Windows.Forms.Button btnView;
		private System.Windows.Forms.Button btnOutput;
	}
}

