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
				_images.CollectionChanged -= Images_CollectionChanged;
				_images.CollectionItemPropertyChanged -= Images_CollectionItemPropertyChanged;
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
			System.Windows.Forms.ContextMenuStrip conImage;
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(ContentsView));
			itmOpen = new System.Windows.Forms.ToolStripMenuItem();
			sepImage1 = new System.Windows.Forms.ToolStripSeparator();
			itmAddToBookmark = new System.Windows.Forms.ToolStripMenuItem();
			sepImage2 = new System.Windows.Forms.ToolStripSeparator();
			itmExport = new System.Windows.Forms.ToolStripMenuItem();
			itmExtract = new System.Windows.Forms.ToolStripMenuItem();
			sepImage3 = new System.Windows.Forms.ToolStripSeparator();
			itmStartViewModeSettingLeft = new System.Windows.Forms.ToolStripMenuItem();
			itmStartViewModeSettingRight = new System.Windows.Forms.ToolStripMenuItem();
			sepImage4 = new System.Windows.Forms.ToolStripSeparator();
			itmDelete = new System.Windows.Forms.ToolStripMenuItem();
			dgvImages = new Controls.DraggableDataGridView();
			clmImage = new System.Windows.Forms.DataGridViewImageColumn();
			clmViewMode = new System.Windows.Forms.DataGridViewComboBoxColumn();
			conImage = new System.Windows.Forms.ContextMenuStrip(components);
			conImage.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)dgvImages).BeginInit();
			SuspendLayout();
			// 
			// conImage
			// 
			conImage.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { itmOpen, sepImage1, itmAddToBookmark, sepImage2, itmExport, itmExtract, sepImage3, itmStartViewModeSettingLeft, itmStartViewModeSettingRight, sepImage4, itmDelete });
			conImage.Name = "conImage";
			resources.ApplyResources(conImage, "conImage");
			// 
			// itmOpen
			// 
			resources.ApplyResources(itmOpen, "itmOpen");
			itmOpen.Name = "itmOpen";
			itmOpen.Click += itmOpen_Click;
			// 
			// sepImage1
			// 
			sepImage1.Name = "sepImage1";
			resources.ApplyResources(sepImage1, "sepImage1");
			// 
			// itmAddToBookmark
			// 
			itmAddToBookmark.Name = "itmAddToBookmark";
			resources.ApplyResources(itmAddToBookmark, "itmAddToBookmark");
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
			// 
			// itmExtract
			// 
			resources.ApplyResources(itmExtract, "itmExtract");
			itmExtract.Name = "itmExtract";
			// 
			// sepImage3
			// 
			sepImage3.Name = "sepImage3";
			resources.ApplyResources(sepImage3, "sepImage3");
			// 
			// itmStartViewModeSettingLeft
			// 
			itmStartViewModeSettingLeft.Name = "itmStartViewModeSettingLeft";
			resources.ApplyResources(itmStartViewModeSettingLeft, "itmStartViewModeSettingLeft");
			itmStartViewModeSettingLeft.Click += itmStartViewModeSettingLeft_Click;
			// 
			// itmStartViewModeSettingRight
			// 
			itmStartViewModeSettingRight.Name = "itmStartViewModeSettingRight";
			resources.ApplyResources(itmStartViewModeSettingRight, "itmStartViewModeSettingRight");
			itmStartViewModeSettingRight.Click += itmStartViewModeSettingRight_Click;
			// 
			// sepImage4
			// 
			sepImage4.Name = "sepImage4";
			resources.ApplyResources(sepImage4, "sepImage4");
			// 
			// itmDelete
			// 
			resources.ApplyResources(itmDelete, "itmDelete");
			itmDelete.Name = "itmDelete";
			itmDelete.Click += itmDelete_Click;
			// 
			// dgvImages
			// 
			dgvImages.AllowDrop = true;
			dgvImages.AllowUserToAddRows = false;
			dgvImages.AllowUserToMoveRows = true;
			dgvImages.AllowUserToResizeColumns = false;
			dgvImages.AllowUserToResizeRows = false;
			dgvImages.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			dgvImages.BackgroundColor = System.Drawing.SystemColors.Control;
			dgvImages.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			dgvImages.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			dgvImages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			dgvImages.ColumnHeadersVisible = false;
			dgvImages.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { clmImage, clmViewMode });
			dgvImages.ContextMenuStrip = conImage;
			resources.ApplyResources(dgvImages, "dgvImages");
			dgvImages.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			dgvImages.GridColor = System.Drawing.SystemColors.Control;
			dgvImages.MultiDrag = true;
			dgvImages.Name = "dgvImages";
			dgvImages.RowHeadersVisible = false;
			dgvImages.RowTemplate.Height = 42;
			dgvImages.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			dgvImages.ShowCellToolTips = false;
			dgvImages.VirtualMode = true;
			dgvImages.RowMoving += dgvImages_RowMoving;
			dgvImages.QueryRowDragDropEffect += dgvImages_QueryRowDragDropEffect;
			dgvImages.CellDoubleClick += dgvImages_CellDoubleClick;
			dgvImages.CellValueNeeded += dgvImages_CellValueNeeded;
			dgvImages.CellValuePushed += dgvImages_CellValuePushed;
			dgvImages.SelectionChanged += dgvImages_SelectionChanged;
			dgvImages.UserDeletedRow += dgvImages_UserDeletedRow;
			dgvImages.UserDeletingRow += dgvImages_UserDeletingRow;
			dgvImages.DragDrop += dgvImages_DragDrop;
			dgvImages.DragEnter += dgvImages_DragEnter;
			// 
			// clmImage
			// 
			clmImage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(clmImage, "clmImage");
			clmImage.Name = "clmImage";
			clmImage.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			// 
			// clmViewMode
			// 
			clmViewMode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			clmViewMode.FillWeight = 80F;
			resources.ApplyResources(clmViewMode, "clmViewMode");
			clmViewMode.Name = "clmViewMode";
			// 
			// ContentsView
			// 
			AllowDrop = true;
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			Controls.Add(dgvImages);
			HideOnClose = true;
			Name = "ContentsView";
			conImage.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)dgvImages).EndInit();
			ResumeLayout(false);
		}

		#endregion
		private Controls.DraggableDataGridView dgvImages;
		private System.Windows.Forms.ToolStripMenuItem itmOpen;
		private System.Windows.Forms.ToolStripMenuItem itmDelete;
		private System.Windows.Forms.ToolStripSeparator sepImage1;
		private System.Windows.Forms.ToolStripMenuItem itmExport;
		private System.Windows.Forms.ToolStripMenuItem itmExtract;
		private System.Windows.Forms.ToolStripSeparator sepImage2;
		private System.Windows.Forms.ToolStripMenuItem itmAddToBookmark;
		private System.Windows.Forms.ToolStripSeparator sepImage3;
		private System.Windows.Forms.ToolStripMenuItem itmStartViewModeSettingLeft;
		private System.Windows.Forms.ToolStripMenuItem itmStartViewModeSettingRight;
		private System.Windows.Forms.ToolStripSeparator sepImage4;
		private System.Windows.Forms.DataGridViewImageColumn clmImage;
		private System.Windows.Forms.DataGridViewComboBoxColumn clmViewMode;
	}
}
