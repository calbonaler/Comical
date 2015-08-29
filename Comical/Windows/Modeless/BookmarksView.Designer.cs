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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ContextMenuStrip conBookmark;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BookmarksView));
			this.itmSelectTarget = new System.Windows.Forms.ToolStripMenuItem();
			this.sepBookmark1 = new System.Windows.Forms.ToolStripSeparator();
			this.itmCreateNew = new System.Windows.Forms.ToolStripMenuItem();
			this.itmInsertAbove = new System.Windows.Forms.ToolStripMenuItem();
			this.itmInsertBelow = new System.Windows.Forms.ToolStripMenuItem();
			this.sepBookmark2 = new System.Windows.Forms.ToolStripSeparator();
			this.itmDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.dgvBookmarks = new Comical.Controls.DraggableDataGridView();
			this.clmName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.clmTarget = new System.Windows.Forms.DataGridViewTextBoxColumn();
			conBookmark = new System.Windows.Forms.ContextMenuStrip(this.components);
			conBookmark.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvBookmarks)).BeginInit();
			this.SuspendLayout();
			// 
			// conBookmark
			// 
			resources.ApplyResources(conBookmark, "conBookmark");
			conBookmark.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmSelectTarget,
            this.sepBookmark1,
            this.itmCreateNew,
            this.itmInsertAbove,
            this.itmInsertBelow,
            this.sepBookmark2,
            this.itmDelete});
			conBookmark.Name = "conBookmark";
			// 
			// itmSelectTarget
			// 
			resources.ApplyResources(this.itmSelectTarget, "itmSelectTarget");
			this.itmSelectTarget.Name = "itmSelectTarget";
			this.itmSelectTarget.Click += new System.EventHandler(this.itmSelectTarget_Click);
			// 
			// sepBookmark1
			// 
			resources.ApplyResources(this.sepBookmark1, "sepBookmark1");
			this.sepBookmark1.Name = "sepBookmark1";
			// 
			// itmCreateNew
			// 
			resources.ApplyResources(this.itmCreateNew, "itmCreateNew");
			this.itmCreateNew.Name = "itmCreateNew";
			this.itmCreateNew.Click += new System.EventHandler(this.itmAdd_Click);
			// 
			// itmInsertAbove
			// 
			resources.ApplyResources(this.itmInsertAbove, "itmInsertAbove");
			this.itmInsertAbove.Name = "itmInsertAbove";
			this.itmInsertAbove.Click += new System.EventHandler(this.itmInsertAbove_Click);
			// 
			// itmInsertBelow
			// 
			resources.ApplyResources(this.itmInsertBelow, "itmInsertBelow");
			this.itmInsertBelow.Name = "itmInsertBelow";
			this.itmInsertBelow.Click += new System.EventHandler(this.itmInsertBelow_Click);
			// 
			// sepBookmark2
			// 
			resources.ApplyResources(this.sepBookmark2, "sepBookmark2");
			this.sepBookmark2.Name = "sepBookmark2";
			// 
			// itmDelete
			// 
			resources.ApplyResources(this.itmDelete, "itmDelete");
			this.itmDelete.Name = "itmDelete";
			this.itmDelete.Click += new System.EventHandler(this.itmRemove_Click);
			// 
			// dgvBookmarks
			// 
			resources.ApplyResources(this.dgvBookmarks, "dgvBookmarks");
			this.dgvBookmarks.AllowDrop = true;
			this.dgvBookmarks.AllowUserToAddRows = false;
			this.dgvBookmarks.AllowUserToMoveRows = true;
			this.dgvBookmarks.AllowUserToResizeColumns = false;
			this.dgvBookmarks.AllowUserToResizeRows = false;
			this.dgvBookmarks.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvBookmarks.BackgroundColor = System.Drawing.SystemColors.Control;
			this.dgvBookmarks.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dgvBookmarks.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.dgvBookmarks.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			this.dgvBookmarks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.dgvBookmarks.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmName,
            this.clmTarget});
			this.dgvBookmarks.ContextMenuStrip = conBookmark;
			this.dgvBookmarks.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnF2;
			this.dgvBookmarks.GridColor = System.Drawing.SystemColors.Control;
			this.dgvBookmarks.Name = "dgvBookmarks";
			this.dgvBookmarks.RowHeadersVisible = false;
			this.dgvBookmarks.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.dgvBookmarks.RowTemplate.Height = 21;
			this.dgvBookmarks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvBookmarks.ShowCellToolTips = false;
			this.dgvBookmarks.VirtualMode = true;
			this.dgvBookmarks.RowMoving += new System.EventHandler<Comical.Controls.RowMovingEventArgs>(this.dgvBookmarks_RowMoving);
			this.dgvBookmarks.QueryActualDestination += new System.EventHandler<Comical.Controls.QueryActualDestinationEventArgs>(this.dgvBookmarks_QueryActualDestination);
			this.dgvBookmarks.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvBookmarks_CellDoubleClick);
			this.dgvBookmarks.CellErrorTextNeeded += new System.Windows.Forms.DataGridViewCellErrorTextNeededEventHandler(this.dgvBookmarks_CellErrorTextNeeded);
			this.dgvBookmarks.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvBookmarks_CellValueNeeded);
			this.dgvBookmarks.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvBookmarks_CellValuePushed);
			this.dgvBookmarks.SelectionChanged += new System.EventHandler(this.dgvBookmarks_SelectionChanged);
			this.dgvBookmarks.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgvBookmarks_UserDeletedRow);
			this.dgvBookmarks.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgvBookmarks_UserDeletingRow);
			// 
			// clmName
			// 
			this.clmName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.clmName.DataPropertyName = "Name";
			resources.ApplyResources(this.clmName, "clmName");
			this.clmName.Name = "clmName";
			this.clmName.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.clmName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// clmTarget
			// 
			this.clmTarget.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.clmTarget.DataPropertyName = "Target";
			resources.ApplyResources(this.clmTarget, "clmTarget");
			this.clmTarget.Name = "clmTarget";
			this.clmTarget.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.clmTarget.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// BookmarksView
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.dgvBookmarks);
			this.HideOnClose = true;
			this.Name = "BookmarksView";
			conBookmark.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvBookmarks)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Controls.DraggableDataGridView dgvBookmarks;
		private System.Windows.Forms.DataGridViewTextBoxColumn clmName;
		private System.Windows.Forms.DataGridViewTextBoxColumn clmTarget;
		private System.Windows.Forms.ToolStripMenuItem itmSelectTarget;
		private System.Windows.Forms.ToolStripSeparator sepBookmark1;
		private System.Windows.Forms.ToolStripMenuItem itmCreateNew;
		private System.Windows.Forms.ToolStripMenuItem itmDelete;
		private System.Windows.Forms.ToolStripMenuItem itmInsertAbove;
		private System.Windows.Forms.ToolStripMenuItem itmInsertBelow;
		private System.Windows.Forms.ToolStripSeparator sepBookmark2;
	}
}
