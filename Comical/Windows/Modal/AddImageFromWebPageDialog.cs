using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows.Forms;
using Comical.Core;

namespace Comical
{
	public partial class AddImageFromWebPageDialog : DialogBase
	{
		public AddImageFromWebPageDialog()
		{
			InitializeComponent();
			AllowExtensions = new Collection<string>();
		}

		public ReadOnlyCollection<Uri> Images { get; private set; }

		public Collection<string> AllowExtensions { get; private set; }

		bool show = true;

		protected override void OnFormClosing(FormClosingEventArgs e)
		{
			if (!txtAttributes.Enabled && e != null)
				e.Cancel = true;
			else
				show = false;
			base.OnFormClosing(e);
		}

		void btnBrowse_Click(object sender, System.EventArgs e)
		{
			using (WindowBrowseDialog dialog = new WindowBrowseDialog())
			{
				dialog.WindowFilter = window =>
					(window.Style & WindowStyles.OverlapedWindow) == WindowStyles.OverlapedWindow &&
					(window.Style & WindowStyles.Visible) == WindowStyles.Visible &&
					window.Handle != Owner.Handle;
				if (dialog.ShowDialog(this) == DialogResult.OK && dialog.SelectedWindow != null)
				{
					var url = dialog.SelectedWindow.Descendant(w => w.ClassName.Equals("edit", StringComparison.OrdinalIgnoreCase));
					if (url != null)
						txtUrl.Text = url.Text;
				}
			}
		}

		async void btnSearch_Click(object sender, EventArgs e)
		{
			Uri pageUrl = null;
			if (Uri.TryCreate(txtUrl.Text, UriKind.Absolute, out pageUrl))
			{
				btnSearch.Enabled = lblUrl.Enabled = txtUrl.Enabled = lblAttributes.Enabled = txtAttributes.Enabled = false;
				List<Uri> urls = new List<Uri>();
				var attributes = txtAttributes.Text.Split(';');
				var document = new HtmlAgilityPack.HtmlDocument();
				document.LoadHtml(await Utils.GetHtml(pageUrl));
				foreach (var node in document.DocumentNode.Descendants())
				{
					foreach (var attribute in attributes.Select(att => node.Attributes[att]).Where(att => att != null && AllowExtensions.Any(ex => System.IO.Path.GetExtension(att.Value) == "." + ex)))
					{
						Uri imageUrl = null;
						if (Uri.TryCreate(pageUrl, attribute.Value, out imageUrl) && !urls.Contains(imageUrl))
						{
							urls.Add(imageUrl);
							var row = dgvResults.Rows[dgvResults.Rows.Add()];
							row.Height = clmImage.Width;
							row.Cells["clmUrl"].Value = imageUrl;
						}
					}
				}
				Action act = async () =>
				{
					for (int i = 0; show && i < dgvResults.Rows.Count; i++)
					{
						try
						{
							using (var client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
							using (var stream = await client.GetStreamAsync(dgvResults["clmUrl", i].Value.ToString()))
							using (var img = Image.FromStream(stream, false, false))
							{
								dgvResults["clmSize", i].Value =
									string.Format(CultureInfo.CurrentCulture, Properties.Resources.ImageSizeStringRepresentation, img.Width, img.Height) + "\r\n\r\n" +
									string.Format(CultureInfo.CurrentCulture, Properties.Resources.ImageResolutionStringRepresentation, img.HorizontalResolution, img.VerticalResolution);
								dgvResults["clmImage", i].Value = img.Resize(new Size(clmImage.Width, clmImage.Width));
							}
						}
						catch (ArgumentException) { }
					}
				};
				act.BeginInvoke(null, null);
				btnSearch.Enabled = lblUrl.Enabled = txtUrl.Enabled = lblAttributes.Enabled = txtAttributes.Enabled = true;
			}
		}

		void btnImportSelected_Click(object sender, EventArgs e)
		{
			Images = new ReadOnlyCollection<Uri>(dgvResults.SelectedRows.Cast<DataGridViewRow>().OrderBy(x => x.Index).Select(row => dgvResults["clmUrl", row.Index].Value as Uri).ToArray());
			DialogResult = DialogResult.OK;
			Close();
		}

		void btnCancel_Click(object sender, EventArgs e) { DialogResult = System.Windows.Forms.DialogResult.Cancel; }

		#region dgvResults EventHandlers

		void dgvResults_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			string url = dgvResults["clmUrl", e.RowIndex].Value.ToString();
			if (!string.IsNullOrEmpty(url))
			{
				var viewer = new Viewer();
				viewer.Show(this);
				viewer.SetImageAsync(url);
			}
		}

		void dgvResults_SelectionChanged(object sender, EventArgs e) { btnImport.Enabled = dgvResults.SelectedRows.Count > 0; }

		#endregion

		void txtUrl_TextChanged(object sender, EventArgs e) { btnSearch.Enabled = Uri.IsWellFormedUriString(txtUrl.Text, UriKind.Absolute); }
	}
}
