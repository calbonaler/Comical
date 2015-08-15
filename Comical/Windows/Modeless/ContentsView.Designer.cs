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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContentsView));
			this.conImage = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.itmOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.itmDelete = new System.Windows.Forms.ToolStripMenuItem();
			this.sepImage1 = new System.Windows.Forms.ToolStripSeparator();
			this.itmExport = new System.Windows.Forms.ToolStripMenuItem();
			this.itmExtract = new System.Windows.Forms.ToolStripMenuItem();
			this.sepImage2 = new System.Windows.Forms.ToolStripSeparator();
			this.itmAddToBookmark = new System.Windows.Forms.ToolStripMenuItem();
			this.dgvImages = new Comical.Controls.DraggableDataGridView();
			this.clmImage = new System.Windows.Forms.DataGridViewImageColumn();
			this.clmViewMode = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.conImage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvImages)).BeginInit();
			this.SuspendLayout();
			// 
			// conImage
			// 
			this.conImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmOpen,
            this.itmDelete,
            this.sepImage1,
            this.itmExport,
            this.itmExtract,
            this.sepImage2,
            this.itmAddToBookmark});
			this.conImage.Name = "conImage";
			resources.ApplyResources(this.conImage, "conImage");
			// 
			// itmOpen
			// 
			resources.ApplyResources(this.itmOpen, "itmOpen");
			this.itmOpen.Name = "itmOpen";
			this.itmOpen.Click += new System.EventHandler(this.itmOpen_Click);
			// 
			// itmDelete
			// 
			resources.ApplyResources(this.itmDelete, "itmDelete");
			this.itmDelete.Name = "itmDelete";
			this.itmDelete.Click += new System.EventHandler(this.itmDelete_Click);
			// 
			// sepImage1
			// 
			this.sepImage1.Name = "sepImage1";
			resources.ApplyResources(this.sepImage1, "sepImage1");
			// 
			// itmExport
			// 
			this.itmExport.Name = "itmExport";
			resources.ApplyResources(this.itmExport, "itmExport");
			// 
			// itmExtract
			// 
			this.itmExtract.Name = "itmExtract";
			resources.ApplyResources(this.itmExtract, "itmExtract");
			// 
			// sepImage2
			// 
			this.sepImage2.Name = "sepImage2";
			resources.ApplyResources(this.sepImage2, "sepImage2");
			// 
			// itmAddToBookmark
			// 
			this.itmAddToBookmark.Name = "itmAddToBookmark";
			resources.ApplyResources(this.itmAddToBookmark, "itmAddToBookmark");
			// 
			// dgvImages
			// 
			this.dgvImages.AllowDrop = true;
			this.dgvImages.AllowUserToAddRows = false;
			this.dgvImages.AllowUserToMoveRows = true;
			this.dgvImages.AllowUserToResizeColumns = false;
			this.dgvImages.AllowUserToResizeRows = false;
			this.dgvImages.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvImages.BackgroundColor = System.Drawing.SystemColors.Control;
			this.dgvImages.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dgvImages.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.dgvImages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvImages.ColumnHeadersVisible = false;
			this.dgvImages.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmImage,
            this.clmViewMode});
			this.dgvImages.ContextMenuStrip = this.conImage;
			resources.ApplyResources(this.dgvImages, "dgvImages");
			this.dgvImages.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.dgvImages.GridColor = System.Drawing.SystemColors.Control;
			this.dgvImages.MultiDrag = true;
			this.dgvImages.Name = "dgvImages";
			this.dgvImages.RowHeadersVisible = false;
			this.dgvImages.RowTemplate.Height = 42;
			this.dgvImages.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvImages.ShowCellToolTips = false;
			this.dgvImages.VirtualMode = true;
			this.dgvImages.RowMoving += new System.EventHandler<Comical.Controls.RowMovingEventArgs>(this.dgvImages_RowMoving);
			this.dgvImages.QueryActualDestination += new System.EventHandler<Comical.Controls.QueryActualDestinationEventArgs>(this.dgvImages_QueryActualDestination);
			this.dgvImages.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvImages_CellDoubleClick);
			this.dgvImages.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvImages_CellValueNeeded);
			this.dgvImages.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvImages_CellValuePushed);
			this.dgvImages.SelectionChanged += new System.EventHandler(this.dgvImages_SelectionChanged);
			this.dgvImages.UserDeletedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dgvImages_UserDeletedRow);
			this.dgvImages.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dgvImages_UserDeletingRow);
			this.dgvImages.DragDrop += new System.Windows.Forms.DragEventHandler(this.dgvImages_DragDrop);
			this.dgvImages.DragEnter += new System.Windows.Forms.DragEventHandler(this.dgvImages_DragEnter);
			// 
			// clmImage
			// 
			this.clmImage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(this.clmImage, "clmImage");
			this.clmImage.Name = "clmImage";
			this.clmImage.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// clmViewMode
			// 
			this.clmViewMode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.clmViewMode.FillWeight = 80F;
			resources.ApplyResources(this.clmViewMode, "clmViewMode");
			this.clmViewMode.Name = "clmViewMode";
			// 
			// ContentsView
			// 
			this.AllowDrop = true;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.dgvImages);
			this.HideOnClose = true;
			this.Name = "ContentsView";
			this.conImage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dgvImages)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridViewImageColumn clmImage;
		private System.Windows.Forms.DataGridViewComboBoxColumn clmViewMode;
		private Controls.DraggableDataGridView dgvImages;
		private System.Windows.Forms.ContextMenuStrip conImage;
		private System.Windows.Forms.ToolStripMenuItem itmOpen;
		private System.Windows.Forms.ToolStripMenuItem itmDelete;
		private System.Windows.Forms.ToolStripSeparator sepImage1;
		private System.Windows.Forms.ToolStripMenuItem itmExport;
		private System.Windows.Forms.ToolStripMenuItem itmExtract;
		private System.Windows.Forms.ToolStripSeparator sepImage2;
		private System.Windows.Forms.ToolStripMenuItem itmAddToBookmark;
	}
}
