/// Description: AY8910 emulator
/// Author: Alex Makeev
/// Date: 13.04.2007
using System;
using System.Xml;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Hardware.Circuits.Sound;
using ZXMAK2.Dependency;
using System.Text;


namespace ZXMAK2.Hardware.General
{
    public class AY8910 : SoundDeviceBase, IPsgDevice
    {
        #region Fields

        private readonly IPsgChip m_chip;
        private readonly PsgPortState m_iraState = new PsgPortState(0xFF);
        private readonly PsgPortState m_irbState = new PsgPortState(0xFF);

        private IMemoryDevice m_memory;
        private bool m_noDos;
        private int m_maskAddr;
        private int m_portAddr;
        private int m_maskData;
        private int m_portData;
        
        private double m_lastTime;

        #endregion Fields


        public AY8910()
        {
            m_chip = Locator.Resolve<IPsgChip>();
            m_chip.UpdateHandler = UpdateDac;

            Category = BusDeviceCategory.Music;
            Name = "AY8910";

            m_noDos = false;
            m_maskAddr = 0xC0FF;       // for compatibility (Quorum for example)
            m_maskData = 0xC0FF;       // for compatibility (Quorum for example)
            m_portAddr = 0xFFFD;
            m_portData = 0xBFFD;
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
            m_chip.ChipFrequency = Utils.GetXmlAttributeAsInt32(node, "frequency", m_chip.ChipFrequency);
            m_chip.AmpType = Utils.GetXmlAttributeAsEnum<AmpType>(node, "ampType", m_chip.AmpType);
            m_chip.PanType = Utils.GetXmlAttributeAsEnum<PanType>(node, "panType", m_chip.PanType);
            NoDos = Utils.GetXmlAttributeAsBool(node, "noDos", NoDos);
            MaskAddr = Utils.GetXmlAttributeAsInt32(node, "maskAddr", MaskAddr);
            MaskData = Utils.GetXmlAttributeAsInt32(node, "maskData", MaskData);
            PortAddr = Utils.GetXmlAttributeAsInt32(node, "portAddr", PortAddr);
            PortData = Utils.GetXmlAttributeAsInt32(node, "portData", PortData);
        }

        protected override void OnConfigSave(XmlNode node)
        {
            base.OnConfigSave(node);
            Utils.SetXmlAttribute(node, "frequency", m_chip.ChipFrequency);
            Utils.SetXmlAttributeAsEnum(node, "ampType", m_chip.AmpType);
            Utils.SetXmlAttributeAsEnum(node, "panType", m_chip.PanType);
            Utils.SetXmlAttribute(node, "noDos", NoDos);
            Utils.SetXmlAttribute(node, "maskAddr", MaskAddr);
            Utils.SetXmlAttribute(node, "maskData", MaskData);
            Utils.SetXmlAttribute(node, "portAddr", PortAddr);
            Utils.SetXmlAttribute(node, "portData", PortData);
        }

