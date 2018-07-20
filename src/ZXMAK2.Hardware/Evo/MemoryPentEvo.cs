using System;
using ZXMAK2.Hardware.Atm;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Attributes;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Engine.Cpu;


namespace ZXMAK2.Hardware.Evo
{
    public class MemoryPentEvo : MemoryBase
    {
        #region Fields

        private readonly int[] m_ru2 = new int[8]; // ATM 7.10 / ATM3(4Mb) memory map

        protected CpuUnit m_cpu;
        protected byte[][] m_romPages;
        private UlaAtm450 m_ulaAtm;
        private bool m_lock;

        private int m_aFF77;
        private int m_pFF77;

        private byte m_pXXBF;   // port EVO EVO
        private byte m_pEFF7;   // port EVO MOD

        private int m_romMask;
        private int m_ramMask;
        
        #endregion Fields


        public MemoryPentEvo(
            string romSetName,
            int romPageCount,
            int ramPageCount)
            : base(romSetName, romPageCount, ramPageCount)
        {
            Name = "PentEvo";
            Description = "PentEvo 4096K Memory Manager";
        }

        public MemoryPentEvo()
            : this("PentEvo", 32, 256)
        {
        }


        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            m_cpu = bmgr.CPU;
            m_ulaAtm = bmgr.FindDevice<UlaAtm450>();

            OnSubscribeIo(bmgr);

            bmgr.Events.SubscribeRdMemM1(0x0000, 0x0000, BusReadM1);
            bmgr.Events.SubscribeWrMem(0x0000, 0x0000, WriteMemXXXX);
            bmgr.Events.SubscribeReset(BusReset);

            // Subscribe before MemoryBase.BusInit 
            // to handle memory switches before read
            base.BusInit(bmgr);
        }

        protected virtual void OnSubscribeIo(IBusManager bmgr)
        {
            bmgr.Events.SubscribeWrIo(0x00FF, 0x00FF & 0x00FF, BusWritePortXXFF_PAL);	// atm_writepal(val);

            bmgr.Events.SubscribeWrIo(0x00FF, 0xFF77 & 0x00FF, BusWritePortXX77_SYS);
            bmgr.Events.SubscribeWrIo(0x37FF, 0x3FF7 & 0x37FF, BusWritePortXFF7_WND);	//ATM3 mask=0x3FFF
            bmgr.Events.SubscribeWrIo(0x8002, 0x7FFD & 0x8002, BusWritePort7FFD_128);
            bmgr.Events.SubscribeWrIo(0xFFFF, 0xEFF7 & 0xFFFF, BusWritePortEFF7_MOD);

            bmgr.Events.SubscribeWrIo(0x00FF, 0x00BF & 0x00FF, BusWritePortXXBF_EVO);
            bmgr.Events.SubscribeRdIo(0x00FF, 0x00BF & 0x00FF, BusReadPortXXBF_EVO);

            bmgr.Events.SubscribeRdIo(0xE0FF, 0x00BE & 0xE0FF, BusReadPortXXBE_CFG);
        }

        protected virtual void WriteMemXXXX(ushort addr, byte value)
        {
            if (m_ulaAtm != null && FNTWR)
            {
                m_ulaAtm.WriteSgen(addr, value);
            }
        }

        protected virtual void BusReadM1(ushort addr, ref byte value)
        {
            //LogAgent.Info(
            //    "{0:D3}-{1:D6}: #{2:X4} = #{3:X2}",
            //    m_cpu.Tact / m_ula.FrameTactCount,
            //    m_cpu.Tact % m_ula.FrameTactCount,
            //    addr,
            //    map[addr >> 14][addr & 0x3FFF]);
            var index = (addr >> 14) + ((CMR0 & 0x10) >> 2);
            var w = m_ru2[index] ^ 0x3FF;
            var isRam = (w & 0x40) == 0;
            if (isRam)
            {
                DOSEN = CPM;
            }
            else if (index != 0 && (addr & 0x3F00) == 0x3D00) //ROM2 & RAM & dosgate
            {
                DOSEN = true;
            }
        }

        #endregion

        #region MemoryBase

        public override bool IsMap48 { get { return false; } }

