namespace Comical
{
	partial class WindowBrowseDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WindowBrowseDialog));
			this.ilIcons = new System.Windows.Forms.ImageList(this.components);
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.lvWindows = new System.Windows.Forms.ListView();
			this.clmTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.SuspendLayout();
			// 
			// ilIcons
			// 
			this.ilIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			resources.ApplyResources(this.ilIcons, "ilIcons");
			this.ilIcons.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// btnOK
			// 
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
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
			// lvWindows
			// 
			resources.ApplyResources(this.lvWindows, "lvWindows");
			this.lvWindows.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmTitle});
			this.lvWindows.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lvWindows.HideSelection = false;
			this.lvWindows.LargeImageList = this.ilIcons;
			this.lvWindows.MultiSelect = false;
			this.lvWindows.Name = "lvWindows";
			this.lvWindows.SmallImageList = this.ilIcons;
			this.lvWindows.TileSize = new System.Drawing.Size(331, 40);
			this.lvWindows.UseCompatibleStateImageBehavior = false;
			this.lvWindows.View = System.Windows.Forms.View.Tile;
			this.lvWindows.ItemActivate += new System.EventHandler(this.btnOK_Click);
			// 
			// clmTitle
			// 
			resources.ApplyResources(this.clmTitle, "clmTitle");
			// 
			// WindowBrowseDialog
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.btnCancel;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.lvWindows);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Name = "WindowBrowseDialog";
			this.Controls.SetChildIndex(this.btnOK, 0);
			this.Controls.SetChildIndex(this.btnCancel, 0);
			this.Controls.SetChildIndex(this.lvWindows, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		System.Windows.Forms.Button btnOK;
		System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ListView lvWindows;
		private System.Windows.Forms.ImageList ilIcons;
		private System.Windows.Forms.ColumnHeader clmTitle;
	}
}