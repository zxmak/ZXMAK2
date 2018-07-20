using System;
using System.Drawing;


namespace ZXMAK2.Engine.Interfaces
{
    public interface IVirtualMachine : IDisposable
    {
        event EventHandler FrameSizeChanged;

        bool IsRunning { get; }
        IBus Bus { get; }
        Size FrameSize { get; }
        
        void DoRun();
        void DoStop();

        void DoReset();
        void DoNmi();

        void SaveConfig();
    }
}
