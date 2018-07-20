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
    // <unmanaged>WAVEFORMATADPCM</unmanaged>
    public class WaveFormatAdpcm : WaveFormat
    {
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal new struct __Native
        {
            public WaveFormat.__Native waveFormat;

            public ushort samplesPerBlock;

            public ushort numberOfCoefficients;

            public short coefficients;

            internal void __MarshalFree()
            {
                this.waveFormat.__MarshalFree();
            }
        }

        public ushort SamplesPerBlock
        {
            get;
            private set;
        }

        public short[] Coefficients1
        {
            get;
            set;
        }

        public short[] Coefficients2
        {
            get;
            set;
        }

        internal WaveFormatAdpcm()
        {
        }

        public WaveFormatAdpcm(int rate, int channels, int blockAlign = 0)
            : base(rate, 4, channels)
        {
            this.waveFormatTag = WAVE_FORMAT.ADPCM;
            this.blockAlign = (short)blockAlign;
            if (blockAlign == 0)
            {
                if (rate <= 11025)
                {
                    blockAlign = 256;
                }
                else if (rate <= 22050)
                {
                    blockAlign = 512;
                }
                else
                {
                    blockAlign = 1024;
                }
            }
            this.SamplesPerBlock = (ushort)(blockAlign * 2 / channels - 12);
            this.averageBytesPerSecond = base.SampleRate * blockAlign / (int)this.SamplesPerBlock;
            this.Coefficients1 = new short[]
			{
				256,
				512,
				0,
				192,
				240,
				460,
				392
			};
            this.Coefficients2 = new short[]
			{
				0,
				-256,
				0,
				64,
				0,
				-208,
				-232
			};
            this.extraSize = 32;
        }

        protected unsafe override IntPtr MarshalToPtr()
        {
            IntPtr intPtr = Marshal.AllocHGlobal(sizeof(WaveFormat.__Native) + 4 + 4 * this.Coefficients1.Length);
            this.__MarshalTo(ref *(WaveFormatAdpcm.__Native*)((void*)intPtr));
            return intPtr;
        }

        internal unsafe void __MarshalFrom(ref WaveFormatAdpcm.__Native @ref)
        {
            base.__MarshalFrom(ref @ref.waveFormat);
            this.SamplesPerBlock = @ref.samplesPerBlock;
            this.Coefficients1 = new short[(int)@ref.numberOfCoefficients];
            this.Coefficients2 = new short[(int)@ref.numberOfCoefficients];
            if (@ref.numberOfCoefficients > 7)
            {
                throw new InvalidOperationException("Unable to read Adpcm format. Too may coefficients (max 7)");
            }
            fixed (short* ptr = &@ref.coefficients)
            {
                for (int i = 0; i < (int)@ref.numberOfCoefficients; i++)
                {
                    this.Coefficients1[i] = ptr[i * 2];
                    this.Coefficients2[i] = ptr[i * 2 + 1];
                }
            }
            this.extraSize = (short)(4 + 4 * @ref.numberOfCoefficients);
        }

        private unsafe void __MarshalTo(ref WaveFormatAdpcm.__Native @ref)
        {
            if (this.Coefficients1.Length > 7)
            {
                throw new InvalidOperationException("Unable to encode Adpcm format. Too may coefficients (max 7)");
            }
            this.extraSize = (short)(4 + 4 * this.Coefficients1.Length);
            base.__MarshalTo(ref @ref.waveFormat);
            @ref.samplesPerBlock = this.SamplesPerBlock;
            @ref.numberOfCoefficients = (ushort)this.Coefficients1.Length;
            fixed (short* ptr = &@ref.coefficients)
            {
                for (int i = 0; i < (int)@ref.numberOfCoefficients; i++)
                {
                    ptr[i * 2] = this.Coefficients1[i];
                    ptr[i * 2 + 1] = this.Coefficients2[i];
                }
            }
        }
    }
}
