using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CommonLibrary;

namespace Comical.Core
{
	public class Comic : IDisposable, INotifyPropertyChanged
	{
		public Comic()
		{
			_context = SynchronizationContext.Current;
			_blocker.PropertyChanged += (s, ev) => RaisePropertyChanged(() => IsBusy);
			Images = new ImageReferenceCollection((ir, sz) =>
			{
				lock (_lockObject)
				{
					using (var ms = GetBinaryImage(ir))
					{
						using (var src = Image.FromStream(ms, false, false))
							return src.Resize(sz);
					}
				}
			}, _context);
			Images.CollectionChanged += (s, ev) => { if (canDirty) IsDirty = true; };
			Images.CollectionItemPropertyChanged += (s, ev) => { if (canDirty) IsDirty = true; };
			Bookmarks = new BookmarkCollection(x => Images[x], _context);
			Bookmarks.CollectionChanged += (s, ev) => { if (canDirty) IsDirty = true; };
			Bookmarks.CollectionItemPropertyChanged += (s, ev) => { if (canDirty) IsDirty = true; };
			PropertyChanged += (s, ev) =>
			{
				if (canDirty && (ev.PropertyName == "Thumbnail" ||
					ev.PropertyName == "Title" ||
					ev.PropertyName == "Author" ||
					ev.PropertyName == "DateOfPublication" ||
					ev.PropertyName == "PageTurningDirection"))
					IsDirty = true;
			};
		}

		Stream _target;
		Stream _saved;
		string _password = "";
		SynchronizationContext _context;
		MultioperationBlocker _blocker = new MultioperationBlocker();
		bool canDirty = true;
		readonly object _lockObject = new object();
		static readonly Version AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
		public static readonly string DefaultPassword = null;

		public event PropertyChangedEventHandler PropertyChanged;

		public void StartDirtyCheck() { canDirty = true; }

		public void StopDirtyCheck() { canDirty = false; }

		protected void RaisePropertyChanged<T>(System.Linq.Expressions.Expression<Func<T>> property)
		{
			var memberExp = property.Body as System.Linq.Expressions.MemberExpression;
			if (memberExp != null && PropertyChanged != null)
				_context.SendIfNeeded(() => PropertyChanged(this, new PropertyChangedEventArgs(memberExp.Member.Name)));
		}

