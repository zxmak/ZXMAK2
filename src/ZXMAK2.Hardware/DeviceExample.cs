using System;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Hardware
{
    public class DeviceExample : BusDeviceBase
    {
        private byte _port00F1;
        private byte _port00F3;


        public DeviceExample()
        {
            Category = BusDeviceCategory.Other;
            Name = "Device Example";
            Description = "Device example.\nThe device listens for the ports 241 and 243 and returns the sum of the outputs through the port 241";
        }


        public override void BusInit(IBusManager bmgr)
        {
            bmgr.Events.SubscribeWrIo(0x00FF, 0x00F1, WritePort00F1);
            bmgr.Events.SubscribeWrIo(0x00FF, 0x00F3, WritePort00F3);
            bmgr.Events.SubscribeRdIo(0x00FF, 0x00F1, ReadPort00F1);
        }

        public override void BusConnect()
        {
        }

        public override void BusDisconnect()
        {
        }

        /// <summary>
        /// Handle write port #F1 (decimal 241)
        /// </summary>
        public void WritePort00F1(ushort addr, byte value, ref bool handled)
        {
            if (handled)        // write port is already handled by another device
                return;
            handled = true;     // set handled flag
            
            _port00F1 = value;
        }

        /// <summary>
        /// Handle write port #F3 (decimal 243)
        /// </summary>
        public void WritePort00F3(ushort addr, byte value, ref bool handled)
        {
            if (handled)        // write port is already handled by another device
                return;
            handled = true;     // set handled flag
            
            _port00F3 = value;
        }

        /// <summary>
        /// Handle read port #F1 (decimal 241)
        /// </summary>
        public void ReadPort00F1(ushort addr, ref byte value, ref bool handled)
        {
            if (handled)        // read port is already handled by another device
                return;
            handled = true;     // set handled flag

            // add values written into ports #0001 and #0003 and return result
            value = (byte)(_port00F1 + _port00F3);
        }
    }
}
