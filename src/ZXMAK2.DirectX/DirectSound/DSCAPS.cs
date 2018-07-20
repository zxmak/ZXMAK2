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
    // <unmanaged>DSCAPS</unmanaged>	
    public class DSCAPS
    {
        internal struct __Native
        {
            public int dwSize;
            public DSCAPS_FLAGS dwFlags;
            public int dwMinSecondarySampleRate;
            public int dwMaxSecondarySampleRate;
            public int dwPrimaryBuffers;
            public int dwMaxHardwareMixingAllBuffers;
            public int dwMaxHardwareMixingStaticBuffers;
            public int dwMaxHardwareMixingStreamingBuffers;
            public int dwFreeHardwareMixingAllBuffers;
            public int dwFreeHardwareMixingStaticBuffers;
            public int dwFreeHardwareMixingStreamingBuffers;
            public int dwMaxHardware3DAllBuffers;
            public int dwMaxHardware3DStaticBuffers;
            public int dwMaxHardware3DStreamingBuffers;
            public int dwFreeHardware3DAllBuffers;
            public int dwFreeHardware3DStaticBuffers;
            public int dwFreeHardware3DStreamingBuffers;
            public int dwTotalHardwareMemBytes;
            public int dwFreeHardwareMemBytes;
            public int dwMaxContigFreeHardwareMemBytes;
            public int dwUnlockTransferRateHardwareBuffers;
            public int dwPlayCpuOverheadSwBuffers;
            public int dwReserved1;
            public int dwReserved2;

            internal void __MarshalFree()
            {
            }
        }

        // <unmanaged>unsigned int dwSize</unmanaged>	
        private int dwSize;

        // <unmanaged>DSCAPS_FLAGS dwFlags</unmanaged>	
        public DSCAPS_FLAGS dwFlags;

        // <unmanaged>unsigned int dwMinSecondarySampleRate</unmanaged>	
        public int dwMinSecondarySampleRate;

        // <unmanaged>unsigned int dwMaxSecondarySampleRate</unmanaged>	
        public int dwMaxSecondarySampleRate;

        // <unmanaged>unsigned int dwPrimaryBuffers</unmanaged>	
        public int dwPrimaryBuffers;

        // <unmanaged>unsigned int dwMaxHwMixingAllBuffers</unmanaged>	
        public int dwMaxHwMixingAllBuffers;

        // <unmanaged>unsigned int dwMaxHwMixingStaticBuffers</unmanaged>	
        public int dwMaxHwMixingStaticBuffers;

        // <unmanaged>unsigned int dwMaxHwMixingStreamingBuffers</unmanaged>	
        public int dwMaxHwMixingStreamingBuffers;

        // <unmanaged>unsigned int dwFreeHwMixingAllBuffers</unmanaged>	
        public int dwFreeHwMixingAllBuffers;

        // <unmanaged>unsigned int dwFreeHwMixingStaticBuffers</unmanaged>	
        public int dwFreeHwMixingStaticBuffers;

        // <unmanaged>unsigned int dwFreeHwMixingStreamingBuffers</unmanaged>	
        public int dwFreeHwMixingStreamingBuffers;

        // <unmanaged>unsigned int dwMaxHw3DAllBuffers</unmanaged>	
        public int dwMaxHw3DAllBuffers;

        // <unmanaged>unsigned int dwMaxHw3DStaticBuffers</unmanaged>	
        public int dwMaxHw3DStaticBuffers;

        // <unmanaged>unsigned int dwMaxHw3DStreamingBuffers</unmanaged>	
        public int dwMaxHw3DStreamingBuffers;

        // <unmanaged>unsigned int dwFreeHw3DAllBuffers</unmanaged>	
        public int dwFreeHw3DAllBuffers;

        // <unmanaged>unsigned int dwFreeHw3DStaticBuffers</unmanaged>	
        public int dwFreeHw3DStaticBuffers;

        // <unmanaged>unsigned int dwFreeHw3DStreamingBuffers</unmanaged>	
        public int dwFreeHw3DStreamingBuffers;

        // <unmanaged>unsigned int dwTotalHwMemBytes</unmanaged>	
        public int dwTotalHwMemBytes;

        // <unmanaged>unsigned int dwFreeHwMemBytes</unmanaged>	
        public int dwFreeHwMemBytes;

        // <unmanaged>unsigned int dwMaxContigFreeHwMemBytes</unmanaged>	
        public int dwMaxContigFreeHwMemBytes;

        // <unmanaged>unsigned int dwUnlockTransferRateHwBuffers</unmanaged>	
        public int dwUnlockTransferRateHwBuffers;

        // <unmanaged>unsigned int dwPlayCpuOverheadSwBuffers</unmanaged>	
        public int dwPlayCpuOverheadSwBuffers;

        // <unmanaged>unsigned int dwReserved1</unmanaged>	
        internal int dwReserved1;

        // <unmanaged>unsigned int dwReserved2</unmanaged>	
        internal int dwReserved2;

        
        public unsafe DSCAPS()
        {
            this.dwSize = sizeof(DSCAPS.__Native);
        }

        internal unsafe static DSCAPS.__Native __NewNative()
        {
            return new DSCAPS.__Native
            {
                dwSize = sizeof(DSCAPS.__Native)
            };
        }

        internal void __MarshalFree(ref DSCAPS.__Native @ref)
        {
            @ref.__MarshalFree();
        }

        internal void __MarshalFrom(ref DSCAPS.__Native @ref)
        {
            this.dwSize = @ref.dwSize;
            this.dwFlags = @ref.dwFlags;
            this.dwMinSecondarySampleRate = @ref.dwMinSecondarySampleRate;
            this.dwMaxSecondarySampleRate = @ref.dwMaxSecondarySampleRate;
            this.dwPrimaryBuffers = @ref.dwPrimaryBuffers;
            this.dwMaxHwMixingAllBuffers = @ref.dwMaxHardwareMixingAllBuffers;
            this.dwMaxHwMixingStaticBuffers = @ref.dwMaxHardwareMixingStaticBuffers;
            this.dwMaxHwMixingStreamingBuffers = @ref.dwMaxHardwareMixingStreamingBuffers;
            this.dwFreeHwMixingAllBuffers = @ref.dwFreeHardwareMixingAllBuffers;
            this.dwFreeHwMixingStaticBuffers = @ref.dwFreeHardwareMixingStaticBuffers;
            this.dwFreeHwMixingStreamingBuffers = @ref.dwFreeHardwareMixingStreamingBuffers;
            this.dwMaxHw3DAllBuffers = @ref.dwMaxHardware3DAllBuffers;
            this.dwMaxHw3DStaticBuffers = @ref.dwMaxHardware3DStaticBuffers;
            this.dwMaxHw3DStreamingBuffers = @ref.dwMaxHardware3DStreamingBuffers;
            this.dwFreeHw3DAllBuffers = @ref.dwFreeHardware3DAllBuffers;
            this.dwFreeHw3DStaticBuffers = @ref.dwFreeHardware3DStaticBuffers;
            this.dwFreeHw3DStreamingBuffers = @ref.dwFreeHardware3DStreamingBuffers;
            this.dwTotalHwMemBytes = @ref.dwTotalHardwareMemBytes;
            this.dwFreeHwMemBytes = @ref.dwFreeHardwareMemBytes;
            this.dwMaxContigFreeHwMemBytes = @ref.dwMaxContigFreeHardwareMemBytes;
            this.dwUnlockTransferRateHwBuffers = @ref.dwUnlockTransferRateHardwareBuffers;
            this.dwPlayCpuOverheadSwBuffers = @ref.dwPlayCpuOverheadSwBuffers;
            this.dwReserved1 = @ref.dwReserved1;
            this.dwReserved2 = @ref.dwReserved2;
        }

        internal void __MarshalTo(ref DSCAPS.__Native @ref)
        {
            @ref.dwSize = this.dwSize;
            @ref.dwFlags = this.dwFlags;
            @ref.dwMinSecondarySampleRate = this.dwMinSecondarySampleRate;
            @ref.dwMaxSecondarySampleRate = this.dwMaxSecondarySampleRate;
            @ref.dwPrimaryBuffers = this.dwPrimaryBuffers;
            @ref.dwMaxHardwareMixingAllBuffers = this.dwMaxHwMixingAllBuffers;
            @ref.dwMaxHardwareMixingStaticBuffers = this.dwMaxHwMixingStaticBuffers;
            @ref.dwMaxHardwareMixingStreamingBuffers = this.dwMaxHwMixingStreamingBuffers;
            @ref.dwFreeHardwareMixingAllBuffers = this.dwFreeHwMixingAllBuffers;
            @ref.dwFreeHardwareMixingStaticBuffers = this.dwFreeHwMixingStaticBuffers;
            @ref.dwFreeHardwareMixingStreamingBuffers = this.dwFreeHwMixingStreamingBuffers;
            @ref.dwMaxHardware3DAllBuffers = this.dwMaxHw3DAllBuffers;
            @ref.dwMaxHardware3DStaticBuffers = this.dwMaxHw3DStaticBuffers;
            @ref.dwMaxHardware3DStreamingBuffers = this.dwMaxHw3DStreamingBuffers;
            @ref.dwFreeHardware3DAllBuffers = this.dwFreeHw3DAllBuffers;
            @ref.dwFreeHardware3DStaticBuffers = this.dwFreeHw3DStaticBuffers;
            @ref.dwFreeHardware3DStreamingBuffers = this.dwFreeHw3DStreamingBuffers;
            @ref.dwTotalHardwareMemBytes = this.dwTotalHwMemBytes;
            @ref.dwFreeHardwareMemBytes = this.dwFreeHwMemBytes;
            @ref.dwMaxContigFreeHardwareMemBytes = this.dwMaxContigFreeHwMemBytes;
            @ref.dwUnlockTransferRateHardwareBuffers = this.dwUnlockTransferRateHwBuffers;
            @ref.dwPlayCpuOverheadSwBuffers = this.dwPlayCpuOverheadSwBuffers;
            @ref.dwReserved1 = this.dwReserved1;
            @ref.dwReserved2 = this.dwReserved2;
        }
    }
}