		protected void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName]string propertyName = "") { _context.SendIfNeeded(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName))); }

		string TargetPath { get { return Path.Combine(CommonUtils.TempFolder.FullName, GetHashCode().ToString("x", CultureInfo.InvariantCulture) + ".tmp"); } }

		FileHeader ReadFile(string fileName, Stream readStream, string password, bool readOnly, BookmarkCollection bookmarks, CancellationToken token, IProgress<int> progress)
		{
			var cic = FileHeader.Create(fileName, readStream);
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
				lock (_lockObject)
				{
					if (cic.FileVersion < new Version(4, 4))
						reader.ReadString(); // 名前
					if (cic.FileVersion < new Version(4, 3))
						reader.ReadString(); // フォーマット
					ImageViewMode m = (ImageViewMode)reader.ReadByte(); // オプション
					int len = reader.ReadInt32(); // サイズ
					if (readOnly)
					{
						addedImages.Add(new ImageReference(m, readStream.Position, len));
						readStream.Seek((long)len, SeekOrigin.Current);
					}
					else
					{
						AllocateWorkspace();
						addedImages.Add(new ImageReference(m, _target.Position, len));
						Crypto.Decrypt(readStream, _target, password, System.Text.Encoding.Unicode, len);
					}
				}
				if (progress != null)
					progress.Report((i + 1) * 100 / images);
			}
			token.ThrowIfCancellationRequested();
			foreach (var addedImage in addedImages)
				Images.Add(addedImage);
			return cic;
		}

		void WriteFile(IReadOnlyList<ImageReference> images, FileHeader cic, BookmarkCollection bookmarks, IProgress<int> progress)
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
					lock (_lockObject)
					{
						ImageReference ir = images[i];
						writer.Write((byte)ir.ViewMode); // 利用情報
						writer.Write(ir.Length); // 画像データ大きさ
						_target.Seek(ir.Position, SeekOrigin.Begin);
						Crypto.Encrypt(_target, writeStream, cic.Password, System.Text.Encoding.Unicode, ir.Length); // 画像データ
					}
					if (progress != null)
						progress.Report((i + 1) * 100 / images.Count);
				}
			}
		}

		public void Clear()
		{
			ThrowIfDisposed();
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
			if (_saved != null)
			{
				_saved.Dispose();
				_saved = null;
			}
			if (_target != null)
			{
				_target.Dispose();
				_target = null;
			}
			IsDirty = false;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				canDirty = false;
				if (Bookmarks != null)
					Bookmarks.Clear();
				if (Images != null)
					Images.Clear();
				_author = null;
				_blocker = null;
				_context = null;
				_dateOfPublication = null;
				_dirty = false;
				_fileVersion = null;
				_pageTurningDirection = Core.PageTurningDirection.None;
				_password = null;
				_readOnly = false;
				if (_saved != null)
				{
					_saved.Dispose();
					_saved = null;
				}
				_savedFilePath = null;
				if (_target != null)
				{
					_target.Dispose();
					try { File.Delete(TargetPath); }
					catch { }
					_target = null;
				}
				if (_thumbnail != null)
				{
					_thumbnail.Dispose();
					_thumbnail = null;
				}
				_title = null;
				Bookmarks = null;
				Images = null;
			}
		}

		void AllocateWorkspace()
		{
			if (_target == null)
				_target = FileStream.Synchronized(new FileStream(TargetPath, FileMode.Create));
		}

		void ImportBinaryImageInternal(byte[] image)
		{
			AllocateWorkspace();
			_target.Seek(0, SeekOrigin.End);
			Images.Add(new ImageReference(ImageViewMode.Default, _target.Position, image.Length));
			_target.Write(image, 0, image.Length);
		}

		public void ImportBinaryImage(byte[] image)
		{
			ThrowIfDisposed();
			if (image == null)
				throw new ArgumentNullException("image");
			ThrowIfReadOnly();
			lock (_lockObject)
				ImportBinaryImageInternal(image);
		}

		public async Task OpenAsync(string fileName, string password, bool readOnly, CancellationToken token, IProgress<int> progress)
		{
			ThrowIfDisposed();
			using (_blocker.Enter())
			{
				Clear();
				Stream temp = null;
				try
				{
					StopDirtyCheck();
					temp = FileStream.Synchronized(new FileStream(fileName, FileMode.Open, FileAccess.Read));
					FileHeader cic = null;
					using (Images.SuspendNotification())
						cic = await Task.Run(() => ReadFile(fileName, temp, password, readOnly, Bookmarks, token, progress));
					SavedFilePath = fileName;
					IsReadOnly = readOnly;
					_password = password;
					Thumbnail = cic.Thumbnail;
					FileVersion = cic.FileVersion;
					Title = cic.Title;
					Author = cic.Author;
					DateOfPublication = cic.DateOfPublication;
					PageTurningDirection = cic.PageTurningDirection;
					if (readOnly)
					{
						_saved = temp;
						temp = null;
					}
				}
				finally
				{
					if (temp != null)
						temp.Dispose();
					StartDirtyCheck();
				}
			}
		}

		public async Task SaveAsync(string fileName, string password, IProgress<int> progress)
		{
			ThrowIfDisposed();
			ThrowIfReadOnly();
			using (_blocker.Enter())
			{
				StopDirtyCheck();
				if (password == DefaultPassword)
					password = _password;
				await Task.Run(() => WriteFile(Images, FileHeader.Create(fileName, Thumbnail, AssemblyVersion, password, Title, Author, DateOfPublication, PageTurningDirection), Bookmarks, progress));
				FileVersion = AssemblyVersion;
				SavedFilePath = fileName;
				_password = password;
				IsDirty = false;
				StartDirtyCheck();
			}
		}

		public async Task AppendAsync(string fileName, string password, CancellationToken token, IProgress<int> progress)
		{
			ThrowIfDisposed();
			ThrowIfReadOnly();
			using (_blocker.Enter())
			using (Images.SuspendNotification())
			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			using (Stream readStream = FileStream.Synchronized(fs))
				await Task.Run(() => ReadFile(fileName, readStream, password, false, new BookmarkCollection(null, null), token, progress));
		}

		public async Task ImportImageFilesAsync(IEnumerable<string> fileNames, IProgress<int> progress)
		{
			ThrowIfDisposed();
			ThrowIfReadOnly();
			using (_blocker.Enter())
			using (Images.SuspendNotification())
			{
				var fileNameList = fileNames.ToArray();
				await Task.Run(() =>
				{
					for (int i = 0; i < fileNameList.Length; Interlocked.Increment(ref i))
					{
						lock (_lockObject)
							ImportBinaryImageInternal(File.ReadAllBytes(fileNameList[i]));
						if (progress != null)
							progress.Report((i + 1) * 100 / fileNameList.Length);
					}
				});
			}
		}

		public async Task ExportAsync(string baseDirectory, IEnumerable<ImageReference> images, IProgress<int> progress)
		{
			ThrowIfDisposed();
			using (_blocker.Enter())
			{
				var imageList = images.ToArray();
				Directory.CreateDirectory(baseDirectory);
				await Task.Run(() =>
				{
					for (int i = 0; i < imageList.Length; Interlocked.Increment(ref i))
					{
						lock (_lockObject)
						{
							using (var ms = GetBinaryImage(imageList[i]))
							{
								Bitmap bmp = new Bitmap(ms);
								File.WriteAllBytes(Path.Combine(baseDirectory, i.ToString(imageList.Length - 1) +
									Array.Find(System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders(),
									item => item.FormatID == bmp.RawFormat.Guid).FilenameExtension.Split(';')[0].Remove(0, 1)),
									ms.ToArray());
							}
						}
						if (progress != null)
							progress.Report((i + 1) * 100 / imageList.Length);
					}
				});
			}
		}

		public async Task ExtractAsync(string fileName, IEnumerable<ImageReference> images, IProgress<int> progress)
		{
			ThrowIfDisposed();
			using (_blocker.Enter())
				await Task.Run(() => WriteFile(images.ToArray(), FileHeader.Create(fileName, null, FileVersion, "", "", Author, null, PageTurningDirection), new BookmarkCollection(null, null), progress));
		}

		public IEnumerable<Spread> ConstructSpreads(bool simpleSpread)
		{
			ThrowIfDisposed();
			Spread spread = null;
			foreach (var image in Images)
			{
				if (simpleSpread || PageTurningDirection == PageTurningDirection.None || image.ViewMode == ImageViewMode.Default)
				{
					if (spread != null)
					{
						yield return spread;
						spread = null;
					}
					yield return new Spread(image);
				}
				else if (image.ViewMode == ImageViewMode.Right)
				{
					if (spread == null)
						spread = new Spread();
					spread.Right = image;
					if (PageTurningDirection == PageTurningDirection.ToLeft)
					{
						yield return spread;
						spread = null;
					}
				}
				else if (image.ViewMode == ImageViewMode.Left)
				{
					if (spread == null)
						spread = new Spread();
					spread.Left = image;
					if (PageTurningDirection == PageTurningDirection.ToRight)
					{
						yield return spread;
						spread = null;
					}
				}
			}
			if (spread != null)
				yield return spread;
		}

		MemoryStream GetBinaryImage(ImageReference ir)
		{
			MemoryStream result = null;
			MemoryStream temp = null;
			try
			{
				temp = new MemoryStream();
				var readStream = IsReadOnly ? _saved : _target;
				long pos = readStream.Position;
				if (pos != ir.Position)
					readStream.Seek(ir.Position, SeekOrigin.Begin);
				Crypto.Decrypt(readStream, temp, IsReadOnly ? this._password : "", System.Text.Encoding.Unicode, ir.Length);
				if (pos != ir.Position)
					readStream.Seek(pos, SeekOrigin.Begin);
				result = temp;
				temp = null;
			}
			finally
			{
				if (temp != null)
					temp.Dispose();
			}
			return result;
		}

		void ThrowIfReadOnly() { if (IsReadOnly) throw new InvalidOperationException(Properties.Resources.CannotModifyClassInReadOnlyMode); }

		protected void ThrowIfDisposed() { if (Images == null) throw new ObjectDisposedException(GetType().Name); }

		public bool IsBusy
		{
			get
			{
				ThrowIfDisposed();
				return _blocker.IsBusy;
			}
		}

		string _savedFilePath = "";
		bool _readOnly = false;
		bool _dirty = false;
		Version _fileVersion = AssemblyVersion;
		Image _thumbnail = null;
		string _title = "";
		string _author = "";
		DateTime? _dateOfPublication = null;
		PageTurningDirection _pageTurningDirection;

		public bool HasSaved
		{
			get
			{
				ThrowIfDisposed();
				return !string.IsNullOrEmpty(SavedFilePath);
			}
		}

		public string SavedFilePath
		{
			get
			{
				ThrowIfDisposed();
				return _savedFilePath;
			}
			private set
			{
				ThrowIfDisposed();
				if (_savedFilePath != value)
				{
					_savedFilePath = value;
					RaisePropertyChanged();
					RaisePropertyChanged(() => HasSaved);
				}
			}
		}

		public bool IsReadOnly
		{
			get
			{
				ThrowIfDisposed();
				return _readOnly;
			}
			private set
			{
				ThrowIfDisposed();
				if (_readOnly != value)
				{
					_readOnly = value;
					RaisePropertyChanged();
				}
			}
		}

		public bool IsDirty
		{
			get
			{
				ThrowIfDisposed();
				return _dirty;
			}
			private set
			{
				ThrowIfDisposed();
				if (_dirty != value)
				{
					_dirty = value;
					RaisePropertyChanged();
				}
			}
		}

		public Version FileVersion
		{
			get
			{
				ThrowIfDisposed();
				return _fileVersion;
			}
			private set
			{
				ThrowIfDisposed();
				if (_fileVersion != value)
				{
					_fileVersion = value;
					RaisePropertyChanged();
				}
			}
		}

		public Image Thumbnail
		{
			get
			{
				ThrowIfDisposed();
				return _thumbnail;
			}
			set
			{
				ThrowIfDisposed();
				if (_thumbnail != value)
				{
					_thumbnail = value;
					RaisePropertyChanged();
				}
			}
		}

		public string Title
		{
			get
			{
				ThrowIfDisposed();
				return _title;
			}
			set
			{
				ThrowIfDisposed();
				if (_title != value)
				{
					_title = value;
					RaisePropertyChanged();
				}
			}
		}

		public string Author
		{
			get
			{
				ThrowIfDisposed();
				return _author;
			}
			set
			{
				ThrowIfDisposed();
				if (_author != value)
				{
					_author = value;
					RaisePropertyChanged();
				}
			}
		}

		public DateTime? DateOfPublication
		{
			get
			{
				ThrowIfDisposed();
				return _dateOfPublication;
			}
			set
			{
				ThrowIfDisposed();
				if (_dateOfPublication != value)
				{
					_dateOfPublication = value;
					RaisePropertyChanged();
				}
			}
		}

		public PageTurningDirection PageTurningDirection
		{
			get
			{
				ThrowIfDisposed();
				return _pageTurningDirection;
			}
			set
			{
				ThrowIfDisposed();
				if (_pageTurningDirection != value)
				{
					_pageTurningDirection = value;
					RaisePropertyChanged();
				}
			}
		}

		public ImageReferenceCollection Images { get; private set; }

		public BookmarkCollection Bookmarks { get; private set; }
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
