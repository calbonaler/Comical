using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using CommonLibrary;

namespace Comical.Core
{
	public class Bookmark : INotifyPropertyChanged
	{
		public Bookmark(string name, int target)
		{
			_name = name;
			_target = target;
		}

		string _name;
		int _target;
		internal Func<int, ImageReference> getImageReferenceAtIndex = x => null;
		internal SynchronizationContext ownerContext;

		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					RaisePropertyChanged("Name");
				}
			}
		}

		public int Target
		{
			get { return _target; }
			set
			{
				if (_target != value)
				{
					_target = value;
					RaisePropertyChanged("Target");
					RaisePropertyChanged("TargetImage");
				}
			}
		}

		public ImageReference TargetImage { get { return getImageReferenceAtIndex(Target); } }

		protected void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
				ownerContext.SendIfNeeded(() => PropertyChanged(this, new PropertyChangedEventArgs(propertyName)));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class BookmarkCollection : System.Collections.ObjectModel.ObservableCollection<Bookmark>
	{
		public BookmarkCollection(Func<int, ImageReference> imageReferenceAtIndex, SynchronizationContext context)
		{
			getImageReferenceAtIndex = imageReferenceAtIndex;
			ownerContext = context;
		}

		Func<int, ImageReference> getImageReferenceAtIndex = x => null;
		SynchronizationContext ownerContext;

		public event EventHandler<CompositePropertyChangedEventArgs> CollectionItemPropertyChanged;

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) { ownerContext.SendIfNeeded(() => base.OnCollectionChanged(e)); }

		protected virtual void OnCollectionItemPropertyChanged(CompositePropertyChangedEventArgs e)
		{
			if (CollectionItemPropertyChanged != null)
				ownerContext.SendIfNeeded(() => CollectionItemPropertyChanged(this, e));
		}

		void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e) { OnCollectionItemPropertyChanged(new CompositePropertyChangedEventArgs(new Dictionary<object, string>() { { sender, e.PropertyName } })); }

		protected override void ClearItems()
		{
			foreach (var item in this)
			{
				item.ownerContext = null;
				item.getImageReferenceAtIndex = x => null;
				item.PropertyChanged -= OnItemPropertyChanged;
			}
			base.ClearItems();
		}

		protected override void InsertItem(int index, Bookmark item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			item.ownerContext = ownerContext;
			item.getImageReferenceAtIndex = getImageReferenceAtIndex;
			item.PropertyChanged += OnItemPropertyChanged;
			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			this[index].ownerContext = null;
			this[index].getImageReferenceAtIndex = x => null;
			this[index].PropertyChanged -= OnItemPropertyChanged;
			base.RemoveItem(index);
		}

		protected override void SetItem(int index, Bookmark item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			this[index].ownerContext = null;
			this[index].getImageReferenceAtIndex = x => null;
			this[index].PropertyChanged -= OnItemPropertyChanged;
			item.ownerContext = ownerContext;
			item.getImageReferenceAtIndex = getImageReferenceAtIndex;
			item.PropertyChanged += OnItemPropertyChanged;
			base.SetItem(index, item);
		}

		internal void Load(Stream stream)
		{
			BinaryReader reader = new BinaryReader(stream, Encoding.Unicode);
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
				Add(new Bookmark(reader.ReadString(), reader.ReadInt32()));
		}

		internal void SaveInto(Stream stream)
		{
			BinaryWriter writer = new BinaryWriter(stream, Encoding.Unicode);
			writer.Write(Count);
			for (int i = 0; i < Count; i++)
			{
				writer.Write(this[i].Name);
				writer.Write(this[i].Target);
			}
		}
	}
}
