namespace Comical
{
	partial class AddImageFromWebPageDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddImageFromWebPageDialog));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			this.lblUrl = new System.Windows.Forms.Label();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.txtUrl = new System.Windows.Forms.TextBox();
			this.btnImport = new System.Windows.Forms.Button();
			this.btnSearch = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.dgvResults = new System.Windows.Forms.DataGridView();
			this.clmImage = new System.Windows.Forms.DataGridViewImageColumn();
			this.clmUrl = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.clmSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.lblAttributes = new System.Windows.Forms.Label();
			this.txtAttributes = new System.Windows.Forms.TextBox();
			this.lblResult = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
			this.SuspendLayout();
			// 
			// lblUrl
			// 
			resources.ApplyResources(this.lblUrl, "lblUrl");
			this.lblUrl.BackColor = System.Drawing.Color.Transparent;
			this.lblUrl.Name = "lblUrl";
			// 
			// btnBrowse
			// 
			resources.ApplyResources(this.btnBrowse, "btnBrowse");
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// txtUrl
			// 
			resources.ApplyResources(this.txtUrl, "txtUrl");
			this.txtUrl.Name = "txtUrl";
			this.txtUrl.TextChanged += new System.EventHandler(this.txtUrl_TextChanged);
			// 
			// btnImport
			// 
			resources.ApplyResources(this.btnImport, "btnImport");
			this.btnImport.Name = "btnImport";
			this.btnImport.UseVisualStyleBackColor = true;
			this.btnImport.Click += new System.EventHandler(this.btnImportSelected_Click);
			// 
			// btnSearch
			// 
			resources.ApplyResources(this.btnSearch, "btnSearch");
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.UseVisualStyleBackColor = true;
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// dgvResults
			// 
			this.dgvResults.AllowUserToAddRows = false;
			this.dgvResults.AllowUserToDeleteRows = false;
			this.dgvResults.AllowUserToResizeColumns = false;
			this.dgvResults.AllowUserToResizeRows = false;
			resources.ApplyResources(this.dgvResults, "dgvResults");
			this.dgvResults.BackgroundColor = System.Drawing.Color.White;
			this.dgvResults.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dgvResults.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.dgvResults.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.dgvResults.ColumnHeadersVisible = false;
			this.dgvResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clmImage,
            this.clmUrl,
            this.clmSize});
			this.dgvResults.GridColor = System.Drawing.Color.Gainsboro;
			this.dgvResults.Name = "dgvResults";
			this.dgvResults.ReadOnly = true;
			this.dgvResults.RowHeadersVisible = false;
			this.dgvResults.RowTemplate.Height = 21;
			this.dgvResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvResults.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvResults_CellContentDoubleClick);
			this.dgvResults.SelectionChanged += new System.EventHandler(this.dgvResults_SelectionChanged);
			// 
			// clmImage
			// 
			this.clmImage.Name = "clmImage";
			this.clmImage.ReadOnly = true;
			resources.ApplyResources(this.clmImage, "clmImage");
			// 
			// clmUrl
			// 
			this.clmUrl.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			resources.ApplyResources(this.clmUrl, "clmUrl");
			this.clmUrl.Name = "clmUrl";
			this.clmUrl.ReadOnly = true;
			this.clmUrl.Resizable = System.Windows.Forms.DataGridViewTriState.False;
			this.clmUrl.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// clmSize
			// 
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.clmSize.DefaultCellStyle = dataGridViewCellStyle1;
			resources.ApplyResources(this.clmSize, "clmSize");
			this.clmSize.Name = "clmSize";
			this.clmSize.ReadOnly = true;
			// 
			// lblAttributes
			// 
			resources.ApplyResources(this.lblAttributes, "lblAttributes");
			this.lblAttributes.BackColor = System.Drawing.Color.Transparent;
			this.lblAttributes.Name = "lblAttributes";
			// 
			// txtAttributes
			// 
			resources.ApplyResources(this.txtAttributes, "txtAttributes");
			this.txtAttributes.Name = "txtAttributes";
			// 
			// lblResult
			// 
			resources.ApplyResources(this.lblResult, "lblResult");
			this.lblResult.BackColor = System.Drawing.Color.Transparent;
			this.lblResult.Name = "lblResult";
			// 
			// AddImageFromWebPageDialog
			// 
			this.AcceptButton = this.btnImport;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.lblResult);
			this.Controls.Add(this.txtAttributes);
			this.Controls.Add(this.lblAttributes);
			this.Controls.Add(this.dgvResults);
			this.Controls.Add(this.lblUrl);
			this.Controls.Add(this.txtUrl);
			this.Controls.Add(this.btnImport);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSearch);
			this.Controls.Add(this.btnBrowse);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
			this.MaximizeBox = true;
			this.Name = "AddImageFromWebPageDialog";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		System.Windows.Forms.Button btnBrowse;
		System.Windows.Forms.TextBox txtUrl;
		System.Windows.Forms.Button btnImport;
		System.Windows.Forms.Button btnSearch;
		System.Windows.Forms.Button btnCancel;
		System.Windows.Forms.DataGridView dgvResults;
		System.Windows.Forms.DataGridViewImageColumn clmImage;
		System.Windows.Forms.DataGridViewTextBoxColumn clmUrl;
		System.Windows.Forms.DataGridViewTextBoxColumn clmSize;
		private System.Windows.Forms.Label lblUrl;
		private System.Windows.Forms.Label lblAttributes;
		private System.Windows.Forms.TextBox txtAttributes;
		private System.Windows.Forms.Label lblResult;
	}
}