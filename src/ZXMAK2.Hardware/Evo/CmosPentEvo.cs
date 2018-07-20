using System;
using System.IO;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Hardware.Evo
{
    public class CmosPentEvo : BusDeviceBase
    {
        #region Fields

        private bool sandbox;
        private MemoryPentEvo mem;
        private FileStream eepromFile;
        private byte[] eeprom;
        private Mode mode;
        private byte addr;

        private byte[] BaseConfData;
        private byte[] BootVerData;
        private string m_fileName = null;

        #endregion


        public CmosPentEvo()
        {
            Category = BusDeviceCategory.Other;
            Name = "CMOS PentEvo";
            Description = "PentEvo RTC";
            
            eeprom = new byte[256];
            BaseConfData = new byte[16];
            BootVerData = new byte[16];
        }


        public bool SHADOW
        {
            get { return (mem == null) ? false : mem.DOSEN || mem.SYSEN || mem.SHADOW; }
        }

        private bool visable
        {
            get { return (mem == null) ? false : mem.CMOSEN; }
        }


        #region BusDeviceBase

        public override void BusInit(IBusManager bmgr)
        {
            sandbox = bmgr.IsSandbox;
            mem = bmgr.FindDevice<MemoryPentEvo>();
            m_fileName = bmgr.GetSatelliteFileName("cmos");

            bmgr.Events.SubscribeReset(Reset);
            bmgr.Events.SubscribeWrIo(0xFEFF, 0xDEF7, WrDEF7);
            bmgr.Events.SubscribeWrIo(0xFEFF, 0xBEF7, WrBEF7);
            bmgr.Events.SubscribeRdIo(0xFEFF, 0xBEF7, RdBEF7);
        }

        public override void BusConnect()
        {
            if (!sandbox)
            {
                using (eepromFile = File.Open(m_fileName, FileMode.OpenOrCreate))
                {
                    if (eepromFile.Length < 256)
                        eepromFile.Write(eeprom, 0, 256);
                    else
                        eepromFile.Read(eeprom, 0, 256);

                    eepromFile.Flush();
                    eepromFile.Close();
                }

                SetData(BaseConfData, "BaseConf Emu", new DateTime(2011, 4, 3));
                SetData(BootVerData, "Boot Emu", new DateTime(2012, 04, 5));
            }
        }

        public override void BusDisconnect()
        {
            if (!sandbox)
            {
                using (eepromFile = File.Open(m_fileName, FileMode.OpenOrCreate))
                {
                    eepromFile.Write(eeprom, 0, 256);
                    eepromFile.Flush();
                    eepromFile.Close();
                }
            }
        }

        #endregion


        #region Private

        void Reset()
        {
            mode = Mode.BaseConfVer; // посмотреть состояние по сбросу
            addr = 0;
        }


        /// <summary>
        /// RTC address port
        /// </summary>
        private void WrDEF7(ushort addr, byte val, ref bool handled)
        {
            if ((addr & 0x0100) == 0 && SHADOW)
                this.addr = val;
            else if ((addr & 0x0100) != 0 && !SHADOW && visable)
                this.addr = val;
        }

        /// <summary>
        /// RTC write data port
        /// </summary>
        private void WrBEF7(ushort addr, byte val, ref bool handled)
        {
            if ((addr & 0x0100) == 0 && SHADOW)
                WrCMOS(val);
            else if ((addr & 0x0100) != 0 && !SHADOW && visable)
                WrCMOS(val);
        }

        /// <summary>
        /// RTC read data port
        /// </summary>
        private void RdBEF7(ushort addr, ref byte val, ref bool handled)
        {
            if ((addr & 0x0100) == 0 && SHADOW)
                val = RdCMOS();
            else if ((addr & 0x0100) != 0 && !SHADOW & visable)
                val = RdCMOS();
        }

        #endregion


        #region RTC emu

        DateTime dt = DateTime.Now;
        bool UF = false;

        byte RdCMOS()
        {
            var curDt = DateTime.Now;

            if (curDt.Subtract(dt).Seconds > 0 || curDt.Millisecond / 500 != dt.Millisecond / 500)
            {
                dt = curDt;
                UF = true;
            }

            if (addr < 0xF0)
            {
                switch (addr)
                {
                    case 0x00:
                        return BDC(dt.Second);
                    case 0x02:
                        return BDC(dt.Minute);
                    case 0x04:
                        return BDC(dt.Hour);
                    case 0x06:
                        return (byte)(dt.DayOfWeek);
                    case 0x07:
                        return BDC(dt.Day);
                    case 0x08:
                        return BDC(dt.Month);
                    case 0x09:
                        return BDC(dt.Year % 100);
                    case 0x0A:
                        return 0x00;
                    case 0x0B:
                        return 0x02;
                    case 0x0C:
                        var res = (byte)(UF ? 0x1C : 0x0C);
                        UF = false;
                        return res;
                    case 0x0D:
                        return 0x80;

                    default:
                        return eeprom[addr];
                }
            }
            else
            {
                switch (mode)
                {
                    case Mode.BaseConfVer:
                        return BaseConfData[addr & 0x0F];
                    case Mode.BootVer:
                        return BootVerData[addr & 0x0F];
                    case Mode.PS2Keyboard:
                        return 0x00;
                    default:
                        return 0xFF;
                }
            }
        }

        void WrCMOS(byte val)
        {
            if (addr < 0xF0)
                eeprom[addr] = val;
            else
            {
                switch (val)
                {
                    case 0:
                        mode = Mode.BaseConfVer;
                        break;
                    case 1:
                        mode = Mode.BootVer;
                        break;
                    case 2:
                        mode = Mode.PS2Keyboard;
                        break;

                    default: // посмотреть поведение по другим кодам
                        mode = Mode.BaseConfVer;
                        break;
                }
            }
        }

        byte BDC(int val)
        {
            var res = val;

            if ((eeprom[11] & 4) == 0)
            {
                var rem = 0;
                res = Math.DivRem(val, 10, out rem);
                res = (res * 16 + rem);
            }

            return (byte)res;
        }

        #endregion

        private void SetData(byte[] BaseConfData, string name, DateTime dateTime)
        {
            var tmp = name.PadRight(12, (char)0).Substring(0, 12);

            for (var i = 0; i < 12; i++)
                BaseConfData[i] = (byte)tmp[i];

            BaseConfData[13] = (byte)(((dateTime.Year - 2000) & 0x3F) << 1);
            var mnt = (byte)(dateTime.Month & 0x0F);
            BaseConfData[13] |= (byte)(mnt >> 3);
            BaseConfData[12] |= (byte)(mnt << 5);
            BaseConfData[12] |= (byte)(dateTime.Day & 0x1F);

        }

        private enum Mode
        {
            BaseConfVer,
            BootVer,
            PS2Keyboard
        }
    }
}
