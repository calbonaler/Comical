using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Comical.Archivers.Manager
{
	public sealed class LzhExtractor
	{
		sealed class LzhDecoder
		{
			FileStream src;
			FileStream dest;
			uint cmpsize;
			uint orisize;
			int np;
			int pbit;
			ushort bitbuf;
			byte subbitbuf;
			byte bitcount;
			ushort blocksize;
			uint loc;
			byte[] pt_len = new byte[128];
			byte[] c_len = new byte[510];
			ushort[] pt_table = new ushort[256];
			ushort[] c_table = new ushort[4096];
			ushort[] right = new ushort[1019];
			ushort[] left = new ushort[1019];

			void Unstore()
			{
				const int UNS_BUFSIZE = 32768;
				int hm;
				byte[] buf = new byte[UNS_BUFSIZE];
				if (cmpsize == uint.MaxValue)
				{
					while ((hm = src.Read(buf, 0, buf.Length)) != 0)
					{
						if (hm == -1)
							break;
						dest.Write(buf, 0, hm);
					}
					return;
				}
				while (cmpsize > 0)
				{
					hm = Math.Min(UNS_BUFSIZE, (int)cmpsize);
					if (0 >= (hm = src.Read(buf, 0, hm)))
						break;
					dest.Write(buf, 0, hm);
					cmpsize -= (uint)hm;
				}
			}

			ushort DecodeC(LzhMethod method)
			{
				if ((int)method >= 4) return DecodeCSt1();
				return 0;
			}

			ushort DecodeCSt1()
			{
				ushort j, mask;
				if (blocksize == 0)
				{
					blocksize = GetBits(16);
					ReadPtLen(19, 5, 3);
					ReadCLen();
					ReadPtLen((short)np, (short)pbit, -1);
				}
				blocksize--;
				j = c_table[bitbuf >> 4];
				if (j < 255 + 255)
					FillBuf(c_len[j]);
				else
				{
					FillBuf(12);
					mask = 1 << (16 - 1);
					do
					{
						if ((bitbuf & mask) != 0) j = right[j];
						else j = left[j];
						mask >>= 1;
					} while (j >= 255 + 255);
					FillBuf((byte)(c_len[j] - 12));
				}
				return j;
			}

			ushort DecodeP(LzhMethod method)
			{
				if ((int)method >= 4) return DecodePSt1();
				return 0;
			}

			ushort DecodePSt1()
			{
				ushort j, mask;

				j = pt_table[bitbuf >> (16 - 8)];
				if (j < np)
					FillBuf(pt_len[j]);
				else
				{
					FillBuf(8);
					mask = 1 << (16 - 1);
					do
					{
						j = (bitbuf & mask) != 0 ? right[j] : left[j];
						mask >>= 1;
					} while (j >= np);
					FillBuf((byte)(pt_len[j] - 8));
				}
				if (j != 0)
					j = (ushort)((1 << (j - 1)) + GetBits((byte)(j - 1)));
				return j;
			}

			void MakeTable(ushort nchar, byte[] bitlen, ushort tablebits, ushort[] table)
			{
				ushort[] count = new ushort[17];
				ushort[] weight = new ushort[17];
				ushort[] start = new ushort[17];
				ushort total;
				int j, k, l, m, n, avail;
				Func<bool, int, int> p;

				avail = nchar;

				for (int i = 1; i <= 16; i++)
				{
					count[i] = 0;
					weight[i] = (ushort)(1 << (16 - i));
				}

				for (int i = 0; i < nchar; i++)
					count[bitlen[i]]++;

				total = 0;
				for (int i = 1; i <= 16; i++)
				{
					start[i] = total;
					total += (ushort)(weight[i] * count[i]);
				}
				if ((total & 0xFFFF) != 0)
					return;

				m = 16 - tablebits;
				for (int i = 1; i <= tablebits; i++)
				{
					start[i] >>= m;
					weight[i] >>= m;
				}

				j = start[tablebits + 1] >> m;
				k = 1 << tablebits;
				if (j != 0)
				{
					for (int i = 0; i < k; i++)
						table[i] = 0;
				}

				for (j = 0; j < nchar; j++)
				{
					k = bitlen[j];
					if (k == 0)
						continue;
					l = start[k] + weight[k];
					if (k <= tablebits)
					{
						for (int i = start[k]; i < l; i++)
							table[i] = (ushort)j;
					}
					else
					{
						uint i = start[k];
						p = (a, b) => !a ? table[start[k] >> m] : (table[start[k] >> m] = (ushort)b);
						i <<= tablebits;
						n = k - tablebits;
						while (--n >= 0)
						{
							if (p(false, 0) == 0)
							{
								right[avail] = left[avail] = 0;
								p(true, avail++);
							}
							int pv = p(false, 0);
							if ((i & 0x8000) != 0)
								p = (a, b) => !a ? right[pv] : (right[pv] = (ushort)b);
							else
								p = (a, b) => !a ? left[pv] : (left[pv] = (ushort)b);
							i <<= 1;
						}
						p(true, j);
					}
					start[k] = (ushort)l;
				}
			}

			void ReadPtLen(short nn, short nbit, short i_special)
			{
				short c;
				short n = (short)GetBits((byte)nbit);

				if (n == 0)
				{
					c = (short)GetBits((byte)nbit);
					for (short i = 0; i < nn; i++) pt_len[i] = 0;
					for (short i = 0; i < 256; i++) pt_table[i] = (ushort)c;
				}
				else
				{
					short i = 0;
					while (i < n)
					{
						c = (short)(bitbuf >> (16 - 3));
						if (c == 7)
						{
							ushort mask = 1 << (16 - 4);
							while ((mask & bitbuf) != 0)
							{
								mask >>= 1;
								c++;
							}
						}
						FillBuf((byte)(c < 7 ? 3 : c - 3));
						pt_len[i++] = (byte)c;
						if (i == i_special)
						{
							c = (short)GetBits(2);
							while (--c >= 0)
								pt_len[i++] = 0;
						}
					}
					while (i < nn)
						pt_len[i++] = 0;
					MakeTable((ushort)nn, pt_len, 8, pt_table);
				}
			}

			void ReadCLen()
			{
				short c, n = (short)GetBits(9);

				if (n == 0)
				{
					c = (short)GetBits(9);
					Array.Clear(c_len, 0, c_len.Length);
					for (short i = 0; i < 4096; i++) c_table[i] = (ushort)c;
				}
				else
				{
					short i = 0;
					while (i < n)
					{
						c = (short)pt_table[bitbuf >> (16 - 8)];
						if (c >= 19)
						{
							ushort mask = 1 << (16 - 9);
							do
							{
								c = (short)((bitbuf & mask) != 0 ? right[c] : left[c]);
								mask >>= 1;
							} while (c >= 19);
						}
						FillBuf(pt_len[c]);
						if (c <= 2)
						{
							if (c == 0) c = 1;
							else if (c == 1) c = (short)(GetBits(4) + 3);
							else c = (short)(GetBits(9) + 20);
							while (--c >= 0)
								c_len[i++] = 0;
						}
						else
							c_len[i++] = (byte)(c - 2);
					}
					while (i < (255 + 255))
						c_len[i++] = 0;
					MakeTable(255 + 255, c_len, 12, c_table);
				}
			}

			public void Decode(LzhMethod method, FileStream srcFile, uint srcSize, FileStream destFile, uint destSize)
			{
				src = srcFile;
				cmpsize = srcSize;
				dest = destFile;
				orisize = destSize;

				uint dicsiz, dicbit;

				switch (method)
				{
					case LzhMethod.LH0: Unstore(); return;
					case LzhMethod.LH4: np = 14; pbit = 4; dicbit = 12; dicsiz = (uint)(1 << (int)dicbit); break;
					case LzhMethod.LH5: np = 14; pbit = 4; dicbit = 13; dicsiz = (uint)(1 << (int)dicbit); break;
					case LzhMethod.LH6: np = 16; pbit = 5; dicbit = 15; dicsiz = (uint)(1 << (int)dicbit); break;
					case LzhMethod.LH7: np = 17; pbit = 5; dicbit = 16; dicsiz = (uint)(1 << (int)dicbit); break;
					default: return;
				}

				byte[] text = new byte[dicsiz];
				for (int i = 0; i < text.Length; i++) text[i] = 0x20;

				InitGetBits();
				blocksize = 0;

				uint count = 0;
				loc = 0;
				int offset = 0x0100 - 3;
				while (count < orisize)
				{
					int c = DecodeC(method);
					if (c <= 255)
					{
						text[loc++] = (byte)c;
						if (loc == dicsiz)
						{
							dest.Write(text, 0, (int)dicsiz);
							loc = 0;
						}
						count++;
					}
					else
					{
						int j = c - offset;
						count += (uint)j;
						int i = DecodeP(method);
						if ((i = (int)(loc - i - 1)) < 0)
							i += (int)dicsiz;

						for (int k = 0; k < j; k++)
						{
							text[loc++] = text[i];
							if (loc >= dicsiz)
							{
								dest.Write(text, 0, (int)dicsiz);
								loc = 0;
							}
							if (++i >= (int)dicsiz)
								i = 0;
						}
					}
				}

				if (loc != 0)
					dest.Write(text, 0, (int)loc);
			}

			void InitGetBits()
			{
				bitbuf = 0;
				subbitbuf = 0;
				bitcount = 0;
				FillBuf(16);
			}

			void FillBuf(byte n)
			{
				while (n > bitcount)
				{
					n -= bitcount;
					bitbuf = (ushort)((bitbuf << bitcount) + (subbitbuf >> (8 - bitcount)));
					if (cmpsize != 0)
					{
						cmpsize--;
						subbitbuf = (byte)src.ReadByte();
					}
					else
						subbitbuf = 0;
					bitcount = 8;
				}
				bitcount -= n;
				bitbuf = (ushort)((bitbuf << n) + (subbitbuf >> (8 - n)));
				subbitbuf <<= n;
			}

			ushort GetBits(byte n)
			{
				ushort x = (ushort)(bitbuf >> (int)(16 - n));
				FillBuf(n);
				return x;
			}
		}

		enum LzhMethod
		{
			Unknown = -1,
			LH0 = 0,
			LH1,
			LH2,
			LH3,
			LH4,
			LH5,
			LH6,
			LH7,
		}

		public static string[] Melt(string arc, string extractTo, params string[] files) { return new LzhExtractor().Extract(arc, extractTo, files); }

		LzhExtractor() { InitCRCTable(); }

		static void InitCRCTable()
		{
			for (ushort i = 0; i < crctable.Length; i++)
			{
				ushort r = i;
				for (ushort j = 0; j < 8; j++)
					r = (ushort)((r & 1) != 0 ? (r >> 1) ^ 0xA001 : r >> 1);
				crctable[i] = r;
			}
		}

		ushort crc;
		byte sum;
		byte h_Level;
		string h_FileName = "";
		string h_Method = "";
		uint h_CompSize;
		uint h_OrigSize;
		ushort h_Attrib;
		uint h_Update;
		static ushort[] crctable = new ushort[256];

		string[] Extract(string arcname, string extractTo, string[] files)
		{
			using (FileStream fs = new FileStream(arcname, FileMode.Open, FileAccess.Read))
			{
				List<string> paths = new List<string>();
				byte[] buffer = new byte[65536];
				uint siz = (uint)fs.Read(buffer, 0, buffer.Length);
				int ps = FindHeader(fs, buffer, siz);
				if (ps == -1)
					return paths.ToArray();
				fs.Seek(ps, SeekOrigin.Begin);
				while (ReadHeader(fs, buffer))
				{
					long bas = fs.Position;
					string name = Path.GetFullPath(System.IO.Path.Combine(extractTo, h_FileName));
					if (h_FileName[0] != '!' && h_FileName[0] != '$' && h_FileName[0] != '%' &&
						files.Contains(h_FileName, StringComparer.Create(System.Globalization.CultureInfo.CurrentCulture, true)))
					{
						using (FileStream dest = new FileStream(name, FileMode.Create, FileAccess.Write))
						{
							paths.Add(name);
							LzhDecoder dec = new LzhDecoder();
							LzhMethod method = LzhMethod.Unknown;
							if (h_Method == "-lh0-") method = LzhMethod.LH0;
							else if (h_Method == "-lh5-") method = LzhMethod.LH5;
							else if (h_Method == "-lh6-") method = LzhMethod.LH6;
							else if (h_Method == "-lh7-") method = LzhMethod.LH7;
							dec.Decode(method, fs, h_CompSize, dest, h_OrigSize);
						}

						File.SetAttributes(name, (FileAttributes)h_Attrib);
						DateTime dt;
						if (h_Level < 2)
							dt = GetDateTime((h_Update >> 16) & 0xFFFF, h_Update & 0xFFFF);
						else
						{
							dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
							dt = dt.AddSeconds(h_Update);
						}
						File.SetCreationTimeUtc(name, dt);
						File.SetLastWriteTimeUtc(name, dt);
					}
					fs.Seek(bas + h_CompSize, SeekOrigin.Begin);
				}
				return paths.ToArray();
			}
		}

		static DateTime GetDateTime(uint date, uint time) { return new DateTime(GetBits(date, 9, 7) + 1980, GetBits(date, 5, 4), GetBits(date, 0, 5), GetBits(time, 11, 5), GetBits(time, 5, 6), GetBits(time, 0, 5) * 2); }

		static int GetBits(uint value, int startBit, int bitCount) { return (int)((value >> startBit) & ((1 << bitCount) - 1)); }

		int FindHeader(FileStream fs, byte[] buffer, uint size)
		{
			byte[] temp = new byte[65536];
			int ans = -1;
			for (uint i = 0; i < size - 20; i++)
			{
				if (buffer[i + 2] == '-' && buffer[i + 3] == 'l' &&
					(buffer[i + 4] == 'h' || buffer[i + 4] == 'z'))
				{
					fs.Seek(i, SeekOrigin.Begin);
					if (ReadHeader(fs, temp))
					{
						ans = (int)i;
						break;
					}
				}
			}
			return ans;
		}

		bool ReadHeader(FileStream fs, byte[] buffer)
		{
			Encoding sjis = Encoding.GetEncoding("Shift-JIS");
			sum = 0;
			crc = 0;
			if (21 != ReadCrc(fs, buffer, 21, 0))
				return false;
			sum -= (byte)(buffer[0] + buffer[1]);
			if ((h_Level = buffer[20]) > 2)
				return false;
			byte bshdr = h_Level == 2 ? (byte)26 : (byte)(buffer[0] + 2);
			if (bshdr < 21 || buffer[0] == 0)
				return false;
			byte hdrsum = buffer[1];
			h_Method = sjis.GetString(buffer, 2, 5);
			h_CompSize = (uint)(buffer[7] + (buffer[8] << 8) + (buffer[9] << 16) + (buffer[10] << 24));
			h_OrigSize = (uint)(buffer[11] + (buffer[12] << 8) + (buffer[13] << 16) + (buffer[14] << 24));
			h_Update = (uint)(buffer[15] + (buffer[16] << 8) + (buffer[17] << 16) + (buffer[18] << 24));
			h_Attrib = buffer[19];
			if (h_Method[0] != '-' || h_Method[1] != 'l')
				return false;
			if (bshdr - 21 != ReadCrc(fs, buffer, bshdr - 21, 21))
				return false;
			if (h_Level != 2)
			{
				if (sum != hdrsum || 21 + 1 + buffer[21] + 2 > bshdr)
					return false;
				h_FileName = sjis.GetString(buffer, 22, buffer[21]);
			}
			string PathName = "";
			if (h_Level != 0)
			{
				uint hdrcrc = 0xFFFFFFFF;
				ushort tmpcrc;
				ushort ehs = (ushort)(buffer[bshdr - 2] | (buffer[bshdr - 1] << 8));
				while (ehs > 2)
				{
					tmpcrc = crc;
					if (ehs != ReadCrc(fs, buffer, ehs, 0))
						return false;
					if (h_Level == 1)
						h_CompSize -= ehs;
					switch (buffer[0])
					{
						case 0x00:
							if (ehs >= 5 && hdrcrc == 0xFFFFFFFF)
							{
								hdrcrc = (ushort)(buffer[1] | (buffer[2] << 8));
								crc = tmpcrc;
								buffer[1] = buffer[2] = 0;
								UpdateCrc(buffer, ehs, 0);
							}
							break;
						case 0x01:
							h_FileName = sjis.GetString(buffer, 1, Math.Min(ehs - 3, 260));
							break;
						case 0x02:
							PathName = sjis.GetString(buffer, 1, Math.Min(ehs - 3, 260));
							break;
						case 0x40:
							if (ehs >= 5)
								h_Attrib = (ushort)(buffer[1] | (buffer[2] << 8));
							break;
					}
					ehs = (ushort)(buffer[ehs - 2] | (buffer[ehs - 1] << 8));
				}
				if (hdrcrc != 0xFFFFFFFF && crc != hdrcrc)
					return false;
			}
			h_FileName = PathName + h_FileName;
			return true;
		}

		void UpdateCrc(byte[] p, int n, int offset)
		{
			for (int i = 0; i < n; i++)
				crc = (ushort)(crctable[(crc ^ p[i + offset]) & 0xFF] ^ (crc >> 8));
		}

		void UpdateSum(byte[] p, int n, int offset) { sum += (byte)Enumerable.Range(offset, n + offset).Select(i => p[i]).Sum(b => (int)b); }

		int ReadCrc(FileStream fs, byte[] p, int n, int offset)
		{
			try { n = fs.Read(p, offset, n); }
			catch (IOException) { }
			UpdateCrc(p, n, offset);
			UpdateSum(p, n, offset);
			return n;
		}
	}
}
