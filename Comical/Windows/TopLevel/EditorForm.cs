using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Comical.Core;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Comical
{
	public partial class EditorForm : Form
	{
		public EditorForm()
		{
			// Comic オブジェクトは必ずコンストラクタ内で初期化しなければならない。
			// メンバ変数の宣言と同時に初期化すると、Application.Run() 前であるため、
			// SynchronizationContext.Current が null となり呼び出しがマーシャリングされない。
			comic = new Comic();
			comic.PropertyChanged += Comic_PropertyChanged;
			comic.Images.CollectionChanged += Comic_CountChanged;
			Comic_PropertyChanged(comic, new System.ComponentModel.PropertyChangedEventArgs("SavedFilePath"));
			InitializeComponent();
			InitializeDockingWindows();
			defaultProgress = new Progress<int>(value => prgStatus.Value = value);
			WindowState = Properties.Settings.Default.EditorWindowState;
			DesktopBounds = Properties.Settings.Default.EditorWindowBounds;
		}

		Comic comic;
		static readonly IReadOnlyList<string> imageExtensions = new [] { "bmp", "dib", "gif", "jpeg", "jpe", "jpg", "jfif", "png", "tiff", "tif", };
		ContentsView imageList = new ContentsView();
		BookmarksView bookmarkList = new BookmarksView();
		DocumentView document = new DocumentView();
		ViewModeSettingsView viewModeSettings = new ViewModeSettingsView();
		Progress<int> defaultProgress;

		void InitializeDockingWindows()
		{
			dpMain.LoadFromXml(Properties.Settings.Default.DockPanelConfiguration, imageList, bookmarkList, document, viewModeSettings);

			imageList.SetImages(comic.Images);
			imageList.FileDropped += async (s, ev) => await AddAnythingLocalAsync(!ev.Control, ev.FileNames.ToArray());
			imageList.ImageReferenceSelected += (s, ev) =>
			{
				itmAddBookmark.Enabled = itmOpenImage.Enabled = itmExclude.Enabled = itmExport.Enabled = itmExtract.Enabled = imageList.SelectedIndicies.Any();
				if (imageList.SelectedIndicies.Any())
				{
					viewModeSettings.SelectionStart = imageList.SelectedIndicies.Last();
					viewModeSettings.SelectionLength = imageList.SelectedIndicies.First() - viewModeSettings.SelectionStart + 1;
				}
			};
			imageList.ExportRequested += itmExport_Click;
			imageList.ExtractRequested += itmExtract_Click;
			imageList.BookmarkRequested += itmAddBookmark_Click;

			bookmarkList.SetImages(comic.Images);
			bookmarkList.SetBookmarks(comic.Bookmarks);
			bookmarkList.BookmarkSelected += (s, ev) => itmRemoveBookmark.Enabled = bookmarkList.SelectedBookmarks.Any();
			bookmarkList.BookmarkNavigated += (s, ev) => imageList.FirstSelectedRowIndex = ev.Bookmark.Target;

			document.SetComic(comic);

			viewModeSettings.SetImages(comic.Images);
		}

		async Task<TaskDialogResult> QuerySaveAsync(Action<TaskDialogResult> beforeSave = null)
		{
			if (!comic.IsDirty)
				return TaskDialogResult.No;
			using (TaskDialog dialog = new TaskDialog())
			{
				dialog.Caption = Application.ProductName;
				dialog.InstructionText = string.Format(CultureInfo.CurrentCulture, Properties.Resources.DoYouSaveChanges, comic.HasSaved ? comic.SavedFilePath : Properties.Resources.Untitled);
				dialog.OwnerWindowHandle = Handle;
				var SaveButton = new TaskDialogButton("SaveButton", Properties.Resources.Save);
				SaveButton.Click += (s, ev) => dialog.Close(TaskDialogResult.Yes);
				var DontSaveButton = new TaskDialogButton("DontSaveButton", Properties.Resources.DontSave);
				DontSaveButton.Click += (s, ev) => dialog.Close(TaskDialogResult.No);
				var CancelButton = new TaskDialogButton("CancelButton", Properties.Resources.Cancel);
				CancelButton.Click += (s, ev) => dialog.Close(TaskDialogResult.Cancel);
				dialog.Controls.Add(SaveButton);
				dialog.Controls.Add(DontSaveButton);
				dialog.Controls.Add(CancelButton);
				dialog.Cancelable = true;
				dialog.StartupLocation = TaskDialogStartupLocation.CenterOwner;
				var result = dialog.Show();
				if (beforeSave != null)
					beforeSave(result);
				if (result == TaskDialogResult.Yes && !await SaveAsync())
					result = TaskDialogResult.Cancel;
				return result;
			}
		}

		IDisposable BeginAsyncWork()
		{
			prgStatus.Value = 0;
			prgStatus.Visible = !(menMain.Enabled = tsMain.Enabled = false);
			var imageListWork = imageList.BeginAsyncWork();
			var bookmarkListWork = bookmarkList.BeginAsyncWork();
			return new DelegateDisposable(() =>
			{
				bookmarkListWork.Dispose();
				imageListWork.Dispose();
				prgStatus.Visible = !(menMain.Enabled = tsMain.Enabled = true);
				lblStatus.Text = string.Empty;
			});
		}

		static void CollectFiles(IEnumerable<string> paths, List<FileHeader> comicFiles, List<ImageReference> images)
		{
			foreach (var path in paths)
			{
				try
				{
					if (System.IO.Directory.Exists(path))
						CollectFiles(System.IO.Directory.EnumerateFileSystemEntries(path), comicFiles, images);
					else if (System.IO.File.Exists(path))
					{
						var fh = new FileHeader(path);
						if (fh.CanOpen)
							comicFiles.Add(fh);
						else if (imageExtensions.Any(ex => string.Equals(System.IO.Path.GetExtension(path), "." + ex, StringComparison.OrdinalIgnoreCase)))
							images.Add(new ImageReference(System.IO.File.ReadAllBytes(path)));
					}
				}
				catch (UnauthorizedAccessException) { }
			}
		}

		static void AddAuthorToHistory(string author)
		{
			for (int i = Properties.Settings.Default.RecentAuthors.Count - 1; i >= 0; i--)
			{
				if (author.Equals(Properties.Settings.Default.RecentAuthors[i], StringComparison.CurrentCulture))
					Properties.Settings.Default.RecentAuthors.RemoveAt(i);
			}
			Properties.Settings.Default.RecentAuthors.Insert(0, author);
		}

		async Task AddAnythingLocalAsync(bool canOpen, params string[] paths)
		{
			List<FileHeader> comicFiles = new List<FileHeader>();
			List<ImageReference> images = new List<ImageReference>();
			using (BeginAsyncWork())
			{
				prgStatus.Style = ProgressBarStyle.Marquee;
				lblStatus.Text = Properties.Resources.ScanningFiles;
				await Task.Run(() => CollectFiles(paths, comicFiles, images));
				prgStatus.Style = ProgressBarStyle.Blocks;
				foreach (var fileHeader in comicFiles)
				{
					string password = string.Empty;
					if (!fileHeader.IsProperPassword(password))
					{
						using (PasswordDialog dialog = new PasswordDialog())
						{
							dialog.Creating = false;
							if (dialog.ShowDialog(this) != System.Windows.Forms.DialogResult.OK)
								continue;
							password = dialog.Password;
							if (!fileHeader.IsProperPassword(password))
							{
								using (TaskDialog errorDialog = new TaskDialog())
								{
									errorDialog.Cancelable = true;
									errorDialog.StartupLocation = TaskDialogStartupLocation.CenterOwner;
									errorDialog.OwnerWindowHandle = Handle;
									errorDialog.Caption = Application.ProductName;
									errorDialog.InstructionText = Properties.Resources.CannotDecryptBecausePasswordNotProper;
									errorDialog.Icon = TaskDialogStandardIcon.Error;
									errorDialog.StandardButtons = TaskDialogStandardButtons.Close;
									errorDialog.Show();
								}
								continue;
							}
						}
					}
					if (canOpen)
					{
						lblStatus.Text = Properties.Resources.OpeningFile;
						await comic.OpenAsync(fileHeader.Path, password, System.Threading.CancellationToken.None, defaultProgress);
						AddAuthorToHistory(comic.Author);
					}
					else
					{
						lblStatus.Text = Properties.Resources.AppendingFile;
						await comic.AppendAsync(fileHeader.Path, password, System.Threading.CancellationToken.None, defaultProgress);
					}
					canOpen = false;
				}
				lblStatus.Text = Properties.Resources.ImportingImages;
				Application.DoEvents();
				imageList.AddImages(images);
			}
		}

		async Task<bool> SaveAsync()
		{
			if (!comic.HasSaved)
				return await SaveAsAsync();
			using (BeginAsyncWork())
			{
				await comic.SaveAsync(comic.SavedFilePath, Comic.DefaultPassword, defaultProgress);
				AddAuthorToHistory(comic.Author);
				return true;
			}
		}

		async Task<bool> SaveAsAsync()
		{
			using (CommonSaveFileDialog dialog = new CommonSaveFileDialog())
			{
				dialog.Filters.Add(new CommonFileDialogFilter("Comicファイル", "*.cic"));
				dialog.AlwaysAppendDefaultExtension = true;
				dialog.DefaultExtension = "cic";
				var btnProtection = new Microsoft.WindowsAPICodePack.Dialogs.Controls.CommonFileDialogMenuItem(Properties.Resources.ConfigureProtection);
				string password = "";
				btnProtection.Click += (s, ev) =>
				{
					using (PasswordDialog passDialog = new PasswordDialog())
					{
						passDialog.Password = password;
						passDialog.Creating = true;
						if (passDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
							password = passDialog.Password;
					}
				};
				var menTool = new Microsoft.WindowsAPICodePack.Dialogs.Controls.CommonFileDialogMenu(Properties.Resources.Tool);
				menTool.Items.Add(btnProtection);
				dialog.Controls.Add(menTool);
				if (Properties.Settings.Default.DefaultSavedFileName.Length > 0)
					dialog.DefaultFileName = string.Format(CultureInfo.CurrentCulture, Properties.Settings.Default.DefaultSavedFileName, comic.Title, comic.Author, comic.DateOfPublication);
				if (dialog.ShowDialog(Handle) != CommonFileDialogResult.Ok)
					return false;
				using (BeginAsyncWork())
				{
					await comic.SaveAsync(dialog.FileName, password, defaultProgress);
					AddAuthorToHistory(comic.Author);
					return true;
				}
			}
		}

		#region FileMenu

		async void itmNew_Click(object sender, EventArgs e)
		{
			if (await QuerySaveAsync() != TaskDialogResult.Cancel)
				comic.Clear();
		}

		async void itmOpen_Click(object sender, EventArgs e)
		{
			if (await QuerySaveAsync() == TaskDialogResult.Cancel)
				return;
			using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
			{
				dialog.DefaultExtension = ".cic";
				dialog.Filters.Add(new CommonFileDialogFilter("Comicファイル", "*.cic"));
				if (dialog.ShowDialog(Handle) == CommonFileDialogResult.Ok)
					await AddAnythingLocalAsync(true, dialog.FileName);
			}
		}

		async void itmSave_Click(object sender, EventArgs e) { await SaveAsync(); }

		async void itmSaveAs_Click(object sender, EventArgs e) { await SaveAsAsync(); }

		void itmDocumentSettings_Click(object sender, EventArgs e) { document.Show(dpMain); }

		void itmExit_Click(object sender, EventArgs e) { Close(); }

		#endregion

		#region EditMenu

		void itmInvertSelections_Click(object sender, EventArgs e) { imageList.InvertSelections(); }

		void itmSelectAll_Click(object sender, EventArgs e) { imageList.SelectAll(); }

		#endregion

		#region ViewMenu

		void itmContentsWindow_Click(object sender, EventArgs e) { imageList.Show(dpMain); }

		void itmBookmarksWindow_Click(object sender, EventArgs e) { bookmarkList.Show(dpMain); }

		#endregion

		#region ImageMenu

		async void itmFromFile_Click(object sender, EventArgs e)
		{
			using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
			{
				dialog.Multiselect = true;
				if (dialog.ShowDialog(Handle) == CommonFileDialogResult.Ok)
					await AddAnythingLocalAsync(false, dialog.FileNames.ToArray());
			}
		}

		async void itmFromFolder_Click(object sender, EventArgs e)
		{
			using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
			{
				dialog.Multiselect = true;
				dialog.IsFolderPicker = true;
				if (dialog.ShowDialog(Handle) == CommonFileDialogResult.Ok)
					await AddAnythingLocalAsync(false, dialog.FileNames.ToArray());
			}
		}

		void itmOpenImage_Click(object sender, EventArgs e) { imageList.OpenFirstSelectedImage(); }

		void itmDelete_Click(object sender, EventArgs e) { imageList.DeleteSelectedImages(); }

		async void itmExport_Click(object sender, EventArgs e)
		{
			using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
			{
				dialog.IsFolderPicker = true;
				dialog.Title = Properties.Resources.ExportImages;
				if (dialog.ShowDialog(Handle) != CommonFileDialogResult.Ok)
					return;
				using (BeginAsyncWork())
					await comic.ExportAsync(dialog.FileName, imageList.SortedSelectedImages, defaultProgress);
			}
		}

		async void itmExtract_Click(object sender, EventArgs e)
		{
			using (CommonSaveFileDialog dialog = new CommonSaveFileDialog())
			{
				dialog.DefaultExtension = ".cic";
				dialog.Filters.Add(new CommonFileDialogFilter("Comicファイル", "*.cic"));
				dialog.Title = Properties.Resources.ExtractImages;
				if (dialog.ShowDialog(Handle) != CommonFileDialogResult.Ok)
					return;
				using (BeginAsyncWork())
					await comic.ExtractAsync(dialog.FileName, imageList.SortedSelectedImages, defaultProgress);
			}
		}

		void itmSetViewMode_Click(object sender, EventArgs e) { viewModeSettings.Show(dpMain); }

		void itmInvertViewMode_Click(object sender, EventArgs e) { viewModeSettings.InvertViewMode(); }

		#endregion

		#region BookmarkMenu

		void itmAddBookmark_Click(object sender, EventArgs e) { bookmarkList.AddBookmarks(imageList.SelectedIndicies.OrderBy(x => x)); }

		void itmDeleteBookmark_Click(object sender, EventArgs e) { bookmarkList.DeleteSelectedBookmarks(); }

		#endregion

		void itmOption_Click(object sender, EventArgs e)
		{
			using (OptionDialog dialog = new OptionDialog())
				dialog.ShowDialog(this);
		}

		void itmAbout_Click(object sender, EventArgs e)
		{
			using (AboutDialog ss = new AboutDialog())
				ss.ShowDialog(this);
		}

		#region frmEditor EventHandlers

		protected override async void OnShown(EventArgs e)
		{
			base.OnShown(e);
			var args = Environment.GetCommandLineArgs();
			if (args.Length >= 2)
				await AddAnythingLocalAsync(true, args[1]);
		}

		protected override async void OnFormClosing(FormClosingEventArgs e)
		{
			base.OnFormClosing(e);
			if (e == null) return;
			if (e.CloseReason != CloseReason.WindowsShutDown)
			{
				if (prgStatus.Visible)
					e.Cancel = true;
				else if (await QuerySaveAsync(res => e.Cancel = res != TaskDialogResult.No) == TaskDialogResult.Yes)
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

		#region Comic EventHandlers

		void Comic_CountChanged(object sender, EventArgs e)
		{
			lblImageCount.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.ImageCountStringRepresentation, comic.Images.Count);
		}

		void Comic_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case "SavedFilePath":
					if (!string.IsNullOrEmpty(comic.SavedFilePath))
						Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.TitleFormat, comic.SavedFilePath, Application.ProductName);
					else
						Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.TitleFormat, Properties.Resources.Untitled, Application.ProductName);
					break;
			}
		}

		#endregion
	}
}
