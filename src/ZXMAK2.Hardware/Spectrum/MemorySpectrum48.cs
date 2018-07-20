using System;

namespace ZXMAK2.Hardware.Spectrum
{
    public class MemorySpectrum48 : MemorySpectrum128
    {
        public MemorySpectrum48()
            : base("ZX48")
        {
            Name = "ZX Spectrum 48";    
        }

        
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

        #endregion
    }
}
