namespace Comical
{
	partial class ViewerForm
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
			if (disposing && (components != null))
			{
				components.Dispose();
				_comic.Dispose();
				_closeBrush.Dispose();
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
			components = new System.ComponentModel.Container();
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewerForm));
			BookmarksContextMenu = new System.Windows.Forms.ContextMenuStrip(components);
			MainPreviewer = new Controls.Previewer();
			SuspendLayout();
			// 
			// BookmarksContextMenu
			// 
			BookmarksContextMenu.Name = "contextMenuStrip1";
			BookmarksContextMenu.ShowImageMargin = false;
			BookmarksContextMenu.Size = new System.Drawing.Size(36, 4);
			// 
			// MainPreviewer
			// 
			MainPreviewer.AutoScroll = true;
			MainPreviewer.BackColor = System.Drawing.Color.Transparent;
			MainPreviewer.Dock = System.Windows.Forms.DockStyle.Fill;
			MainPreviewer.Location = new System.Drawing.Point(0, 0);
			MainPreviewer.Margin = new System.Windows.Forms.Padding(4);
			MainPreviewer.Name = "MainPreviewer";
			MainPreviewer.Size = new System.Drawing.Size(331, 328);
			MainPreviewer.StretchMode = Comical.Controls.PreviewerStretchMode.Uniform;
			MainPreviewer.TabIndex = 0;
			// 
			// ViewerForm
			// 
			AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			BackColor = System.Drawing.Color.Black;
			ClientSize = new System.Drawing.Size(331, 328);
			ControlBox = false;
			Controls.Add(MainPreviewer);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
			Margin = new System.Windows.Forms.Padding(4);
			Name = "ViewerForm";
			TopMost = true;
			ResumeLayout(false);
		}

		#endregion

		private Controls.Previewer MainPreviewer;
		private System.Windows.Forms.ContextMenuStrip BookmarksContextMenu;


	}
}