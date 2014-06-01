using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using CommonLibrary;

namespace Comical.Core
{
	public class ImageReference : INotifyPropertyChanged
	{
		internal ImageReference(ImageViewMode mode, long pos, int len)
		{
			_mode = mode;
			Position = pos;
			Length = len;
		}

		internal SynchronizationContext ownerContext;
		internal Func<ImageReference, Size, Image> getImage = (ir, sz) => null;
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

		public long Position { get; private set; }

		public int Length { get; private set; }

		public Image GetImage() { return getImage(this, Size.Empty); }

		public Image GetImage(Size size) { return getImage(this, size); }

		protected void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				ownerContext.SendIfNeeded(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class ImageReferenceCollection : IReadOnlyList<ImageReference>, INotifyCollectionChanged
	{
		public ImageReferenceCollection(Func<ImageReference, Size, Image> getImage, SynchronizationContext context)
		{
			ExcludedItems = new ReadOnlyCollection<ImageReference>(excluded);
			ownerContext = context;
			this.getImage = getImage;
		}

		Func<ImageReference, Size, Image> getImage = (ir, sz) => null;
		List<ImageReference> references = new List<ImageReference>();
		List<ImageReference> excluded = new List<ImageReference>();
		readonly object lockObject = new object();
		public ReadOnlyCollection<ImageReference> ExcludedItems { get; private set; }
		bool _notificationSuspended = false;
		List<NotifyCollectionChangedEventArgs> collectionChanges = new List<NotifyCollectionChangedEventArgs>();
		Dictionary<object, string> itemChanges = new Dictionary<object, string>();
		SynchronizationContext ownerContext = null;

		protected void ClearItems()
		{
			lock (lockObject)
			{
				for (int i = references.Count - 1; i >= 0; i--)
				{
					references[i].PropertyChanged -= OnItemPropertyChanged;
					references[i].ownerContext = null;
					references[i].getImage = (ir, sz) => null;
					references.RemoveAt(i);
				}
				for (int i = excluded.Count - 1; i >= 0; i--)
				{
					excluded[i].PropertyChanged -= OnItemPropertyChanged;
					excluded[i].ownerContext = null;
					excluded[i].getImage = (ir, sz) => null;
					excluded.RemoveAt(i);
				}
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		protected void InsertItem(int index, ImageReference item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			item.PropertyChanged += OnItemPropertyChanged;
			item.ownerContext = ownerContext;
			item.getImage = getImage;
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

		protected void ExcludeItem(int index)
		{
			var item = this[index];
			lock (lockObject)
			{
				excluded.Add(item);
				references.RemoveAt(index);
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
		}

		protected void IncludeItem(int excludedIndex, int insertIndex)
		{
			lock (lockObject)
			{
				references.Insert(insertIndex, excluded[excludedIndex]);
				excluded.RemoveAt(excludedIndex);
			}
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this[insertIndex], insertIndex));
		}

		public void Move(int oldIndex, int newIndex) { MoveItems(oldIndex, 1, newIndex); }

		public void MoveRange(int oldIndex, int count, int newIndex) { MoveItems(oldIndex, count, newIndex); }

		public void Include(int excludedIndex) { IncludeItem(excludedIndex, references.Count); }

		public void Include(int excludedIndex, int insertIndex) { IncludeItem(excludedIndex, insertIndex); }

		public int IndexOf(ImageReference item) { return references.IndexOf(item); }

		public void ExcludeAt(int index) { ExcludeItem(index); }

		public ImageReference this[int index] { get { return references[index]; } }

		internal void Add(ImageReference item) { InsertItem(Count, item); }
		
		internal void Clear() { ClearItems(); }

		public bool Contains(ImageReference item) { return references.Contains(item); }

		public int Count { get { return references.Count; } }

		public bool Exclude(ImageReference item)
		{
			ExcludeItem(IndexOf(item));
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
