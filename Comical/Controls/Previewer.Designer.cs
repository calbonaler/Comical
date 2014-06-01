namespace Comical.Controls
{
	partial class Previewer
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

		#region コンポーネント デザイナーで生成されたコード

		/// <summary> 
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent()
		{
			this.picPreview = new Comical.Controls.FocusablePictureBox();
			((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
			this.SuspendLayout();
			// 
			// picPreview
			// 
			this.picPreview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picPreview.Location = new System.Drawing.Point(0, 0);
			this.picPreview.Name = "picPreview";
			this.picPreview.Size = new System.Drawing.Size(150, 150);
			this.picPreview.TabIndex = 0;
			this.picPreview.TabStop = false;
			this.picPreview.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.picPreview_LoadCompleted);
			this.picPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.picPreview_Paint);
			this.picPreview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picPreview_MouseDown);
			this.picPreview.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picPreview_MouseMove);
			this.picPreview.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picPreview_MouseUp);
			// 
			// Previewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.Controls.Add(this.picPreview);
			this.Name = "Previewer";
			((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Comical.Controls.FocusablePictureBox picPreview;
	}
}
