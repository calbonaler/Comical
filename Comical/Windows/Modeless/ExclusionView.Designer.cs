namespace Comical
{
	partial class ExclusionView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExclusionView));
			this.dgvTrashBox = new Comical.Controls.DraggableDataGridView();
			this.clmImage = new System.Windows.Forms.DataGridViewImageColumn();
			((System.ComponentModel.ISupportInitialize)(this.dgvTrashBox)).BeginInit();
			this.SuspendLayout();
			// 
			// dgvTrashBox
			// 
			this.dgvTrashBox.AllowUserToAddRows = false;
			this.dgvTrashBox.AllowUserToDeleteRows = false;
			this.dgvTrashBox.AllowUserToMoveRows = true;
			this.dgvTrashBox.AllowUserToResizeColumns = false;
			this.dgvTrashBox.AllowUserToResizeRows = false;
			this.dgvTrashBox.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.dgvTrashBox.BackgroundColor = System.Drawing.SystemColors.Control;
			this.dgvTrashBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dgvTrashBox.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.dgvTrashBox.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.dgvTrashBox.ColumnHeadersVisible = false;
			this.dgvTrashBox.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmImage});
			resources.ApplyResources(this.dgvTrashBox, "dgvTrashBox");
			this.dgvTrashBox.GridColor = System.Drawing.SystemColors.Control;
			this.dgvTrashBox.Name = "dgvTrashBox";
			this.dgvTrashBox.ReadOnly = true;
			this.dgvTrashBox.RowHeadersVisible = false;
			this.dgvTrashBox.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.dgvTrashBox.RowTemplate.Height = 96;
			this.dgvTrashBox.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvTrashBox.ShowCellToolTips = false;
			this.dgvTrashBox.VirtualMode = true;
			this.dgvTrashBox.RowMoving += new System.EventHandler<Comical.Controls.RowMovingEventArgs>(this.dgvTrashBox_RowMoving);
			this.dgvTrashBox.QueryActualDestination += new System.EventHandler<Comical.Controls.QueryActualDestinationEventArgs>(this.dgvTrashBox_QueryActualDestination);
			this.dgvTrashBox.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvTrashBox_CellValueNeeded);
			// 
			// clmImage
			// 
			this.clmImage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(this.clmImage, "clmImage");
			this.clmImage.Name = "clmImage";
			this.clmImage.ReadOnly = true;
			// 
			// ExclusionView
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.dgvTrashBox);
			this.HideOnClose = true;
			this.Name = "ExclusionView";
			((System.ComponentModel.ISupportInitialize)(this.dgvTrashBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridViewImageColumn clmImage;
		private Controls.DraggableDataGridView dgvTrashBox;
	}
}
