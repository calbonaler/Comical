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
				icd.Dispose();
				closeBrush.Dispose();
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewerForm));
			this.conBookmarks = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.prevMain = new Comical.Controls.Previewer();
			this.SuspendLayout();
			// 
			// conBookmarks
			// 
			this.conBookmarks.Name = "contextMenuStrip1";
			this.conBookmarks.ShowImageMargin = false;
			this.conBookmarks.Size = new System.Drawing.Size(36, 4);
			// 
			// prevMain
			// 
			this.prevMain.AutoScroll = true;
			this.prevMain.BackColor = System.Drawing.Color.Transparent;
			this.prevMain.Description = null;
			this.prevMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.prevMain.Image = null;
			this.prevMain.Location = new System.Drawing.Point(0, 0);
			this.prevMain.Name = "prevMain";
			this.prevMain.Size = new System.Drawing.Size(284, 262);
			this.prevMain.StretchMode = Comical.Controls.PreviewerStretchMode.Uniform;
			this.prevMain.TabIndex = 0;
			// 
			// frmViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.Black;
			this.ClientSize = new System.Drawing.Size(284, 262);
			this.ControlBox = false;
			this.Controls.Add(this.prevMain);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "frmViewer";
			this.ShowInTaskbar = false;
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.ResumeLayout(false);

		}

		#endregion

		private Controls.Previewer prevMain;
		private System.Windows.Forms.ContextMenuStrip conBookmarks;


	}
}