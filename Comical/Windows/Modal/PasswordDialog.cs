using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Comical
{
	public partial class PasswordDialog : DialogBase
	{
		public PasswordDialog() { InitializeComponent(); }

		#region PublicProperties

		public bool Creating
		{
			get { return txtConfirmationPassword.Visible; }
			set
			{
				if (value)
				{
					Height = 244;
					Description = Properties.Resources.InputPasswordToEncrypt;
					txtPassword.Location = new Point(txtPassword.Location.X, 66);
				}
				else
				{
					Height = 180;
					Description = Properties.Resources.InputPasswordToDecrypt;
					txtPassword.Location = new Point(txtPassword.Location.X, 50);
				}
				lblConfirmationPassword.Visible = txtConfirmationPassword.Visible = lblPassword.Visible = value;
			}
		}

		public string Password
		{
			get { return txtPassword.Text; }
			set { txtPassword.Text = txtConfirmationPassword.Text = value; }
		}

		#endregion

		void btnOK_Click(object sender, EventArgs e)
		{
			if (Creating)
			{
				if (txtPassword.Text != txtConfirmationPassword.Text)
				{
					TaskDialog.Show(Properties.Resources.PasswordsNotSame, Properties.Resources.InputSamePasswordInBothBox, Text, TaskDialogStandardButtons.Close, TaskDialogStandardIcon.Error, ownerWindowHandle: Handle);
					txtConfirmationPassword.Focus();
					txtConfirmationPassword.SelectAll();
					return;
				}
			}
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.Close();
		}

		void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.Close();
		}
	}
}
