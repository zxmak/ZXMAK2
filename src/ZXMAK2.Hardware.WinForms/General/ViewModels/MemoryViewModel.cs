using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Mvvm;

namespace ZXMAK2.Hardware.WinForms.General.ViewModels
{
    public class MemoryViewModel : BaseDebuggerViewModel
    {
        public MemoryViewModel(IDebuggable target, ISynchronizeInvoke synchronizeInvoke)
            : base (target, synchronizeInvoke)
        {
        }

        public byte[] GetData(ushort addr, int len)
        {
            var data = new byte[len];
            Target.ReadMemory(addr, data, 0, len);
            return data;
        }
    }
}
