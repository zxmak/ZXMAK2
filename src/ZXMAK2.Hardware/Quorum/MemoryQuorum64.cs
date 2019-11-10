using System;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Hardware.Spectrum;

namespace ZXMAK2.Hardware.Quorum
{
    public class MemoryQuorum64 : MemoryQuorum
    {
        public MemoryQuorum64()
            : base("Quorum64", 1, 8) // use 8 RAM pages to satisfy strange UlaDeviceBase requirements
        {
            Name = "QUORUM 64K";
        }

        #region MemoryBase

        public override bool IsMap48 { get { return true; } }
        public override bool IsRom48 { get { return true; } }
        public override byte CMR0
        {
            get { return 0x30; }
            set { UpdateMapping(); }
        }
        public override int GetRomIndex(RomId romId) { return 0; }

        protected override void UpdateMapping()
        {
            // D1 of port #00 selects upper screen the way, similar to that of ZX Spectrum 128
            var videoPage = (CMR1 & 0x02) == 0 ? 5 : 7;

            // Turn on Shadow RAM on TR-DOS requests or when explicitly turned on by OUT 0,1
            var isNoRom = DOSEN || (CMR1 & Q_F_RAM) != 0;

            m_ula.SetPageMapping(videoPage, 0, 5, 2, 7);

            MapRead0000 = isNoRom ? RamPages[0] : RomPages[0];
            MapRead4000 = RamPages[5];
            MapRead8000 = RamPages[2];
            MapReadC000 = RamPages[7];

            MapWrite0000 = RamPages[0];
            MapWrite4000 = MapRead4000;
            MapWrite8000 = MapRead8000;
            MapWriteC000 = MapReadC000;
        }
        #endregion
    }
}
