using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Comical.Core
{
	public class Comic : IDisposable, INotifyPropertyChanged
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
		
		bool _canDirty = true;
		ImageReferenceCollection _images = new ImageReferenceCollection();
		BookmarkCollection _bookmarks = new BookmarkCollection();

		public ImageReferenceCollection Images { get { return _images; } }

		public BookmarkCollection Bookmarks { get { return _bookmarks; } }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				using (EnterUndirtiableSection())
				{
					if (_images != null)
					{
						_images.Clear();
						_images.Dispose();
						_images = null;
					}
					if (_bookmarks != null)
					{
						_bookmarks.Clear();
						_bookmarks.Dispose();
						_bookmarks = null;
					}
					Thumbnail = null;
					Title = Author = string.Empty;
					IsDirty = false;
				}
			}
		}

		async Task<FileHeader> ReadFileAsync(string fileName, BookmarkCollection bookmarks, IProgress<int> progress)
		{
			using (FileStream readStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			{
				var cic = FileHeader.Load(readStream);
				// ID確認
				if (cic == null)
					throw new ArgumentException(Properties.Resources.InvalidFileFormat);
				bookmarks.Load(readStream); // 目次
				await Images.LoadAsync(readStream, cic.FileVersion, progress).ConfigureAwait(false); // 画像
				return cic;
			}
		}

		static async Task WriteFileAsync(string fileName, IReadOnlyList<ImageReference> images, FileHeader cic, BookmarkCollection bookmarks, IProgress<int> progress)
		{
			using (FileStream writeStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
			{
				cic.Save(writeStream);
				bookmarks.Save(writeStream);
				using (BinaryWriter writer = new BinaryWriter(writeStream, System.Text.Encoding.Unicode, true))
				{
					writer.Write(images.Count); // 画像数
					for (int i = 0; i < images.Count; i++)
					{
						await images[i].SaveAsync(writer).ConfigureAwait(false);
						progress?.Report((i + 1) * 100 / images.Count);
					}
				}
			}
		}

		public void Clear()
		{
			using (EnterUndirtiableSection())
			{
				Thumbnail = null;
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

		public async Task OpenAsync(string fileName, IProgress<int> progress)
		{
			using (EnterSingleOperation())
			using (EnterUndirtiableSection())
			using (Images.EnterUnnotifiedSection())
			{
				Images.Clear();
				Bookmarks.Clear();
				IsDirty = false;
				var res = await ReadFileAsync(fileName, Bookmarks, progress).ConfigureAwait(false);
				Thumbnail = res.Thumbnail;
				FileVersion = res.FileVersion;
				Title = res.Title;
				Author = res.Author;
				DateOfPublication = res.DateOfPublication;
				PageTurningDirection = res.PageTurningDirection;
			}
		}

		public async Task SaveAsync(string fileName, IProgress<int> progress)
		{
			using (EnterSingleOperation())
			using (EnterUndirtiableSection())
			{
				var inconsistency = CheckInconsistency();
				if (inconsistency != ConsistencyValidatedDataTypes.None)
					throw new InconsistentDataException(Properties.Resources.InconsistentData, inconsistency);
				var header = new FileHeader(Title, Author, DateOfPublication, PageTurningDirection, Thumbnail);
				await WriteFileAsync(fileName, Images.ToArray(), header, Bookmarks, progress).ConfigureAwait(false);
				FileVersion = header.FileVersion;
				IsDirty = false;
			}
		}

		public async Task AppendAsync(string fileName, IProgress<int> progress)
		{
			using (EnterSingleOperation())
			using (Images.EnterUnnotifiedSection())
			using (var bookmarks = new BookmarkCollection())
				await ReadFileAsync(fileName, bookmarks, progress).ConfigureAwait(false);
		}

		public async Task ExportAsync(string baseDirectory, IEnumerable<ImageReference> images, Func<Stream, string> extensionProvider, IProgress<int> progress)
		{
			using (EnterSingleOperation())
			{
				var imageList = images.ToArray();
				Directory.CreateDirectory(baseDirectory);
				for (int i = 0; i < imageList.Length; Interlocked.Increment(ref i))
				{
					using (var ms = imageList[i].OpenImageStream())
					using (FileStream fs = new FileStream(Path.Combine(baseDirectory, i.ToString(System.Globalization.CultureInfo.CurrentCulture) + extensionProvider(ms)), FileMode.Create, FileAccess.Write))
						await ms.CopyToAsync(fs).ConfigureAwait(false);
					progress?.Report((i + 1) * 100 / imageList.Length);
				}
			}
		}

		public async Task ExtractAsync(string fileName, IEnumerable<ImageReference> images, IProgress<int> progress)
		{
			using (EnterSingleOperation())
			using (var bookmarks = new BookmarkCollection())
				await WriteFileAsync(fileName, images.ToArray(), new FileHeader(string.Empty, Author, null, PageTurningDirection, null), bookmarks, progress).ConfigureAwait(false);
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

		Version _fileVersion = FileHeader.LatestSupportedFileVersion;
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

		byte[] _thumbnail = null;
		public byte[] Thumbnail
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
