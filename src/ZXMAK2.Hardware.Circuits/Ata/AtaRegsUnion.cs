using System;


namespace ZXMAK2.Hardware.Circuits.Ata
{
    public class AtaRegsUnion
    {
        public byte[] __regs = new byte[12];

        public byte data { get { return __regs[0]; } set { __regs[0] = value; } }
        // for write, features
        public HD_ERROR err { get { return (HD_ERROR)__regs[1]; } set { __regs[1] = (byte)value; } }
        public byte count { get { return __regs[2]; } set { __regs[2] = value; } }
        public ATAPI_INT_REASON intreason { get { return (ATAPI_INT_REASON)__regs[2]; } set { __regs[2] = (byte)value; } }
        public byte sec { get { return __regs[3]; } set { __regs[3] = value; } }
        public UInt16 cyl
        {
            get { return (UInt16)(__regs[4] | (__regs[5] << 8)); }
            set { __regs[4] = (byte)value; __regs[5] = (byte)(value >> 8); }
        }
        public UInt16 atapi_count
        {
            get { return (UInt16)(__regs[4] | (__regs[5] << 8)); }
            set { __regs[4] = (byte)value; __regs[5] = (byte)(value >> 8); }
        }
        public byte cyl_l { get { return __regs[4]; } set { __regs[4] = value; } }
        public byte cyl_h { get { return __regs[5]; } set { __regs[5] = value; } }
        public byte devhead { get { return __regs[6]; } set { __regs[6] = value; } }
        // for write, cmd
        public HD_STATUS status { get { return (HD_STATUS)__regs[7]; } set { __regs[7] = (byte)value; } }
        /*                  */
        // reg8 - control (CS1,DA=6)
        public HD_CONTROL control { get { return (HD_CONTROL)__regs[8]; } set { __regs[8] = (byte)value; } }
        public byte feat { get { return __regs[9]; } set { __regs[9] = value; } }
        public byte cmd { get { return __regs[10]; } set { __regs[10] = value; } }
        // reserved
        public byte reserved { get { return __regs[11]; } set { __regs[11] = value; } }

        public AtaRegsUnion()
        {
            __regs[(int)AtaReg.FeatureError] = 0x00;
            __regs[(int)AtaReg.SectorCount] = 0x01;
            __regs[(int)AtaReg.SectorNumber] = 0x01;
            __regs[(int)AtaReg.CylinderLow] = 0x00;
            __regs[(int)AtaReg.CylinderHigh] = 0x00;
            __regs[(int)AtaReg.HeadAndDrive] = 0xA0;
            __regs[(int)AtaReg.CommandStatus] = 0x00;
            __regs[(int)AtaReg.ControlAltStatus] = 0x00;
        }
    }
}
