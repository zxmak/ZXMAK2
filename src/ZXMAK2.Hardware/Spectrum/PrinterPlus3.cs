using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Hardware.Spectrum
{
    public class PrinterPlus3 : BusDeviceBase
    {
        public PrinterPlus3()
        {
            Category = BusDeviceCategory.Other;
            Name = "LPT PLUS-3";
            Description = "Plus-3 LPT Printer (Centronix)\r\nPrint to the file (settings not implemented yet)";
        }


        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            bmgr.Events.SubscribeRdIo(0xF002, 0x0000, portDataRead);
            bmgr.Events.SubscribeWrIo(0xF002, 0x0000, portDataWrite);
            bmgr.Events.SubscribeWrIo(0xF002, 0x1000, portStrbWrite);
        }

        public override void BusConnect()
        {
        }

        public override void BusDisconnect()
        {
        }

        #endregion

        private byte m_data = 0;
        private byte m_strb = 0;

        private void portDataWrite(ushort addr, byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;
            m_data = value;
        }

        private void portDataRead(ushort addr, ref byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;
            value &= 0xFE;	// reset BUSY flag to show ready state
        }

        private void portStrbWrite(ushort addr, byte value, ref bool handled)
        {
            if ((m_strb & 0x10) == 0 && (value & 0x10) != 0)
            {
                //using (FileStream fs = new FileStream("C:\\ZXPRN.TXT", FileMode.Append, FileAccess.Write, FileShare.Read))
                //{
                //    fs.WriteByte(m_data);
                //}
            }
            m_strb = value;
        }
    }
}
