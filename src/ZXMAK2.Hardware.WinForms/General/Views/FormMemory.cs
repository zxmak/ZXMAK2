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
    public partial class FormMemory : DockContent
    {
        private MemoryViewModel _dataContext;

        public FormMemory()
        {
            InitializeComponent();
        }

        public void Attach(IDebuggable target)
        {
            _dataContext = new MemoryViewModel(target, this);
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
                    if (!_dataContext.IsRunning)
                    {
                        dataPanel.UpdateLines();
                        dataPanel.Refresh();
                    }
                    break;
            }
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
    }
}
