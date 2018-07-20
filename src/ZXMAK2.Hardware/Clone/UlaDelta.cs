using System;
using ZXMAK2.Hardware.Spectrum;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Hardware.Clone
{
    public class UlaDelta : UlaDeviceBase
    {
        public UlaDelta()
        {
            Name = "Delta-C [Cheboksary-91/74]";
        }
        

        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            base.BusInit(bmgr);
            bmgr.Events.SubscribeRdMem(0xC000, 0x4000, ReadMem4000);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0x4000, ReadMem4000);
            bmgr.Events.SubscribeRdNoMreq(0xC000, 0x4000, NoMreq4000);
            bmgr.Events.SubscribeWrNoMreq(0xC000, 0x4000, NoMreq4000);
        }

        #endregion

        #region Bus Handlers

        protected override void WriteMem4000(ushort addr, byte value)
        {
            int frameTact = (int)(CPU.Tact % FrameTactCount);
            CPU.Tact += m_contention[frameTact];
            base.WriteMem4000(addr, value);
        }

        protected void ReadMem4000(ushort addr, ref byte value)
        {
            int frameTact = (int)(CPU.Tact % FrameTactCount);
            CPU.Tact += m_contention[frameTact];
        }

        protected void NoMreq4000(ushort addr)
        {
            int frameTact = (int)(CPU.Tact % FrameTactCount);
            CPU.Tact += m_contention[frameTact];
        }

        #endregion

        protected override SpectrumRendererParams CreateSpectrumRendererParams()
        {
            // Delta-C
            // Total Size:          448 x 320
            // Visible Size:        384 x 304 (72+256+56 x 64+192+48)
            var timing = SpectrumRenderer.CreateParams();
            timing.c_ulaLineTime = 224;
            timing.c_ulaFirstPaperLine = 68;
            timing.c_ulaFirstPaperTact = 65;// 68;
            timing.c_frameTactCount = 69216;//69888;

            timing.c_ulaBorderTop = 32;
            timing.c_ulaBorderBottom = 32;
            timing.c_ulaBorderLeftT = 16;
            timing.c_ulaBorderRightT = 16;

            timing.c_ulaIntBegin = 0;
            timing.c_ulaIntLength = 836;//224;
            timing.c_ulaFlashPeriod = 8;

            timing.c_ulaWidth = (timing.c_ulaBorderLeftT + 128 + timing.c_ulaBorderRightT) * 2;
            timing.c_ulaHeight = (timing.c_ulaBorderTop + 192 + timing.c_ulaBorderBottom);
            return timing;
        }

        protected override void OnTimingChanged()
        {
            base.OnTimingChanged();
            m_contention = UlaSpectrum48.CreateContentionTable(
                SpectrumRenderer.Params,
                new int[] { 6, 5, 4, 3, 2, 1, 0, 0, });
        }

        private int[] m_contention;
    }
}