        public override int GetRomIndex(RomId romId)
        {
            switch (romId)
            {
                case RomId.ROM_SOS: return 0x1C;
                case RomId.ROM_DOS: return 0x1D;
                case RomId.ROM_128: return 0x1E;
                case RomId.ROM_SYS: return 0x1F;
            }
            throw new NotImplementedException();
        }

        protected override void UpdateMapping()
        {
            m_lock = (CMR0 & 0x20) != 0;
            if (PEN)
            {
                int videoPage = (CMR0 & 0x08) == 0 ? 5 : 7;
                if (m_ulaAtm != null)
                {
                    m_ulaAtm.SetPageMappingAtm(
                        VIDEO, 
                        videoPage, 
                        -1, 
                        -1, 
                        -1, 
                        -1);
                }
                else
                {
                    m_ula.SetPageMapping(
                        videoPage, -1, -1, -1, -1);
                }
                int romPage = RomPages.Length - 1;
                MapRead0000 = RomPages[romPage];
                MapRead4000 = RomPages[romPage];
                MapRead8000 = RomPages[romPage];
                MapReadC000 = RomPages[romPage];

                MapWrite0000 = m_trashPage;
                MapWrite4000 = m_trashPage;
                MapWrite8000 = m_trashPage;
                MapWriteC000 = m_trashPage;

                Map48[0] = -1;
                Map48[1] = -1;
                Map48[2] = -1;
                Map48[3] = -1;
            }
            else
            {
                int videoPage = (CMR0 & 0x08) == 0 ? 5 : 7;

                var index = (CMR0 & 0x10) >> 2;

                // high 2 bits of ram page stored 
                // in the high byte of m_ru2[wnd]
                var w0 = m_ru2[index + 0] ^ 0x3FF;
                var w1 = m_ru2[index + 1] ^ 0x3FF;
                var w2 = m_ru2[index + 2] ^ 0x3FF;
                var w3 = m_ru2[index + 3] ^ 0x3FF;
                var isRam0 = (w0 & 0x40) == 0;
                var isRam1 = (w1 & 0x40) == 0;
                var isRam2 = (w2 & 0x40) == 0;
                var isRam3 = (w3 & 0x40) == 0;
                var isDos0 = (w0 & 0x80) == 0;
                var isDos1 = (w1 & 0x80) == 0;
                var isDos2 = (w2 & 0x80) == 0;
                var isDos3 = (w3 & 0x80) == 0;
                var kpa0 = CMR0 & 7;
                var kpa0mask = 0x38;
                if (!ZX128)
                {
                    var sega = ((CMR0 & 0xC0) >> 6) | ((CMR0 & 0x20) >> 3);	//PENT1024: D5,D7,D6,D2,D1,D0
                    kpa0 |= sega << 3;
                    //kpa0 = (byte)((CMR0 & 0xE0) >> 2 | (CMR0 & 0x7));
                    kpa0mask = 0xC0;
                }
                var kpa8 = DOSEN ? 1 : 0;
                var romPage0 = (isDos0 ? kpa8 | (w0 & 0x3E) : w0 & 0x3F) & m_romMask;
                var romPage1 = (isDos1 ? kpa8 | (w1 & 0x3E) : w1 & 0x3F) & m_romMask;
                var romPage2 = (isDos2 ? kpa8 | (w2 & 0x3E) : w2 & 0x3F) & m_romMask;
                var romPage3 = (isDos3 ? kpa8 | (w3 & 0x3E) : w3 & 0x3F) & m_romMask;
                var ramPage0 = ((isDos0 ? (w0 & kpa0mask) | kpa0 : (w0 & 0x3F)) | ((w0 >> 2) & 0xC0)) & m_ramMask;
                var ramPage1 = ((isDos1 ? (w1 & kpa0mask) | kpa0 : (w1 & 0x3F)) | ((w1 >> 2) & 0xC0)) & m_ramMask;
                var ramPage2 = ((isDos2 ? (w2 & kpa0mask) | kpa0 : (w2 & 0x3F)) | ((w2 >> 2) & 0xC0)) & m_ramMask;
                var ramPage3 = ((isDos3 ? (w3 & kpa0mask) | kpa0 : (w3 & 0x3F)) | ((w3 >> 2) & 0xC0)) & m_ramMask;

                if (W0RAM0)
                {
                    isRam0 = true;
                    ramPage0 = 0;
                }

                if (m_ulaAtm != null)
                {
                    m_ulaAtm.SetPageMappingAtm(
                        VIDEO,
                        videoPage,
                        isRam0 ? ramPage0 : -1,
                        isRam1 ? ramPage1 : -1,
                        isRam2 ? ramPage2 : -1,
                        isRam3 ? ramPage3 : -1);
                }
                else
                {
                    m_ula.SetPageMapping(
                        videoPage,
                        isRam0 ? ramPage0 : -1,
                        isRam1 ? ramPage1 : -1,
                        isRam2 ? ramPage2 : -1,
                        isRam3 ? ramPage3 : -1);
                }

                MapRead0000 = isRam0 ? RamPages[ramPage0] : RomPages[romPage0];
                MapRead4000 = isRam1 ? RamPages[ramPage1] : RomPages[romPage1];
                MapRead8000 = isRam2 ? RamPages[ramPage2] : RomPages[romPage2];
                MapReadC000 = isRam3 ? RamPages[ramPage3] : RomPages[romPage3];

                MapWrite0000 = isRam0 ? MapRead0000 : m_trashPage;
                MapWrite4000 = isRam1 ? MapRead4000 : m_trashPage;
                MapWrite8000 = isRam2 ? MapRead8000 : m_trashPage;
                MapWriteC000 = isRam3 ? MapReadC000 : m_trashPage;

                Map48[0] = isRam0 ? -1 : romPage0;
                Map48[1] = isRam1 ? ramPage1 : -1;
                Map48[2] = isRam2 ? ramPage2 : -1;
                Map48[3] = isRam3 ? ramPage3 : -1;
            }
        }

