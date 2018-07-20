using System;
using System.Xml;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using System.Text;


namespace ZXMAK2.Hardware.General
{
    public class KempstonJoystick : BusDeviceBase, IJoystickDevice
    {
        #region Fields

        private IMemoryDevice m_memory;
        private string m_hostId = string.Empty;

        private bool m_noDos;
        private int m_mask;
        private int m_port;

        #endregion Fields


        public KempstonJoystick()
        {
            Category = BusDeviceCategory.Other;
            Name = "JOYSTICK KEMPSTON";
            Description = "Kempston Joystick";

            m_noDos = true;
            m_mask = 0xE0;    // zx128 by default
            m_port = 0x1F;
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
            HostId = Utils.GetXmlAttributeAsString(node, "hostId", HostId);
        }

        protected override void OnConfigSave(XmlNode node)
        {
            base.OnConfigSave(node);
            Utils.SetXmlAttribute(node, "noDos", NoDos);
            Utils.SetXmlAttribute(node, "mask", Mask);
            Utils.SetXmlAttribute(node, "port", Port);
            Utils.SetXmlAttribute(node, "hostId", HostId);
        }

        protected override void OnProcessConfigChange()
        {
            base.OnProcessConfigChange();
            var builder = new StringBuilder();
            builder.Append("Kempston Joystick");
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("NoDos: {0}", NoDos));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("Mask:  #{0:X4}", Mask));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("Port:  #{0:X4}", Port));
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            // thanks to weiv for info
            if (Mask == 0x20 && Port == 0x1f)
                builder.Append("Classic ZX48 config");
            else if (Mask == 0xe0 && Port == 0x1f)
                builder.Append("Classic ZX128 config");
            else
                builder.Append("Custom config (not compatible with ZX48 & ZX128)");
            builder.Append(Environment.NewLine);
            Description = builder.ToString();
        }

        #endregion Properties


        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            m_memory = m_noDos ? bmgr.FindDevice<IMemoryDevice>() : null;
            bmgr.Events.SubscribeRdIo(Mask, Port & Mask, ReadPort1F);
        }

        public override void BusConnect()
        {
        }

        public override void BusDisconnect()
        {
        }

        #endregion IBusDevice


        public IJoystickState JoystickState { get; set; }
        
        public string HostId 
        {
            get { return m_hostId; }
            set { m_hostId = value; OnConfigChanged(); }
        }


        protected virtual void ReadPort1F(ushort addr, ref byte value, ref bool handled)
        {
            if (handled || (m_memory != null && m_memory.DOSEN)) // nodos?
                return;
            handled = true;

            value = 0x00;
            if (JoystickState.IsRight) value |= 0x01;
            if (JoystickState.IsLeft) value |= 0x02;
            if (JoystickState.IsDown) value |= 0x04;
            if (JoystickState.IsUp) value |= 0x08;
            if (JoystickState.IsFire) value |= 0x10;
        }
    }
}
