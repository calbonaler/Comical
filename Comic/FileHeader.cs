using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Comical.Core
{
	public class FileHeader
	{
		FileHeader()
		{
			Title = "";
			Author = "";
			PageTurningDirection = PageTurningDirection.ToRight;
			Password = "";
			Path = "";
		}

		const string sample = "This file has already been decoded.";
		byte[] fileIdentifier;
		byte[] hashData;

		public Image Thumbnail { get; private set; }

		static uint GetThumbnailSize(Stream stream, bool reset)
		{
			long pos = stream.Position;
			if (pos != 0)
				stream.Seek(0, SeekOrigin.Begin);
			byte[] bs = new byte[2];
			stream.Read(bs, 0, bs.Length);
			uint size;
			if (System.Text.Encoding.ASCII.GetString(bs) == "BM")
			{
				size = (uint)stream.ReadByte() + (uint)(stream.ReadByte() << 8) +
					(uint)(stream.ReadByte() << 16) + (uint)(stream.ReadByte() << 24);
			}
			else
				size = 0;
			if (reset && pos != stream.Position)
				stream.Seek(pos, SeekOrigin.Begin);
			return size;
		}

		public bool CanOpen { get { return Encoding.ASCII.GetString(fileIdentifier) == "CIC"; } }

		public byte FileVersionMajor { get; private set; }

		public byte FileVersionMinor { get; private set; }

		public Version FileVersion { get { return new Version(FileVersionMajor, FileVersionMinor); } }

		public bool IsProperPassword(string password)
		{
			return hashData.Length == 0 || Encoding.Unicode.GetString(Crypto.Decrypt(hashData, password, Encoding.Unicode)) == sample;
		}

		public string Title { get; private set; }

		public string Author { get; private set; }

		public DateTime? DateOfPublication { get; private set; }

		public PageTurningDirection PageTurningDirection { get; private set; }

		public string Password { get; private set; }

		public string Path { get; private set; }

		public bool PathExists { get { return File.Exists(Path); } }

		internal void SaveInto(Stream stream)
		{
			if (Thumbnail != null)
				Thumbnail.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
			stream.Write(fileIdentifier, 0, fileIdentifier.Length);
			stream.WriteByte(FileVersionMajor);
			if (FileVersion.Major >= 4)
				stream.WriteByte(FileVersionMinor);
			stream.WriteByte((byte)hashData.Length);
			stream.Write(hashData, 0, hashData.Length);
			BinaryWriter writer = new BinaryWriter(stream, Encoding.Unicode);
			writer.Write(Title);
			writer.Write(Author);
			writer.Write((ushort)(DateOfPublication ?? new DateTime(1, 1, 1)).Year);
			writer.Write((byte)(DateOfPublication ?? new DateTime(1, 1, 1)).Month);
			writer.Write((byte)(DateOfPublication ?? new DateTime(1, 1, 1)).Day);
			if (FileVersion.Major >= 4)
				writer.Write((byte)PageTurningDirection);
		}

		public static FileHeader Create(string fileName)
		{
			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			using (Stream stream = FileStream.Synchronized(fs))
				return Create(fileName, stream);
		}
		
		internal static FileHeader Create(string fileName, Stream stream)
		{
			FileHeader op = new FileHeader();
			op.Path = fileName;
			var l = GetThumbnailSize(stream, true);
			if (l > 0)
			{
				op.Thumbnail = new Bitmap(Image.FromStream(stream, false, false));
				stream.Seek(l, SeekOrigin.Begin);
			}
			byte[] bs = new byte[3];
			stream.Read(bs, 0, bs.Length);
			op.fileIdentifier = bs;
			op.FileVersionMajor = (byte)stream.ReadByte();
			if (op.FileVersionMajor >= 4)
				op.FileVersionMinor = (byte)stream.ReadByte();
			if (op.CanOpen && op.FileVersion <= System.Reflection.Assembly.GetExecutingAssembly().GetName().Version)
			{
				byte dmLen = (byte)stream.ReadByte();
				if (dmLen > 0)
				{
					byte[] dm = new byte[dmLen];
					stream.Read(dm, 0, dm.Length);
					op.hashData = dm;
				}
				else
					op.hashData = new byte[0];
				BinaryReader reader = new BinaryReader(stream, Encoding.Unicode);
				op.Title = reader.ReadString() ?? "";
				op.Author = reader.ReadString() ?? "";
				int year = reader.ReadUInt16();
				int month = reader.ReadByte();
				int day = reader.ReadByte();
				if (year > 1 && year <= 9999 && month >= 1 && month <= 12 && day >= 1 && day <= DateTime.DaysInMonth(year, month))
					op.DateOfPublication = new DateTime(year, month, day);
				else
					op.DateOfPublication = null;
				op.PageTurningDirection = op.FileVersion.Major >= 4 ? (PageTurningDirection)reader.ReadByte() : PageTurningDirection.ToRight;
			}
			return op;
		}

		internal static FileHeader Create(string fileName, Image thumbnail, Version fileVersion, string password, string title, string author, DateTime? dateOfPublication, PageTurningDirection pageTurningDirection)
		{
			return new FileHeader()
			{
				Path = fileName,
				Thumbnail = thumbnail,
				fileIdentifier = new byte[] { 0x43, 0x49, 0x43 },
				FileVersionMajor = (byte)fileVersion.Major,
				FileVersionMinor = (byte)fileVersion.Minor,
				hashData = password.Length > 0 ? Crypto.Encrypt(Encoding.Unicode.GetBytes(sample), password, Encoding.Unicode) : new byte[0],
				Title = title,
				Author = author,
				DateOfPublication = dateOfPublication,
				PageTurningDirection = pageTurningDirection,
				Password = password,
			};
		}
	}
}
