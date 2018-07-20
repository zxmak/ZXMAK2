using System;
using System.Runtime.InteropServices;


namespace ZXMAK2.Host.Media.Tools
{
    internal sealed class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public unsafe static extern void CopyMemory(int* destination, int* source, int length);
    }
}
