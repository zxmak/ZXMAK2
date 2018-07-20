using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Hardware.WinForms.General.ViewModels;
using ZXMAK2.Mvvm.BindingTools;
using ZXMAK2.Host.WinForms.BindingTools;

namespace ZXMAK2.Hardware.WinForms.General.Views
{
    public partial class FormBreakpoints : DockContent
    {
        private BreakpointsViewModel _dataContext;
        private BindingService _binding = new BindingService();


        public FormBreakpoints()
        {
            InitializeComponent();
            _binding.RegisterAdapterFactory<Control>(
                arg => new ControlBindingAdapter(arg));
            _binding.RegisterAdapterFactory<ListBox>(
                arg => new ListBoxBindingAdapter(arg));
        }

        public void Attach(IDebuggable target)
        {
            _dataContext = new BreakpointsViewModel(target, this);
            _dataContext.Attach();
            Bind();

            _binding.DataContext = _dataContext;
        }

        protected override void OnClosed(EventArgs e)
        {
            _binding.Dispose();
            _dataContext.Detach();
            base.OnClosed(e);
        }

        private void Bind()
        {
            _binding.Bind(lstItems, "DataSource", "Breakpoints");
            _binding.Bind(lstItems, "SelectedItem", "SelectedBreakpoint");
        }
    }
}
