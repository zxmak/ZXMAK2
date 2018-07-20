using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Serializers.SnapshotSerializers;
using ZXMAK2.Model.Tape.Interfaces;
using ZXMAK2.Model.Tape.Entities;
using ZXMAK2.Engine.Tools;


namespace ZXMAK2.Serializers.TapeSerializers
{
    public class WavSerializer : FormatSerializer
    {
        private ITapeDevice m_tape;


        public WavSerializer(ITapeDevice tape)
        {
            m_tape = tape;
        }


        #region FormatSerializer

        public override string FormatGroup { get { return "Tape images"; } }
        public override string FormatName { get { return "WAV image"; } }
        public override string FormatExtension { get { return "WAV"; } }

        public override bool CanDeserialize { get { return true; } }

        public override void Deserialize(Stream stream)
        {
            m_tape.Blocks.Clear();
            var blocks = Load(stream, m_tape.TactsPerSecond);
            if (blocks != null)
            {
                m_tape.Blocks.AddRange(blocks);
            }
            m_tape.Reset();
        }

        private static IEnumerable<ITapeBlock> Load(Stream stream, int tactsPerSecond)
        {
            var reader = new WavStreamReader(stream);

            var ratio = tactsPerSecond / (double)reader.Header.FmtSampleRate; // usually 3.5mhz / 44khz

            var rleData = LoadRleData(reader);

            var list = new List<TapeBlock>();
            var pulses = new List<int>();
            var blockTime = 0;
            var blockCounter = 0;
            foreach (var rle in rleData)
            {
                var len = (int)Math.Round(rle * ratio, MidpointRounding.AwayFromZero);
                pulses.Add(len);

                blockTime += len;
                if (blockTime >= tactsPerSecond * 2)
                {
                    var tb = new TapeBlock();
                    tb.Description = string.Format("WAV-{0:D3}", blockCounter++);
                    tb.Periods = new List<int>(pulses);
                    list.Add(tb);
                    blockTime = 0;
                    pulses.Clear();
                }
            }
            if (pulses.Count > 0)
            {
                var tb = new TapeBlock();
                tb.Description = string.Format("WAV-{0:D3}", blockCounter++);
                tb.Periods = new List<int>(pulses);
                list.Add(tb);
            }
            return list;
        }

        #endregion


        #region Private

        private static IEnumerable<int> LoadRleData(WavStreamReader reader)
        {
            var list = new List<int>();
            var smpCounter = 0;
            var state = reader.ReadNext();
            for (var i = 0; i < reader.Count; i++)
            {
                var sample = reader.ReadNext();
                smpCounter++;
                if ((state < 0 && sample < 0) ||
                    (state >= 0 && sample >= 0))
                {
                    continue;
                }
                list.Add(smpCounter);
                smpCounter = 0;
                state = sample;
            }
            return list;
        }

        #endregion Private
    }

    public class WavStreamReader
    {
        private Stream m_stream;
        private WavHeader m_header = new WavHeader();

        public WavStreamReader(Stream stream)
        {
            m_stream = stream;
            m_header.Deserialize(stream);
        }

        public WavHeader Header { get { return m_header; } }

        public int Count { get { return m_header.DataSize / m_header.FmtBlockAlign; } }

        public Int32 ReadNext()
        {
            // check - sample should be in PCM format
            if (m_header.FmtCode != WAVE_FORMAT_PCM &&
                m_header.FmtCode != WAVE_FORMAT_IEEE_FLOAT)
            {
                throw new FormatException(string.Format(
                    "Not supported audio format: fmtCode={0}, bitDepth={1}",
                    m_header.FmtCode,
                    m_header.FmtBitDepth));
            }
            byte[] data = new byte[m_header.FmtBlockAlign];
            m_stream.Read(data, 0, data.Length);
            if (m_header.FmtCode == WAVE_FORMAT_PCM)
            {
                // use first channel only
                if (m_header.FmtBitDepth == 8)
                    return GetSamplePcm8(data, 0, 0);
                if (m_header.FmtBitDepth == 16)
                    return GetSamplePcm16(data, 0, 0);
                if (m_header.FmtBitDepth == 24)
                    return GetSamplePcm24(data, 0, 0);
                if (m_header.FmtBitDepth == 32)
                    return GetSamplePcm32(data, 0, 0);
            }
            else if (m_header.FmtCode == WAVE_FORMAT_IEEE_FLOAT)
            {
                // use first channel only
                if (m_header.FmtBitDepth == 32)
                    return GetSampleFloat32(data, 0, 0);
                if (m_header.FmtBitDepth == 64)
                    return GetSampleFloat64(data, 0, 0);
            }
            throw new NotSupportedException(string.Format(
                "Not supported audio format ({0}/{1} bit)",
                m_header.FmtCode == WAVE_FORMAT_PCM ? "PCM" : "FLOAT",
                m_header.FmtBitDepth));
        }

        private static Int32 GetSamplePcm8(byte[] bufferRaw, int offset, int channel)
        {
            return bufferRaw[offset + channel] - 128;
        }

        private static Int32 GetSamplePcm16(byte[] bufferRaw, int offset, int channel)
        {
            return BitConverter.ToInt16(bufferRaw, offset + 2 * channel);
        }

