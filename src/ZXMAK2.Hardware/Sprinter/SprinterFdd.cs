using System.Xml;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Engine.Cpu;
using ZXMAK2.Hardware.Circuits.Fdd;
using ZXMAK2.Model.Disk;
using ZXMAK2.Serializers;
using ZXMAK2.Host.Entities;
using ZXMAK2.Resources;


namespace ZXMAK2.Hardware.Sprinter
{
    public sealed class SprinterFdd : BusDeviceBase, IBetaDiskDevice
    {
        #region Fields

        private readonly IconDescriptor _iconRd = new IconDescriptor("FDDRD", ResourceImages.OsdFddRd);
        private readonly IconDescriptor _iconWr = new IconDescriptor("FDDWR", ResourceImages.OsdFddWr);
        private bool _sandbox = false;
        private CpuUnit _cpu;
        private IMemoryDevice _memory;
        private Wd1793 _wd = new Wd1793(2);

        private byte _isBdiMode;
        
        #endregion

        public SprinterFdd()
        {
            Category = BusDeviceCategory.Disk;
            Name = "FDD SPRINTER";
            Description = "Sprinter FDD controller WD1793";
            LoadManager = new DiskLoadManager(_wd.FDD[0]);
        }


        #region Properties

        public ISerializeManager LoadManager { get; private set; }

        public bool OpenPorts { get; set; }

        #endregion


        #region BusDeviceBase

