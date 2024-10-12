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
			System.Windows.Forms.ToolStripMenuItem FileMenuItem;
			System.Windows.Forms.ToolStripMenuItem NewMenuItem;
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorForm));
			System.Windows.Forms.ToolStripMenuItem OpenMenuItem;
			System.Windows.Forms.ToolStripSeparator FileMenuSeparator1;
			System.Windows.Forms.ToolStripMenuItem SaveMenuItem;
			System.Windows.Forms.ToolStripMenuItem SaveAsMenuItem;
			System.Windows.Forms.ToolStripSeparator FileMenuSeparator2;
			System.Windows.Forms.ToolStripMenuItem ConfigureDocumentMenuItem;
			System.Windows.Forms.ToolStripSeparator FileMenuSeparator3;
			System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
			System.Windows.Forms.ToolStripMenuItem ImagesMenuItem;
			System.Windows.Forms.ToolStripMenuItem AddImagesMenuItem;
			System.Windows.Forms.ToolStripMenuItem AddImagesFromFilesMenuItem;
			System.Windows.Forms.ToolStripMenuItem AddImagesFromFolderMenuItem;
			System.Windows.Forms.ToolStripSeparator ImageMenuSeparator1;
			System.Windows.Forms.ToolStripSeparator ImageMenuSeparator2;
			System.Windows.Forms.ToolStripSeparator ImageMenuSeparator3;
			System.Windows.Forms.ToolStripMenuItem StartViewModeSettingLeftMenuItem;
			System.Windows.Forms.ToolStripMenuItem StartViewModeSettingRightMenuItem;
			System.Windows.Forms.ToolStripSeparator ImageMenuSeparator4;
			System.Windows.Forms.ToolStripMenuItem BookmarksMenuItem;
			System.Windows.Forms.ToolStripMenuItem ToolMenuItem;
			System.Windows.Forms.ToolStripMenuItem OptionMenuItem;
			System.Windows.Forms.ToolStripMenuItem HelpMenuItem;
			System.Windows.Forms.ToolStripMenuItem AboutMenuItem;
			System.Windows.Forms.StatusStrip MainStatusBar;
			System.Windows.Forms.ToolStripMenuItem ViewMenuItem;
			System.Windows.Forms.ToolStripMenuItem ContentsWindowMenuItem;
			System.Windows.Forms.ToolStripMenuItem BookmarksWindowMenuItem;
			System.Windows.Forms.ToolStripButton NewButton;
			System.Windows.Forms.ToolStripButton OpenButton;
			System.Windows.Forms.ToolStripButton SaveButton;
			OpenImageMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			DeleteImagesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			ExportImagesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			ExtractImagesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			StartViewModeSettingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			InvertViewModeMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			SetAsThumbnailMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			AddBookmarksMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			DeleteBookmarksMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			StatusProgressBar = new System.Windows.Forms.ToolStripProgressBar();
			ImageCountLabel = new System.Windows.Forms.ToolStripStatusLabel();
			MainMenu = new System.Windows.Forms.MenuStrip();
			MainToolBar = new System.Windows.Forms.ToolStrip();
			MainDockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
			FileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			NewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			OpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			FileMenuSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			SaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			SaveAsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			FileMenuSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			ConfigureDocumentMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			FileMenuSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			ImagesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			AddImagesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			AddImagesFromFilesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			AddImagesFromFolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			ImageMenuSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			ImageMenuSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			ImageMenuSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			StartViewModeSettingLeftMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			StartViewModeSettingRightMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			ImageMenuSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			BookmarksMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			ToolMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			OptionMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			HelpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			AboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			MainStatusBar = new System.Windows.Forms.StatusStrip();
			ViewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			ContentsWindowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			BookmarksWindowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			NewButton = new System.Windows.Forms.ToolStripButton();
			OpenButton = new System.Windows.Forms.ToolStripButton();
			SaveButton = new System.Windows.Forms.ToolStripButton();
			MainStatusBar.SuspendLayout();
			MainMenu.SuspendLayout();
			MainToolBar.SuspendLayout();
			SuspendLayout();
			// 
			// FileMenuItem
			// 
			FileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { NewMenuItem, OpenMenuItem, FileMenuSeparator1, SaveMenuItem, SaveAsMenuItem, FileMenuSeparator2, ConfigureDocumentMenuItem, FileMenuSeparator3, ExitMenuItem });
			FileMenuItem.Name = "FileMenuItem";
			resources.ApplyResources(FileMenuItem, "FileMenuItem");
			// 
			// NewMenuItem
			// 
			resources.ApplyResources(NewMenuItem, "NewMenuItem");
			NewMenuItem.Name = "NewMenuItem";
			NewMenuItem.Click += OnNewMenuItemClick;
			// 
			// OpenMenuItem
			// 
			resources.ApplyResources(OpenMenuItem, "OpenMenuItem");
			OpenMenuItem.Name = "OpenMenuItem";
			OpenMenuItem.Click += OnOpenMenuItemClick;
			// 
			// FileMenuSeparator1
			// 
			FileMenuSeparator1.Name = "FileMenuSeparator1";
			resources.ApplyResources(FileMenuSeparator1, "FileMenuSeparator1");
			// 
			// SaveMenuItem
			// 
			resources.ApplyResources(SaveMenuItem, "SaveMenuItem");
			SaveMenuItem.Name = "SaveMenuItem";
			SaveMenuItem.Click += OnSaveMenuItemClick;
			// 
			// SaveAsMenuItem
			// 
			SaveAsMenuItem.Name = "SaveAsMenuItem";
			resources.ApplyResources(SaveAsMenuItem, "SaveAsMenuItem");
			SaveAsMenuItem.Click += OnSaveAsMenuItemClick;
			// 
			// FileMenuSeparator2
			// 
			FileMenuSeparator2.Name = "FileMenuSeparator2";
			resources.ApplyResources(FileMenuSeparator2, "FileMenuSeparator2");
			// 
			// ConfigureDocumentMenuItem
			// 
			resources.ApplyResources(ConfigureDocumentMenuItem, "ConfigureDocumentMenuItem");
			ConfigureDocumentMenuItem.Name = "ConfigureDocumentMenuItem";
			ConfigureDocumentMenuItem.Click += OnConfigureDocumentMenuItemClick;
			// 
			// FileMenuSeparator3
			// 
			FileMenuSeparator3.Name = "FileMenuSeparator3";
			resources.ApplyResources(FileMenuSeparator3, "FileMenuSeparator3");
			// 
			// ExitMenuItem
			// 
			ExitMenuItem.Name = "ExitMenuItem";
			resources.ApplyResources(ExitMenuItem, "ExitMenuItem");
			ExitMenuItem.Click += OnExitMenuItemClick;
			// 
			// ImagesMenuItem
			// 
			ImagesMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { AddImagesMenuItem, ImageMenuSeparator1, OpenImageMenuItem, DeleteImagesMenuItem, ImageMenuSeparator2, ExportImagesMenuItem, ExtractImagesMenuItem, ImageMenuSeparator3, StartViewModeSettingMenuItem, InvertViewModeMenuItem, ImageMenuSeparator4, SetAsThumbnailMenuItem });
			ImagesMenuItem.Name = "ImagesMenuItem";
			resources.ApplyResources(ImagesMenuItem, "ImagesMenuItem");
			// 
			// AddImagesMenuItem
			// 
			AddImagesMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { AddImagesFromFilesMenuItem, AddImagesFromFolderMenuItem });
			AddImagesMenuItem.Name = "AddImagesMenuItem";
			resources.ApplyResources(AddImagesMenuItem, "AddImagesMenuItem");
			// 
			// AddImagesFromFilesMenuItem
			// 
			resources.ApplyResources(AddImagesFromFilesMenuItem, "AddImagesFromFilesMenuItem");
			AddImagesFromFilesMenuItem.Name = "AddImagesFromFilesMenuItem";
			AddImagesFromFilesMenuItem.Click += OnAddImagesFromFilesMenuItemClick;
			// 
			// AddImagesFromFolderMenuItem
			// 
			resources.ApplyResources(AddImagesFromFolderMenuItem, "AddImagesFromFolderMenuItem");
			AddImagesFromFolderMenuItem.Name = "AddImagesFromFolderMenuItem";
			AddImagesFromFolderMenuItem.Click += OnAddImagesFromFolderMenuItemClick;
			// 
			// ImageMenuSeparator1
			// 
			ImageMenuSeparator1.Name = "ImageMenuSeparator1";
			resources.ApplyResources(ImageMenuSeparator1, "ImageMenuSeparator1");
			// 
			// OpenImageMenuItem
			// 
			resources.ApplyResources(OpenImageMenuItem, "OpenImageMenuItem");
			OpenImageMenuItem.Name = "OpenImageMenuItem";
			OpenImageMenuItem.Click += OnOpenImageMenuItemClick;
			// 
			// DeleteImagesMenuItem
			// 
			resources.ApplyResources(DeleteImagesMenuItem, "DeleteImagesMenuItem");
			DeleteImagesMenuItem.Name = "DeleteImagesMenuItem";
			DeleteImagesMenuItem.Click += OnDeleteImagesMenuItemClick;
			// 
			// ImageMenuSeparator2
			// 
			ImageMenuSeparator2.Name = "ImageMenuSeparator2";
			resources.ApplyResources(ImageMenuSeparator2, "ImageMenuSeparator2");
			// 
			// ExportImagesMenuItem
			// 
			resources.ApplyResources(ExportImagesMenuItem, "ExportImagesMenuItem");
			ExportImagesMenuItem.Name = "ExportImagesMenuItem";
			ExportImagesMenuItem.Click += OnExportImagesMenuItemClick;
			// 
			// ExtractImagesMenuItem
			// 
			resources.ApplyResources(ExtractImagesMenuItem, "ExtractImagesMenuItem");
			ExtractImagesMenuItem.Name = "ExtractImagesMenuItem";
			ExtractImagesMenuItem.Click += OnExtractImagesMenuItemClick;
			// 
			// ImageMenuSeparator3
			// 
			ImageMenuSeparator3.Name = "ImageMenuSeparator3";
			resources.ApplyResources(ImageMenuSeparator3, "ImageMenuSeparator3");
			// 
			// StartViewModeSettingMenuItem
			// 
			StartViewModeSettingMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { StartViewModeSettingLeftMenuItem, StartViewModeSettingRightMenuItem });
			resources.ApplyResources(StartViewModeSettingMenuItem, "StartViewModeSettingMenuItem");
			StartViewModeSettingMenuItem.Name = "StartViewModeSettingMenuItem";
			// 
			// StartViewModeSettingLeftMenuItem
			// 
			StartViewModeSettingLeftMenuItem.Name = "StartViewModeSettingLeftMenuItem";
			resources.ApplyResources(StartViewModeSettingLeftMenuItem, "StartViewModeSettingLeftMenuItem");
			StartViewModeSettingLeftMenuItem.Click += OnStartViewModeSettingLeftMenuItemClick;
			// 
			// StartViewModeSettingRightMenuItem
			// 
			StartViewModeSettingRightMenuItem.Name = "StartViewModeSettingRightMenuItem";
			resources.ApplyResources(StartViewModeSettingRightMenuItem, "StartViewModeSettingRightMenuItem");
			StartViewModeSettingRightMenuItem.Click += OnStartViewModeSettingRightMenuItemClick;
			// 
			// InvertViewModeMenuItem
			// 
			resources.ApplyResources(InvertViewModeMenuItem, "InvertViewModeMenuItem");
			InvertViewModeMenuItem.Name = "InvertViewModeMenuItem";
			InvertViewModeMenuItem.Click += OnInvertViewModeMenuItemClick;
			// 
			// ImageMenuSeparator4
			// 
			ImageMenuSeparator4.Name = "ImageMenuSeparator4";
			resources.ApplyResources(ImageMenuSeparator4, "ImageMenuSeparator4");
			// 
			// SetAsThumbnailMenuItem
			// 
			resources.ApplyResources(SetAsThumbnailMenuItem, "SetAsThumbnailMenuItem");
			SetAsThumbnailMenuItem.Name = "SetAsThumbnailMenuItem";
			SetAsThumbnailMenuItem.Click += OnSetAsThumbnailMenuItemClick;
			// 
			// BookmarksMenuItem
			// 
			BookmarksMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { AddBookmarksMenuItem, DeleteBookmarksMenuItem });
			BookmarksMenuItem.Name = "BookmarksMenuItem";
			resources.ApplyResources(BookmarksMenuItem, "BookmarksMenuItem");
			// 
			// AddBookmarksMenuItem
			// 
			resources.ApplyResources(AddBookmarksMenuItem, "AddBookmarksMenuItem");
			AddBookmarksMenuItem.Name = "AddBookmarksMenuItem";
			AddBookmarksMenuItem.Click += OnAddBookmarksMenuItemClick;
			// 
			// DeleteBookmarksMenuItem
			// 
			resources.ApplyResources(DeleteBookmarksMenuItem, "DeleteBookmarksMenuItem");
			DeleteBookmarksMenuItem.Name = "DeleteBookmarksMenuItem";
			DeleteBookmarksMenuItem.Click += OnDeleteBookmarksMenuItemClick;
			// 
			// ToolMenuItem
			// 
			ToolMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { OptionMenuItem });
			ToolMenuItem.Name = "ToolMenuItem";
			resources.ApplyResources(ToolMenuItem, "ToolMenuItem");
			// 
			// OptionMenuItem
			// 
			OptionMenuItem.Name = "OptionMenuItem";
			resources.ApplyResources(OptionMenuItem, "OptionMenuItem");
			OptionMenuItem.Click += OnOptionMenuItemClick;
			// 
			// HelpMenuItem
			// 
			HelpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { AboutMenuItem });
			HelpMenuItem.Name = "HelpMenuItem";
			resources.ApplyResources(HelpMenuItem, "HelpMenuItem");
			// 
			// AboutMenuItem
			// 
			AboutMenuItem.Name = "AboutMenuItem";
			resources.ApplyResources(AboutMenuItem, "AboutMenuItem");
			AboutMenuItem.Click += OnAboutMenuItemClick;
			// 
			// MainStatusBar
			// 
			MainStatusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { StatusLabel, StatusProgressBar, ImageCountLabel });
			resources.ApplyResources(MainStatusBar, "MainStatusBar");
			MainStatusBar.Name = "MainStatusBar";
			MainStatusBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			// 
			// StatusLabel
			// 
			StatusLabel.Name = "StatusLabel";
			resources.ApplyResources(StatusLabel, "StatusLabel");
			StatusLabel.Spring = true;
			// 
			// StatusProgressBar
			// 
			StatusProgressBar.Name = "StatusProgressBar";
			resources.ApplyResources(StatusProgressBar, "StatusProgressBar");
			// 
			// ImageCountLabel
			// 
			resources.ApplyResources(ImageCountLabel, "ImageCountLabel");
			ImageCountLabel.Name = "ImageCountLabel";
			// 
			// ViewMenuItem
			// 
			ViewMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { ContentsWindowMenuItem, BookmarksWindowMenuItem });
			ViewMenuItem.Name = "ViewMenuItem";
			resources.ApplyResources(ViewMenuItem, "ViewMenuItem");
			// 
			// ContentsWindowMenuItem
			// 
			resources.ApplyResources(ContentsWindowMenuItem, "ContentsWindowMenuItem");
			ContentsWindowMenuItem.Name = "ContentsWindowMenuItem";
			ContentsWindowMenuItem.Click += OnContentsWindowMenuItemClick;
			// 
			// BookmarksWindowMenuItem
			// 
			resources.ApplyResources(BookmarksWindowMenuItem, "BookmarksWindowMenuItem");
			BookmarksWindowMenuItem.Name = "BookmarksWindowMenuItem";
			BookmarksWindowMenuItem.Click += OnBookmarksWindowMenuItemClick;
			// 
			// NewButton
			// 
			NewButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(NewButton, "NewButton");
			NewButton.Name = "NewButton";
			NewButton.Click += OnNewMenuItemClick;
			// 
			// OpenButton
			// 
			OpenButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(OpenButton, "OpenButton");
			OpenButton.Name = "OpenButton";
			OpenButton.Click += OnOpenMenuItemClick;
			// 
			// SaveButton
			// 
			SaveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(SaveButton, "SaveButton");
			SaveButton.Name = "SaveButton";
			SaveButton.Click += OnSaveMenuItemClick;
			// 
			// MainMenu
			// 
			MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { FileMenuItem, ViewMenuItem, ImagesMenuItem, BookmarksMenuItem, ToolMenuItem, HelpMenuItem });
			resources.ApplyResources(MainMenu, "MainMenu");
			MainMenu.Name = "MainMenu";
			MainMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			// 
			// MainToolBar
			// 
			MainToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { NewButton, OpenButton, SaveButton });
			resources.ApplyResources(MainToolBar, "MainToolBar");
			MainToolBar.Name = "MainToolBar";
			MainToolBar.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			// 
			// MainDockPanel
			// 
			resources.ApplyResources(MainDockPanel, "MainDockPanel");
			MainDockPanel.DockBackColor = System.Drawing.SystemColors.Control;
			MainDockPanel.Name = "MainDockPanel";
			// 
			// EditorForm
			// 
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			Controls.Add(MainDockPanel);
			Controls.Add(MainStatusBar);
			Controls.Add(MainToolBar);
			Controls.Add(MainMenu);
			MainMenuStrip = MainMenu;
			Name = "EditorForm";
			MainStatusBar.ResumeLayout(false);
			MainStatusBar.PerformLayout();
			MainMenu.ResumeLayout(false);
			MainMenu.PerformLayout();
			MainToolBar.ResumeLayout(false);
			MainToolBar.PerformLayout();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		System.Windows.Forms.ToolStripStatusLabel StatusLabel;
		System.Windows.Forms.ToolStripStatusLabel ImageCountLabel;
		System.Windows.Forms.ToolStripProgressBar StatusProgressBar;
		System.Windows.Forms.ToolStripMenuItem DeleteImagesMenuItem;
		System.Windows.Forms.ToolStripMenuItem DeleteBookmarksMenuItem;
		System.Windows.Forms.ToolStripMenuItem AddBookmarksMenuItem;
		private System.Windows.Forms.ToolStrip MainToolBar;
		private System.Windows.Forms.ToolStripMenuItem ExportImagesMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ExtractImagesMenuItem;
		private System.Windows.Forms.MenuStrip MainMenu;
		private WeifenLuo.WinFormsUI.Docking.DockPanel MainDockPanel;
		private System.Windows.Forms.ToolStripMenuItem OpenImageMenuItem;
		private System.Windows.Forms.ToolStripMenuItem StartViewModeSettingMenuItem;
		private System.Windows.Forms.ToolStripMenuItem InvertViewModeMenuItem;
		private System.Windows.Forms.ToolStripMenuItem SetAsThumbnailMenuItem;
	}
}