        protected override void Init(int romPageCount, int ramPageCount)
        {
            base.Init(romPageCount, ramPageCount);
            m_romMask = RomPages.Length - 1;
            if (m_romMask > 0x1F)
            {
                m_romMask = 0x1F;
            }
            m_ramMask = RamPages.Length - 1;
            if (m_ramMask > 0xFF)
            {
                m_ramMask = 0xFF;
            }
        }

        #endregion

        #region Hardware Values

        [HardwareValue("PEN", Description = "Disable memory manager")]
        public bool PEN
        {
            get { return (m_aFF77 & 0x100) == 0; }
            set { m_aFF77 = (m_aFF77 & ~0x100) | (value ? 0x0000 : 0x0100); UpdateMapping(); }
        }

        [HardwareValue("CPM", Description = "Enable continous access for extended ports and TRDOS ROM")]
        public bool CPM
        {
            get { return (m_aFF77 & 0x200) == 0; }
            set { m_aFF77 = (m_aFF77 & ~0x200) | (value ? 0x0000 : 0x0200); if (value) DOSEN = true; UpdateMapping(); }
        }

        [HardwareValue("PEN2", Description = "Enable palette change through port #FF")]
        public bool PEN2
        {
            get { return (m_aFF77 & 0x4000) == 0; }
            set { m_aFF77 = (m_aFF77 & ~0x4000) | (value ? 0x0000 : 0x4000); UpdateMapping(); }
        }

        [HardwareValue("RG", Description = "Low bits of video mode")]
        public int RG
        {
            get { return m_pFF77 & 7; }
            set { m_pFF77 = (m_pFF77 & 0xF8) | (value & 7); UpdateMapping(); }
        }

        [HardwareValue("Z_I", Description = "Enable HSYNC interrupts")]
        public bool Z_I
        {
            get { return (m_pFF77 & 0x20) == 0; }
            set { m_pFF77 = (m_pFF77 & ~0x20) | (value ? 0x20 : 0x00); UpdateMapping(); }
        }

        [HardwareValue("SHADOW", Description = "Enable shadow ports")]
        public bool SHADOW
        {
            get { return (m_pXXBF & 1) != 0; }
            set { m_pXXBF = (byte)((m_pXXBF & ~1) | (value ? 1 : 0)); }
        }

        [HardwareValue("FNTWR", Description = "Allow write to font RAM")]
        public bool FNTWR
        {
            get { return (m_pXXBF & 4) != 0; }
            set { m_pXXBF = (byte)((m_pXXBF & ~4) | (value ? 4 : 0)); }
        }

