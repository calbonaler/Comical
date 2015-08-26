using System.ComponentModel;
using System.IO;
using System.Text;
using Comical.Infrastructures;

namespace Comical.Core
{
	public class Bookmark : INotifyPropertyChanged
	{
		string _name = string.Empty;
		int _target;

		public string Name
		{
			get { return _name; }
			set
			{
				if (_name != value)
				{
					_name = value ?? string.Empty;
					PropertyChanged.Raise(this);
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
					PropertyChanged.Raise(this);
				}
			}
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
				{
					var bookmark = new Bookmark();
					bookmark.Name = reader.ReadString();
					bookmark.Target = reader.ReadInt32();
					Add(bookmark);
				}
			}
		}

		internal void SaveInto(Stream stream)
		{
			using (var writer = new BinaryWriter(stream, Encoding.Unicode, true))
			{
				writer.Write(Count);
				for (int i = 0; i < Count; i++)
				{
					writer.Write(this[i].Name);
					writer.Write(this[i].Target);
				}
			}
		}
	}
}
