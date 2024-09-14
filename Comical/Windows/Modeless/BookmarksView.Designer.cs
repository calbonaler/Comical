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
				_bookmarks.CollectionChanged -= Bookmarks_CollectionChanged;
				_images.CollectionChanged -= Images_CollectionChanged;
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
			System.Windows.Forms.ContextMenuStrip conBookmark;
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(BookmarksView));
			itmSelectTarget = new System.Windows.Forms.ToolStripMenuItem();
			sepBookmark1 = new System.Windows.Forms.ToolStripSeparator();
			itmCreateNew = new System.Windows.Forms.ToolStripMenuItem();
			itmInsertAbove = new System.Windows.Forms.ToolStripMenuItem();
			itmInsertBelow = new System.Windows.Forms.ToolStripMenuItem();
			sepBookmark2 = new System.Windows.Forms.ToolStripSeparator();
			itmDelete = new System.Windows.Forms.ToolStripMenuItem();
			dgvBookmarks = new Controls.DraggableDataGridView();
			clmName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			clmTarget = new System.Windows.Forms.DataGridViewTextBoxColumn();
			conBookmark = new System.Windows.Forms.ContextMenuStrip(components);
			conBookmark.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)dgvBookmarks).BeginInit();
			SuspendLayout();
			// 
			// conBookmark
			// 
			conBookmark.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { itmSelectTarget, sepBookmark1, itmCreateNew, itmInsertAbove, itmInsertBelow, sepBookmark2, itmDelete });
			conBookmark.Name = "conBookmark";
			resources.ApplyResources(conBookmark, "conBookmark");
			// 
			// itmSelectTarget
			// 
			resources.ApplyResources(itmSelectTarget, "itmSelectTarget");
			itmSelectTarget.Name = "itmSelectTarget";
			itmSelectTarget.Click += itmSelectTarget_Click;
			// 
			// sepBookmark1
			// 
			sepBookmark1.Name = "sepBookmark1";
			resources.ApplyResources(sepBookmark1, "sepBookmark1");
			// 
			// itmCreateNew
			// 
			itmCreateNew.Name = "itmCreateNew";
			resources.ApplyResources(itmCreateNew, "itmCreateNew");
			itmCreateNew.Click += itmAdd_Click;
			// 
			// itmInsertAbove
			// 
			itmInsertAbove.Name = "itmInsertAbove";
			resources.ApplyResources(itmInsertAbove, "itmInsertAbove");
			itmInsertAbove.Click += itmInsertAbove_Click;
			// 
			// itmInsertBelow
			// 
			itmInsertBelow.Name = "itmInsertBelow";
			resources.ApplyResources(itmInsertBelow, "itmInsertBelow");
			itmInsertBelow.Click += itmInsertBelow_Click;
			// 
			// sepBookmark2
			// 
			sepBookmark2.Name = "sepBookmark2";
			resources.ApplyResources(sepBookmark2, "sepBookmark2");
			// 
			// itmDelete
			// 
			resources.ApplyResources(itmDelete, "itmDelete");
			itmDelete.Name = "itmDelete";
			itmDelete.Click += itmRemove_Click;
			// 
			// dgvBookmarks
			// 
			dgvBookmarks.AllowDrop = true;
			dgvBookmarks.AllowUserToAddRows = false;
			dgvBookmarks.AllowUserToMoveRows = true;
			dgvBookmarks.AllowUserToResizeColumns = false;
			dgvBookmarks.AllowUserToResizeRows = false;
			dgvBookmarks.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			dgvBookmarks.BackgroundColor = System.Drawing.SystemColors.Control;
			dgvBookmarks.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			dgvBookmarks.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			dgvBookmarks.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dgvBookmarks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			dgvBookmarks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { clmName, clmTarget });
			dgvBookmarks.ContextMenuStrip = conBookmark;
			resources.ApplyResources(dgvBookmarks, "dgvBookmarks");
			dgvBookmarks.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
			dgvBookmarks.GridColor = System.Drawing.SystemColors.Control;
			dgvBookmarks.Name = "dgvBookmarks";
			dgvBookmarks.RowHeadersVisible = false;
			dgvBookmarks.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			dgvBookmarks.RowTemplate.Height = 21;
			dgvBookmarks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			dgvBookmarks.ShowCellToolTips = false;
			dgvBookmarks.VirtualMode = true;
			dgvBookmarks.RowMoving += dgvBookmarks_RowMoving;
			dgvBookmarks.QueryRowDragDropEffect += dgvBookmarks_QueryRowDragDropEffect;
			dgvBookmarks.CellDoubleClick += dgvBookmarks_CellDoubleClick;
			dgvBookmarks.CellErrorTextNeeded += dgvBookmarks_CellErrorTextNeeded;
			dgvBookmarks.CellValueNeeded += dgvBookmarks_CellValueNeeded;
			dgvBookmarks.CellValuePushed += dgvBookmarks_CellValuePushed;
			dgvBookmarks.SelectionChanged += dgvBookmarks_SelectionChanged;
			dgvBookmarks.UserDeletedRow += dgvBookmarks_UserDeletedRow;
			dgvBookmarks.UserDeletingRow += dgvBookmarks_UserDeletingRow;
			// 
			// clmName
			// 
			clmName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			clmName.DataPropertyName = "Name";
			resources.ApplyResources(clmName, "clmName");
			clmName.Name = "clmName";
			clmName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			clmName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// clmTarget
			// 
			clmTarget.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			clmTarget.DataPropertyName = "Target";
			resources.ApplyResources(clmTarget, "clmTarget");
			clmTarget.Name = "clmTarget";
			clmTarget.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			clmTarget.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// BookmarksView
			// 
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			Controls.Add(dgvBookmarks);
			HideOnClose = true;
			Name = "BookmarksView";
			conBookmark.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)dgvBookmarks).EndInit();
			ResumeLayout(false);
		}

		#endregion

		private Controls.DraggableDataGridView dgvBookmarks;
		private System.Windows.Forms.ToolStripMenuItem itmSelectTarget;
		private System.Windows.Forms.ToolStripSeparator sepBookmark1;
		private System.Windows.Forms.ToolStripMenuItem itmCreateNew;
		private System.Windows.Forms.ToolStripMenuItem itmDelete;
		private System.Windows.Forms.ToolStripMenuItem itmInsertAbove;
		private System.Windows.Forms.ToolStripMenuItem itmInsertBelow;
		private System.Windows.Forms.ToolStripSeparator sepBookmark2;
		private System.Windows.Forms.DataGridViewTextBoxColumn clmName;
		private System.Windows.Forms.DataGridViewTextBoxColumn clmTarget;
	}
}
