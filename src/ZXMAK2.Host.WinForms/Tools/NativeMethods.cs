/* 
 *  Copyright 2008, 2015 Alex Makeev
 * 
 *  This file is part of ZXMAK2 (ZX Spectrum virtual machine).
 *
 *  ZXMAK2 is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  ZXMAK2 is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with ZXMAK2.  If not, see <http://www.gnu.org/licenses/>.
 *
 */
using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Diagnostics;
using System.Security;


namespace ZXMAK2.Host.WinForms.Tools
{
    [SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        public static bool IsWinApiNotAvailable { get; private set; }
        
        #region P/Invoke

        [DllImport("kernel32.dll", SetLastError = true)]
        public unsafe static extern void CopyMemory(int* destination, int* source, int length);

        [DllImport("kernel32.dll", SetLastError = true)]
        public unsafe static extern void CopyMemory(uint* destination, uint* source, int length);


        private const uint TIMERR_NOERROR = 0;
        private const uint TIMERR_NOCANDO = 96 + 1;

        [DllImport("winmm.dll")]
        private static extern uint timeBeginPeriod(uint uPeriod);

        [DllImport("winmm.dll")]
        private static extern uint timeEndPeriod(uint uPeriod);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetAncestor(IntPtr hwnd, int gaFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);


        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner


            public Point Location
            {
                get { return new Point(Left, Top); }
            }

            public Size Size
            {
                get { return new Size(Right - Left, Bottom - Top); }
            }
        }

        #endregion P/Invoke


        #region Wrappers

        public static uint TimeBeginPeriod(uint uPeriod)
        {
            if (IsWinApiNotAvailable)
            {
                return TIMERR_NOCANDO;
            }
            try
            {
                var result = timeBeginPeriod(uPeriod);
                if (result != TIMERR_NOERROR)
                {
                    Logger.Debug("timeBeginPeriod({0}): error {1}", uPeriod, result);
                }
                return result;
            }
            catch (EntryPointNotFoundException ex)
            {
                Logger.Warn(ex);
                IsWinApiNotAvailable = true;
            }
            catch (Exception ex)
            {
                Logger.Debug("{0}: {1}", ex.GetType().Name, ex.Message);
            }
            return TIMERR_NOCANDO;
        }

        public static uint TimeEndPeriod(uint uPeriod)
        {
            if (IsWinApiNotAvailable)
            {
                return TIMERR_NOCANDO;
            }
            try
            {
                var result = timeEndPeriod(uPeriod);
                if (result != TIMERR_NOERROR)
                {
                    Logger.Debug("timeEndPeriod({0}): error {1}", uPeriod, result);
                }
            }
            catch (EntryPointNotFoundException ex)
            {
                Logger.Warn(ex);
                IsWinApiNotAvailable = true;
            }
            catch (Exception ex)
            {
                Logger.Debug("{0}: {1}", ex.GetType().Name, ex.Message);
            }
            return TIMERR_NOCANDO;
        }

        public static Rectangle GetWindowRect(IntPtr hWnd)
        {
            RECT wndRect;
            if (!NativeMethods.GetWindowRect(hWnd, out wndRect))
            {
                Trace.WriteLine("GetWindowRect failed");
                return Rectangle.Empty;
            }
            return new Rectangle(wndRect.Location, wndRect.Size);
        }

        public static unsafe void CopyStride(
            int* pDstBuffer,
            int* pSrcBuffer,
            int width,
            int height,
            int dstStride)
        {
            if (dstStride == width)
            {
                CopyMemory(pDstBuffer, pSrcBuffer, (width * height) << 2);
                return;
            }
            var lineSize = width << 2;
            var srcLine = pSrcBuffer;
            var dstLine = pDstBuffer;
            for (var y = 0; y < height; y++)
            {
                CopyMemory(dstLine, srcLine, lineSize);
                srcLine += width;
                dstLine += dstStride;
            }
        }

        #endregion Wrappers
    }
}
