namespace Comical
{
	partial class ManageArchiverDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

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
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageArchiverDialog));
			this.btnClose = new System.Windows.Forms.Button();
			this.lvArchivers = new System.Windows.Forms.ListView();
			this.clmName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.clmVersion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.ilStatus = new System.Windows.Forms.ImageList(this.components);
			this.btnInstall = new System.Windows.Forms.Button();
			this.btnUninstall = new System.Windows.Forms.Button();
			this.btnUpdate = new System.Windows.Forms.Button();
			this.llConfigureAutomaticUpdate = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// btnClose
			// 
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.Name = "btnClose";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// lvArchivers
			// 
			resources.ApplyResources(this.lvArchivers, "lvArchivers");
			this.lvArchivers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmName,
            this.clmVersion});
			this.lvArchivers.FullRowSelect = true;
			this.lvArchivers.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.lvArchivers.HideSelection = false;
			this.lvArchivers.LargeImageList = this.ilStatus;
			this.lvArchivers.MultiSelect = false;
			this.lvArchivers.Name = "lvArchivers";
			this.lvArchivers.SmallImageList = this.ilStatus;
			this.lvArchivers.UseCompatibleStateImageBehavior = false;
			this.lvArchivers.View = System.Windows.Forms.View.Details;
			this.lvArchivers.SelectedIndexChanged += new System.EventHandler(this.lvArchivers_SelectedIndexChanged);
			// 
			// clmName
			// 
			resources.ApplyResources(this.clmName, "clmName");
			// 
			// clmVersion
			// 
			resources.ApplyResources(this.clmVersion, "clmVersion");
			// 
			// ilStatus
			// 
			this.ilStatus.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilStatus.ImageStream")));
			this.ilStatus.TransparentColor = System.Drawing.Color.Transparent;
			this.ilStatus.Images.SetKeyName(0, "installed");
			this.ilStatus.Images.SetKeyName(1, "uninstalled");
			// 
			// btnInstall
			// 
			resources.ApplyResources(this.btnInstall, "btnInstall");
			this.btnInstall.Name = "btnInstall";
			this.btnInstall.UseVisualStyleBackColor = true;
			this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
			// 
			// btnUninstall
			// 
			resources.ApplyResources(this.btnUninstall, "btnUninstall");
			this.btnUninstall.Name = "btnUninstall";
			this.btnUninstall.UseVisualStyleBackColor = true;
			this.btnUninstall.Click += new System.EventHandler(this.btnUninstall_Click);
			// 
			// btnUpdate
			// 
			resources.ApplyResources(this.btnUpdate, "btnUpdate");
			this.btnUpdate.Name = "btnUpdate";
			this.btnUpdate.UseVisualStyleBackColor = true;
			this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
			// 
			// llConfigureAutomaticUpdate
			// 
			resources.ApplyResources(this.llConfigureAutomaticUpdate, "llConfigureAutomaticUpdate");
			this.llConfigureAutomaticUpdate.ActiveLinkColor = System.Drawing.SystemColors.HotTrack;
			this.llConfigureAutomaticUpdate.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.llConfigureAutomaticUpdate.LinkColor = System.Drawing.SystemColors.HotTrack;
			this.llConfigureAutomaticUpdate.Name = "llConfigureAutomaticUpdate";
			this.llConfigureAutomaticUpdate.TabStop = true;
			this.llConfigureAutomaticUpdate.VisitedLinkColor = System.Drawing.SystemColors.HotTrack;
			this.llConfigureAutomaticUpdate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llConfigureAutomaticUpdate_LinkClicked);
			// 
			// ManageArchiverDialog
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.llConfigureAutomaticUpdate);
			this.Controls.Add(this.btnUpdate);
			this.Controls.Add(this.btnUninstall);
			this.Controls.Add(this.btnInstall);
			this.Controls.Add(this.lvArchivers);
			this.Controls.Add(this.btnClose);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ManageArchiverDialog";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.ListView lvArchivers;
		private System.Windows.Forms.ColumnHeader clmName;
		private System.Windows.Forms.ColumnHeader clmVersion;
		private System.Windows.Forms.Button btnInstall;
		private System.Windows.Forms.Button btnUninstall;
		private System.Windows.Forms.Button btnUpdate;
		private System.Windows.Forms.ImageList ilStatus;
		private System.Windows.Forms.LinkLabel llConfigureAutomaticUpdate;
	}
}