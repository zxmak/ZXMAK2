using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using System.Xml;
using ZXMAK2.Engine;
using System;
using System.Text;


namespace ZXMAK2.Hardware.General
{
    public class CovoxMono : SoundDeviceBase
    {
        private IMemoryDevice m_memory;
        private bool m_noDos;
        private int m_mask;
        private int m_port;

        
        public CovoxMono()
        {
            Category = BusDeviceCategory.Sound;
            Name = "COVOX MONO";

            m_noDos = true;
            m_mask = 0xFF;      // pentagon config by default
            m_port = 0xFB;
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

        public int Mask
        {
            get { return m_mask; }
            set
            {
                m_mask = value;
                OnConfigChanged();
            }
        }

        public int Port
        {
            get { return m_port; }
            set
            {
                m_port = value;
                OnConfigChanged();
            }
        }

        protected override void OnConfigLoad(XmlNode node)
        {
            base.OnConfigLoad(node);
            NoDos = Utils.GetXmlAttributeAsBool(node, "noDos", NoDos);
            Mask = Utils.GetXmlAttributeAsInt32(node, "mask", Mask);
            Port = Utils.GetXmlAttributeAsInt32(node, "port", Port);
        }

        protected override void OnConfigSave(XmlNode node)
        {
            base.OnConfigSave(node);
            Utils.SetXmlAttribute(node, "noDos", NoDos);
            Utils.SetXmlAttribute(node, "mask", Mask);
            Utils.SetXmlAttribute(node, "port", Port);
        }


        protected override void OnProcessConfigChange()
        {
            base.OnProcessConfigChange();

            // process Volume change...
            m_mult = (ushort.MaxValue * Volume) / (100 * 0xFF);

            // update description...
            var builder = new StringBuilder();
            builder.Append("COVOX MONO");
            builder.Append(Environment.NewLine);
            builder.Append("PENTAGON-type 8 bit mono covox");
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("NoDos: {0}", NoDos));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("Mask:  #{0:X4}", Mask));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("Port:  #{0:X4}", Port));
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
            //bmgr.SubscribeWrIo(0x0004, 0x00FB & 0x0004, WritePort);
            bmgr.Events.SubscribeWrIo(Mask, Port & Mask, WritePort);
        }

        #endregion

        private int m_mult = 0;


        private void WritePort(ushort addr, byte value, ref bool handled)
        {
            if (handled || (m_memory != null && m_memory.DOSEN))
                return;
            handled = true;

            var dac = (ushort)(value * m_mult);
            UpdateDac(dac, dac);
        }
    }
}
