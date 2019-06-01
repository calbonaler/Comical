using Comical.Core;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
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
			var calendar = CultureInfo.CurrentCulture.OptionalCalendars.FirstOrDefault(cal => !(cal is GregorianCalendar));
			if (calendar != null)
			{
				_formatInfo = (DateTimeFormatInfo)CultureInfo.CurrentCulture.DateTimeFormat.Clone();
				_formatInfo.Calendar = calendar;
			}
		}

		Comic _comic;
		readonly DateTimeFormatInfo _formatInfo;

		protected override string GetPersistString() => "Document";

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			dtpPublished_ValueChanged(dtpPublished, EventArgs.Empty);
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
			var size = image?.Size ?? new Size(0, 0);
			preThumbnail.Image = image;
			lblSize.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.ImageSizeStringRepresentation, size.Width, size.Height);
		}

		void LoadImage(byte[] binaryImage)
		{
			if (_comic != null)
				_comic.Thumbnail = binaryImage;
			var size = new Size(0, 0);
			if (binaryImage != null)
			{
				using (var ms = new MemoryStream(binaryImage))
				using (var img = Image.FromStream(ms))
				{
					preThumbnail.Image = new Bitmap(img);
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
					cmbBindingSide.SelectedIndex = (int)BindingSide.Right;
			}
		}

		void Comic_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => this.InvokeIfNeeded(() =>
		{
			switch (e.PropertyName)
			{
				case nameof(_comic.Title):
					txtTitle.Text = _comic.Title;
					break;
				case nameof(_comic.Author):
					cmbAuthor.Text = _comic.Author;
					break;
				case nameof(_comic.Published):
					dtpPublished.Checked = _comic.Published != null;
					if (_comic.Published != null && _comic.Published >= dtpPublished.MinDate && _comic.Published <= dtpPublished.MaxDate)
						dtpPublished.Value = (DateTime)_comic.Published;
					break;
				case nameof(_comic.BindingSide):
					cmbBindingSide.SelectedIndex = (int)_comic.BindingSide;
					break;
				case nameof(_comic.Thumbnail):
					LoadImage(_comic.Thumbnail);
					break;
			}
		});

		void ComicImageCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => this.InvokeIfNeeded(() =>
		{
			lblThumbnail.Enabled = numThumbnailIndex.Enabled = btnUpdate.Enabled = _comic != null && _comic.Images.Count > 0;
			if (_comic != null && _comic.Images.Count > 0)
				numThumbnailIndex.Maximum = _comic.Images.Count - 1;
		});

		void btnEdit_Click(object sender, EventArgs e)
		{
			if (preThumbnail.Image == null)
				return;
			using (var dialog = new ImageEditDialog())
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

		void dtpPublished_ValueChanged(object sender, EventArgs e)
		{
			txtCultureDependingPublished.Enabled = dtpPublished.Checked && _formatInfo != null;
			txtCultureDependingPublished.Text =
				txtCultureDependingPublished.Enabled &&
				dtpPublished.Value >= _formatInfo.Calendar.MinSupportedDateTime && dtpPublished.Value < _formatInfo.Calendar.MaxSupportedDateTime ?
				dtpPublished.Value.ToString(_formatInfo.LongDatePattern, _formatInfo) : string.Empty;
			if (_comic != null)
				_comic.Published = dtpPublished.Checked ? dtpPublished.Value : (DateTime?)null;
		}

		void txtCultureDependingPublished_TextChanged(object sender, EventArgs e)
		{
			if (_formatInfo != null &&
				DateTime.TryParse(txtCultureDependingPublished.Text, _formatInfo, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces, out var date) &&
				(date.Year != dtpPublished.Value.Year || date.Month != dtpPublished.Value.Month || date.Day != dtpPublished.Value.Day))
				dtpPublished.Value = date;
		}

		void cmbBindingSide_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_comic != null)
				_comic.BindingSide = (BindingSide)cmbBindingSide.SelectedIndex;
		}

		void DocumentDialog_Disposed(object sender, EventArgs e) => SetComic(null);

		void btnUpdate_Click(object sender, EventArgs e)
		{
			if (numThumbnailIndex.Enabled)
				LoadImage(_comic.Images[(int)numThumbnailIndex.Value].CreateImage());
		}
	}
}
