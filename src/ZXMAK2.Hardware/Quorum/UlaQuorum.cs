using System;

namespace ZXMAK2.Hardware.Quorum
{
    public class UlaQuorum : UlaDeviceBase
    {
        public UlaQuorum()
        {
            Name = "QUORUM";
        }

        
        protected override SpectrumRendererParams CreateSpectrumRendererParams()
        {
            // Кворум БК-04
            // Total Size:          448 x 312
            // Visible Size:        384 x 296 (72+256+56 x 64+192+40)
            var timing = SpectrumRenderer.CreateParams();
            timing.c_ulaLineTime = 224;
            timing.c_ulaFirstPaperLine = 80;      // proof???80
            timing.c_ulaFirstPaperTact = 65;// 68;      // proof???68 [32sync+36border+128scr+28border]
            timing.c_frameTactCount = 69888;      // for pentagon mod = 71680

            timing.c_ulaBorderTop = 32;//64;
            timing.c_ulaBorderBottom = 32;//40;
            timing.c_ulaBorderLeftT = 16;
            timing.c_ulaBorderRightT = 16;

            timing.c_ulaIntBegin = 0;
            timing.c_ulaIntLength = 32;

            timing.c_ulaWidth = (timing.c_ulaBorderLeftT + 128 + timing.c_ulaBorderRightT) * 2;
            timing.c_ulaHeight = (timing.c_ulaBorderTop + 192 + timing.c_ulaBorderBottom);
            return timing;
        }

        protected override void WritePortFE(ushort addr, byte value, ref bool handled)
        {
            if ((addr & 0x99) == (0xFE & 0x99))
                base.WritePortFE(addr, value, ref handled);
        }
    }
}
