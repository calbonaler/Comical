using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommonLibrary;

namespace Comical.Archivers.Manager
{
	public static class LzhExtractor
	{
		static class LzhDecoder
		{
			sealed class BitStream
			{
				public BitStream(Stream source)
				{
					_source = source;
					FillBuffer(16);
				}

				Stream _source;
				ushort _bitBuffer;
				byte _subBitBuffer;
				byte _subBitCount;

				void FillBuffer(int bitCount)
				{
					while (true)
					{
						_bitBuffer = (ushort)(_bitBuffer << Math.Min(bitCount, _subBitCount) | _subBitBuffer >> (8 - Math.Min(bitCount, _subBitCount)));
						if (bitCount > _subBitCount)
						{
							_subBitBuffer = (byte)Math.Max(_source.ReadByte(), 0);
							bitCount -= _subBitCount;
							_subBitCount = 8;
						}
						else
						{
							_subBitBuffer <<= bitCount;
							_subBitCount -= (byte)bitCount;
							return;
						}
					}
				}

				public ushort Peek(int bitCount) { return (ushort)(_bitBuffer >> (16 - bitCount)); }

				public ushort Read(int bitCount)
				{
					var x = Peek(bitCount);
					FillBuffer(bitCount);
					return x;
				}
			}

			sealed class Pointer<T>
			{
				public Pointer(IList<T> array, int index)
				{
					_array = array;
					_index = index;
				}

				IList<T> _array;
				int _index;

				public T Value
				{
					get { return _array[_index]; }
					set { _array[_index] = value; }
				}
			}

			sealed class DecoderPair
			{
				public DecoderPair(IReadOnlyList<ushort> table, IReadOnlyList<byte> lengths)
				{
					_table = table;
					_lengths = lengths;
					_tableBitCount = 31 - CommonUtils.CountLeadingZero((uint)table.Count);
				}

				IReadOnlyList<ushort> _table;
				IReadOnlyList<byte> _lengths;
				int _tableBitCount;

				public ushort Decode(BitStream source, IReadOnlyList<ushort> left, IReadOnlyList<ushort> right)
				{
					var data = _table[source.Peek(_tableBitCount)];
					if (data < _lengths.Count)
						source.Read(_lengths[data]);
					else
					{
						source.Read(_tableBitCount);
						for (var mask = 1 << (16 - 1); data >= _lengths.Count; mask >>= 1)
							data = (source.Peek(16) & mask) != 0 ? right[data] : left[data];
						source.Read(_lengths[data] - _tableBitCount);
					}
					return data;
				}
			}

			sealed class SlidingWindow : IDisposable
			{
				public SlidingWindow(Stream destination, int dictionarySize)
				{
					_dest = destination;
					_dictionary = new byte[dictionarySize];
				}

				~SlidingWindow() { Dispose(false); }

				Stream _dest;
				byte[] _dictionary;
				int _writing;
				
				public void Dispose()
				{
					Dispose(true);
					GC.SuppressFinalize(this);
				}

				void Dispose(bool disposing)
				{
					if (_dest != null)
					{
						if (_writing > 0)
							_dest.Write(_dictionary, 0, _dictionary.Length);
						_dest = null;
					}
				}

				public void Write(byte data)
				{
					_dictionary[_writing++] = data;
					if (_writing == _dictionary.Length)
					{
						_dest.Write(_dictionary, 0, _dictionary.Length);
						_writing = 0;
					}
				}

				public int WriteDuplicate(int disp, int count)
				{
					var index = _writing - disp - 1;
					if (index < 0)
						index += _dictionary.Length;
					for (int i = 0; i < count; i++)
					{
						Write(_dictionary[index++]);
						if (index == _dictionary.Length)
							index = 0;
					}
					return count;
				}
			}

