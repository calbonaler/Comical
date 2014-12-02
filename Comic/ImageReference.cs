using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using CommonLibrary;

namespace Comical.Core
{
	public class ImageReference : INotifyPropertyChanged
	{
		internal ImageReference(byte[] data, ImageViewMode mode, SynchronizationContext ownerContext)
		{
			_data = data;
			_mode = mode;
			_ownerContext = ownerContext;
		}

		byte[] _data;
		SynchronizationContext _ownerContext;
		ImageViewMode _mode;

		public ImageViewMode ViewMode
		{
			get { return _mode; }
			set
			{
				if (value != _mode)
				{
					_mode = value;
					RaisePropertyChanged("ViewMode");
				}
			}
		}

		public int Length { get { return _data.Length; } }

		public Image GetImage() { return GetImage(Size.Empty); }

		public Image GetImage(Size size)
		{
			using (var ms = new MemoryStream(_data))
			using (var image = Image.FromStream(ms))
				return image.Resize(size);
		}

		internal MemoryStream GetBinaryImageNoLock()
		{
			MemoryStream ms = null;
			MemoryStream temp = null;
			try
			{
				temp = new MemoryStream(_data);
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

		internal void ReleaseContext() { _ownerContext = null; }

		protected void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				_ownerContext.SendIfNeeded(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class ImageReferenceCollection : IReadOnlyList<ImageReference>, INotifyCollectionChanged
	{
		List<ImageReference> references = new List<ImageReference>();
		readonly object lockObject = new object();
		bool _notificationSuspended = false;
		List<NotifyCollectionChangedEventArgs> collectionChanges = new List<NotifyCollectionChangedEventArgs>();
		Dictionary<object, string> itemChanges = new Dictionary<object, string>();

		protected void ClearItems()
		{
			lock (lockObject)
			{
				for (int i = references.Count - 1; i >= 0; i--)
				{
					references[i].PropertyChanged -= OnItemPropertyChanged;
					references[i].ReleaseContext();
					references.RemoveAt(i);
				}
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		protected void InsertItem(int index, ImageReference item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			item.PropertyChanged += OnItemPropertyChanged;
			references.Insert(index, item);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
		}

		protected void MoveItems(int oldIndex, int count, int newIndex)
		{
			ImageReference[] refes;
			lock (lockObject)
			{
				refes = new ImageReference[count];
				for (int i = 0; i < count; i++)
					refes[i] = references[oldIndex + i];
				references.RemoveRange(oldIndex, count);
				references.InsertRange(newIndex, refes);
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, refes, newIndex, oldIndex));
		}

		protected void RemoveItem(int index)
		{
			var item = this[index];
			references.RemoveAt(index);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
		}

		public void Move(int oldIndex, int newIndex) { MoveItems(oldIndex, 1, newIndex); }

		public void MoveRange(int oldIndex, int count, int newIndex) { MoveItems(oldIndex, count, newIndex); }

		public int IndexOf(ImageReference item) { return references.IndexOf(item); }

		public void RemoveAt(int index) { RemoveItem(index); }

		public ImageReference this[int index] { get { return references[index]; } }

		internal void Add(ImageReference item) { InsertItem(Count, item); }
		
		internal void Clear() { ClearItems(); }

		public bool Contains(ImageReference item) { return references.Contains(item); }

		public int Count { get { return references.Count; } }

		public bool Remove(ImageReference item)
		{
			RemoveItem(IndexOf(item));
			return true;
		}

		public IEnumerator<ImageReference> GetEnumerator() { return references.GetEnumerator(); }

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null)
			{
				if (_notificationSuspended)
					collectionChanges.Add(e);
				else
					CollectionChanged(this, e);
			}
		}

		protected virtual void OnCollectionItemPropertyChanged(CompositePropertyChangedEventArgs e)
		{
			if (CollectionItemPropertyChanged != null)
				CollectionItemPropertyChanged(this, e);
		}

		void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_notificationSuspended)
				itemChanges.Add(sender, e.PropertyName);
			else
				OnCollectionItemPropertyChanged(new CompositePropertyChangedEventArgs(new Dictionary<object, string>() { { sender, e.PropertyName } }));
		}

		public IDisposable SuspendNotification()
		{
			_notificationSuspended = true;
			return new Resumer(this);
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public event EventHandler<CompositePropertyChangedEventArgs> CollectionItemPropertyChanged;

		class Resumer : IDisposable
		{
			public Resumer(ImageReferenceCollection owner) { _owner = owner; }

			ImageReferenceCollection _owner;

			public void Dispose()
			{
				if (_owner != null)
				{
					_owner._notificationSuspended = false;
					if (_owner.collectionChanges.Any(x => x.Action == NotifyCollectionChangedAction.Reset) || _owner.collectionChanges.Count > 20)
						_owner.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
					else if (_owner.collectionChanges.Count > 0)
						foreach (var ch in _owner.collectionChanges)
							_owner.OnCollectionChanged(ch);
					_owner.collectionChanges.Clear();
					if (_owner.itemChanges.Count > 0)
						_owner.OnCollectionItemPropertyChanged(new CompositePropertyChangedEventArgs(_owner.itemChanges));
					_owner.itemChanges.Clear();
					_owner = null;
				}
			}
		}
	}
}
