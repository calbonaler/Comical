using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Comical.Core
{
	public class Comic : IDisposable, INotifyPropertyChanged
	{
		public Comic()
		{
			Images.CollectionChanged += OnCollectionsChanged;
			Images.CollectionItemPropertyChanged += OnCollectionsChanged;
			Bookmarks.CollectionChanged += OnCollectionsChanged;
			Bookmarks.CollectionItemPropertyChanged += OnCollectionsChanged;
			PropertyChanged += OnSelfPropertyChanged;
		}

		public ImageReferenceCollection Images { get; private set; } = [];

		public BookmarkCollection Bookmarks { get; private set; } = [];

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				PropertyChanged -= OnSelfPropertyChanged;
				Bookmarks.CollectionItemPropertyChanged -= OnCollectionsChanged;
				Bookmarks.CollectionChanged -= OnCollectionsChanged;
				Images.CollectionItemPropertyChanged -= OnCollectionsChanged;
				Images.CollectionChanged -= OnCollectionsChanged;
				if (!Images.IsDisposed)
				{
					Images.Clear();
					Images.Dispose();
				}
				if (!Bookmarks.IsDisposed)
				{
					Bookmarks.Clear();
					Bookmarks.Dispose();
				}
				Thumbnail = null;
				Title = Author = string.Empty;
				IsDirty = false;
			}
		}

		async Task<FileHeader> ReadFileAsync(string fileName, BookmarkCollection bookmarks, IProgress<int> progress)
		{
			using var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			var fileHeader = await FileHeader.LoadAsync(stream).ConfigureAwait(false) ?? throw new ArgumentException(Properties.Resources.InvalidFileFormat);
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

		public void Clear(BindingSide bindingSide)
		{
			Thumbnail = null;
			Published = null;
			Title = Author = "";
			BindingSide = bindingSide;
			Images.Clear();
			Bookmarks.Clear();
			IsDirty = false;
		}

		bool IsInconsistent()
		{
			for (var i = 0; i < Bookmarks.Count; i++)
			{
				if (Bookmarks[i].Target < 0 || Bookmarks[i].Target >= Images.Count)
					return true;
			}
			return false;
		}

		public async Task OpenAsync(string fileName, IProgress<int> progress)
		{
			using (EnterSingleOperation())
			{
				Images.Clear();
				Bookmarks.Clear();
				var res = await ReadFileAsync(fileName, Bookmarks, progress).ConfigureAwait(false);
				Thumbnail = res.Thumbnail;
				FileVersion = res.FileVersion;
				Title = res.Title;
				Author = res.Author;
				Published = res.Published;
				BindingSide = res.BindingSide;
				IsDirty = false;
			}
		}

		public async Task SaveAsync(string fileName, IProgress<int> progress)
		{
			using (EnterSingleOperation())
			{
				if (IsInconsistent())
					throw new InconsistentDataException(Properties.Resources.InconsistentData);
				var header = new FileHeader(Title, Author, Published, BindingSide, Thumbnail);
				await WriteFileAsync(fileName, header, Images.ToArray(), Bookmarks, progress).ConfigureAwait(false);
				FileVersion = header.FileVersion;
				IsDirty = false;
			}
		}

		public async Task AppendAsync(string fileName, IProgress<int> progress)
		{
			using (EnterSingleOperation())
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
				await WriteFileAsync(fileName, new FileHeader(string.Empty, Author, null, BindingSide, null), images.ToArray(), [], progress).ConfigureAwait(false);
		}

		public IEnumerable<Spread> ConstructSpreads()
		{
			var pages = new ImageReference?[2];
			Spread Flush()
			{
				var spread = new Spread(pages[0], pages[1]);
				pages[0] = pages[1] = null;
				return spread;
			}
			foreach (var image in Images)
			{
				if (BindingSide == BindingSide.Default || image.ViewMode == ImageViewMode.Default)
				{
					if (pages.Any(x => x != null))
						yield return Flush();
					yield return new Spread(image);
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

		protected IDisposable EnterSingleOperation()
		{
			if (IsBusy)
				throw new InvalidOperationException(Properties.Resources.MultiAsyncOperationIsNotSupported);
			IsBusy = true;
			return new DelegateDisposable(() => IsBusy = false);
		}

		void OnCollectionsChanged(object? sender, EventArgs e) => IsDirty = true;

		void OnSelfPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(Thumbnail) or nameof(Title) or nameof(Author) or nameof(Published) or nameof(BindingSide))
				IsDirty = true;
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
			private set => Utils.SetProperty(ref _dirty, value, this, PropertyChanged);
		}

		Version _fileVersion = FileHeader.LatestSupportedFileVersion;
		public Version FileVersion
		{
			get => _fileVersion;
			private set => Utils.SetProperty(ref _fileVersion, value, this, PropertyChanged);
		}

		Binary? _thumbnail = null;
		public Binary? Thumbnail
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

		public event PropertyChangedEventHandler? PropertyChanged;
	}

	public class Spread
	{
		public Spread(ImageReference? left, ImageReference? right)
		{
			_fillSpread = false;
			Left = left;
			Right = right;
		}

		public Spread(ImageReference fill)
		{
			_fillSpread = true;
			Left = fill;
			Right = null;
		}

		readonly bool _fillSpread;

		public ImageReference? Fill => _fillSpread ? Left : null;

		public ImageReference? Left { get; }

		public ImageReference? Right { get; }
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

	public class InconsistentDataException : Exception
	{
		public InconsistentDataException() { }
		public InconsistentDataException(string message) : base(message) { }
		public InconsistentDataException(string message, Exception inner) : base(message, inner) { }
	}
}
