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
			System.Windows.Forms.Label MagnifyRatioLabel;
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageEditDialog));
			System.Windows.Forms.Button OKButton;
			System.Windows.Forms.Button CancelButton;
			MagnifyRatioNumericUpDown = new System.Windows.Forms.NumericUpDown();
			SizeLabel = new System.Windows.Forms.Label();
			PreviewBox = new Comical.Controls.ScrollBaredControl();
			MagnifyRatioLabel = new System.Windows.Forms.Label();
			OKButton = new System.Windows.Forms.Button();
			CancelButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)MagnifyRatioNumericUpDown).BeginInit();
			SuspendLayout();
			// 
			// MagnifyRatioLabel
			// 
			resources.ApplyResources(MagnifyRatioLabel, "MagnifyRatioLabel");
			MagnifyRatioLabel.BackColor = System.Drawing.Color.Transparent;
			MagnifyRatioLabel.Name = "MagnifyRatioLabel";
			// 
			// OKButton
			// 
			resources.ApplyResources(OKButton, "OKButton");
			OKButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			OKButton.Name = "OKButton";
			OKButton.UseVisualStyleBackColor = true;
			OKButton.Click += OnOKButtonClick;
			// 
			// CancelButton
			// 
			resources.ApplyResources(CancelButton, "CancelButton");
			CancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			CancelButton.Name = "CancelButton";
			CancelButton.UseVisualStyleBackColor = true;
			// 
			// MagnifyRatioNumericUpDown
			// 
			resources.ApplyResources(MagnifyRatioNumericUpDown, "MagnifyRatioNumericUpDown");
			MagnifyRatioNumericUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
			MagnifyRatioNumericUpDown.Name = "MagnifyRatioNumericUpDown";
			MagnifyRatioNumericUpDown.Value = new decimal(new int[] { 100, 0, 0, 0 });
			MagnifyRatioNumericUpDown.ValueChanged += OnRecalculateRequested;
			// 
			// SizeLabel
			// 
			resources.ApplyResources(SizeLabel, "SizeLabel");
			SizeLabel.BackColor = System.Drawing.Color.Transparent;
			SizeLabel.Name = "SizeLabel";
			// 
			// PreviewBox
			// 
			resources.ApplyResources(PreviewBox, "PreviewBox");
			PreviewBox.BackColor = System.Drawing.Color.White;
			PreviewBox.Name = "PreviewBox";
			PreviewBox.ScrollChanged += OnPreviewBoxScrollChanged;
			PreviewBox.Paint += OnPreviewBoxPaint;
			PreviewBox.KeyDown += OnPreviewBoxKeyDown;
			PreviewBox.KeyUp += OnPreviewBoxKeyUp;
			PreviewBox.MouseDown += OnPreviewBoxMouseDown;
			PreviewBox.MouseLeave += OnPreviewBoxMouseLeave;
			PreviewBox.MouseMove += OnPreviewBoxMouseMove;
			PreviewBox.MouseUp += OnPreviewBoxMouseUp;
			PreviewBox.Resize += OnPreviewBoxResize;
			// 
			// ImageEditDialog
			// 
			AcceptButton = OKButton;
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = CancelButton;
			Controls.Add(PreviewBox);
			Controls.Add(SizeLabel);
			Controls.Add(MagnifyRatioNumericUpDown);
			Controls.Add(CancelButton);
			Controls.Add(MagnifyRatioLabel);
			Controls.Add(OKButton);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			MaximizeBox = true;
			Name = "ImageEditDialog";
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			Controls.SetChildIndex(OKButton, 0);
			Controls.SetChildIndex(MagnifyRatioLabel, 0);
			Controls.SetChildIndex(CancelButton, 0);
			Controls.SetChildIndex(MagnifyRatioNumericUpDown, 0);
			Controls.SetChildIndex(SizeLabel, 0);
			Controls.SetChildIndex(PreviewBox, 0);
			((System.ComponentModel.ISupportInitialize)MagnifyRatioNumericUpDown).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		System.Windows.Forms.NumericUpDown MagnifyRatioNumericUpDown;
		private System.Windows.Forms.Label SizeLabel;
		private Controls.ScrollBaredControl PreviewBox;
	}
}