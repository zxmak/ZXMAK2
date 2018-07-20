using ZXMAK2.Hardware.Atm;


namespace ZXMAK2.Hardware.Evo
{
    public class UlaPentEvo : UlaAtm450
    {
        public UlaPentEvo()
        {
            Name = "PENTEVO";
        }
        

        protected override SpectrumRendererParams CreateSpectrumRendererParams()
        {
            // Pentagon 128K
            // Total Size:          448 x 320
            // Visible Size:        384 x 304 (72+256+56 x 64+192+48)
            var timing = SpectrumRenderer.CreateParams();
            timing.c_frameTactCount = 71680;
            timing.c_ulaLineTime = 224;
            timing.c_ulaFirstPaperLine = 80;
            timing.c_ulaFirstPaperTact = 65;// 68;      // 68 [32sync+36border+128scr+28border]
            timing.c_ulaBorder4T = false;

            timing.c_ulaBorderTop = 32;
            timing.c_ulaBorderBottom = 32;
            timing.c_ulaBorderLeftT = 16;
            timing.c_ulaBorderRightT = 16;

            timing.c_ulaIntBegin = 0;
            timing.c_ulaIntLength = 32;
            timing.c_ulaFlashPeriod = 25;   // TODO: check?

            timing.c_ulaWidth = (timing.c_ulaBorderLeftT + 128 + timing.c_ulaBorderRightT) * 2;
            timing.c_ulaHeight = (timing.c_ulaBorderTop + 192 + timing.c_ulaBorderBottom);
            return timing;
        }
    }
}
