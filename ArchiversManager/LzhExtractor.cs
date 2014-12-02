using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Comical.Archivers.Manager
{
	public static class LzhExtractor
	{
		sealed class LzhDecoder
		{
			ushort bitBuffer;
			
			ushort DecodeCode(Stream source, ushort[] codeTable, byte[] codeLength, ushort[] left, ushort[] right)
            {
                var code = codeTable[bitBuffer >> (16 - 12)];
                if (code < codeLength.Length)
                    FillBuffer(source, codeLength[code]);
                else
                {
                    FillBuffer(source, 12);
					for (var mask = 1 << (16 - 1); code >= codeLength.Length; mask >>= 1)
						code = (bitBuffer & mask) != 0 ? right[code] : left[code];
                    FillBuffer(source, (byte)(codeLength[code] - 12));
                }
                return code;
            }

			ushort DecodePointer(Stream source, ushort[] pointerTable, byte[] pointerLength, ushort[] left, ushort[] right, int pointerCount)
			{
				var pointer = pointerTable[bitBuffer >> (16 - 8)];
				if (pointer < pointerCount)
					FillBuffer(source, pointerLength[pointer]);
				else
				{
					FillBuffer(source, 8);
					for (var mask = 1 << (16 - 1); pointer >= pointerCount; mask >>= 1)
						pointer = (bitBuffer & mask) != 0 ? right[pointer] : left[pointer];
					FillBuffer(source, (byte)(pointerLength[pointer] - 8));
				}
				if (pointer != 0)
					pointer = (ushort)((1 << (pointer - 1)) + ReadBits(source, (byte)(pointer - 1)));
				return pointer;
			}

			static void MakeTable(ushort bitLengthCount, byte[] bitLength, ushort tableBitCount, /*W*/ushort[] table, /*W*/ushort[] left, /*W*/ushort[] right)
			{
				ushort[] count = new ushort[17];
				for (int i = 0; i < bitLengthCount; i++)
					count[bitLength[i]]++;
				ushort total = 0;
				ushort[] weight = new ushort[17];
				ushort[] start = new ushort[17];
				for (int i = 1; i <= 16; i++)
				{
					start[i] = total;
					weight[i] = (ushort)(1 << (16 - i));
					total += (ushort)(weight[i] * count[i]);
				}
				if (total != 0)
					return;
				for (int i = 1; i <= tableBitCount; i++)
				{
					start[i] >>= 16 - tableBitCount;
					weight[i] >>= 16 - tableBitCount;
				}
				if (start[tableBitCount + 1] >> (16 - tableBitCount) != 0)
					Array.Clear(table, 0, 1 << tableBitCount);
				int available = bitLengthCount;
				for (ushort i = 0; i < bitLengthCount; i++)
				{
					if (bitLength[i] == 0)
						continue;
					if (bitLength[i] <= tableBitCount)
					{
						for (ushort j = 0; j < weight[bitLength[i]]; j++)
							table[j + start[bitLength[i]]] = i;
					}
					else
					{
						var pointer = Tuple.Create(table, start[bitLength[i]] >> (16 - tableBitCount));
						for (int j = 0, direction = start[bitLength[i]] << tableBitCount; j < bitLength[i] - tableBitCount; j++, direction <<= 1)
						{
							if (pointer.Item1[pointer.Item2] == 0)
							{
								right[available] = left[available] = 0;
								pointer.Item1[pointer.Item2] = (ushort)available++;
							}
							pointer = Tuple.Create((direction & 0x8000) != 0 ? right : left, (int)pointer.Item1[pointer.Item2]);
						}
						pointer.Item1[pointer.Item2] = i;
					}
					start[bitLength[i]] += weight[bitLength[i]];
				}
			}

			void ReadPointerLength(Stream source, /*W*/ushort[] pointerTable, /*W*/byte[] pointerLength, /*W*/ushort[] left, /*W*/ushort[] right, ushort pointerLengthCount, byte bitCount, int i_special)
			{
				var n = ReadBits(source, bitCount);
				if (n == 0)
				{
					var c = ReadBits(source, bitCount);
					Array.Clear(pointerLength, 0, pointerLengthCount);
					for (int i = 0; i < pointerTable.Length; i++)
                        pointerTable[i] = c;
				}
				else
				{
					ushort i = 0;
					while (i < n)
					{
						var c = (ushort)(bitBuffer >> (16 - 3));
						if (c == ((1 << 3) - 1))
						{
							for (ushort mask = 1 << (16 - 4); (mask & bitBuffer) != 0; mask >>= 1)
								c++;
						}
						FillBuffer(source, (byte)(c < 7 ? 3 : c - 3));
						pointerLength[i++] = (byte)c;
						if (i == i_special)
						{
							c = ReadBits(source, 2);
							while (c-- > 0)
								pointerLength[i++] = 0;
						}
					}
					while (i < pointerLengthCount)
						pointerLength[i++] = 0;
					MakeTable(pointerLengthCount, pointerLength, 8, pointerTable, left, right);
				}
			}

			void ReadCodeLength(Stream source, /*W*/ushort[] codeTable, /*W*/byte[] codeLength, ushort[] pointerTable, byte[] pointerLength, /*W*/ushort[] left, /*W*/ushort[] right)
			{
				var n = ReadBits(source, 9);
				if (n == 0)
				{
					var c = ReadBits(source, 9);
					Array.Clear(codeLength, 0, codeLength.Length);
					for (int i = 0; i < codeTable.Length; i++)
						codeTable[i] = c;
				}
				else
				{
					ushort i = 0;
					while (i < n)
					{
						var c = pointerTable[bitBuffer >> (16 - 8)];
						for (ushort mask = 1 << (16 - 9); c >= 19; mask >>= 1)
							c = (bitBuffer & mask) != 0 ? right[c] : left[c];
						FillBuffer(source, pointerLength[c]);
						if (c <= 2)
						{
							if (c == 0)
								c = 1;
							else if (c == 1)
								c = (ushort)(ReadBits(source, 4) + 3);
							else
								c = (ushort)(ReadBits(source, 9) + 20);
							while (c-- > 0)
								codeLength[i++] = 0;
						}
						else
							codeLength[i++] = (byte)(c - 2);
					}
					while (i < codeLength.Length)
						codeLength[i++] = 0;
					MakeTable((ushort)codeLength.Length, codeLength, 12, codeTable, left, right);
				}
			}

			static void Unstore(Stream source, uint compressedSize, Stream destination)
			{
				byte[] buffer = new byte[32768];
				int bytesRead;
				if (compressedSize == uint.MaxValue)
				{
					while ((bytesRead = source.Read(buffer, 0, buffer.Length)) != 0)
					{
						if (bytesRead == -1)
							break;
						destination.Write(buffer, 0, bytesRead);
					}
				}
				else
				{
					while (compressedSize > 0)
					{
						if ((bytesRead = source.Read(buffer, 0, Math.Min(buffer.Length, (int)compressedSize))) <= 0)
							break;
						destination.Write(buffer, 0, bytesRead);
						compressedSize -= (uint)bytesRead;
					}
				}
			}

			public void Decode(LzhMethod method, Stream source, uint compressedSize, Stream destination, uint originalSize)
			{
                if (method == LzhMethod.LH0)
                {
					Unstore(source, compressedSize, destination);
                    return;
                }
				ushort pointerCount;
				byte pointerBitCount;
				int dictionarySize;
				switch (method)
				{
					case LzhMethod.LH4: pointerCount = 14; pointerBitCount = 4; dictionarySize = 1 << 12; break;
					case LzhMethod.LH5: pointerCount = 14; pointerBitCount = 4; dictionarySize = 1 << 13; break;
					case LzhMethod.LH6: pointerCount = 16; pointerBitCount = 5; dictionarySize = 1 << 15; break;
					case LzhMethod.LH7: pointerCount = 17; pointerBitCount = 5; dictionarySize = 1 << 16; break;
					default: return;
				}
				byte[] text = new byte[dictionarySize];
				remainingSize = compressedSize;
				bitBuffer = 0;
				subBitBuffer = 0;
				subBitCount = 0;
				FillBuffer(source, 16);
				ushort blockSize = 0;
				int location = 0;
				const int offset = 0x0100 - 3;
				ushort[] pointerTable = new ushort[256];
				byte[] pointerLength = new byte[128];
				ushort[] codeTable = new ushort[4096];
				byte[] codeLength = new byte[510];
				ushort[] right = new ushort[1019];
				ushort[] left = new ushort[1019];
                for (uint count = 0; count < originalSize; )
                {
					if (blockSize == 0)
					{
						blockSize = ReadBits(source, 16);
						ReadPointerLength(source, pointerTable, pointerLength, left, right, 19, 5, 3);
						ReadCodeLength(source, codeTable, codeLength, pointerTable, pointerLength, left, right);
						ReadPointerLength(source, pointerTable, pointerLength, left, right, pointerCount, pointerBitCount, -1);
					}
					blockSize--;
					var code = DecodeCode(source, codeTable, codeLength, left, right);
                    if (code <= 255)
                    {
                        text[location++] = (byte)code;
                        if (location == dictionarySize)
                        {
                            destination.Write(text, 0, dictionarySize);
                            location = 0;
                        }
                        count++;
                    }
                    else
                    {
                        count += (uint)(code - offset);
						int pointer = DecodePointer(source, pointerTable, pointerLength, left, right, pointerCount);
                        if ((pointer = location - pointer - 1) < 0)
                            pointer += dictionarySize;
                        for (int i = 0; i < code - offset; i++)
                        {
                            text[location++] = text[pointer++];
                            if (location == dictionarySize)
                            {
                                destination.Write(text, 0, dictionarySize);
                                location = 0;
                            }
                            if (pointer == dictionarySize)
                                pointer = 0;
                        }
                    }
                }
				if (location != 0)
					destination.Write(text, 0, location);
			}

			byte subBitBuffer;
			byte subBitCount;
			uint remainingSize;

			void FillBuffer(Stream source, byte bitCount)
			{
				while (bitCount > subBitCount)
				{
					bitCount -= subBitCount;
					bitBuffer = (ushort)(bitBuffer << subBitCount | subBitBuffer >> (8 - subBitCount));
					subBitBuffer = (byte)(remainingSize-- > 0 ? source.ReadByte() : 0);
					subBitCount = 8;
				}
				subBitCount -= bitCount;
				bitBuffer = (ushort)(bitBuffer << bitCount | subBitBuffer >> (8 - bitCount));
				subBitBuffer <<= bitCount;
			}

			ushort ReadBits(Stream source, byte bitCount)
			{
				var x = (ushort)(bitBuffer >> (int)(16 - bitCount));
				FillBuffer(source, bitCount);
				return x;
			}
		}

		sealed class HeaderInfo
		{
			public string FileName;
			public LzhMethod Method;
			public uint CompressedSize;
			public uint OriginalSize;
			public FileAttributes FileAttribute;
			public DateTime UpdatedTime;
		}

		enum LzhMethod
		{
			Unknown,
			LH0,
			LH1,
			LH2,
			LH3,
			LH4,
			LH5,
			LH6,
			LH7,
		}

		public static string[] Melt(string arc, string extractTo, params string[] files)
		{
			using (var fs = new FileStream(arc, FileMode.Open, FileAccess.Read))
			{
				var paths = new List<string>();
				var buffer = new byte[65536];
				var info = FindHeader(fs, buffer);
				while (info != null)
				{
					var basePosition = fs.Position;
					var name = Path.GetFullPath(Path.Combine(extractTo, info.FileName));
					if (info.FileName[0] != '!' && info.FileName[0] != '$' && info.FileName[0] != '%' && files.Contains(info.FileName, StringComparer.CurrentCultureIgnoreCase))
					{
						using (var dest = new FileStream(name, FileMode.Create, FileAccess.Write))
						{
							paths.Add(name);
							new LzhDecoder().Decode(info.Method, fs, info.CompressedSize, dest, info.OriginalSize);
						}
						File.SetAttributes(name, info.FileAttribute);
						File.SetCreationTimeUtc(name, info.UpdatedTime);
						File.SetLastWriteTimeUtc(name, info.UpdatedTime);
					}
					fs.Seek(basePosition + info.CompressedSize, SeekOrigin.Begin);
					info = ReadHeader(fs, buffer);
				}
				return paths.ToArray();
			}
		}

		static ushort[] CreateCrcTable()
		{
			ushort[] crcTable = new ushort[256];
			for (ushort i = 0; i < crcTable.Length; i++)
			{
				crcTable[i] = i;
				for (int j = 0; j < 8; j++)
					crcTable[i] = (ushort)((crcTable[i] & 1) != 0 ? (crcTable[i] >> 1) ^ 0xA001 : crcTable[i] >> 1);
			}
			return crcTable;
		}

		static readonly ushort[] _crcTable = CreateCrcTable();

		static int GetBits(uint value, int startBit, int bitCount) { return (int)((value >> startBit) & ((1 << bitCount) - 1)); }

		static HeaderInfo FindHeader(Stream stream, byte[] buffer)
		{
			var size = stream.Read(buffer, 0, buffer.Length);
			var tempBuffer = new byte[65536];
			for (int i = 0; i < size - 20; i++)
			{
				if (buffer[i + 2] == '-' && buffer[i + 3] == 'l' && (buffer[i + 4] == 'h' || buffer[i + 4] == 'z') && buffer[i + 6] == '-')
				{
					stream.Seek(i, SeekOrigin.Begin);
					var info = ReadHeader(stream, tempBuffer);
					if (info != null)
						return info;
				}
			}
			return null;
		}

		static HeaderInfo ReadHeader(Stream stream, byte[] buffer)
		{
			if (stream.Read(buffer, 0, 21) != 21)
				return null;
			var level = buffer[20];
			var baseHeaderSize = level >= 2 ? 26 : buffer[0] + 2;
			if (level > 2 || baseHeaderSize < 21 || buffer[0] == 0 || buffer[2] != '-' || buffer[3] != 'l' || buffer[6] != '-' ||
				stream.Read(buffer, 21, baseHeaderSize - 21) != baseHeaderSize - 21)
				return null;
			var crc = CalculateCrc(buffer, 0, baseHeaderSize, 0);
			var info = new HeaderInfo();
			var shiftJis = Encoding.GetEncoding("Shift-JIS");
			{
				info.FileName = string.Empty;
				if (level < 2)
				{
					byte sum = 0;
					for (int i = 2; i < baseHeaderSize; i++)
						sum = unchecked((byte)(sum + buffer[i]));
					if (sum != buffer[1] || baseHeaderSize - 21 < 1 + buffer[21] + 2)
						return null;
					info.FileName = shiftJis.GetString(buffer, 22, buffer[21]);
				}
				info.CompressedSize = (uint)buffer[7] + ((uint)buffer[8] << 8) + ((uint)buffer[9] << 16) + ((uint)buffer[10] << 24);
				info.OriginalSize = (uint)buffer[11] + ((uint)buffer[12] << 8) + ((uint)buffer[13] << 16) + ((uint)buffer[14] << 24);
				info.FileAttribute = (FileAttributes)buffer[19];
				Enum.TryParse(Encoding.ASCII.GetString(buffer, 3, 3), true, out info.Method);
				var date = (uint)buffer[17] + ((uint)buffer[18] << 8);
				var time = (uint)buffer[15] + ((uint)buffer[16] << 8);
				info.UpdatedTime = level < 2 ?
					new DateTime(GetBits(date, 9, 7) + 1980, GetBits(date, 5, 4), GetBits(date, 0, 5), GetBits(time, 11, 5), GetBits(time, 5, 6), GetBits(time, 0, 5) * 2) :
					new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(time + (date << 16));
			}
			var directoryName = string.Empty;
			if (level > 0)
			{
				var extendedHeaderCrc = 0xFFFFFFFF;
				var extendedHeaderSize = (ushort)(buffer[baseHeaderSize - 2] | (buffer[baseHeaderSize - 1] << 8));
				while (extendedHeaderSize > 2)
				{
					if (stream.Read(buffer, 0, extendedHeaderSize) != extendedHeaderSize)
						return null;
					if (level == 1)
						info.CompressedSize -= extendedHeaderSize;
					if (buffer[0] == 0x00 && extendedHeaderSize >= 5 && extendedHeaderCrc == 0xFFFFFFFF) // Common Extended Header
					{
						extendedHeaderCrc = (uint)buffer[1] | ((uint)buffer[2] << 8);
						buffer[1] = buffer[2] = 0;
						crc = CalculateCrc(buffer, 0, extendedHeaderSize, crc);
					}
					else
					{
						crc = CalculateCrc(buffer, 0, extendedHeaderSize, crc);
						if (buffer[0] == 0x01) // File Name Extended Header
							info.FileName = shiftJis.GetString(buffer, 1, Math.Min(extendedHeaderSize - 3, 260));
						else if (buffer[0] == 0x02) // Directory Name Extended Header
							directoryName = shiftJis.GetString(buffer, 1, Math.Min(extendedHeaderSize - 3, 260));
						else if (buffer[0] == 0x40 && extendedHeaderSize >= 5) // MS-DOS File Attribute Extended Header
							info.FileAttribute = (FileAttributes)(buffer[1] | (buffer[2] << 8));
					}
					extendedHeaderSize = (ushort)(buffer[extendedHeaderSize - 2] | (buffer[extendedHeaderSize - 1] << 8));
				}
				if (extendedHeaderCrc != 0xFFFFFFFF && crc != extendedHeaderCrc)
					return null;
			}
			info.FileName = directoryName + info.FileName;
			return info;
		}

        static int CalculateCrc(byte[] array, int offset, int count, int beforeCrc)
		{
			for (int i = 0; i < count; i++)
				beforeCrc = (ushort)(_crcTable[(beforeCrc ^ array[i + offset]) & 0xFF] ^ (beforeCrc >> 8));
			return beforeCrc;
		}
	}
}
