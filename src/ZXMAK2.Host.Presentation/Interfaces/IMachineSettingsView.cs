using System;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;


namespace ZXMAK2.Host.Presentation.Interfaces
{
    public interface IMachineSettingsView : IDisposable
    {
        void Init(IHostService host, IVirtualMachine vm);
        DlgResult ShowDialog(object owner);
    }
}
