using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Comical.Core;
using CPDialogs = Microsoft.WindowsAPICodePack.Dialogs;

namespace Comical
{
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
			WindowState = Properties.Settings.Default.EditorWindowState;
			DesktopBounds = Properties.Settings.Default.EditorWindowBounds;
		}

		readonly Comic _comic = new();
		string? _savedFilePath;
		static readonly IReadOnlyList<string> ImageExtensions = ["bmp", "dib", "gif", "jpeg", "jpe", "jpg", "jfif", "png", "tiff", "tif",];
		readonly ContentsView _imageList;
		readonly BookmarksView _bookmarkList;
		readonly DocumentView _document;

		string? SavedFilePath
		{
			get => _savedFilePath;
			set
			{
				if (!string.Equals(_savedFilePath, value, StringComparison.Ordinal))
				{
					_savedFilePath = value;
					UpdateTitle();
				}
			}
		}

		string HumanReadableSavedFileName => string.IsNullOrEmpty(SavedFilePath) ? Properties.Resources.Untitled : Path.GetFileName(SavedFilePath);

		void UpdateTitle() => Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.TitleFormat, HumanReadableSavedFileName, _comic.IsDirty ? Properties.Resources.DirtyMark : string.Empty, Application.ProductName);

		void InitializeDockingWindows()
		{
			MainDockPanel.Theme = new WeifenLuo.WinFormsUI.Docking.VS2015LightTheme();

			LoadFromXml(MainDockPanel, Properties.Settings.Default.DockPanelConfiguration, _imageList, _bookmarkList, _document);

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

		async Task<CPDialogs.TaskDialogResult> QuerySaveAsync(Action<CPDialogs.TaskDialogResult>? beforeSave = null)
		{
			if (!_comic.IsDirty)
				return CPDialogs.TaskDialogResult.No;
			using var dialog = new CPDialogs.TaskDialog();
			dialog.Caption = Application.ProductName;
			dialog.InstructionText = string.Format(CultureInfo.CurrentCulture, Properties.Resources.DoYouSaveChanges, HumanReadableSavedFileName);
			dialog.OwnerWindowHandle = Handle;
			CPDialogs.TaskDialogButton SaveButton = new CPDialogs.TaskDialogButton(nameof(SaveButton), Properties.Resources.Save);
			SaveButton.Click += (s, ev) => dialog.Close(CPDialogs.TaskDialogResult.Yes);
			CPDialogs.TaskDialogButton DoNotSaveButton = new CPDialogs.TaskDialogButton(nameof(DoNotSaveButton), Properties.Resources.DoNotSave);
			DoNotSaveButton.Click += (s, ev) => dialog.Close(CPDialogs.TaskDialogResult.No);
			CPDialogs.TaskDialogButton CancelButton = new CPDialogs.TaskDialogButton(nameof(CancelButton), Properties.Resources.Cancel);
			CancelButton.Click += (s, ev) => dialog.Close(CPDialogs.TaskDialogResult.Cancel);
			dialog.Controls.Add(SaveButton);
			dialog.Controls.Add(DoNotSaveButton);
			dialog.Controls.Add(CancelButton);
			dialog.Cancelable = true;
			dialog.StartupLocation = CPDialogs.TaskDialogStartupLocation.CenterOwner;
			var result = dialog.Show();
			beforeSave?.Invoke(result);
			if (result == CPDialogs.TaskDialogResult.Yes && !await SaveAsync())
				result = CPDialogs.TaskDialogResult.Cancel;
			return result;
		}

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
			for (var i = Properties.Settings.Default.RecentAuthors.Count - 1; i >= 0; i--)
			{
				if (author.Equals(Properties.Settings.Default.RecentAuthors[i], StringComparison.CurrentCulture))
					Properties.Settings.Default.RecentAuthors.RemoveAt(i);
			}
			Properties.Settings.Default.RecentAuthors.Insert(0, author);
		}

		async Task AddAnythingLocalAsync(bool canOpen, IEnumerable<string> paths)
		{
			using (BeginAsyncWork())
			{
				StatusProgressBar.Style = ProgressBarStyle.Marquee;
				StatusLabel.Text = Properties.Resources.ScanningFiles;
				var comicFiles = new List<string>();
				var images = new List<ImageReference>();
				await CollectFilesAsync(paths, comicFiles, images);
				StatusProgressBar.Style = ProgressBarStyle.Blocks;
				var i = 0;
				if (canOpen && comicFiles.Count > 0)
				{
					StatusLabel.Text = Properties.Resources.OpeningFile;
					await _comic.OpenAsync(comicFiles[i], new Progress<int>(x => this.InvokeIfNeeded(() => StatusProgressBar.Value = x / comicFiles.Count)));
					AddAuthorToHistory(_comic.Author);
					SavedFilePath = comicFiles[i];
					i++;
				}
				StatusLabel.Text = Properties.Resources.AppendingFile;
				for (; i < comicFiles.Count; i++)
					await _comic.AppendAsync(comicFiles[i], new Progress<int>(x => this.InvokeIfNeeded(() => StatusProgressBar.Value = (100 * i + x) / comicFiles.Count)));
				StatusLabel.Text = Properties.Resources.ImportingImages;
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
				using var dialog = new CPDialogs.TaskDialog();
				dialog.InstructionText = Properties.Resources.InconsistentData_Instruction;
				dialog.Text = Properties.Resources.InconsistentData_Text;
				dialog.Caption = Application.ProductName;
				dialog.StandardButtons = CPDialogs.TaskDialogStandardButtons.Close;
				dialog.Icon = CPDialogs.TaskDialogStandardIcon.Error;
				dialog.OwnerWindowHandle = Handle;
				dialog.Show();
				return false;
			}
		}

		async Task<bool> SaveAsync() => string.IsNullOrEmpty(SavedFilePath) ? await SaveAsAsync() : await SaveAsync(SavedFilePath);

		async Task<bool> SaveAsAsync()
		{
			using var dialog = new CPDialogs.CommonSaveFileDialog();
			dialog.Filters.Add(new CPDialogs.CommonFileDialogFilter(Properties.Resources.ComicalImageCollection, "*.cic"));
			dialog.AlwaysAppendDefaultExtension = true;
			dialog.DefaultExtension = "cic";
			if (Properties.Settings.Default.DefaultSavedFileName.Length > 0)
				dialog.DefaultFileName = string.Format(CultureInfo.CurrentCulture, Properties.Settings.Default.DefaultSavedFileName, _comic.Title, _comic.Author, _comic.Published);
			if (dialog.ShowDialog(Handle) != CPDialogs.CommonFileDialogResult.Ok)
				return false;
			var result = await SaveAsync(dialog.FileName);
			SavedFilePath = dialog.FileName;
			return result;
		}

		static void LoadFromXml(WeifenLuo.WinFormsUI.Docking.DockPanel panel, string xml, params WeifenLuo.WinFormsUI.Docking.IDockContent[] contents)
		{
			if (string.IsNullOrEmpty(xml))
				return;
			using var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml));
			panel.LoadFromXml(ms, persistString => Array.Find(contents, x => string.Equals(persistString, x.DockHandler.GetPersistStringCallback(), StringComparison.Ordinal)));
		}

		#region FileMenu

		async void OnNewMenuItemClick(object? sender, EventArgs e)
		{
			if (await QuerySaveAsync() != CPDialogs.TaskDialogResult.Cancel)
			{
				_comic.Clear(BindingSide.Right);
				SavedFilePath = string.Empty;
			}
		}

		async void OnOpenMenuItemClick(object? sender, EventArgs e)
		{
			if (await QuerySaveAsync() == CPDialogs.TaskDialogResult.Cancel)
				return;
			using var dialog = new CPDialogs.CommonOpenFileDialog();
			dialog.DefaultExtension = ".cic";
			dialog.Filters.Add(new CPDialogs.CommonFileDialogFilter(Properties.Resources.ComicalImageCollection, "*.cic"));
			if (dialog.ShowDialog(Handle) == CPDialogs.CommonFileDialogResult.Ok)
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
			using var dialog = new CPDialogs.CommonOpenFileDialog();
			dialog.Multiselect = true;
			if (dialog.ShowDialog(Handle) == CPDialogs.CommonFileDialogResult.Ok)
				await AddAnythingLocalAsync(false, dialog.FileNames);
		}

		async void OnAddImagesFromFolderMenuItemClick(object? sender, EventArgs e)
		{
			using var dialog = new CPDialogs.CommonOpenFileDialog();
			dialog.Multiselect = true;
			dialog.IsFolderPicker = true;
			if (dialog.ShowDialog(Handle) == CPDialogs.CommonFileDialogResult.Ok)
				await AddAnythingLocalAsync(false, dialog.FileNames);
		}

		void OnOpenImageMenuItemClick(object? sender, EventArgs e) => _imageList.OpenFirstSelectedImage();

		void OnDeleteImagesMenuItemClick(object? sender, EventArgs e) => _imageList.DeleteSelectedImages();

		async void OnExportImagesMenuItemClick(object? sender, EventArgs e)
		{
			using var dialog = new CPDialogs.CommonOpenFileDialog();
			dialog.IsFolderPicker = true;
			dialog.Title = Properties.Resources.ExportImages;
			if (dialog.ShowDialog(Handle) != CPDialogs.CommonFileDialogResult.Ok)
				return;
			using (BeginAsyncWork())
			{
				await _comic.ExportAsync(dialog.FileName, _imageList.SelectedIndices.OrderBy(x => x).Select(x => _comic.Images[x]), data =>
				{
					using var image = data.ToImage();
					var codecInfo = image.GetImageCodecInfo();
					return codecInfo == null || codecInfo.FilenameExtension == null ? string.Empty : codecInfo.FilenameExtension.Split(';')[0].Remove(0, 1);
				}, new Progress<int>(x => this.InvokeIfNeeded(() => StatusProgressBar.Value = x)));
			}
		}

		async void OnExtractImagesMenuItemClick(object? sender, EventArgs e)
		{
			using var dialog = new CPDialogs.CommonSaveFileDialog();
			dialog.DefaultExtension = ".cic";
			dialog.Filters.Add(new CPDialogs.CommonFileDialogFilter(Properties.Resources.ComicalImageCollection, "*.cic"));
			dialog.Title = Properties.Resources.ExtractImages;
			if (dialog.ShowDialog(Handle) != CPDialogs.CommonFileDialogResult.Ok)
				return;
			using (BeginAsyncWork())
				await _comic.ExtractAsync(dialog.FileName, _imageList.SelectedIndices.OrderBy(x => x).Select(x => _comic.Images[x]),
					new Progress<int>(x => this.InvokeIfNeeded(() => StatusProgressBar.Value = x)));
		}

		void OnStartViewModeSettingLeftMenuItemClick(object? sender, EventArgs e) => _imageList.SetSelectedImagesViewModes(true);

		void OnStartViewModeSettingRightMenuItemClick(object? sender, EventArgs e) => _imageList.SetSelectedImagesViewModes(false);

		void OnInvertViewModeMenuItemClick(object? sender, EventArgs e) => _imageList.InvertSelectedImagesViewModes();

		void OnSetAsThumbnailMenuItemClick(object? sender, EventArgs e)
		{
			var firstImageIndex = _imageList.SelectedIndices.FirstOrDefault(-1);
			if (firstImageIndex >= 0)
				_comic.Thumbnail = _comic.Images[firstImageIndex].Data;
		}

		#endregion

		#region BookmarkMenu

		void OnAddBookmarksMenuItemClick(object? sender, EventArgs e)
		{
			_comic.Bookmarks.AddRange(_imageList.SelectedIndices.OrderBy(x => x).Select(x => new Bookmark() { Target = x }));
		}

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
				else if (await QuerySaveAsync(res => e.Cancel = res != CPDialogs.TaskDialogResult.No) == CPDialogs.TaskDialogResult.Yes)
					Close();
			}
		}

		protected override void OnFormClosed(FormClosedEventArgs e)
		{
			base.OnFormClosed(e);
			using (var ms = new System.IO.MemoryStream())
			{
				MainDockPanel.SaveAsXml(ms, System.Text.Encoding.UTF8);
				Properties.Settings.Default.DockPanelConfiguration = System.Text.Encoding.UTF8.GetString(ms.ToArray());
			}
			Properties.Settings.Default.EditorWindowState = WindowState;
			if (WindowState == FormWindowState.Normal)
				Properties.Settings.Default.EditorWindowBounds = DesktopBounds;
			Properties.Settings.Default.Save();
		}

		#endregion

		void OnComicImagesCollectionChanged(object? sender, EventArgs e) => this.InvokeIfNeeded(() => ImageCountLabel.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.ImageCountStringRepresentation, _comic.Images.Count));

		void OnComicPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) => this.InvokeIfNeeded(UpdateTitle);
	}
}
