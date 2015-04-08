using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Comical.Core
{
	public class Comic : INotifyPropertyChanged
	{
		public Comic()
		{
			var context = SynchronizationContext.Current;
			Images = new ImageReferenceCollection(context);
			Images.CollectionChanged += (s, ev) => IsDirty = true;
			Images.CollectionItemPropertyChanged += (s, ev) => IsDirty = true;
			Bookmarks = new BookmarkCollection(Images, context);
			Bookmarks.CollectionChanged += (s, ev) => IsDirty = true;
			Bookmarks.CollectionItemPropertyChanged += (s, ev) => IsDirty = true;
			PropertyChanged += (s, ev) =>
			{
				if (ev.PropertyName == "Thumbnail" ||
					ev.PropertyName == "Title" ||
					ev.PropertyName == "Author" ||
					ev.PropertyName == "DateOfPublication" ||
					ev.PropertyName == "PageTurningDirection")
					IsDirty = true;
			};
		}

		string _password = "";
		bool _canDirty = true;
		static readonly Version AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
		public static readonly string DefaultPassword = null;
		
		static Tuple<FileHeader, ImageReference[]> ReadFile(string fileName, string password, BookmarkCollection bookmarks, CancellationToken token, IProgress<int> progress)
		{
			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			using (Stream readStream = FileStream.Synchronized(fs))
			{
				var cic = new FileHeader(fileName, readStream);
				// ID確認
				if (!cic.CanOpen)
					throw new ArgumentException(Properties.Resources.InvalidFileFormat);
				if (cic.FileVersion > AssemblyVersion)
					throw new ArgumentException(Properties.Resources.UnsupportedFileVersion);
				// 復号化済みマーク
				if (!cic.IsProperPassword(password))
					throw new WrongPasswordException(Properties.Resources.PasswordIsNotProper);
				var addedImages = new List<ImageReference>();
				bookmarks.Load(readStream); // 目次
				BinaryReader reader = new BinaryReader(readStream, System.Text.Encoding.Unicode);
				int images = reader.ReadInt32(); // 画像数
				for (int i = 0; i < images && !token.IsCancellationRequested; Interlocked.Increment(ref i))
				{
					if (cic.FileVersion < new Version(4, 4))
						reader.ReadString(); // 名前
					if (cic.FileVersion < new Version(4, 3))
						reader.ReadString(); // フォーマット
					ImageViewMode m = (ImageViewMode)reader.ReadByte(); // オプション
					int len = reader.ReadInt32(); // サイズ
					using (MemoryStream ms = new MemoryStream())
					{
						Crypto.Transform(readStream, ms, password, System.Text.Encoding.Unicode, len, true);
						addedImages.Add(new ImageReference(ms.ToArray()) { ViewMode = m });
					}
					if (progress != null)
						progress.Report((i + 1) * 100 / images);
				}
				token.ThrowIfCancellationRequested();
				return new Tuple<FileHeader, ImageReference[]>(cic, addedImages.ToArray());
			}
		}

		static void WriteFile(IReadOnlyList<ImageReference> images, FileHeader cic, BookmarkCollection bookmarks, IProgress<int> progress)
		{
			using (FileStream fs = new FileStream(cic.Path, FileMode.Create, FileAccess.Write))
			using (Stream writeStream = FileStream.Synchronized(fs))
			{
				cic.SaveInto(writeStream);
				bookmarks.SaveInto(writeStream);
				BinaryWriter writer = new BinaryWriter(writeStream, System.Text.Encoding.Unicode);
				writer.Write(images.Count); // 画像数
				for (int i = 0; i < images.Count; Interlocked.Increment(ref i))
				{
					ImageReference ir = images[i];
					writer.Write((byte)ir.ViewMode); // 利用情報
					writer.Write(ir.Length); // 画像データ大きさ
					using (var binImage = ir.GetReadOnlyBinaryImage())
						Crypto.Transform(binImage, writeStream, cic.Password, System.Text.Encoding.Unicode, ir.Length, false); // 画像データ
					if (progress != null)
						progress.Report((i + 1) * 100 / images.Count);
				}
			}
		}

		public void Clear()
		{
			SavedFilePath = "";
			if (Thumbnail != null)
			{
				Thumbnail.Dispose();
				Thumbnail = null;
			}
			DateOfPublication = null;
			Title = Author = "";
			Images.Clear();
			Bookmarks.Clear();
			IsDirty = false;
		}

		public async Task OpenAsync(string fileName, string password, CancellationToken token, IProgress<int> progress)
		{
			using (EnterSingleOperation())
			{
				Clear();
				using (EnterUndirtiableSection())
				using (Images.EnterUnnotifiedSection())
				{
					var res = await Task.Run(() => ReadFile(fileName, password, Bookmarks, token, progress));
					SavedFilePath = fileName;
					_password = password;
					Thumbnail = res.Item1.Thumbnail;
					FileVersion = res.Item1.FileVersion;
					Title = res.Item1.Title;
					Author = res.Item1.Author;
					DateOfPublication = res.Item1.DateOfPublication;
					PageTurningDirection = res.Item1.PageTurningDirection;
					foreach (var ir in res.Item2)
						Images.Add(ir);
				}
			}
		}

		public async Task SaveAsync(string fileName, string password, IProgress<int> progress)
		{
			using (EnterSingleOperation())
			using (EnterUndirtiableSection())
			{
				if (password == DefaultPassword)
					password = _password;
				await Task.Run(() => WriteFile(Images.ToArray(), new FileHeader(fileName, password, AssemblyVersion)
				{
					Thumbnail = Thumbnail,
					Title = Title,
					Author = Author,
					DateOfPublication = DateOfPublication,
					PageTurningDirection = PageTurningDirection
				}, Bookmarks, progress));
				FileVersion = AssemblyVersion;
				SavedFilePath = fileName;
				_password = password;
				IsDirty = false;
			}
		}

		public async Task AppendAsync(string fileName, string password, CancellationToken token, IProgress<int> progress)
		{
			using (EnterSingleOperation())
			using (Images.EnterUnnotifiedSection())
			{
				foreach (var ir in (await Task.Run(() => ReadFile(fileName, password, new BookmarkCollection(null, null), token, progress))).Item2)
					Images.Add(ir);
			}
		}

		public async Task ExportAsync(string baseDirectory, IEnumerable<ImageReference> images, IProgress<int> progress)
		{
			using (EnterSingleOperation())
			{
				var imageList = images.ToArray();
				Directory.CreateDirectory(baseDirectory);
				await Task.Run(() =>
				{
					for (int i = 0; i < imageList.Length; Interlocked.Increment(ref i))
					{
						using (var ms = imageList[i].GetReadOnlyBinaryImage())
						using (Bitmap bmp = new Bitmap(ms))
						{
							File.WriteAllBytes(Path.Combine(baseDirectory,
								i.ToString(imageList.Length - 1) + Array.Find(System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders(), item => item.FormatID == bmp.RawFormat.Guid).FilenameExtension.Split(';')[0].Remove(0, 1)),
								ms.ToArray());
						}
						if (progress != null)
							progress.Report((i + 1) * 100 / imageList.Length);
					}
				});
			}
		}

		public async Task ExtractAsync(string fileName, IEnumerable<ImageReference> images, IProgress<int> progress)
		{
			using (EnterSingleOperation())
				await Task.Run(() => WriteFile(images.ToArray(), new FileHeader(fileName, "", AssemblyVersion) { Author = Author, PageTurningDirection = PageTurningDirection }, new BookmarkCollection(null, null), progress));
		}

		public IEnumerable<Spread> ConstructSpreads(bool simpleSpread)
		{
			Spread spread = null;
			foreach (var image in Images)
			{
				if (simpleSpread || PageTurningDirection == PageTurningDirection.None || image.ViewMode == ImageViewMode.Default)
				{
					if (spread != null)
						yield return spread;
					spread = new Spread();
					spread.SetImage(image, ImageViewMode.Default);
					yield return spread;
					spread = null;
				}
				else
				{
					if (spread == null)
						spread = new Spread();
					spread.SetImage(image, image.ViewMode);
					if (PageTurningDirection == (PageTurningDirection)(3 - image.ViewMode))
					{
						yield return spread;
						spread = null;
					}
				}
			}
			if (spread != null)
				yield return spread;
		}
		
		public IDisposable EnterUndirtiableSection()
		{
			_canDirty = false;
			return new DelegateDisposable(() => _canDirty = true);
		}

		protected IDisposable EnterSingleOperation()
		{
			if (IsBusy)
				throw new InvalidOperationException(Properties.Resources.MultiAsyncOperationIsNotSupported);
			IsBusy = true;
			return new DelegateDisposable(() => IsBusy = false);
		}

		bool _busy = false;
		public bool IsBusy
		{
			get { return _busy; }
			private set
			{
				if (_busy != value)
				{
					_busy = value;
					PropertyChanged.Raise(this, null);
				}
			}
		}

		public bool HasSaved { get { return !string.IsNullOrEmpty(SavedFilePath); } }

		string _savedFilePath = "";
		public string SavedFilePath
		{
			get { return _savedFilePath; }
			private set
			{
				if (_savedFilePath != value)
				{
					_savedFilePath = value;
					PropertyChanged.Raise(this, null);
					PropertyChanged.Raise(this, null, () => HasSaved);
				}
			}
		}

		bool _dirty = false;
		public bool IsDirty
		{
			get { return _dirty; }
			private set
			{
				if (_dirty != value && (_canDirty || !value))
				{
					_dirty = value;
					PropertyChanged.Raise(this, null);
				}
			}
		}

		Version _fileVersion = AssemblyVersion;
		public Version FileVersion
		{
			get { return _fileVersion; }
			private set
			{
				if (_fileVersion != value)
				{
					_fileVersion = value;
					PropertyChanged.Raise(this, null);
				}
			}
		}

		Image _thumbnail = null;
		public Image Thumbnail
		{
			get { return _thumbnail; }
			set
			{
				if (_thumbnail != value)
				{
					_thumbnail = value;
					PropertyChanged.Raise(this, null);
				}
			}
		}

		string _title = "";
		public string Title
		{
			get { return _title; }
			set
			{
				if (_title != value)
				{
					_title = value;
					PropertyChanged.Raise(this, null);
				}
			}
		}

		string _author = "";
		public string Author
		{
			get { return _author; }
			set
			{
				if (_author != value)
				{
					_author = value;
					PropertyChanged.Raise(this, null);
				}
			}
		}

		DateTime? _dateOfPublication = null;
		public DateTime? DateOfPublication
		{
			get { return _dateOfPublication; }
			set
			{
				if (_dateOfPublication != value)
				{
					_dateOfPublication = value;
					PropertyChanged.Raise(this, null);
				}
			}
		}

		PageTurningDirection _pageTurningDirection;
		public PageTurningDirection PageTurningDirection
		{
			get { return _pageTurningDirection; }
			set
			{
				if (_pageTurningDirection != value)
				{
					_pageTurningDirection = value;
					PropertyChanged.Raise(this, null);
				}
			}
		}

		public ImageReferenceCollection Images { get; private set; }

		public BookmarkCollection Bookmarks { get; private set; }
		
		public event PropertyChangedEventHandler PropertyChanged;
	}

	public enum ImageViewMode
	{
		Default = 0,
		Left = 1,
		Right = 2,
	}

	public enum PageTurningDirection
	{
		None = 0,
		ToLeft = 1,
		ToRight = 2,
	}

	[Serializable]
	public class WrongPasswordException : Exception
	{
		public WrongPasswordException() { }

		public WrongPasswordException(string message) : base(message) { }

		public WrongPasswordException(string message, Exception innerException) : base(message, innerException) { }

		protected WrongPasswordException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
