using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comical.Core;

public class FileHeader
{
	public FileHeader(string title, string author, DateTime? published, BindingSide bindingSide, Binary? thumbnail)
		: this(
			title,
			author,
			published,
			bindingSide,
			thumbnail,
			LatestSupportedFileVersion
		)
	{ }

	FileHeader(string title, string author, DateTime? published, BindingSide bindingSide, Binary? thumbnail, Version fileVersion)
	{
		Title = title ?? string.Empty;
		Author = author ?? string.Empty;
		Published = published;
		BindingSide = bindingSide;
		Thumbnail = thumbnail;
		FileVersion = fileVersion;
	}

	public static async Task<FileHeader?> LoadAsync(string fileName)
	{
		using var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
		return await LoadAsync(fs).ConfigureAwait(false);
	}

	public static async Task<FileHeader?> LoadAsync(Stream stream)
	{
		ArgumentNullException.ThrowIfNull(stream);

		Binary? thumbnail = null;
		var headerBuffer = ArrayPool<byte>.Shared.Rent(6);
		try
		{
			var header = headerBuffer.AsMemory(..6);
			var readHeaderLength = await stream.ReadExactlyNoThrowAsync(header[0..FileIdentifier.Length]).ConfigureAwait(false);
			if (readHeaderLength == FileIdentifier.Length && header.Span[..2].SequenceEqual("BM"u8))
			{
				readHeaderLength += await stream.ReadExactlyNoThrowAsync(header[readHeaderLength..]).ConfigureAwait(false);
				if (readHeaderLength < header.Length)
					return null;
				thumbnail = await Binary.TryFromStreamAsync(stream, (int)BitConverter.ToUInt32(header.Span[2..]) - header.Length, header).ConfigureAwait(false);
				if (thumbnail == null)
					return null;
				readHeaderLength = await stream.ReadExactlyNoThrowAsync(header[..FileIdentifier.Length]).ConfigureAwait(false);
			}

			if (readHeaderLength < FileIdentifier.Length)
				return null;
			if (!header.Span[..FileIdentifier.Length].SequenceEqual(FileIdentifier))
				return null;
		}
		finally { ArrayPool<byte>.Shared.Return(headerBuffer); }

		var majorFileVersion = stream.ReadByte();
		var minorFileVersion = 0;
		if (majorFileVersion >= 4)
			minorFileVersion = stream.ReadByte();
		var fileVersion = new Version(majorFileVersion, minorFileVersion);
		if (fileVersion > LatestSupportedFileVersion)
			return null;

		var skip = stream.ReadByte();
		stream.Seek(skip, SeekOrigin.Current);
		using var reader = new BinaryReader(stream, Encoding.Unicode, true);
		var title = reader.ReadString() ?? string.Empty;
		var author = reader.ReadString() ?? string.Empty;
		int year = reader.ReadUInt16();
		int month = reader.ReadByte();
		int day = reader.ReadByte();
		DateTime? published = null;
		if (year > 1 && year <= 9999 && month >= 1 && month <= 12 && day >= 1 && day <= DateTime.DaysInMonth(year, month))
			published = new DateTime(year, month, day);
		var bindingSide = fileVersion.Major >= 4 ? (BindingSide)reader.ReadByte() : BindingSide.Right;

		return new FileHeader(title, author, published, bindingSide, thumbnail, fileVersion);
	}

	public static readonly Version LatestSupportedFileVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version!;
	static ReadOnlySpan<byte> FileIdentifier => "CIC"u8;

	public Binary? Thumbnail { get; }

	public Version FileVersion { get; }

	public string Title { get; }

	public string Author { get; }

	public DateTime? Published { get; }

	public BindingSide BindingSide { get; }

	internal async Task SaveAsync(Stream stream)
	{
		if (Thumbnail != null)
			await Thumbnail.WriteToAsync(stream).ConfigureAwait(false);
		await stream.WriteAsync(FileIdentifier.ToArray()).ConfigureAwait(false);
		stream.WriteByte((byte)FileVersion.Major);
		if (FileVersion.Major >= 4)
			stream.WriteByte((byte)FileVersion.Minor);
		stream.WriteByte(0);
		using var writer = new BinaryWriter(stream, Encoding.Unicode, true);
		writer.Write(Title);
		writer.Write(Author);
		writer.Write((ushort)(Published ?? new DateTime(1, 1, 1)).Year);
		writer.Write((byte)(Published ?? new DateTime(1, 1, 1)).Month);
		writer.Write((byte)(Published ?? new DateTime(1, 1, 1)).Day);
		if (FileVersion.Major >= 4)
			writer.Write((byte)BindingSide);
	}
}
