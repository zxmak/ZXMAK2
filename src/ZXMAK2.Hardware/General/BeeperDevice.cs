using System;
using System.Linq;
using System.Xml;
using System.Text;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Engine;

namespace ZXMAK2.Hardware.General
{
    public class BeeperDevice : SoundDeviceBase
    {
        #region Fields

        private readonly ushort[] m_dac = new ushort[4];
        private IMemoryDevice m_memory;
        private int m_portState;

        private int m_bitEarMask;
        private int m_bitMicMask;
        private int m_shiftEar;
        private int m_shiftMic;
        private int m_fixMic;

        private bool m_noDos;
        private int m_mask;
        private int m_port;
        private int m_bitEar;
        private int m_bitMic;

        #endregion Fields


        public BeeperDevice()
        {
            Category = BusDeviceCategory.Sound;
            Name = "BEEPER";
            Description = "Standard Beeper";

            m_noDos = true;
            m_mask = 0x01;
            m_port = 0xFE;
            m_bitMic = -1;
            m_bitEar = 4;
            //OnProcessConfigChange();

            Volume = 40;    // default 100% is too loud and jamming AY
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

        public int BitEar
        {
            get { return m_bitEar; }
            set
            {
                m_bitEar = value;
                m_bitEarMask = (value >= 0 && value <= 7) ? 1 << value : 0;
                m_shiftEar = (value >= 0 && value <= 7) ? 9 - value : 0;
                OnConfigChanged();
            }
        }

        public int BitMic
        {
            get { return m_bitMic; }
            set
            {
                m_bitMic = value;
                m_bitMicMask = (value >= 0 && value <= 7) ? 1 << value : 0;
                m_shiftMic = (value >= 0 && value <= 7) ? 8 - value : 0;
                m_fixMic = (value >= 0 && value <= 7) ? 0 : 1;
                OnConfigChanged();
            }
        }

        protected override void OnConfigLoad(XmlNode node)
        {
            base.OnConfigLoad(node);
            NoDos = Utils.GetXmlAttributeAsBool(node, "noDos", NoDos);
            Mask = Utils.GetXmlAttributeAsInt32(node, "mask", Mask);
            Port = Utils.GetXmlAttributeAsInt32(node, "port", Port);
            BitEar = Utils.GetXmlAttributeAsInt32(node, "bitEar", BitEar);
            BitMic = Utils.GetXmlAttributeAsInt32(node, "bitMic", BitMic);
        }

        protected override void OnConfigSave(XmlNode node)
        {
            base.OnConfigSave(node);
            Utils.SetXmlAttribute(node, "noDos", NoDos);
            Utils.SetXmlAttribute(node, "mask", Mask);
            Utils.SetXmlAttribute(node, "port", Port);
            Utils.SetXmlAttribute(node, "bitEar", BitEar);
            Utils.SetXmlAttribute(node, "bitMic", BitMic);
        }

        protected override void OnProcessConfigChange()
        {
            base.OnProcessConfigChange();
            
            // process Volume change...
            // http://www.worldofspectrum.org/faq/reference/48kreference.htm
            // issue 2: 0.39D, 0.73D, 3.66D, 3.79D
            // issue 3: 0.34D, 0.66D, 3.56D, 3.70D
            //
            // -0.5D/+0.1D => temp fix to correct nonlinearity of soundcard ADC
            var amps = new[] { 0.34D, 0.66D + 0.2D, 3.56D - 0.5D, 3.70D };
            var ampMin = amps.Min();
            var norm = amps.Select(amp => amp - ampMin);
            var ampMax = norm.Max();
            norm = norm.Select(amp => amp / ampMax);
            var normAmps = norm.ToArray();
            for (var i = 0; i < m_dac.Length; i++)
            {
                m_dac[i] = ScaleDacValue(0xFFFF, (normAmps[i] * Volume) / 100D);
            }

            // update description...
            var builder = new StringBuilder();
            builder.Append("Common Beeper Device");
            builder.Append(Environment.NewLine);
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("NoDos: {0}", NoDos));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("Mask:  #{0:X4}", Mask));
            builder.Append(Environment.NewLine);
            builder.Append(string.Format("Port:  #{0:X4}", Port));
            builder.Append(Environment.NewLine);
            if (BitEar >= 0)
                builder.Append(string.Format("Ear:   D{0}", BitEar));
            else
                builder.Append(string.Format("Ear:   disabled"));
            builder.Append(Environment.NewLine);
            if (BitMic >= 0)
                builder.Append(string.Format("Mic:   D{0}", BitMic));
            else
                builder.Append(string.Format("Mic:   disabled"));
            builder.Append(Environment.NewLine);
            Description = builder.ToString();
        }

        #endregion Properties


        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            base.BusInit(bmgr);
            m_memory = m_noDos ? bmgr.FindDevice<IMemoryDevice>() : null;
            bmgr.Events.SubscribeWrIo(Mask, Port & Mask, WritePortFE);
        }

        #endregion

        #region Bus Handlers

        protected virtual void WritePortFE(ushort addr, byte value, ref bool handled)
        {
            //if (handled)
            //	return;
            if (m_memory != null && m_memory.DOSEN)
            {
                return;
            }
            var maskedValue = value & (m_bitEarMask | m_bitMicMask);
            if (maskedValue == m_portState)
            {
                return;
            }
            m_portState = maskedValue;
            var ear = (maskedValue & m_bitEarMask) << m_shiftEar;
            var mic = (maskedValue & m_bitMicMask) << m_shiftMic;
            var v = m_dac[(ear | mic | (ear >> m_fixMic)) >> 8];
            UpdateDac(v, v);
        }

        #endregion

        private static ushort ScaleDacValue(ushort value, double coef)
        {
            coef = double.IsNaN(coef) ? 1D : coef;
            coef = coef > 1D ? 1D : coef;
            coef = coef < 0D ? 0D : coef;
            return (ushort)Math.Floor((double)value * coef);
        }
    }
}
