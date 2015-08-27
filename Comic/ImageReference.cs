using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using Comical.Infrastructures;

namespace Comical.Core
{
	public class ImageReference : INotifyPropertyChanged
	{
		public ImageReference(byte[] data)
		{
			if (data == null)
				throw new ArgumentNullException(nameof(data));
			_data = data;
		}

		byte[] _data;
		ImageViewMode _mode;

		public ImageViewMode ViewMode
		{
			get { return _mode; }
			set
			{
				if (value != _mode)
				{
					_mode = value;
					PropertyChanged.Raise(this);
				}
			}
		}
		
		public Stream OpenImageStream()
		{
			MemoryStream ms = null;
			MemoryStream temp = null;
			try
			{
				temp = new MemoryStream(_data, false);
				ms = temp;
				temp = null;
			}
			finally
			{
				if (temp != null)
					temp.Dispose();
			}
			return ms;
		}

		internal static async Task<ImageReference> LoadAsync(BinaryReader reader, string password, Version fileVersion)
		{
			if (fileVersion < new Version(4, 4))
				reader.ReadString(); // 名前
			if (fileVersion < new Version(4, 3))
				reader.ReadString(); // フォーマット
			ImageViewMode m = (ImageViewMode)reader.ReadByte(); // オプション
			int len = reader.ReadInt32(); // サイズ
			using (MemoryStream ms = new MemoryStream())
			{
				await Crypto.TransformAsync(reader.BaseStream, ms, password, System.Text.Encoding.Unicode, len, true).ConfigureAwait(false);
				return new ImageReference(ms.ToArray()) { ViewMode = m };
			}
		}

		internal async Task SaveAsync(BinaryWriter writer, string password)
		{
			writer.Write((byte)ViewMode); // 利用情報
			writer.Write(_data.Length); // 画像データ大きさ
			using (var binImage = OpenImageStream())
				await Crypto.TransformAsync(binImage, writer.BaseStream, password, System.Text.Encoding.Unicode, _data.Length, false).ConfigureAwait(false); // 画像データ
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class ImageReferenceCollection : SynchronizedObservableCollection<ImageReference>
	{
		bool _notificationSuspended = false;
		bool _collectionChanged = false;
		List<KeyValuePair<object, string>> _itemChanges = new List<KeyValuePair<object, string>>();
		
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

		internal async Task LoadAsync(Stream stream, string password, Version fileVersion, IProgress<int> progress)
		{
			using (BinaryReader reader = new BinaryReader(stream, System.Text.Encoding.Unicode, true))
			{
				int images = reader.ReadInt32(); // 画像数
				for (int i = 0; i < images; i++)
				{
					Add(await ImageReference.LoadAsync(reader, password, fileVersion).ConfigureAwait(false));
					progress?.Report((i + 1) * 100 / images);
				}
			}
		}
	}
}
