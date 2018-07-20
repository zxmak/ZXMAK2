using System;
using System.IO;
using System.Xml;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Engine.Cpu;
using ZXMAK2.Hardware.Circuits.Ata;
using ZXMAK2.Host.Entities;
using ZXMAK2.Resources;


namespace ZXMAK2.Hardware.Atm
{
    public class IdeAtm : BusDeviceBase
    {
        #region Fields

        private readonly AtaPort m_ata = new AtaPort();
        private readonly IconDescriptor m_iconHdd = new IconDescriptor("HDD", ResourceImages.OsdHddRd);
        private bool m_sandbox = false;
        private CpuUnit m_cpu;
        private string m_ideFileName;
        private byte m_ide_wr_hi;
        private byte m_ide_rd_hi;
        
        #endregion Fields


        public IdeAtm()
        {
            Category = BusDeviceCategory.Disk;
            Name = "IDE ATM";
            Description = "ATM IDE controller\r\nPlease edit *.vmide file for configuration settings";
        }


        #region IBusDevice Members

        public override void BusInit(IBusManager bmgr)
        {
            m_sandbox = bmgr.IsSandbox;
            m_cpu = bmgr.CPU;

            m_ideFileName = bmgr.GetSatelliteFileName("vmide");

            bmgr.RegisterIcon(m_iconHdd);
            bmgr.Events.SubscribeBeginFrame(BusBeginFrame);
            bmgr.Events.SubscribeEndFrame(BusEndFrame);

            bmgr.Events.SubscribeReset(BusReset);

            const int mask = 0x001F;
            bmgr.Events.SubscribeRdIo(mask, 0xEF & mask, ReadIde);
            bmgr.Events.SubscribeWrIo(mask, 0xEF & mask, WriteIde);
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

        protected virtual void WriteIde(ushort addr, byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;

            if ((addr & 0x0100) != 0)
            {
                if (LogIo)
                {
                    Logger.Info("IDE WR DATA HI: #{0:X2} @ PC=#{1:X4}", value, m_cpu.regs.PC);
                }
                m_ide_wr_hi = value;
                return;
            }
            addr >>= 5;
            addr &= 7;
            if (addr != 0)
            {
                AtaWrite((AtaReg)addr, value);
            }
            else
            {
                var data = value | (m_ide_wr_hi << 8);
                if (LogIo)
                {
                    Logger.Info("IDE WR DATA LO: #{0:X2} @ PC=#{1:X4} [#{2:X4}]", value, m_cpu.regs.PC, data);
                }
                m_ata.WriteData((ushort)data);
            }
            // ??m_ata.reset();
        }

        protected virtual void ReadIde(ushort addr, ref byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;

            if ((addr & 0x0100) != 0)
            {
                value = m_ide_rd_hi;
                if (LogIo)
                {
                    Logger.Info("IDE RD DATA HI: #{0:X2} @ PC=#{1:X4}", value, m_cpu.regs.PC);
                }
                return;
            }
            addr >>= 5;
            addr &= 7;
            if (addr != 0)
            {
                value = AtaRead((AtaReg)addr);
            }
            else
            {
                var data = m_ata.ReadData();
                m_ide_rd_hi = (byte)(data >> 8);
                value = (byte)data;
                if (LogIo)
                {
                    Logger.Info("IDE RD DATA LO: #{0:X2} @ PC=#{1:X4} [#{2:X4}]", value, m_cpu.regs.PC, data);
                }
            }
            // ??value = m_ata.read_intrq() & 0x80
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

        #endregion Private
    }
}
