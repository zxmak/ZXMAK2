using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Hardware.Clone
{
    public class UlaLeningrad : UlaDeviceBase
    {
        public UlaLeningrad()
        {
            Name = "Leningrad";
        }
        

        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            base.BusInit(bmgr);
            bmgr.Events.SubscribeRdMemM1(0x0000, 0x0000, busM1);
        }

        #endregion

        #region Bus Handlers

        private void busM1(ushort addr, ref byte value)
        {
            CPU.Tact += CPU.Tact & 1;
        }

        #endregion

        protected override SpectrumRendererParams CreateSpectrumRendererParams()
        {
            // Leningrad 1
            // Total Size:          448 x 320
            // Visible Size:        384 x 304 (72+256+56 x 64+192+48)

            // 224 = 128T scr + 32T right + 32T HSync + 32T left
            // 312 = 192 scr + 40 bottom + 16 VSync + 64 top border
            var timing = SpectrumRenderer.CreateParams();
            timing.c_frameTactCount = 69888;   // +
            timing.c_ulaLineTime = 224;        // +
            timing.c_ulaFirstPaperLine = 64;   // +
            timing.c_ulaFirstPaperTact = 61;//64;

            timing.c_ulaBorderTop = 32;// 48;
            timing.c_ulaBorderBottom = 32;// 48;
            timing.c_ulaBorderLeftT = 16;// 32;      // +
            timing.c_ulaBorderRightT = 16;// 32;     // +

            timing.c_ulaIntBegin = 64 + (64 + 192) * 224;
            timing.c_ulaIntLength = 32;

            timing.c_ulaWidth = (timing.c_ulaBorderLeftT + 128 + timing.c_ulaBorderRightT) * 2;
            timing.c_ulaHeight = (timing.c_ulaBorderTop + 192 + timing.c_ulaBorderBottom);
            return timing;
        }
    }
}
