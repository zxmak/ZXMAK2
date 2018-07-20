using ZXMAK2.Hardware.Circuits;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Hardware.Sprinter
{
    public sealed class SprinterRTC : BusDeviceBase
    {
        #region Fields

        private readonly RtcChip _rtc = new RtcChip(RtcChipType.DS12885);
        private bool _isSandBox;
        private string _fileName;

        #endregion Fields


        public SprinterRTC()
        {
            Category = BusDeviceCategory.Other;
            Name = "CMOS SPRINTER";
            Description = "Sprinter RTC";
        }
        

        #region IBusDevice Members

        public override void BusInit(IBusManager bmgr)
        {
            _isSandBox = bmgr.IsSandbox;
            bmgr.Events.SubscribeReset(ResetBus);
            bmgr.Events.SubscribeWrIo(0xFFFF, 0xBFBD, WritePortData);  //CMOS_DWR
            bmgr.Events.SubscribeWrIo(0xFFFF, 0xDFBD, WritePortAddr);  //CMOS_AWR
            bmgr.Events.SubscribeRdIo(0xFFFF, 0xFFBD, ReadPortData);  //CMOS_DRD

            _fileName = bmgr.GetSatelliteFileName("cmos");
        }

        public override void BusConnect()
        {
            if (!_isSandBox && _fileName != null)
            {
                _rtc.Load(_fileName);
            }
        }

        public override void BusDisconnect()
        {
            if (!_isSandBox && _fileName != null)
            {
                _rtc.Save(_fileName);
            }
        }

        #endregion


        #region Bus

        private void ResetBus()
        {
            _rtc.WriteAddr(0);
        }


        /// <summary>
        /// RTC address port
        /// </summary>
        private void WritePortAddr(ushort addr, byte val, ref bool handled)
        {
            _rtc.WriteAddr(val);
        }

        /// <summary>
        /// RTC write data port
        /// </summary>
        private void WritePortData(ushort addr, byte val, ref bool handled)
        {
            _rtc.WriteData(val);
        }

        /// <summary>
        /// RTC read data port
        /// </summary>
        private void ReadPortData(ushort addr, ref byte val, ref bool handled)
        {
            _rtc.ReadData(ref val);
        }

        #endregion
    }
}
