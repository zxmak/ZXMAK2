using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Hardware.WinForms.General.ViewModels;

namespace ZXMAK2.Hardware.WinForms.General.Views
{
    public partial class FormDisassembly : DockContent
    {
        private DisassemblyViewModel _dataContext;

        public FormDisassembly()
        {
            InitializeComponent();
        }

        public void Attach(IDebuggable target)
        {
            _dataContext = new DisassemblyViewModel(target, this);
            _dataContext.PropertyChanged += DataContext_OnPropertyChanged;
            _dataContext.Attach();
        }

        protected override void OnClosed(EventArgs e)
        {
            _dataContext.PropertyChanged -= DataContext_OnPropertyChanged;
            _dataContext.Detach();
            base.OnClosed(e);
        }

        private void DataContext_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsRunning":
                    if (_dataContext.IsRunning)
                    {
                        dasmPanel.UpdateLines();
                        dasmPanel.Refresh();
                    }
                    break;
                case "ActiveAddress":
                    if (_dataContext.ActiveAddress != null)
                    {
                        dasmPanel.ActiveAddress = _dataContext.ActiveAddress.Value;
                    }
                    break;
            }
        }

        private bool DasmPanel_OnCheckExecuting(object sender, ushort addr)
        {
            return _dataContext != null &&
                !_dataContext.IsRunning &&
                _dataContext.ActiveAddress.HasValue &&
                _dataContext.ActiveAddress.Value == addr;
        }

        private void DasmPanel_OnGetDasm(object sender, ushort addr, out string dasm, out int len)
        {
            if (_dataContext == null)
            {
                dasm = "N/A";
                len = 1;
                return;
            }
            _dataContext.GetDisassembly(addr, out dasm, out len);
        }

        private void DasmPanel_OnGetData(object sender, ushort addr, int len, out byte[] data)
        {
            if (_dataContext == null)
            {
                data = new byte[1];
                return;
            }
            data = _dataContext.GetData(addr, len);
        }

        private bool DasmPanel_OnCheckBreakpoint(object sender, ushort addr)
        {
            return _dataContext != null && 
                _dataContext.CheckBreakpoint(addr);
        }

        private void DasmPanel_OnBreakpointClick(object sender, ushort addr)
        {
            if (_dataContext == null || !_dataContext.CommandSetBreakpoint.CanExecute(addr))
            {
                return;
            }
            _dataContext.CommandSetBreakpoint.Execute(addr);
        }
    }
}
