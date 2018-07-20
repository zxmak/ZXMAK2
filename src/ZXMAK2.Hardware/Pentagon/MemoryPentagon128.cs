using System;

using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Hardware.Pentagon
{
    public class MemoryPentagon128 : MemoryBase
    {
        public MemoryPentagon128()
            : base("Pentagon", 4, 8)
        {
            Name = "Pentagon 128K";
        }

        
        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            bmgr.Events.SubscribeWrIo(0x8002, 0x0000, writePort7FFD);
            bmgr.Events.SubscribeReset(busReset);

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


        private bool m_lock = false;

        protected override void OnPowerOn()
        {
            base.OnPowerOn();
            for (var i = 0; i < RamPages.Length; i++)
            {
                // Pentagon - A3, A6, A7, false
                // Delta-C - A0, A6, A8, true
                FillRamPowerOn(RamPages[i], 3, 6, 7, false);
            }
        }

        protected static void FillRamPowerOn(
            byte[] ramPage,
            int b0,
            int b1,
            int b2,
            bool inverse)
        {
            for (var j = 0; j < 0x4000; j++)
            {
                var value = (((j >> b0) ^ (j >> b1) ^ (j >> b2)) & 1) == 0 ?
                    0x00 : 0xFF;
                if (inverse)
                {
                    value ^= 0xFF;
                }
                ramPage[j] = (byte)value;
            }
            var random = new Random();
            for (var j = 0; j < 256; j++)
            {
                if (random.Next(100) > 97)
                {
                    var rnd = (byte)random.Next(256);
                    for (var k = j; k < 0x4000; k += 256)
                    {
                        if (random.Next(100) > 25)
                        {
                            ramPage[k] = rnd;
                        }
                    }
                }
            }
        }

        #region Bus Handlers

        private void writePort7FFD(ushort addr, byte value, ref bool handled)
        {
            if (!m_lock)
                CMR0 = value;
        }

        private void busReset()
        {
            CMR0 = 0;
        }

        #endregion
    }
}
