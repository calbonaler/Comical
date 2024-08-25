using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace Comical.Core
{
	public class ImageReference(Binary data) : INotifyPropertyChanged
	{
		ImageViewMode _mode;

		public Binary Data { get; } = data ?? throw new ArgumentNullException(nameof(data));

		public ImageViewMode ViewMode
		{
			get => _mode;
			set => Utils.SetProperty(ref _mode, value, this, PropertyChanged);
		}

		internal static async Task<ImageReference> LoadAsync(BinaryReader reader, Version fileVersion)
		{
			if (fileVersion < new Version(4, 4))
				reader.ReadString(); // 名前
			if (fileVersion < new Version(4, 3))
				reader.ReadString(); // フォーマット
			var mode = (ImageViewMode)reader.ReadByte();
			var buffer = new byte[reader.ReadInt32()]; // サイズ
			await reader.BaseStream.ReadExactlyNoThrowAsync(buffer).ConfigureAwait(false);
			return new ImageReference(new Binary(buffer)) { ViewMode = mode };
		}

		internal async Task SaveAsync(BinaryWriter writer)
		{
			writer.Write((byte)ViewMode); // 利用情報
			writer.Write(Data.Length); // 画像データ大きさ
			await Data.WriteToAsync(writer.BaseStream).ConfigureAwait(false); // 画像データ
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class ImageReferenceCollection : SynchronizedObservableCollection<ImageReference>
	{
		bool _notificationSuspended = false;
		bool _collectionChanged = false;
		readonly List<KeyValuePair<object, string>> _itemChanges = [];

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
