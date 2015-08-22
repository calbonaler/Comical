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
			comic.Images.CollectionChanged += Comic_CountChanged;
			InitializeComponent();
			InitializeDockingWindows();
			defaultProgress = new Progress<int>(value => prgStatus.Value = value);
			WindowState = Properties.Settings.Default.EditorWindowState;
			DesktopBounds = Properties.Settings.Default.EditorWindowBounds;
		}

		Comic comic;
		string savedFilePath = string.Empty;
		static readonly IReadOnlyList<string> imageExtensions = new [] { "bmp", "dib", "gif", "jpeg", "jpe", "jpg", "jfif", "png", "tiff", "tif", };
		ContentsView imageList = new ContentsView();
		BookmarksView bookmarkList = new BookmarksView();
		DocumentView document = new DocumentView();
		ViewModeSettingsView viewModeSettings = new ViewModeSettingsView();
		Progress<int> defaultProgress;

		void InitializeDockingWindows()
		{
			LoadFromXml(dpMain, Properties.Settings.Default.DockPanelConfiguration, imageList, bookmarkList, document, viewModeSettings);

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
				dialog.InstructionText = string.Format(CultureInfo.CurrentCulture, Properties.Resources.DoYouSaveChanges, HumanReadableSavedFileName);
				dialog.OwnerWindowHandle = Handle;
				TaskDialogButton SaveButton = new TaskDialogButton(nameof(SaveButton), Properties.Resources.Save);
				SaveButton.Click += (s, ev) => dialog.Close(TaskDialogResult.Yes);
				TaskDialogButton DoNotSaveButton = new TaskDialogButton(nameof(DoNotSaveButton), Properties.Resources.DoNotSave);
				DoNotSaveButton.Click += (s, ev) => dialog.Close(TaskDialogResult.No);
				TaskDialogButton CancelButton = new TaskDialogButton(nameof(CancelButton), Properties.Resources.Cancel);
				CancelButton.Click += (s, ev) => dialog.Close(TaskDialogResult.Cancel);
				dialog.Controls.Add(SaveButton);
				dialog.Controls.Add(DoNotSaveButton);
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

		static async Task CollectFiles(IEnumerable<string> paths, List<FileHeader> comicFiles, List<ImageReference> images)
		{
			foreach (var path in paths)
			{
				try
				{
					if (System.IO.Directory.Exists(path))
					{
						await CollectFiles(System.IO.Directory.EnumerateFileSystemEntries(path), comicFiles, images).ConfigureAwait(false);
						continue;
					}
					if (!System.IO.File.Exists(path))
						continue;
					var fh = new FileHeader(path);
					if (fh.CanOpen)
					{
						comicFiles.Add(fh);
						continue;
					}
					if (!imageExtensions.Any(ex => string.Equals(System.IO.Path.GetExtension(path), "." + ex, StringComparison.OrdinalIgnoreCase)))
						continue;
					using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
					{
						using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read))
							await fs.CopyToAsync(ms).ConfigureAwait(false);
						images.Add(new ImageReference(ms.ToArray()));
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

		async Task NewAsync()
		{
			if (await QuerySaveAsync() != TaskDialogResult.Cancel)
			{
				comic.Clear();
				savedFilePath = string.Empty;
				OnSavedFilePathChanged();
			}
		}

		async Task OpenAsync()
		{
			if (await QuerySaveAsync() == TaskDialogResult.Cancel)
				return;
			using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
			{
				dialog.DefaultExtension = ".cic";
				dialog.Filters.Add(new CommonFileDialogFilter("Comicファイル", "*.cic"));
				if (dialog.ShowDialog(Handle) == CommonFileDialogResult.Ok)
				{
					await AddAnythingLocalAsync(true, dialog.FileName);
					savedFilePath = dialog.FileName;
					OnSavedFilePathChanged();
				}
			}
		}

		async Task AddAnythingLocalAsync(bool canOpen, params string[] paths)
		{
			using (BeginAsyncWork())
			{
				prgStatus.Style = ProgressBarStyle.Marquee;
				lblStatus.Text = Properties.Resources.ScanningFiles;
				List<FileHeader> comicFiles = new List<FileHeader>();
				List<ImageReference> images = new List<ImageReference>();
				await CollectFiles(paths, comicFiles, images);
				prgStatus.Style = ProgressBarStyle.Blocks;
				foreach (var fileHeader in comicFiles)
				{
					string password = string.Empty;
					if (!fileHeader.IsProperPassword(password))
					{
						using (PasswordDialog dialog = new PasswordDialog())
						{
							dialog.Creating = false;
							if (dialog.ShowDialog(this) != DialogResult.OK)
								continue;
							password = dialog.Password;
							if (!fileHeader.IsProperPassword(password))
							{
								TaskDialog.Show(Properties.Resources.CannotDecryptBecausePasswordNotProper, null, Application.ProductName, TaskDialogStandardButtons.Close, TaskDialogStandardIcon.Error, ownerWindowHandle: Handle);
								continue;
							}
						}
					}
					if (canOpen)
					{
						lblStatus.Text = Properties.Resources.OpeningFile;
						await comic.OpenAsync(fileHeader.Path, password, defaultProgress);
						AddAuthorToHistory(comic.Author);
					}
					else
					{
						lblStatus.Text = Properties.Resources.AppendingFile;
						await comic.AppendAsync(fileHeader.Path, password, defaultProgress);
					}
					canOpen = false;
				}
				lblStatus.Text = Properties.Resources.ImportingImages;
				imageList.AddImages(images);
			}
		}

		async Task<bool> SaveAsync(string fileName, string password)
		{
			try
			{
				using (BeginAsyncWork())
				{
					await comic.SaveAsync(fileName, password, defaultProgress);
					AddAuthorToHistory(comic.Author);
					return true;
				}
			}
			catch (InconsistentDataException ex)
			{
				TaskDialog.Show(
					Properties.Resources.InconsistentData_Instruction,
					string.Format(Properties.Resources.InconsistentData_Text, string.Join(", ", SplitEnumValue(ex.DataTypes).Select(x => Properties.Resources.ResourceManager.GetString("InconsistentData_DataTypes_" + x.ToString(), Properties.Resources.Culture)))),
					Application.ProductName,
					TaskDialogStandardButtons.Close,
					TaskDialogStandardIcon.Error,
					ownerWindowHandle: Handle
				);
				return false;
			}
		}

		async Task<bool> SaveAsync()
		{
			if (string.IsNullOrEmpty(savedFilePath))
				return await SaveAsAsync();
			return await SaveAsync(savedFilePath, Comic.DefaultPassword);
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
						if (passDialog.ShowDialog(this) == DialogResult.OK)
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
				var result = await SaveAsync(dialog.FileName, password);
				savedFilePath = dialog.FileName;
				OnSavedFilePathChanged();
				return result;
			}
		}

		void OnSavedFilePathChanged() { Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.TitleFormat, HumanReadableSavedFileName, Application.ProductName); }

		string HumanReadableSavedFileName => string.IsNullOrEmpty(savedFilePath) ? Properties.Resources.Untitled : System.IO.Path.GetFileName(savedFilePath);

		static void LoadFromXml(WeifenLuo.WinFormsUI.Docking.DockPanel panel, string xml, params WeifenLuo.WinFormsUI.Docking.IDockContent[] contents)
		{
			if (string.IsNullOrEmpty(xml))
				return;
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(xml)))
				panel.LoadFromXml(ms, persistString => Array.Find(contents, x => string.Equals(persistString, x.DockHandler.GetPersistStringCallback(), StringComparison.Ordinal)));
		}

		static IEnumerable<T> SplitEnumValue<T>(T value) where T : struct
		{
			if (value.Equals(Enum.ToObject(typeof(T), 0)))
				return Enumerable.Empty<T>();
			return Enum.GetValues(typeof(T)).Cast<T>().Where(x => !x.Equals(Enum.ToObject(typeof(T), 0)) && ((Enum)(object)value).HasFlag((Enum)(object)x));
		}

		#region FileMenu

		async void itmNew_Click(object sender, EventArgs e) { await NewAsync(); }

		async void itmOpen_Click(object sender, EventArgs e) { await OpenAsync(); }

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
			OnSavedFilePathChanged();
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

		void Comic_CountChanged(object sender, EventArgs e) { lblImageCount.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.ImageCountStringRepresentation, comic.Images.Count); }
	}
}