        [HardwareValue("CMOSEN", Description = "Enable CMOS ports shadow independent")]
        public bool CMOSEN
        {
            get { return (m_pEFF7 & 0x80) != 0; }
            set { m_pEFF7 = (byte)((m_pEFF7 & 0x7F) | (value ? 0x80 : 0)); }
        }

        [HardwareValue("W0RAM0", Description = "Map RAM#00 to window #0000..#3FFF (max priority)")]
        public bool W0RAM0
        {
            get { return (m_pEFF7 & 0x08) != 0; }
            set { m_pEFF7 = (byte)((m_pEFF7 & 0xF7) | (value ? 0x08 : 0)); }
        }

        [HardwareValue("ZX128", Description = "Enable ZX Spectrum 128 compatible #7FFD port (otherwise Pentagon 1024)")]
        public bool ZX128
        {
            get { return (m_pEFF7 & 0x04) != 0; }
            set { m_pEFF7 = (byte)((m_pEFF7 & 0xFB) | (value ? 0x04 : 0)); }
        }

        [HardwareValue("TURBO", Description = "Enable turbo mode")]
        public bool TURBO
        {
            get { return (m_pEFF7 & 0x10) != 0; }
            set { m_pEFF7 = (byte)((m_pEFF7 & 0xEF) | (value ? 0x10 : 0)); }
        }

        [HardwareValue("RGEX", Description = "High bits of video mode (when RG=3)")]
        public int RGEX
        {
            get { return (m_pEFF7 & 0x01) | ((m_pEFF7 & 0x20) >> 4); }
            set { m_pEFF7 = (byte)((m_pEFF7 & 0xDE) | (value & 0x01) | ((value & 2) << 4)); }
        }

        [HardwareValue("VIDEO", Description = "Video Mode")]
        public AtmVideoMode VIDEO
        {
            get { return (AtmVideoMode)(RG | (RGEX << 3)); }
            set { RG = (int)value & 7; RGEX = (int)value >> 3; }
        }

        [HardwareReadOnly(true)]
        [HardwareValue("RU2", Description = "RU2 RAM memory manager content")]
        public int[] RU2
        {
            get { return m_ru2; }
        }

        [HardwareValue("DOSEN", Description = "TRDOS flag")]
        public override bool DOSEN
        {
            get { return base.DOSEN; }
            set { base.DOSEN = value | CPM; }
        }

        public override bool SYSEN
        {
            get { return SHADOW; }
            set { SHADOW = value; }
        }

        #endregion

        #region Bus Handlers

        protected virtual void BusWritePortXXFF_PAL(ushort addr, byte value, ref bool handled)
        {
            if ((DOSEN || SHADOW) && PEN2 && m_ulaAtm != null)
            {
                m_ulaAtm.SetPaletteAtm2(value);
            }
        }

        protected virtual void BusWritePortXX77_SYS(ushort addr, byte value, ref bool handled) // ATM2
        {
            if (DOSEN || SHADOW)
            {
                m_pFF77 = value;
                m_aFF77 = addr;
                if (CPM) DOSEN = true;
                UpdateMapping();
                //cpu.int_gate = (comp.pFF77 & 0x20) != false;
                //set_banks();
            }
        }

        protected virtual void BusWritePortXFF7_WND(ushort addr, byte value, ref bool handled) // ATM2
        {
            if (DOSEN || SHADOW)
            {
                var wnd = ((CMR0 & 0x10) >> 2) | ((addr >> 14) & 3);
                if ((addr & 0x0800) != 0)
                {
                    m_ru2[wnd] = value | 0x0300;
                }
                else
                {
                    // store high 2 bits in the high byte of m_ru2[wnd]
                    m_ru2[wnd] &= 0x80; // save dos
                    m_ru2[wnd] |= 0x40; // always ram
                    m_ru2[wnd] |= (value & 0x3F) | ((value << 2) & 0x300);
                }
                UpdateMapping();
            }
        }

        protected virtual void BusWritePort7FFD_128(ushort addr, byte value, ref bool handled)
        {
            if (m_lock)
            {
                return;
            }
            CMR0 = value;
        }

        protected virtual void BusWritePortXXBF_EVO(ushort addr, byte value, ref bool handled)
        {
            m_pXXBF = value;
            UpdateMapping();
        }

