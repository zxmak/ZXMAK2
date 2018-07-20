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


namespace ZXMAK2.DirectX.DirectSound
{
    // <unmanaged>DSEFFECTDESC</unmanaged>	
    internal class DSEFFECTDESC
    {
        internal struct __Native
        {
            // <unmanaged>unsigned int dwSize</unmanaged>	
            public int dwSize;

            // <unmanaged>unsigned int dwFlags</unmanaged>	
            public int dwFlags;

            // <unmanaged>GUID guidDSFXClass</unmanaged>	
            public Guid guidDSFXClass;

            // <unmanaged>ULONG_PTR dwReserved1</unmanaged>	
            public IntPtr dwReserved1;

            // <unmanaged>ULONG_PTR dwReserved2</unmanaged>	
            public IntPtr dwReserved2;

            
            internal void __MarshalFree()
            {
            }
        }

        private int dwSize;
        public int dwFlags;
        public Guid guidDSFXClass;
        internal IntPtr dwReserved1;
        internal IntPtr dwReserved2;


        public unsafe DSEFFECTDESC()
        {
            this.dwSize = sizeof(DSEFFECTDESC.__Native);
        }

        internal unsafe static DSEFFECTDESC.__Native __NewNative()
        {
            return new DSEFFECTDESC.__Native
            {
                dwSize = sizeof(DSEFFECTDESC.__Native)
            };
        }

        internal void __MarshalFree(ref DSEFFECTDESC.__Native @ref)
        {
            @ref.__MarshalFree();
        }

        internal void __MarshalFrom(ref DSEFFECTDESC.__Native @ref)
        {
            this.dwSize = @ref.dwSize;
            this.dwFlags = @ref.dwFlags;
            this.guidDSFXClass = @ref.guidDSFXClass;
            this.dwReserved1 = @ref.dwReserved1;
            this.dwReserved2 = @ref.dwReserved2;
        }

        internal void __MarshalTo(ref DSEFFECTDESC.__Native @ref)
        {
            @ref.dwSize = this.dwSize;
            @ref.dwFlags = this.dwFlags;
            @ref.guidDSFXClass = this.guidDSFXClass;
            @ref.dwReserved1 = this.dwReserved1;
            @ref.dwReserved2 = this.dwReserved2;
        }
    }
}
