namespace Comical
{
	partial class ImageEditDialog
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
			if (disposing)
			{
				internalImage?.Dispose();
				internalImage = null;
				components?.Dispose();
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
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageEditDialog));
			System.Windows.Forms.Button btnOK;
			System.Windows.Forms.Button btnCancel;
			System.Windows.Forms.TableLayoutPanel tlpPreview;
			hsPreview = new System.Windows.Forms.HScrollBar();
			picPreview = new Controls.FocusablePictureBox();
			vsPreview = new System.Windows.Forms.VScrollBar();
			numMagnifyRatio = new System.Windows.Forms.NumericUpDown();
			lblSize = new System.Windows.Forms.Label();
			lblMagnifyRatio = new System.Windows.Forms.Label();
			btnOK = new System.Windows.Forms.Button();
			btnCancel = new System.Windows.Forms.Button();
			tlpPreview = new System.Windows.Forms.TableLayoutPanel();
			tlpPreview.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)picPreview).BeginInit();
			((System.ComponentModel.ISupportInitialize)numMagnifyRatio).BeginInit();
			SuspendLayout();
			// 
			// lblMagnifyRatio
			// 
			resources.ApplyResources(lblMagnifyRatio, "lblMagnifyRatio");
			lblMagnifyRatio.BackColor = System.Drawing.Color.Transparent;
			lblMagnifyRatio.Name = "lblMagnifyRatio";
			// 
			// btnOK
			// 
			resources.ApplyResources(btnOK, "btnOK");
			btnOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			btnOK.Name = "btnOK";
			btnOK.UseVisualStyleBackColor = true;
			btnOK.Click += btnOK_Click;
			// 
			// btnCancel
			// 
			resources.ApplyResources(btnCancel, "btnCancel");
			btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			btnCancel.Name = "btnCancel";
			btnCancel.UseVisualStyleBackColor = true;
			// 
			// tlpPreview
			// 
			resources.ApplyResources(tlpPreview, "tlpPreview");
			tlpPreview.BackColor = System.Drawing.Color.White;
			tlpPreview.Controls.Add(hsPreview, 0, 1);
			tlpPreview.Controls.Add(picPreview, 0, 0);
			tlpPreview.Controls.Add(vsPreview, 1, 0);
			tlpPreview.Name = "tlpPreview";
			tlpPreview.Resize += RecalculateRequested;
			// 
			// hsPreview
			// 
			resources.ApplyResources(this.hsPreview, "hsPreview");
			this.hsPreview.Name = "hsPreview";
			this.hsPreview.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hsPreview_Scroll);
			// 
			// picPreview
			// 
			picPreview.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(picPreview, "picPreview");
			picPreview.Name = "picPreview";
			picPreview.TabStop = false;
			picPreview.KeyDown += picPreview_KeyDown;
			picPreview.KeyUp += picPreview_KeyUp;
			picPreview.Paint += picPreview_Paint;
			picPreview.MouseDown += picPreview_MouseDown;
			picPreview.MouseLeave += picPreview_MouseLeave;
			picPreview.MouseMove += picPreview_MouseMove;
			picPreview.MouseUp += picPreview_MouseUp;
			// 
			// vsPreview
			// 
			resources.ApplyResources(vsPreview, "vsPreview");
			vsPreview.Name = "vsPreview";
			vsPreview.Scroll += vsPreview_Scroll;
			// 
			// numMagnifyRatio
			// 
			resources.ApplyResources(numMagnifyRatio, "numMagnifyRatio");
			numMagnifyRatio.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			numMagnifyRatio.Name = "numMagnifyRatio";
			numMagnifyRatio.Value = new decimal(new int[] { 100, 0, 0, 0 });
			numMagnifyRatio.ValueChanged += RecalculateRequested;
			// 
			// lblSize
			// 
			resources.ApplyResources(lblSize, "lblSize");
			lblSize.BackColor = System.Drawing.Color.Transparent;
			lblSize.Name = "lblSize";
			// 
			// ImageEditDialog
			// 
			AcceptButton = btnOK;
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			CancelButton = btnCancel;
			resources.ApplyResources(this, "$this");
			Controls.Add(tlpPreview);
			Controls.Add(lblSize);
			Controls.Add(numMagnifyRatio);
			Controls.Add(btnCancel);
			Controls.Add(lblMagnifyRatio);
			Controls.Add(btnOK);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			MaximizeBox = true;
			Name = "ImageEditDialog";
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			Controls.SetChildIndex(btnOK, 0);
			Controls.SetChildIndex(lblMagnifyRatio, 0);
			Controls.SetChildIndex(btnCancel, 0);
			Controls.SetChildIndex(numMagnifyRatio, 0);
			Controls.SetChildIndex(lblSize, 0);
			Controls.SetChildIndex(tlpPreview, 0);
			tlpPreview.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)picPreview).EndInit();
			((System.ComponentModel.ISupportInitialize)numMagnifyRatio).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		Controls.FocusablePictureBox picPreview;
		System.Windows.Forms.NumericUpDown numMagnifyRatio;
		private System.Windows.Forms.Label lblSize;
		private System.Windows.Forms.HScrollBar hsPreview;
		private System.Windows.Forms.VScrollBar vsPreview;

	}
}