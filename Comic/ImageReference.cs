using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

namespace Comical.Core
{
	public class ImageReference : INotifyPropertyChanged
	{
		public ImageReference(byte[] data)
		{
			if (data == null)
				throw new ArgumentNullException("data");
			_data = data;
		}

		byte[] _data;
		internal SynchronizationContext OwnerContext;
		ImageViewMode _mode;

		public ImageViewMode ViewMode
		{
			get { return _mode; }
			set
			{
				if (value != _mode)
				{
					_mode = value;
					PropertyChanged.Raise(this, OwnerContext);
				}
			}
		}

		public int Length { get { return _data.Length; } }

		public Image GetImage() { return GetImage(Size.Empty); }

		public Image GetImage(Size size)
		{
			using (var ms = new MemoryStream(_data, false))
			using (var image = Image.FromStream(ms))
				return image.Resize(size);
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

	public class ImageReferenceCollection : IList<ImageReference>, IReadOnlyList<ImageReference>, INotifyCollectionChanged
	{
		public ImageReferenceCollection(SynchronizationContext context) { _ownerContext = context; }

		SynchronizationContext _ownerContext;
		List<ImageReference> _references = new List<ImageReference>();
		bool _notificationSuspended = false;
		bool _collectionChanged = false;
		List<KeyValuePair<ImageReference, string>> _itemChanges = new List<KeyValuePair<ImageReference, string>>();

		protected void ClearItems()
		{
			lock (_references)
			{
				for (int i = _references.Count - 1; i >= 0; i--)
				{
					_references[i].PropertyChanged -= OnItemPropertyChanged;
					_references[i].OwnerContext = null;
					_references.RemoveAt(i);
				}
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		protected void InsertItem(int index, ImageReference item)
		{
			lock (_references)
			{
				if (item == null)
					throw new ArgumentNullException("item");
				item.PropertyChanged += OnItemPropertyChanged;
				item.OwnerContext = _ownerContext;
				_references.Insert(index, item);
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
		}

		protected void MoveItems(int oldIndex, int count, int newIndex)
		{
			ImageReference[] refes;
			lock (_references)
			{
				refes = new ImageReference[count];
				for (int i = 0; i < count; i++)
					refes[i] = _references[oldIndex + i];
				_references.RemoveRange(oldIndex, count);
				_references.InsertRange(newIndex, refes);
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, refes, newIndex, oldIndex));
		}

		protected void SetItem(int index, ImageReference item)
		{
			ImageReference oldItem;
			lock (_references)
			{
				if (item == null)
					throw new ArgumentNullException("item");
				oldItem = _references[index];
				oldItem.PropertyChanged -= OnItemPropertyChanged;
				oldItem.OwnerContext = null;
				_references[index] = item;
				item.PropertyChanged += OnItemPropertyChanged;
				item.OwnerContext = _ownerContext;
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, index));
		}

		protected bool RemoveItem(int index)
		{
			ImageReference item;
			lock (_references)
			{
				if (index < 0 || index >= _references.Count)
					return false;
				item = _references[index];
				item.PropertyChanged -= OnItemPropertyChanged;
				item.OwnerContext = null;
				_references.RemoveAt(index);
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
			return true;
		}

		public void Move(int oldIndex, int newIndex) { MoveItems(oldIndex, 1, newIndex); }

		public void MoveRange(int oldIndex, int count, int newIndex) { MoveItems(oldIndex, count, newIndex); }

		public int IndexOf(ImageReference item) { lock (_references) return _references.IndexOf(item); }

		public void RemoveAt(int index) { RemoveItem(index); }

		public ImageReference this[int index]
		{
			get { lock (_references) return _references[index]; }
			set { SetItem(index, value); }
		}

		public void Add(ImageReference item) { InsertItem(Count, item); }

		public void Insert(int index, ImageReference item) { InsertItem(index, item); }

		public void Clear() { ClearItems(); }

		public bool Contains(ImageReference item) { lock (_references) return _references.Contains(item); }

		public int Count { get { return _references.Count; } }

		public bool Remove(ImageReference item) { return RemoveItem(_references.IndexOf(item)); }

		public void CopyTo(ImageReference[] array, int arrayIndex) { lock (_references) _references.CopyTo(array, arrayIndex); }

		public bool IsReadOnly { get { return false; } }

		public IEnumerator<ImageReference> GetEnumerator() { lock (_references) return _references.GetEnumerator(); }

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (CollectionChanged != null)
			{
				if (_notificationSuspended)
					_collectionChanged = true;
				else
					_ownerContext.SendIfNeeded(() => CollectionChanged(this, e));
			}
		}

		protected virtual void OnCollectionItemPropertyChanged(CompositePropertyChangedEventArgs<ImageReference> e)
		{
			if (CollectionItemPropertyChanged != null)
				_ownerContext.SendIfNeeded(() => CollectionItemPropertyChanged(this, e));
		}

		void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			var castedSender = (ImageReference)sender;
			if (_notificationSuspended)
				_itemChanges.Add(new KeyValuePair<ImageReference, string>(castedSender, e.PropertyName));
			else
				OnCollectionItemPropertyChanged(new CompositePropertyChangedEventArgs<ImageReference>(new[] { new KeyValuePair<ImageReference, string>(castedSender, e.PropertyName) }));
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
					OnCollectionItemPropertyChanged(new CompositePropertyChangedEventArgs<ImageReference>(_itemChanges));
				_itemChanges.Clear();
			});
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;
		public event EventHandler<CompositePropertyChangedEventArgs<ImageReference>> CollectionItemPropertyChanged;
	}
}
