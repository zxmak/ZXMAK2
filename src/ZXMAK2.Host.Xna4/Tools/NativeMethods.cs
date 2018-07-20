using System;
using System.Runtime.InteropServices;
using System.Security;


namespace ZXMAK2.Host.Xna4.Tools
{
    [SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public unsafe static extern void CopyMemory(uint* destination, uint* source, int length);
    }
}
