

namespace ZXMAK2.Host.Interfaces
{
    public interface IFrameSound
    {
        int SampleRate { get; }
        void Refresh();
        uint[] GetBuffer();
    }
}
