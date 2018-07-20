using System;
using ZXMAK2.Hardware.General;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Hardware.Atm
{
    public class FddAtm450 : FddController
    {
        public FddAtm450()
        {
            Name = "FDD ATM450";
            Description = "FDD WD1793 + specific ATM450 ports";
        }


        protected override void OnSubscribeIo(IBusManager bmgr)
        {
            bmgr.Events.SubscribeWrIo(0x0083, 0x001F & 0x0083, BusWriteFdc);
            bmgr.Events.SubscribeRdIo(0x0083, 0x001F & 0x0083, BusReadFdc);
            bmgr.Events.SubscribeWrIo(0x00E3, 0x00FF & 0x00E3, BusWriteSys);
            bmgr.Events.SubscribeRdIo(0x00E3, 0x00FF & 0x00E3, BusReadSys);
        }
    }
}
