using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
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

		public int Length => _data.Length;

		public Image GetImage() => GetImage(Size.Empty);

		public Image GetImage(Size size)
		{
			using (var ms = new MemoryStream(_data, false))
			using (var image = Image.FromStream(ms))
			{
				Bitmap bitmap = null;
				Bitmap tmp = null;
				try
				{
					if (size.IsEmpty)
						tmp = new Bitmap(image);
					else
					{
						var x = Math.Min(image.Width * size.Height, size.Width * image.Height);
						tmp = new Bitmap(image, x / image.Height, x / image.Width);
					}
					bitmap = tmp;
					tmp = null;
				}
				finally
				{
					if (tmp != null)
						tmp.Dispose();
				}
				return bitmap;
			}
		}

		internal MemoryStream GetReadOnlyBinaryImage()
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
	}
}
