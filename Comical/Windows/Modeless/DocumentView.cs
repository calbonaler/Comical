using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using Comical.Core;
using WeifenLuo.WinFormsUI.Docking;

namespace Comical
{
	public partial class DocumentView : DockContent
	{
		public DocumentView()
		{
			InitializeComponent();
			Disposed += new EventHandler(DocumentDialog_Disposed);
			cmbAuthor.Items.AddRange(Properties.Settings.Default.RecentAuthors.Cast<string>().ToArray());
			currentCalendar = CultureInfo.CurrentCulture.OptionalCalendars.FirstOrDefault(cal => !(cal is GregorianCalendar));
		}

		Comic _comic;
		Calendar currentCalendar = null;

		protected override string GetPersistString() => "Document";

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			txtCultureDependingDateOfPublication.Enabled = dtpDateOfPublication.Checked && currentCalendar != null;
			if (currentCalendar != null)
			{
				var format = (CultureInfo.CurrentCulture.Clone() as CultureInfo).DateTimeFormat;
				format.Calendar = currentCalendar;
				txtCultureDependingDateOfPublication.Text = dtpDateOfPublication.Value.ToString(format.LongDatePattern, format);
			}
		}

		void LoadImage(Image image)
		{
			if (_comic != null)
			{
				using (var stream = new MemoryStream())
				{
					image.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
					_comic.Thumbnail = stream.ToArray();
				}
			}
			Size size = image?.Size ?? new Size(0, 0);
			preThumbnail.LoadImage(image);
			lblSize.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.ImageSizeStringRepresentation, size.Width, size.Height);
		}

		void LoadImage(byte[] binaryImage)
		{
			if (_comic != null)
				_comic.Thumbnail = binaryImage;
			Size size = new Size(0, 0);
			if (binaryImage != null)
			{
				using (MemoryStream ms = new MemoryStream(binaryImage))
				using (Image img = Image.FromStream(ms))
				{
					preThumbnail.LoadImage(new Bitmap(img));
					size = img.Size;
				}
			}
			lblSize.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.ImageSizeStringRepresentation, size.Width, size.Height);
		}

		public void SetComic(Comic comic)
		{
			if (_comic == comic)
				return;
			if (_comic != null)
			{
				_comic.PropertyChanged -= Comic_PropertyChanged;
				if (_comic.Images != null)
					_comic.Images.CollectionChanged -= ComicImageCollection_CollectionChanged;
			}
			_comic = comic;
			if (comic != null)
			{
				comic.PropertyChanged += Comic_PropertyChanged;
				if (comic.Images != null)
					comic.Images.CollectionChanged += ComicImageCollection_CollectionChanged;
				ComicImageCollection_CollectionChanged(comic.Images, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
				using (comic.EnterUndirtiableSection())
					cmbPageTurningDirection.SelectedIndex = (int)PageTurningDirection.ToRight - 1;
			}
		}
		
		void Comic_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			this.InvokeIfNeeded(() =>
			{
				switch (e.PropertyName)
				{
					case nameof(_comic.Title):
						txtTitle.Text = _comic.Title;
						break;
					case nameof(_comic.Author):
						cmbAuthor.Text = _comic.Author;
						break;
					case nameof(_comic.DateOfPublication):
						dtpDateOfPublication.Checked = _comic.DateOfPublication != null;
						if (_comic.DateOfPublication != null && _comic.DateOfPublication >= dtpDateOfPublication.MinDate && _comic.DateOfPublication <= dtpDateOfPublication.MaxDate)
							dtpDateOfPublication.Value = (DateTime)_comic.DateOfPublication;
						break;
					case nameof(_comic.PageTurningDirection):
						cmbPageTurningDirection.SelectedIndex = (int)_comic.PageTurningDirection - 1;
						break;
					case nameof(_comic.Thumbnail):
						LoadImage(_comic.Thumbnail);
						break;
				}
			});
		}

		void ComicImageCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			this.InvokeIfNeeded(() =>
			{
				lblThumbnail.Enabled = numThumbnailIndex.Enabled = btnUpdate.Enabled = _comic != null && _comic.Images.Count > 0;
				if (_comic != null && _comic.Images.Count > 0)
					numThumbnailIndex.Maximum = _comic.Images.Count - 1;
			});
		}

		void btnEdit_Click(object sender, EventArgs e)
		{
			if (preThumbnail.Image == null)
				return;
			using (ImageEditingDialog dialog = new ImageEditingDialog())
			{
				dialog.Image = preThumbnail.Image;
				if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
					LoadImage(dialog.Image);
			}
		}

		void btnSearchOnBrowser_Click(object sender, EventArgs e) => System.Diagnostics.Process.Start("http://www.google.co.jp/search?q=" + Uri.EscapeDataString(cmbAuthor.Text + " " + txtTitle.Text));

		void txtTitle_TextChanged(object sender, EventArgs e)
		{
			if (_comic != null)
				_comic.Title = txtTitle.Text;
		}

		void cmbAuthor_TextChanged(object sender, EventArgs e)
		{
			if (_comic != null)
				_comic.Author = cmbAuthor.Text;
		}

		void dtpDateOfIssue_ValueChanged(object sender, EventArgs e)
		{
			txtCultureDependingDateOfPublication.Enabled = dtpDateOfPublication.Checked && currentCalendar != null;
			if (currentCalendar != null)
			{
				var format = (CultureInfo.CurrentCulture.Clone() as CultureInfo).DateTimeFormat;
				format.Calendar = currentCalendar;
				txtCultureDependingDateOfPublication.Text = dtpDateOfPublication.Value.ToString(format.LongDatePattern, format);
			}
			if (_comic != null)
				_comic.DateOfPublication = dtpDateOfPublication.Checked ? dtpDateOfPublication.Value : (DateTime?)null;
		}

		void txtCultureDependingDateOfIssue_TextChanged(object sender, EventArgs e)
		{
			if (currentCalendar == null)
				return;
			var format = (CultureInfo.CurrentCulture.Clone() as CultureInfo).DateTimeFormat;
			format.Calendar = currentCalendar;
			DateTime date;
			if (DateTime.TryParse(txtCultureDependingDateOfPublication.Text, format,
				DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces, out date))
			{
				if (date.Year != dtpDateOfPublication.Value.Year || date.Month != dtpDateOfPublication.Value.Month || date.Day != dtpDateOfPublication.Value.Day)
					dtpDateOfPublication.Value = date;
			}
		}

		void cmbPageTurningDirection_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_comic != null)
				_comic.PageTurningDirection = (PageTurningDirection)(cmbPageTurningDirection.SelectedIndex + 1);
		}

		void DocumentDialog_Disposed(object sender, EventArgs e) { SetComic(null); }

		void btnUpdate_Click(object sender, EventArgs e)
		{
			if (numThumbnailIndex.Enabled)
				LoadImage(_comic.Images[(int)numThumbnailIndex.Value].GetImage());
		}
	}
}