			// codeLengths: symbol -> length of code
			// symbols: code -> symbol
			static ushort[] MakeTable(IReadOnlyList<byte> codeLengths, byte tableBitCount, /*W*/ushort[] left, /*W*/ushort[] right)
			{
				ushort[] count = new ushort[17];
				foreach (var length in codeLengths)
					count[length]++;
				ushort total = 0;
				ushort[] start = new ushort[17];
				for (int i = 1; i <= 16; i++)
				{
					start[i] = (ushort)(i <= tableBitCount ? total >> (16 - tableBitCount) : total);
					total += (ushort)((1 << (16 - i)) * count[i]);
				}
				System.Diagnostics.Debug.Assert(total == 0);
				ushort[] symbols = new ushort[1 << tableBitCount];
				var availableLinkId = (ushort)codeLengths.Count;
				for (ushort symbol = 0; symbol < codeLengths.Count; symbol++)
				{
					if (codeLengths[symbol] == 0)
						continue;
					ushort weight;
					if (codeLengths[symbol] <= tableBitCount)
					{
						weight = (ushort)(1 << (tableBitCount - codeLengths[symbol]));
						for (int j = 0; j < weight; j++)
							symbols[start[codeLengths[symbol]] + j] = symbol;
					}
					else
					{
						weight = (ushort)(1 << (16 - codeLengths[symbol]));
						var pointer = new Pointer<ushort>(symbols, start[codeLengths[symbol]] >> (16 - tableBitCount));
						for (int j = tableBitCount; j < codeLengths[symbol]; j++)
						{
							if (pointer.Value == 0)
							{
								pointer.Value = availableLinkId++;
								right[pointer.Value] = left[pointer.Value] = 0;
							}
							pointer = new Pointer<ushort>((start[codeLengths[symbol]] & (1 << (16 - j - 1))) != 0 ? right : left, pointer.Value);
						}
						pointer.Value = symbol;
					}
					start[codeLengths[symbol]] += weight;
				}
				return symbols;
			}

			static DecoderPair ReadPointerLength(BitStream source, /*W*/ushort[] left, /*W*/ushort[] right, ushort pointerLengthCount, int i_special)
			{
				byte[] pointerLength = new byte[pointerLengthCount];
				int bitCount = 32 - CommonUtils.CountLeadingZero(pointerLengthCount);
				var n = source.Read(bitCount);
				if (n == 0)
					return new DecoderPair(Enumerable.Repeat(source.Read(bitCount), 1 << 8).ToArray(), pointerLength);
				for (ushort i = 0; i < n; )
				{
					var c = source.Peek(3);
					if (c == 7)
					{
						for (ushort mask = 1 << (16 - 4); (mask & source.Peek(16)) != 0; mask >>= 1)
							c++;
					}
					source.Read(c < 7 ? 3 : c - 3);
					pointerLength[i++] = (byte)c;
					if (i == i_special)
					{
						c = source.Read(2);
						while (c-- > 0)
							pointerLength[i++] = 0;
					}
				}
				return new DecoderPair(MakeTable(pointerLength, 8, left, right), pointerLength);
			}

			static DecoderPair ReadCodeLength(BitStream source, DecoderPair pointer, /*W*/ushort[] left, /*W*/ushort[] right)
			{
				byte[] codeLength = new byte[510];
				var n = source.Read(9);
				if (n == 0)
					return new DecoderPair(Enumerable.Repeat(source.Read(9), 1 << 12).ToArray(), codeLength);
				for (ushort i = 0; i < n; )
				{
					var c = pointer.Decode(source, left, right);
					if (c <= 2)
					{
						if (c == 0)
							c = 1;
						else if (c == 1)
							c = (ushort)(source.Read(4) + 3);
						else
							c = (ushort)(source.Read(9) + 20);
						while (c-- > 0)
							codeLength[i++] = 0;
					}
					else
						codeLength[i++] = (byte)(c - 2);
				}
				return new DecoderPair(MakeTable(codeLength, 12, left, right), codeLength);
			}

			public static void Decode(LzhMethod method, Stream source, Stream destination, uint originalSize)
			{
                if (method == LzhMethod.LH0)
                {
					byte[] buffer = new byte[32768];
					int bytesRead;
					while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
						destination.Write(buffer, 0, bytesRead);
                    return;
                }
				ushort pointerCount;
				int dictionarySize;
				switch (method)
				{
					case LzhMethod.LH4: pointerCount = 14; dictionarySize = 1 << 12; break;
					case LzhMethod.LH5: pointerCount = 14; dictionarySize = 1 << 13; break;
					case LzhMethod.LH6: pointerCount = 16; dictionarySize = 1 << 15; break;
					case LzhMethod.LH7: pointerCount = 17; dictionarySize = 1 << 16; break;
					default: return;
				}
				using (SlidingWindow window = new SlidingWindow(destination, dictionarySize))
				{
					var src = new BitStream(source);
					ushort blockSize = 0;
					DecoderPair pointers = null;
					DecoderPair codes = null;
					ushort[] right = new ushort[1019];
					ushort[] left = new ushort[1019];
					for (uint count = 0; count < originalSize; )
					{
						if (blockSize == 0)
						{
							blockSize = src.Read(16);
							codes = ReadCodeLength(src, ReadPointerLength(src, left, right, 19, 3), left, right);
							pointers = ReadPointerLength(src, left, right, pointerCount, -1);
						}
						blockSize--;
						var code = codes.Decode(src, left, right);
						if (code <= byte.MaxValue)
						{
							window.Write((byte)code);
							count++;
						}
						else
						{
							int reading = pointers.Decode(src, left, right);
							if (reading != 0)
								reading = (ushort)((1 << (reading - 1)) + src.Read(reading - 1));
							count += (uint)window.WriteDuplicate(reading, code - 0x100 + 3);
						}
					}
				}
			}
		}

