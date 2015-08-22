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
		string _name = string.Empty;
		int _target;
		internal IReadOnlyList<ImageReference> imageReferences;
		internal SynchronizationContext OwnerContext;

		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value ?? string.Empty;
					PropertyChanged.Raise(this, OwnerContext);
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
					PropertyChanged.Raise(this, OwnerContext);
					PropertyChanged.Raise(this, OwnerContext, nameof(TargetImage));
				}
			}
		}

		public ImageReference TargetImage => imageReferences?[Target];

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
				item.OwnerContext = null;
				item.imageReferences = null;
				item.PropertyChanged -= OnItemPropertyChanged;
			}
			base.ClearItems();
		}

		protected override void InsertItem(int index, Bookmark item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));
			item.OwnerContext = _ownerContext;
			item.imageReferences = _imageReferences;
			item.PropertyChanged += OnItemPropertyChanged;
			base.InsertItem(index, item);
		}

		protected override void RemoveItem(int index)
		{
			this[index].OwnerContext = null;
			this[index].imageReferences = null;
			this[index].PropertyChanged -= OnItemPropertyChanged;
			base.RemoveItem(index);
		}

		protected override void SetItem(int index, Bookmark item)
		{
			if (item == null)
				throw new ArgumentNullException(nameof(item));
			this[index].OwnerContext = null;
			this[index].imageReferences = null;
			this[index].PropertyChanged -= OnItemPropertyChanged;
			item.OwnerContext = _ownerContext;
			item.imageReferences = _imageReferences;
			item.PropertyChanged += OnItemPropertyChanged;
			base.SetItem(index, item);
		}

		internal void Load(Stream stream)
		{
			BinaryReader reader = new BinaryReader(stream, Encoding.Unicode);
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				var bookmark = new Bookmark();
				bookmark.Name = reader.ReadString();
				bookmark.Target = reader.ReadInt32();
				Add(bookmark);
			}
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
