namespace Comical
{
	partial class PasswordDialog
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
		void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PasswordDialog));
			this.btnCancel = new System.Windows.Forms.Button();
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.lblConfirmationPassword = new System.Windows.Forms.Label();
			this.txtConfirmationPassword = new System.Windows.Forms.TextBox();
			this.btnOK = new System.Windows.Forms.Button();
			this.lblPassword = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// txtPassword
			// 
			resources.ApplyResources(this.txtPassword, "txtPassword");
			this.txtPassword.Name = "txtPassword";
			this.txtPassword.UseSystemPasswordChar = true;
			// 
			// lblConfirmationPassword
			// 
			resources.ApplyResources(this.lblConfirmationPassword, "lblConfirmationPassword");
			this.lblConfirmationPassword.BackColor = System.Drawing.Color.Transparent;
			this.lblConfirmationPassword.Name = "lblConfirmationPassword";
			// 
			// txtConfirmationPassword
			// 
			resources.ApplyResources(this.txtConfirmationPassword, "txtConfirmationPassword");
			this.txtConfirmationPassword.Name = "txtConfirmationPassword";
			this.txtConfirmationPassword.UseSystemPasswordChar = true;
			// 
			// btnOK
			// 
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.Name = "btnOK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// lblPassword
			// 
			resources.ApplyResources(this.lblPassword, "lblPassword");
			this.lblPassword.BackColor = System.Drawing.Color.Transparent;
			this.lblPassword.Name = "lblPassword";
			// 
			// PasswordDialog
			// 
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.lblPassword);
			this.Controls.Add(this.txtPassword);
			this.Controls.Add(this.txtConfirmationPassword);
			this.Controls.Add(this.lblConfirmationPassword);
			this.DoubleBuffered = true;
			this.Name = "PasswordDialog";
			this.Controls.SetChildIndex(this.lblConfirmationPassword, 0);
			this.Controls.SetChildIndex(this.txtConfirmationPassword, 0);
			this.Controls.SetChildIndex(this.txtPassword, 0);
			this.Controls.SetChildIndex(this.lblPassword, 0);
			this.Controls.SetChildIndex(this.btnCancel, 0);
			this.Controls.SetChildIndex(this.btnOK, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		System.Windows.Forms.TextBox txtPassword;
		System.Windows.Forms.TextBox txtConfirmationPassword;
		System.Windows.Forms.Label lblConfirmationPassword;
		System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label lblPassword;

	}
}