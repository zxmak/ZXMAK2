//#define WRITE_RAW
/*
 * Resampler algorithm is based on the source code 
 * of unreal speccy v0.37.6 by SMT
 */
using System;
using System.Xml;
using System.Collections.Generic;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Engine.Cpu;


namespace ZXMAK2.Hardware
{
    public abstract class SoundDeviceBase : BusDeviceBase, ISoundRenderer
    {
        #region Fields

        private readonly Queue<SndOut> m_sndQueue = new Queue<SndOut>();
        private readonly Queue<SndOut> m_sndQueueNext = new Queue<SndOut>();
        
        private CpuUnit m_cpu;
        private int m_volume;
        private uint[] m_audioBuffer;   // render buffer (short|(short<<16)
        private bool m_isFrameOpen;
        private long m_startStamp;
        private int m_frameTactCount;
        private int m_lastDacTact;

        private int m_dstPos, m_dstStart;
        private uint m_clockRate, m_sampleRate;
        private uint m_tick, m_baseTick;
        private ulong m_passedClkTicks, m_passedSndTicks;
        //private uint m_multConst;
        private uint m_mix_l, m_mix_r;
        private uint m_s1_l, m_s1_r;
        private uint m_s2_l, m_s2_r;

#if WRITE_RAW
        private WavSampleWriter m_wavWriter;
#endif

        #endregion Fields


        protected SoundDeviceBase()
        {
            m_mix_l = 0x8000;
            m_mix_r = 0x8000;
            m_volume = 100;
            m_frameTactCount = 3500000 / 50;
            SampleRate = 44100;
            Category = BusDeviceCategory.Sound;
        }


        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            m_cpu = bmgr.CPU;
            var ula = bmgr.FindDevice<IUlaDevice>();
            m_frameTactCount = ula != null ? ula.FrameTactCount : 71680;
            bmgr.Events.SubscribeBeginFrame(BeginFrame);
            bmgr.Events.SubscribeEndFrame(EndFrame);
            ApplyTimings(m_frameTactCount * 50, SampleRate);
            OnProcessConfigChange(); // update volume
        }
        
        public override void BusConnect()
        {
#if WRITE_RAW
            if (GetType().Name == "AY8910")
            {
                m_wavWriter = new WavSampleWriter(this.GetType().Name + ".wav", SampleRate, 16, 2);
            }
#endif
        }

        public override void BusDisconnect()
        {
#if WRITE_RAW_WAV
            if (m_wavWriter != null)
            {
                m_wavWriter.Dispose();
            }
#endif
        }

        protected override void OnConfigLoad(XmlNode node)
        {
            base.OnConfigLoad(node);
            Volume = Utils.GetXmlAttributeAsInt32(node, "volume", Volume);
        }

        protected override void OnConfigSave(XmlNode node)
        {
            base.OnConfigSave(node);
            Utils.SetXmlAttribute(node, "volume", Volume);
        }

        #endregion


        #region ISoundRenderer

        public int SampleRate { get; set; }

        public uint[] AudioBuffer
        {
            get { return m_audioBuffer ?? new uint[882]; }
        }

        public int Volume
        {
            get { return m_volume; }
            set
            {
                value = value < 0 ? 0 : value;
                value = value > 100 ? 100 : value;
                var oldVolume = m_volume;
                m_volume = value;
                OnConfigChanged();
            }
        }

        #endregion

        #region Bus Handlers

        private void BeginFrame()
        {
            if (m_isFrameOpen)
            {
                return;
            }
            m_isFrameOpen = true;
            m_startStamp = m_cpu.Tact - (m_cpu.Tact % m_frameTactCount);
            m_lastDacTact = -1;
            OnBeginFrame();
        }

        private void EndFrame()
        {
            if (!m_isFrameOpen)
            {
                return;
            }
            OnEndFrame();
            m_isFrameOpen = false;
        }

        protected virtual void OnBeginFrame()
        {
            m_sndQueue.Clear();
            while (m_sndQueueNext.Count > 0)
            {
                m_sndQueue.Enqueue(m_sndQueueNext.Dequeue());
            }
        }

