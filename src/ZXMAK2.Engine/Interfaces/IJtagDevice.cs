

namespace ZXMAK2.Engine.Interfaces
{
    public interface IJtagDevice
    {
        void Attach(IDebuggable dbg);
        void Detach();
    }
}
