using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Comical.Core;
using Comical.Properties;
using WeifenLuo.WinFormsUI.Docking;

namespace Comical;

public partial class EditorForm : Form
{
	public EditorForm()
	{
		_comic.Clear(BindingSide.Right);
		_imageList = new ContentsView(_comic.Images);
		_bookmarkList = new BookmarksView(_comic.Images, _comic.Bookmarks);
		_document = new DocumentView(_comic);
		_comic.PropertyChanged += OnComicPropertyChanged;
		_comic.Images.CollectionChanged += OnComicImagesCollectionChanged;
		InitializeComponent();
		InitializeDockingWindows();
		WindowState = Settings.Default.EditorWindowState;
		DesktopBounds = Settings.Default.EditorWindowBounds;
	}

	readonly Comic _comic = new();
	static readonly IReadOnlyList<string> ImageExtensions = ["bmp", "dib", "gif", "jpeg", "jpe", "jpg", "jfif", "png", "tiff", "tif",];
	readonly ContentsView _imageList;
	readonly BookmarksView _bookmarkList;
	readonly DocumentView _document;

	string? SavedFilePath
	{
		get;
		set
		{
			if (!string.Equals(field, value, StringComparison.Ordinal))
			{
				field = value;
				UpdateTitle();
			}
		}
	}

	string HumanReadableSavedFileName => string.IsNullOrEmpty(SavedFilePath) ? Resources.Untitled : Path.GetFileName(SavedFilePath);

#pragma warning disable CA1863 // リソースに対してCompositeFormatは使用できない
	void UpdateTitle() =>
		Text = string.Format(CultureInfo.CurrentCulture, Resources.TitleFormat,
			HumanReadableSavedFileName,
			_comic.IsDirty ? Resources.DirtyMark : string.Empty,
			Application.ProductName);
#pragma warning restore CA1863

	void InitializeDockingWindows()
	{
		MainDockPanel.Theme = new VS2015LightTheme();

		var defaultDockContents = new DockContent[] { _imageList, _bookmarkList, _document };
		LoadFromXml(MainDockPanel, Settings.Default.DockPanelConfiguration, defaultDockContents);
		foreach (var content in defaultDockContents)
		{
			if (content.DockPanel == null)
			{
				content.Show(MainDockPanel);
				content.Hide();
			}
		}

		_imageList.FileDropped += async (s, ev) => await AddAnythingLocalAsync(!ev.Control, ev.FileNames);
		_imageList.ImageReferenceSelected += (s, ev) =>
		{
			var count = _imageList.SelectedIndices.Count();
			OpenImageMenuItem.Enabled = SetAsThumbnailMenuItem.Enabled = count == 1;
			AddBookmarksMenuItem.Enabled = DeleteImagesMenuItem.Enabled = ExportImagesMenuItem.Enabled = ExtractImagesMenuItem.Enabled = StartViewModeSettingMenuItem.Enabled = InvertViewModeMenuItem.Enabled = count > 0;
		};
		_imageList.ExportRequested += OnExportImagesMenuItemClick;
		_imageList.ExtractRequested += OnExtractImagesMenuItemClick;
		_imageList.BookmarkRequested += OnAddBookmarksMenuItemClick;
		_imageList.SetAsThumbnailRequested += OnSetAsThumbnailMenuItemClick;

		_bookmarkList.BookmarkSelected += (s, ev) => DeleteBookmarksMenuItem.Enabled = _bookmarkList.SelectedIndices.Any();
		_bookmarkList.BookmarkNavigated += (s, ev) => _imageList.SelectSingleImage(ev.Bookmark.Target);
	}

	async Task<DialogResult> QuerySaveAsync(Action<DialogResult>? beforeSave = null)
	{
		if (!_comic.IsDirty)
			return DialogResult.No;
		var saveButton = new TaskDialogButton(Resources.Save);
		var doNotSaveButton = new TaskDialogButton(Resources.DoNotSave);
#pragma warning disable CA1863 // リソースに対してCompositeFormatは使用できない
		var selectedButton = TaskDialog.ShowDialog(this, new()
		{
			Caption = Application.ProductName,
			Heading = string.Format(CultureInfo.CurrentCulture, Resources.DoYouSaveChanges, HumanReadableSavedFileName),
			Buttons = [saveButton, doNotSaveButton, TaskDialogButton.Cancel],
		});
#pragma warning restore CA1863
		var result = selectedButton == saveButton ? DialogResult.Yes :
			selectedButton == doNotSaveButton ? DialogResult.No :
			DialogResult.Cancel;
		beforeSave?.Invoke(result);
		if (result == DialogResult.Yes && !await SaveAsync())
			result = DialogResult.Cancel;
		return result;
	}

