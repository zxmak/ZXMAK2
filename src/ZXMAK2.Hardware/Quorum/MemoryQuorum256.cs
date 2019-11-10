using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Engine.Interfaces;

namespace ZXMAK2.Hardware.Quorum
{
    public class MemoryQuorum256 : MemoryQuorum
    {
        #region Fields
        private bool m_lock;
        #endregion

        public MemoryQuorum256()
            : base("Quorum128", 4, 16) // Quorum 256 does not use a separate ROM set
        {
            Name = "QUORUM 256K";
        }

        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            bmgr.Events.SubscribeWrIo(0x801A, 0x7FFD & 0x801A, BusWritePort7FFD);

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
            get { return (CMR0 & 0x10) != 0; }
        }

        protected override void UpdateMapping()
        {
            var ramPage = CMR0 & 7;
            ramPage |= ((CMR0 & 0xC0) >> 3);
            ramPage &= 0x0F;     //256K

            var romPage = (CMR0 & 0x10) != 0 ?
                GetRomIndex(RomId.ROM_SOS) :
                GetRomIndex(RomId.ROM_128);
            var videoPage = (CMR0 & 0x08) == 0 ? 5 : 7;

            var ramPage0000 = ((CMR1 & Q_RAM_8) != 0) ? 8 : 0;
            var isBlkWr = (CMR1 & Q_BLK_WR) != 0;
            var isNoRom = (CMR1 & Q_F_RAM) != 0;
            var isDosRom = (CMR1 & Q_TR_DOS) != 0;

            m_lock = isBlkWr;

            if (SYSEN && !isDosRom)
            {
                romPage = GetRomIndex(RomId.ROM_SYS);
            }
            if (DOSEN)      // trdos or 48/128
            {
                romPage = GetRomIndex(RomId.ROM_DOS);
                isNoRom = !isDosRom;
            }

            m_ula.SetPageMapping(
                videoPage,
                !isBlkWr ? ramPage0000 : -1,
                5,
                2,
                ramPage);

            MapRead0000 = isNoRom ? RamPages[ramPage0000] : RomPages[romPage];
            MapRead4000 = RamPages[5];
            MapRead8000 = RamPages[2];
            MapReadC000 = RamPages[ramPage];

            MapWrite0000 = isBlkWr ? m_trashPage : RamPages[ramPage0000];
            MapWrite4000 = MapRead4000;
            MapWrite8000 = MapRead8000;
            MapWriteC000 = MapReadC000;
        }

        public override bool SYSEN
        {
            get { return (CMR1 & Q_B_ROM) == 0; }
            set
            {
                if (value)
                    CMR1 |= Q_B_ROM;
                else
                    CMR1 &= Q_B_ROM ^ 0xFF;
                UpdateMapping();
            }
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

        #endregion


        #region Private

        protected virtual void BusWritePort7FFD(ushort addr, byte value, ref bool handled)
        {
            //LogAgent.Info("PC: #{0:X4}  CMR0 <- #{1:X2} {2}", m_cpu.regs.PC, value, m_lock ? "[locked]" : string.Empty);
            if (!m_lock)
            {
                CMR0 = value;
            }
        }
        #endregion
    }
}
