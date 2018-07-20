/* 
 *  Copyright 2008-2018 Alex Makeev
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
 *  Description: DirectX native wrapper
 *  Date: 15.07.2018
 */
using System;
using System.Runtime.InteropServices;
using ZXMAK2.DirectX.Vectors;

namespace ZXMAK2.DirectX.Direct3D
{
    // d3d9types.h
    [StructLayout(LayoutKind.Sequential)]
    public struct D3DPRESENT_PARAMETERS
    {
        public int BackBufferWidth;             // UINT
        public int BackBufferHeight;            // UINT
        public D3DFORMAT BackBufferFormat;      // D3DFORMAT
        public int BackBufferCount;             // UINT

        public D3DMULTISAMPLE_TYPE MultiSampleType;    // D3DMULTISAMPLE_TYPE
        public int MultiSampleQuality;          // DWORD

        public D3DSWAPEFFECT SwapEffect;        // D3DSWAPEFFECT
        public IntPtr hDeviceWindow;            // HWND
        public RawBool Windowed;                // BOOL
        public RawBool EnableAutoDepthStencil;  // BOOL
        public D3DFORMAT AutoDepthStencilFormat;// D3DFORMAT
        public D3DPRESENTFLAG Flags;            // DWORD

        /// <summary>
        /// FullScreen_RefreshRateInHz must be zero for Windowed mode
        /// </summary>
        public int FullScreen_RefreshRateInHz;  // UINT
        public D3DPRESENT_INTERVAL PresentationInterval;        // UINT
    }
}
