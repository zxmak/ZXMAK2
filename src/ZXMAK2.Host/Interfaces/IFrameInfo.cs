

namespace ZXMAK2.Host.Interfaces
{
    public interface IFrameInfo
    {
        IIconDescriptor[] Icons { get; }

        int StartTact { get; }
        double UpdateTime { get; }
        bool IsRefresh { get; }
        int SampleRate { get; }
    }
}
