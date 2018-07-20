/*
 * Programmable Sound Generator
 * AY-3-8910 / YM2149F chip emulator
 * Mixer algorithms & amplitude tables is based on 
 * the source code of unreal speccy v0.38 by SMT
 */
using System;
using System.Collections.Generic;


namespace ZXMAK2.Hardware.Circuits.Sound
{
    public class PsgChip : IPsgChip
    {
        #region Fields

        private readonly Dictionary<PanType, uint[]> m_panTables = new Dictionary<PanType, uint[]>();
        private readonly Dictionary<AmpType, uint[]> m_ampTables = new Dictionary<AmpType, uint[]>();
        private readonly byte[] m_regs = new byte[256];
        
        private int m_chipFrequency;
        private double m_renderStep;
        
        private int m_volume;
        private AmpType m_ampType;
        private PanType m_panType;


        private int m_cntA;         // channel generator counter
        private int m_cntB;
        private int m_cntC;
        private int m_cntN;         // noise generator counter
        private int m_cntE;         // envelope generator couner
        private int m_divA;         // channel generator divider
        private int m_divB;
        private int m_divC;
        private int m_divN;         // noise generator divider
        private int m_divE;         // envelope generator divider
        private int m_bitA;         // channel generator state (0 / -1)
        private int m_bitB;
        private int m_bitC;
        private int m_bitN;         // noise generator state (0 / -1)
        private int m_seedN;        // noise seed
        private int m_bit0;         // mixer state
        private int m_bit1;
        private int m_bit2;
        private int m_bit3;
        private int m_bit4;
        private int m_bit5;
        private int m_ea;           // channel envelope mask (0 - amp; -1 - envelope)
        private int m_eb;
        private int m_ec;
        private int m_va;           // channel amplitude (0 for envelope)
        private int m_vb;
        private int m_vc;
        private int m_env;          // envelope state
        private int m_denv;         // envelope direction

        private uint[][] m_volumes; // DAC tables

        #endregion Fields


        public PsgChip()
        {
            m_ampTables.Add(AmpType.Ay8910, SoundTables.AmplitudeAy8910);
            m_ampTables.Add(AmpType.Ym2149F, SoundTables.AmplitudeYm2149);
            m_ampTables.Add(AmpType.Ym2203, SoundTables.AmplitudeYm2149);
            m_ampTables.Add(AmpType.Custom, SoundTables.AmplitudeCustom);

            m_panTables.Add(PanType.Abc, SoundTables.PanAbc);
            m_panTables.Add(PanType.Acb, SoundTables.PanAcb);
            m_panTables.Add(PanType.Bac, SoundTables.PanBac);
            m_panTables.Add(PanType.Bca, SoundTables.PanBca);
            m_panTables.Add(PanType.Cab, SoundTables.PanCab);
            m_panTables.Add(PanType.Cba, SoundTables.PanCba);
            m_panTables.Add(PanType.Mono, SoundTables.PanMono);
            m_panTables.Add(PanType.Custom, SoundTables.PanCustom);

            ChipFrequency = 1773400;    // default ZX Spectrum 128
            //Volume = 100;
            m_volume = 100;
            UpdateVolumeTables();
            Reset();
        }


        #region Properties

        public int ChipFrequency
        {
            get { return m_chipFrequency; }
            set
            {
                m_chipFrequency = Math.Max(496, value);
                m_renderStep = 50D * 8D / m_chipFrequency;
            }
        }

        public int Volume
        {
            get { return m_volume; }
            set
            {
                value = Math.Max(0, value);     // [0..100]
                value = Math.Min(100, value);
                if (m_volume == value)
                {
                    return;
                }
                m_volume = value;
                UpdateVolumeTables();
            }
        }

        public AmpType AmpType
        {
            get { return m_ampType; }
            set
            {
                if (m_ampType == value)
                {
                    return;
                }
                m_ampType = value;
                UpdateVolumeTables();
            }
        }

        public PanType PanType
        {
            get { return m_panType; }
            set
            {
                if (m_panType == value)
                {
                    return;
                }
                m_panType = value;
                UpdateVolumeTables();
            }
        }

        public byte RegAddr { get; set; }

        public Action<double, ushort, ushort> UpdateHandler { get; set; }
        
        #endregion Properties


        #region Public

        public void Reset()
        {
            for (var i = 0; i < m_regs.Length; i++)
            {
                SetReg(i, 0);
            }
            RegAddr = 0;
        }

        public byte GetReg(int index)
        {
            if (index >= m_regs.Length)
            {
                return 0xFF;
            }
            return m_regs[index];
        }

        public void SetReg(int index, byte value)
        {
            SetReg(-1, -1, index, (byte)(value ^ 1));
            SetReg(-1, -1, index, value);
        }

