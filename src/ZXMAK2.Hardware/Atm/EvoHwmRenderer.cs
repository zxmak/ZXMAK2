

namespace ZXMAK2.Hardware.Atm
{
    public class EvoHwmRenderer : SpectrumRenderer
    {
        protected override int CalcTableAddrAt(int sx, int sy)
        {
            var addr = CalcTableAddrBw(sx, sy);
            return addr + 0x2000;
        }
    }
}
