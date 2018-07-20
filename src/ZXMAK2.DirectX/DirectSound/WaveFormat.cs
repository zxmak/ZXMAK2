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
using System.IO;
using System.Runtime.InteropServices;

namespace ZXMAK2.DirectX.DirectSound
{
    public class WaveFormat
    {
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal struct __Native
        {
            public WaveFormat.__PcmNative pcmWaveFormat;

            /// <summary>number of following bytes</summary>
            public short extraSize;

            internal void __MarshalFree()
            {
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal struct __PcmNative
        {
            // <summary>format type</summary>
            public WAVE_FORMAT waveFormatTag;

            // <summary>number of channels</summary>
            public short channels;

            // <summary>sample rate</summary>
            public int sampleRate;

            // <summary>for buffer estimation</summary>
            public int averageBytesPerSecond;

            // <summary>block size of data</summary>
            public short blockAlign;

            // <summary>number of bits per sample of mono data</summary>
            public short bitsPerSample;

            internal void __MarshalFree()
            {
            }
        }

        // <summary>format type</summary>
        protected WAVE_FORMAT waveFormatTag;

        // <summary>number of channels</summary>
        protected short channels;

        // <summary>sample rate</summary>
        protected int sampleRate;

        // <summary>for buffer estimation</summary>
        protected int averageBytesPerSecond;

        // <summary>block size of data</summary>
        protected short blockAlign;

        // <summary>number of bits per sample of mono data</summary>
        protected short bitsPerSample;

        // <summary>number of following bytes</summary>
        protected short extraSize;

        public WAVE_FORMAT Encoding
        {
            get { return this.waveFormatTag; }
        }

        /// <summary>
        /// Returns the number of channels (1=mono,2=stereo etc)
        /// </summary>
        public int Channels
        {
            get { return (int)this.channels; }
        }

        /// <summary>
        /// Returns the sample rate (samples per second)
        /// </summary>
        public int SampleRate
        {
            get { return this.sampleRate; }
        }

        /// <summary>
        /// Returns the average number of bytes used per second
        /// </summary>
        public int AverageBytesPerSecond
        {
            get { return this.averageBytesPerSecond; }
        }

        /// <summary>
        /// Returns the block alignment
        /// </summary>
        public int BlockAlign
        {
            get { return (int)this.blockAlign; }
        }

        /// <summary>
        /// Returns the number of bits per sample (usually 16 or 32, sometimes 24 or 8)
        /// Can be 0 for some codecs
        /// </summary>
        public int BitsPerSample
        {
            get { return (int)this.bitsPerSample; }
        }

        /// <summary>
        /// Returns the number of extra bytes used by this waveformat. Often 0,
        /// except for compressed formats which store extra data after the WAVEFORMATEX header
        /// </summary>
        public int ExtraSize
        {
            get { return (int)this.extraSize; }
        }

        internal void __MarshalFree(ref WaveFormat.__Native @ref)
        {
            @ref.__MarshalFree();
        }

        internal void __MarshalFrom(ref WaveFormat.__Native @ref)
        {
            this.waveFormatTag = @ref.pcmWaveFormat.waveFormatTag;
            this.channels = @ref.pcmWaveFormat.channels;
            this.sampleRate = @ref.pcmWaveFormat.sampleRate;
            this.averageBytesPerSecond = @ref.pcmWaveFormat.averageBytesPerSecond;
            this.blockAlign = @ref.pcmWaveFormat.blockAlign;
            this.bitsPerSample = @ref.pcmWaveFormat.bitsPerSample;
            this.extraSize = @ref.extraSize;
        }

        internal void __MarshalTo(ref WaveFormat.__Native @ref)
        {
            @ref.pcmWaveFormat.waveFormatTag = this.waveFormatTag;
            @ref.pcmWaveFormat.channels = this.channels;
            @ref.pcmWaveFormat.sampleRate = this.sampleRate;
            @ref.pcmWaveFormat.averageBytesPerSecond = this.averageBytesPerSecond;
            @ref.pcmWaveFormat.blockAlign = this.blockAlign;
            @ref.pcmWaveFormat.bitsPerSample = this.bitsPerSample;
            @ref.extraSize = this.extraSize;
        }

        internal void __MarshalFree(ref WaveFormat.__PcmNative @ref)
        {
            @ref.__MarshalFree();
        }

        internal void __MarshalFrom(ref WaveFormat.__PcmNative @ref)
        {
            this.waveFormatTag = @ref.waveFormatTag;
            this.channels = @ref.channels;
            this.sampleRate = @ref.sampleRate;
            this.averageBytesPerSecond = @ref.averageBytesPerSecond;
            this.blockAlign = @ref.blockAlign;
            this.bitsPerSample = @ref.bitsPerSample;
            this.extraSize = 0;
        }

        internal void __MarshalTo(ref WaveFormat.__PcmNative @ref)
        {
            @ref.waveFormatTag = this.waveFormatTag;
            @ref.channels = this.channels;
            @ref.sampleRate = this.sampleRate;
            @ref.averageBytesPerSecond = this.averageBytesPerSecond;
            @ref.blockAlign = this.blockAlign;
            @ref.bitsPerSample = this.bitsPerSample;
        }

        /// <summary>
        /// Creates a new PCM 44.1Khz stereo 16 bit format
        /// </summary>
        public WaveFormat()
            : this(44100, 16, 2)
        {
        }

        /// <summary>
        /// Creates a new 16 bit wave format with the specified sample
        /// rate and channel count
        /// </summary>
        /// <param name="sampleRate">Sample Rate</param>
        /// <param name="channels">Number of channels</param>
        public WaveFormat(int sampleRate, int channels)
            : this(sampleRate, 16, channels)
        {
        }

        /// <summary>
        /// Gets the size of a wave buffer equivalent to the latency in milliseconds.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <returns></returns>
        public int ConvertLatencyToByteSize(int milliseconds)
        {
            int num = (int)((double)this.AverageBytesPerSecond / 1000.0 * (double)milliseconds);
            if (num % this.BlockAlign != 0)
            {
                num = num + this.BlockAlign - num % this.BlockAlign;
            }
            return num;
        }

        /// <summary>
        /// Creates a WaveFormat with custom members
        /// </summary>
        /// <param name="tag">The encoding</param>
        /// <param name="sampleRate">Sample Rate</param>
        /// <param name="channels">Number of channels</param>
        /// <param name="averageBytesPerSecond">Average Bytes Per Second</param>
        /// <param name="blockAlign">Block Align</param>
        /// <param name="bitsPerSample">Bits Per Sample</param>
        /// <returns></returns>
        public static WaveFormat CreateCustomFormat(WAVE_FORMAT tag, int sampleRate, int channels, int averageBytesPerSecond, int blockAlign, int bitsPerSample)
        {
            return new WaveFormat
            {
                waveFormatTag = tag,
                channels = (short)channels,
                sampleRate = sampleRate,
                averageBytesPerSecond = averageBytesPerSecond,
                blockAlign = (short)blockAlign,
                bitsPerSample = (short)bitsPerSample,
                extraSize = 0
            };
        }

        /// <summary>
        /// Creates a new PCM format with the specified sample rate, bit depth and channels
        /// </summary>
        public WaveFormat(int rate, int bits, int channels)
        {
            if (channels < 1)
            {
                throw new ArgumentOutOfRangeException("channels", "Channels must be 1 or greater");
            }
            this.waveFormatTag = ((bits < 32) ? WAVE_FORMAT.PCM : WAVE_FORMAT.IEEE_FLOAT);
            this.channels = (short)channels;
            this.sampleRate = rate;
            this.bitsPerSample = (short)bits;
            this.extraSize = 0;
            this.blockAlign = (short)(channels * (bits / 8));
            this.averageBytesPerSecond = this.sampleRate * (int)this.blockAlign;
        }

        /// <summary>
        /// Creates a new 32 bit IEEE floating point wave format
        /// </summary>
        /// <param name="sampleRate">sample rate</param>
        /// <param name="channels">number of channels</param>
        public static WaveFormat CreateIeeeFloatWaveFormat(int sampleRate, int channels)
        {
            WaveFormat waveFormat = new WaveFormat
            {
                waveFormatTag = WAVE_FORMAT.IEEE_FLOAT,
                channels = (short)channels,
                bitsPerSample = 32,
                sampleRate = sampleRate,
                blockAlign = (short)(4 * channels)
            };
            waveFormat.averageBytesPerSecond = sampleRate * (int)waveFormat.blockAlign;
            waveFormat.extraSize = 0;
            return waveFormat;
        }

        /// <summary>
        /// Helper function to retrieve a WaveFormat structure from a pointer
        /// </summary>
        /// <param name="rawdata">Buffer to the WaveFormat rawdata</param>
        /// <returns>WaveFormat structure</returns>
        public unsafe static WaveFormat MarshalFrom(byte[] rawdata)
        {
            fixed (byte* ptr = rawdata)
            {
                return WaveFormat.MarshalFrom((IntPtr)(void*)ptr);
            }
        }

        /// <summary>
        /// Helper function to retrieve a WaveFormat structure from a pointer
        /// </summary>
        /// <param name="pointer">Pointer to the WaveFormat rawdata</param>
        /// <returns>WaveFormat structure</returns>
        public unsafe static WaveFormat MarshalFrom(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
            {
                return null;
            }
            WaveFormat.__PcmNative _PcmNative = *(WaveFormat.__PcmNative*)((void*)pointer);
            WAVE_FORMAT waveFormatEncoding = _PcmNative.waveFormatTag;
            if (_PcmNative.channels <= 2 && (waveFormatEncoding == WAVE_FORMAT.PCM || waveFormatEncoding == WAVE_FORMAT.IEEE_FLOAT || waveFormatEncoding == WAVE_FORMAT.WMAUDIO2 || waveFormatEncoding == WAVE_FORMAT.WMAUDIO3))
            {
                WaveFormat waveFormat = new WaveFormat();
                waveFormat.__MarshalFrom(ref _PcmNative);
                return waveFormat;
            }
            if (waveFormatEncoding == WAVE_FORMAT.EXTENSIBLE)
            {
                WaveFormatExtensible waveFormatExtensible = new WaveFormatExtensible();
                waveFormatExtensible.__MarshalFrom(ref *(WaveFormatExtensible.__Native*)((void*)pointer));
                return waveFormatExtensible;
            }
            if (waveFormatEncoding == WAVE_FORMAT.ADPCM)
            {
                WaveFormatAdpcm waveFormatAdpcm = new WaveFormatAdpcm();
                waveFormatAdpcm.__MarshalFrom(ref *(WaveFormatAdpcm.__Native*)((void*)pointer));
                return waveFormatAdpcm;
            }
            throw new InvalidOperationException(string.Format("Unsupported WaveFormat [{0}]", new object[]
			{
				waveFormatEncoding
			}));
        }

        protected unsafe virtual IntPtr MarshalToPtr()
        {
            IntPtr intPtr = Marshal.AllocHGlobal(sizeof(WaveFormat.__Native));
            this.__MarshalTo(ref *(WaveFormat.__Native*)((void*)intPtr));
            return intPtr;
        }

        /// <summary>
        /// Helper function to marshal WaveFormat to an IntPtr
        /// </summary>
        /// <param name="format">WaveFormat</param>
        /// <returns>IntPtr to WaveFormat structure (needs to be freed by callee)</returns>
        public static IntPtr MarshalToPtr(WaveFormat format)
        {
            if (format == null)
            {
                return IntPtr.Zero;
            }
            return format.MarshalToPtr();
        }

        /// <summary>
        /// Reads a new WaveFormat object from a stream
        /// </summary>
        /// <param name="br">A binary reader that wraps the stream</param>
        public WaveFormat(BinaryReader br)
        {
            int num = br.ReadInt32();
            if (num < 16)
            {
                throw new InvalidDataException("Invalid WaveFormat Structure");
            }
            this.waveFormatTag = (WAVE_FORMAT)br.ReadUInt16();
            this.channels = br.ReadInt16();
            this.sampleRate = br.ReadInt32();
            this.averageBytesPerSecond = br.ReadInt32();
            this.blockAlign = br.ReadInt16();
            this.bitsPerSample = br.ReadInt16();
            this.extraSize = 0;
            if (num > 16)
            {
                this.extraSize = br.ReadInt16();
                if ((int)this.extraSize > num - 18)
                {
                    this.extraSize = (short)(num - 18);
                }
            }
        }

        /// <summary>
        /// Reports this WaveFormat as a string
        /// </summary>
        /// <returns>String describing the wave format</returns>
        public override string ToString()
        {
            WAVE_FORMAT waveFormatEncoding = this.waveFormatTag;
            if (waveFormatEncoding == WAVE_FORMAT.EXTENSIBLE || waveFormatEncoding == WAVE_FORMAT.PCM)
            {
                return string.Format(
                    CultureInfo.InvariantCulture, 
                    "{0} bit PCM: {1}kHz {2} channels", 
                    new object[] { 
                        this.bitsPerSample, 
                        this.sampleRate / 1000,
					    this.channels });
            }
            return this.waveFormatTag.ToString();
        }

        /// <summary>
        /// Compares with another WaveFormat object
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if the objects are the same</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is WaveFormat))
            {
                return false;
            }
            WaveFormat waveFormat = (WaveFormat)obj;
            return this.waveFormatTag == waveFormat.waveFormatTag && this.channels == waveFormat.channels && this.sampleRate == waveFormat.sampleRate && this.averageBytesPerSecond == waveFormat.averageBytesPerSecond && this.blockAlign == waveFormat.blockAlign && this.bitsPerSample == waveFormat.bitsPerSample;
        }

