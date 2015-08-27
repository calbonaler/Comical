using System;
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
				throw new ArgumentNullException(nameof(fileVersion));
			Path = fileName;
			fileIdentifier = new byte[] { 0x43, 0x49, 0x43 };
			FileVersionMajor = (byte)fileVersion.Major;
			FileVersionMinor = (byte)fileVersion.Minor;
			hashData = password.Length > 0 ? Crypto.Transform(Encoding.Unicode.GetBytes(sample), password, Encoding.Unicode, false) : new byte[0];
			Password = password;
		}

		public FileHeader(string fileName)
		{
			Path = fileName;
			using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				InitializeWithStream(fs);
		}

		internal FileHeader(FileStream stream)
		{
			Path = stream.Name;
			InitializeWithStream(stream);
		}

		void InitializeWithStream(Stream stream)
		{
			var l = GetThumbnailSize(stream);
			if (l > 0)
			{
				Thumbnail = new byte[l];
				stream.Read(Thumbnail, 0, (int)l);
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
				using (BinaryReader reader = new BinaryReader(stream, Encoding.Unicode, true))
				{
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
		}

		const string sample = "This file has already been decoded.";
		byte[] fileIdentifier;
		byte[] hashData;
		string title = string.Empty;
		string author = string.Empty;

		public byte[] Thumbnail { get; set; }

		static uint GetThumbnailSize(Stream stream)
		{
			long pos = stream.Position;
			if (pos != 0)
				stream.Seek(0, SeekOrigin.Begin);
			byte[] bs = new byte[2];
			stream.Read(bs, 0, bs.Length);
			uint size;
			if (Encoding.ASCII.GetString(bs) == "BM")
			{
				size = (uint)stream.ReadByte() + (uint)(stream.ReadByte() << 8) +
					(uint)(stream.ReadByte() << 16) + (uint)(stream.ReadByte() << 24);
			}
			else
				size = 0;
			if (pos != stream.Position)
				stream.Seek(pos, SeekOrigin.Begin);
			return size;
		}

		public bool CanOpen => Encoding.ASCII.GetString(fileIdentifier) == "CIC";

		public byte FileVersionMajor { get; private set; }

		public byte FileVersionMinor { get; private set; }

		public Version FileVersion => new Version(FileVersionMajor, FileVersionMinor);

		public bool IsProperPassword(string password) => hashData.Length == 0 || Encoding.Unicode.GetString(Crypto.Transform(hashData, password, Encoding.Unicode, true)) == sample;

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

		public string Password { get; }

		public string Path { get; }

		public bool PathExists => File.Exists(Path);

		internal void SaveInto(Stream stream)
		{
			stream.Write(Thumbnail, 0, Thumbnail.Length);
			stream.Write(fileIdentifier, 0, fileIdentifier.Length);
			stream.WriteByte(FileVersionMajor);
			if (FileVersion.Major >= 4)
				stream.WriteByte(FileVersionMinor);
			stream.WriteByte((byte)hashData.Length);
			stream.Write(hashData, 0, hashData.Length);
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
