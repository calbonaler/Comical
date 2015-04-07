using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace Comical.Core
{
	public class FileHeader
	{
		public FileHeader(string fileName, string password, Version fileVersion)
		{
			password = password ?? string.Empty;
			if (fileVersion == null)
				throw new ArgumentNullException("fileVersion");
			Path = fileName;
			fileIdentifier = new byte[] { 0x43, 0x49, 0x43 };
			FileVersionMajor = (byte)fileVersion.Major;
			FileVersionMinor = (byte)fileVersion.Minor;
			hashData = password.Length > 0 ? Crypto.Encrypt(Encoding.Unicode.GetBytes(sample), password, Encoding.Unicode) : new byte[0];
			Password = password;
		}

		public FileHeader(string fileName)
		{
			Path = fileName;
			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
			using (Stream stream = FileStream.Synchronized(fs))
				InitializeWithStream(stream);
		}

		internal FileHeader(string fileName, Stream stream)
		{
			Path = fileName;
			InitializeWithStream(stream);
		}

		void InitializeWithStream(Stream stream)
		{
			var l = GetThumbnailSize(stream, true);
			if (l > 0)
			{
				Thumbnail = new Bitmap(Image.FromStream(stream, false, false));
				stream.Seek(l, SeekOrigin.Begin);
			}
			byte[] bs = new byte[3];
			stream.Read(bs, 0, bs.Length);
			fileIdentifier = bs;
			FileVersionMajor = (byte)stream.ReadByte();
			if (FileVersionMajor >= 4)
				FileVersionMinor = (byte)stream.ReadByte();
			if (CanOpen && FileVersion <= System.Reflection.Assembly.GetExecutingAssembly().GetName().Version)
			{
				byte dmLen = (byte)stream.ReadByte();
				if (dmLen > 0)
				{
					byte[] dm = new byte[dmLen];
					stream.Read(dm, 0, dm.Length);
					hashData = dm;
				}
				else
					hashData = new byte[0];
				BinaryReader reader = new BinaryReader(stream, Encoding.Unicode);
				Title = reader.ReadString();
				Author = reader.ReadString();
				int year = reader.ReadUInt16();
				int month = reader.ReadByte();
				int day = reader.ReadByte();
				if (year > 1 && year <= 9999 && month >= 1 && month <= 12 && day >= 1 && day <= DateTime.DaysInMonth(year, month))
					DateOfPublication = new DateTime(year, month, day);
				else
					DateOfPublication = null;
				PageTurningDirection = FileVersion.Major >= 4 ? (PageTurningDirection)reader.ReadByte() : PageTurningDirection.ToRight;
			}
		}

		const string sample = "This file has already been decoded.";
		byte[] fileIdentifier;
		byte[] hashData;
		string title = string.Empty;
		string author = string.Empty;

		public Image Thumbnail { get; set; }

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

		public string Title
		{
			get { return title; }
			set { title = value ?? string.Empty; }
		}

		public string Author
		{
			get { return author; }
			set { author = value ?? string.Empty; }
		}

		public DateTime? DateOfPublication { get; set; }

		public PageTurningDirection PageTurningDirection { get; set; }

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
	}
}
