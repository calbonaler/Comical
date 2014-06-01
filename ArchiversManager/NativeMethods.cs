using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Comical.Archivers
{
	static class NativeMethods
	{
		[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false)]
		public extern static IntPtr LoadLibrary(string fileName);

		[DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public extern static bool FreeLibrary(IntPtr moduleHandle);

		[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
		public extern static IntPtr GetProcAddress(IntPtr moduleHandle, string procName);

		[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false)]
		public static extern uint SearchPath(string path, string fileName, string extension, uint bufferLength, StringBuilder buffer, out IntPtr filePart);
	}
}
