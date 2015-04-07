using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;

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
		internal IReadOnlyList<ImageReference> imageReferences;
		internal SynchronizationContext ownerContext;

		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value;
					PropertyChanged.Raise(this, ownerContext);
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
					PropertyChanged.Raise(this, ownerContext);
					PropertyChanged.Raise(this, ownerContext, () => TargetImage);
				}
			}
		}

		public ImageReference TargetImage { get { return imageReferences == null ? null : imageReferences[Target]; } }

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class BookmarkCollection : System.Collections.ObjectModel.ObservableCollection<Bookmark>
	{
		public BookmarkCollection(IReadOnlyList<ImageReference> imageReferences, SynchronizationContext context)
		{
			_imageReferences = imageReferences;
			_ownerContext = context;
		}

		IReadOnlyList<ImageReference> _imageReferences;
		SynchronizationContext _ownerContext;

		public event EventHandler<CompositePropertyChangedEventArgs<Bookmark>> CollectionItemPropertyChanged;

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) { _ownerContext.SendIfNeeded(() => base.OnCollectionChanged(e)); }

		protected virtual void OnCollectionItemPropertyChanged(CompositePropertyChangedEventArgs<Bookmark> e)
		{
			if (CollectionItemPropertyChanged != null)
				_ownerContext.SendIfNeeded(() => CollectionItemPropertyChanged(this, e));
		}

		void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e) { OnCollectionItemPropertyChanged(new CompositePropertyChangedEventArgs<Bookmark>(new[] { new KeyValuePair<Bookmark, string>((Bookmark)sender, e.PropertyName) })); }

		protected override void ClearItems()
		{
			foreach (var item in this)
			{
				item.ownerContext = null;
				item.imageReferences = null;
				item.PropertyChanged -= OnItemPropertyChanged;
			}
			base.ClearItems();
		}

		protected override void InsertItem(int index, Bookmark item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			item.ownerContext = _ownerContext;
			item.imageReferences = _imageReferences;
			item.PropertyChanged += OnItemPropertyChanged;
			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			this[index].ownerContext = null;
			this[index].imageReferences = null;
			this[index].PropertyChanged -= OnItemPropertyChanged;
			base.RemoveItem(index);
		}

		protected override void SetItem(int index, Bookmark item)
		{
			if (item == null)
				throw new ArgumentNullException("item");
			this[index].ownerContext = null;
			this[index].imageReferences = null;
			this[index].PropertyChanged -= OnItemPropertyChanged;
			item.ownerContext = _ownerContext;
			item.imageReferences = _imageReferences;
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
