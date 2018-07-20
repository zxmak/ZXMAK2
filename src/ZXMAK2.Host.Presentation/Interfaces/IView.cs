using System;
using System.ComponentModel;


namespace ZXMAK2.Host.Presentation.Interfaces
{
    public interface IView : IDisposable
    {
        void Show(IMainView parent);
        void Hide();
        void Close();

        event EventHandler ViewClosed;
        event CancelEventHandler ViewClosing;
    }
}
