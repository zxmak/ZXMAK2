using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;
using ZXMAK2.Host.Tools;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using System.Text;
using System;
using System.Xml;


namespace ZXMAK2.Hardware.General
{
    public class KeyboardDevice : BusDeviceBase, IKeyboardDevice
    {
        #region Fields

        private readonly KeyboardMatrix _matrix;
        private int[] _rows;
        private IKeyboardState m_keyboardState = null;

        private IMemoryDevice m_memory;
        private bool m_noDos;
        private int m_mask;
        private int m_port;

        #endregion Fields


        public KeyboardDevice()
        {
            Category = BusDeviceCategory.Keyboard;
            Name = "KEYBOARD";

            m_noDos = true;
            m_mask = 0x01;
            m_port = 0xFE;
            _matrix = KeyboardMatrix.Deserialize(
                KeyboardMatrix.DefaultRows,
                Path.Combine(Utils.GetAppFolder(), "Keyboard.config"));
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
                m_mask = value & 0x00FF; // high addr byte is required
                OnConfigChanged();
            }
        }

        public int Port
        {
            get { return m_port; }
            set
            {
                m_port = value & 0x00FF;
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
            var builder = new StringBuilder();
            builder.Append("Common Spectrum Keyboard");
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("NoDos: {0}", NoDos));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("Mask:  #{0:X2}", Mask));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("Port:  #{0:X2}", Port));
            builder.Append(Environment.NewLine);
            Description = builder.ToString();
        }

        #endregion Properties


        
        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            m_memory = m_noDos ? bmgr.FindDevice<IMemoryDevice>() : null;
            bmgr.Events.SubscribeRdIo(Mask, Port & Mask, ReadPortFe);
        }

        public override void BusConnect()
        {
        }

        public override void BusDisconnect()
        {
        }

        #endregion IBusDevice

        
        #region IKeyboardDevice

        public IKeyboardState KeyboardState
        {
            get { return m_keyboardState; }
			set 
            { 
                m_keyboardState = value;
                _rows = _matrix.Scan(value);//new MockState(Key.A, Key.B));
            }
        }

        #endregion

		
        #region Private

        private void ReadPortFe(ushort addr, ref byte value, ref bool handled)
		{
            if (handled || (m_memory != null && m_memory.DOSEN))
				return;
            //handled = true;
			value &= 0xE0;
			value |= (byte)(~KeyboardMatrix.ScanPort(_rows, addr) & 0x1F);
		}
		
		#endregion Private
	}
}
