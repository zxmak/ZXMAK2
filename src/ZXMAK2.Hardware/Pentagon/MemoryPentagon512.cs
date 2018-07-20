using System;
using System.Xml;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Hardware.Pentagon
{
    public class MemoryPentagon512 : MemoryBase
    {
        #region Fields

        private bool m_enableShadow;
        private bool m_lock = false;

        #endregion Fields


        public MemoryPentagon512()
            : base("Pentagon", 4, 32)
        {
            Name = "Pentagon 512K";
            EnableShadow = true;
        }


        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            bmgr.Events.SubscribeWrIo(0x8002, 0x0000, writePort7FFD);

            bmgr.Events.SubscribeRdMemM1(0xFF00, 0x3D00, BusReadMem3D00_M1);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0x4000, BusReadMemRamM1);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0x8000, BusReadMemRamM1);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0xC000, BusReadMemRamM1);
            bmgr.Events.SubscribeNmiAck(BusNmiAck);
            bmgr.Events.SubscribeReset(BusReset);

            // Subscribe before MemoryBase.BusInit 
            // to handle memory switches before read
            base.BusInit(bmgr);
        }

        protected override void OnConfigLoad(XmlNode itemNode)
        {
            base.OnConfigLoad(itemNode);
            EnableShadow = Utils.GetXmlAttributeAsBool(itemNode, "enableShadow", EnableShadow);
        }

        protected override void OnConfigSave(XmlNode itemNode)
        {
            base.OnConfigSave(itemNode);
            Utils.SetXmlAttribute(itemNode, "enableShadow", EnableShadow);
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
            if (SYSEN)
                romPage = GetRomIndex(RomId.ROM_SYS);

            int sega = (CMR0 & 0xC0) >> 6; // PENT512: D7,D6,D2,D1,D0

            ramPage |= sega << 3;

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

        protected virtual void BusReadMem3D00_M1(ushort addr, ref byte value)
        {
            if (!DOSEN && IsRom48)
            {
                DOSEN = true;
            }
        }

        protected virtual void BusReadMemRamM1(ushort addr, ref byte value)
        {
            if (SYSEN)
            {
                SYSEN = false;
            }
            if (DOSEN)
            {
                DOSEN = false;
            }
        }

        #endregion

        #region Bus Handlers

        private void writePort7FFD(ushort addr, byte value, ref bool handled)
        {
            if (!m_lock)
                CMR0 = value;
        }

        private void BusNmiAck()
        {
            // enable shadow rom
            SYSEN = EnableShadow;//true;
        }

        private void BusReset()
        {
            SYSEN = EnableShadow;//true;
            CMR0 = 0;
        }

        #endregion Bus Handlers


        public bool EnableShadow 
        {
            get { return m_enableShadow; }
            set { m_enableShadow = value; OnConfigChanged(); }
        }
    }
}
