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
			System.Windows.Forms.TableLayoutPanel PreviewPanel;
			PreviewHScrollBar = new System.Windows.Forms.HScrollBar();
			PreviewBox = new Controls.FocusablePictureBox();
			PreviewVScrollBar = new System.Windows.Forms.VScrollBar();
			MagnifyRatioNumericUpDown = new System.Windows.Forms.NumericUpDown();
			SizeLabel = new System.Windows.Forms.Label();
			MagnifyRatioLabel = new System.Windows.Forms.Label();
			OKButton = new System.Windows.Forms.Button();
			CancelButton = new System.Windows.Forms.Button();
			PreviewPanel = new System.Windows.Forms.TableLayoutPanel();
			PreviewPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)PreviewBox).BeginInit();
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
			// PreviewPanel
			// 
			resources.ApplyResources(PreviewPanel, "PreviewPanel");
			PreviewPanel.BackColor = System.Drawing.Color.White;
			PreviewPanel.Controls.Add(PreviewHScrollBar, 0, 1);
			PreviewPanel.Controls.Add(PreviewBox, 0, 0);
			PreviewPanel.Controls.Add(PreviewVScrollBar, 1, 0);
			PreviewPanel.Name = "PreviewPanel";
			PreviewPanel.Resize += OnRecalculateRequested;
			// 
			// PreviewHScrollBar
			// 
			resources.ApplyResources(PreviewHScrollBar, "PreviewHScrollBar");
			PreviewHScrollBar.Name = "PreviewHScrollBar";
			PreviewHScrollBar.Scroll += OnPreviewScrollBarsScroll;
			// 
			// PreviewBox
			// 
			PreviewBox.BackColor = System.Drawing.Color.White;
			resources.ApplyResources(PreviewBox, "PreviewBox");
			PreviewBox.Name = "PreviewBox";
			PreviewBox.TabStop = false;
			PreviewBox.KeyDown += OnPreviewBoxKeyDown;
			PreviewBox.KeyUp += OnPreviewBoxKeyUp;
			PreviewBox.Paint += OnPreviewBoxPaint;
			PreviewBox.MouseDown += OnPreviewBoxMouseDown;
			PreviewBox.MouseLeave += OnPreviewBoxMouseLeave;
			PreviewBox.MouseMove += OnPreviewBoxMouseMove;
			PreviewBox.MouseUp += OnPreviewBoxMouseUp;
			// 
			// PreviewVScrollBar
			// 
			resources.ApplyResources(PreviewVScrollBar, "PreviewVScrollBar");
			PreviewVScrollBar.Name = "PreviewVScrollBar";
			PreviewVScrollBar.Scroll += OnPreviewScrollBarsScroll;
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
			// ImageEditDialog
			// 
			AcceptButton = OKButton;
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = CancelButton;
			Controls.Add(PreviewPanel);
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
			Controls.SetChildIndex(PreviewPanel, 0);
			PreviewPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)PreviewBox).EndInit();
			((System.ComponentModel.ISupportInitialize)MagnifyRatioNumericUpDown).EndInit();
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		Controls.FocusablePictureBox PreviewBox;
		System.Windows.Forms.NumericUpDown MagnifyRatioNumericUpDown;
		private System.Windows.Forms.Label SizeLabel;
		private System.Windows.Forms.HScrollBar PreviewHScrollBar;
		private System.Windows.Forms.VScrollBar PreviewVScrollBar;

	}
}