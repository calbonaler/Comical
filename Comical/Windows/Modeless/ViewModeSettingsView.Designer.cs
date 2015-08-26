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
			this.btnSetStartedAtLeft = new System.Windows.Forms.Button();
			this.numStartIndex = new System.Windows.Forms.NumericUpDown();
			this.numSelectionCount = new System.Windows.Forms.NumericUpDown();
			this.tplMain = new System.Windows.Forms.TableLayoutPanel();
			this.tplAcceptButtons = new System.Windows.Forms.TableLayoutPanel();
			this.btnSetStartedAtRight = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.numStartIndex)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numSelectionCount)).BeginInit();
			this.tplMain.SuspendLayout();
			this.tplAcceptButtons.SuspendLayout();
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
			// btnSetStartedAtLeft
			// 
			resources.ApplyResources(this.btnSetStartedAtLeft, "btnSetStartedAtLeft");
			this.btnSetStartedAtLeft.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSetStartedAtLeft.Name = "btnSetStartedAtLeft";
			this.btnSetStartedAtLeft.UseVisualStyleBackColor = true;
			this.btnSetStartedAtLeft.Click += new System.EventHandler(this.btnSetStartedAtLeft_Click);
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
			// tplMain
			// 
			resources.ApplyResources(this.tplMain, "tplMain");
			this.tplMain.Controls.Add(this.numStartIndex, 1, 0);
			this.tplMain.Controls.Add(this.lblSelectionCount, 0, 1);
			this.tplMain.Controls.Add(this.numSelectionCount, 1, 1);
			this.tplMain.Controls.Add(this.lblStartIndex, 0, 0);
			this.tplMain.Controls.Add(this.tplAcceptButtons, 0, 2);
			this.tplMain.Name = "tplMain";
			// 
			// tplAcceptButtons
			// 
			resources.ApplyResources(this.tplAcceptButtons, "tplAcceptButtons");
			this.tplMain.SetColumnSpan(this.tplAcceptButtons, 2);
			this.tplAcceptButtons.Controls.Add(this.btnSetStartedAtLeft, 0, 0);
			this.tplAcceptButtons.Controls.Add(this.btnSetStartedAtRight, 1, 0);
			this.tplAcceptButtons.Name = "tplAcceptButtons";
			// 
			// btnSetStartedAtRight
			// 
			resources.ApplyResources(this.btnSetStartedAtRight, "btnSetStartedAtRight");
			this.btnSetStartedAtRight.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSetStartedAtRight.Name = "btnSetStartedAtRight";
			this.btnSetStartedAtRight.UseVisualStyleBackColor = true;
			this.btnSetStartedAtRight.Click += new System.EventHandler(this.btnSetStartedAtRight_Click);
			// 
			// ViewModeSettingsView
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.tplMain);
			this.HideOnClose = true;
			this.Name = "ViewModeSettingsView";
			((System.ComponentModel.ISupportInitialize)(this.numStartIndex)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numSelectionCount)).EndInit();
			this.tplMain.ResumeLayout(false);
			this.tplMain.PerformLayout();
			this.tplAcceptButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		System.Windows.Forms.NumericUpDown numStartIndex;
		System.Windows.Forms.NumericUpDown numSelectionCount;
		private System.Windows.Forms.Button btnSetStartedAtLeft;
		private System.Windows.Forms.Label lblStartIndex;
		private System.Windows.Forms.Label lblSelectionCount;
		private System.Windows.Forms.TableLayoutPanel tplMain;
		private System.Windows.Forms.TableLayoutPanel tplAcceptButtons;
		private System.Windows.Forms.Button btnSetStartedAtRight;
	}
}