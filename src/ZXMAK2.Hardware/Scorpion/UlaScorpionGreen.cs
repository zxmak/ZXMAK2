

namespace ZXMAK2.Hardware.Scorpion
{
    public class UlaScorpionGreen : UlaDeviceBase
    {
        public UlaScorpionGreen()
        {
            Name = "Scorpion [Green]";
        }
        

        protected override SpectrumRendererParams CreateSpectrumRendererParams()
        {
            // Scorpion [Green PCB]
            // Total Size:          448 x 316
            // Visible Size:        320 x 240 (32+256+32 x 24+192+24)
            var timing = SpectrumRenderer.CreateParams();
            timing.c_ulaLineTime = 224;
            timing.c_ulaFirstPaperLine = 64;
            timing.c_ulaFirstPaperTact = 61;//64;      // ?64 [40sync+24border+128scr+32border]
            timing.c_frameTactCount = 70784;
            timing.c_ulaBorder4T = true;
            timing.c_ulaBorder4Tstage = 3;

            timing.c_ulaBorderTop = 32;
            timing.c_ulaBorderBottom = 32;
            timing.c_ulaBorderLeftT = 16;
            timing.c_ulaBorderRightT = 16;

            timing.c_ulaIntBegin = 61;
            timing.c_ulaIntLength = 32;    // according to fuse

            timing.c_ulaWidth = (timing.c_ulaBorderLeftT + 128 + timing.c_ulaBorderRightT) * 2;
            timing.c_ulaHeight = (timing.c_ulaBorderTop + 192 + timing.c_ulaBorderBottom);
            return timing;
        }

        protected override void WritePortFE(ushort addr, byte value, ref bool handled)
        {
            if (!Memory.DOSEN && (addr & 0x23) == (0xFE & 0x23))
                base.WritePortFE(addr, value, ref handled);
        }
    }
}