        protected virtual void BusReadPortXXBF_EVO(ushort addr, ref byte value, ref bool handled)
        {
            value = (byte)((value & 0xF0) | (m_pXXBF & 0x0F));
        }

        protected virtual void BusWritePortEFF7_MOD(ushort addr, byte value, ref bool handled)
        {
            m_pEFF7 = value;
            UpdateMapping();
        }

        protected virtual void BusReadPortXXBE_CFG(ushort addr, ref byte value, ref bool handled)
        {
            switch ((addr >> 8) & 0x1F)
            {
                case 0x00:
                case 0x01:
                case 0x02:
                case 0x03:
                case 0x04:
                case 0x05:
                case 0x06:
                case 0x07:
                    value = (byte)(GetCfgPage((addr & 7) >> 8) ^ 0xFF);
                    break;
                case 0x08:
                    value = (byte)GetCfgIsRam();
                    break;
                case 0x09:
                    value = (byte)GetCfgIsDos();
                    break;
                case 0x0A:
                    value = CMR0;
                    break;
                case 0x0B:
                    value = m_pEFF7;
                    break;
                case 0x0C:
                    value = (byte)((m_pFF77 & 0x0F) |
                        ((m_aFF77 & 0x4000) >> 7) |
                        ((m_aFF77 & 0x0200) >> 3) |
                        ((m_aFF77 & 0x0100) >> 3) |
                        (DOSEN ? 0x10 : 0));
                    break;
            }
        }

        private int GetCfgIsDos()
        {
            var value = 0;
            var mask = 0x01;
            for (var index = 0; index < 8; index++)
            {
                // high 2 bits of ram page stored 
                // in the high byte of m_ru2[wnd]
                var w = m_ru2[index] ^ 0x3FF;
                var isDos = (w & 0x80) == 0;
                if (isDos) value |= mask;
                mask <<= 1;
            }
            return value;
        }

        private int GetCfgIsRam()
        {
            var value = 0;
            var mask = 0x01;
            for (var index = 0; index < 8; index++)
            {
                // high 2 bits of ram page stored 
                // in the high byte of m_ru2[wnd]
                var w = m_ru2[index] ^ 0x3FF;
                var isRam = (w & 0x40) == 0;
                if (isRam) value |= mask;
                mask <<= 1;
            }
            return value;
        }

        private int GetCfgPage(int index)
        {
            var romMask = RomPages.Length - 1;
            if (romMask > 0x1F)
            {
                romMask = 0x1F;
            }
            var ramMask = RamPages.Length - 1;
            if (ramMask > 0xFF)
            {
                ramMask = 0xFF;
            }

            // high 2 bits of ram page stored 
            // in the high byte of m_ru2[wnd]
            var w = m_ru2[index] ^ 0x3FF;
            var kpa0 = CMR0 & 7;
            var kpa8 = DOSEN ? 1 : 0;
            var isDos = (w & 0x80) == 0;
            var isRam = (w & 0x40) == 0;
            var romPage = (isDos ? kpa8 | (w & 0x3E) : w & 0x3F) & romMask;
            var ramPage = ((isDos ? (w & 0x38) | kpa0 : (w & 0x3F)) | ((w >> 2) & 0xC0)) & ramMask;
            if ((index & 3) == 0 && W0RAM0)
            {
                isRam = true;
                ramPage = 0;
            }
            return isRam ? ramPage : romPage;
        }

        protected virtual void BusReset()
        {
            m_aFF77 = 0x4000;   // RESET: A14=1, A9=0, A8=0
            m_pFF77 = 3;        // RESET: D3=0, D2..D0=011
            m_pXXBF = 0;        // RESET=0
            m_pEFF7 = 0;        // RESET=0
            DOSEN = CPM;

            CMR0 = 0;
            CMR1 = 0;
            UpdateMapping();
        }

        public override void ResetState()
        {
            base.ResetState();
            BusReset();

            // Init 128K
            m_aFF77 = 0xFF77;
            m_pFF77 = 0x00AB;
            Array.Copy(
                new int[] { 0x381, 0x37a, 0x37d, 0x3ff, 0x383, 0x37a, 0x37d, 0x3ff },
                m_ru2,
                m_ru2.Length);
            DOSEN = CPM;
        }

        #endregion
    }
}