	[SuppressMessage("Performance", "CA1859", Justification = "内部実装を隠蔽し変更容易性を保つため")]
	IDisposable BeginAsyncWork()
	{
		StatusProgressBar.Value = 0;
		StatusProgressBar.Visible = !(MainMenu.Enabled = MainToolBar.Enabled = false);
		return new CompositeDisposable()
		{
			() =>
			{
				StatusProgressBar.Visible = !(MainMenu.Enabled = MainToolBar.Enabled = true);
				StatusLabel.Text = string.Empty;
			},
			_imageList.BeginAsyncWork(),
			_bookmarkList.BeginAsyncWork(),
			_document.BeginAsyncWork(),
		};
	}

	static async Task CollectFilesAsync(IEnumerable<string> paths, List<string> comicFiles, List<ImageReference> images)
	{
		foreach (var path in paths)
		{
			try
			{
				if (Directory.Exists(path))
				{
					await CollectFilesAsync(Directory.EnumerateFileSystemEntries(path), comicFiles, images).ConfigureAwait(false);
					continue;
				}
				if (!File.Exists(path))
					continue;
				if (await FileHeader.LoadAsync(path).ConfigureAwait(false) != null)
				{
					comicFiles.Add(path);
					continue;
				}
				if (!ImageExtensions.Any(ex => string.Equals(Path.GetExtension(path), "." + ex, StringComparison.OrdinalIgnoreCase)))
					continue;
				images.Add(new ImageReference(await Binary.FromFileAsync(path)));
			}
			catch (UnauthorizedAccessException) { }
		}
	}

	static void AddAuthorToHistory(string author)
	{
		for (var i = Settings.Default.RecentAuthors.Count - 1; i >= 0; i--)
		{
#pragma warning disable CA1309 // ユーザ入力値の履歴との比較なのでカルチャ依存が望ましい
			if (author.Equals(Settings.Default.RecentAuthors[i], StringComparison.CurrentCulture))
				Settings.Default.RecentAuthors.RemoveAt(i);
#pragma warning restore CA1309
		}
		Settings.Default.RecentAuthors.Insert(0, author);
	}

	async Task AddAnythingLocalAsync(bool canOpen, IEnumerable<string> paths)
	{
		using (BeginAsyncWork())
		{
			StatusProgressBar.Style = ProgressBarStyle.Marquee;
			StatusLabel.Text = Resources.ScanningFiles;
			var comicFiles = new List<string>();
			var images = new List<ImageReference>();
			await CollectFilesAsync(paths, comicFiles, images);
			StatusProgressBar.Style = ProgressBarStyle.Blocks;
			var i = 0;
			if (canOpen && comicFiles.Count > 0)
			{
				StatusLabel.Text = Resources.OpeningFile;
				await _comic.OpenAsync(comicFiles[i], new Progress<int>(x => this.InvokeIfNeeded(() => StatusProgressBar.Value = x / comicFiles.Count)));
				AddAuthorToHistory(_comic.Author);
				SavedFilePath = comicFiles[i];
				i++;
			}
			StatusLabel.Text = Resources.AppendingFile;
			for (; i < comicFiles.Count; i++)
				await _comic.AppendAsync(comicFiles[i], new Progress<int>(x => this.InvokeIfNeeded(() => StatusProgressBar.Value = (100 * i + x) / comicFiles.Count)));
			StatusLabel.Text = Resources.ImportingImages;
			_comic.Images.AddRange(images);
		}
	}

	async Task<bool> SaveAsync(string fileName)
	{
		try
		{
			using (BeginAsyncWork())
			{
				await _comic.SaveAsync(fileName, new Progress<int>(x => this.InvokeIfNeeded(() => StatusProgressBar.Value = x)));
				AddAuthorToHistory(_comic.Author);
				return true;
			}
		}
		catch (InconsistentDataException)
		{
			TaskDialog.ShowDialog(this, new TaskDialogPage
			{
				Heading = Resources.InconsistentData_Instruction,
				Text = Resources.InconsistentData_Text,
				Caption = Application.ProductName,
				Buttons = [TaskDialogButton.Close],
				Icon = TaskDialogIcon.Error
			});
			return false;
		}
	}