        protected virtual void OnEndFrame()
        {
            Render(m_sndQueue, m_frameTactCount);
#if WRITE_RAW
            if (m_wavWriter != null)
            {
                m_wavWriter.Write(m_audioBuffer, 0, m_audioBuffer.Length);
            }
#endif
        }

        #endregion


        protected int FrameTactCount
        {
            get { return m_frameTactCount; }
        }

        /// <summary>
        /// Returns frame time 0D...1D, the result may be more than 1D (overframe)
        /// </summary>
        protected double GetFrameTime()
        {
            var frameTact = m_cpu.Tact - m_startStamp;
            return (double)frameTact / (double)m_frameTactCount;
        }

        protected void UpdateDac(double frameTime, short left, short right)
        {
            var l = (ushort)(left - short.MinValue);
            var r = (ushort)(right - short.MinValue);
            var timestamp = (int)(frameTime * m_frameTactCount + 0.5D);
            UpdateDacInt(timestamp, l, r);
        }

        protected void UpdateDac(double frameTime, ushort left, ushort right)
        {
            var timestamp = (int)(frameTime * m_frameTactCount + 0.5D);
            UpdateDacInt(timestamp, left, right);
        }

        protected void UpdateDac(short left, short right)
        {
            var l = (ushort)(left - short.MinValue);
            var r = (ushort)(right - short.MinValue);
            var timestamp = (int)(GetFrameTime() * m_frameTactCount + 0.5D);
            UpdateDacInt(timestamp, l, r);
        }

        protected void UpdateDac(ushort left, ushort right)
        {
            var timestamp = (int)(GetFrameTime() * m_frameTactCount + 0.5D);
            UpdateDacInt(timestamp, left, right);
        }


        #region Private

        private void ApplyTimings(int frequency, int sampleRate)
        {
            if (frequency < 1 || sampleRate < 1)
            {
                Logger.Error("ApplyTimings: frequency={0}, sampleRate={1}", frequency, sampleRate);
                frequency = frequency < 1 ? 71680 * 50 : frequency;
                sampleRate = sampleRate < 1 ? 44100 : frequency;
            }
            if ((sampleRate % 50) != 0)
            {
                Logger.Error("ApplyTimings: sampleRate={1} (not a multiple of 50)", frequency, sampleRate);
                sampleRate = sampleRate - (sampleRate % 50) + 50;
            }
            m_clockRate = (uint)frequency;
            m_sampleRate = (uint)sampleRate;
            var bufferLength = (int)Math.Ceiling(m_sampleRate / 50D);
            m_audioBuffer = new uint[bufferLength];   // 44100 => 882
            m_sndQueue.Clear();
            m_sndQueueNext.Clear();

            m_tick = 0;
            m_dstPos = m_dstStart = 0;
            m_passedSndTicks = m_passedClkTicks = 0;

            //m_multConst = (uint)(((ulong)m_sampleRate << (int)(MULT_C + TICK_FF)) / m_clockRate);
        }

        private void UpdateDacInt(int timestamp, ushort left, ushort right)
        {
            if (timestamp <= m_lastDacTact)
            {
                Logger.Warn(
                    "Incorrect call to UpdateDac: timestamp={0}, previous timestamp={1}", 
                    timestamp, 
                    m_lastDacTact);
                return;
            }
            var sndout = new SndOut();
            sndout.Left = left;
            sndout.Right = right;

            var frameLength = m_frameTactCount;
            if (timestamp < frameLength)
            {
                // inframe
                sndout.Timestamp = timestamp;
                m_sndQueue.Enqueue(sndout);
            }
            else
            {
                // overframe
                sndout.Timestamp = timestamp - frameLength;
                m_sndQueueNext.Enqueue(sndout);
            }
            m_lastDacTact = sndout.Timestamp;
        }

