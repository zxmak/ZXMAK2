using System;
using ZXMAK2.Engine.Cpu;
using ZXMAK2.Engine.Attributes;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Hardware.Atm
{
    public class MemoryAtm450 : MemoryBase
    {
        #region Fields

        private UlaAtm450 m_ulaAtm;
        protected CpuUnit m_cpu;
        
        private bool m_lock = false;
        private byte m_aFE;
        private byte m_aFB;
        
        #endregion Fields


        public MemoryAtm450()
            : base("ATM450", 8, 32)
        {
            Name = "ATM450 512K";
            Description = "ATM450 512K Memory Manager";
        }


        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            m_cpu = bmgr.CPU;
            m_ulaAtm = bmgr.FindDevice<UlaAtm450>();

            bmgr.Events.SubscribeRdIo(0x0001, 0x0000, BusReadPortFE);      // bit Z emulation
            bmgr.Events.SubscribeWrIo(0x0001, 0x0000, BusWritePortFE);
            bmgr.Events.SubscribeRdIo(0x0004, 0x00FB & 0x0004, BusReadPortFB);   // CPSYS [(addr & 0x7F)==0x7B]
            bmgr.Events.SubscribeWrIo(0x8202, 0x7FFD & 0x8202, BusWritePort7FFD);
            bmgr.Events.SubscribeWrIo(0x8202, 0xFDFD & 0x8202, BusWritePortFDFD);
            bmgr.Events.SubscribeWrIo(0x8202, 0x7DFD & 0x8202, BusWritePort7DFD); // atm_writepal(val);
            bmgr.Events.SubscribeRdMemM1(0xFF00, 0x3D00, BusReadMem3D00_M1);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0x4000, BusReadMemRamM1);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0x8000, BusReadMemRamM1);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0xC000, BusReadMemRamM1);

            //bmgr.Events.SubscribeRdIo(0x0005, 0x00FE & 0x0005, BusReadPortFE);      // bit Z emulation
            //bmgr.Events.SubscribeWrIo(0x0005, 0x00FE & 0x0005, BusWritePortFE);
            //bmgr.Events.SubscribeRdIo(0x0005, 0x00FB & 0x0005, BusReadPortFB);   // CPSYS [(addr & 0x7F)==0x7B]
            //bmgr.Events.SubscribeWrIo(0x8202, 0x7FFD & 0x8202, BusWritePort7FFD);
            //bmgr.Events.SubscribeWrIo(0x8202, 0xFDFD & 0x8202, BusWritePortFDFD);
            //bmgr.Events.SubscribeWrIo(0x8202, 0x7DFD & 0x8202, BusWritePort7DFD); // atm_writepal(val);
            //bmgr.Events.SubscribeRdMemM1(0xFF00, 0x3D00, BusReadMem3D00_M1);
            //bmgr.Events.SubscribeRdMemM1(0xC000, 0x4000, BusReadMemRamM1);
            //bmgr.Events.SubscribeRdMemM1(0xC000, 0x8000, BusReadMemRamM1);
            //bmgr.Events.SubscribeRdMemM1(0xC000, 0xC000, BusReadMemRamM1);

            bmgr.Events.SubscribeReset(BusReset);

            // Subscribe before MemoryBase.BusInit 
            // to handle memory switches before read
            base.BusInit(bmgr);
        }

        #endregion

        #region MemoryBase

        public override bool IsMap48 
        { 
            get { return false; } 
        }

        public override bool IsRom48
        {
            get 
            { 
                return MapRead0000 == RomPages[GetRomIndex(RomId.ROM_SOS)] ||
                    MapRead0000 == RomPages[GetRomIndex(RomId.ROM_SOS)+4]; 
            }
        }

        protected override void UpdateMapping()
        {
            m_lock = (CMR0 & 0x20) != 0;

            var norom = CPUS;                 // CPUS
            if (!norom)
            {
                if (m_lock)
                {
                    SetAfeAfb(AFE, (byte)(AFB & 0x7F), false);
                }
                if (DOSEN && CPNET)   // what is CPNET?
                {
                    // more priority, then 7FFD
                    SetAfeAfb(AFE, (byte)(AFB | 0x80), false);
                }
            }

            int romPage;
            if (CPSYS)
            {
                romPage = GetRomIndex(RomId.ROM_SYS);
            }
            else if (DOSEN)      // trdos or 48/128
            {
                romPage = GetRomIndex(RomId.ROM_DOS);
            }
            else
            {
                romPage = (CMR0 & 0x10) != 0 ?
                    GetRomIndex(RomId.ROM_SOS) :
                    GetRomIndex(RomId.ROM_128);
            }
            romPage |= CMR1 & 4;    // extended 64K rom (if exists)

            var videoPage = (CMR0 & 0x08) == 0 ? 5 : 7;

            var ramPage = CMR0 & 7;
            var sega = CMR1 & 3;   // & 7  for 1024K
            ramPage |= sega << 3;

            if (m_ulaAtm != null)
            {
                var videoMode = (AtmVideoMode)(((AFE >> 6) & 1) | ((AFE >> 4) & 2)); // (m_aFE >> 5) & 3
                m_ulaAtm.SetPageMappingAtm(
                    videoMode,
                    videoPage,
                    norom ? 0 : -1,
                    norom ? 4 : 5,
                    2,
                    ramPage);
            }
            else
            {
                m_ula.SetPageMapping(
                    videoPage,
                    norom ? 0 : -1,
                    norom ? 4 : 5,
                    2,
                    ramPage);
            }

            MapRead0000 = norom ? RamPages[0] : RomPages[romPage];
            MapRead4000 = norom ? RamPages[4] : RamPages[5];
            MapRead8000 = RamPages[2];
            MapReadC000 = RamPages[ramPage];

            MapWrite0000 = norom ? MapRead0000 : m_trashPage;
            MapWrite4000 = MapRead4000;
            MapWrite8000 = MapRead8000;
            MapWriteC000 = MapReadC000;
        }

        private void SetAfeAfb(byte afe, byte afb, bool updateMapping)
        {
            var changeAfe = afe ^ m_aFE;
            var changeAfb = afb ^ m_aFB;
            m_aFE = afe;
            m_aFB = afb;
            if ((changeAfe & 0x40) != 0)
            {
                MemSwap();
            }
            if (updateMapping && ((changeAfe & 0xE0) != 0 || changeAfb != 0))
            {
                UpdateMapping();
            }
        }

        [HardwareValue("AFE", Description="High address byte of the last port #FE output")]
        public byte AFE
        {
            get { return m_aFE; }
            set { SetAfeAfb(value, AFB, true); }
        }

        [HardwareValue("AFB", Description="High address byte of the last port #FB output")]
        public byte AFB
        {
            get { return m_aFB; }
            set { SetAfeAfb(AFE, value, true); }
        }

        [HardwareValue("CPUS", Description="Enable RAM cache")]
        public bool CPUS
        {
            get { return (AFE & 0x80) == 0; }
            set { AFE = (byte)((AFE & ~0x80) | (value ? 0x80:0)); }
        }

        [HardwareValue("CPSYS", Description="Select system ROM")]
        public bool CPSYS
        {
            get { return (AFB & 0x80) != 0; }
            set { AFB = (byte)((AFB & ~0x80) | (value ? 0x80:0)); }
        }

        [HardwareValue("CPNET", Description="")]
        public bool CPNET
        {
            get { return (CMR1 & 8) != 0; }
            set { CMR1 = (byte)((CMR1 & ~8) | (value ? 8:0)); }
        }

        [HardwareValue("SYSEN", Description="")]
        public override bool SYSEN
        {
            get { return (AFB & 0x80) != 0 && (AFE & 0x80) != 0; }
            set
            {
                if (value)
                {
                    SetAfeAfb((byte)(AFE | 0x80), (byte)(AFB | 0x80), true);
                }
                else
                {
                    SetAfeAfb((byte)(AFE | 0x80), (byte)(AFB & 0x7F), true);
                }
            }
        }

        [HardwareValue("MEMSWAP", Description = "Swap A5-A7 and A8-A10")]
        public bool MEMSWAP
        {
            get { return (AFE & 0x40) != 0; }
            set { AFE = (byte)((AFE & 0xBF) | (value ? 0x40 : 0)); }
        }

        public override int GetRomIndex(RomId romId)
        {
            switch (romId)
            {
                case RomId.ROM_128: return 2;
                case RomId.ROM_SOS: return 3;
                case RomId.ROM_DOS: return 1;
                case RomId.ROM_SYS: return 0;
            }
            Logger.Error("Unknown RomName: {0}", romId);
            throw new InvalidOperationException("Unknown RomName");
        }

        public override string GetRomName(int pageNo)
        {
            var name = base.GetRomName(pageNo & 3);
            return string.Format("{0}{1}", name, pageNo>>2);
        }

        #endregion

        
        #region Bus Handlers

        protected virtual void BusReadPortFE(ushort addr, ref byte value, ref bool handled)
        {
            value &= 0x7F;
            value |= Atm450_z((int)(m_cpu.Tact % m_ula.FrameTactCount));
        }

        protected virtual void BusReadPortFB(ushort addr, ref byte value, ref bool handled)
        {
            AFB = (byte)addr;
        }

        protected virtual void BusWritePortFE(ushort addr, byte value, ref bool handled)
        {
            AFE = (byte)addr;
        }

        protected virtual void BusWritePort7FFD(ushort addr, byte value, ref bool handled)
        {
            if (m_lock)
            {
                return;
            }
            CMR0 = value;
        }

        protected virtual void BusWritePortFDFD(ushort addr, byte value, ref bool handled)
        {
            CMR1 = value;
        }

        protected virtual void BusWritePort7DFD(ushort addr, byte value, ref bool handled)
        {
            if (m_ulaAtm != null)
            {
                m_ulaAtm.SetPaletteAtm(value);
            }
        }

        protected virtual void BusReadMem3D00_M1(ushort addr, ref byte value)
        {
            if (!DOSEN && IsRom48)
            {
                DOSEN = true;
            }
        }

        protected virtual void BusReadMemRamM1(ushort addr, ref byte value)
        {
            if (DOSEN)
            {
                DOSEN = false;
            }
        }

        protected virtual void BusReset()
        {
            DOSEN = false;
            CMR0 = 0;
            CMR1 = 0;

            //RM_DOS
            //m_aFB = 0;

            //DEFAULT
            //0x60 => set mode 3 (standard spectrum 256x192)
            //SetAfeAfb(0x80 | 0x60, 0x80, true);
            SetAfeAfb((byte)(AFE | 0x80), (byte)(AFB | 0x80), true);
        }

        #endregion


        #region Private

        private void MemSwap()
        {
            // ATM hi-res video modes swap RAM/CPU address bus A5-A7<=>A8-A10
            foreach (var page in RamPages)
            {
                var buffer = new byte[16384];
                for (var addr = 0; addr < buffer.Length; addr++)
                {
                    var eaddr = (addr & 0x381F) |
                        ((addr >> 3) & 0xE0) |
                        ((addr << 3) & 0x700);
                    buffer[addr] = page[eaddr];
                }
                for (var addr = 0; addr < buffer.Length; addr++)
                {
                    page[addr] = buffer[addr];
                }
            }
        }

        private byte Atm450_z(int t)
        {
            // PAL hardware gives 3 zeros in secret short time intervals
            if (m_ula.FrameTactCount < 80000)
            { 
                // NORMAL SPEED mode
                if ((uint)(t - 7200) < 40 || (uint)(t - 7284) < 40 || (uint)(t - 7326) < 40) return 0;
            }
            else
            { 
                // TURBO mode
                if ((uint)(t - 21514) < 40 || (uint)(t - 21703) < 80 || (uint)(t - 21808) < 40) return 0;
            }
            return 0x80;
        }

        #endregion
    }
}
