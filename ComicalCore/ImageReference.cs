using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace Comical.Core;

public class ImageReference(Binary data) : INotifyPropertyChanged
{
	ImageViewMode _mode;

	public Binary Data { get; } = data ?? throw new ArgumentNullException(nameof(data));

	public ImageViewMode ViewMode
	{
		get => _mode;
		set => Utils.SetProperty(ref _mode, value, this, PropertyChanged);
	}

	internal static async Task<ImageReference> LoadAsync(BinaryReader reader, Version fileVersion)
	{
		if (fileVersion < new Version(4, 4))
			reader.ReadString(); // 名前
		if (fileVersion < new Version(4, 3))
			reader.ReadString(); // フォーマット
		var mode = (ImageViewMode)reader.ReadByte();
		var size = reader.ReadInt32(); // サイズ
		var binary = await Binary.FromStreamAsync(reader.BaseStream, size, false).ConfigureAwait(false);
		return new ImageReference(binary) { ViewMode = mode };
	}

	internal async Task SaveAsync(BinaryWriter writer)
	{
		writer.Write((byte)ViewMode); // 利用情報
		writer.Write(Data.Length); // 画像データ大きさ
		await Data.WriteToAsync(writer.BaseStream).ConfigureAwait(false); // 画像データ
	}

	public event PropertyChangedEventHandler? PropertyChanged;
}

public class ImageReferenceCollection : SynchronizedObservableCollection<ImageReference>
{
	internal async Task LoadAsync(BinaryReader reader, Version fileVersion, IProgress<int> progress)
	{
		var images = new ImageReference[reader.ReadInt32()];
		for (var i = 0; i < images.Length; i++)
		{
			images[i] = await ImageReference.LoadAsync(reader, fileVersion).ConfigureAwait(false);
			progress?.Report((i + 1) * 100 / images.Length);
		}
		AddRange(images);
	}

	internal static async Task SaveAsync(IReadOnlyList<ImageReference> images, BinaryWriter writer, IProgress<int> progress)
	{
		writer.Write(images.Count);
		for (var i = 0; i < images.Count; i++)
		{
			await images[i].SaveAsync(writer).ConfigureAwait(false);
			progress?.Report((i + 1) * 100 / images.Count);
		}
	}
}