        public double SetReg(
            double lastTime,
            double time,
            int index,
            byte value)
        {
            if (index >= 0x10)
            {
                return lastTime;
            }
            if (((1 << index) & ((1 << 1) | (1 << 3) | (1 << 5) | (1 << 13))) != 0)
            {
                value &= 0x0F;
            }
            if (((1 << index) & ((1 << 6) | (1 << 8) | (1 << 9) | (1 << 10))) != 0)
            {
                value &= 0x1F;
            }
            if (index != 13 && m_regs[index] == value)
            {
                return lastTime;
            }
            m_regs[index] = value;

            if (lastTime >= 0D)
            {
                lastTime = Update(lastTime, time);
            }

            switch (index)
            {
                case 0:
                case 1:
                    m_divA = m_regs[PsgRegId.FREQA_FINE] | (m_regs[PsgRegId.FREQA_COARSE] << 8);
                    break;
                case 2:
                case 3:
                    m_divB = m_regs[PsgRegId.FREQB_FINE] | (m_regs[PsgRegId.FREQB_COARSE] << 8);
                    break;
                case 4:
                case 5:
                    m_divC = m_regs[PsgRegId.FREQC_FINE] | (m_regs[PsgRegId.FREQC_COARSE] << 8);
                    break;
                case 6:
                    m_divN = value*2;
                    break;
                case 7:
                    m_bit0 = 0 - ((value >> 0) & 1);
                    m_bit1 = 0 - ((value >> 1) & 1);
                    m_bit2 = 0 - ((value >> 2) & 1);
                    m_bit3 = 0 - ((value >> 3) & 1);
                    m_bit4 = 0 - ((value >> 4) & 1);
                    m_bit5 = 0 - ((value >> 5) & 1);
                    break;
                case 8:
                    m_ea = (value & 0x10) != 0 ? -1 : 0;
                    m_va = ((value & 0x0F) * 2 + 1) & ~m_ea;
                    break;
                case 9:
                    m_eb = (value & 0x10) != 0 ? -1 : 0;
                    m_vb = ((value & 0x0F) * 2 + 1) & ~m_eb;
                    break;
                case 10:
                    m_ec = (value & 0x10) != 0 ? -1 : 0;
                    m_vc = ((value & 0x0F) * 2 + 1) & ~m_ec;
                    break;
                case 11:
                case 12:
                    m_divE = m_regs[PsgRegId.ENVELOPE_FINE] | (m_regs[PsgRegId.ENVELOPE_COARSE] << 8);
                    break;
                case 13:
                    m_cntE = 0;
                    if ((m_regs[PsgRegId.ENVELOPE_SHAPE] & 4) != 0)
                    {
                        m_env = 0;      // attack
                        m_denv = 1;
                    }
                    else
                    {
                        m_env = 31;     // decay
                        m_denv = -1;
                    }
                    break;
            }
            return lastTime;
        }

        public double Update(double startTime, double endTime)
        {
            var updateHandler = UpdateHandler;
            if (updateHandler == null)
            {
                return startTime;
            }
            for (; startTime < endTime; startTime += m_renderStep)
            {
                if (++m_cntA >= m_divA)
                {
                    m_cntA = 0;
                    m_bitA ^= -1;
                }
                if (++m_cntB >= m_divB)
                {
                    m_cntB = 0;
                    m_bitB ^= -1;
                }
                if (++m_cntC >= m_divC)
                {
                    m_cntC = 0;
                    m_bitC ^= -1;
                }
                if (++m_cntN >= m_divN)
                {
                    m_cntN = 0;
                    m_seedN = (m_seedN * 2 + 1) ^ (((m_seedN >> 16) ^ (m_seedN >> 13)) & 1);
                    m_bitN = 0 - ((m_seedN >> 16) & 1);
                }
                if (++m_cntE >= m_divE)
                {
                    m_cntE = 0;
                    m_env += m_denv;
                    if ((m_env & ~31) != 0)
                    {
                        var mask = 1 << m_regs[PsgRegId.ENVELOPE_SHAPE];
                        if ((mask & ((1 << 0) | (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5) | (1 << 6) | (1 << 7) | (1 << 9) | (1 << 15))) != 0)
                        {
                            m_env = m_denv = 0;
                        }
                        else if ((mask & ((1 << 8) | (1 << 12))) != 0)
                        {
                            m_env &= 31;
                        }
                        else if ((mask & ((1 << 10) | (1 << 14))) != 0)
                        {
                            m_denv = -m_denv;
                            m_env = m_env + m_denv;
                        }
                        else //11,13
                        {
                            m_env = 31;
                            m_denv = 0;
                        }
                    }
                }
                var mixA = ((m_ea & m_env) | m_va) & ((m_bitA | m_bit0) & (m_bitN | m_bit3));
                var mixB = ((m_eb & m_env) | m_vb) & ((m_bitB | m_bit1) & (m_bitN | m_bit4));
                var mixC = ((m_ec & m_env) | m_vc) & ((m_bitC | m_bit2) & (m_bitN | m_bit5));

                var left = m_volumes[0][mixA];
                var right = m_volumes[1][mixA];
                left += m_volumes[2][mixB];
                right += m_volumes[3][mixB];
                left += m_volumes[4][mixC];
                right += m_volumes[5][mixC];
                updateHandler(startTime, (ushort)left, (ushort)right);
            }
            return startTime;
        }

        #endregion Public


        #region Private

        private void UpdateVolumeTables()
        {
            var volume = (ulong)0xFFFF * (ulong)m_volume / 100UL;
            var ampTable = GetAmpTable(AmpType);
            var panTable = GetPanTable(PanType);
            m_volumes = new uint[6][];
            for (var j = 0; j < m_volumes.Length; j++)
            {
                m_volumes[j] = new uint[32];
                for (var i = 0; i < m_volumes[j].Length; i++)
                {
                    m_volumes[j][i] = (uint)((volume * ampTable[i] * panTable[j]) / (65535 * 100 * 3));
                }
            }
        }

        private uint[] GetAmpTable(AmpType ampType)
        {
            if (m_ampTables.ContainsKey(ampType))
            {
                return m_ampTables[ampType];
            }
            Logger.Error("Unknown PsgType: {0}", ampType);
            return new uint[32];
        }

        private uint[] GetPanTable(PanType panType)
        {
            if (m_panTables.ContainsKey(panType))
            {
                return m_panTables[panType];
            }
            Logger.Error("Unknown PanType: {0}", panType);
            return new uint[6];
        }

        #endregion Private
    }
}
