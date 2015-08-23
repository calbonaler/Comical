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
			System.Windows.Forms.Button btnCancel;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PasswordDialog));
			System.Windows.Forms.Button btnOK;
			this.txtPassword = new System.Windows.Forms.TextBox();
			this.lblConfirmationPassword = new System.Windows.Forms.Label();
			this.txtConfirmationPassword = new System.Windows.Forms.TextBox();
			this.lblPassword = new System.Windows.Forms.Label();
			btnCancel = new System.Windows.Forms.Button();
			btnOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			resources.ApplyResources(btnCancel, "btnCancel");
			btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			btnCancel.Name = "btnCancel";
			btnCancel.UseVisualStyleBackColor = true;
			btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
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
			resources.ApplyResources(btnOK, "btnOK");
			btnOK.Name = "btnOK";
			btnOK.UseVisualStyleBackColor = true;
			btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// lblPassword
			// 
			resources.ApplyResources(this.lblPassword, "lblPassword");
			this.lblPassword.BackColor = System.Drawing.Color.Transparent;
			this.lblPassword.Name = "lblPassword";
			// 
			// PasswordDialog
			// 
			this.AcceptButton = btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = btnCancel;
			this.Controls.Add(btnOK);
			this.Controls.Add(btnCancel);
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
			this.Controls.SetChildIndex(btnCancel, 0);
			this.Controls.SetChildIndex(btnOK, 0);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		System.Windows.Forms.TextBox txtPassword;
		System.Windows.Forms.TextBox txtConfirmationPassword;
		System.Windows.Forms.Label lblConfirmationPassword;
		private System.Windows.Forms.Label lblPassword;

	}
}