        private static Int32 GetSamplePcm24(byte[] bufferRaw, int offset, int channel)
        {
            Int32 result;
            int subOffset = offset + channel * 3;
            if (BitConverter.IsLittleEndian)
            {
                result = ((sbyte)bufferRaw[2 + subOffset]) * 0x10000;
                result |= bufferRaw[1 + subOffset] * 0x100;
                result |= bufferRaw[0 + subOffset];
            }
            else
            {
                result = ((sbyte)bufferRaw[0 + subOffset]) * 0x10000;
                result |= bufferRaw[1 + subOffset] * 0x100;
                result |= bufferRaw[2 + subOffset];
            }
            return result;
        }

        private static Int32 GetSamplePcm32(byte[] bufferRaw, int offset, int channel)
        {
            return BitConverter.ToInt32(bufferRaw, offset + 4 * channel);
        }

        private static Int32 GetSampleFloat32(byte[] data, int offset, int channel)
        {
            float fSample = BitConverter.ToSingle(data, offset + 4 * channel);
            // convert to 32 bit integer
            return (Int32)(fSample * Int32.MaxValue);
        }

        private static Int32 GetSampleFloat64(byte[] data, int offset, int channel)
        {
            double fSample = BitConverter.ToDouble(data, offset + 8 * channel);
            // convert to 32 bit integer
            return (Int32)(fSample * Int32.MaxValue);
        }

        private const int WAVE_FORMAT_PCM = 1;              /* PCM */
        private const int WAVE_FORMAT_IEEE_FLOAT = 3;       /* IEEE float */
        private const int WAVE_FORMAT_ALAW = 6;             /* 8-bit ITU-T G.711 A-law */
        private const int WAVE_FORMAT_MULAW = 7;            /* 8-bit ITU-T G.711 µ-law */
        private const int WAVE_FORMAT_EXTENSIBLE = 0xFFFE;  /* Determined by SubFormat */
    }

    public class WavHeader
    {
        // RIFF chunk (12 bytes)
        public Int32 RiffChunkId;           // "RIFF"
        public Int32 RiffFileSize;
        public Int32 RiffType;          // "WAVE"

        // Format chunk (24 bytes)
        public Int32 FmtId;             // "fmt "
        public Int32 FmtSize;
        public Int16 FmtCode;
        public Int16 FmtChannels;
        public Int32 FmtSampleRate;
        public Int32 FmtAvgBps;
        public Int16 FmtBlockAlign;
        public Int16 FmtBitDepth;
        public Int16 FmtExtraSize;

        // Data chunk
        public Int32 DataId;            // "data"
        public Int32 DataSize;          // The data size should be file size - 36 bytes.


        public void Deserialize(Stream stream)
        {
            StreamHelper.Read(stream, out RiffChunkId);
            StreamHelper.Read(stream, out RiffFileSize);
            StreamHelper.Read(stream, out RiffType);
            if (RiffChunkId != BitConverter.ToInt32(Encoding.ASCII.GetBytes("RIFF"), 0))
            {
                throw new FormatException("Invalid WAV file header");
            }
            if (RiffType != BitConverter.ToInt32(Encoding.ASCII.GetBytes("WAVE"), 0))
            {
                throw new FormatException(string.Format(
                    "Not supported RIFF type: '{0}'",
                    Encoding.ASCII.GetString(BitConverter.GetBytes(RiffType))));
            }
            Int32 chunkId;
            Int32 chunkSize;
            while (stream.Position < stream.Length)
            {
                StreamHelper.Read(stream, out chunkId);
                StreamHelper.Read(stream, out chunkSize);
                string strChunkId = Encoding.ASCII.GetString(
                    BitConverter.GetBytes(chunkId));
                if (strChunkId == "fmt ")
                {
                    ReadFmt(stream, chunkId, chunkSize);
                }
                else if (strChunkId == "data")
                {
                    ReadData(stream, chunkId, chunkSize);
                    break;
                }
                else
                {
                    stream.Seek(chunkSize, SeekOrigin.Current);
                }
            }
            if (FmtId != BitConverter.ToInt32(Encoding.ASCII.GetBytes("fmt "), 0))
            {
                throw new FormatException("WAV format chunk not found");
            }
            if (DataId != BitConverter.ToInt32(Encoding.ASCII.GetBytes("data"), 0))
            {
                throw new FormatException("WAV data chunk not found");
            }
        }

        private void ReadData(Stream stream, int chunkId, int chunkSize)
        {
            DataId = chunkId;
            DataSize = chunkSize;
        }

        private void ReadFmt(Stream stream, int chunkId, int chunkSize)
        {
            FmtId = chunkId;
            FmtSize = chunkSize;
            StreamHelper.Read(stream, out FmtCode);
            StreamHelper.Read(stream, out FmtChannels);
            StreamHelper.Read(stream, out FmtSampleRate);
            StreamHelper.Read(stream, out FmtAvgBps);
            StreamHelper.Read(stream, out FmtBlockAlign);
            StreamHelper.Read(stream, out FmtBitDepth);
            if (FmtSize == 18)
            {
                // Read any extra values
                StreamHelper.Read(stream, out FmtExtraSize);
                stream.Seek(FmtExtraSize, SeekOrigin.Current);
            }
        }
    }
}
