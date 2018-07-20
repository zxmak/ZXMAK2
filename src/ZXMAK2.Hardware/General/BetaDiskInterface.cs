using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Hardware.General
{
    public class BetaDiskInterface : FddController
    {
        public BetaDiskInterface()
        {
            Name = "BDI-128";
            Description = "Beta Disk Interface\r\nDOSEN enabler + WD1793";
        }
        

        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            base.BusInit(bmgr);

            bmgr.Events.SubscribeRdMemM1(0xFF00, 0x3D00, BusReadMem3D00_M1);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0x4000, BusReadMemRam);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0x8000, BusReadMemRam);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0xC000, BusReadMemRam);

            bmgr.Events.SubscribeReset(BusReset);
            bmgr.Events.SubscribeNmiRq(BusNmiRq);
            bmgr.Events.SubscribeNmiAck(BusNmiAck);
        }

        #endregion


        #region Private

        protected virtual void BusReadMem3D00_M1(ushort addr, ref byte value)
        {
            if (m_memory.IsRom48)
            {
                m_memory.DOSEN = true;
            }
        }

        protected virtual void BusReadMemRam(ushort addr, ref byte value)
        {
            if (m_memory.DOSEN)
            {
                m_memory.DOSEN = false;
            }
        }

        protected virtual void BusReset()
        {
            m_memory.DOSEN = false;
        }

        protected virtual void BusNmiRq(BusCancelArgs e)
        {
            e.Cancel = !m_memory.IsRom48;
        }

        protected virtual void BusNmiAck()
        {
            m_memory.DOSEN = true;
        }

        #endregion
    }
}
