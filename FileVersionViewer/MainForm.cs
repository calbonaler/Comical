using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonLibrary;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace FileVersionViewer
{
	public partial class MainForm : Form
	{
		public MainForm() { InitializeComponent(); }

		protected override void OnLoad(EventArgs e)
		{
			lvCicFiles.SetWindow("Explorer");
			base.OnLoad(e);
		}

		private void btnBrowse_Click(object sender, EventArgs e)
		{
			using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
			{
				dialog.IsFolderPicker = true;
				if (dialog.ShowDialog(Handle) == CommonFileDialogResult.Ok)
					txtFolder.Text = dialog.FileName;
			}
		}

		private async void btnView_Click(object sender, EventArgs e)
		{
			lvCicFiles.Items.Clear();
			foreach (var file in System.IO.Directory.GetFiles(txtFolder.Text))
			{
				var version = await Task.Factory.StartNew<Version>(() => Comical.Core.FileHeader.Create(file).FileVersion);
				if (version != null)
					lvCicFiles.Items.Add(new ListViewItem(new[] { System.IO.Path.GetFileNameWithoutExtension(file), version.ToString() }));
			}
			btnOutput.Enabled = true;
		}

		private void btnOutput_Click(object sender, EventArgs e)
		{
			using (CommonSaveFileDialog dialog = new CommonSaveFileDialog())
			{
				dialog.DefaultExtension = "csv";
				dialog.Filters.Add(new CommonFileDialogFilter("CSV ファイル", "*.csv"));
				if (dialog.ShowDialog(Handle) == CommonFileDialogResult.Ok)
				{
					using (System.IO.StreamWriter writer = new System.IO.StreamWriter(dialog.FileName, false, Encoding.Unicode))
					{
						foreach (ListViewItem item in lvCicFiles.Items)
							writer.WriteLine(item.SubItems[0].Text + "," + item.SubItems[1].Text);
					}
				}
			}
		}

		private void txtFolder_TextChanged(object sender, EventArgs e) { btnView.Enabled = !string.IsNullOrEmpty(txtFolder.Text); }
	}
}
