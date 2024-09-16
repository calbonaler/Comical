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
			System.Windows.Forms.ContextMenuStrip OptionContextMenu;
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(Viewer));
			Controls.ToolStripRadioMenuItem OriginalSizeMenuItem;
			FitToWindowMenuItem = new Controls.ToolStripRadioMenuItem();
			MainPreviewer = new Controls.Previewer();
			OptionContextMenu = new System.Windows.Forms.ContextMenuStrip(components);
			OriginalSizeMenuItem = new Controls.ToolStripRadioMenuItem();
			OptionContextMenu.SuspendLayout();
			SuspendLayout();
			// 
			// OptionContextMenu
			// 
			OptionContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { FitToWindowMenuItem, OriginalSizeMenuItem });
			OptionContextMenu.Name = "conOption";
			resources.ApplyResources(OptionContextMenu, "OptionContextMenu");
			// 
			// FitToWindowMenuItem
			// 
			FitToWindowMenuItem.Checked = true;
			FitToWindowMenuItem.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			FitToWindowMenuItem.Group = 0;
			FitToWindowMenuItem.Name = "FitToWindowMenuItem";
			resources.ApplyResources(FitToWindowMenuItem, "FitToWindowMenuItem");
			FitToWindowMenuItem.CheckedChanged += OnSizeMenuItemsCheckedChanged;
			// 
			// OriginalSizeMenuItem
			// 
			OriginalSizeMenuItem.Group = 0;
			OriginalSizeMenuItem.Name = "OriginalSizeMenuItem";
			resources.ApplyResources(OriginalSizeMenuItem, "OriginalSizeMenuItem");
			OriginalSizeMenuItem.CheckedChanged += OnSizeMenuItemsCheckedChanged;
			// 
			// MainPreviewer
			// 
			resources.ApplyResources(MainPreviewer, "MainPreviewer");
			MainPreviewer.BackColor = System.Drawing.Color.Transparent;
			MainPreviewer.ContextMenuStrip = OptionContextMenu;
			MainPreviewer.ForeColor = System.Drawing.Color.White;
			MainPreviewer.Name = "MainPreviewer";
			MainPreviewer.StretchMode = Comical.Controls.PreviewerStretchMode.Uniform;
			// 
			// Viewer
			// 
			resources.ApplyResources(this, "$this");
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			BackColor = System.Drawing.Color.Black;
			Controls.Add(MainPreviewer);
			DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
			MinimizeBox = false;
			Name = "Viewer";
			ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
			ShowInTaskbar = false;
			OptionContextMenu.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion
		private Controls.ToolStripRadioMenuItem FitToWindowMenuItem;
		private Controls.Previewer MainPreviewer;
	}
}
