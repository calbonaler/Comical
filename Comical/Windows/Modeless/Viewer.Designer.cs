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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Viewer));
			System.Windows.Forms.ContextMenuStrip conOption;
			Comical.Controls.ToolStripRadioMenuItem itmOriginalSize;
			this.itmFitToWindow = new Comical.Controls.ToolStripRadioMenuItem();
			this.preMain = new Comical.Controls.Previewer();
			conOption = new System.Windows.Forms.ContextMenuStrip(this.components);
			itmOriginalSize = new Comical.Controls.ToolStripRadioMenuItem();
			conOption.SuspendLayout();
			this.SuspendLayout();
			// 
			// itmFitToWindow
			// 
			this.itmFitToWindow.Checked = true;
			this.itmFitToWindow.CheckOnClick = true;
			this.itmFitToWindow.CheckState = System.Windows.Forms.CheckState.Checked;
			this.itmFitToWindow.Group = 0;
			this.itmFitToWindow.Name = "itmFitToWindow";
			resources.ApplyResources(this.itmFitToWindow, "itmFitToWindow");
			this.itmFitToWindow.CheckedChanged += new System.EventHandler(this.itmFitToWindow_CheckedChanged);
			// 
			// conOption
			// 
			conOption.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itmFitToWindow,
            itmOriginalSize});
			conOption.Name = "conOption";
			resources.ApplyResources(conOption, "conOption");
			// 
			// itmOriginalSize
			// 
			itmOriginalSize.CheckOnClick = true;
			itmOriginalSize.Group = 0;
			itmOriginalSize.Name = "itmOriginalSize";
			resources.ApplyResources(itmOriginalSize, "itmOriginalSize");
			itmOriginalSize.CheckedChanged += new System.EventHandler(this.itmFitToWindow_CheckedChanged);
			// 
			// preMain
			// 
			resources.ApplyResources(this.preMain, "preMain");
			this.preMain.BackColor = System.Drawing.Color.Transparent;
			this.preMain.Description = null;
			this.preMain.ForeColor = System.Drawing.Color.White;
			this.preMain.Image = null;
			this.preMain.Name = "preMain";
			this.preMain.StretchMode = Comical.Controls.PreviewerStretchMode.Uniform;
			// 
			// Viewer
			// 
			resources.ApplyResources(this, "$this");
			this.BackColor = System.Drawing.Color.Black;
			this.ContextMenuStrip = conOption;
			this.Controls.Add(this.preMain);
			this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
			this.MinimizeBox = false;
			this.Name = "Viewer";
			this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
			this.ShowInTaskbar = false;
			this.TabPageContextMenuStrip = conOption;
			conOption.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private Controls.ToolStripRadioMenuItem itmFitToWindow;
		private Controls.Previewer preMain;
	}
}
