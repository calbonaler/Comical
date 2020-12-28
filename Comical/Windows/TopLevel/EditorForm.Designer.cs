namespace Comical
{
	partial class EditorForm
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
				_imageList.SetImages(null);
				_bookmarkList.SetBookmarks(null);
				_bookmarkList.SetImages(null);
				_document.SetComic(null);
				if (components != null)
					components.Dispose();
				_comic.Dispose();
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
			System.Windows.Forms.ToolStripMenuItem itmFile;
			System.Windows.Forms.ToolStripMenuItem itmNew;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorForm));
			System.Windows.Forms.ToolStripMenuItem itmOpen;
			System.Windows.Forms.ToolStripSeparator sepFile1;
			System.Windows.Forms.ToolStripMenuItem itmSave;
			System.Windows.Forms.ToolStripMenuItem itmSaveAs;
			System.Windows.Forms.ToolStripSeparator sepFile2;
			System.Windows.Forms.ToolStripMenuItem itmDocumentSettings;
			System.Windows.Forms.ToolStripSeparator sepFile3;
			System.Windows.Forms.ToolStripMenuItem itmExit;
			System.Windows.Forms.ToolStripMenuItem itmImage;
			System.Windows.Forms.ToolStripMenuItem itmAdd;
			System.Windows.Forms.ToolStripMenuItem itmFromFiles;
			System.Windows.Forms.ToolStripMenuItem itmFromFolder;
			System.Windows.Forms.ToolStripSeparator sepImage1;
			System.Windows.Forms.ToolStripSeparator sepImage2;
			System.Windows.Forms.ToolStripSeparator sepImage3;
			System.Windows.Forms.ToolStripMenuItem itmWithLeft;
			System.Windows.Forms.ToolStripMenuItem itmWithRight;
			System.Windows.Forms.ToolStripMenuItem itmBookmark;
			System.Windows.Forms.ToolStripMenuItem itmTool;
			System.Windows.Forms.ToolStripMenuItem itmOption;
			System.Windows.Forms.ToolStripMenuItem itmHelp;
			System.Windows.Forms.ToolStripMenuItem itmAbout;
			System.Windows.Forms.StatusStrip stsMain;
			System.Windows.Forms.ToolStripMenuItem itmView;
			System.Windows.Forms.ToolStripMenuItem itmContentsWindow;
			System.Windows.Forms.ToolStripMenuItem itmBookmarksWindow;
			System.Windows.Forms.ToolStripButton btnNew;
			System.Windows.Forms.ToolStripButton btnOpen;
			System.Windows.Forms.ToolStripButton btnSave;
			this.itmOpenImage = new System.Windows.Forms.ToolStripMenuItem();
			this.itmExclude = new System.Windows.Forms.ToolStripMenuItem();
			this.itmExport = new System.Windows.Forms.ToolStripMenuItem();
			this.itmExtract = new System.Windows.Forms.ToolStripMenuItem();
			this.itmSetViewMode = new System.Windows.Forms.ToolStripMenuItem();
			this.itmInvertViewMode = new System.Windows.Forms.ToolStripMenuItem();
			this.itmAddBookmark = new System.Windows.Forms.ToolStripMenuItem();
			this.itmDeleteBookmark = new System.Windows.Forms.ToolStripMenuItem();
			this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.prgStatus = new System.Windows.Forms.ToolStripProgressBar();
			this.lblImageCount = new System.Windows.Forms.ToolStripStatusLabel();
			this.menMain = new System.Windows.Forms.MenuStrip();
			this.tsMain = new System.Windows.Forms.ToolStrip();
			this.dpMain = new WeifenLuo.WinFormsUI.Docking.DockPanel();
			itmFile = new System.Windows.Forms.ToolStripMenuItem();
			itmNew = new System.Windows.Forms.ToolStripMenuItem();
			itmOpen = new System.Windows.Forms.ToolStripMenuItem();
			sepFile1 = new System.Windows.Forms.ToolStripSeparator();
			itmSave = new System.Windows.Forms.ToolStripMenuItem();
			itmSaveAs = new System.Windows.Forms.ToolStripMenuItem();
			sepFile2 = new System.Windows.Forms.ToolStripSeparator();
			itmDocumentSettings = new System.Windows.Forms.ToolStripMenuItem();
			sepFile3 = new System.Windows.Forms.ToolStripSeparator();
			itmExit = new System.Windows.Forms.ToolStripMenuItem();
			itmImage = new System.Windows.Forms.ToolStripMenuItem();
			itmAdd = new System.Windows.Forms.ToolStripMenuItem();
			itmFromFiles = new System.Windows.Forms.ToolStripMenuItem();
			itmFromFolder = new System.Windows.Forms.ToolStripMenuItem();
			sepImage1 = new System.Windows.Forms.ToolStripSeparator();
			sepImage2 = new System.Windows.Forms.ToolStripSeparator();
			sepImage3 = new System.Windows.Forms.ToolStripSeparator();
			itmWithLeft = new System.Windows.Forms.ToolStripMenuItem();
			itmWithRight = new System.Windows.Forms.ToolStripMenuItem();
			itmBookmark = new System.Windows.Forms.ToolStripMenuItem();
			itmTool = new System.Windows.Forms.ToolStripMenuItem();
			itmOption = new System.Windows.Forms.ToolStripMenuItem();
			itmHelp = new System.Windows.Forms.ToolStripMenuItem();
			itmAbout = new System.Windows.Forms.ToolStripMenuItem();
			stsMain = new System.Windows.Forms.StatusStrip();
			itmView = new System.Windows.Forms.ToolStripMenuItem();
			itmContentsWindow = new System.Windows.Forms.ToolStripMenuItem();
			itmBookmarksWindow = new System.Windows.Forms.ToolStripMenuItem();
			btnNew = new System.Windows.Forms.ToolStripButton();
			btnOpen = new System.Windows.Forms.ToolStripButton();
			btnSave = new System.Windows.Forms.ToolStripButton();
			stsMain.SuspendLayout();
			this.menMain.SuspendLayout();
			this.tsMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// itmFile
			// 
			itmFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            itmNew,
            itmOpen,
            sepFile1,
            itmSave,
            itmSaveAs,
            sepFile2,
            itmDocumentSettings,
            sepFile3,
            itmExit});
			itmFile.Name = "itmFile";
			resources.ApplyResources(itmFile, "itmFile");
			// 
			// itmNew
			// 
			resources.ApplyResources(itmNew, "itmNew");
			itmNew.Name = "itmNew";
			itmNew.Click += new System.EventHandler(this.itmNew_Click);
			// 
			// itmOpen
			// 
			resources.ApplyResources(itmOpen, "itmOpen");
			itmOpen.Name = "itmOpen";
			itmOpen.Click += new System.EventHandler(this.itmOpen_Click);
			// 
			// sepFile1
			// 
			sepFile1.Name = "sepFile1";
			resources.ApplyResources(sepFile1, "sepFile1");
			// 
			// itmSave
			// 
			resources.ApplyResources(itmSave, "itmSave");
			itmSave.Name = "itmSave";
			itmSave.Click += new System.EventHandler(this.itmSave_Click);
			// 
			// itmSaveAs
			// 
			itmSaveAs.Name = "itmSaveAs";
			resources.ApplyResources(itmSaveAs, "itmSaveAs");
			itmSaveAs.Click += new System.EventHandler(this.itmSaveAs_Click);
			// 
			// sepFile2
			// 
			sepFile2.Name = "sepFile2";
			resources.ApplyResources(sepFile2, "sepFile2");
			// 
			// itmDocumentSettings
			// 
			resources.ApplyResources(itmDocumentSettings, "itmDocumentSettings");
			itmDocumentSettings.Name = "itmDocumentSettings";
			itmDocumentSettings.Click += new System.EventHandler(this.itmDocumentSettings_Click);
			// 
			// sepFile3
			// 
			sepFile3.Name = "sepFile3";
			resources.ApplyResources(sepFile3, "sepFile3");
			// 
			// itmExit
			// 
			itmExit.Name = "itmExit";
			resources.ApplyResources(itmExit, "itmExit");
			itmExit.Click += new System.EventHandler(this.itmExit_Click);
			// 
			// itmImage
			// 
			itmImage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            itmAdd,
            sepImage1,
            this.itmOpenImage,
            this.itmExclude,
            sepImage2,
            this.itmExport,
            this.itmExtract,
            sepImage3,
            this.itmSetViewMode,
            this.itmInvertViewMode});
			itmImage.Name = "itmImage";
			resources.ApplyResources(itmImage, "itmImage");
			// 
			// itmAdd
			// 
			itmAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            itmFromFiles,
            itmFromFolder});
			itmAdd.Name = "itmAdd";
			resources.ApplyResources(itmAdd, "itmAdd");
			// 
			// itmFromFiles
			// 
			resources.ApplyResources(itmFromFiles, "itmFromFiles");
			itmFromFiles.Name = "itmFromFiles";
			itmFromFiles.Click += new System.EventHandler(this.itmFromFile_Click);
			// 
			// itmFromFolder
			// 
			resources.ApplyResources(itmFromFolder, "itmFromFolder");
			itmFromFolder.Name = "itmFromFolder";
			itmFromFolder.Click += new System.EventHandler(this.itmFromFolder_Click);
			// 
			// sepImage1
			// 
			sepImage1.Name = "sepImage1";
			resources.ApplyResources(sepImage1, "sepImage1");
			// 
			// itmOpenImage
			// 
			resources.ApplyResources(this.itmOpenImage, "itmOpenImage");
			this.itmOpenImage.Name = "itmOpenImage";
			this.itmOpenImage.Click += new System.EventHandler(this.itmOpenImage_Click);
			// 
			// itmExclude
			// 
			resources.ApplyResources(this.itmExclude, "itmExclude");
			this.itmExclude.Name = "itmExclude";
			this.itmExclude.Click += new System.EventHandler(this.itmDelete_Click);
			// 
			// sepImage2
			// 
			sepImage2.Name = "sepImage2";
			resources.ApplyResources(sepImage2, "sepImage2");
			// 
			// itmExport
			// 
			resources.ApplyResources(this.itmExport, "itmExport");
			this.itmExport.Name = "itmExport";
			this.itmExport.Click += new System.EventHandler(this.itmExport_Click);
			// 
			// itmExtract
			// 
			resources.ApplyResources(this.itmExtract, "itmExtract");
			this.itmExtract.Name = "itmExtract";
			this.itmExtract.Click += new System.EventHandler(this.itmExtract_Click);
			// 
			// sepImage3
			// 
			sepImage3.Name = "sepImage3";
			resources.ApplyResources(sepImage3, "sepImage3");
			// 
			// itmSetViewMode
			// 
			this.itmSetViewMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            itmWithLeft,
            itmWithRight});
			resources.ApplyResources(this.itmSetViewMode, "itmSetViewMode");
			this.itmSetViewMode.Name = "itmSetViewMode";
			// 
			// itmWithLeft
			// 
			itmWithLeft.Name = "itmWithLeft";
			resources.ApplyResources(itmWithLeft, "itmWithLeft");
			itmWithLeft.Click += new System.EventHandler(this.itmWithLeft_Click);
			// 
			// itmWithRight
			// 
			itmWithRight.Name = "itmWithRight";
			resources.ApplyResources(itmWithRight, "itmWithRight");
			itmWithRight.Click += new System.EventHandler(this.itmWithRight_Click);
			// 
			// itmInvertViewMode
			// 
			resources.ApplyResources(this.itmInvertViewMode, "itmInvertViewMode");
			this.itmInvertViewMode.Name = "itmInvertViewMode";
			this.itmInvertViewMode.Click += new System.EventHandler(this.itmInvertViewMode_Click);
			// 
			// itmBookmark
			// 
			itmBookmark.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmAddBookmark,
            this.itmDeleteBookmark});
			itmBookmark.Name = "itmBookmark";
			resources.ApplyResources(itmBookmark, "itmBookmark");
			// 
			// itmAddBookmark
			// 
			resources.ApplyResources(this.itmAddBookmark, "itmAddBookmark");
			this.itmAddBookmark.Name = "itmAddBookmark";
			this.itmAddBookmark.Click += new System.EventHandler(this.itmAddBookmark_Click);
			// 
			// itmDeleteBookmark
			// 
			resources.ApplyResources(this.itmDeleteBookmark, "itmDeleteBookmark");
			this.itmDeleteBookmark.Name = "itmDeleteBookmark";
			this.itmDeleteBookmark.Click += new System.EventHandler(this.itmDeleteBookmark_Click);
			// 
			// itmTool
			// 
			itmTool.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            itmOption});
			itmTool.Name = "itmTool";
			resources.ApplyResources(itmTool, "itmTool");
			// 
			// itmOption
			// 
			itmOption.Name = "itmOption";
			resources.ApplyResources(itmOption, "itmOption");
			itmOption.Click += new System.EventHandler(this.itmOption_Click);
			// 
			// itmHelp
			// 
			itmHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            itmAbout});
			itmHelp.Name = "itmHelp";
			resources.ApplyResources(itmHelp, "itmHelp");
			// 
			// itmAbout
			// 
			itmAbout.Name = "itmAbout";
			resources.ApplyResources(itmAbout, "itmAbout");
			itmAbout.Click += new System.EventHandler(this.itmAbout_Click);
			// 
			// stsMain
			// 
			stsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.prgStatus,
            this.lblImageCount});
			resources.ApplyResources(stsMain, "stsMain");
			stsMain.Name = "stsMain";
			stsMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			// 
			// lblStatus
			// 
			this.lblStatus.Name = "lblStatus";
			resources.ApplyResources(this.lblStatus, "lblStatus");
			this.lblStatus.Spring = true;
			// 
			// prgStatus
			// 
			this.prgStatus.Name = "prgStatus";
			resources.ApplyResources(this.prgStatus, "prgStatus");
			// 
			// lblImageCount
			// 
			resources.ApplyResources(this.lblImageCount, "lblImageCount");
			this.lblImageCount.Name = "lblImageCount";
			// 
			// itmView
			// 
			itmView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            itmContentsWindow,
            itmBookmarksWindow});
			itmView.Name = "itmView";
			resources.ApplyResources(itmView, "itmView");
			// 
			// itmContentsWindow
			// 
			resources.ApplyResources(itmContentsWindow, "itmContentsWindow");
			itmContentsWindow.Name = "itmContentsWindow";
			itmContentsWindow.Click += new System.EventHandler(this.itmContentsWindow_Click);
			// 
			// itmBookmarksWindow
			// 
			resources.ApplyResources(itmBookmarksWindow, "itmBookmarksWindow");
			itmBookmarksWindow.Name = "itmBookmarksWindow";
			itmBookmarksWindow.Click += new System.EventHandler(this.itmBookmarksWindow_Click);
			// 
			// btnNew
			// 
			btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(btnNew, "btnNew");
			btnNew.Name = "btnNew";
			btnNew.Click += new System.EventHandler(this.itmNew_Click);
			// 
			// btnOpen
			// 
			btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(btnOpen, "btnOpen");
			btnOpen.Name = "btnOpen";
			btnOpen.Click += new System.EventHandler(this.itmOpen_Click);
			// 
			// btnSave
			// 
			btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(btnSave, "btnSave");
			btnSave.Name = "btnSave";
			btnSave.Click += new System.EventHandler(this.itmSave_Click);
			// 
			// menMain
			// 
			this.menMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            itmFile,
            itmView,
            itmImage,
            itmBookmark,
            itmTool,
            itmHelp});
			resources.ApplyResources(this.menMain, "menMain");
			this.menMain.Name = "menMain";
			this.menMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			// 
			// tsMain
			// 
			this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            btnNew,
            btnOpen,
            btnSave});
			resources.ApplyResources(this.tsMain, "tsMain");
			this.tsMain.Name = "tsMain";
			this.tsMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			// 
			// dpMain
			// 
			resources.ApplyResources(this.dpMain, "dpMain");
			this.dpMain.DockBackColor = System.Drawing.SystemColors.Control;
			this.dpMain.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
			this.dpMain.Name = "dpMain";
			// 
			// EditorForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.dpMain);
			this.Controls.Add(stsMain);
			this.Controls.Add(this.tsMain);
			this.Controls.Add(this.menMain);
			this.MainMenuStrip = this.menMain;
			this.Name = "EditorForm";
			stsMain.ResumeLayout(false);
			stsMain.PerformLayout();
			this.menMain.ResumeLayout(false);
			this.menMain.PerformLayout();
			this.tsMain.ResumeLayout(false);
			this.tsMain.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		System.Windows.Forms.ToolStripStatusLabel lblStatus;
		System.Windows.Forms.ToolStripStatusLabel lblImageCount;
		System.Windows.Forms.ToolStripProgressBar prgStatus;
		System.Windows.Forms.ToolStripMenuItem itmExclude;
		System.Windows.Forms.ToolStripMenuItem itmDeleteBookmark;
		System.Windows.Forms.ToolStripMenuItem itmAddBookmark;
		private System.Windows.Forms.ToolStrip tsMain;
		private System.Windows.Forms.ToolStripMenuItem itmExport;
		private System.Windows.Forms.ToolStripMenuItem itmExtract;
		private System.Windows.Forms.MenuStrip menMain;
		private WeifenLuo.WinFormsUI.Docking.DockPanel dpMain;
		private System.Windows.Forms.ToolStripMenuItem itmOpenImage;
		private System.Windows.Forms.ToolStripMenuItem itmSetViewMode;
		private System.Windows.Forms.ToolStripMenuItem itmInvertViewMode;
	}
}