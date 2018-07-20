using System;
using System.ComponentModel;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Host.Presentation.Interfaces
{
    public interface IMainView : IDisposable
    {
        object DataContext { get; set; }
        
        IHostService Host { get; }
        ICommandManager CommandManager { get; }

        event EventHandler ViewOpened;
        event EventHandler ViewClosed;
        event EventHandler RequestFrame;

        void Run();
        void Close();
        void Activate();
    }
}
