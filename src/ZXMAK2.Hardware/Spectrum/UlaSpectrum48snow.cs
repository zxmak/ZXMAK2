using System;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Hardware.Spectrum
{
    public class UlaSpectrum48snow : UlaSpectrum48
    {
        public UlaSpectrum48snow()
        {
            Name = "ZX Spectrum 48 [snow]";

            SnowRenderer = new SpectrumSnowRenderer();
            SnowRenderer.Params = CreateSpectrumRendererParams();
            SnowRenderer.Palette = SpectrumSnowRenderer.CreatePalette();
            Renderer = SnowRenderer;
        }


        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            base.BusInit(bmgr);
            bmgr.Events.SubscribeRdMemM1(0x0000, 0x0000, ReadMemM1);
        }

        #endregion

        protected override IMemoryDevice Memory
        {
            set
            {
                base.Memory = value;
                SnowRenderer.MemoryPage = Memory.RamPages[m_videoPage];
            }
        }

        protected SpectrumSnowRenderer SnowRenderer;

        protected unsafe void ReadMemM1(ushort addr, ref byte value)
        {
            if ((CPU.regs.IR & 0xC000) == 0x4000)
            {
                int frameTactT3 = (int)((CPU.Tact + 3) % FrameTactCount);
                if (SnowRenderer.IsUlaFetch(frameTactT3))
                {
                    UpdateState(frameTactT3 - 1);
                    SnowRenderer.Snow = 2;
                }
            }
        }
    }
}