	async Task<bool> SaveAsync() => string.IsNullOrEmpty(SavedFilePath) ? await SaveAsAsync() : await SaveAsync(SavedFilePath);

	async Task<bool> SaveAsAsync()
	{
		using var dialog = new SaveFileDialog();
		dialog.Filter = $"{Resources.ComicalImageCollection}|*.cic";
		dialog.AddExtension = true;
		dialog.DefaultExt = "cic";
		if (Settings.Default.DefaultSavedFileName.Length > 0)
			dialog.FileName = string.Format(CultureInfo.CurrentCulture, Settings.Default.DefaultSavedFileName, _comic.Title, _comic.Author, _comic.Published);
		if (dialog.ShowDialog(this) != DialogResult.OK)
			return false;
		var result = await SaveAsync(dialog.FileName);
		SavedFilePath = dialog.FileName;
		return result;
	}

	static void LoadFromXml(DockPanel panel, string xml, IEnumerable<IDockContent> contents)
	{
		if (string.IsNullOrEmpty(xml))
			return;
		using var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml));
		panel.LoadFromXml(ms, persistString => contents.FirstOrDefault(x => string.Equals(persistString, x.DockHandler.GetPersistStringCallback(), StringComparison.Ordinal)));
	}

	#region FileMenu

	async void OnNewMenuItemClick(object? sender, EventArgs e)
	{
		if (await QuerySaveAsync() != DialogResult.Cancel)
		{
			_comic.Clear(BindingSide.Right);
			SavedFilePath = string.Empty;
		}
	}

	async void OnOpenMenuItemClick(object? sender, EventArgs e)
	{
		if (await QuerySaveAsync() == DialogResult.Cancel)
			return;
		using var dialog = new OpenFileDialog();
		dialog.DefaultExt = ".cic";
		dialog.Filter = $"{Resources.ComicalImageCollection}|*.cic";
		if (dialog.ShowDialog(this) == DialogResult.OK)
			await AddAnythingLocalAsync(true, Enumerable.Repeat(dialog.FileName, 1));
	}

	async void OnSaveMenuItemClick(object? sender, EventArgs e) => await SaveAsync();

	async void OnSaveAsMenuItemClick(object? sender, EventArgs e) => await SaveAsAsync();

	void OnConfigureDocumentMenuItemClick(object? sender, EventArgs e) => _document.Show(MainDockPanel);

	void OnExitMenuItemClick(object? sender, EventArgs e) => Close();

	#endregion

	#region ViewMenu

	void OnContentsWindowMenuItemClick(object? sender, EventArgs e) => _imageList.Show(MainDockPanel);

	void OnBookmarksWindowMenuItemClick(object? sender, EventArgs e) => _bookmarkList.Show(MainDockPanel);

	#endregion

	#region ImageMenu

	async void OnAddImagesFromFilesMenuItemClick(object? sender, EventArgs e)
	{
		using var dialog = new OpenFileDialog();
		dialog.Multiselect = true;
		if (dialog.ShowDialog(this) == DialogResult.OK)
			await AddAnythingLocalAsync(false, dialog.FileNames);
	}

	async void OnAddImagesFromFolderMenuItemClick(object? sender, EventArgs e)
	{
		using var dialog = new FolderBrowserDialog();
		if (dialog.ShowDialog(this) == DialogResult.OK)
			await AddAnythingLocalAsync(false, Enumerable.Repeat(dialog.SelectedPath, 1));
	}

	void OnOpenImageMenuItemClick(object? sender, EventArgs e) => _imageList.OpenFirstSelectedImage();

	void OnDeleteImagesMenuItemClick(object? sender, EventArgs e) => _imageList.DeleteSelectedImages();

	async void OnExportImagesMenuItemClick(object? sender, EventArgs e)
	{
		using var dialog = new FolderBrowserDialog();
		dialog.Description = Resources.ExportImages;
		dialog.UseDescriptionForTitle = true;
		if (dialog.ShowDialog(this) != DialogResult.OK)
			return;
		using (BeginAsyncWork())
		{
			await _comic.ExportAsync(dialog.SelectedPath, _imageList.SelectedIndices.Order().Select(x => _comic.Images[x]), data =>
			{
				using var image = data.ToImage();
				var codecInfo = image.GetImageCodecInfo();
				return codecInfo == null || codecInfo.FilenameExtension == null ? string.Empty : codecInfo.FilenameExtension.Split(';')[0][1..];
			}, new Progress<int>(x => this.InvokeIfNeeded(() => StatusProgressBar.Value = x)));
		}
	}

	async void OnExtractImagesMenuItemClick(object? sender, EventArgs e)
	{
		using var dialog = new SaveFileDialog();
		dialog.DefaultExt = ".cic";
		dialog.Filter = $"{Resources.ComicalImageCollection}|*.cic";
		dialog.Title = Resources.ExtractImages;
		if (dialog.ShowDialog(this) != DialogResult.OK)
			return;
		using (BeginAsyncWork())
			await _comic.ExtractAsync(dialog.FileName, _imageList.SelectedIndices.Order().Select(x => _comic.Images[x]),
				new Progress<int>(x => this.InvokeIfNeeded(() => StatusProgressBar.Value = x)));
	}

	void OnStartViewModeSettingLeftMenuItemClick(object? sender, EventArgs e) => _imageList.SetSelectedImagesViewModes(true);

	void OnStartViewModeSettingRightMenuItemClick(object? sender, EventArgs e) => _imageList.SetSelectedImagesViewModes(false);

	void OnInvertViewModeMenuItemClick(object? sender, EventArgs e) => _imageList.InvertSelectedImagesViewModes();

	void OnSetAsThumbnailMenuItemClick(object? sender, EventArgs e)
	{
		var firstImageIndex = _imageList.SelectedIndices.FirstOrDefault(-1);
		if (firstImageIndex >= 0)
			_comic.Thumbnail = _comic.Images[firstImageIndex].Data.EnsureBitmap();
	}

	#endregion

	#region BookmarkMenu

	void OnAddBookmarksMenuItemClick(object? sender, EventArgs e)
		=> _comic.Bookmarks.AddRange(_imageList.SelectedIndices.Order().Select(x => new Bookmark() { Target = x }));

	void OnDeleteBookmarksMenuItemClick(object? sender, EventArgs e) => _bookmarkList.DeleteSelectedBookmarks();

	#endregion

	void OnOptionMenuItemClick(object? sender, EventArgs e)
	{
		using var dialog = new OptionDialog();
		dialog.ShowDialog(this);
	}

	void OnAboutMenuItemClick(object? sender, EventArgs e)
	{
		using var ss = new AboutDialog();
		ss.ShowDialog(this);
	}

	#region EditorForm EventHandlers

	protected override async void OnShown(EventArgs e)
	{
		base.OnShown(e);
		SavedFilePath = string.Empty;
		var args = Environment.GetCommandLineArgs();
		if (args.Length >= 2)
			await AddAnythingLocalAsync(true, Enumerable.Repeat(args[1], 1));
	}

	protected override async void OnFormClosing(FormClosingEventArgs e)
	{
		base.OnFormClosing(e);
		if (e == null) return;
		if (e.CloseReason != CloseReason.WindowsShutDown)
		{
			if (StatusProgressBar.Visible)
				e.Cancel = true;
			else if (await QuerySaveAsync(res => e.Cancel = res != DialogResult.No) == DialogResult.Yes)
				Close();
		}
	}

	protected override void OnFormClosed(FormClosedEventArgs e)
	{
		base.OnFormClosed(e);
		using (var ms = new MemoryStream())
		{
			MainDockPanel.SaveAsXml(ms, Encoding.UTF8);
			Settings.Default.DockPanelConfiguration = Encoding.UTF8.GetString(ms.ToArray());
		}
		Settings.Default.EditorWindowState = WindowState;
		if (WindowState == FormWindowState.Normal)
			Settings.Default.EditorWindowBounds = DesktopBounds;
		Settings.Default.Save();
	}

	#endregion

#pragma warning disable CA1863 // リソースに対してCompositeFormatは使用できない
	void OnComicImagesCollectionChanged(object? sender, EventArgs e)
		=> this.InvokeIfNeeded(() => ImageCountLabel.Text = string.Format(CultureInfo.CurrentCulture, Resources.ImageCountStringRepresentation, _comic.Images.Count));
#pragma warning restore CA1863

	void OnComicPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) => this.InvokeIfNeeded(UpdateTitle);
}
