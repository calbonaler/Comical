namespace Comical
{
	partial class ViewModeSettingsView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewModeSettingsView));
			this.lblStartIndex = new System.Windows.Forms.Label();
			this.lblSelectionCount = new System.Windows.Forms.Label();
			this.btnSet = new System.Windows.Forms.Button();
			this.numStartIndex = new System.Windows.Forms.NumericUpDown();
			this.numSelectionCount = new System.Windows.Forms.NumericUpDown();
			this.radLeftStart = new System.Windows.Forms.RadioButton();
			this.radRightStart = new System.Windows.Forms.RadioButton();
			((System.ComponentModel.ISupportInitialize)(this.numStartIndex)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numSelectionCount)).BeginInit();
			this.SuspendLayout();
			// 
			// lblStartIndex
			// 
			resources.ApplyResources(this.lblStartIndex, "lblStartIndex");
			this.lblStartIndex.Name = "lblStartIndex";
			// 
			// lblSelectionCount
			// 
			resources.ApplyResources(this.lblSelectionCount, "lblSelectionCount");
			this.lblSelectionCount.Name = "lblSelectionCount";
			// 
			// btnSet
			// 
			resources.ApplyResources(this.btnSet, "btnSet");
			this.btnSet.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSet.Name = "btnSet";
			this.btnSet.UseVisualStyleBackColor = true;
			this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
			// 
			// numStartIndex
			// 
			resources.ApplyResources(this.numStartIndex, "numStartIndex");
			this.numStartIndex.Name = "numStartIndex";
			this.numStartIndex.ValueChanged += new System.EventHandler(this.numStartIndex_ValueChanged);
			// 
			// numSelectionCount
			// 
			resources.ApplyResources(this.numSelectionCount, "numSelectionCount");
			this.numSelectionCount.Name = "numSelectionCount";
			// 
			// radLeftStart
			// 
			resources.ApplyResources(this.radLeftStart, "radLeftStart");
			this.radLeftStart.Checked = true;
			this.radLeftStart.Name = "radLeftStart";
			this.radLeftStart.TabStop = true;
			this.radLeftStart.UseVisualStyleBackColor = true;
			// 
			// radRightStart
			// 
			resources.ApplyResources(this.radRightStart, "radRightStart");
			this.radRightStart.Name = "radRightStart";
			this.radRightStart.UseVisualStyleBackColor = true;
			// 
			// ViewModeSettingsView
			// 
			this.AcceptButton = this.btnSet;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.btnSet);
			this.Controls.Add(this.radRightStart);
			this.Controls.Add(this.radLeftStart);
			this.Controls.Add(this.numSelectionCount);
			this.Controls.Add(this.lblSelectionCount);
			this.Controls.Add(this.numStartIndex);
			this.Controls.Add(this.lblStartIndex);
			this.HideOnClose = true;
			this.Name = "ViewModeSettingsView";
			((System.ComponentModel.ISupportInitialize)(this.numStartIndex)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numSelectionCount)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		System.Windows.Forms.NumericUpDown numStartIndex;
		System.Windows.Forms.NumericUpDown numSelectionCount;
		System.Windows.Forms.RadioButton radLeftStart;
		System.Windows.Forms.RadioButton radRightStart;
		private System.Windows.Forms.Button btnSet;
		private System.Windows.Forms.Label lblStartIndex;
		private System.Windows.Forms.Label lblSelectionCount;
	}
}