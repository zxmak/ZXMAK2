using System;


namespace ZXMAK2.Host.Interfaces
{
    public interface IHostMouse : IDisposable
    {
        IMouseState MouseState { get; }
        bool IsCaptured { get; }

        void Scan();
        void Capture();
        void Uncapture();
    }
}
