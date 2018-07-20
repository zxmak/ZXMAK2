using System;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;

namespace ZXMAK2.Hardware.Spectrum
{
    public class MemorySpectrum128 : MemoryBase
    {
        public MemorySpectrum128(string romSetName)
            : base(romSetName, 4, 8)
        {
            Name = "ZX Spectrum 128";    
        }

        public MemorySpectrum128()
            : this("ZX128")
        {
        }

        
        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            bmgr.Events.SubscribeWrIo(0x8002, 0x0000, writePort7FFD);
            bmgr.Events.SubscribeRdIo(0x8002, 0x0000, readPort7FFD);
            bmgr.Events.SubscribeReset(BusReset);

            // Subscribe before MemoryBase.BusInit 
            // to handle memory switches before read
            base.BusInit(bmgr);
        }

        #endregion

        #region MemoryBase

        public override bool IsMap48 { get { return m_lock; } }

        protected override void UpdateMapping()
        {
            m_lock = (CMR0 & 0x20) != 0;
            int ramPage = CMR0 & 7;
            int romPage = (CMR0 & 0x10) != 0 ?
                GetRomIndex(RomId.ROM_SOS) :
                GetRomIndex(RomId.ROM_128);
            int videoPage = (CMR0 & 0x08) == 0 ? 5 : 7;

            if (DOSEN)      // trdos or 48/128
                romPage = GetRomIndex(RomId.ROM_DOS);

            m_ula.SetPageMapping(videoPage, -1, 5, 2, ramPage);
            MapRead0000 = RomPages[romPage];
            MapRead4000 = RamPages[5];
            MapRead8000 = RamPages[2];
            MapReadC000 = RamPages[ramPage];

            MapWrite0000 = m_trashPage;
            MapWrite4000 = MapRead4000;
            MapWrite8000 = MapRead8000;
            MapWriteC000 = MapReadC000;

            Map48[0] = romPage;
            Map48[1] = 5;
            Map48[2] = 2;
            Map48[3] = ramPage;
        }

        public override int GetRomIndex(RomId romId)
        {
            switch (romId)
            {
                case RomId.ROM_128: return 0;
                case RomId.ROM_SOS: return 1;
                case RomId.ROM_DOS: return 2;
                case RomId.ROM_SYS: return 3;
            }
            Logger.Error("Unknown RomName: {0}", romId);
            throw new InvalidOperationException("Unknown RomName");
        }

        #endregion

        #region Bus Handlers

        private void writePort7FFD(ushort addr, byte value, ref bool handled)
        {
            if (!m_lock)
                CMR0 = value;
        }

        private void readPort7FFD(ushort addr, ref byte value, ref bool handled)
        {
            if (!m_lock)
                CMR0 = value;// (byte)((CMR0 & 0x20) | (value & ~0x20));
        }

        protected virtual void BusReset()
        {
            CMR0 = 0;
        }

        #endregion


        private bool m_lock = false;
    }
}