		sealed class SubReadStream : Stream
		{
			readonly long _startInSuperStream;
			long _positionInSuperStream;
			readonly long _endInSuperStream;
			readonly Stream _superStream;
			bool _isDisposed;

			public override long Length { get { return _endInSuperStream - _startInSuperStream; } }

			public override long Position
			{
				get { return _positionInSuperStream - _startInSuperStream; }
				set { throw new NotSupportedException(); }
			}

			public override bool CanRead { get { return _superStream.CanRead; } }

			public override bool CanSeek { get { return false; } }

			public override bool CanWrite { get { return false; } }

			public SubReadStream(Stream superStream, long startPosition, long maxLength)
			{
				_startInSuperStream = startPosition;
				_positionInSuperStream = startPosition;
				_endInSuperStream = startPosition + maxLength;
				_superStream = superStream;
				_isDisposed = false;
			}

			void ThrowIfDisposed()
			{
				if (_isDisposed)
					throw new ObjectDisposedException(base.GetType().Name);
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				ThrowIfDisposed();
				if (_superStream.Position != _positionInSuperStream)
					_superStream.Seek(_positionInSuperStream, SeekOrigin.Begin);
				if (_positionInSuperStream + count > _endInSuperStream)
					count = (int)(_endInSuperStream - _positionInSuperStream);
				int readBytes = _superStream.Read(buffer, offset, count);
				_positionInSuperStream += (long)readBytes;
				return readBytes;
			}

			public override long Seek(long offset, SeekOrigin origin) { throw new NotSupportedException(); }

			public override void SetLength(long value) { throw new NotSupportedException(); }

			public override void Write(byte[] buffer, int offset, int count) { throw new NotSupportedException(); }

			public override void Flush() { throw new NotSupportedException(); }

			protected override void Dispose(bool disposing)
			{
				if (disposing && !_isDisposed)
					_isDisposed = true;
				base.Dispose(disposing);
			}
		}

		sealed class HeaderInfo
		{
			public string RelativeFilePath = string.Empty;
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
			LH4,
			LH5,
			LH6,
			LH7,
			LHD,
		}

		public static bool Melt(string arc, string extractTo, params string[] files)
		{
			using (var fs = new FileStream(arc, FileMode.Open, FileAccess.Read))
			{
				var info = FindHeader(fs);
				if (info == null)
					return false;
				do
				{
					var basePosition = fs.Position;
					var fileName = Path.GetFileName(info.RelativeFilePath);
					if (fileName.Length > 0 && fileName[0] != '!' && fileName[0] != '$' && fileName[0] != '%' && files.Contains(fileName, StringComparer.CurrentCultureIgnoreCase))
					{
						var filePath = Path.GetFullPath(Path.Combine(extractTo, fileName));
						using (var dest = new FileStream(filePath, FileMode.Create, FileAccess.Write))
							LzhDecoder.Decode(info.Method, new SubReadStream(fs, basePosition, info.CompressedSize), dest, info.OriginalSize);
						File.SetAttributes(filePath, info.FileAttribute);
						File.SetCreationTimeUtc(filePath, info.UpdatedTime);
						File.SetLastWriteTimeUtc(filePath, info.UpdatedTime);
					}
					fs.Seek(basePosition + info.CompressedSize, SeekOrigin.Begin);
				} while ((info = ReadHeader(fs)) != null);
				return true;
			}
		}

		static IReadOnlyList<ushort> CreateCrcTable()
		{
			ushort[] table = new ushort[256];
			for (ushort i = 0; i < table.Length; i++)
			{
				table[i] = i;
				for (int j = 0; j < 8; j++)
					table[i] = (ushort)((table[i] & 1) != 0 ? (table[i] >> 1) ^ 0xA001 : table[i] >> 1);
			}
			return table;
		}

		static readonly IReadOnlyList<ushort> _crcTable = CreateCrcTable();

