using System;
using System.Text;
using System.Xml;
using ZXMAK2.Hardware.Circuits;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Hardware.General
{
    public class CmosGluck : BusDeviceBase
    {
        #region Fields

        private readonly RtcChip m_rtc = new RtcChip(RtcChipType.DS12885);
        private bool m_isSandBox;
        private string m_fileName;

        private IMemoryDevice m_memory;
        private bool m_noDos;
        private int m_maskAddr;
        private int m_portAddr;
        private int m_maskData;
        private int m_portData;


        #endregion Fields


        public CmosGluck()
        {
            Category = BusDeviceCategory.Other;
            Name = "CMOS GLUCK";

            m_noDos = false;
            m_maskAddr = 0xF008;
            m_maskData = 0xF008;
            m_portAddr = 0xD000;
            m_portData = 0xB000;
            OnProcessConfigChange();
        }


        #region Properies

        public bool NoDos
        {
            get { return m_noDos; }
            set
            {
                m_noDos = value;
                OnConfigChanged();
            }
        }

        public int MaskAddr
        {
            get { return m_maskAddr; }
            set
            {
                m_maskAddr = value;
                OnConfigChanged();
            }
        }

        public int PortAddr
        {
            get { return m_portAddr; }
            set
            {
                m_portAddr = value;
                OnConfigChanged();
            }
        }

        public int MaskData
        {
            get { return m_maskData; }
            set
            {
                m_maskData = value;
                OnConfigChanged();
            }
        }

        public int PortData
        {
            get { return m_portData; }
            set
            {
                m_portData = value;
                OnConfigChanged();
            }
        }

        protected override void OnConfigLoad(XmlNode node)
        {
            base.OnConfigLoad(node);
            NoDos = Utils.GetXmlAttributeAsBool(node, "noDos", NoDos);
            MaskAddr = Utils.GetXmlAttributeAsInt32(node, "maskAddr", MaskAddr);
            PortAddr = Utils.GetXmlAttributeAsInt32(node, "portAddr", PortAddr);
            MaskData = Utils.GetXmlAttributeAsInt32(node, "maskData", MaskData);
            PortData = Utils.GetXmlAttributeAsInt32(node, "portData", PortData);
        }

        protected override void OnConfigSave(XmlNode node)
        {
            base.OnConfigSave(node);
            Utils.SetXmlAttribute(node, "noDos", NoDos);
            Utils.SetXmlAttribute(node, "maskAddr", MaskAddr);
            Utils.SetXmlAttribute(node, "portAddr", PortAddr);
            Utils.SetXmlAttribute(node, "maskData", MaskData);
            Utils.SetXmlAttribute(node, "portData", PortData);
        }

        protected override void OnProcessConfigChange()
        {
            base.OnProcessConfigChange();

            // update description...
            var builder = new StringBuilder();
            builder.Append("GLUCK CMOS");
            builder.Append(Environment.NewLine);
            builder.Append("DS12885 based RTC/CMOS device");
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("NoDos: {0}", NoDos));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("MaskAddr:  #{0:X4}", MaskAddr));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("MaskData:  #{0:X4}", MaskData));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("PortAddr:  #{0:X4}", PortAddr));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("PortData:  #{0:X4}", PortData));
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            Description = builder.ToString();
        }

        #endregion Properies



        #region IBusDevice Members

        public override void BusInit(IBusManager bmgr)
        {
            m_isSandBox = bmgr.IsSandbox;
            m_memory = m_noDos ? bmgr.FindDevice<IMemoryDevice>() : null;
            bmgr.Events.SubscribeRdIo(MaskData, PortData & MaskData, BusReadData);   // DATA IN
            bmgr.Events.SubscribeWrIo(MaskData, PortData & MaskData, BusWriteData);  // DATA OUT
            bmgr.Events.SubscribeWrIo(MaskAddr, PortAddr & MaskAddr, BusWriteAddr);  // REG

            m_fileName = bmgr.GetSatelliteFileName("cmos");
        }

        public override void BusConnect()
        {
            if (!m_isSandBox && m_fileName != null)
            {
                m_rtc.Load(m_fileName);
            }
        }

        public override void BusDisconnect()
        {
            if (!m_isSandBox && m_fileName != null)
            {
                m_rtc.Save(m_fileName);
            }
        }

        #endregion


        #region Bus Handlers

        private void BusWriteAddr(ushort addr, byte value, ref bool handled)
        {
            if (handled || (m_memory != null && m_memory.DOSEN))
                return;
            handled = true;
            m_rtc.WriteAddr(value);
        }

        private void BusWriteData(ushort addr, byte value, ref bool handled)
        {
            if (handled || (m_memory != null && m_memory.DOSEN))
                return;
            handled = true;
            m_rtc.WriteData(value);
        }

        private void BusReadData(ushort addr, ref byte value, ref bool handled)
        {
            if (handled || (m_memory != null && m_memory.DOSEN))
                return;
            handled = true;
            m_rtc.ReadData(ref value);
        }

        #endregion
    }
}
