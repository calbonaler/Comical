namespace Comical
{
	partial class ImageEditingDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageEditingDialog));
			this.lblMagnifyRatio = new System.Windows.Forms.Label();
			this.picPreview = new Comical.Controls.FocusablePictureBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.numMagnifyRatio = new System.Windows.Forms.NumericUpDown();
			this.lblSize = new System.Windows.Forms.Label();
			this.tlpPreview = new System.Windows.Forms.TableLayoutPanel();
			this.hsPreview = new System.Windows.Forms.HScrollBar();
			this.vsPreview = new System.Windows.Forms.VScrollBar();
			((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numMagnifyRatio)).BeginInit();
			this.tlpPreview.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblMagnifyRatio
			// 
			resources.ApplyResources(this.lblMagnifyRatio, "lblMagnifyRatio");
			this.lblMagnifyRatio.BackColor = System.Drawing.Color.Transparent;
			this.lblMagnifyRatio.Name = "lblMagnifyRatio";
			// 
			// picPreview
			// 
			this.picPreview.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(this.picPreview, "picPreview");
			this.picPreview.Name = "picPreview";
			this.picPreview.TabStop = false;
			this.picPreview.KeyDown += new System.Windows.Forms.KeyEventHandler(this.picPreview_KeyDown);
			this.picPreview.KeyUp += new System.Windows.Forms.KeyEventHandler(this.picPreview_KeyUp);
			this.picPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.picPreview_Paint);
			this.picPreview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picPreview_MouseDown);
			this.picPreview.MouseLeave += new System.EventHandler(this.picPreview_MouseLeave);
			this.picPreview.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picPreview_MouseMove);
			this.picPreview.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picPreview_MouseUp);
			// 
			// btnOK
			// 
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnOK.Name = "btnOK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// numMagnifyRatio
			// 
			resources.ApplyResources(this.numMagnifyRatio, "numMagnifyRatio");
			this.numMagnifyRatio.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numMagnifyRatio.Name = "numMagnifyRatio";
			this.numMagnifyRatio.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.numMagnifyRatio.ValueChanged += new System.EventHandler(this.RecalculateRequested);
			// 
			// lblSize
			// 
			resources.ApplyResources(this.lblSize, "lblSize");
			this.lblSize.BackColor = System.Drawing.Color.Transparent;
			this.lblSize.Name = "lblSize";
			// 
			// tlpPreview
			// 
			resources.ApplyResources(this.tlpPreview, "tlpPreview");
			this.tlpPreview.BackColor = System.Drawing.Color.White;
			this.tlpPreview.Controls.Add(this.hsPreview, 0, 1);
			this.tlpPreview.Controls.Add(this.picPreview, 0, 0);
			this.tlpPreview.Controls.Add(this.vsPreview, 1, 0);
			this.tlpPreview.Name = "tlpPreview";
			this.tlpPreview.Resize += new System.EventHandler(this.RecalculateRequested);
			// 
			// hsPreview
			// 
			resources.ApplyResources(this.hsPreview, "hsPreview");
			this.hsPreview.Name = "hsPreview";
			this.hsPreview.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hsPreview_Scroll);
			// 
			// vsPreview
			// 
			resources.ApplyResources(this.vsPreview, "vsPreview");
			this.vsPreview.Name = "vsPreview";
			this.vsPreview.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vsPreview_Scroll);
			// 
			// ImageEditingDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.btnCancel;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tlpPreview);
			this.Controls.Add(this.lblSize);
			this.Controls.Add(this.numMagnifyRatio);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.lblMagnifyRatio);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			this.MaximizeBox = true;
			this.Name = "ImageEditingDialog";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Controls.SetChildIndex(this.btnOK, 0);
			this.Controls.SetChildIndex(this.lblMagnifyRatio, 0);
			this.Controls.SetChildIndex(this.btnCancel, 0);
			this.Controls.SetChildIndex(this.numMagnifyRatio, 0);
			this.Controls.SetChildIndex(this.lblSize, 0);
			this.Controls.SetChildIndex(this.tlpPreview, 0);
			((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numMagnifyRatio)).EndInit();
			this.tlpPreview.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		Controls.FocusablePictureBox picPreview;
		System.Windows.Forms.NumericUpDown numMagnifyRatio;
		private System.Windows.Forms.Label lblMagnifyRatio;
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblSize;
		private System.Windows.Forms.TableLayoutPanel tlpPreview;
		private System.Windows.Forms.HScrollBar hsPreview;
		private System.Windows.Forms.VScrollBar vsPreview;

	}
}