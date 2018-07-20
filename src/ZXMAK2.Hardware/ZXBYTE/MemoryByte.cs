using System;
using System.Xml;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Attributes;
using ZXMAK2.Hardware.Spectrum;
using ZXMAK2.Mvvm;


namespace ZXMAK2.Hardware.ZXBYTE
{
    public class MemoryByte : MemorySpectrum128
    {
        #region Fields

        private readonly byte[] m_dd66 = new byte[512];
        private readonly byte[] m_dd71 = new byte[2048];
        private int m_sovmest = 1; // COBMECT="OFF"
        private int m_rd1f = 0;

        private ICommand CommandSwitcher { get; set; }

        #endregion


        public MemoryByte()
            : base("ZXBYTE")
        {
            Name = "BYTE 48K";
            Description = "Memory Module \"Byte\" 48K\r\nVersion 1.2";
        }


        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            bmgr.Events.SubscribeRdIo(0x75, 0x1F & 0x75, BusReadPort1F);
            CommandSwitcher = new CommandDelegate(
                CommandSwitcher_OnExecute,
                () => true,
                "BYTE \"COBMECT.\"");
            CommandSwitcher.Checked = COBMECT;
            bmgr.AddCommandUi(CommandSwitcher);

            // Subscribe before MemoryBase.BusInit 
            // to handle memory switches before read
            base.BusInit(bmgr);
        }

        protected override void OnConfigLoad(XmlNode itemNode)
        {
            base.OnConfigLoad(itemNode);
            COBMECT = Utils.GetXmlAttributeAsBool(itemNode, "enableCobmect", COBMECT);
        }

        protected override void OnConfigSave(XmlNode itemNode)
        {
            base.OnConfigSave(itemNode);
            Utils.SetXmlAttribute(itemNode, "enableCobmect", COBMECT);
        }

        #endregion


        #region MemoryBase

        public override bool IsMap48 { get { return true; } }

        public override byte CMR0
        {
            get { return 0x30; }
            set { UpdateMapping(); }
        }

        public override byte CMR1
        {
            get { return 0x00; }
            set { UpdateMapping(); }
        }

        protected override void BusReset()
        {
            base.BusReset();
            m_rd1f = 0;
        }

        protected override void OnLoadRomPage(string pageName, byte[] data)
        {
            if (pageName == "DD66")
            {
                Array.Copy(data, m_dd66, data.Length);
            }
            else if (pageName == "DD71")
            {
                Array.Copy(data, m_dd71, data.Length);
            }
            else
            {
                base.OnLoadRomPage(pageName, data);
            }
        }

        protected override void ReadMem0000(ushort addr, ref byte value)
        {
            if (m_rd1f != 0 &&
                !DOSEN)
            {
                var adr66 = ((addr >> 7) & 0xFF) | ((m_sovmest << 8) & 0x100);
                var dat66 = m_dd66[adr66];
                if ((dat66 & 0x10) == 0)
                {
                    var adr71 = ((dat66 & 0x0F) << 7) | (addr & 0x7F);
                    value = m_dd71[adr71];
                    return;
                }
            }
            base.ReadMem0000(addr, ref value);
        }

        public override byte RDMEM_DBG(ushort addr)
        {
            if (addr < 0x4000 &&
                m_rd1f != 0 &&
                !DOSEN)
            {
                var adr66 = ((addr >> 7) & 0xFF) | ((m_sovmest << 8) & 0x100);
                var dat66 = m_dd66[adr66];
                if ((dat66 & 0x10) == 0)
                {
                    var adr71 = ((dat66 & 0x0F) << 7) | (addr & 0x7F);
                    return m_dd71[adr71];
                }
            }
            return base.RDMEM_DBG(addr);
        }

        #endregion

        
        #region Bus Handlers

        private void BusReadPort1F(ushort addr, ref byte value, ref bool handled)
        {
            if (!DOSEN)
            {
                m_rd1f = 1;
            }
        }

        #endregion


        [HardwareSwitch("COBMECT", Description = "Enable Spectrum compatible mode")]
        public bool COBMECT
        {
            get { return m_sovmest == 0; }
            set
            {
                var iValue = value ? 0 : 1;
                if (m_sovmest != iValue)
                {
                    m_sovmest = iValue;
                    if (CommandSwitcher != null)
                    {
                        CommandSwitcher.Checked = COBMECT;
                    }
                    OnConfigChanged();
                }
            }
        }

        
        #region IGuiExtension Members

        private void CommandSwitcher_OnExecute()
        {
            COBMECT = !COBMECT;
            CommandSwitcher.Checked = COBMECT;
        }

        #endregion IGuiExtension Members
    }
}
