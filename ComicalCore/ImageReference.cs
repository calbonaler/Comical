using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace Comical.Core
{
	public class ImageReference : INotifyPropertyChanged
	{
		public ImageReference(byte[] data) => _data = data ?? throw new ArgumentNullException(nameof(data));

		readonly byte[] _data;
		ImageViewMode _mode;

		public ImageViewMode ViewMode
		{
			get => _mode;
			set => Utils.SetProperty(ref _mode, value, this, PropertyChanged);
		}

		public Stream OpenImageStream()
		{
			MemoryStream ms = null;
			try { ms = new MemoryStream(_data, false); }
			catch
			{
				ms?.Dispose();
				throw;
			}
			return ms;
		}

		internal static async Task<ImageReference> LoadAsync(BinaryReader reader, Version fileVersion)
		{
			if (fileVersion < new Version(4, 4))
				reader.ReadString(); // 名前
			if (fileVersion < new Version(4, 3))
				reader.ReadString(); // フォーマット
			var mode = (ImageViewMode)reader.ReadByte();
			var buffer = new byte[reader.ReadInt32()]; // サイズ
			await reader.BaseStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
			return new ImageReference(buffer) { ViewMode = mode };
		}

		internal async Task SaveAsync(BinaryWriter writer)
		{
			writer.Write((byte)ViewMode); // 利用情報
			writer.Write(_data.Length); // 画像データ大きさ
			await writer.BaseStream.WriteAsync(_data, 0, _data.Length).ConfigureAwait(false); // 画像データ
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class ImageReferenceCollection : SynchronizedObservableCollection<ImageReference>
	{
		bool _notificationSuspended = false;
		bool _collectionChanged = false;
		readonly List<KeyValuePair<object, string>> _itemChanges = new List<KeyValuePair<object, string>>();

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (_notificationSuspended)
				_collectionChanged = true;
			else
				base.OnCollectionChanged(e);
		}

		protected override void OnCollectionItemPropertyChanged(CollectionItemPropertyChangedEventArgs e)
		{
			if (!_notificationSuspended)
			{
				base.OnCollectionItemPropertyChanged(e);
				return;
			}
			if (e == null)
				return;
			foreach (var propertyNames in e.PropertyNames)
			{
				foreach (var propertyName in propertyNames)
					_itemChanges.Add(new KeyValuePair<object, string>(propertyNames.Key, propertyName));
			}
		}

		public IDisposable EnterUnnotifiedSection()
		{
			_notificationSuspended = true;
			return new DelegateDisposable(() =>
			{
				_notificationSuspended = false;
				if (_collectionChanged)
					OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				_collectionChanged = false;
				if (_itemChanges.Count > 0)
				{
					OnCollectionItemPropertyChanged(new CollectionItemPropertyChangedEventArgs(_itemChanges));
					_itemChanges.Clear();
				}
			});
		}

		internal async Task LoadAsync(BinaryReader reader, Version fileVersion, IProgress<int> progress)
		{
			var imageCount = reader.ReadInt32();
			for (var i = 0; i < imageCount; i++)
			{
				Add(await ImageReference.LoadAsync(reader, fileVersion).ConfigureAwait(false));
				progress?.Report((i + 1) * 100 / imageCount);
			}
		}

		internal static async Task SaveAsync(IReadOnlyList<ImageReference> images, BinaryWriter writer, IProgress<int> progress)
		{
			writer.Write(images.Count);
			for (var i = 0; i < images.Count; i++)
			{
				await images[i].SaveAsync(writer).ConfigureAwait(false);
				progress?.Report((i + 1) * 100 / images.Count);
			}
		}
	}
}
