using ZXMAK2.Host.Interfaces;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using System.Xml;
using ZXMAK2.Engine;
using System.Text;
using System;


namespace ZXMAK2.Hardware.General
{
    public class KempstonMouseDevice : BusDeviceBase, IMouseDevice
    {
        #region Fields

        private IMemoryDevice m_memory;
        private bool m_noDos;
        private int m_maskX;
        private int m_portX;
        private int m_maskY;
        private int m_portY;
        private int m_maskB;
        private int m_portB;
        
        #endregion Fields


        public KempstonMouseDevice()
        {
            Category = BusDeviceCategory.Mouse;
            Name = "MOUSE KEMPSTON";

            var mask = 0xFFFF;// 0x05FF;
            m_noDos = true;
            m_maskX = mask;
            m_maskY = mask;
            m_maskB = mask;
            m_portX = 0xFBDF;
            m_portY = 0xFFDF;
            m_portB = 0xFADF;
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

        public int MaskX
        {
            get { return m_maskX; }
            set
            {
                m_maskX = value;
                OnConfigChanged();
            }
        }

        public int PortX
        {
            get { return m_portX; }
            set
            {
                m_portX = value;
                OnConfigChanged();
            }
        }

        public int MaskY
        {
            get { return m_maskY; }
            set
            {
                m_maskY = value;
                OnConfigChanged();
            }
        }

        public int PortY
        {
            get { return m_portY; }
            set
            {
                m_portY = value;
                OnConfigChanged();
            }
        }

        public int MaskB
        {
            get { return m_maskB; }
            set
            {
                m_maskB = value;
                OnConfigChanged();
            }
        }

        public int PortB
        {
            get { return m_portB; }
            set
            {
                m_portB = value;
                OnConfigChanged();
            }
        }

        protected override void OnConfigLoad(XmlNode node)
        {
            base.OnConfigLoad(node);
            NoDos = Utils.GetXmlAttributeAsBool(node, "noDos", NoDos);
            MaskX = Utils.GetXmlAttributeAsInt32(node, "maskX", MaskX);
            PortX = Utils.GetXmlAttributeAsInt32(node, "portX", PortX);
            MaskY = Utils.GetXmlAttributeAsInt32(node, "maskY", MaskY);
            PortY = Utils.GetXmlAttributeAsInt32(node, "portY", PortY);
            MaskB = Utils.GetXmlAttributeAsInt32(node, "maskB", MaskB);
            PortB = Utils.GetXmlAttributeAsInt32(node, "portB", PortB);
        }

        protected override void OnConfigSave(XmlNode node)
        {
            base.OnConfigSave(node);
            Utils.SetXmlAttribute(node, "noDos", NoDos);
            Utils.SetXmlAttribute(node, "maskX", MaskX);
            Utils.SetXmlAttribute(node, "portX", PortX);
            Utils.SetXmlAttribute(node, "maskY", MaskY);
            Utils.SetXmlAttribute(node, "portY", PortY);
            Utils.SetXmlAttribute(node, "maskB", MaskB);
            Utils.SetXmlAttribute(node, "portB", PortB);
        }


        protected override void OnProcessConfigChange()
        {
            base.OnProcessConfigChange();

            // update description...
            var builder = new StringBuilder();
            builder.Append("KEMPSTON MOUSE");
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("NoDos: {0}", NoDos));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("MaskX:  #{0:X4}", MaskX));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("MaskY:  #{0:X4}", MaskY));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("MaskB:  #{0:X4}", MaskB));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("PortX:  #{0:X4}", PortX));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("PortY:  #{0:X4}", PortY));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("PortB:  #{0:X4}", PortB));
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            Description = builder.ToString();
        }

        #endregion Properties


        #region IBusDevice Members

        public override void BusInit(IBusManager bmgr)
        {
            m_memory = m_noDos ? bmgr.FindDevice<IMemoryDevice>() : null;
            bmgr.Events.SubscribeRdIo(MaskB, PortB & MaskB, ReadPortBtn);
            bmgr.Events.SubscribeRdIo(MaskX, PortX & MaskX, ReadPortX);
            bmgr.Events.SubscribeRdIo(MaskY, PortY & MaskY, ReadPortY);
        }

        public override void BusConnect()
        {
        }

        public override void BusDisconnect()
        {
        }

        #endregion


        #region IMouseDevice Members

		public IMouseState MouseState { get; set; }
		
        #endregion


        #region Private

        private void ReadPortBtn(ushort addr, ref byte value, ref bool handled)
        {
            if (handled || (m_memory != null && m_memory.DOSEN))
                return;
            handled = true;
			
            var b = MouseState != null ? MouseState.Buttons : 0;
			b = ((b & 1) << 1) | ((b & 2) >> 1) | (b & 0xFC);			// D0 - right, D1 - left, D2 - middle
			value = (byte)(b ^ 0xFF);     //  Kempston mouse buttons
        }

        private void ReadPortX(ushort addr, ref byte value, ref bool handled)
        {
            if (handled || (m_memory != null && m_memory.DOSEN))
                return;
            handled = true;
			
            var x = MouseState != null ? MouseState.X : 0;
            value = (byte)(x / 3);			//  Kempston mouse X        
        }

        private void ReadPortY(ushort addr, ref byte value, ref bool handled)
        {
            if (handled || (m_memory != null && m_memory.DOSEN))
                return;
            handled = true;
			
            var y = MouseState != null ? MouseState.Y : 0;
			value = (byte)(-y / 3);			//	Kempston mouse Y
        }

        #endregion
    }
}