		static int GetBits(uint value, int startBit, int bitCount) { return (int)((value >> startBit) & ((1 << bitCount) - 1)); }

		static bool IsProperMethod(byte[] buffer, int index) { return buffer[index] == '-' && buffer[index + 1] == 'l' && (buffer[index + 2] == 'h' || buffer[index + 2] == 'z') && buffer[index + 4] == '-'; }

		static HeaderInfo FindHeader(Stream stream)
		{
			byte[] buffer = new byte[65536];
			var size = stream.Read(buffer, 0, buffer.Length);
			for (int i = 0; i < size - 20; i++)
			{
				if (IsProperMethod(buffer, i + 2))
				{
					stream.Seek(i, SeekOrigin.Begin);
					var info = ReadHeader(stream);
					if (info != null)
						return info;
				}
			}
			return null;
		}

		static HeaderInfo ReadHeader(Stream stream)
		{
			byte[] buffer = new byte[65536];
			if (stream.Read(buffer, 0, 21) != 21)
				return null;
			var level = buffer[20];
			var headerSize = (ushort)(level >= 2 ? 26 : buffer[0] + 2);
			if (level > 2 || headerSize < 21 || buffer[0] == 0 || !IsProperMethod(buffer, 2) || stream.Read(buffer, 21, headerSize - 21) != headerSize - 21)
				return null;
			var info = new HeaderInfo();
			var shiftJis = Encoding.GetEncoding(932);
			{
				uint date = BitConverter.ToUInt16(buffer, 17);
				uint time = BitConverter.ToUInt16(buffer, 15);
				if (level < 2)
				{
					if (buffer.Skip(2).Take(headerSize - 2).Aggregate((x, y) => unchecked((byte)(x + y))) != buffer[1] || headerSize - 21 < 1 + buffer[21] + 2)
						return null;
					info.RelativeFilePath = shiftJis.GetString(buffer, 22, buffer[21]);
					info.UpdatedTime = new DateTime(GetBits(date, 9, 7) + 1980, GetBits(date, 5, 4), GetBits(date, 0, 5), GetBits(time, 11, 5), GetBits(time, 5, 6), GetBits(time, 0, 5) * 2);
				}
				else
					info.UpdatedTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(time + (date << 16));
				info.CompressedSize = BitConverter.ToUInt32(buffer, 7);
				info.OriginalSize = BitConverter.ToUInt32(buffer, 11);
				info.FileAttribute = (FileAttributes)buffer[19];
				if (!Enum.TryParse(Encoding.ASCII.GetString(buffer, 3, 3), true, out info.Method))
					info.Method = LzhMethod.Unknown;
			}
			if (level > 0)
			{
				var directoryName = string.Empty;
				uint? expectedCrc = null;
				ushort crc = 0;
				while (true)
				{
					for (int i = 0; i < headerSize; i++)
						crc = (ushort)(_crcTable[(crc & 0xFF) ^ buffer[i]] ^ (crc >> 8));
					headerSize = BitConverter.ToUInt16(buffer, headerSize - 2);
					if (headerSize <= 2)
					{
						if (expectedCrc != null && crc != expectedCrc)
							return null;
						break;
					}
					if (stream.Read(buffer, 0, headerSize) != headerSize)
						return null;
					if (level == 1)
						info.CompressedSize -= headerSize;
					if (buffer[0] == 0x00 && expectedCrc == null) // Common Extended Header
					{
						expectedCrc = BitConverter.ToUInt16(buffer, 1);
						buffer[1] = buffer[2] = 0;
					}
					else if (buffer[0] == 0x01) // File Name Extended Header
						info.RelativeFilePath = shiftJis.GetString(buffer, 1, Math.Min(headerSize - 3, 260));
					else if (buffer[0] == 0x02) // Directory Name Extended Header
					{
						var newBuffer = new byte[Math.Min(headerSize - 3, 260)];
						Array.Copy(buffer, 1, newBuffer, 0, newBuffer.Length);
						for (int i = 0; i < newBuffer.Length; i++)
						{
							if (newBuffer[i] == 0xFF)
								newBuffer[i] = (byte)'/';
						}
						directoryName = shiftJis.GetString(newBuffer);
					}
					else if (buffer[0] == 0x40) // MS-DOS File Attribute Extended Header
						info.FileAttribute = (FileAttributes)BitConverter.ToUInt16(buffer, 1);
				}
				info.RelativeFilePath = directoryName + info.RelativeFilePath;
			}
			return info;
		}
	}
}
