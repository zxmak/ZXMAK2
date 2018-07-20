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
 *  Date: 10.07.2018
 */
using System;
using System.Runtime.InteropServices;


namespace ZXMAK2.DirectX.DirectSound
{
    // <unmanaged>DSBUFFERDESC</unmanaged>	
    public class DSBUFFERDESC
    {
        internal struct __Native
        {
            // <unmanaged>unsigned int dwSize</unmanaged>	
            public int dwSize;

            // <unmanaged>DSBCAPS_FLAGS dwFlags</unmanaged>	
            public DSBCAPS_FLAGS dwFlags;

            // <unmanaged>unsigned int dwBufferBytes</unmanaged>	
            public int dwBufferBytes;

            // <unmanaged>unsigned int dwReserved</unmanaged>
            public int dwReserved;

            // <unmanaged>WAVEFORMATEX* lpwfxFormat</unmanaged>	
            public IntPtr lpwfxFormat;

            // <unmanaged>GUID guid3DAlgorithm</unmanaged>	
            public Guid guid3DAlgorithm;

            internal void __MarshalFree()
            {
                if (this.lpwfxFormat != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(this.lpwfxFormat);
                }
            }
        }

        // <unmanaged>unsigned int dwSize</unmanaged>	
        private int dwSize;

        // <unmanaged>DSBCAPS_FLAGS dwFlags</unmanaged>	
        public DSBCAPS_FLAGS dwFlags;

        // <unmanaged>unsigned int dwBufferBytes</unmanaged>	
        public int dwBufferBytes;

#pragma warning disable 0649
        // <unmanaged>unsigned int dwReserved</unmanaged>
        internal int dwReserved;
#pragma warning restore 0649

        // <unmanaged>WAVEFORMATEX* lpwfxFormat</unmanaged>	
        internal IntPtr lpwfxFormat;

        // <unmanaged>GUID guid3DAlgorithm</unmanaged>	
        public Guid guid3DAlgorithm;

        public WaveFormat Format;

        public unsafe DSBUFFERDESC()
        {
            this.dwSize = sizeof(DSBUFFERDESC.__Native);
            this.lpwfxFormat = IntPtr.Zero;
        }

        internal void __MarshalFree(ref DSBUFFERDESC.__Native @ref)
        {
            @ref.__MarshalFree();
        }

        internal void __MarshalTo(ref DSBUFFERDESC.__Native @ref)
        {
            @ref.dwSize = this.dwSize;
            @ref.dwFlags = this.dwFlags;
            @ref.dwBufferBytes = this.dwBufferBytes;
            @ref.dwReserved = this.dwReserved;
            @ref.lpwfxFormat = WaveFormat.MarshalToPtr(this.Format);
            @ref.guid3DAlgorithm = this.guid3DAlgorithm;
        }

        internal unsafe static DSBUFFERDESC.__Native __NewNative()
        {
            return new DSBUFFERDESC.__Native
            {
                dwSize = sizeof(DSBUFFERDESC.__Native)
            };
        }
    }
}
