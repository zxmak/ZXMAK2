using System;
using ZXMAK2.Hardware.General;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Hardware.Evo
{
    public class FddPentEvo : FddController
    {
        private byte m_p2F, m_p4F, m_p6F, m_p8F;

        
        public FddPentEvo()
        {
            Name = "FDD PentEvo";
            Description = "FDD WD1793 + specific PentEvo ports";
        }


        protected override void OnSubscribeIo(IBusManager bmgr)
        {
            bmgr.Events.SubscribeWrIo(0x009F, 0x001F, BusWriteFdc);
            bmgr.Events.SubscribeRdIo(0x009F, 0x001F, BusReadFdc);
            bmgr.Events.SubscribeWrIo(0x00FF, 0x00FF, BusWriteSys);
            bmgr.Events.SubscribeRdIo(0x00FF, 0x00FF, BusReadSys);

            bmgr.Events.SubscribeWrIo(0x00FF, 0x002F, WritePortEmu);
            bmgr.Events.SubscribeWrIo(0x00FF, 0x004F, WritePortEmu);
            bmgr.Events.SubscribeWrIo(0x00FF, 0x006F, WritePortEmu);
            bmgr.Events.SubscribeWrIo(0x00FF, 0x008F, WritePortEmu);
            bmgr.Events.SubscribeRdIo(0x00FF, 0x002F, ReadPortEmu);
            bmgr.Events.SubscribeRdIo(0x00FF, 0x004F, ReadPortEmu);
            bmgr.Events.SubscribeRdIo(0x00FF, 0x006F, ReadPortEmu);
            bmgr.Events.SubscribeRdIo(0x00FF, 0x008F, ReadPortEmu);
        }

        protected virtual void WritePortEmu(ushort addr, byte val, ref bool handled)
        {
            if (IsActive)
            {
                switch ((addr & 0x00FF) >> 4)
                {
                    case 0x02:
                        m_p2F = val;
                        break;
                    case 0x04:
                        m_p4F = val;
                        break;
                    case 0x06:
                        m_p6F = val;
                        break;
                    case 0x08:
                        m_p8F = val;
                        break;
                }
            }
        }

        protected virtual void ReadPortEmu(ushort addr, ref byte val, ref bool handled)
        {
            if (IsActive)
            {
                switch ((addr & 0x00FF) >> 4)
                {
                    case 0x02:
                        val = m_p2F;
                        break;
                    case 0x04:
                        val = m_p4F;
                        break;
                    case 0x06:
                        val = m_p6F;
                        break;
                    case 0x08:
                        val = m_p8F;
                        break;
                }
            }
        }
    }
}
