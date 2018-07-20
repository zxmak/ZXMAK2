using System;


namespace ZXMAK2.Host.WinForms.Mdx
{
    public interface IAllocatorPresenter : IDisposable
    {
        event EventHandler PresentCompleted;
        string ErrorMessage { get; }
        bool IsRendering { get; }
        int RefreshRate { get; }
        
        void Attach(IntPtr hWnd);
        void Detach();
        void Register(IRenderer renderer);
        void Unregister(IRenderer renderer);
    }
}
