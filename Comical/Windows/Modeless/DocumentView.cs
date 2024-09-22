using Comical.Core;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using WeifenLuo.WinFormsUI.Docking;

namespace Comical
{
	public partial class DocumentView : DockContent
	{
		public DocumentView(Comic comic)
		{
			InitializeComponent();
			AuthorComboBox.Items.AddRange(Properties.Settings.Default.RecentAuthors.Cast<string>().ToArray());
			var calendar = CultureInfo.CurrentCulture.OptionalCalendars.FirstOrDefault(cal => cal is not GregorianCalendar);
			if (calendar != null)
			{
				_formatInfo = (DateTimeFormatInfo)CultureInfo.CurrentCulture.DateTimeFormat.Clone();
				_formatInfo.Calendar = calendar;
			}

			_comic = comic;
			_comic.PropertyChanged += OnComicPropertyChanged;
			_comic.Images.CollectionChanged += OnComicImagesCollectionChanged;
			OnComicImagesCollectionChanged(_comic.Images, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(System.Collections.Specialized.NotifyCollectionChangedAction.Reset));
			OnComicPropertyChanged(_comic, new System.ComponentModel.PropertyChangedEventArgs(nameof(_comic.BindingSide)));
		}

		readonly Comic _comic;
		readonly DateTimeFormatInfo? _formatInfo;

		protected override string GetPersistString() => "Document";

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			OnPublishedDateTimePickerValueChanged(PublishedDateTimePicker, EventArgs.Empty);
		}

		void LoadImage(Binary? binaryImage)
		{
			_comic.Thumbnail = binaryImage;
			ThumbnailPreviewer.Image?.Dispose();
			ThumbnailPreviewer.Image = binaryImage?.ToImage();
			var size = ThumbnailPreviewer.Image?.Size ?? default;
			SizeLabel.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.ImageSizeStringRepresentation, size.Width, size.Height);
		}

		void OnComicPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) => this.InvokeIfNeeded(() =>
		{
			switch (e.PropertyName)
			{
				case nameof(_comic.Title):
					TitleTextBox.Text = _comic.Title;
					break;
				case nameof(_comic.Author):
					AuthorComboBox.Text = _comic.Author;
					break;
				case nameof(_comic.Published):
					PublishedDateTimePicker.Checked = _comic.Published != null;
					if (_comic.Published != null && _comic.Published >= PublishedDateTimePicker.MinDate && _comic.Published <= PublishedDateTimePicker.MaxDate)
						PublishedDateTimePicker.Value = (DateTime)_comic.Published;
					break;
				case nameof(_comic.BindingSide):
					BindingSideComboBox.SelectedIndex = (int)_comic.BindingSide;
					break;
				case nameof(_comic.Thumbnail):
					LoadImage(_comic.Thumbnail);
					break;
			}
		});

		void OnComicImagesCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => this.InvokeIfNeeded(() =>
		{
			ThumbnailLabel.Enabled = ThumbnailIndexNumericUpDown.Enabled = UpdateButton.Enabled = _comic.Images.Count > 0;
			if (_comic.Images.Count > 0)
				ThumbnailIndexNumericUpDown.Maximum = _comic.Images.Count - 1;
		});

		void OnEditButtonClick(object? sender, EventArgs e)
		{
			if (_comic.Thumbnail == null)
				return;
			using var dialog = new ImageEditDialog();
			dialog.Image = _comic.Thumbnail;
			if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				LoadImage(dialog.Image);
		}

		void OnSearchOnBrowserButtonClick(object? sender, EventArgs e)
		{
			Process.Start(new ProcessStartInfo()
			{
				FileName = "http://www.google.co.jp/search?q=" + Uri.EscapeDataString(AuthorComboBox.Text + " " + TitleTextBox.Text),
				UseShellExecute = true,
			});
		}

		void OnTitleTextBoxTextChanged(object? sender, EventArgs e) => _comic.Title = TitleTextBox.Text;

		void OnAuthorComboBoxTextChanged(object? sender, EventArgs e) => _comic.Author = AuthorComboBox.Text;

		void OnPublishedDateTimePickerValueChanged(object? sender, EventArgs e)
		{
			CultureDependingPublishedTextBox.Enabled = PublishedDateTimePicker.Checked && _formatInfo != null;
			CultureDependingPublishedTextBox.Text =
				CultureDependingPublishedTextBox.Enabled && _formatInfo != null &&
				PublishedDateTimePicker.Value >= _formatInfo.Calendar.MinSupportedDateTime && PublishedDateTimePicker.Value < _formatInfo.Calendar.MaxSupportedDateTime ?
				PublishedDateTimePicker.Value.ToString(_formatInfo.LongDatePattern, _formatInfo) : string.Empty;
			_comic.Published = PublishedDateTimePicker.Checked ? PublishedDateTimePicker.Value : null;
		}

		void OnCultureDependingPublishedTextBoxTextChanged(object? sender, EventArgs e)
		{
			if (_formatInfo != null &&
				DateTime.TryParse(CultureDependingPublishedTextBox.Text, _formatInfo, DateTimeStyles.AllowInnerWhite | DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AllowWhiteSpaces, out var date) &&
				(date.Year != PublishedDateTimePicker.Value.Year || date.Month != PublishedDateTimePicker.Value.Month || date.Day != PublishedDateTimePicker.Value.Day))
				PublishedDateTimePicker.Value = date;
		}

		void OnBindingSideComboBoxSelectedIndexChanged(object? sender, EventArgs e) => _comic.BindingSide = (BindingSide)BindingSideComboBox.SelectedIndex;

		void OnUpdateButtonClick(object? sender, EventArgs e)
		{
			if (ThumbnailIndexNumericUpDown.Enabled)
				LoadImage(_comic.Images[(int)ThumbnailIndexNumericUpDown.Value].Data);
		}
	}
}
