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
			System.Windows.Forms.Label lblMagnifyRatio;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageEditingDialog));
			System.Windows.Forms.Button btnOK;
			System.Windows.Forms.Button btnCancel;
			System.Windows.Forms.TableLayoutPanel tlpPreview;
			this.picPreview = new Comical.Controls.FocusablePictureBox();
			this.numMagnifyRatio = new System.Windows.Forms.NumericUpDown();
			this.lblSize = new System.Windows.Forms.Label();
			this.hsPreview = new System.Windows.Forms.HScrollBar();
			this.vsPreview = new System.Windows.Forms.VScrollBar();
			lblMagnifyRatio = new System.Windows.Forms.Label();
			btnOK = new System.Windows.Forms.Button();
			btnCancel = new System.Windows.Forms.Button();
			tlpPreview = new System.Windows.Forms.TableLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numMagnifyRatio)).BeginInit();
			tlpPreview.SuspendLayout();
			this.SuspendLayout();
			// 
			// lblMagnifyRatio
			// 
			resources.ApplyResources(lblMagnifyRatio, "lblMagnifyRatio");
			lblMagnifyRatio.BackColor = System.Drawing.Color.Transparent;
			lblMagnifyRatio.Name = "lblMagnifyRatio";
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
			resources.ApplyResources(btnOK, "btnOK");
			btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			btnOK.Name = "btnOK";
			btnOK.UseVisualStyleBackColor = true;
			btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			resources.ApplyResources(btnCancel, "btnCancel");
			btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			btnCancel.Name = "btnCancel";
			btnCancel.UseVisualStyleBackColor = true;
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
			resources.ApplyResources(tlpPreview, "tlpPreview");
			tlpPreview.BackColor = System.Drawing.Color.White;
			tlpPreview.Controls.Add(this.hsPreview, 0, 1);
			tlpPreview.Controls.Add(this.picPreview, 0, 0);
			tlpPreview.Controls.Add(this.vsPreview, 1, 0);
			tlpPreview.Name = "tlpPreview";
			tlpPreview.Resize += new System.EventHandler(this.RecalculateRequested);
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
			this.AcceptButton = btnOK;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = btnCancel;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(tlpPreview);
			this.Controls.Add(this.lblSize);
			this.Controls.Add(this.numMagnifyRatio);
			this.Controls.Add(btnCancel);
			this.Controls.Add(lblMagnifyRatio);
			this.Controls.Add(btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			this.MaximizeBox = true;
			this.Name = "ImageEditingDialog";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Controls.SetChildIndex(btnOK, 0);
			this.Controls.SetChildIndex(lblMagnifyRatio, 0);
			this.Controls.SetChildIndex(btnCancel, 0);
			this.Controls.SetChildIndex(this.numMagnifyRatio, 0);
			this.Controls.SetChildIndex(this.lblSize, 0);
			this.Controls.SetChildIndex(tlpPreview, 0);
			((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numMagnifyRatio)).EndInit();
			tlpPreview.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		Controls.FocusablePictureBox picPreview;
		System.Windows.Forms.NumericUpDown numMagnifyRatio;
		private System.Windows.Forms.Label lblSize;
		private System.Windows.Forms.HScrollBar hsPreview;
		private System.Windows.Forms.VScrollBar vsPreview;

	}
}