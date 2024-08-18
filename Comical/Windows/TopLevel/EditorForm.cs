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
			_comic.PropertyChanged += Comic_PropertyChanged;
			_comic.Images.CollectionChanged += Comic_CountChanged;
			InitializeComponent();
			InitializeDockingWindows();
			WindowState = Properties.Settings.Default.EditorWindowState;
			DesktopBounds = Properties.Settings.Default.EditorWindowBounds;
		}

		readonly Comic _comic = new Comic();
		string _savedFilePath;
		static readonly IReadOnlyList<string> ImageExtensions = new[] { "bmp", "dib", "gif", "jpeg", "jpe", "jpg", "jfif", "png", "tiff", "tif", };
		readonly ContentsView _imageList = new ContentsView();
		readonly BookmarksView _bookmarkList = new BookmarksView();
		readonly DocumentView _document = new DocumentView();

		string SavedFilePath
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
			dpMain.Theme = new WeifenLuo.WinFormsUI.Docking.VS2015LightTheme();

			LoadFromXml(dpMain, Properties.Settings.Default.DockPanelConfiguration, _imageList, _bookmarkList, _document);

			_imageList.SetImages(_comic.Images);
			_imageList.FileDropped += async (s, ev) => await AddAnythingLocalAsync(!ev.Control, ev.FileNames);
			_imageList.ImageReferenceSelected += (s, ev) => itmAddBookmark.Enabled = itmOpenImage.Enabled = itmExclude.Enabled = itmExport.Enabled = itmExtract.Enabled = itmSetViewMode.Enabled = itmInvertViewMode.Enabled = _imageList.SelectedIndicies.Any();
			_imageList.ExportRequested += itmExport_Click;
			_imageList.ExtractRequested += itmExtract_Click;
			_imageList.BookmarkRequested += itmAddBookmark_Click;

			_bookmarkList.SetImages(_comic.Images);
			_bookmarkList.SetBookmarks(_comic.Bookmarks);
			_bookmarkList.BookmarkSelected += (s, ev) => itmDeleteBookmark.Enabled = _bookmarkList.SelectedBookmarks.Any();
			_bookmarkList.BookmarkNavigated += (s, ev) => _imageList.FirstSelectedRowIndex = ev.Bookmark.Target;

			_document.SetComic(_comic);
		}

		async Task<CPDialogs.TaskDialogResult> QuerySaveAsync(Action<CPDialogs.TaskDialogResult> beforeSave = null)
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
			prgStatus.Value = 0;
			prgStatus.Visible = !(menMain.Enabled = tsMain.Enabled = false);
			return new CompositeDisposable()
			{
				() =>
				{
					prgStatus.Visible = !(menMain.Enabled = tsMain.Enabled = true);
					lblStatus.Text = string.Empty;
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
				prgStatus.Style = ProgressBarStyle.Marquee;
				lblStatus.Text = Properties.Resources.ScanningFiles;
				var comicFiles = new List<string>();
				var images = new List<ImageReference>();
				await CollectFilesAsync(paths, comicFiles, images);
				prgStatus.Style = ProgressBarStyle.Blocks;
				var i = 0;
				if (canOpen && comicFiles.Count > 0)
				{
					lblStatus.Text = Properties.Resources.OpeningFile;
					await _comic.OpenAsync(comicFiles[i], new Progress<int>(x => this.InvokeIfNeeded(() => prgStatus.Value = x / comicFiles.Count)));
					AddAuthorToHistory(_comic.Author);
					SavedFilePath = comicFiles[i];
					i++;
				}
				lblStatus.Text = Properties.Resources.AppendingFile;
				for (; i < comicFiles.Count; i++)
					await _comic.AppendAsync(comicFiles[i], new Progress<int>(x => this.InvokeIfNeeded(() => prgStatus.Value = (100 * i + x) / comicFiles.Count)));
				lblStatus.Text = Properties.Resources.ImportingImages;
				_imageList.AddImages(images);
			}
		}

		async Task<bool> SaveAsync(string fileName)
		{
			try
			{
				using (BeginAsyncWork())
				{
					await _comic.SaveAsync(fileName, new Progress<int>(x => this.InvokeIfNeeded(() => prgStatus.Value = x)));
					AddAuthorToHistory(_comic.Author);
					return true;
				}
			}
			catch (InconsistentDataException ex)
			{
				using var dialog = new CPDialogs.TaskDialog();
				dialog.InstructionText = Properties.Resources.InconsistentData_Instruction;
				dialog.Text = string.Format(Properties.Resources.InconsistentData_Text, string.Join(", ", SplitEnumValue(ex.DataTypes).Select(x => Properties.Resources.ResourceManager.GetString("InconsistentData_DataTypes_" + x.ToString(), Properties.Resources.Culture))));
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

		static IEnumerable<T> SplitEnumValue<T>(T value) where T : Enum
		{
			var enumZero = Enum.ToObject(typeof(T), 0);
			return value.Equals(enumZero) ? Enumerable.Empty<T>()
				: ((T[])Enum.GetValues(typeof(T))).Where(x => !x.Equals(enumZero) && value.HasFlag(x));
		}

		#region FileMenu

		async void itmNew_Click(object sender, EventArgs e)
		{
			if (await QuerySaveAsync() != CPDialogs.TaskDialogResult.Cancel)
			{
				_comic.Clear();
				SavedFilePath = string.Empty;
			}
		}

		async void itmOpen_Click(object sender, EventArgs e)
		{
			if (await QuerySaveAsync() == CPDialogs.TaskDialogResult.Cancel)
				return;
			using var dialog = new CPDialogs.CommonOpenFileDialog();
			dialog.DefaultExtension = ".cic";
			dialog.Filters.Add(new CPDialogs.CommonFileDialogFilter(Properties.Resources.ComicalImageCollection, "*.cic"));
			if (dialog.ShowDialog(Handle) == CPDialogs.CommonFileDialogResult.Ok)
				await AddAnythingLocalAsync(true, Enumerable.Repeat(dialog.FileName, 1));
		}

		async void itmSave_Click(object sender, EventArgs e) => await SaveAsync();

		async void itmSaveAs_Click(object sender, EventArgs e) => await SaveAsAsync();

		void itmDocumentSettings_Click(object sender, EventArgs e) => _document.Show(dpMain);

		void itmExit_Click(object sender, EventArgs e) => Close();

		#endregion

		#region ViewMenu

		void itmContentsWindow_Click(object sender, EventArgs e) => _imageList.Show(dpMain);

		void itmBookmarksWindow_Click(object sender, EventArgs e) => _bookmarkList.Show(dpMain);

		#endregion

		#region ImageMenu

		async void itmFromFile_Click(object sender, EventArgs e)
		{
			using var dialog = new CPDialogs.CommonOpenFileDialog();
			dialog.Multiselect = true;
			if (dialog.ShowDialog(Handle) == CPDialogs.CommonFileDialogResult.Ok)
				await AddAnythingLocalAsync(false, dialog.FileNames);
		}

		async void itmFromFolder_Click(object sender, EventArgs e)
		{
			using var dialog = new CPDialogs.CommonOpenFileDialog();
			dialog.Multiselect = true;
			dialog.IsFolderPicker = true;
			if (dialog.ShowDialog(Handle) == CPDialogs.CommonFileDialogResult.Ok)
				await AddAnythingLocalAsync(false, dialog.FileNames);
		}

		void itmOpenImage_Click(object sender, EventArgs e) => _imageList.OpenFirstSelectedImage();

		void itmDelete_Click(object sender, EventArgs e) => _imageList.DeleteSelectedImages();

		async void itmExport_Click(object sender, EventArgs e)
		{
			using var dialog = new CPDialogs.CommonOpenFileDialog();
			dialog.IsFolderPicker = true;
			dialog.Title = Properties.Resources.ExportImages;
			if (dialog.ShowDialog(Handle) != CPDialogs.CommonFileDialogResult.Ok)
				return;
			using (BeginAsyncWork())
			{
				await _comic.ExportAsync(dialog.FileName, _imageList.SortedSelectedImages, data =>
				{
					using var image = data.ToImage();
					return image.GetImageCodecInfo().FilenameExtension.Split(';')[0].Remove(0, 1);
				}, new Progress<int>(x => this.InvokeIfNeeded(() => prgStatus.Value = x)));
			}
		}

		async void itmExtract_Click(object sender, EventArgs e)
		{
			using var dialog = new CPDialogs.CommonSaveFileDialog();
			dialog.DefaultExtension = ".cic";
			dialog.Filters.Add(new CPDialogs.CommonFileDialogFilter(Properties.Resources.ComicalImageCollection, "*.cic"));
			dialog.Title = Properties.Resources.ExtractImages;
			if (dialog.ShowDialog(Handle) != CPDialogs.CommonFileDialogResult.Ok)
				return;
			using (BeginAsyncWork())
				await _comic.ExtractAsync(dialog.FileName, _imageList.SortedSelectedImages, new Progress<int>(x => this.InvokeIfNeeded(() => prgStatus.Value = x)));
		}

		void itmWithLeft_Click(object sender, EventArgs e) => _imageList.SetViewModes(true);

		void itmWithRight_Click(object sender, EventArgs e) => _imageList.SetViewModes(false);

		void itmInvertViewMode_Click(object sender, EventArgs e) => _imageList.InvertViewMode();

		#endregion

		#region BookmarkMenu

		void itmAddBookmark_Click(object sender, EventArgs e) => _bookmarkList.AddBookmarks(_imageList.SelectedIndicies.OrderBy(x => x));

		void itmDeleteBookmark_Click(object sender, EventArgs e) => _bookmarkList.DeleteSelectedBookmarks();

		#endregion

		void itmOption_Click(object sender, EventArgs e)
		{
			using var dialog = new OptionDialog();
			dialog.ShowDialog(this);
		}

		void itmAbout_Click(object sender, EventArgs e)
		{
			using var ss = new AboutDialog();
			ss.ShowDialog(this);
		}

		#region frmEditor EventHandlers

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
				if (prgStatus.Visible)
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
				dpMain.SaveAsXml(ms, System.Text.Encoding.UTF8);
				Properties.Settings.Default.DockPanelConfiguration = System.Text.Encoding.UTF8.GetString(ms.ToArray());
			}
			Properties.Settings.Default.EditorWindowState = WindowState;
			if (WindowState == FormWindowState.Normal)
				Properties.Settings.Default.EditorWindowBounds = DesktopBounds;
			Properties.Settings.Default.Save();
		}

		#endregion

		void Comic_CountChanged(object sender, EventArgs e) => this.InvokeIfNeeded(() => lblImageCount.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.ImageCountStringRepresentation, _comic.Images.Count));

		void Comic_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) => this.InvokeIfNeeded(UpdateTitle);
	}
}
