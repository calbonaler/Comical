using System;
using System.IO;
using System.Text;

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
			) { }

		FileHeader(string title, string author, DateTime? dateOfPublication, PageTurningDirection pageTurningDirection, byte[] thumbnail, Version fileVersion)
		{
			Title = title ?? string.Empty;
			Author = author ?? string.Empty;
			DateOfPublication = dateOfPublication;
			PageTurningDirection = pageTurningDirection;
			Thumbnail = thumbnail;
			FileVersion = fileVersion;
		}

		public static FileHeader Load(string fileName)
		{
			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				return Load(fs);
		}

		public static FileHeader Load(Stream stream)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));
			byte[] thumbnail = null;
			long pos = stream.Position;
			byte[] bitmapHeader = new byte[6];
			stream.Read(bitmapHeader, 0, bitmapHeader.Length);
			stream.Seek(pos, SeekOrigin.Begin);
			if (Encoding.ASCII.GetString(bitmapHeader, 0, 2) == "BM")
			{
				var size = BitConverter.ToUInt32(bitmapHeader, 2);
				thumbnail = new byte[size];
				stream.Read(thumbnail, 0, (int)size);
			}

			byte[] cicHeader = new byte[3];
			stream.Read(cicHeader, 0, cicHeader.Length);
			if (Encoding.ASCII.GetString(cicHeader) != "CIC")
				return null;

			int major = stream.ReadByte();
			int minor = 0;
			if (major >= 4)
				minor = stream.ReadByte();
			Version fileVersion = new Version(major, minor);
			if (fileVersion > LatestSupportedFileVersion)
				return null;
			
			var skip = stream.ReadByte();
			stream.Seek(skip, SeekOrigin.Current);
			using (BinaryReader reader = new BinaryReader(stream, Encoding.Unicode, true))
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
		
		internal void Save(Stream stream)
		{
			stream.Write(Thumbnail, 0, Thumbnail.Length);
			stream.Write(FileIdentifier, 0, FileIdentifier.Length);
			stream.WriteByte((byte)FileVersion.Major);
			if (FileVersion.Major >= 4)
				stream.WriteByte((byte)FileVersion.Minor);
			stream.WriteByte(0);
			using (BinaryWriter writer = new BinaryWriter(stream, Encoding.Unicode, true))
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
