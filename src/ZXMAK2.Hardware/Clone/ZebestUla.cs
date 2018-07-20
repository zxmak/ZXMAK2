using ZXMAK2.Hardware.Pentagon;


namespace ZXMAK2.Hardware.Clone
{
    public class ZebestUla : UlaPentagon
    {
        public ZebestUla()
        {
            Name = "Zebest ULA (Pentagon+BRIGHT)";
            Description = "ULA device based on Pentagon + border bright mod\n\rbit 6 of port #FE = bright bit for border";
        }
        
        protected override SpectrumRendererParams CreateSpectrumRendererParams()
        {
            var timing = base.CreateSpectrumRendererParams();
            timing.c_ulaBorderTop = 64;
            timing.c_ulaBorderBottom = 48;
            timing.c_ulaBorderLeftT = 28;
            timing.c_ulaBorderRightT = 28;

            timing.c_ulaWidth = (timing.c_ulaBorderLeftT + 128 + timing.c_ulaBorderRightT) * 2;
            timing.c_ulaHeight = (timing.c_ulaBorderTop + 192 + timing.c_ulaBorderBottom);
            return timing;
        }

        public override byte PortFE
        {
            set
            {
                base.PortFE = value;
                var borderIndex = (value & 7) | ((value & 0x40) >> 3);
                Renderer.UpdateBorder(borderIndex);
            }
        }
    }
}
