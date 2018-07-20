using System;
using System.Linq;
using System.IO;
using System.Text;
using ZXMAK2.Engine.Tools;


namespace ZXMAK2.Host.Media.Audio
{
    public class WavSampleWriter : IDisposable
    {
        private readonly string m_fileName;
        private readonly int m_sampleRate;
        private readonly FileStream m_fileStream;
        private readonly RIFF_HDR m_riffHdr = new RIFF_HDR();
        private readonly RIFF_CHUNK_FMT m_riffChunkFmt = new RIFF_CHUNK_FMT();
        private readonly RIFF_CHUNK_FACT m_riffChunkFact = new RIFF_CHUNK_FACT();

        public WavSampleWriter(string fileName, int sampleRate)
        {
            const int sampleBits = 16;
            const int channelCount = 2;
            m_fileName = fileName;
            m_sampleRate = sampleRate;
            m_fileStream = new System.IO.FileStream(
                fileName,
                FileMode.Create,
                FileAccess.Write,
                FileShare.Read);

            m_riffChunkFact.value = 0;

            m_riffChunkFmt.wFormatTag = 0x0001;
            m_riffChunkFmt.wChannels = (ushort)channelCount;
            m_riffChunkFmt.dwSamplesPerSec = (uint)sampleRate;
            m_riffChunkFmt.wBitsPerSample = (ushort)sampleBits;
            m_riffChunkFmt.wBlockAlign = (ushort)(m_riffChunkFmt.wChannels * m_riffChunkFmt.wBitsPerSample / 8);  //0x0004;
            m_riffChunkFmt.dwAvgBytesPerSec = m_riffChunkFmt.dwSamplesPerSec * m_riffChunkFmt.wBlockAlign;

            m_riffHdr.RiffSize = 4 +						// wave_id	
                RIFF_CHUNK_FMT.Size +						// FMT
                RIFF_CHUNK_FACT.Size;						// FACT

            m_fileStream.Seek(0, System.IO.SeekOrigin.Begin);
            m_riffHdr.Write(m_fileStream);
            m_riffChunkFmt.Write(m_fileStream);
            m_riffChunkFact.Write(m_fileStream);
            RIFF_CHUNK_DATA riffData = new RIFF_CHUNK_DATA(m_riffChunkFact.value);
            riffData.Write(m_fileStream);
        }

        public void Write(uint[] buffer, int offset, int length)
        {
            m_fileStream.Seek(0, SeekOrigin.End);
            for (var i = 0; i < length; i++)
            {
                StreamHelper.Write(m_fileStream, buffer[i + offset]);
            }

            m_riffHdr.RiffSize = (uint)m_fileStream.Length - 8;
            m_riffChunkFact.value += (uint)length;

            m_fileStream.Seek(0, SeekOrigin.Begin);
            m_riffHdr.Write(m_fileStream);
            m_riffChunkFmt.Write(m_fileStream);
            m_riffChunkFact.Write(m_fileStream);
            var riffData = new RIFF_CHUNK_DATA(m_riffChunkFact.value * m_riffChunkFmt.wBlockAlign);
            riffData.Write(m_fileStream);
        }

        public void Dispose()
        {
            m_fileStream.Close();
        }


        private class RIFF_HDR      // size = 3*4 (align 4)
        {
            public UInt32 riff_id = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("RIFF"), 0);
            public UInt32 RiffSize;  // data after riff_hdr length
            public UInt32 wave_id = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("WAVE"), 0);

            public void Write(Stream stream)
            {
                StreamHelper.Write(stream, riff_id);
                StreamHelper.Write(stream, RiffSize);
                StreamHelper.Write(stream, wave_id);
            }

            public const uint Size = 3 * 4;
        }

        private class RIFF_CHUNK_FMT    // size = 2*4 + 0x12
        {
            public UInt32 id;	// identifier, e.g. "fmt " or "data"
            public UInt32 len;	// remaining chunk length after header

            public UInt16 wFormatTag;         // Format category
            public UInt16 wChannels;          // Number of channels
            public UInt32 dwSamplesPerSec;   // Sampling rate
            public UInt32 dwAvgBytesPerSec;  // For buffer estimation
            public UInt16 wBlockAlign;        // Data block size
            public UInt16 wBitsPerSample;     // Sample size
            public UInt16 __zero = 0x0000;

            public RIFF_CHUNK_FMT()
            {
                id = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("fmt "), 0);
                len = 0x12;
            }

            public void Write(Stream stream)
            {
                StreamHelper.Write(stream, id);
                StreamHelper.Write(stream, len);

                StreamHelper.Write(stream, wFormatTag);
                StreamHelper.Write(stream, wChannels);
                StreamHelper.Write(stream, dwSamplesPerSec);
                StreamHelper.Write(stream, dwAvgBytesPerSec);
                StreamHelper.Write(stream, wBlockAlign);
                StreamHelper.Write(stream, wBitsPerSample);
                StreamHelper.Write(stream, __zero);
            }

            public const uint Size = 2 * 4 + 0x12;
        }

        private class RIFF_CHUNK_FACT    // size = 2*4 + 4
        {
            public UInt32 id;	// identifier, e.g. "fmt " or "data"
            public UInt32 len;	// remaining chunk length after header

            public UInt32 value;

            public RIFF_CHUNK_FACT()
            {
                id = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("fact"), 0);
                len = 4;
            }

            public void Write(Stream stream)
            {
                StreamHelper.Write(stream, id);
                StreamHelper.Write(stream, len);
                StreamHelper.Write(stream, value);
            }

            public const uint Size = 2 * 4 + 4;
        }

        private class RIFF_CHUNK_DATA    // size = 2*4 + ...
        {
            public UInt32 id;	// identifier, e.g. "fmt " or "data"
            public UInt32 len;	// remaining chunk length after header

            public RIFF_CHUNK_DATA(uint length)
            {
                id = BitConverter.ToUInt32(Encoding.ASCII.GetBytes("data"), 0);
                len = length;
            }

            public void Write(Stream stream)
            {
                StreamHelper.Write(stream, id);
                StreamHelper.Write(stream, len);
            }

            public const uint Size = 2 * 4;
        }
    }
}
