using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using System.Xml;
using ZXMAK2.Engine;
using System;
using System.Text;


namespace ZXMAK2.Hardware.General
{
    public class CovoxStereo : SoundDeviceBase
    {
        private IMemoryDevice m_memory;
        private bool m_noDos;
        private int m_maskL;
        private int m_portL;
        private int m_maskR;
        private int m_portR;

        private ushort m_left = 0;
        private ushort m_right = 0;
        private int m_mult = 0;

        
        public CovoxStereo()
        {
            Category = BusDeviceCategory.Sound;
            Name = "COVOX STEREO";

            m_noDos = true;
            m_maskL = 0xFF;     // PROFI config by default
            m_portL = 0x5F;
            m_maskR = 0xFF;
            m_portR = 0x3F;
            OnProcessConfigChange();
        }


        #region Properties

        public bool NoDos
        {
            get { return m_noDos; }
            set
            {
                m_noDos = value;
                OnConfigChanged();
            }
        }

        public int MaskL
        {
            get { return m_maskL; }
            set
            {
                m_maskL = value;
                OnConfigChanged();
            }
        }

        public int PortL
        {
            get { return m_portL; }
            set
            {
                m_portL = value;
                OnConfigChanged();
            }
        }

        public int MaskR
        {
            get { return m_maskR; }
            set
            {
                m_maskR = value;
                OnConfigChanged();
            }
        }

        public int PortR
        {
            get { return m_portR; }
            set
            {
                m_portR = value;
                OnConfigChanged();
            }
        }

        protected override void OnConfigLoad(XmlNode node)
        {
            base.OnConfigLoad(node);
            NoDos = Utils.GetXmlAttributeAsBool(node, "noDos", NoDos);
            MaskL = Utils.GetXmlAttributeAsInt32(node, "maskL", MaskL);
            PortL = Utils.GetXmlAttributeAsInt32(node, "portL", PortL);
            MaskR = Utils.GetXmlAttributeAsInt32(node, "maskR", MaskR);
            PortR = Utils.GetXmlAttributeAsInt32(node, "portR", PortR);
        }

        protected override void OnConfigSave(XmlNode node)
        {
            base.OnConfigSave(node);
            Utils.SetXmlAttribute(node, "noDos", NoDos);
            Utils.SetXmlAttribute(node, "maskL", MaskL);
            Utils.SetXmlAttribute(node, "portL", PortL);
            Utils.SetXmlAttribute(node, "maskR", MaskR);
            Utils.SetXmlAttribute(node, "portR", PortR);
        }


        protected override void OnProcessConfigChange()
        {
            base.OnProcessConfigChange();

            // process Volume change...
            m_mult = (ushort.MaxValue * Volume) / (100 * 0xFF);

            // update description...
            var builder = new StringBuilder();
            builder.Append("COVOX STEREO");
            builder.Append(Environment.NewLine);
            builder.Append("PROFI-type 8-bit stereo covox");
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("NoDos: {0}", NoDos));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("MaskL:  #{0:X4}", MaskL));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("MaskR:  #{0:X4}", MaskR));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("PortL:  #{0:X4}", PortL));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("PortR:  #{0:X4}", PortR));
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            Description = builder.ToString();
        }

        #endregion Properties

        

        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            base.BusInit(bmgr);
            m_memory = m_noDos ? bmgr.FindDevice<IMemoryDevice>() : null;
            bmgr.Events.SubscribeWrIo(MaskR, PortR & MaskR, WritePortR);
            bmgr.Events.SubscribeWrIo(MaskL, PortL & MaskL, WritePortL);
        }

        #endregion


        private void WritePortL(ushort addr, byte value, ref bool handled)
        {
            if (handled || (m_memory != null && m_memory.DOSEN))
                return;
            handled = true;
            
            m_left = (ushort)(value * m_mult);
            UpdateDac(m_left, m_right);
        }

        private void WritePortR(ushort addr, byte value, ref bool handled)
        {
            if (handled || (m_memory != null && m_memory.DOSEN))
                return;
            handled = true;
            
            m_right = (ushort)(value * m_mult);
            UpdateDac(m_left, m_right);
        }
    }
}
