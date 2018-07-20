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
using System.Globalization;
using System.Runtime.InteropServices;


namespace ZXMAK2.DirectX.DirectSound
{
    // <unmanaged>WAVEFORMATEXTENSIBLE</unmanaged>
    public class WaveFormatExtensible : WaveFormat
    {
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal new struct __Native
        {
            public WaveFormat.__Native waveFormat;

            public short wValidBitsPerSample;

            public SPEAKER_FLAGS dwChannelMask;

            public Guid subFormat;

            internal void __MarshalFree()
            {
                this.waveFormat.__MarshalFree();
            }
        }

        private short wValidBitsPerSample;

        public Guid GuidSubFormat;

        public SPEAKER_FLAGS ChannelMask;

        internal WaveFormatExtensible()
        {
        }

        public WaveFormatExtensible(int rate, int bits, int channels)
            : base(rate, bits, channels)
        {
            this.waveFormatTag = WAVE_FORMAT.EXTENSIBLE;
            this.extraSize = 22;
            this.wValidBitsPerSample = (short)bits;
            int num = 0;
            for (int i = 0; i < channels; i++)
            {
                num |= 1 << i;
            }
            this.ChannelMask = (SPEAKER_FLAGS)num;
            this.GuidSubFormat = ((bits == 32) ? new Guid("00000003-0000-0010-8000-00aa00389b71") : new Guid("00000001-0000-0010-8000-00aa00389b71"));
        }

        protected unsafe override IntPtr MarshalToPtr()
        {
            IntPtr intPtr = Marshal.AllocHGlobal(sizeof(WaveFormatExtensible.__Native));
            this.__MarshalTo(ref *(WaveFormatExtensible.__Native*)((void*)intPtr));
            return intPtr;
        }

        internal void __MarshalFrom(ref WaveFormatExtensible.__Native @ref)
        {
            base.__MarshalFrom(ref @ref.waveFormat);
            this.wValidBitsPerSample = @ref.wValidBitsPerSample;
            this.ChannelMask = @ref.dwChannelMask;
            this.GuidSubFormat = @ref.subFormat;
        }

        internal void __MarshalTo(ref WaveFormatExtensible.__Native @ref)
        {
            base.__MarshalTo(ref @ref.waveFormat);
            @ref.wValidBitsPerSample = this.wValidBitsPerSample;
            @ref.dwChannelMask = this.ChannelMask;
            @ref.subFormat = this.GuidSubFormat;
        }

        internal static WaveFormatExtensible.__Native __NewNative()
        {
            WaveFormatExtensible.__Native result = default(WaveFormatExtensible.__Native);
            result.waveFormat.extraSize = 22;
            return result;
        }

        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture, 
                "{0} wBitsPerSample:{1} ChannelMask:{2} SubFormat:{3} extraSize:{4}", 
                new object[] {
				    base.ToString(),
				    this.wValidBitsPerSample,
				    this.ChannelMask,
				    this.GuidSubFormat,
				    this.extraSize });
        }
    }
}
