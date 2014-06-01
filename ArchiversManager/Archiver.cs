using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CommonLibrary;

namespace Comical.Archivers
{
	public class Archiver : IDisposable
	{
		delegate ushort GetVersionFunc();
		delegate bool GetRunningFunc();
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		delegate bool CheckArchiveFunc([MarshalAs(UnmanagedType.LPStr)]string fileName, int mode);
		[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]
		delegate int ExecuteFunc(IntPtr hWnd, [MarshalAs(UnmanagedType.LPStr)]string commandLine, [MarshalAs(UnmanagedType.LPStr)]StringBuilder output, uint size);

		public Archiver(string dllName, string functionName, string extractCommand)
		{
			var p = GetPath(dllName);
			if (string.IsNullOrEmpty(p))
				throw new DllNotFoundException();
			if ((ModuleHandle = NativeMethods.LoadLibrary(p)) == IntPtr.Zero)
				throw new Win32Exception(Properties.Resources.FailedDllLoading, Marshal.GetExceptionForHR(Marshal.GetLastWin32Error()));
			DllFileName = dllName;
			ExecuteFunctionName = functionName;
			ExtractCommand = extractCommand;
		}

		~Archiver() { Dispose(false); }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				DllFileName = null;
				ExecuteFunctionName = null;
				ExtractCommand = null;
			}
			if (ModuleHandle != IntPtr.Zero)
			{
				NativeMethods.FreeLibrary(ModuleHandle);
				ModuleHandle = IntPtr.Zero;
			}
		}

		#region For Extensibility

		protected TDelegate GetFunction<TDelegate>(string funcSuffix) where TDelegate : class
		{
			if (!typeof(TDelegate).IsSubclassOf(typeof(Delegate)))
				throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.TypeParameterMustBeDerivedType, "TDelegate", "System.Delegate"));
			string funcName = this.ExecuteFunctionName + funcSuffix;
			IntPtr funcAddr = NativeMethods.GetProcAddress(this.ModuleHandle, funcName);
			if (funcAddr == IntPtr.Zero)
				throw new MissingMethodException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.ApiFunctionNotFound, funcName));
			return Marshal.GetDelegateForFunctionPointer(funcAddr, typeof(TDelegate)) as TDelegate;
		}

		protected bool CheckArchive(string filePath, int mode) { return GetFunction<CheckArchiveFunc>("CheckArchive")(filePath, mode); }

		protected void Execute(IntPtr handle, string command, StringBuilder output, int length) { Marshal.ThrowExceptionForHR(GetFunction<ExecuteFunc>("")(handle, command, output, (uint)length)); }

		#endregion

		public bool CanExtract(string filePath) { return CheckArchive(filePath, 0); }

		static string ProcessWithDoubleQuotation(string src, Func<string, string> process) { return "\"" + process(Regex.Replace(src, "^\"?([^\"]*)\"?$", "$1")) + "\""; }

		static string ToCommandStringFromFile(string fileName)
		{
			if (fileName.Length == 0)
				return fileName;
			return ProcessWithDoubleQuotation(fileName, (arg) =>
			{
				// "-","@"で始まるファイル名の処理
				if (arg[0] == '-' || arg[0] == '@')
					arg = ".\\" + arg;
				return arg;
			});
		}

		static string ToCommandStringFromDirectory(string dirName)
		{
			if (dirName.Length == 0)
				return dirName;
			return ProcessWithDoubleQuotation(dirName, (arg) =>
			{
				// 最後に"\"
				if (arg[arg.Length - 1] != '\\')
					arg += "\\";
				return arg;
			});
		}

		public virtual void Extract(string archive, string extractTo, string fileSpecifier)
		{
			if (!File.Exists(archive))
				throw new FileNotFoundException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.ArchiveNotFound, archive));
			if (IsBusy)
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.ArchiverIsBusy, DllFileName));
			if (!CanExtract(archive))
				throw new CannotExtractArchiveException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.CannotExtractArchive, archive, DllFileName));
			
			Directory.CreateDirectory(extractTo);

			//コマンドラインの作成
			string cmd = ExtractCommand.Replace("{arc}", ToCommandStringFromFile(archive))
				.Replace("{dir}", ToCommandStringFromDirectory(extractTo)).Replace("{filespec}", fileSpecifier);

			//展開する
			Execute(IntPtr.Zero, cmd, null, 0);
		}

		public static bool Exists(string dllName) { return !string.IsNullOrEmpty(GetPath(dllName)); }

		public static string GetPath(string dllName)
		{
			IntPtr pathPtr;
			StringBuilder sb = new StringBuilder(1024);
			bool b = NativeMethods.SearchPath(null, dllName, null, 1024, sb, out pathPtr) != 0;
			return b ? sb.ToString() : string.Empty;
		}

		public string DllPath { get { return GetPath(DllFileName); } }

		public string DllFileName { get; private set; }

		protected string ExecuteFunctionName { get; private set; }

		protected IntPtr ModuleHandle { get; private set; }

		public string ExtractCommand { get; private set; }

		public Version Version
		{
			get
			{
				try
				{
					ushort ret = GetFunction<GetVersionFunc>("GetVersion")();
					return new Version(ret / 100, ret % 100);
				}
				catch (MissingMethodException) { return new Version(); }
			}
		}

		public bool IsBusy { get { return GetFunction<GetRunningFunc>("GetRunning")(); } }
	}

	public class ArchiverSetting
	{
		public ArchiverSetting() { BundleDllName = ""; }

		public string DllName { get; set; }

		public string FunctionName { get; set; }

		public string ExtractCommand { get; set; }

		public string WebPage { get; set; }

		public string BundleDllName { get; set; }

		public ArchiverSetting DependedArchiver { get; set; }

		public bool Exists { get { return Archiver.Exists(DllName); } }

		public string Path { get { return Archiver.GetPath(DllName); } }

		public async Task<Version> GetUploadedVersion()
		{
			CancellationTokenSource cts = new CancellationTokenSource();
			var getHtmlTask = CommonUtils.GetHtml(new Uri("http://www.madobe.net/archiver/lib/" + WebPage), cts.Token);
			var timeout = Task.Delay(2000);
			try
			{
				if (await Task.WhenAny(getHtmlTask, timeout) == getHtmlTask)
				{
					var mWWWC = Regex.Matches(await getHtmlTask, "<META\\s+name=\"WWWC\"\\s+content=\"[\\d/]+\\s+(?<vmaj>\\d+):(?<vmin>\\d+)\\s+(?<dll>.+?)\\s+.+\\[File:.+?].+\">", RegexOptions.IgnoreCase)
						.Cast<Match>().FirstOrDefault(m => m.Success && m.Groups["dll"].Value.Equals(DllName, StringComparison.OrdinalIgnoreCase));
					if (mWWWC != null)
						return new Version(int.Parse(mWWWC.Groups["vmaj"].Value, CultureInfo.InvariantCulture), int.Parse(mWWWC.Groups["vmin"].Value, CultureInfo.InvariantCulture));
				}
				else
					cts.Cancel();
			}
			catch (WebException) { }
			return new Version();
		}

		public async Task<string[]> GetDownloadUrls()
		{
			var code = await CommonUtils.GetHtml(new Uri("http://www.madobe.net/archiver/lib/" + WebPage), CancellationToken.None);
			var mWWWC = Regex.Matches(code, "<META\\s+name=\"WWWC\"\\s+content=\"[\\d/]+\\s+\\d+:\\d+\\s+(?<dll>.+?)\\s+.+\\[File:(?<file>.+?)].+\">", RegexOptions.IgnoreCase)
				.Cast<Match>().FirstOrDefault(m => m.Success && m.Groups["dll"].Value.Equals(DllName, StringComparison.OrdinalIgnoreCase));
			return Regex.Matches(code, "<META\\s+name=\"download\"\\s+content=\"(?<url>.+/" + mWWWC.Groups["file"].Value + "/?)\">", RegexOptions.IgnoreCase)
				.Cast<Match>().Where(m => m.Success).Select(m => m.Groups["url"].Value).ToArray();
		}

		public async Task<bool> IsLatestVersionAvailable()
		{
			using (var arc = CreateArchiver())
				return arc.Version < await GetUploadedVersion();
		}

		public Archiver CreateArchiver()
		{
			if (Exists)
			{
				Archiver arc = null;
				try
				{
					arc = new Archiver(DllName, FunctionName, ExtractCommand);
					return arc;
				}
				catch (Win32Exception)
				{
					if (arc != null)
						arc.Dispose();
				}
			}
			return null;
		}
	}

	public static class ArchiversConfiguration
	{
		static IEnumerable<ArchiverSetting> ReadSettings()
		{
			ArchiverSetting unlha32, unzip32, sevenzip32;
			yield return unlha32 = new ArchiverSetting()
			{
				DllName = "Unlha32.dll",
				FunctionName = "Unlha",
				ExtractCommand = "x -a1 -r1 {arc} {dir} {filespec}",
				WebPage = "unlha32.html",
			};
			yield return unzip32 = new ArchiverSetting()
			{
				DllName = "UnZip32.dll",
				FunctionName = "UnZip",
				ExtractCommand = "-x {arc} {dir} {filespec}",
				WebPage = "unzip32.html",
				DependedArchiver = unlha32
			};
			yield return new ArchiverSetting()
			{
				DllName = "Unrar32.dll",
				FunctionName = "Unrar",
				ExtractCommand = "-x -r {arc} {dir} {filespec}",
				WebPage = "unrar32.html",
				BundleDllName = "UnRAR.dll",
				DependedArchiver = unlha32
			};
			yield return new ArchiverSetting()
			{
				DllName = "Tar32.dll",
				FunctionName = "Tar",
				ExtractCommand = "-x {arc} {dir} {filespec}",
				WebPage = "tar32.html",
				DependedArchiver = unzip32,
			};
			yield return new ArchiverSetting()
			{
				DllName = "Bga32.dll",
				FunctionName = "Bga",
				ExtractCommand = "x -a -r {arc} {dir} {filespec}",
				WebPage = "bga32.html",
				DependedArchiver = unlha32,
			};
			yield return new ArchiverSetting()
			{
				DllName = "Unarj32j.dll",
				FunctionName = "Unarj",
				ExtractCommand = "x -jyc -r {arc} {dir} {filespec}",
				WebPage = "unarj32.html",
				DependedArchiver = unlha32,
			};
			yield return new ArchiverSetting()
			{
				DllName = "UnGCA32.dll",
				FunctionName = "UnGCA",
				ExtractCommand = "e -sx1 {arc} {dir} {filespec}",
				WebPage = "ungca32.html",
				DependedArchiver = unlha32,
			};
			yield return sevenzip32 = new ArchiverSetting()
			{
				DllName = "7-zip32.dll",
				FunctionName = "SevenZip",
				ExtractCommand = "x -r {arc} {dir} {filespec}",
				WebPage = "7-zip32.html",
				DependedArchiver = unzip32,
			};
			yield return new ArchiverSetting()
			{
				DllName = "UnImp32.dll",
				FunctionName = "UnImp",
				ExtractCommand = "x -r1 {arc} {dir} {filespec}",
				WebPage = "unimp32.html",
				DependedArchiver = unlha32,
			};
			yield return new ArchiverSetting()
			{
				DllName = "UnHki32.dll",
				FunctionName = "UnHki",
				ExtractCommand = "x -r1 {arc} {dir} {filespec}",
				WebPage = "unhki32.html",
				DependedArchiver = unzip32,
			};
			yield return new ArchiverSetting()
			{
				DllName = "UnAceV2J.dll",
				FunctionName = "UnAce",
				ExtractCommand = "x {arc} {dir} {filespec}",
				WebPage = "UnAceV2J.html",
				BundleDllName = "UnAceV2.dll",
				DependedArchiver = unzip32,
			};
			yield return new ArchiverSetting()
			{
				DllName = "unbel32.dll",
				FunctionName = "Unbel",
				ExtractCommand = "{arc} {dir}",
				WebPage = "unbel32.html",
				DependedArchiver = unzip32,
			};
			yield return new ArchiverSetting()
			{
				DllName = "UnIso32.dll",
				FunctionName = "UnIso",
				ExtractCommand = "x -r {arc} {dir} {filespec}",
				WebPage = "uniso32.html",
				DependedArchiver = sevenzip32,
			};
		}

		static readonly IReadOnlyList<ArchiverSetting> settings = new List<ArchiverSetting>(ReadSettings());

		public static Archiver FindArchiverToExtract(string filePath)
		{
			Archiver extractor = null;
			foreach (var arcset in Settings)
			{
				Archiver arc = null;
				try
				{
					arc = arcset.CreateArchiver();
					if (arc != null && arc.CanExtract(filePath))
					{
						extractor = arc;
						arc = null;
						break;
					}
				}
				finally
				{
					if (arc != null)
						arc.Dispose();
				}
			}
			return extractor;
		}

		public static IReadOnlyList<ArchiverSetting> Settings { get { return settings; } }

		public static IEnumerable<ArchiverSetting> GetDependingArchivers(this ArchiverSetting setting) { return Settings.Where(x => x.DependedArchiver == setting); }

		static Process RunArchiversManager(IntPtr ownerHandle, string args)
		{
			ProcessStartInfo psi = new ProcessStartInfo();
			psi.FileName = System.Reflection.Assembly.GetExecutingAssembly().Location;
			psi.Verb = "RunAs";
			psi.Arguments = ownerHandle.ToString() + " " + args;
			Process process = null;
			try { process = Process.Start(psi); }
			catch (System.ComponentModel.Win32Exception ex) { System.Diagnostics.Trace.WriteLine(ex); }
			return process;
		}

		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
		public static Process Install(IntPtr ownerHandle, ArchiverSetting set)
		{
			if (set == null)
				throw new ArgumentNullException("set");
			return RunArchiversManager(ownerHandle, "/install " + set.DllName);
		}

		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
		public static Process UpdateAll(IntPtr ownerHandle) { return RunArchiversManager(ownerHandle, "/update"); }

		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand)]
		public static Process Uninstall(IntPtr ownerHandle, ArchiverSetting set)
		{
			if (set == null)
				throw new ArgumentNullException("set");
			return RunArchiversManager(ownerHandle, "/uninstall " + set.DllName);
		}
	}

	[Serializable]
	public class CannotExtractArchiveException : Exception
	{
		public CannotExtractArchiveException() { }

		public CannotExtractArchiveException(string message) : base(message) { }

		public CannotExtractArchiveException(string message, Exception innerException) : base(message, innerException) { }

		protected CannotExtractArchiveException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