        public override void BusInit(IBusManager bmgr)
        {
            _cpu = bmgr.CPU;
            _sandbox = bmgr.IsSandbox;
            _memory = bmgr.FindDevice<IMemoryDevice>();

            bmgr.RegisterIcon(_iconRd);
            bmgr.RegisterIcon(_iconWr);
            bmgr.Events.SubscribeBeginFrame(BusBeginFrame);
            bmgr.Events.SubscribeEndFrame(BusEndFrame);

            bmgr.Events.SubscribeRdMemM1(0xFF00, 0x3D00, BusReadMem3D00_M1);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0x4000, BusReadMemRam);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0x8000, BusReadMemRam);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0xC000, BusReadMemRam);

            OnSubscribeIo(bmgr);

            bmgr.Events.SubscribeReset(BusReset);
            bmgr.Events.SubscribeNmiRq(BusNmiRq);
            bmgr.Events.SubscribeNmiAck(BusNmiAck);

            foreach (var fs in LoadManager.GetSerializers())
            {
                bmgr.AddSerializer(fs);
            }
        }

        public override void BusConnect()
        {
            if (!_sandbox)
            {
                foreach (DiskImage di in FDD)
                    di.Connect();
            }
        }

        public override void BusDisconnect()
        {
            if (!_sandbox)
            {
                foreach (DiskImage di in FDD)
                    di.Disconnect();
            }
            if (_memory != null)
            {
                _memory.DOSEN = false;
            }
        }

        protected override void OnConfigLoad(XmlNode itemNode)
        {
            base.OnConfigLoad(itemNode);
            NoDelay = Utils.GetXmlAttributeAsBool(itemNode, "noDelay", false);
            LogIo = Utils.GetXmlAttributeAsBool(itemNode, "logIo", false);
            for (var i = 0; i < _wd.FDD.Length; i++)
            {
                var inserted = false;
                var readOnly = true;
                var fileName = string.Empty;
                var node = itemNode.SelectSingleNode(string.Format("Drive[@index='{0}']", i));
                if (node != null)
                {
                    inserted = Utils.GetXmlAttributeAsBool(node, "inserted", inserted);
                    readOnly = Utils.GetXmlAttributeAsBool(node, "readOnly", readOnly);
                    fileName = Utils.GetXmlAttributeAsString(node, "fileName", fileName);
                }
                // will be opened on Connect
                _wd.FDD[i].FileName = fileName;
                _wd.FDD[i].IsWP = readOnly;
                _wd.FDD[i].Present = inserted;
            }
        }

        protected override void OnConfigSave(XmlNode itemNode)
        {
            base.OnConfigSave(itemNode);
            Utils.SetXmlAttribute(itemNode, "noDelay", NoDelay);
            Utils.SetXmlAttribute(itemNode, "logIo", LogIo);
            for (var i = 0; i < _wd.FDD.Length; i++)
            {
                if (_wd.FDD[i].Present)
                {
                    XmlNode xn = itemNode.AppendChild(itemNode.OwnerDocument.CreateElement("Drive"));
                    Utils.SetXmlAttribute(xn, "index", i);
                    Utils.SetXmlAttribute(xn, "inserted", _wd.FDD[i].Present);
                    Utils.SetXmlAttribute(xn, "readOnly", _wd.FDD[i].IsWP);
                    if (!string.IsNullOrEmpty(_wd.FDD[i].FileName))
                    {
                        Utils.SetXmlAttribute(xn, "fileName", _wd.FDD[i].FileName);
                    }
                }
            }
        }

        #endregion


        #region IBetaDiskInterface

        public bool DOSEN
        {
            get { return _memory.DOSEN; }
            set { _memory.DOSEN = value; }
        }

        public DiskImage[] FDD
        {
            get { return _wd.FDD; }
        }

        public bool NoDelay
        {
            get { return _wd.NoDelay; }
            set { _wd.NoDelay = value; }
        }

        public bool LogIo { get; set; }

        #endregion

        #region Private

        private void OnSubscribeIo(IBusManager bmgr)
        {
            bmgr.Events.SubscribeWrIo(0x83, 0x1F & 0x83, WritePortFdc);
            bmgr.Events.SubscribeRdIo(0x83, 0x1F & 0x83, ReadPortFdc);
            bmgr.Events.SubscribeWrIo(0x83, 0xFF & 0x83, WritePortSys);
            bmgr.Events.SubscribeRdIo(0x83, 0xFF & 0x83, ReadPortSys);
            bmgr.Events.SubscribeWrIo(0xFF, 0xBD, WritePortBdiMode);
        }

        private void BusBeginFrame()
        {
            _wd.LedRd = false;
            _wd.LedWr = false;
        }

        private void BusEndFrame()
        {
            _iconWr.Visible = _wd.LedWr;
            _iconRd.Visible = !_wd.LedWr && _wd.LedRd;
        }

        private void ReadPortFdc(ushort addr, ref byte value, ref bool handled)
        {
            if (!handled && (this.DOSEN || this.OpenPorts))
            {
                handled = true;
                int fdcReg = (addr & 0x60) >> 5;
                value = _wd.Read(_cpu.Tact, (WD93REG)fdcReg);
                if (LogIo)
                {
                    LogIoRead(_cpu.Tact, (WD93REG)fdcReg, value);
                }
            }
        }

        private void ReadPortSys(ushort addr, ref byte value, ref bool handled)
        {
            if (!handled && (this.DOSEN || this.OpenPorts))
            {
                handled = true;
                value = _wd.Read(_cpu.Tact, WD93REG.SYS);
                if (LogIo)
                {
                    LogIoRead(_cpu.Tact, WD93REG.SYS, value);
                }
            }
        }

        private void WritePortBdiMode(ushort addr, byte value, ref bool handled)
        {
            if (!handled && (this.DOSEN || this.OpenPorts))
            {
                handled = true;
                //0x21 - Set 1440 
                //0x01 - Set  720
                _isBdiMode = value;
                
                if (LogIo)
                {
                    Logger.Debug(
                        "WD93 BDI MODE <== #{0:X2} [PC=#{1:X4}, T={2}]",
                        value,
                        _cpu.regs.PC,
                        _cpu.Tact);
                }
            }
        }

        private void WritePortFdc(ushort addr, byte value, ref bool handled)
        {
            if (!handled && (this.DOSEN || this.OpenPorts))
            {
                handled = true;
                int fdcReg = (addr & 0x60) >> 5;
                if (LogIo)
                {
                    LogIoWrite(_cpu.Tact, (WD93REG)fdcReg, value);
                }
                _wd.Write(_cpu.Tact, (WD93REG)fdcReg, value);
            }
        }


        private void WritePortSys(ushort addr, byte value, ref bool handled)
        {
            if (!handled && (this.DOSEN || this.OpenPorts))
            {
                handled = true;
                if (LogIo)
                {
                    LogIoWrite(_cpu.Tact, WD93REG.SYS, value);
                }
                _wd.Write(_cpu.Tact, WD93REG.SYS, value);
            }
        }

        private void BusReadMem3D00_M1(ushort addr, ref byte value)
        {
            if (_memory.IsRom48)
            {
                DOSEN = true;
            }
        }

        private void BusReadMemRam(ushort addr, ref byte value)
        {
            if (DOSEN)
            {
                DOSEN = false;
            }
        }

        private void BusReset()
        {
            DOSEN = false;
            _isBdiMode = 1; // Возможно не верное значение
        }

        private void BusNmiRq(BusCancelArgs e)
        {
            e.Cancel = DOSEN;
        }

        private void BusNmiAck()
        {
            DOSEN = true;
        }

        private void LogIoWrite(long tact, WD93REG reg, byte value)
        {
            Logger.Debug(
                "WD93 {0} <== #{1:X2} [PC=#{2:X4}, T={3}]",
                reg,
                value,
                _cpu.regs.PC,
                tact);
        }

        private void LogIoRead(long tact, WD93REG reg, byte value)
        {
            Logger.Debug(
                "WD93 {0} ==> #{1:X2} [PC=#{2:X4}, T={3}]",
                reg,
                value,
                _cpu.regs.PC,
                tact);
        }

        #endregion
    }
}
