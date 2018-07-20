using System;


namespace ZXMAK2.Host.Interfaces
{
    public interface IHostKeyboard : IDisposable
    {
        void Scan();
        IKeyboardState State { get; }
    }
}
