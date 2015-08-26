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
using Comical.Infrastructures;

namespace Comical.Core
{
	public class Comic : INotifyPropertyChanged
	{
		public Comic()
		{
			Images.CollectionChanged += (s, ev) => IsDirty = true;
			Images.CollectionItemPropertyChanged += (s, ev) => IsDirty = true;
			Bookmarks.CollectionChanged += (s, ev) => IsDirty = true;
			Bookmarks.CollectionItemPropertyChanged += (s, ev) => IsDirty = true;
			PropertyChanged += (s, ev) =>
			{
				if (ev.PropertyName == nameof(Thumbnail) ||
					ev.PropertyName == nameof(Title) ||
					ev.PropertyName == nameof(Author) ||
					ev.PropertyName == nameof(DateOfPublication) ||
					ev.PropertyName == nameof(PageTurningDirection))
					IsDirty = true;
			};
		}
		
		string _password = "";
		bool _canDirty = true;
		static readonly Version AssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
		public static readonly string DefaultPassword = null;

		public ImageReferenceCollection Images { get; } = new ImageReferenceCollection();

		public BookmarkCollection Bookmarks { get; } = new BookmarkCollection();

		async Task<FileHeader> ReadFileAsync(string fileName, string password, BookmarkCollection bookmarks, IProgress<int> progress)
		{
			using (FileStream readStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				var cic = new FileHeader(readStream);
				// ID確認
				if (!cic.CanOpen)
					throw new ArgumentException(Properties.Resources.InvalidFileFormat);
				if (cic.FileVersion > AssemblyVersion)
					throw new ArgumentException(Properties.Resources.UnsupportedFileVersion);
				// 復号化済みマーク
				if (!cic.IsProperPassword(password))
					throw new WrongPasswordException(Properties.Resources.PasswordIsNotProper);
				bookmarks.Load(readStream); // 目次
				using (BinaryReader reader = new BinaryReader(readStream, System.Text.Encoding.Unicode, true))
				{
					int images = reader.ReadInt32(); // 画像数
					for (int i = 0; i < images; i++)
					{
						if (cic.FileVersion < new Version(4, 4))
							reader.ReadString(); // 名前
						if (cic.FileVersion < new Version(4, 3))
							reader.ReadString(); // フォーマット
						ImageViewMode m = (ImageViewMode)reader.ReadByte(); // オプション
						int len = reader.ReadInt32(); // サイズ
						using (MemoryStream ms = new MemoryStream())
						{
							await Crypto.TransformAsync(readStream, ms, password, System.Text.Encoding.Unicode, len, true).ConfigureAwait(false);
							Images.Add(new ImageReference(ms.ToArray()) { ViewMode = m });
						}
						progress?.Report((i + 1) * 100 / images);
					}
				}
				return cic;
			}
		}

		static async Task WriteFileAsync(IReadOnlyList<ImageReference> images, FileHeader cic, BookmarkCollection bookmarks, IProgress<int> progress)
		{
			using (FileStream writeStream = new FileStream(cic.Path, FileMode.Create, FileAccess.Write))
			{
				cic.SaveInto(writeStream);
				bookmarks.SaveInto(writeStream);
				using (BinaryWriter writer = new BinaryWriter(writeStream, System.Text.Encoding.Unicode, true))
				{
					writer.Write(images.Count); // 画像数
					for (int i = 0; i < images.Count; i++)
					{
						ImageReference ir = images[i];
						writer.Write((byte)ir.ViewMode); // 利用情報
						writer.Write(ir.Length); // 画像データ大きさ
						using (var binImage = ir.GetReadOnlyBinaryImage())
							await Crypto.TransformAsync(binImage, writeStream, cic.Password, System.Text.Encoding.Unicode, ir.Length, false).ConfigureAwait(false); // 画像データ
						progress?.Report((i + 1) * 100 / images.Count);
					}
				}
			}
		}