        /// <summary>
        /// Provides a hash code for this WaveFormat
        /// </summary>
        /// <returns>A hash code</returns>
        public override int GetHashCode()
        {
            return (int)(this.waveFormatTag ^ (WAVE_FORMAT)this.channels) ^ this.sampleRate ^ this.averageBytesPerSecond ^ (int)this.blockAlign ^ (int)this.bitsPerSample;
        }
    }

    // <unmanaged>WAVE_FORMAT_ENCODING</unmanaged>	
    public enum WAVE_FORMAT : short
    {
        // <unmanaged>WAVE_FORMAT_UNKNOWN</unmanaged>	
        UNKNOWN = 0,
        // <unmanaged>WAVE_FORMAT_ADPCM</unmanaged>	
        ADPCM = 2,
        // <unmanaged>WAVE_FORMAT_IEEE_FLOAT</unmanaged>	
        IEEE_FLOAT = 3,

        // <unmanaged>WAVE_FORMAT_WMAUDIO2</unmanaged>	
        WMAUDIO2 = 353,
        // <unmanaged>WAVE_FORMAT_WMAUDIO3</unmanaged>	
        WMAUDIO3 = 354,

        // <unmanaged>WAVE_FORMAT_EXTENSIBLE</unmanaged>	
        EXTENSIBLE = -2,
        
        // <unmanaged>WAVE_FORMAT_PCM</unmanaged>	
        PCM = 1
    }
}
