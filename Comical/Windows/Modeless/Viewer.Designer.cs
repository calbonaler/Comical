namespace Comical
{
	partial class Viewer
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
			components = new System.ComponentModel.Container();
			System.Windows.Forms.ContextMenuStrip conOption;
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(Viewer));
			Controls.ToolStripRadioMenuItem itmOriginalSize;
			itmFitToWindow = new Controls.ToolStripRadioMenuItem();
			preMain = new Controls.Previewer();
			conOption = new System.Windows.Forms.ContextMenuStrip(components);
			itmOriginalSize = new Controls.ToolStripRadioMenuItem();
			conOption.SuspendLayout();
			SuspendLayout();
			// 
			// conOption
			// 
			conOption.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { itmFitToWindow, itmOriginalSize });
			conOption.Name = "conOption";
			resources.ApplyResources(conOption, "conOption");
			// 
			// itmFitToWindow
			// 
			itmFitToWindow.Checked = true;
			itmFitToWindow.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			itmFitToWindow.Group = 0;
			itmFitToWindow.Name = "itmFitToWindow";
			resources.ApplyResources(itmFitToWindow, "itmFitToWindow");
			itmFitToWindow.CheckedChanged += itmFitToWindow_CheckedChanged;
			// 
			// itmOriginalSize
			// 
			itmOriginalSize.Group = 0;
			itmOriginalSize.Name = "itmOriginalSize";
			resources.ApplyResources(itmOriginalSize, "itmOriginalSize");
			itmOriginalSize.CheckedChanged += itmFitToWindow_CheckedChanged;
			// 
			// preMain
			// 
			resources.ApplyResources(preMain, "preMain");
			preMain.BackColor = System.Drawing.Color.Transparent;
			preMain.ContextMenuStrip = conOption;
			preMain.ForeColor = System.Drawing.Color.White;
			preMain.Name = "preMain";
			preMain.StretchMode = Comical.Controls.PreviewerStretchMode.Uniform;
			// 
			// Viewer
			// 
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			BackColor = System.Drawing.Color.Black;
			Controls.Add(preMain);
			DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
			MinimizeBox = false;
			Name = "Viewer";
			ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
			ShowInTaskbar = false;
			conOption.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion
		private Controls.ToolStripRadioMenuItem itmFitToWindow;
		private Controls.Previewer preMain;
	}
}
