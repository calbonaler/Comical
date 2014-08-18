using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using Comical.Core;
using CommonLibrary;
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
		string[] imageExtensions = new [] { "bmp", "dib", "gif", "jpeg", "jpe", "jpg", "jfif", "png", "tiff", "tif", };
		ContentsView imageList = new ContentsView();
		BookmarksView bookmarkList = new BookmarksView();
		ExclusionView trashBox = new ExclusionView();
		DocumentView document = new DocumentView();
		GoToIndexView goToIndex = new GoToIndexView();
		ViewModeSettingsView viewModeSettings = new ViewModeSettingsView();
		Progress<int> defaultProgress;

		void InitializeDockingWindows()
		{
			if (!string.IsNullOrEmpty(Properties.Settings.Default.DockPanelConfiguration))
			{
				using (var ms = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(Properties.Settings.Default.DockPanelConfiguration)))
				{
					dpMain.LoadFromXml(ms, ps =>
					{
						if (ps == "ImageList")
							return imageList;
						else if (ps == "BookmarkList")
							return bookmarkList;
						else if (ps == "TrashBox")
							return trashBox;
						else if (ps == "Document")
							return document;
						else if (ps == "GoToIndex")
							return goToIndex;
						else if (ps == "ViewModeSettings")
							return viewModeSettings;
						return null;
					});
				}
			}

			imageList.SetComic(comic);
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

			bookmarkList.SetImages(comic.Images);
			bookmarkList.SetBookmarks(comic.Bookmarks);
			bookmarkList.BookmarkSelected += (s, ev) => itmRemoveBookmark.Enabled = bookmarkList.SelectedBookmarks.Any();
			bookmarkList.BookmarkNavigated += (s, ev) => imageList.FirstSelectedRowIndex = ev.Bookmark.Target;

			trashBox.SetImages(comic.Images);
			trashBox.ImageReferenceSelected += (s, ev) => itmInclude.Enabled = trashBox.SelectedIndicies.Any();

			document.SetComic(comic);

			goToIndex.SetImages(comic.Images);
			goToIndex.Go += (s, ev) => imageList.FirstSelectedRowIndex = goToIndex.Index;

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

		void BeginAsyncWork()
		{
			prgStatus.Value = 0;
			prgStatus.Visible = imageList.ReadOnly = bookmarkList.ReadOnly = !(menMain.Enabled = tsMain.Enabled = false);
		}

		void EndAsyncWork()
		{
			prgStatus.Visible = imageList.ReadOnly = bookmarkList.ReadOnly = !(menMain.Enabled = tsMain.Enabled = true);
			lblStatus.Text = string.Empty;
		}

		static void EnumerateFiles(string path, Action<string> action)
		{
			try
			{
				foreach (var dir in System.IO.Directory.GetDirectories(path))
					EnumerateFiles(dir, action);
			}
			catch (UnauthorizedAccessException) { }
			foreach (var file in System.IO.Directory.GetFiles(path))
				action(file);
		}

		async Task AddAnythingLocalAsync(bool open, params string[] paths)
		{
			List<string> openableFiles = new List<string>();
			List<string> archives = new List<string>();
			FileHeader cicFileToOpen = null;
			Action<string> sortingFiles = file =>
			{
				FileHeader fh;
				if ((fh = FileHeader.Create(file)).CanOpen)
				{
					if (cicFileToOpen == null)
						cicFileToOpen = fh;
				}
				else if (imageExtensions.Any(ex => System.IO.Path.GetExtension(file).ToLowerInvariant() == "." + ex))
					openableFiles.Add(file);
				else
					archives.Add(file);
			};
			var di = CommonUtils.TempFolder.CreateSubdirectory("ZipExtract");
			try
			{
				BeginAsyncWork();
				prgStatus.Style = ProgressBarStyle.Marquee;
				lblStatus.Text = Properties.Resources.ScanningFiles;
				await Task.Run(() =>
				{
					foreach (var path in paths)
					{
						if (System.IO.File.Exists(path))
							sortingFiles(path);
						else if (System.IO.Directory.Exists(path))
							EnumerateFiles(path, sortingFiles);
					}
				});
				prgStatus.Style = ProgressBarStyle.Blocks;
				string fileSpecifier = string.Join(" ", imageExtensions.Select(it => "*." + it));
				for (int i = 0; i < archives.Count; i++)
				{
					var arc = Archivers.ArchiversConfiguration.FindArchiverToExtract(archives[i]);
					if (arc == null)
						continue;
					using (arc)
					{
						var extractFolder = System.IO.Path.Combine(di.FullName, System.IO.Path.GetFileNameWithoutExtension(archives[i]));
						arc.Extract(archives[i], extractFolder, fileSpecifier);
						EnumerateFiles(extractFolder, sortingFiles);
					}
				}
				bool hasOpened = false;
				if (cicFileToOpen != null)
				{
					string password = "";
					if (cicFileToOpen.IsProperPassword(""))
						hasOpened = true;
					else
					{
						using (PasswordDialog dialog = new PasswordDialog())
						{
							dialog.Creating = false;
							if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
							{
								if (cicFileToOpen.IsProperPassword(dialog.Password))
								{
									password = dialog.Password;
									hasOpened = true;
								}
								else
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
								}
							}
						}
					}
					if (hasOpened)
					{
						var progress = openableFiles.Any() ? new Progress<int>(x => prgStatus.Value = x / 2) : defaultProgress;
						if (open)
						{
							lblStatus.Text = Properties.Resources.OpeningFile;
							await comic.OpenAsync(cicFileToOpen.Path, password, false, System.Threading.CancellationToken.None, progress);
							Properties.Settings.Default.RecentAuthors.Add(comic.Author);
						}
						else
						{
							lblStatus.Text = Properties.Resources.AppendingFile;
							await comic.AppendAsync(cicFileToOpen.Path, password, System.Threading.CancellationToken.None, progress);
						}
					}
				}
				lblStatus.Text = Properties.Resources.ImportingImages;
				await comic.ImportImageFilesAsync(openableFiles, hasOpened ? new Progress<int>(x => prgStatus.Value = 50 + x / 2) : defaultProgress);
			}
			finally
			{
				EndAsyncWork();
				di.Delete(true);
			}
		}

		async Task<bool> SaveAsync()
		{
			if (comic.HasSaved)
			{
				try
				{
					BeginAsyncWork();
					await comic.SaveAsync(comic.SavedFilePath, Comic.DefaultPassword, defaultProgress);
					Properties.Settings.Default.RecentAuthors.Add(comic.Author);
					return true;
				}
				finally { EndAsyncWork(); }
			}
			else
				return await SaveAsAsync();
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
				if (dialog.ShowDialog(Handle) == CommonFileDialogResult.Ok)
				{
					try
					{
						BeginAsyncWork();
						await comic.SaveAsync(dialog.FileName, password, defaultProgress);
						Properties.Settings.Default.RecentAuthors.Add(comic.Author);
						return true;
					}
					finally { EndAsyncWork(); }
				}
				return false;
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
			if (await QuerySaveAsync() != TaskDialogResult.Cancel)
			{
				using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
				{
					dialog.DefaultExtension = ".cic";
					dialog.Filters.Add(new CommonFileDialogFilter("Comicファイル", "*.cic"));
					if (dialog.ShowDialog(Handle) == CommonFileDialogResult.Ok)
						await AddAnythingLocalAsync(true, dialog.FileName);
				}
			}
		}

		async void itmSave_Click(object sender, EventArgs e) { await SaveAsync(); }

		async void itmSaveAs_Click(object sender, EventArgs e) { await SaveAsAsync(); }

		void itmDocumentSettings_Click(object sender, EventArgs e) { document.Show(dpMain); }

		void itmExit_Click(object sender, EventArgs e) { this.Close(); }

		#endregion

		#region EditMenu

		void itmInvertSelections_Click(object sender, EventArgs e) { imageList.InvertSelections(); }

		void itmSelectAll_Click(object sender, EventArgs e) { imageList.SelectAll(); }

		void itmGoToIndex_Click(object sender, EventArgs e) { goToIndex.Show(dpMain); }

		#endregion

		#region ViewMenu

		void itmContentsWindow_Click(object sender, EventArgs e) { imageList.Show(dpMain); }

		void itmBookmarksWindow_Click(object sender, EventArgs e) { bookmarkList.Show(dpMain); }

		void itmTrashBox_Click(object sender, EventArgs e) { trashBox.Show(dpMain); }

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
				dialog.IsFolderPicker = true;
				if (dialog.ShowDialog(Handle) == CommonFileDialogResult.Ok)
					await AddAnythingLocalAsync(false, dialog.FileName);
			}
		}

		async void itmByBrowser_Click(object sender, EventArgs e)
		{
			using (AddImageFromWebPageDialog dialog = new AddImageFromWebPageDialog())
			{
				foreach (var item in imageExtensions)
					dialog.AllowExtensions.Add(item);
				if (dialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
				{
					try
					{
						BeginAsyncWork();
						using (WebClient client = new WebClient())
						using (comic.Images.SuspendNotification())
						{
							for (int i = 0; i < dialog.Images.Count; i++)
							{
								try { comic.ImportBinaryImage(await client.DownloadDataTaskAsync(dialog.Images[i])); }
								catch (WebException) { }
								await Task.Delay(100);
								lblStatus.Text = string.Format(CultureInfo.CurrentCulture, Properties.Resources.Downloading, dialog.Images.Count, i + 1);
								prgStatus.Value = (i + 1) * 100 / dialog.Images.Count;
							}
						}
					}
					finally { EndAsyncWork(); }
				}
			}
		}

		void itmOpenImage_Click(object sender, EventArgs e) { imageList.OpenFirstSelectedImage(); }

		void itmDelete_Click(object sender, EventArgs e) { imageList.ExcludeSelectedImages(); }

		void itmRestore_Click(object sender, EventArgs e)
		{
			foreach (var x in trashBox.SelectedIndicies.OrderBy(x => x).ToArray())
				comic.Images.Include(x);
		}

		async void itmExport_Click(object sender, EventArgs e)
		{
			using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
			{
				dialog.IsFolderPicker = true;
				dialog.Title = Properties.Resources.ExportImages;
				if (dialog.ShowDialog(Handle) == CommonFileDialogResult.Ok)
				{
					try
					{
						BeginAsyncWork();
						await comic.ExportAsync(dialog.FileName, imageList.SelectedIndicies.OrderBy(x => x).Select(x => comic.Images[x]), defaultProgress);
					}
					finally { EndAsyncWork(); }
				}
			}
		}

		async void itmExtract_Click(object sender, EventArgs e)
		{
			using (CommonSaveFileDialog dialog = new CommonSaveFileDialog())
			{
				dialog.DefaultExtension = ".cic";
				dialog.Filters.Add(new CommonFileDialogFilter("Comicファイル", "*.cic"));
				dialog.Title = Properties.Resources.ExtractImages;
				if (dialog.ShowDialog(Handle) == CommonFileDialogResult.Ok)
				{
					try
					{
						BeginAsyncWork();
						await comic.ExtractAsync(dialog.FileName, imageList.SelectedIndicies.OrderBy(x => x).Select(x => comic.Images[x]), defaultProgress);
					}
					finally { EndAsyncWork(); }
				}
			}
		}

		void itmSetViewMode_Click(object sender, EventArgs e) { viewModeSettings.Show(dpMain); }

		void itmInvertViewMode_Click(object sender, EventArgs e)
		{
			foreach (var image in comic.Images)
			{
				if (image.ViewMode == ImageViewMode.Left)
					image.ViewMode = ImageViewMode.Right;
				else if (image.ViewMode == ImageViewMode.Right)
					image.ViewMode = ImageViewMode.Left;
			}
		}

		#endregion

		#region BookmarkMenu

		void itmAddBookmark_Click(object sender, EventArgs e) { imageList.BookmarkSelectedImages(); }

		void itmDeleteBookmark_Click(object sender, EventArgs e)
		{
			foreach (var bookmark in bookmarkList.SelectedBookmarks.ToArray())
				comic.Bookmarks.Remove(bookmark);
		}

		#endregion

		#region ToolMenu

		void itmManageArchiver_Click(object sender, EventArgs e)
		{
			Action<IWin32Window> afterClosed = null;
			using (ManageArchiverDialog dialog = new ManageArchiverDialog())
			{
				dialog.ShowDialog(this);
				afterClosed = dialog.AfterClosed;
			}
			if (afterClosed != null)
				afterClosed(this);
		}

		void itmOption_Click(object sender, EventArgs e)
		{
			using (OptionDialog dialog = new OptionDialog())
				dialog.ShowDialog(this);
		}

		#endregion

		void itmAbout_Click(object sender, EventArgs e)
		{
			using (AboutDialog ss = new AboutDialog())
				ss.ShowDialog(this);
		}

		#region frmEditor EventHandlers

		protected override async void OnShown(EventArgs e)
		{
			base.OnShown(e);
			if (Properties.Settings.Default.CheckArchiverUpdateWhenStarted)
			{
				lblStatus.Text = Properties.Resources.CheckingArchiversUpdate;
				bool needUpdate = false;
				foreach (var set in Archivers.ArchiversConfiguration.Settings)
				{
					if (set.Exists && await set.IsLatestVersionAvailable())
					{
						needUpdate = true;
						break;
					}
				}
				lblStatus.Text = string.Empty;
				if (needUpdate)
				{
					using (TaskDialog dialog = new TaskDialog())
					{
						dialog.Cancelable = true;
						dialog.StartupLocation = TaskDialogStartupLocation.CenterOwner;
						dialog.OwnerWindowHandle = Handle;
						dialog.Caption = Application.ProductName;
						dialog.InstructionText = Properties.Resources.NewerVersionFoundMessage;
						dialog.Icon = TaskDialogStandardIcon.None;
						var bok = new TaskDialogButton("btnOK", Properties.Resources.Update);
						bok.UseElevationIcon = true;
						bok.Click += (s, ev) => dialog.Close(TaskDialogResult.Ok);
						var bcancel = new TaskDialogButton("btnCancel", Properties.Resources.Cancel);
						bcancel.Click += (s, ev) => dialog.Close(TaskDialogResult.Cancel);
						dialog.Controls.Add(bok);
						dialog.Controls.Add(bcancel);
						if (dialog.Show() == TaskDialogResult.Ok)
							Archivers.ArchiversConfiguration.UpdateAll(Handle);
					}
				}
			}
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
			Properties.Settings.Default.EditorWindowState = this.WindowState;
			if (this.WindowState == FormWindowState.Normal)
				Properties.Settings.Default.EditorWindowBounds = this.DesktopBounds;
			Properties.Settings.Default.Save();
		}

		#endregion

		#region Comic EventHandlers

		void Comic_CountChanged(object sender, EventArgs e)
		{
			itmSaveAs.Enabled = itmSave.Enabled = itmSelectAll.Enabled = itmInvertSelections.Enabled = itmInvertViewMode.Enabled = btnSave.Enabled = comic.Images.Count > 0;
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