		public void Clear()
		{
			using (EnterUndirtiableSection())
			{
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
		}

		public ConsistencyValidatedDataTypes CheckInconsistency()
		{
			for (int i = 0; i < Bookmarks.Count; i++)
			{
				if (Bookmarks[i].Target < Images.Count && Bookmarks[i].Target >= 0)
					continue;
				return ConsistencyValidatedDataTypes.Images | ConsistencyValidatedDataTypes.Bookmarks;
			}
			return ConsistencyValidatedDataTypes.None;
		}

		public async Task OpenAsync(string fileName, string password, IProgress<int> progress)
		{
			using (EnterSingleOperation())
			using (EnterUndirtiableSection())
			using (Images.EnterUnnotifiedSection())
			{
				Images.Clear();
				Bookmarks.Clear();
				IsDirty = false;
				var res = await ReadFileAsync(fileName, password, Bookmarks, progress).ConfigureAwait(false);
				_password = password;
				if (Thumbnail != null)
					Thumbnail.Dispose();
				Thumbnail = res.Thumbnail;
				FileVersion = res.FileVersion;
				Title = res.Title;
				Author = res.Author;
				DateOfPublication = res.DateOfPublication;
				PageTurningDirection = res.PageTurningDirection;
			}
		}

		public async Task SaveAsync(string fileName, string password, IProgress<int> progress)
		{
			using (EnterSingleOperation())
			using (EnterUndirtiableSection())
			{
				var inconsistency = CheckInconsistency();
				if (inconsistency != ConsistencyValidatedDataTypes.None)
					throw new InconsistentDataException(Properties.Resources.InconsistentData, inconsistency);
				if (password == DefaultPassword)
					password = _password;
				await WriteFileAsync(Images.ToArray(), new FileHeader(fileName, password, AssemblyVersion)
				{
					Thumbnail = Thumbnail,
					Title = Title,
					Author = Author,
					DateOfPublication = DateOfPublication,
					PageTurningDirection = PageTurningDirection
				}, Bookmarks, progress).ConfigureAwait(false);
				FileVersion = AssemblyVersion;
				_password = password;
				IsDirty = false;
			}
		}

		public async Task AppendAsync(string fileName, string password, IProgress<int> progress)
		{
			using (EnterSingleOperation())
			using (Images.EnterUnnotifiedSection())
				await ReadFileAsync(fileName, password, new BookmarkCollection(), progress).ConfigureAwait(false);
		}

		public async Task ExportAsync(string baseDirectory, IEnumerable<ImageReference> images, IProgress<int> progress)
		{
			using (EnterSingleOperation())
			{
				var imageList = images.ToArray();
				Directory.CreateDirectory(baseDirectory);
				for (int i = 0; i < imageList.Length; Interlocked.Increment(ref i))
				{
					using (var ms = imageList[i].GetReadOnlyBinaryImage())
					{
						string ext;
						using (Bitmap bmp = new Bitmap(ms))
							ext = Array.Find(System.Drawing.Imaging.ImageCodecInfo.GetImageDecoders(), item => item.FormatID == bmp.RawFormat.Guid).FilenameExtension.Split(';')[0].Remove(0, 1);
						using (FileStream fs = new FileStream(Path.Combine(baseDirectory, i.ToString(System.Globalization.CultureInfo.CurrentCulture) + ext), FileMode.Create, FileAccess.Write))
							await ms.CopyToAsync(fs).ConfigureAwait(false);
					}
					progress?.Report((i + 1) * 100 / imageList.Length);
				}
			}
		}

		public async Task ExtractAsync(string fileName, IEnumerable<ImageReference> images, IProgress<int> progress)
		{
			using (EnterSingleOperation())
				await WriteFileAsync(images.ToArray(), new FileHeader(fileName, "", AssemblyVersion) { Author = Author, PageTurningDirection = PageTurningDirection }, new BookmarkCollection(), progress).ConfigureAwait(false);
		}

		public IEnumerable<Spread> ConstructSpreads(bool simpleSpread)
		{
			int? left = null;
			int? right = null;
			for (int i = 0; i < Images.Count; i++)
			{
				if (simpleSpread || PageTurningDirection == PageTurningDirection.None || Images[i].ViewMode == ImageViewMode.Default)
				{
					if (left != null || right != null)
					{
						yield return new Spread(left, right, false);
						left = right = null;
					}
					yield return new Spread(i, null, true);
				}
				else
				{
					if (left != null && Images[i].ViewMode == ImageViewMode.Left || right != null && Images[i].ViewMode == ImageViewMode.Right)
					{
						yield return new Spread(left, right, false);
						left = right = null;
					}
					if (Images[i].ViewMode == ImageViewMode.Left)
						left = i;
					else
						right = i;
					if (PageTurningDirection == (PageTurningDirection)(3 - Images[i].ViewMode))
					{
						yield return new Spread(left, right, false);
						left = right = null;
					}
				}
			}
			if (left != null || right != null)
				yield return new Spread(left, right, false);
		}
		
		public IDisposable EnterUndirtiableSection()
		{
			if (!_canDirty)
				return new DelegateDisposable(() => { });
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
					PropertyChanged.Raise(this);
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
					PropertyChanged.Raise(this);
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
					PropertyChanged.Raise(this);
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
					PropertyChanged.Raise(this);
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
					PropertyChanged.Raise(this);
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
					PropertyChanged.Raise(this);
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
					PropertyChanged.Raise(this);
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
					PropertyChanged.Raise(this);
				}
			}
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class Spread
	{
		public Spread(int? left, int? right, bool fillSpread)
		{
			Left = left;
			Right = right;
			FillSpread = fillSpread;
		}

		public int? Left { get; }

		public int? Right { get; }

		public bool FillSpread { get; }
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

	[Flags]
	public enum ConsistencyValidatedDataTypes
	{
		None = 0,
		Images = 1,
		Bookmarks = 2,
	}

	[Serializable]
	public class WrongPasswordException : Exception
	{
		public WrongPasswordException() { }
		public WrongPasswordException(string message) : base(message) { }
		public WrongPasswordException(string message, Exception innerException) : base(message, innerException) { }
		protected WrongPasswordException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	[Serializable]
	public class InconsistentDataException : Exception
	{
		public InconsistentDataException() : this(ConsistencyValidatedDataTypes.None) { }
		public InconsistentDataException(ConsistencyValidatedDataTypes dataTypes) { DataTypes = dataTypes; }
		public InconsistentDataException(string message) : this(message, ConsistencyValidatedDataTypes.None) { }
		public InconsistentDataException(string message, ConsistencyValidatedDataTypes dataTypes) : base(message) { DataTypes = dataTypes; }
		public InconsistentDataException(string message, Exception inner) : base(message, inner) { }
		protected InconsistentDataException(SerializationInfo info, StreamingContext context) : base(info, context) { DataTypes = (ConsistencyValidatedDataTypes)info.GetInt32(nameof(DataTypes)); }
		public ConsistencyValidatedDataTypes DataTypes { get; private set; }
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue(nameof(DataTypes), (int)DataTypes);
		}
	}
}
