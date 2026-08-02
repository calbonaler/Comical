using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace Comical.Core;

public class Bookmark : INotifyPropertyChanged
{
	public string Name { get; set => Utils.SetProperty(ref field, value ?? string.Empty, this, PropertyChanged); } = string.Empty;

	public int Target { get; set => Utils.SetProperty(ref field, value, this, PropertyChanged); }

	internal static Bookmark Load(BinaryReader reader)
	{
		var name = reader.ReadString();
		var target = reader.ReadInt32();
		return new Bookmark() { Name = name, Target = target };
	}

	internal void Save(BinaryWriter writer)
	{
		writer.Write(Name);
		writer.Write(Target);
	}

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class BookmarkCollection : SynchronizedObservableCollection<Bookmark>
{
	internal void Load(BinaryReader reader) => AddRange(Enumerable.Repeat(0, reader.ReadInt32()).Select(_ => Bookmark.Load(reader)));

	internal static void Save(IReadOnlyList<Bookmark> bookmarks, BinaryWriter writer)
	{
		writer.Write(bookmarks.Count);
		foreach (var bookmark in bookmarks)
			bookmark.Save(writer);
	}
}
