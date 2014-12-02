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
				if (components != null)
					components.Dispose();
				comic.Dispose();
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorForm));
			WeifenLuo.WinFormsUI.Docking.DockPanelSkin dockPanelSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPanelSkin();
			WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin autoHideStripSkin1 = new WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin();
			WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient1 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin dockPaneStripSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin();
			WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient dockPaneStripGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient2 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient2 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient3 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient dockPaneStripToolWindowGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient4 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient5 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient3 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient6 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient7 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
			this.itmFile = new System.Windows.Forms.ToolStripMenuItem();
			this.itmNew = new System.Windows.Forms.ToolStripMenuItem();
			this.itmOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.sepFile1 = new System.Windows.Forms.ToolStripSeparator();
			this.itmSave = new System.Windows.Forms.ToolStripMenuItem();
			this.itmSaveAs = new System.Windows.Forms.ToolStripMenuItem();
			this.sepFile2 = new System.Windows.Forms.ToolStripSeparator();
			this.itmDocumentSettings = new System.Windows.Forms.ToolStripMenuItem();
			this.sepFile3 = new System.Windows.Forms.ToolStripSeparator();
			this.itmExit = new System.Windows.Forms.ToolStripMenuItem();
			this.itmEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.itmSelectAll = new System.Windows.Forms.ToolStripMenuItem();
			this.itmInvertSelections = new System.Windows.Forms.ToolStripMenuItem();
			this.sepEdit = new System.Windows.Forms.ToolStripSeparator();
			this.itmGoToIndex = new System.Windows.Forms.ToolStripMenuItem();
			this.itmImage = new System.Windows.Forms.ToolStripMenuItem();
			this.itmAdd = new System.Windows.Forms.ToolStripMenuItem();
			this.itmFromFiles = new System.Windows.Forms.ToolStripMenuItem();
			this.itmFromFolder = new System.Windows.Forms.ToolStripMenuItem();
			this.sepAdd = new System.Windows.Forms.ToolStripSeparator();
			this.itmFromWebPage = new System.Windows.Forms.ToolStripMenuItem();
			this.sepImage1 = new System.Windows.Forms.ToolStripSeparator();
			this.itmOpenImage = new System.Windows.Forms.ToolStripMenuItem();
			this.itmExclude = new System.Windows.Forms.ToolStripMenuItem();
			this.sepImage2 = new System.Windows.Forms.ToolStripSeparator();
			this.itmExport = new System.Windows.Forms.ToolStripMenuItem();
			this.itmExtract = new System.Windows.Forms.ToolStripMenuItem();
			this.sepImage3 = new System.Windows.Forms.ToolStripSeparator();
			this.itmSetViewMode = new System.Windows.Forms.ToolStripMenuItem();
			this.itmInvertViewMode = new System.Windows.Forms.ToolStripMenuItem();
			this.itmBookmark = new System.Windows.Forms.ToolStripMenuItem();
			this.itmAddBookmark = new System.Windows.Forms.ToolStripMenuItem();
			this.itmRemoveBookmark = new System.Windows.Forms.ToolStripMenuItem();
			this.itmTool = new System.Windows.Forms.ToolStripMenuItem();
			this.itmArchivers = new System.Windows.Forms.ToolStripMenuItem();
			this.sepTool = new System.Windows.Forms.ToolStripSeparator();
			this.itmOption = new System.Windows.Forms.ToolStripMenuItem();
			this.itmHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.itmAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.stsMain = new System.Windows.Forms.StatusStrip();
			this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.prgStatus = new System.Windows.Forms.ToolStripProgressBar();
			this.lblImageCount = new System.Windows.Forms.ToolStripStatusLabel();
			this.menMain = new System.Windows.Forms.MenuStrip();
			this.itmView = new System.Windows.Forms.ToolStripMenuItem();
			this.itmContentsWindow = new System.Windows.Forms.ToolStripMenuItem();
			this.itmBookmarksWindow = new System.Windows.Forms.ToolStripMenuItem();
			this.tsMain = new System.Windows.Forms.ToolStrip();
			this.btnNew = new System.Windows.Forms.ToolStripButton();
			this.btnOpen = new System.Windows.Forms.ToolStripButton();
			this.btnSave = new System.Windows.Forms.ToolStripButton();
			this.dpMain = new WeifenLuo.WinFormsUI.Docking.DockPanel();
			this.stsMain.SuspendLayout();
			this.menMain.SuspendLayout();
			this.tsMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// itmFile
			// 
			resources.ApplyResources(this.itmFile, "itmFile");
			this.itmFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmNew,
            this.itmOpen,
            this.sepFile1,
            this.itmSave,
            this.itmSaveAs,
            this.sepFile2,
            this.itmDocumentSettings,
            this.sepFile3,
            this.itmExit});
			this.itmFile.Name = "itmFile";
			// 
			// itmNew
			// 
			resources.ApplyResources(this.itmNew, "itmNew");
			this.itmNew.Name = "itmNew";
			this.itmNew.Click += new System.EventHandler(this.itmNew_Click);
			// 
			// itmOpen
			// 
			resources.ApplyResources(this.itmOpen, "itmOpen");
			this.itmOpen.Name = "itmOpen";
			this.itmOpen.Click += new System.EventHandler(this.itmOpen_Click);
			// 
			// sepFile1
			// 
			resources.ApplyResources(this.sepFile1, "sepFile1");
			this.sepFile1.Name = "sepFile1";
			// 
			// itmSave
			// 
			resources.ApplyResources(this.itmSave, "itmSave");
			this.itmSave.Name = "itmSave";
			this.itmSave.Click += new System.EventHandler(this.itmSave_Click);
			// 
			// itmSaveAs
			// 
			resources.ApplyResources(this.itmSaveAs, "itmSaveAs");
			this.itmSaveAs.Name = "itmSaveAs";
			this.itmSaveAs.Click += new System.EventHandler(this.itmSaveAs_Click);
			// 
			// sepFile2
			// 
			resources.ApplyResources(this.sepFile2, "sepFile2");
			this.sepFile2.Name = "sepFile2";
			// 
			// itmDocumentSettings
			// 
			resources.ApplyResources(this.itmDocumentSettings, "itmDocumentSettings");
			this.itmDocumentSettings.Name = "itmDocumentSettings";
			this.itmDocumentSettings.Click += new System.EventHandler(this.itmDocumentSettings_Click);
			// 
			// sepFile3
			// 
			resources.ApplyResources(this.sepFile3, "sepFile3");
			this.sepFile3.Name = "sepFile3";
			// 
			// itmExit
			// 
			resources.ApplyResources(this.itmExit, "itmExit");
			this.itmExit.Name = "itmExit";
			this.itmExit.Click += new System.EventHandler(this.itmExit_Click);
			// 
			// itmEdit
			// 
			resources.ApplyResources(this.itmEdit, "itmEdit");
			this.itmEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmSelectAll,
            this.itmInvertSelections,
            this.sepEdit,
            this.itmGoToIndex});
			this.itmEdit.Name = "itmEdit";
			// 
			// itmSelectAll
			// 
			resources.ApplyResources(this.itmSelectAll, "itmSelectAll");
			this.itmSelectAll.Name = "itmSelectAll";
			this.itmSelectAll.Click += new System.EventHandler(this.itmSelectAll_Click);
			// 
			// itmInvertSelections
			// 
			resources.ApplyResources(this.itmInvertSelections, "itmInvertSelections");
			this.itmInvertSelections.Name = "itmInvertSelections";
			this.itmInvertSelections.Click += new System.EventHandler(this.itmInvertSelections_Click);
			// 
			// sepEdit
			// 
			resources.ApplyResources(this.sepEdit, "sepEdit");
			this.sepEdit.Name = "sepEdit";
			// 
			// itmGoToIndex
			// 
			resources.ApplyResources(this.itmGoToIndex, "itmGoToIndex");
			this.itmGoToIndex.Name = "itmGoToIndex";
			this.itmGoToIndex.Click += new System.EventHandler(this.itmGoToIndex_Click);
			// 
			// itmImage
			// 
			resources.ApplyResources(this.itmImage, "itmImage");
			this.itmImage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmAdd,
            this.sepImage1,
            this.itmOpenImage,
            this.itmExclude,
            this.sepImage2,
            this.itmExport,
            this.itmExtract,
            this.sepImage3,
            this.itmSetViewMode,
            this.itmInvertViewMode});
			this.itmImage.Name = "itmImage";
			// 
			// itmAdd
			// 
			resources.ApplyResources(this.itmAdd, "itmAdd");
			this.itmAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmFromFiles,
            this.itmFromFolder,
            this.sepAdd,
            this.itmFromWebPage});
			this.itmAdd.Name = "itmAdd";
			// 
			// itmFromFiles
			// 
			resources.ApplyResources(this.itmFromFiles, "itmFromFiles");
			this.itmFromFiles.Name = "itmFromFiles";
			this.itmFromFiles.Click += new System.EventHandler(this.itmFromFile_Click);
			// 
			// itmFromFolder
			// 
			resources.ApplyResources(this.itmFromFolder, "itmFromFolder");
			this.itmFromFolder.Name = "itmFromFolder";
			this.itmFromFolder.Click += new System.EventHandler(this.itmFromFolder_Click);
			// 
			// sepAdd
			// 
			resources.ApplyResources(this.sepAdd, "sepAdd");
			this.sepAdd.Name = "sepAdd";
			// 
			// itmFromWebPage
			// 
			resources.ApplyResources(this.itmFromWebPage, "itmFromWebPage");
			this.itmFromWebPage.Name = "itmFromWebPage";
			this.itmFromWebPage.Click += new System.EventHandler(this.itmByBrowser_Click);
			// 
			// sepImage1
			// 
			resources.ApplyResources(this.sepImage1, "sepImage1");
			this.sepImage1.Name = "sepImage1";
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
			resources.ApplyResources(this.sepImage2, "sepImage2");
			this.sepImage2.Name = "sepImage2";
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
			resources.ApplyResources(this.sepImage3, "sepImage3");
			this.sepImage3.Name = "sepImage3";
			// 
			// itmSetViewMode
			// 
			resources.ApplyResources(this.itmSetViewMode, "itmSetViewMode");
			this.itmSetViewMode.Name = "itmSetViewMode";
			this.itmSetViewMode.Click += new System.EventHandler(this.itmSetViewMode_Click);
			// 
			// itmInvertViewMode
			// 
			resources.ApplyResources(this.itmInvertViewMode, "itmInvertViewMode");
			this.itmInvertViewMode.Name = "itmInvertViewMode";
			this.itmInvertViewMode.Click += new System.EventHandler(this.itmInvertViewMode_Click);
			// 
			// itmBookmark
			// 
			resources.ApplyResources(this.itmBookmark, "itmBookmark");
			this.itmBookmark.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmAddBookmark,
            this.itmRemoveBookmark});
			this.itmBookmark.Name = "itmBookmark";
			// 
			// itmAddBookmark
			// 
			resources.ApplyResources(this.itmAddBookmark, "itmAddBookmark");
			this.itmAddBookmark.Name = "itmAddBookmark";
			this.itmAddBookmark.Click += new System.EventHandler(this.itmAddBookmark_Click);
			// 
			// itmRemoveBookmark
			// 
			resources.ApplyResources(this.itmRemoveBookmark, "itmRemoveBookmark");
			this.itmRemoveBookmark.Name = "itmRemoveBookmark";
			this.itmRemoveBookmark.Click += new System.EventHandler(this.itmDeleteBookmark_Click);
			// 
			// itmTool
			// 
			resources.ApplyResources(this.itmTool, "itmTool");
			this.itmTool.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmArchivers,
            this.sepTool,
            this.itmOption});
			this.itmTool.Name = "itmTool";
			// 
			// itmArchivers
			// 
			resources.ApplyResources(this.itmArchivers, "itmArchivers");
			this.itmArchivers.Name = "itmArchivers";
			this.itmArchivers.Click += new System.EventHandler(this.itmManageArchiver_Click);
			// 
			// sepTool
			// 
			resources.ApplyResources(this.sepTool, "sepTool");
			this.sepTool.Name = "sepTool";
			// 
			// itmOption
			// 
			resources.ApplyResources(this.itmOption, "itmOption");
			this.itmOption.Name = "itmOption";
			this.itmOption.Click += new System.EventHandler(this.itmOption_Click);
			// 
			// itmHelp
			// 
			resources.ApplyResources(this.itmHelp, "itmHelp");
			this.itmHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmAbout});
			this.itmHelp.Name = "itmHelp";
			// 
			// itmAbout
			// 
			resources.ApplyResources(this.itmAbout, "itmAbout");
			this.itmAbout.Name = "itmAbout";
			this.itmAbout.Click += new System.EventHandler(this.itmAbout_Click);
			// 
			// stsMain
			// 
			resources.ApplyResources(this.stsMain, "stsMain");
			this.stsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.prgStatus,
            this.lblImageCount});
			this.stsMain.Name = "stsMain";
			this.stsMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			// 
			// lblStatus
			// 
			resources.ApplyResources(this.lblStatus, "lblStatus");
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Spring = true;
			// 
			// prgStatus
			// 
			resources.ApplyResources(this.prgStatus, "prgStatus");
			this.prgStatus.Name = "prgStatus";
			// 
			// lblImageCount
			// 
			resources.ApplyResources(this.lblImageCount, "lblImageCount");
			this.lblImageCount.Name = "lblImageCount";
			// 
			// menMain
			// 
			resources.ApplyResources(this.menMain, "menMain");
			this.menMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmFile,
            this.itmEdit,
            this.itmView,
            this.itmImage,
            this.itmBookmark,
            this.itmTool,
            this.itmHelp});
			this.menMain.Name = "menMain";
			this.menMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			// 
			// itmView
			// 
			resources.ApplyResources(this.itmView, "itmView");
			this.itmView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmContentsWindow,
            this.itmBookmarksWindow});
			this.itmView.Name = "itmView";
			// 
			// itmContentsWindow
			// 
			resources.ApplyResources(this.itmContentsWindow, "itmContentsWindow");
			this.itmContentsWindow.Name = "itmContentsWindow";
			this.itmContentsWindow.Click += new System.EventHandler(this.itmContentsWindow_Click);
			// 
			// itmBookmarksWindow
			// 
			resources.ApplyResources(this.itmBookmarksWindow, "itmBookmarksWindow");
			this.itmBookmarksWindow.Name = "itmBookmarksWindow";
			this.itmBookmarksWindow.Click += new System.EventHandler(this.itmBookmarksWindow_Click);
			// 
			// tsMain
			// 
			resources.ApplyResources(this.tsMain, "tsMain");
			this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNew,
            this.btnOpen,
            this.btnSave});
			this.tsMain.Name = "tsMain";
			this.tsMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			// 
			// btnNew
			// 
			resources.ApplyResources(this.btnNew, "btnNew");
			this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnNew.Name = "btnNew";
			this.btnNew.Click += new System.EventHandler(this.itmNew_Click);
			// 
			// btnOpen
			// 
			resources.ApplyResources(this.btnOpen, "btnOpen");
			this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnOpen.Name = "btnOpen";
			this.btnOpen.Click += new System.EventHandler(this.itmOpen_Click);
			// 
			// btnSave
			// 
			resources.ApplyResources(this.btnSave, "btnSave");
			this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.btnSave.Name = "btnSave";
			this.btnSave.Click += new System.EventHandler(this.itmSave_Click);
			// 
			// dpMain
			// 
			resources.ApplyResources(this.dpMain, "dpMain");
			this.dpMain.DockBackColor = System.Drawing.SystemColors.Control;
			this.dpMain.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
			this.dpMain.Name = "dpMain";
			dockPanelGradient1.EndColor = System.Drawing.SystemColors.ControlLight;
			dockPanelGradient1.StartColor = System.Drawing.SystemColors.ControlLight;
			autoHideStripSkin1.DockStripGradient = dockPanelGradient1;
			tabGradient1.EndColor = System.Drawing.SystemColors.Control;
			tabGradient1.StartColor = System.Drawing.SystemColors.Control;
			tabGradient1.TextColor = System.Drawing.SystemColors.ControlDarkDark;
			autoHideStripSkin1.TabGradient = tabGradient1;
			autoHideStripSkin1.TextFont = new System.Drawing.Font("メイリオ", 9F);
			dockPanelSkin1.AutoHideStripSkin = autoHideStripSkin1;
			tabGradient2.EndColor = System.Drawing.SystemColors.ControlLightLight;
			tabGradient2.StartColor = System.Drawing.SystemColors.ControlLightLight;
			tabGradient2.TextColor = System.Drawing.SystemColors.ControlText;
			dockPaneStripGradient1.ActiveTabGradient = tabGradient2;
			dockPanelGradient2.EndColor = System.Drawing.SystemColors.Control;
			dockPanelGradient2.StartColor = System.Drawing.SystemColors.Control;
			dockPaneStripGradient1.DockStripGradient = dockPanelGradient2;
			tabGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
			tabGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
			tabGradient3.TextColor = System.Drawing.SystemColors.ControlText;
			dockPaneStripGradient1.InactiveTabGradient = tabGradient3;
			dockPaneStripSkin1.DocumentGradient = dockPaneStripGradient1;
			dockPaneStripSkin1.TextFont = new System.Drawing.Font("メイリオ", 9F);
			tabGradient4.EndColor = System.Drawing.SystemColors.ActiveCaption;
			tabGradient4.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
			tabGradient4.StartColor = System.Drawing.SystemColors.GradientActiveCaption;
			tabGradient4.TextColor = System.Drawing.SystemColors.ActiveCaptionText;
			dockPaneStripToolWindowGradient1.ActiveCaptionGradient = tabGradient4;
			tabGradient5.EndColor = System.Drawing.SystemColors.Control;
			tabGradient5.StartColor = System.Drawing.SystemColors.Control;
			tabGradient5.TextColor = System.Drawing.SystemColors.ControlText;
			dockPaneStripToolWindowGradient1.ActiveTabGradient = tabGradient5;
			dockPanelGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
			dockPanelGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
			dockPaneStripToolWindowGradient1.DockStripGradient = dockPanelGradient3;
			tabGradient6.EndColor = System.Drawing.SystemColors.InactiveCaption;
			tabGradient6.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
			tabGradient6.StartColor = System.Drawing.SystemColors.GradientInactiveCaption;
			tabGradient6.TextColor = System.Drawing.SystemColors.InactiveCaptionText;
			dockPaneStripToolWindowGradient1.InactiveCaptionGradient = tabGradient6;
			tabGradient7.EndColor = System.Drawing.Color.Transparent;
			tabGradient7.StartColor = System.Drawing.Color.Transparent;
			tabGradient7.TextColor = System.Drawing.SystemColors.ControlDarkDark;
			dockPaneStripToolWindowGradient1.InactiveTabGradient = tabGradient7;
			dockPaneStripSkin1.ToolWindowGradient = dockPaneStripToolWindowGradient1;
			dockPanelSkin1.DockPaneStripSkin = dockPaneStripSkin1;
			this.dpMain.Skin = dockPanelSkin1;
			// 
			// EditorForm
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.dpMain);
			this.Controls.Add(this.stsMain);
			this.Controls.Add(this.tsMain);
			this.Controls.Add(this.menMain);
			this.MainMenuStrip = this.menMain;
			this.Name = "EditorForm";
			this.stsMain.ResumeLayout(false);
			this.stsMain.PerformLayout();
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
		System.Windows.Forms.ToolStripMenuItem itmGoToIndex;
		System.Windows.Forms.ToolStripMenuItem itmExclude;
		System.Windows.Forms.ToolStripMenuItem itmSetViewMode;
		System.Windows.Forms.ToolStripMenuItem itmSave;
		System.Windows.Forms.ToolStripMenuItem itmSaveAs;
		System.Windows.Forms.ToolStripMenuItem itmInvertSelections;
		System.Windows.Forms.ToolStripMenuItem itmSelectAll;
		System.Windows.Forms.ToolStripMenuItem itmInvertViewMode;
		System.Windows.Forms.ToolStripMenuItem itmRemoveBookmark;
		System.Windows.Forms.ToolStripMenuItem itmAddBookmark;
		private System.Windows.Forms.ToolStrip tsMain;
		private System.Windows.Forms.ToolStripButton btnNew;
		private System.Windows.Forms.ToolStripButton btnOpen;
		private System.Windows.Forms.ToolStripButton btnSave;
		private System.Windows.Forms.ToolStripMenuItem itmExport;
		private System.Windows.Forms.ToolStripMenuItem itmExtract;
		private System.Windows.Forms.ToolStripMenuItem itmDocumentSettings;
		private System.Windows.Forms.MenuStrip menMain;
		private WeifenLuo.WinFormsUI.Docking.DockPanel dpMain;
		private System.Windows.Forms.ToolStripMenuItem itmFile;
		private System.Windows.Forms.ToolStripMenuItem itmNew;
		private System.Windows.Forms.ToolStripMenuItem itmOpen;
		private System.Windows.Forms.ToolStripSeparator sepFile1;
		private System.Windows.Forms.ToolStripSeparator sepFile2;
		private System.Windows.Forms.ToolStripSeparator sepFile3;
		private System.Windows.Forms.ToolStripMenuItem itmExit;
		private System.Windows.Forms.ToolStripMenuItem itmEdit;
		private System.Windows.Forms.ToolStripSeparator sepEdit;
		private System.Windows.Forms.ToolStripMenuItem itmImage;
		private System.Windows.Forms.ToolStripMenuItem itmAdd;
		private System.Windows.Forms.ToolStripMenuItem itmFromFiles;
		private System.Windows.Forms.ToolStripMenuItem itmFromFolder;
		private System.Windows.Forms.ToolStripSeparator sepAdd;
		private System.Windows.Forms.ToolStripMenuItem itmFromWebPage;
		private System.Windows.Forms.ToolStripSeparator sepImage1;
		private System.Windows.Forms.ToolStripSeparator sepImage2;
		private System.Windows.Forms.ToolStripSeparator sepImage3;
		private System.Windows.Forms.ToolStripMenuItem itmBookmark;
		private System.Windows.Forms.ToolStripMenuItem itmTool;
		private System.Windows.Forms.ToolStripMenuItem itmOption;
		private System.Windows.Forms.ToolStripMenuItem itmHelp;
		private System.Windows.Forms.ToolStripMenuItem itmAbout;
		private System.Windows.Forms.StatusStrip stsMain;
		private System.Windows.Forms.ToolStripMenuItem itmView;
		private System.Windows.Forms.ToolStripMenuItem itmContentsWindow;
		private System.Windows.Forms.ToolStripMenuItem itmBookmarksWindow;
		private System.Windows.Forms.ToolStripMenuItem itmOpenImage;
		private System.Windows.Forms.ToolStripMenuItem itmArchivers;
		private System.Windows.Forms.ToolStripSeparator sepTool;

	}
}