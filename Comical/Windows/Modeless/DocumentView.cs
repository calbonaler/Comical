using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Comical.Controls;
using Comical.Core;
using Comical.Properties;
using WeifenLuo.WinFormsUI.Docking;

namespace Comical;

public partial class DocumentView : DockContent
{
	public DocumentView(Comic comic)
	{
		InitializeComponent();
		AuthorComboBox.Items.AddRange([.. Settings.Default.RecentAuthors]);
		Settings.Default.RecentAuthors.CollectionChanged += OnRecentAuthorsCollectionChanged;
		var calendar = CultureInfo.CurrentCulture.OptionalCalendars.FirstOrDefault(cal => cal is not GregorianCalendar);
		if (calendar != null)
		{
			_formatInfo = (DateTimeFormatInfo)CultureInfo.CurrentCulture.DateTimeFormat.Clone();
			_formatInfo.Calendar = calendar;
		}

		_comic = comic;
		_comic.PropertyChanged += OnComicPropertyChanged;
		OnComicPropertyChanged(_comic, new PropertyChangedEventArgs(nameof(_comic.BindingSide)));
	}

	readonly Comic _comic;
	readonly DateTimeFormatInfo? _formatInfo;

	protected override string GetPersistString() => "Document";

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
		OnPublishedDateTimePickerValueChanged(PublishedDateTimePicker, EventArgs.Empty);
	}

	public IDisposable BeginAsyncWork()
	{
		Enabled = false;
		return new DelegateDisposable(() =>
		{
			Enabled = true;
			OnPublishedDateTimePickerValueChanged(PublishedDateTimePicker, EventArgs.Empty);
		});
	}

	void OnRecentAuthorsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		AuthorComboBox.Items.Clear();
		AuthorComboBox.Items.AddRange([.. Settings.Default.RecentAuthors]);
	}

	void OnComicPropertyChanged(object? sender, PropertyChangedEventArgs e) => this.InvokeIfNeeded(() =>
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
				ThumbnailClipper.SetImage(_comic.Thumbnail);
				ThumbnailEditButton.Enabled = _comic.Thumbnail is not null;
				break;
		}
	});

	void OnSearchOnBrowserButtonClick(object? sender, EventArgs e) => Process.Start(new ProcessStartInfo()
	{
		FileName = "http://www.google.co.jp/search?q=" + Uri.EscapeDataString(AuthorComboBox.Text + " " + TitleTextBox.Text),
		UseShellExecute = true,
	});

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

	static int? DataObjectToDraggedImageReferenceIndex(IDataObject dataObject) =>
		dataObject.GetDataPresent(typeof(DataGridViewDraggedRowSet)) &&
		dataObject.GetData(typeof(DataGridViewDraggedRowSet)) is DataGridViewDraggedRowSet rowSet &&
		rowSet.Items.Count == 1 &&
		rowSet.Items[0] is ImageReference ? rowSet.StartIndex : null;

	void OnEditButtonClick(object? sender, EventArgs e) => ThumbnailClipper.BeginEdit();

	void OnOKButtonClick(object sender, EventArgs e) => _comic.Thumbnail = ThumbnailClipper.CommitEdit(ImageFormat.Bmp);

	void OnCancelButtonClick(object sender, EventArgs e) => ThumbnailClipper.CancelEdit();

	void OnMagnifyRatioNumericUpDownValueChanged(object sender, EventArgs e) => ThumbnailClipper.MagnifyRatio = (int)MagnifyRatioNumericUpDown.Value;

	void OnThumbnailClipperDragEnter(object? sender, DragEventArgs e)
	{
		Debug.Assert(e.Data != null, "I think this never happens.");
		e.Effect = DataObjectToDraggedImageReferenceIndex(e.Data) is not null ? DragDropEffects.Copy : DragDropEffects.None;
	}

	void OnThumbnailClipperDragDrop(object? sender, DragEventArgs e)
	{
		Debug.Assert(e.Data != null, "I think this never happens.");
		if (DataObjectToDraggedImageReferenceIndex(e.Data) is { } index)
			_comic.Thumbnail = _comic.Images[index].Data.EnsureBitmap();
	}

	void OnThumbnailClipperPropertyChanged(object sender, PropertyChangedEventArgs e)
	{
		switch (e.PropertyName)
		{
			case nameof(ThumbnailClipper.IsEditing):
				MagnifyRatioNumericUpDown.Visible = ThumbnailEditOKButton.Visible = ThumbnailEditCancelButton.Visible = ThumbnailClipper.IsEditing;
				ThumbnailEditButton.Visible = !ThumbnailClipper.IsEditing;
				break;
			case nameof(ThumbnailClipper.MagnifyRatio):
				MagnifyRatioNumericUpDown.Value = ThumbnailClipper.MagnifyRatio;
				break;
			case nameof(ThumbnailClipper.ClippedImageSize):
				var size = ThumbnailClipper.ClippedImageSize;
#pragma warning disable CA1863 // リソースに対してCompositeFormatは使用できない
				SizeLabel.Text = string.Format(CultureInfo.CurrentCulture, Resources.ImageSizeStringRepresentation, size.Width, size.Height);
#pragma warning restore CA1863
				break;
		}
	}

	void OnThumbnailMaximizeButtonClick(object sender, EventArgs e)
	{
		MainSplitContainer.Panel1Collapsed = !MainSplitContainer.Panel1Collapsed;
		ThumbnailMaximizeButton.Text = MainSplitContainer.Panel1Collapsed ? "❯❯" : "❮❮";
	}
}