        protected override void OnProcessConfigChange()
        {
            base.OnProcessConfigChange();
            if (m_chip != null && m_chip.Volume != Volume)
            {
                m_chip.Volume = Volume;
            }

            // update description...
            var builder = new StringBuilder();
            builder.Append("AY8910 Programmable Sound Generator");
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

        #endregion Properties


        #region Public

        public byte RegAddr
        {
            get { return m_chip.RegAddr; }
            set { m_chip.RegAddr = value; }
        }

        public byte GetReg(int index)
        {
            return m_chip.GetReg(index);
        }

        public void SetReg(int index, byte value)
        {
            m_chip.SetReg(index, value);
            index &= 0x0F;
            if (index == PsgRegId.IRA)
            {
                OnWriteIra(value);
            }
            else if (index == PsgRegId.IRB)
            {
                OnWriteIrb(value);
            }
        }

        public event Action<IPsgDevice, PsgPortState> IraHandler;
        public event Action<IPsgDevice, PsgPortState> IrbHandler;

        #endregion Public


        #region SoundDeviceBase

        public override void BusInit(IBusManager bmgr)
        {
            base.BusInit(bmgr);
            m_lastTime = 0D;
            m_memory = m_noDos ? bmgr.FindDevice<IMemoryDevice>() : null;
            bmgr.Events.SubscribeWrIo(MaskAddr, PortAddr & MaskAddr, WritePortAddr);   // #FFFD (reg#)
            bmgr.Events.SubscribeRdIo(MaskAddr, PortAddr & MaskAddr, ReadPortData);    // #FFFD (rd data/reg#)
            bmgr.Events.SubscribeWrIo(MaskData, PortData & MaskData, WritePortData);   // #BFFD (data)
            bmgr.Events.SubscribeReset(Bus_OnReset);
        }

        protected override void OnEndFrame()
        {
            m_lastTime = m_chip.Update(m_lastTime, 1D);
            if (m_lastTime >= 1D)
            {
                m_lastTime -= Math.Floor(m_lastTime);
            }
            else
            {
                Logger.Warn("EndFrame: m_lastTime={0:F9}", m_lastTime);
            }
            base.OnEndFrame();
        }

        private void WritePortAddr(ushort addr, byte value, ref bool handled)
        {
            if (m_memory != null && m_memory.DOSEN)
                return;
            
            //if (handled)
            //    return;
            //handled = true;
            m_chip.RegAddr = value;
        }

        private void WritePortData(ushort addr, byte value, ref bool handled)
        {
            if (m_memory != null && m_memory.DOSEN)
                return;
            //if (handled)
            //    return;
            //handled = true;
            var index = m_chip.RegAddr;
            m_lastTime = m_chip.SetReg(m_lastTime, GetFrameTime(), index, value);
            index &= 0x0F;
            if (index == PsgRegId.IRA)
            {
                OnWriteIra(value);
            }
            else if (index == PsgRegId.IRB)
            {
                OnWriteIrb(value);
            }
        }

        private void ReadPortData(ushort addr, ref byte value, ref bool handled)
        {
            if (handled || (m_memory != null && m_memory.DOSEN))
                return;
            handled = true;

            var index = m_chip.RegAddr;
            var indexF = index & 0x0F;
            if (indexF == PsgRegId.IRA)
            {
                value = OnReadIra();
                return;
            }
            else if (indexF == PsgRegId.IRB)
            {
                value = OnReadIrb();
                return;
            }
            value = m_chip.GetReg(m_chip.RegAddr);
        }

        private void Bus_OnReset()
        {
            m_chip.Reset();
        }

        private byte OnReadIra()
        {
            m_iraState.DirOut = (m_chip.GetReg(PsgRegId.MIXER_CONTROL) & 0x40) != 0;
            m_iraState.InState = m_iraState.DirOut ? m_iraState.OutState : (byte)0;
            var iraHandler = IraHandler;
            if (iraHandler != null)
            {
                iraHandler(this, m_iraState);
            }
            return m_iraState.InState;
        }

        private byte OnReadIrb()
        {
            m_irbState.DirOut = (m_chip.GetReg(PsgRegId.MIXER_CONTROL) & 0x80) != 0;
            m_irbState.InState = m_irbState.DirOut ? m_irbState.OutState : (byte)0;
            var irbHandler = IrbHandler;
            if (irbHandler != null)
            {
                irbHandler(this, m_irbState);
            }
            return m_irbState.InState;
        }

        private void OnWriteIra(byte value)
        {
            m_iraState.DirOut = (m_chip.GetReg(PsgRegId.MIXER_CONTROL) & 0x40) != 0;
            m_iraState.OutState = value;
            var iraHandler = IraHandler;
            if (iraHandler != null)
            {
                m_iraState.InState = m_iraState.DirOut ? m_iraState.OutState : (byte)0;
                iraHandler(this, m_iraState);
            }
        }

        private void OnWriteIrb(byte value)
        {
            m_irbState.DirOut = (m_chip.GetReg(PsgRegId.MIXER_CONTROL) & 0x80) != 0;
            m_irbState.OutState = value;
            var irbHandler = IrbHandler;
            if (irbHandler != null)
            {
                m_irbState.InState = m_irbState.DirOut ? m_irbState.OutState : (byte)0;
                irbHandler(this, m_irbState);
            }
        }

        #endregion SoundDeviceBase
    }
}