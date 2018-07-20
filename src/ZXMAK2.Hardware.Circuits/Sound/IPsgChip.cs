using System;


namespace ZXMAK2.Hardware.Circuits.Sound
{
    public interface IPsgChip
    {
        int ChipFrequency { get; set; }
        PanType PanType { get; set; }
        AmpType AmpType { get; set; }
        int Volume { get; set; }
        byte RegAddr { get; set; }

        Action<double, ushort, ushort> UpdateHandler { get; set; }

        void Reset();
        byte GetReg(int index);
        void SetReg(int index, byte value);
        double SetReg(double lastTime, double time, int index, byte value);
        double Update(double startTime, double endTime);
    }
}
