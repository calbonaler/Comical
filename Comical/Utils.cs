using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Comical
{
	static class Utils
	{
		static class NativeMethods
		{
			[DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
			public static extern int SetWindowTheme(IntPtr windowHandle, string appName, string idList);

			[DllImport("DwmApi.dll")]
			public static extern int DwmExtendFrameIntoClientArea(IntPtr hwnd, ref NativeMargins m);
		}

		[StructLayout(LayoutKind.Sequential)]
		struct NativeMargins
		{
			int LeftWidth;
			int RightWidth;
			int TopHeight;
			int BottomHeight;
			public NativeMargins(bool fullWindow) { LeftWidth = RightWidth = TopHeight = BottomHeight = (fullWindow ? -1 : 0); }
		};

		[ComImport, Guid("275c23e2-3747-11d0-9fea-00aa003f8646")]
		class CMultiLanguage { }

		[ComImport, Guid("DCCFC164-2B38-11d2-B7EC-00C04F8F5D9A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
		interface IMultiLanguage2
		{
			void GetNumberOfCodePageInf([Out]out uint pcCodePage);
			void GetCodePageInfo(uint uiCodePage, ushort LangId, [Out]IntPtr pCodePageInfo);
			void GetFamilyCodePage(uint uiCodePage, [Out]out uint puiFamilyCodePage);
			void EnumCodePage(uint grfFlags, ushort LangId, [Out, MarshalAs(UnmanagedType.Interface)]out object ppEnumCodePage);
			void GetCharsetInfo([MarshalAs(UnmanagedType.BStr)]string Charset, [Out]IntPtr pCharsetInfo);
			[PreserveSig]
			int IsConvertible(uint dwSrcEncoding, uint dwDstEncoding);
			void ConvertString([In, Out]ref uint pdwMode, uint dwSrcEncoding, uint dwDstEncoding,
				IntPtr pSrcStr, [In, Out]ref uint pcSrcSize, [Out]IntPtr pDstStr, [In, Out]ref uint pcDstSize);
			void ConvertStringToUnicode([In, Out]ref uint pdwMode, uint dwEncoding,
				IntPtr pSrcStr, [In, Out]ref uint pcSrcSize, [Out]IntPtr pDstStr, [In, Out]ref uint pcDstSize);
			void ConvertStringFromUnicode([In, Out]ref uint pdwMode, uint dwEncoding,
				IntPtr pSrcStr, [In, Out]ref uint pcSrcSize, [Out]IntPtr pDstStr, [In, Out]ref uint pcDstSize);
			void ConvertStringReset();
			void GetRfc1766FromLcid(uint Locale, [Out, MarshalAs(UnmanagedType.BStr)]StringBuilder pbstrRfc1766);
			void GetLcidFromRfc1766([Out]out uint pLocale, [MarshalAs(UnmanagedType.BStr)]string bstrRfc1766);
			void EnumRfc1766(ushort LangId, [Out, MarshalAs(UnmanagedType.Interface)]out object ppEnumRfc1766);
			void GetRfc1766Info(uint Locale, ushort LangId, [Out]IntPtr pRfc1766Info);
			void CreateConvertCharset(uint uiSrcCodePage, uint uiDstCodePage, uint dwProperty, [Out, MarshalAs(UnmanagedType.Interface)]out object ppMLangConvertCharset);
			void ConvertStringInIStream([In, Out]ref uint pdwMode, uint dwFlag, [MarshalAs(UnmanagedType.LPWStr)]string lpFallBack, uint dwSrcEncoding, uint dwDstEncoding,
				[MarshalAs(UnmanagedType.Interface)]object pstmIn, [MarshalAs(UnmanagedType.Interface)]object pstmOut);
			void ConvertStringToUnicodeEx([In, Out]ref uint pdwMode, uint dwEncoding,
				[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]byte[] pSrcStr, [In, Out]ref uint pcSrcSize,
				[Out, MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeParamIndex = 5)]char[] pDstStr, [In, Out]ref uint pcDstSize,
				uint dwFlag, [MarshalAs(UnmanagedType.LPWStr)]string lpFallBack);
			void ConvertStringFromUnicodeEx([In, Out]ref uint pdwMode, uint dwEncoding,
				[MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.U2, SizeParamIndex = 3)]char[] pSrcStr, [In, Out]ref uint pcSrcSize,
				[Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 5)]byte[] pDstStr, [In, Out]ref uint pcDstSize,
				uint dwFlag, [MarshalAs(UnmanagedType.LPWStr)]string lpFallBack);
			void DetectCodepageInIStream(IncomingDataTypes dwFlag, uint dwPrefWinCodePage, [MarshalAs(UnmanagedType.Interface)]object pstmIn,
				[Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 4)]DetectEncodingInfo[] lpEncoding, [In, Out]ref int pnScores);
			[PreserveSig]
			int DetectInputCodepage(
				[In]IncomingDataTypes dwFlag,
				[In]uint dwPrefWinCodePage,
				[In]IntPtr pSrcStr,
				[In, Out]ref int pcSrcSize,
				[In, Out, MarshalAs(UnmanagedType.LPArray)]DetectEncodingInfo[] lpEncoding,
				[In, Out]ref int pnScores);
			[PreserveSig]
			int ValidateCodePage(uint uiCodePage, IntPtr hwnd);
			void GetCodePageDescription(uint uiCodePage, uint lcid, [Out, MarshalAs(UnmanagedType.LPWStr)]StringBuilder lpWideCharStr, int cchWideChar);
			[PreserveSig]
			int IsCodePageInstallable(uint uiCodePage);
			void SetMimeDBSource(uint dwSource);
			void GetNumberOfScripts([Out]out uint pnScripts);
			void EnumScripts(uint dwFlags, ushort LangId, [Out, MarshalAs(UnmanagedType.Interface)]out object ppEnumScript);
			[PreserveSig]
			int ValidateCodePageEx(uint uiCodePage, IntPtr hWnd, uint dwIODControl);
		}

		public static async Task<string> GetHtml(Uri url)
		{
			using (var client = new HttpClient(new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
			using (var response = await client.GetAsync(url))
			{
				var bytes = await response.Content.ReadAsByteArrayAsync();
				var charset = response.Content.Headers.ContentType.CharSet;
				var infos = DetectEncoding(bytes, IncomingDataTypes.Html, !string.IsNullOrEmpty(charset) ? Encoding.GetEncoding(charset) : null);
				return infos.Length == 0 ? "" : infos[0].Encoding.GetString(bytes);
			}
		}

		public static void SetWindow(this Control control, string appName)
		{
			if (control == null)
				throw new ArgumentNullException("control");
			Marshal.ThrowExceptionForHR(NativeMethods.SetWindowTheme(control.Handle, appName, null));
		}

		public static void ExtendFrameIntoClientArea(this Form form, bool fullWindow)
		{
			if (form == null)
				throw new ArgumentNullException("form");
			NativeMargins m = new NativeMargins(fullWindow);
			Marshal.ThrowExceptionForHR(NativeMethods.DwmExtendFrameIntoClientArea(form.Handle, ref m));
		}

		/// <summary>指定されたバイト配列のエンコーディングを判定します。</summary>
		/// <param name="input">エンコーディングを判定するバイト配列を指定します。</param>
		/// <param name="dataType">指定されたバイト配列の判定にかかわる情報を指定します。</param>
		/// <param name="preffered">希望するエンコーディングを指定します。</param>
		/// <returns>判定の情報を格納する <see cref="DetectEncodingInfo"/> 構造体の配列。</returns>
		public static DetectEncodingInfo[] DetectEncoding(byte[] input, IncomingDataTypes dataType, Encoding preffered)
		{
			if (input == null)
				throw new ArgumentNullException("input");
			int inputLength = input.Length;
			DetectEncodingInfo[] infos = new DetectEncodingInfo[16];
			int infosize = infos.Length;

			IntPtr inputHandle = IntPtr.Zero;
			int ou = 0;
			try
			{
				inputHandle = Marshal.AllocCoTaskMem(Marshal.SizeOf(input[0]) * inputLength);
				Marshal.Copy(input, 0, inputHandle, inputLength);
				IMultiLanguage2 ml2 = (IMultiLanguage2)new CMultiLanguage();
				ou = ml2.DetectInputCodepage(dataType, (uint)(preffered != null ? preffered.CodePage : 0), inputHandle, ref inputLength, infos, ref infosize);
			}
			finally { Marshal.FreeCoTaskMem(inputHandle); }

			if (ou != 0)
				return new DetectEncodingInfo[0];
			if (infosize < infos.Length)
				Array.Resize(ref infos, infosize);
			return infos;
		}

		public static void LoadFromXml(this WeifenLuo.WinFormsUI.Docking.DockPanel panel, string xml, params WeifenLuo.WinFormsUI.Docking.IDockContent[] contents)
		{
			if (panel == null)
				throw new ArgumentNullException("panel");
			if (string.IsNullOrEmpty(xml))
				return;
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(xml)))
				panel.LoadFromXml(ms, persistString => Array.Find(contents, x => string.Equals(persistString, x.DockHandler.GetPersistStringCallback(), StringComparison.Ordinal)));
		}
	}

	/// <summary>エンコーディング判定の結果を格納します。</summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct DetectEncodingInfo : IEquatable<DetectEncodingInfo>
	{
		uint nLangID;
		uint nCodePage;
		int nDocPercent;
		int nConfidence;

		/// <summary>データから検出されたプライマリ言語 ID を取得します。</summary>
		public int LanguageId { get { return (int)nLangID; } }
		/// <summary>検出された言語中に存在する文字列の割合を取得します。</summary>
		public int StringPercent { get { return nDocPercent; } }
		/// <summary>検出されたデータの正確性を示す値を取得します。</summary>
		public int Accuracy { get { return nConfidence; } }
		/// <summary>データから検出されたエンコーディングを取得します。</summary>
		public Encoding Encoding { get { return Encoding.GetEncoding((int)nCodePage); } }

		public bool Equals(DetectEncodingInfo other) { return LanguageId == other.LanguageId && nCodePage == other.nCodePage && StringPercent == other.StringPercent && Accuracy == other.Accuracy; }

		public override bool Equals(object obj)
		{
			if (obj is DetectEncodingInfo)
				return Equals((DetectEncodingInfo)obj);
			return false;
		}

		public override int GetHashCode() { return LanguageId.GetHashCode() ^ nCodePage.GetHashCode() ^ StringPercent.GetHashCode() ^ Accuracy.GetHashCode(); }

		public override string ToString() { return string.Format(CultureInfo.CurrentCulture, "CodePage: {0}, {1}%", nCodePage, StringPercent); }

		public static bool operator ==(DetectEncodingInfo left, DetectEncodingInfo right) { return left.Equals(right); }

		public static bool operator !=(DetectEncodingInfo left, DetectEncodingInfo right) { return !(left == right); }
	}

	/// <summary>エンコーディングを判定するデータのタイプを表します。</summary>
	[Flags]
	public enum IncomingDataTypes
	{
		/// <summary>既定値が使用されます。</summary>
		None = 0x00,
		/// <summary>ストリームには 7bit データが含まれています。</summary>
		Bit7 = 0x01,
		/// <summary>ストリームには 8bit データが含まれています。</summary>
		Bit8 = 0x02,
		/// <summary>ストリームにはダブルバイトデータが含まれています。</summary>
		Dbcs = 0x04,
		/// <summary>ストリームは HTML ページです。</summary>
		Html = 0x08,
		/// <summary>サポートされていません。</summary>
		Number = 0x10,
	}
}
