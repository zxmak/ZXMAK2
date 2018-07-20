using System;
using System.IO;
using System.Xml;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Cpu;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Hardware.Circuits.Ata;
using ZXMAK2.Host.Entities;
using ZXMAK2.Resources;


namespace ZXMAK2.Hardware.Profi
{
    public class IdeProfi : BusDeviceBase
    {
        #region Fields

        private bool m_sandbox = false;
        private CpuUnit m_cpu;
        private IMemoryDevice m_memory;
        private IconDescriptor m_iconHdd = new IconDescriptor("HDD", ResourceImages.OsdHddRd);
        private AtaPort m_ata = new AtaPort();
        private string m_ideFileName;
        private int m_ide_write;
        private int m_ide_read;

        #endregion Fields


        public IdeProfi()
        {
            Category = BusDeviceCategory.Disk;
            Name = "IDE PROFI";
            Description = "PROFI IDE controller\r\nPlease edit *.vmide file for configuration settings";
        }


        #region IBusDevice Members

        public override void BusInit(IBusManager bmgr)
        {
            m_sandbox = bmgr.IsSandbox;
            m_cpu = bmgr.CPU;
            m_memory = bmgr.FindDevice<IMemoryDevice>();

            m_ideFileName = bmgr.GetSatelliteFileName("vmide");

            bmgr.RegisterIcon(m_iconHdd);
            bmgr.Events.SubscribeBeginFrame(BusBeginFrame);
            bmgr.Events.SubscribeEndFrame(BusEndFrame);

            bmgr.Events.SubscribeReset(BusReset);

            bmgr.Events.SubscribeRdIo(0x9F, 0x8B, ReadIde);
            bmgr.Events.SubscribeWrIo(0x9F, 0x8B, WriteIde);
        }

        public override void BusConnect()
        {
            if (!m_sandbox)
            {
                Load();
                m_ata.Open();
            }
        }

        private void Load()
        {
            if (string.IsNullOrEmpty(m_ideFileName))
            {
                return;
            }
            if (File.Exists(m_ideFileName))
            {
                m_ata.Devices[0].DeviceInfo.Load(m_ideFileName);
            }
            else
            {
                m_ata.Devices[0].DeviceInfo.Save(m_ideFileName);
            }
        }

        public override void BusDisconnect()
        {
            //if (!m_sandbox)
            //{
            //}
        }

        protected override void OnConfigLoad(XmlNode itemNode)
        {
            base.OnConfigLoad(itemNode);
            LogIo = Utils.GetXmlAttributeAsBool(itemNode, "logIo", false);
        }

        protected override void OnConfigSave(XmlNode itemNode)
        {
            base.OnConfigSave(itemNode);
            Utils.SetXmlAttribute(itemNode, "logIo", LogIo);
        }

        #endregion


        #region Properties

        public bool LogIo
        {
            get { return m_ata.LogIo; }
            set { m_ata.LogIo = value; }
        }

        #endregion


        #region Private

        protected virtual void BusBeginFrame()
        {
            m_ata.LedIo = false;
        }

        protected virtual void BusEndFrame()
        {
            m_iconHdd.Visible = m_ata.LedIo;
        }

        protected virtual void BusReset()
        {
        }

        protected virtual bool IsExtendedMode
        {
            get
            {
                var cpm = (m_memory.CMR1 & 0x20) != 0;
                var rom48 = (m_memory.CMR0 & 0x10) != 0;
                var csExtended = cpm && rom48;
                return csExtended;
            }
        }

        protected virtual void WriteIde(ushort addr, byte value, ref bool handled)
        {
            if (handled || !IsExtendedMode)
                return;

            if ((addr & 0x40) != 0)
            {
                handled = true;
                // cs1
                if ((addr & 0x20) == 0)
                {
                    if (LogIo)
                    {
                        Logger.Info("IDE WR DATA HI: #{0:X2} @ PC=#{1:X4}", value, m_cpu.regs.PC);
                    }
                    m_ide_write = value;
                    return;
                }
                addr >>= 8;
                addr &= 7;
                if (addr != 0)
                {
                    AtaWrite((AtaReg)addr, value);
                }
                else
                {
                    var dataWord = (ushort)(value | (m_ide_write << 8));
                    if (LogIo)
                    {
                        Logger.Info("IDE WR DATA LO: #{0:X2} @ PC=#{1:X4} [#{2:X4}]", value, m_cpu.regs.PC, dataWord);
                    }
                    m_ata.WriteData(dataWord);
                }
                return;
            }
            if ((addr & 0x20) != 0)
            {
                // cs3
                if (((addr >> 8) & 7) == 6)
                {
                    handled = true;
                    AtaWrite(AtaReg.ControlAltStatus, value);
                    return;
                }
            }
        }

        protected virtual void ReadIde(ushort addr, ref byte value, ref bool handled)
        {
            if (handled || !IsExtendedMode)
                return;

            if ((addr & 0x40) != 0)
            {
                // cs1
                handled = true;
                if ((addr & 0x20) != 0)
                {
                    value = (byte)m_ide_read;
                    if (LogIo)
                    {
                        Logger.Info("IDE RD DATA HI: #{0:X2} @ PC=#{1:X4}", value, m_cpu.regs.PC);
                    }
                    return;
                }
                addr >>= 8;
                addr &= 7;
                if (addr != 0)
                {
                    value = AtaRead((AtaReg)addr);
                    return;
                }
                var dataWord = m_ata.ReadData();
                m_ide_read = (dataWord >> 8) & 0xFF;
                value = (byte)dataWord;
                if (LogIo)
                {
                    Logger.Info("IDE RD DATA LO: #{0:X2} @ PC=#{1:X4} [#{2:X4}]", value, m_cpu.regs.PC, dataWord);
                }
                return;
            }
        }


        private void AtaWrite(AtaReg ataReg, byte value)
        {
            if (LogIo)
            {
                Logger.Info("IDE WR {0,-13}: #{1:X2} @ PC=#{2:X4}", ataReg, value, m_cpu.regs.PC);
            }
            m_ata.Write(ataReg, value);
        }

        private byte AtaRead(AtaReg ataReg)
        {
            var value = m_ata.Read(ataReg);
            if (LogIo)
            {
                Logger.Info("IDE RD {0,-13}: #{1:X2} @ PC=#{2:X4}", ataReg, value, m_cpu.regs.PC);
            }
            return value;
        }

        //--
        // ??m_ata.reset();
        // ??value = m_ata.read_intrq() & 0x80
        #endregion Private
    }
}
