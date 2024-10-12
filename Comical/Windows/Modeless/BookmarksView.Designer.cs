namespace Comical
{
	partial class BookmarksView
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
				_bookmarks.CollectionChanged -= OnBookmarksCollectionChanged;
				_images.CollectionChanged -= OnImagesCollectionChanged;
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
			System.Windows.Forms.ContextMenuStrip BookmarkContextMenu;
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(BookmarksView));
			SelectTargetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			BookmarkMenuSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			CreateNewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			InsertAboveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			InsertBelowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			BookmarkMenuSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			DeleteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			BookmarksDataGridView = new Controls.DraggableDataGridView();
			NameDataGridViewColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			TargetDataGridViewColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			BookmarkContextMenu = new System.Windows.Forms.ContextMenuStrip(components);
			BookmarkContextMenu.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)BookmarksDataGridView).BeginInit();
			SuspendLayout();
			// 
			// BookmarkContextMenu
			// 
			BookmarkContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { SelectTargetMenuItem, BookmarkMenuSeparator1, CreateNewMenuItem, InsertAboveMenuItem, InsertBelowMenuItem, BookmarkMenuSeparator2, DeleteMenuItem });
			BookmarkContextMenu.Name = "conBookmark";
			resources.ApplyResources(BookmarkContextMenu, "BookmarkContextMenu");
			// 
			// SelectTargetMenuItem
			// 
			resources.ApplyResources(SelectTargetMenuItem, "SelectTargetMenuItem");
			SelectTargetMenuItem.Name = "SelectTargetMenuItem";
			SelectTargetMenuItem.Click += OnSelectTargetMenuItemClick;
			// 
			// BookmarkMenuSeparator1
			// 
			BookmarkMenuSeparator1.Name = "BookmarkMenuSeparator1";
			resources.ApplyResources(BookmarkMenuSeparator1, "BookmarkMenuSeparator1");
			// 
			// CreateNewMenuItem
			// 
			CreateNewMenuItem.Name = "CreateNewMenuItem";
			resources.ApplyResources(CreateNewMenuItem, "CreateNewMenuItem");
			CreateNewMenuItem.Click += OnCreateNewMenuItemClick;
			// 
			// InsertAboveMenuItem
			// 
			InsertAboveMenuItem.Name = "InsertAboveMenuItem";
			resources.ApplyResources(InsertAboveMenuItem, "InsertAboveMenuItem");
			InsertAboveMenuItem.Click += OnInsertAboveMenuItemClick;
			// 
			// InsertBelowMenuItem
			// 
			InsertBelowMenuItem.Name = "InsertBelowMenuItem";
			resources.ApplyResources(InsertBelowMenuItem, "InsertBelowMenuItem");
			InsertBelowMenuItem.Click += OnInsertBelowMenuItemClick;
			// 
			// BookmarkMenuSeparator2
			// 
			BookmarkMenuSeparator2.Name = "BookmarkMenuSeparator2";
			resources.ApplyResources(BookmarkMenuSeparator2, "BookmarkMenuSeparator2");
			// 
			// DeleteMenuItem
			// 
			resources.ApplyResources(DeleteMenuItem, "DeleteMenuItem");
			DeleteMenuItem.Name = "DeleteMenuItem";
			DeleteMenuItem.Click += OnDeleteMenuItemClick;
			// 
			// BookmarksDataGridView
			// 
			BookmarksDataGridView.AllowDrop = true;
			BookmarksDataGridView.AllowUserToAddRows = false;
			BookmarksDataGridView.AllowUserToDragRows = true;
			BookmarksDataGridView.AllowUserToResizeColumns = false;
			BookmarksDataGridView.AllowUserToResizeRows = false;
			BookmarksDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			BookmarksDataGridView.BackgroundColor = System.Drawing.SystemColors.Control;
			BookmarksDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			BookmarksDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			BookmarksDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			BookmarksDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			BookmarksDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { NameDataGridViewColumn, TargetDataGridViewColumn });
			BookmarksDataGridView.ContextMenuStrip = BookmarkContextMenu;
			resources.ApplyResources(BookmarksDataGridView, "BookmarksDataGridView");
			BookmarksDataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
			BookmarksDataGridView.GridColor = System.Drawing.SystemColors.Control;
			BookmarksDataGridView.Name = "BookmarksDataGridView";
			BookmarksDataGridView.RowHeadersVisible = false;
			BookmarksDataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			BookmarksDataGridView.RowTemplate.Height = 21;
			BookmarksDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			BookmarksDataGridView.ShowCellToolTips = false;
			BookmarksDataGridView.VirtualMode = true;
			BookmarksDataGridView.RowDragStarting += OnBookmarksDataGridViewRowDragStarting;
			BookmarksDataGridView.RowDropped += OnBookmarksDataGridViewRowDropped;
			BookmarksDataGridView.QueryRowDragDropEffect += OnBookmarksDataGridViewQueryRowDragDropEffect;
			BookmarksDataGridView.CellDoubleClick += OnBookmarksDataGridViewCellDoubleClick;
			BookmarksDataGridView.CellErrorTextNeeded += OnBookmarksDataGridViewCellErrorTextNeeded;
			BookmarksDataGridView.CellValueNeeded += OnBookmarksDataGridViewCellValueNeeded;
			BookmarksDataGridView.CellValuePushed += OnBookmarksDataGridViewCellValuePushed;
			BookmarksDataGridView.SelectionChanged += OnBookmarksDataGridViewSelectionChanged;
			BookmarksDataGridView.UserDeletedRow += OnBookmarksDataGridViewUserDeletedRow;
			BookmarksDataGridView.UserDeletingRow += OnBookmarksDataGridViewUserDeletingRow;
			// 
			// NameDataGridViewColumn
			// 
			NameDataGridViewColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			NameDataGridViewColumn.DataPropertyName = "Name";
			resources.ApplyResources(NameDataGridViewColumn, "NameDataGridViewColumn");
			NameDataGridViewColumn.Name = "NameDataGridViewColumn";
			NameDataGridViewColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			NameDataGridViewColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// TargetDataGridViewColumn
			// 
			TargetDataGridViewColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			TargetDataGridViewColumn.DataPropertyName = "Target";
			resources.ApplyResources(TargetDataGridViewColumn, "TargetDataGridViewColumn");
			TargetDataGridViewColumn.Name = "TargetDataGridViewColumn";
			TargetDataGridViewColumn.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			TargetDataGridViewColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// BookmarksView
			// 
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			Controls.Add(BookmarksDataGridView);
			HideOnClose = true;
			Name = "BookmarksView";
			BookmarkContextMenu.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)BookmarksDataGridView).EndInit();
			ResumeLayout(false);
		}

		#endregion

		private Controls.DraggableDataGridView BookmarksDataGridView;
		private System.Windows.Forms.ToolStripMenuItem SelectTargetMenuItem;
		private System.Windows.Forms.ToolStripSeparator BookmarkMenuSeparator1;
		private System.Windows.Forms.ToolStripMenuItem CreateNewMenuItem;
		private System.Windows.Forms.ToolStripMenuItem DeleteMenuItem;
		private System.Windows.Forms.ToolStripMenuItem InsertAboveMenuItem;
		private System.Windows.Forms.ToolStripMenuItem InsertBelowMenuItem;
		private System.Windows.Forms.ToolStripSeparator BookmarkMenuSeparator2;
		private System.Windows.Forms.DataGridViewTextBoxColumn NameDataGridViewColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn TargetDataGridViewColumn;
	}
}
