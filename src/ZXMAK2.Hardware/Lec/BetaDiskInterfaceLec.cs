using ZXMAK2.Hardware.General;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Hardware.Lec
{
    public class BetaDiskInterfaceLec : BetaDiskInterface
    {
        #region Fields

        private bool m_betaHack = false;

        #endregion

        
        public BetaDiskInterfaceLec()
        {
            Name = "BDI LEC (beta)";
            Description = "Beta Disk Interface + LEC extension hack";
        }


        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            base.BusInit(bmgr);
            bmgr.Events.SubscribeWrIo(0x8002, 0x00FD & 0x8002, BusWriteBdiHack);
        }

        #endregion

        
        #region Private

        protected void BusWriteBdiHack(ushort addr, byte value, ref bool handled)
        {
            m_betaHack = (value & 0x10) != 0;
        }

        protected override void BusReadMem3D00_M1(ushort addr, ref byte value)
        {
            if (!m_betaHack && m_memory.IsRom48)
            {
                DOSEN = true;
            }
        }

        protected override void BusReadMemRam(ushort addr, ref byte value)
        {
            if (!m_betaHack && m_memory.DOSEN)
            {
                DOSEN = false;
            }
        }

        protected override void BusReset()
        {
            m_betaHack = false;
            base.BusReset();
        }

        #endregion
    }
}
