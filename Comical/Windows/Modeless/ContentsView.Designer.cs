namespace Comical
{
	partial class ContentsView
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
			if (disposing)
			{
				_images.CollectionChanged -= OnImagesCollectionChanged;
				_images.CollectionItemPropertyChanged -= OnImagesCollectionItemPropertyChanged;
				_thumbnailCache.Dispose();
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
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			System.Windows.Forms.ContextMenuStrip ImageContextMenu;
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(ContentsView));
			OpenMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			ImageMenuSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			AddToBookmarkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			ImageMenuSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			ExportMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			ExtractMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			ImageMenuSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			StartViewModeSettingLeftMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			StartViewModeSettingRightMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			ImageMenuSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			DeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			ImagesDataGridView = new Controls.DraggableDataGridView();
			ImageDataGridViewColumn = new System.Windows.Forms.DataGridViewImageColumn();
			ViewModeDataGridViewColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
			ImageContextMenu = new System.Windows.Forms.ContextMenuStrip(components);
			ImageContextMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)ImagesDataGridView).BeginInit();
			SuspendLayout();
			// 
			// ImageContextMenu
			// 
			ImageContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { OpenMenuItem, ImageMenuSeparator1, AddToBookmarkMenuItem, ImageMenuSeparator2, ExportMenuItem, ExtractMenuItem, ImageMenuSeparator3, StartViewModeSettingLeftMenuItem, StartViewModeSettingRightMenuItem, ImageMenuSeparator4, DeleteMenuItem });
			ImageContextMenu.Name = "conImage";
			resources.ApplyResources(ImageContextMenu, "ImageContextMenu");
			// 
			// OpenMenuItem
			// 
			resources.ApplyResources(OpenMenuItem, "OpenMenuItem");
			OpenMenuItem.Name = "OpenMenuItem";
			OpenMenuItem.Click += OnOpenMenuItemClick;
			// 
			// ImageMenuSeparator1
			// 
			ImageMenuSeparator1.Name = "ImageMenuSeparator1";
			resources.ApplyResources(ImageMenuSeparator1, "ImageMenuSeparator1");
			// 
			// AddToBookmarkMenuItem
			// 
			AddToBookmarkMenuItem.Name = "AddToBookmarkMenuItem";
			resources.ApplyResources(AddToBookmarkMenuItem, "AddToBookmarkMenuItem");
			// 
			// ImageMenuSeparator2
			// 
			ImageMenuSeparator2.Name = "ImageMenuSeparator2";
			resources.ApplyResources(ImageMenuSeparator2, "ImageMenuSeparator2");
			// 
			// ExportMenuItem
			// 
			resources.ApplyResources(ExportMenuItem, "ExportMenuItem");
			ExportMenuItem.Name = "ExportMenuItem";
			// 
			// ExtractMenuItem
			// 
			resources.ApplyResources(ExtractMenuItem, "ExtractMenuItem");
			ExtractMenuItem.Name = "ExtractMenuItem";
			// 
			// ImageMenuSeparator3
			// 
			ImageMenuSeparator3.Name = "ImageMenuSeparator3";
			resources.ApplyResources(ImageMenuSeparator3, "ImageMenuSeparator3");
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
			// ImageMenuSeparator4
			// 
			ImageMenuSeparator4.Name = "ImageMenuSeparator4";
			resources.ApplyResources(ImageMenuSeparator4, "ImageMenuSeparator4");
			// 
			// DeleteMenuItem
			// 
			resources.ApplyResources(DeleteMenuItem, "DeleteMenuItem");
			DeleteMenuItem.Name = "DeleteMenuItem";
			DeleteMenuItem.Click += OnDeleteMenuItemClick;
			// 
			// ImagesDataGridView
			// 
			ImagesDataGridView.AllowDrop = true;
			ImagesDataGridView.AllowUserToAddRows = false;
			ImagesDataGridView.AllowUserToDragRows = true;
			ImagesDataGridView.AllowUserToResizeColumns = false;
			ImagesDataGridView.AllowUserToResizeRows = false;
			ImagesDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			ImagesDataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
			ImagesDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			ImagesDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			ImagesDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			ImagesDataGridView.ColumnHeadersVisible = false;
			ImagesDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { ImageDataGridViewColumn, ViewModeDataGridViewColumn });
			ImagesDataGridView.ContextMenuStrip = ImageContextMenu;
			resources.ApplyResources(ImagesDataGridView, "ImagesDataGridView");
			ImagesDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			ImagesDataGridView.GridColor = System.Drawing.SystemColors.Control;
			ImagesDataGridView.MultiDrag = true;
			ImagesDataGridView.Name = "ImagesDataGridView";
			ImagesDataGridView.RowHeadersVisible = false;
			ImagesDataGridView.RowTemplate.Height = 42;
			ImagesDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			ImagesDataGridView.ShowCellToolTips = false;
			ImagesDataGridView.VirtualMode = true;
			ImagesDataGridView.RowDragStarting += OnImagesDataGridViewRowDragStarting;
			ImagesDataGridView.RowDropped += OnImagesDataGridViewRowDropped;
			ImagesDataGridView.QueryRowDragDropEffect += OnImagesDataGridViewQueryRowDragDropEffect;
			ImagesDataGridView.CellDoubleClick += OnImagesDataGridViewCellDoubleClick;
			ImagesDataGridView.CellValueNeeded += OnImagesDataGridViewCellValueNeeded;
			ImagesDataGridView.CellValuePushed += OnImagesDataGridViewCellValuePushed;
			ImagesDataGridView.SelectionChanged += OnImagesDataGridViewSelectionChanged;
			ImagesDataGridView.UserDeletedRow += OnImagesDataGridViewUserDeletedRow;
			ImagesDataGridView.UserDeletingRow += OnImagesDataGridViewUserDeletingRow;
			ImagesDataGridView.DragDrop += OnImagesDataGridViewDragDrop;
			ImagesDataGridView.DragEnter += OnImagesDataGridViewDragEnter;
			// 
			// ImageDataGridViewColumn
			// 
			ImageDataGridViewColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(ImageDataGridViewColumn, "ImageDataGridViewColumn");
			ImageDataGridViewColumn.Name = "ImageDataGridViewColumn";
			ImageDataGridViewColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// ViewModeDataGridViewColumn
			// 
			ViewModeDataGridViewColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			ViewModeDataGridViewColumn.FillWeight = 80F;
			resources.ApplyResources(ViewModeDataGridViewColumn, "ViewModeDataGridViewColumn");
			ViewModeDataGridViewColumn.Name = "ViewModeDataGridViewColumn";
			// 
			// ContentsView
			// 
			AllowDrop = true;
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			Controls.Add(ImagesDataGridView);
			HideOnClose = true;
			Name = "ContentsView";
			ImageContextMenu.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)ImagesDataGridView).EndInit();
			ResumeLayout(false);
		}

		#endregion
		private Controls.DraggableDataGridView ImagesDataGridView;
		private System.Windows.Forms.ToolStripMenuItem OpenMenuItem;
		private System.Windows.Forms.ToolStripMenuItem DeleteMenuItem;
		private System.Windows.Forms.ToolStripSeparator ImageMenuSeparator1;
		private System.Windows.Forms.ToolStripMenuItem ExportMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ExtractMenuItem;
		private System.Windows.Forms.ToolStripSeparator ImageMenuSeparator2;
		private System.Windows.Forms.ToolStripMenuItem AddToBookmarkMenuItem;
		private System.Windows.Forms.ToolStripSeparator ImageMenuSeparator3;
		private System.Windows.Forms.ToolStripMenuItem StartViewModeSettingLeftMenuItem;
		private System.Windows.Forms.ToolStripMenuItem StartViewModeSettingRightMenuItem;
		private System.Windows.Forms.ToolStripSeparator ImageMenuSeparator4;
		private System.Windows.Forms.DataGridViewImageColumn ImageDataGridViewColumn;
		private System.Windows.Forms.DataGridViewComboBoxColumn ViewModeDataGridViewColumn;
	}
}
