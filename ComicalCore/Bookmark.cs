using System.ComponentModel;
using System.IO;
using System.Text;

namespace Comical.Core
{
	public class Bookmark : INotifyPropertyChanged
	{
		string _name = string.Empty;
		int _target;

		public string Name
		{
			get => _name;
			set => Utils.SetProperty(ref _name, value ?? string.Empty, this, PropertyChanged);
		}

		public int Target
		{
			get => _target;
			set => Utils.SetProperty(ref _target, value, this, PropertyChanged);
		}

		internal static Bookmark Load(BinaryReader reader)
		{
			var bookmark = new Bookmark();
			bookmark.Name = reader.ReadString();
			bookmark.Target = reader.ReadInt32();
			return bookmark;
		}

		internal void Save(BinaryWriter writer)
		{
			writer.Write(Name);
			writer.Write(Target);
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class BookmarkCollection : SynchronizedObservableCollection<Bookmark>
	{
		internal void Load(Stream stream)
		{
			using (var reader = new BinaryReader(stream, Encoding.Unicode, true))
			{
				int count = reader.ReadInt32();
				for (int i = 0; i < count; i++)
					Add(Bookmark.Load(reader));
			}
		}

		internal void Save(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.Unicode, true))
			{
				writer.Write(Count);
				for (int i = 0; i < Count; i++)
					this[i].Save(writer);
			}
		}
	}
}
