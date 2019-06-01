using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Comical.Core
{
	public class FileHeader
	{
		public FileHeader(string title, string author, DateTime? dateOfPublication, PageTurningDirection pageTurningDirection, byte[] thumbnail)
			: this(
				title,
				author,
				dateOfPublication,
				pageTurningDirection,
				thumbnail,
				LatestSupportedFileVersion
			)
		{ }

		FileHeader(string title, string author, DateTime? dateOfPublication, PageTurningDirection pageTurningDirection, byte[] thumbnail, Version fileVersion)
		{
			Title = title ?? string.Empty;
			Author = author ?? string.Empty;
			DateOfPublication = dateOfPublication;
			PageTurningDirection = pageTurningDirection;
			Thumbnail = thumbnail;
			FileVersion = fileVersion;
		}

		public static async Task<FileHeader> LoadAsync(string fileName)
		{
			using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				return await LoadAsync(fs).ConfigureAwait(false);
		}

		public static async Task<FileHeader> LoadAsync(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));

			var header = new byte[6];
			var readHeaderLength = await stream.ReadAsync(header, 0, FileIdentifier.Length).ConfigureAwait(false);
			byte[] thumbnail = null;
			if (readHeaderLength == FileIdentifier.Length && Encoding.ASCII.GetString(header, 0, 2) == "BM")
			{
				readHeaderLength += await stream.ReadAsync(header, readHeaderLength, header.Length - readHeaderLength).ConfigureAwait(false);
				if (readHeaderLength < header.Length)
					return null;
				thumbnail = new byte[BitConverter.ToUInt32(header, 2)];
				header.CopyTo(thumbnail, 0);
				var readThumbnailLength = await stream.ReadAsync(thumbnail, header.Length, thumbnail.Length - header.Length).ConfigureAwait(false);
				if (header.Length + readThumbnailLength < thumbnail.Length)
					return null;
				readHeaderLength = await stream.ReadAsync(header, 0, FileIdentifier.Length).ConfigureAwait(false);
			}

			if (readHeaderLength < FileIdentifier.Length)
				return null;
			if (!header.Take(FileIdentifier.Length).SequenceEqual(FileIdentifier))
				return null;

			var majorFileVersion = stream.ReadByte();
			var minorFileVersion = 0;
			if (majorFileVersion >= 4)
				minorFileVersion = stream.ReadByte();
			var fileVersion = new Version(majorFileVersion, minorFileVersion);
			if (fileVersion > LatestSupportedFileVersion)
				return null;

			var skip = stream.ReadByte();
			stream.Seek(skip, SeekOrigin.Current);
			using (var reader = new BinaryReader(stream, Encoding.Unicode, true))
			{
				var title = reader.ReadString() ?? string.Empty;
				var author = reader.ReadString() ?? string.Empty;
				int year = reader.ReadUInt16();
				int month = reader.ReadByte();
				int day = reader.ReadByte();
				DateTime? dateOfPublication = null;
				if (year > 1 && year <= 9999 && month >= 1 && month <= 12 && day >= 1 && day <= DateTime.DaysInMonth(year, month))
					dateOfPublication = new DateTime(year, month, day);
				var pageTurningDirection = fileVersion.Major >= 4 ? (PageTurningDirection)reader.ReadByte() : PageTurningDirection.ToRight;

				return new FileHeader(title, author, dateOfPublication, pageTurningDirection, thumbnail, fileVersion);
			}
		}

		public static readonly Version LatestSupportedFileVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
		static readonly byte[] FileIdentifier = new byte[] { 0x43, 0x49, 0x43 };

		public byte[] Thumbnail { get; }

		public Version FileVersion { get; }

		public string Title { get; }

		public string Author { get; }

		public DateTime? DateOfPublication { get; }

		public PageTurningDirection PageTurningDirection { get; }

		internal async Task SaveAsync(Stream stream)
		{
			if (Thumbnail != null)
				await stream.WriteAsync(Thumbnail, 0, Thumbnail.Length).ConfigureAwait(false);
			await stream.WriteAsync(FileIdentifier, 0, FileIdentifier.Length).ConfigureAwait(false);
			stream.WriteByte((byte)FileVersion.Major);
			if (FileVersion.Major >= 4)
				stream.WriteByte((byte)FileVersion.Minor);
			stream.WriteByte(0);
			using (var writer = new BinaryWriter(stream, Encoding.Unicode, true))
			{
				writer.Write(Title);
				writer.Write(Author);
				writer.Write((ushort)(DateOfPublication ?? new DateTime(1, 1, 1)).Year);
				writer.Write((byte)(DateOfPublication ?? new DateTime(1, 1, 1)).Month);
				writer.Write((byte)(DateOfPublication ?? new DateTime(1, 1, 1)).Day);
				if (FileVersion.Major >= 4)
					writer.Write((byte)PageTurningDirection);
			}
		}
	}
}
