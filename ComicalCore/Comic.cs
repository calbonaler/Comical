using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
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
					ev.PropertyName == nameof(Published) ||
					ev.PropertyName == nameof(BindingSide))
					IsDirty = true;
			};
		}

		bool _canDirty = true;

		public ImageReferenceCollection Images { get; private set; } = new ImageReferenceCollection();

		public BookmarkCollection Bookmarks { get; private set; } = new BookmarkCollection();

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
					if (Images != null)
					{
						Images.Clear();
						Images.Dispose();
						Images = null;
					}
					if (Bookmarks != null)
					{
						Bookmarks.Clear();
						Bookmarks.Dispose();
						Bookmarks = null;
					}
					Thumbnail = null;
					Title = Author = string.Empty;
					IsDirty = false;
				}
			}
		}

		async Task<FileHeader> ReadFileAsync(string fileName, BookmarkCollection bookmarks, IProgress<int> progress)
		{
			using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			var fileHeader = await FileHeader.LoadAsync(stream).ConfigureAwait(false);
			// ID確認
			if (fileHeader == null)
				throw new ArgumentException(Properties.Resources.InvalidFileFormat);
			using (var reader = new BinaryReader(stream, Encoding.Unicode, true))
			{
				bookmarks.Load(reader); // 目次
				await Images.LoadAsync(reader, fileHeader.FileVersion, progress).ConfigureAwait(false); // 画像
			}
			return fileHeader;
		}

		static async Task WriteFileAsync(string fileName, FileHeader fileHeader, IReadOnlyList<ImageReference> images, IReadOnlyList<Bookmark> bookmarks, IProgress<int> progress)
		{
			using var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
			await fileHeader.SaveAsync(stream).ConfigureAwait(false);
			using var writer = new BinaryWriter(stream, Encoding.Unicode, true);
			BookmarkCollection.Save(bookmarks, writer);
			await ImageReferenceCollection.SaveAsync(images, writer, progress).ConfigureAwait(false);
		}

		public void Clear()
		{
			using (EnterUndirtiableSection())
			{
				Thumbnail = null;
				Published = null;
				Title = Author = "";
				Images.Clear();
				Bookmarks.Clear();
				IsDirty = false;
			}
		}

		public ConsistencyValidatedDataTypes CheckInconsistency()
		{
			for (var i = 0; i < Bookmarks.Count; i++)
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
				Published = res.Published;
				BindingSide = res.BindingSide;
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
				var header = new FileHeader(Title, Author, Published, BindingSide, Thumbnail);
				await WriteFileAsync(fileName, header, Images.ToArray(), Bookmarks, progress).ConfigureAwait(false);
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

		public async Task ExportAsync(string baseDirectory, IEnumerable<ImageReference> images, Func<Binary, string> extensionProvider, IProgress<int> progress)
		{
			using (EnterSingleOperation())
			{
				var imageList = images.ToArray();
				Directory.CreateDirectory(baseDirectory);
				for (var i = 0; i < imageList.Length; Interlocked.Increment(ref i))
				{
					using (var fs = new FileStream(Path.Combine(baseDirectory, i.ToString(CultureInfo.CurrentCulture) + extensionProvider(imageList[i].Data)), FileMode.Create, FileAccess.Write))
						await imageList[i].Data.WriteToAsync(fs).ConfigureAwait(false);
					progress?.Report((i + 1) * 100 / imageList.Length);
				}
			}
		}

		public async Task ExtractAsync(string fileName, IEnumerable<ImageReference> images, IProgress<int> progress)
		{
			using (EnterSingleOperation())
				await WriteFileAsync(fileName, new FileHeader(string.Empty, Author, null, BindingSide, null), images.ToArray(), Array.Empty<Bookmark>(), progress).ConfigureAwait(false);
		}

		public IEnumerable<Spread> ConstructSpreads()
		{
			var pages = new ImageReference[2];
			Spread Flush()
			{
				var spread = new Spread(pages[0], pages[1], false);
				pages[0] = pages[1] = null;
				return spread;
			}
			foreach (var image in Images)
			{
				if (BindingSide == BindingSide.Default || image.ViewMode == ImageViewMode.Default)
				{
					if (pages.Any(x => x != null))
						yield return Flush();
					yield return new Spread(image, null, true);
				}
				else
				{
					if (pages[(int)image.ViewMode - 1] != null)
						yield return Flush();
					pages[(int)image.ViewMode - 1] = image;
					if (BindingSide == (BindingSide)(3 - (int)image.ViewMode))
						yield return Flush();
				}
			}
			if (pages.Any(x => x != null))
				yield return Flush();
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
			get => _busy;
			private set => Utils.SetProperty(ref _busy, value, this, PropertyChanged);
		}

		bool _dirty = false;
		public bool IsDirty
		{
			get => _dirty;
			private set
			{
				if (_canDirty || !value)
					Utils.SetProperty(ref _dirty, value, this, PropertyChanged);
			}
		}

		Version _fileVersion = FileHeader.LatestSupportedFileVersion;
		public Version FileVersion
		{
			get => _fileVersion;
			private set => Utils.SetProperty(ref _fileVersion, value, this, PropertyChanged);
		}

		Binary _thumbnail = null;
		public Binary Thumbnail
		{
			get => _thumbnail;
			set => Utils.SetProperty(ref _thumbnail, value, this, PropertyChanged);
		}

		string _title = "";
		public string Title
		{
			get => _title;
			set => Utils.SetProperty(ref _title, value, this, PropertyChanged);
		}

		string _author = "";
		public string Author
		{
			get => _author;
			set => Utils.SetProperty(ref _author, value, this, PropertyChanged);
		}

		DateTime? _published = null;
		public DateTime? Published
		{
			get => _published;
			set => Utils.SetProperty(ref _published, value, this, PropertyChanged);
		}

		BindingSide _bindingSide;
		public BindingSide BindingSide
		{
			get => _bindingSide;
			set => Utils.SetProperty(ref _bindingSide, value, this, PropertyChanged);
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class Spread
	{
		public Spread(ImageReference left, ImageReference right, bool fillSpread)
		{
			Left = left;
			Right = right;
			IsFillSpread = fillSpread;
		}

		public ImageReference Left { get; }

		public ImageReference Right { get; }

		public bool IsFillSpread { get; }
	}

	public enum ImageViewMode
	{
		Default = 0,
		Left = 1,
		Right = 2,
	}

	public enum BindingSide
	{
		Default = 0,
		Left = 1,
		Right = 2,
	}

	[Flags]
	public enum ConsistencyValidatedDataTypes
	{
		None = 0,
		Images = 1,
		Bookmarks = 2,
	}

	public class InconsistentDataException : Exception
	{
		public InconsistentDataException() : this(ConsistencyValidatedDataTypes.None) { }
		public InconsistentDataException(ConsistencyValidatedDataTypes dataTypes) => DataTypes = dataTypes;
		public InconsistentDataException(string message) : this(message, ConsistencyValidatedDataTypes.None) { }
		public InconsistentDataException(string message, ConsistencyValidatedDataTypes dataTypes) : base(message) => DataTypes = dataTypes;
		public InconsistentDataException(string message, Exception inner) : base(message, inner) { }
		public ConsistencyValidatedDataTypes DataTypes { get; private set; }
	}
}
