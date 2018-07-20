using System;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Hardware.Spectrum
{
    public class MemoryPlus3 : MemoryBase
    {
        #region Fields

        private bool m_lock = false;

        #endregion Fields


        public MemoryPlus3()
            : base("PLUS3", 4, 8)
        {
            Name = "ZX Spectrum +3";
            Description = "Spectrum +3 MMU alpha version";
        }

        
        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            bmgr.Events.SubscribeWrIo(0xC002, 0x4000, BusWritePort7FFD);
            bmgr.Events.SubscribeWrIo(0xF002, 0x1000, BusWritePort1FFD);
            bmgr.Events.SubscribeReset(BusReset);

            // Subscribe before MemoryBase.BusInit 
            // to handle memory switches before read
            base.BusInit(bmgr);
        }

        #endregion

        #region MemoryBase

        public override int GetRomIndex(RomId romId)
        {
            switch (romId)
            {
                case RomId.ROM_128: return 0;		// +3 Editor
                case RomId.ROM_SYS: return 1;		// +3 Syntax
                case RomId.ROM_DOS: return 2;		// +3 DOS
                case RomId.ROM_SOS: return 3;		// +3 SOS
            }
            Logger.Error("Unknown RomName: {0}", romId);
            throw new InvalidOperationException("Unknown RomName");
        }

        public override bool DOSEN
        {
            get { return (CMR1 & 1) != 0; }
            set
            {
                if ((CMR1 & 1) != (value ? 1 : 0))
                {
                    CMR1 = (byte)((CMR1 & 0xFE) | (value ? 1 : 0));
                    UpdateMapping();
                }
            }
        }

        public override bool IsMap48 { get { return m_lock && !DOSEN; } }

        protected override void UpdateMapping()
        {
            m_lock = (CMR0 & 0x20) != 0;
            if (DOSEN && !m_lock)
            {
                // +3 DOS
                switch ((CMR1 >> 1) & 3)
                {
                    case 0:
                        MapRead0000 = MapWrite0000 = RamPages[Map48[0] = 0];
                        MapRead4000 = MapWrite4000 = RamPages[Map48[1] = 1];
                        MapRead8000 = MapWrite8000 = RamPages[Map48[2] = 2];
                        MapReadC000 = MapWriteC000 = RamPages[Map48[3] = 3];
                        break;
                    case 1:
                        MapRead0000 = MapWrite0000 = RamPages[Map48[0] = 4];
                        MapRead4000 = MapWrite4000 = RamPages[Map48[1] = 5];
                        MapRead8000 = MapWrite8000 = RamPages[Map48[2] = 6];
                        MapReadC000 = MapWriteC000 = RamPages[Map48[3] = 7];
                        break;
                    case 2:
                        MapRead0000 = MapWrite0000 = RamPages[Map48[0] = 4];
                        MapRead4000 = MapWrite4000 = RamPages[Map48[1] = 5];
                        MapRead8000 = MapWrite8000 = RamPages[Map48[2] = 6];
                        MapReadC000 = MapWriteC000 = RamPages[Map48[3] = 3];
                        break;
                    case 3:
                        MapRead0000 = MapWrite0000 = RamPages[Map48[0] = 4];
                        MapRead4000 = MapWrite4000 = RamPages[Map48[1] = 7];
                        MapRead8000 = MapWrite8000 = RamPages[Map48[2] = 6];
                        MapReadC000 = MapWriteC000 = RamPages[Map48[3] = 3];
                        break;
                }
                int videoPageDos = (CMR0 & 0x08) == 0 ? 5 : 7;
                m_ula.SetPageMapping(videoPageDos, -1, 5, 2, Map48[3]);
            }
            else
            {
                // +3 SOS (48K compatible)
                int ramPage = CMR0 & 7;
                int romPage = ((CMR0 & 0x10) >> 4) | ((CMR1 & 0x04) >> 1);
                MapRead0000 = RomPages[Map48[0] = romPage];
                MapRead4000 = RamPages[Map48[1] = 5];
                MapRead8000 = RamPages[Map48[2] = 2];
                MapReadC000 = RamPages[Map48[3] = ramPage];

                MapWrite0000 = m_trashPage;
                MapWrite4000 = MapRead4000;
                MapWrite8000 = MapRead8000;
                MapWriteC000 = MapReadC000;

                int videoPage = (CMR0 & 0x08) == 0 ? 5 : 7;
                m_ula.SetPageMapping(videoPage, -1, 5, 2, ramPage);
            }
        }

        #endregion

        #region Bus Handlers

        protected virtual void BusWritePort7FFD(ushort addr, byte value, ref bool handled)
        {
            if (!m_lock)
                CMR0 = value;
        }

        protected virtual void BusWritePort1FFD(ushort addr, byte value, ref bool handled)
        {
            CMR1 = value;
        }

        protected virtual void BusReset()
        {
            CMR0 = 0;
            CMR1 = 0;
        }

        #endregion
    }
}
