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
				_imageList.Dispose();
				_bookmarkList.Dispose();
				_document.Dispose();
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
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorForm));
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
			itmOpenImage = new System.Windows.Forms.ToolStripMenuItem();
			itmExclude = new System.Windows.Forms.ToolStripMenuItem();
			itmExport = new System.Windows.Forms.ToolStripMenuItem();
			itmExtract = new System.Windows.Forms.ToolStripMenuItem();
			itmSetViewMode = new System.Windows.Forms.ToolStripMenuItem();
			itmInvertViewMode = new System.Windows.Forms.ToolStripMenuItem();
			itmAddBookmark = new System.Windows.Forms.ToolStripMenuItem();
			itmDeleteBookmark = new System.Windows.Forms.ToolStripMenuItem();
			lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
			prgStatus = new System.Windows.Forms.ToolStripProgressBar();
			lblImageCount = new System.Windows.Forms.ToolStripStatusLabel();
			menMain = new System.Windows.Forms.MenuStrip();
			tsMain = new System.Windows.Forms.ToolStrip();
			dpMain = new WeifenLuo.WinFormsUI.Docking.DockPanel();
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
			menMain.SuspendLayout();
			tsMain.SuspendLayout();
			SuspendLayout();
			// 
			// itmFile
			// 
			itmFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { itmNew, itmOpen, sepFile1, itmSave, itmSaveAs, sepFile2, itmDocumentSettings, sepFile3, itmExit });
			itmFile.Name = "itmFile";
			resources.ApplyResources(itmFile, "itmFile");
			// 
			// itmNew
			// 
			resources.ApplyResources(itmNew, "itmNew");
			itmNew.Name = "itmNew";
			itmNew.Click += itmNew_Click;
			// 
			// itmOpen
			// 
			resources.ApplyResources(itmOpen, "itmOpen");
			itmOpen.Name = "itmOpen";
			itmOpen.Click += itmOpen_Click;
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
			itmSave.Click += itmSave_Click;
			// 
			// itmSaveAs
			// 
			itmSaveAs.Name = "itmSaveAs";
			resources.ApplyResources(itmSaveAs, "itmSaveAs");
			itmSaveAs.Click += itmSaveAs_Click;
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
			itmDocumentSettings.Click += itmDocumentSettings_Click;
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
			itmExit.Click += itmExit_Click;
			// 
			// itmImage
			// 
			itmImage.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { itmAdd, sepImage1, itmOpenImage, itmExclude, sepImage2, itmExport, itmExtract, sepImage3, itmSetViewMode, itmInvertViewMode });
			itmImage.Name = "itmImage";
			resources.ApplyResources(itmImage, "itmImage");
			// 
			// itmAdd
			// 
			itmAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { itmFromFiles, itmFromFolder });
			itmAdd.Name = "itmAdd";
			resources.ApplyResources(itmAdd, "itmAdd");
			// 
			// itmFromFiles
			// 
			resources.ApplyResources(itmFromFiles, "itmFromFiles");
			itmFromFiles.Name = "itmFromFiles";
			itmFromFiles.Click += itmFromFile_Click;
			// 
			// itmFromFolder
			// 
			resources.ApplyResources(itmFromFolder, "itmFromFolder");
			itmFromFolder.Name = "itmFromFolder";
			itmFromFolder.Click += itmFromFolder_Click;
			// 
			// sepImage1
			// 
			sepImage1.Name = "sepImage1";
			resources.ApplyResources(sepImage1, "sepImage1");
			// 
			// itmOpenImage
			// 
			resources.ApplyResources(itmOpenImage, "itmOpenImage");
			itmOpenImage.Name = "itmOpenImage";
			itmOpenImage.Click += itmOpenImage_Click;
			// 
			// itmExclude
			// 
			resources.ApplyResources(itmExclude, "itmExclude");
			itmExclude.Name = "itmExclude";
			itmExclude.Click += itmDelete_Click;
			// 
			// sepImage2
			// 
			sepImage2.Name = "sepImage2";
			resources.ApplyResources(sepImage2, "sepImage2");
			// 
			// itmExport
			// 
			resources.ApplyResources(itmExport, "itmExport");
			itmExport.Name = "itmExport";
			itmExport.Click += itmExport_Click;
			// 
			// itmExtract
			// 
			resources.ApplyResources(itmExtract, "itmExtract");
			itmExtract.Name = "itmExtract";
			itmExtract.Click += itmExtract_Click;
			// 
			// sepImage3
			// 
			sepImage3.Name = "sepImage3";
			resources.ApplyResources(sepImage3, "sepImage3");
			// 
			// itmSetViewMode
			// 
			itmSetViewMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { itmWithLeft, itmWithRight });
			resources.ApplyResources(itmSetViewMode, "itmSetViewMode");
			itmSetViewMode.Name = "itmSetViewMode";
			// 
			// itmWithLeft
			// 
			itmWithLeft.Name = "itmWithLeft";
			resources.ApplyResources(itmWithLeft, "itmWithLeft");
			itmWithLeft.Click += itmWithLeft_Click;
			// 
			// itmWithRight
			// 
			itmWithRight.Name = "itmWithRight";
			resources.ApplyResources(itmWithRight, "itmWithRight");
			itmWithRight.Click += itmWithRight_Click;
			// 
			// itmInvertViewMode
			// 
			resources.ApplyResources(itmInvertViewMode, "itmInvertViewMode");
			itmInvertViewMode.Name = "itmInvertViewMode";
			itmInvertViewMode.Click += itmInvertViewMode_Click;
			// 
			// itmBookmark
			// 
			itmBookmark.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { itmAddBookmark, itmDeleteBookmark });
			itmBookmark.Name = "itmBookmark";
			resources.ApplyResources(itmBookmark, "itmBookmark");
			// 
			// itmAddBookmark
			// 
			resources.ApplyResources(itmAddBookmark, "itmAddBookmark");
			itmAddBookmark.Name = "itmAddBookmark";
			itmAddBookmark.Click += itmAddBookmark_Click;
			// 
			// itmDeleteBookmark
			// 
			resources.ApplyResources(itmDeleteBookmark, "itmDeleteBookmark");
			itmDeleteBookmark.Name = "itmDeleteBookmark";
			itmDeleteBookmark.Click += itmDeleteBookmark_Click;
			// 
			// itmTool
			// 
			itmTool.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { itmOption });
			itmTool.Name = "itmTool";
			resources.ApplyResources(itmTool, "itmTool");
			// 
			// itmOption
			// 
			itmOption.Name = "itmOption";
			resources.ApplyResources(itmOption, "itmOption");
			itmOption.Click += itmOption_Click;
			// 
			// itmHelp
			// 
			itmHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { itmAbout });
			itmHelp.Name = "itmHelp";
			resources.ApplyResources(itmHelp, "itmHelp");
			// 
			// itmAbout
			// 
			itmAbout.Name = "itmAbout";
			resources.ApplyResources(itmAbout, "itmAbout");
			itmAbout.Click += itmAbout_Click;
			// 
			// stsMain
			// 
			stsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { lblStatus, prgStatus, lblImageCount });
			resources.ApplyResources(stsMain, "stsMain");
			stsMain.Name = "stsMain";
			stsMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			// 
			// lblStatus
			// 
			lblStatus.Name = "lblStatus";
			resources.ApplyResources(lblStatus, "lblStatus");
			lblStatus.Spring = true;
			// 
			// prgStatus
			// 
			prgStatus.Name = "prgStatus";
			resources.ApplyResources(prgStatus, "prgStatus");
			// 
			// lblImageCount
			// 
			resources.ApplyResources(lblImageCount, "lblImageCount");
			lblImageCount.Name = "lblImageCount";
			// 
			// itmView
			// 
			itmView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { itmContentsWindow, itmBookmarksWindow });
			itmView.Name = "itmView";
			resources.ApplyResources(itmView, "itmView");
			// 
			// itmContentsWindow
			// 
			resources.ApplyResources(itmContentsWindow, "itmContentsWindow");
			itmContentsWindow.Name = "itmContentsWindow";
			itmContentsWindow.Click += itmContentsWindow_Click;
			// 
			// itmBookmarksWindow
			// 
			resources.ApplyResources(itmBookmarksWindow, "itmBookmarksWindow");
			itmBookmarksWindow.Name = "itmBookmarksWindow";
			itmBookmarksWindow.Click += itmBookmarksWindow_Click;
			// 
			// btnNew
			// 
			btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(btnNew, "btnNew");
			btnNew.Name = "btnNew";
			btnNew.Click += itmNew_Click;
			// 
			// btnOpen
			// 
			btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(btnOpen, "btnOpen");
			btnOpen.Name = "btnOpen";
			btnOpen.Click += itmOpen_Click;
			// 
			// btnSave
			// 
			btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(btnSave, "btnSave");
			btnSave.Name = "btnSave";
			btnSave.Click += itmSave_Click;
			// 
			// menMain
			// 
			menMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { itmFile, itmView, itmImage, itmBookmark, itmTool, itmHelp });
			resources.ApplyResources(menMain, "menMain");
			menMain.Name = "menMain";
			menMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			// 
			// tsMain
			// 
			tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { btnNew, btnOpen, btnSave });
			resources.ApplyResources(tsMain, "tsMain");
			tsMain.Name = "tsMain";
			tsMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			// 
			// dpMain
			// 
			resources.ApplyResources(dpMain, "dpMain");
			dpMain.DockBackColor = System.Drawing.SystemColors.Control;
			dpMain.Name = "dpMain";
			// 
			// EditorForm
			// 
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			Controls.Add(dpMain);
			Controls.Add(stsMain);
			Controls.Add(tsMain);
			Controls.Add(menMain);
			MainMenuStrip = menMain;
			Name = "EditorForm";
			stsMain.ResumeLayout(false);
			stsMain.PerformLayout();
			menMain.ResumeLayout(false);
			menMain.PerformLayout();
			tsMain.ResumeLayout(false);
			tsMain.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
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