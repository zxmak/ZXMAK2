using ZXMAK2.Model.Tape.Entities;


namespace ZXMAK2.Model.Tape.Interfaces
{
    public interface ITapeBlock
    {
        int TzxId { get; }
        string Description { get; }
        TapeCommand Command { get; }
        
        int Count { get; }
        int GetPeriod(int index, int cpuFrequency);
        byte[] GetData();
    }
}