        private void Render(Queue<SndOut> queue, int clkTicks)
        {
            try
            {
                StartFrame();
                while (queue.Count > 0)
                {
                    var src = queue.Dequeue();
                    // if (src.timestamp > clk_ticks) continue; // wrong input data leads to crash
                    Update(src.Timestamp, src.Left, src.Right);
                }
                EndFrame(clkTicks);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void StartFrame()
        {
            m_dstStart = m_dstPos = 0;
            m_baseTick = m_tick;
        }

        private void Update(int timestamp, uint left, uint right)
        {
            if ((left ^ m_mix_l) == 0 && (right ^ m_mix_r) == 0)
            {
                return;
            }

            //[vv]   unsigned endtick = (timestamp * mult_const) >> MULT_C;
            ulong endtick = ((uint)timestamp * (ulong)m_sampleRate * TICK_F) / m_clockRate;

            FlushFrame((uint)(m_baseTick + endtick));
            m_mix_l = left; m_mix_r = right;
        }

        private int EndFrame(int clkTicks)
        {
            // adjusting 'clk_ticks' with whole history will fix accumulation of rounding errors
            //uint64_t endtick = ((passed_clk_ticks + clk_ticks) * mult_const) >> MULT_C;
            ulong endtick = ((m_passedClkTicks + (uint)clkTicks) * (ulong)m_sampleRate * TICK_F) / m_clockRate;
            FlushFrame((uint)(endtick - m_passedSndTicks));

            var readySamples = m_dstPos - m_dstStart;

            m_tick -= (uint)(readySamples << (int)TICK_FF);
            m_passedSndTicks += (uint)(readySamples << (int)TICK_FF);
            m_passedClkTicks += (uint)clkTicks;

            return readySamples;
        }

        private void FlushFrame(uint endTick)
        {
            uint scale;
            if (((endTick ^ m_tick) & ~(TICK_F - 1)) == 0)
            {
                //same discrete as before
                scale = s_filterDiff[(endTick & (TICK_F - 1)) + TICK_F] - s_filterDiff[(m_tick & (TICK_F - 1)) + TICK_F];
                m_s2_l += m_mix_l * scale;
                m_s2_r += m_mix_r * scale;

                scale = s_filterDiff[endTick & (TICK_F - 1)] - s_filterDiff[m_tick & (TICK_F - 1)];
                m_s1_l += m_mix_l * scale;
                m_s1_r += m_mix_r * scale;

                m_tick = endTick;
            }
            else if (m_dstPos < m_audioBuffer.Length) // check buffer overlap
            {
                scale = FilterSumFullU - s_filterDiff[(m_tick & (TICK_F - 1)) + TICK_F];

                uint sampleValue = ((m_mix_l * scale + m_s2_l) >> 16) |
                    ((m_mix_r * scale + m_s2_r) & 0xFFFF0000);

                // [write sample]
                m_audioBuffer[m_dstPos] = GetSigned(sampleValue);
                m_dstPos++;

                scale = FilterSumHalfU - s_filterDiff[m_tick & (TICK_F - 1)];
                m_s2_l = m_s1_l + m_mix_l * scale;
                m_s2_r = m_s1_r + m_mix_r * scale;

                m_tick = (m_tick | (TICK_F - 1)) + 1;

                if (((endTick ^ m_tick) & ~(TICK_F - 1)) != 0)
                {
                    // assume s_filterCoeff is symmetric
                    uint val_l = m_mix_l * FilterSumHalfU;
                    uint val_r = m_mix_r * FilterSumHalfU;
                    do
                    {
                        // check buffer overlap
                        if (m_dstPos >= m_audioBuffer.Length)
                        {
                            break;
                        }
                        uint sampleValue2 = ((m_s2_l + val_l) >> 16) +
                            ((m_s2_r + val_r) & 0xFFFF0000); // save s2+val

                        // [write sample]
                        m_audioBuffer[m_dstPos] = GetSigned(sampleValue2);
                        m_dstPos++;

                        m_tick += TICK_F;
                        m_s2_l = val_l; m_s2_r = val_r; // s2=s1, s1=0;

                    } while (((endTick ^ m_tick) & ~(TICK_F - 1)) != 0);
                }

                m_tick = endTick;

                scale = s_filterDiff[(endTick & (TICK_F - 1)) + TICK_F] - FilterSumHalfU;
                m_s2_l += m_mix_l * scale;
                m_s2_r += m_mix_r * scale;

                scale = s_filterDiff[endTick & (TICK_F - 1)];
                m_s1_l = m_mix_l * scale;
                m_s1_r = m_mix_r * scale;
            }
        }

        private static uint GetSigned(uint sample)
        {
            var left = (ushort)sample + short.MinValue;
            var right = (ushort)(sample>>16) + short.MinValue;
            return (uint)((ushort)left | ((ushort)right << 16));
        }

        private class SndOut
        {
            public int Timestamp; // in 'system clock' ticks
            public uint Left;
            public uint Right;
        }

        #endregion Private


        #region Filter

        static SoundDeviceBase()
        {
            s_filterDiff = new uint[TICK_F * 2];
            var sum = 0D;
            for (var i = 0; i < TICK_F * 2; i++)
            {
                s_filterDiff[i] = (uint)(int)(sum * 0x10000);
                sum += s_filterCoeff[i];
            }
        }

        private const double FilterSumFull = 1.0D;
        private const double FilterSumHalf = 0.5D;
        private const uint FilterSumFullU = (uint)(FilterSumFull * 0x10000);
        private const uint FilterSumHalfU = (uint)(FilterSumHalf * 0x10000);

        private const uint TICK_FF = 6;			// oversampling ratio: 2^6 = 64
        private const uint TICK_F = 1 << (int)TICK_FF;
        private const uint MULT_C = 12;			// fixed point precision for 'system tick -> sound tick'

        private static readonly uint[] s_filterDiff;
        
        private static readonly double[] s_filterCoeff = new double[]// [TICK_F*2]
        {
            // filter designed with Matlab's DSP toolbox
            0.000797243121022152, 0.000815206499600866, 0.000844792477531490, 0.000886460636664257,
            0.000940630171246217, 0.001007677515787512, 0.001087934129054332, 0.001181684445143001,
            0.001289164001921830, 0.001410557756409498, 0.001545998595893740, 0.001695566052785407,
            0.001859285230354019, 0.002037125945605404, 0.002229002094643918, 0.002434771244914945,
            0.002654234457752337, 0.002887136343664226, 0.003133165351783907, 0.003391954293894633,
            0.003663081102412781, 0.003946069820687711, 0.004240391822953223, 0.004545467260249598,
            0.004860666727631453, 0.005185313146989532, 0.005518683858848785, 0.005860012915564928,
            0.006208493567431684, 0.006563280932335042, 0.006923494838753613, 0.007288222831108771,
            0.007656523325719262, 0.008027428904915214, 0.008399949736219575, 0.008773077102914008,
            0.009145787031773989, 0.009517044003286715, 0.009885804729257883, 0.010251021982371376,
            0.010611648461991030, 0.010966640680287394, 0.011314962852635887, 0.011655590776166550,
            0.011987515680350414, 0.012309748033583185, 0.012621321289873522, 0.012921295559959939,
            0.013208761191466523, 0.013482842243062109, 0.013742699838008606, 0.013987535382970279,
            0.014216593638504731, 0.014429165628265581, 0.014624591374614174, 0.014802262449059521,
            0.014961624326719471, 0.015102178534818147, 0.015223484586101132, 0.015325161688957322,
            0.015406890226980602, 0.015468413001680802, 0.015509536233058410, 0.015530130313785910,
            0.015530130313785910, 0.015509536233058410, 0.015468413001680802, 0.015406890226980602,
            0.015325161688957322, 0.015223484586101132, 0.015102178534818147, 0.014961624326719471,
            0.014802262449059521, 0.014624591374614174, 0.014429165628265581, 0.014216593638504731,
            0.013987535382970279, 0.013742699838008606, 0.013482842243062109, 0.013208761191466523,
            0.012921295559959939, 0.012621321289873522, 0.012309748033583185, 0.011987515680350414,
            0.011655590776166550, 0.011314962852635887, 0.010966640680287394, 0.010611648461991030,
            0.010251021982371376, 0.009885804729257883, 0.009517044003286715, 0.009145787031773989,
            0.008773077102914008, 0.008399949736219575, 0.008027428904915214, 0.007656523325719262,
            0.007288222831108771, 0.006923494838753613, 0.006563280932335042, 0.006208493567431684,
            0.005860012915564928, 0.005518683858848785, 0.005185313146989532, 0.004860666727631453,
            0.004545467260249598, 0.004240391822953223, 0.003946069820687711, 0.003663081102412781,
            0.003391954293894633, 0.003133165351783907, 0.002887136343664226, 0.002654234457752337,
            0.002434771244914945, 0.002229002094643918, 0.002037125945605404, 0.001859285230354019,
            0.001695566052785407, 0.001545998595893740, 0.001410557756409498, 0.001289164001921830,
            0.001181684445143001, 0.001087934129054332, 0.001007677515787512, 0.000940630171246217,
            0.000886460636664257, 0.000844792477531490, 0.000815206499600866, 0.000797243121022152
        };
        
        #endregion Filter
    }

#if WRITE_RAW
    public class WavSampleWriter : IDisposable
    {
        private string m_fileName;
        private int m_sampleRate;
        private int m_sampleBits;
        private int m_channelCount;
        private System.IO.FileStream m_fileStream;
        private RIFF_HDR m_riffHdr = new RIFF_HDR();
        private RIFF_CHUNK_FMT m_riffChunkFmt = new RIFF_CHUNK_FMT();
        private RIFF_CHUNK_FACT m_riffChunkFact = new RIFF_CHUNK_FACT();

        public WavSampleWriter(string fileName, int sampleRate, int sampleBits, int channelCount)
        {
            m_fileName = fileName;
            m_sampleRate = sampleRate;
            m_sampleBits = sampleBits;
            m_channelCount = channelCount;
            m_fileStream = new System.IO.FileStream(
                fileName,
                System.IO.FileMode.Create,
                System.IO.FileAccess.Write,
                System.IO.FileShare.Read);

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
            m_fileStream.Seek(0, System.IO.SeekOrigin.End);
            for (int i = 0; i < length; i++)
                ZXMAK2.Engine.Tools.StreamHelper.Write(m_fileStream, buffer[i + offset]);

            m_riffHdr.RiffSize = (uint)m_fileStream.Length - 8;
            m_riffChunkFact.value += (uint)length;

            m_fileStream.Seek(0, System.IO.SeekOrigin.Begin);
            m_riffHdr.Write(m_fileStream);
            m_riffChunkFmt.Write(m_fileStream);
            m_riffChunkFact.Write(m_fileStream);
            RIFF_CHUNK_DATA riffData = new RIFF_CHUNK_DATA(m_riffChunkFact.value * m_riffChunkFmt.wBlockAlign);
            riffData.Write(m_fileStream);
        }


        public void Dispose()
        {
            m_fileStream.Close();
        }


        private class RIFF_HDR      // size = 3*4 (align 4)
        {
            public UInt32 riff_id = BitConverter.ToUInt32(System.Text.Encoding.ASCII.GetBytes("RIFF"), 0);
            public UInt32 RiffSize;  // data after riff_hdr length
            public UInt32 wave_id = BitConverter.ToUInt32(System.Text.Encoding.ASCII.GetBytes("WAVE"), 0);

            public void Write(System.IO.FileStream stream)
            {
                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, riff_id);
                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, RiffSize);
                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, wave_id);
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
                id = BitConverter.ToUInt32(System.Text.Encoding.ASCII.GetBytes("fmt "), 0);
                len = 0x12;
            }

            public void Write(System.IO.FileStream stream)
            {
                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, id);
                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, len);

                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, wFormatTag);
                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, wChannels);
                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, dwSamplesPerSec);
                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, dwAvgBytesPerSec);
                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, wBlockAlign);
                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, wBitsPerSample);
                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, __zero);
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
                id = BitConverter.ToUInt32(System.Text.Encoding.ASCII.GetBytes("fact"), 0);
                len = 4;
            }

            public void Write(System.IO.FileStream stream)
            {
                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, id);
                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, len);
                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, value);
            }

            public const uint Size = 2 * 4 + 4;
        }
        private class RIFF_CHUNK_DATA    // size = 2*4 + ...
        {
            public UInt32 id;	// identifier, e.g. "fmt " or "data"
            public UInt32 len;	// remaining chunk length after header

            public RIFF_CHUNK_DATA(uint length)
            {
                id = BitConverter.ToUInt32(System.Text.Encoding.ASCII.GetBytes("data"), 0);
                len = length;
            }

            public void Write(System.IO.FileStream stream)
            {
                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, id);
                ZXMAK2.Engine.Tools.StreamHelper.Write(stream, len);
            }

            public const uint Size = 2 * 4;
        }
    }
#endif
}
