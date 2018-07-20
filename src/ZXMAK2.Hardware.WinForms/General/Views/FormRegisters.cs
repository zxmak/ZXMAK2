using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Hardware.WinForms.General.ViewModels;
using ZXMAK2.Mvvm;
using ZXMAK2.Mvvm.BindingTools;
using ZXMAK2.Host.WinForms.Tools;
using ZXMAK2.Host.WinForms.BindingTools;


namespace ZXMAK2.Hardware.WinForms.General.Views
{
    public partial class FormRegisters : DockContent
    {
        private RegistersViewModel _dataContext;
        private BindingService _binding = new BindingService();
        
        
        public FormRegisters()
        {
            InitializeComponent();
            _binding.RegisterAdapterFactory<Control>(
                arg => new ControlBindingAdapter(arg));
        }


        public void Attach(IDebuggable target)
        {
            _dataContext = new RegistersViewModel(target, this);
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


        #region Binding

        private void Bind()
        {
            _binding.Bind(this, "IsRunning", "IsRunning");
            
            var pText = "Text";
            _binding.Bind(txtRegPc, pText, "Pc", Converters.RegPairToString);
            _binding.Bind(txtRegSp, pText, "Sp", Converters.RegPairToString);
            _binding.Bind(txtRegIr, pText, "Ir", Converters.RegPairToString);
            _binding.Bind(txtRegIm, pText, "Im");
            _binding.Bind(txtRegWz, pText, "Wz", Converters.RegPairToString);
            _binding.Bind(txtRegLpc, pText, "Lpc", Converters.RegPairToString);
            _binding.Bind(txtRegAf, pText, "Af", Converters.RegPairToString);
            _binding.Bind(txtRegAf_, pText, "Af_", Converters.RegPairToString);
            _binding.Bind(txtRegHl, pText, "Hl", Converters.RegPairToString);
            _binding.Bind(txtRegHl_, pText, "Hl_", Converters.RegPairToString);
            _binding.Bind(txtRegDe, pText, "De", Converters.RegPairToString);
            _binding.Bind(txtRegDe_, pText, "De_", Converters.RegPairToString);
            _binding.Bind(txtRegBc, pText, "Bc", Converters.RegPairToString);
            _binding.Bind(txtRegBc_, pText, "Bc_", Converters.RegPairToString);
            _binding.Bind(txtRegIx, pText, "Ix", Converters.RegPairToString);
            _binding.Bind(txtRegIy, pText, "Iy", Converters.RegPairToString);
            _binding.Bind(lblRzxFetchValue, pText, "RzxFetch");
            _binding.Bind(lblRzxInputValue, pText, "RzxInput");
            _binding.Bind(lblRzxFrameValue, pText, "RzxFrame");
            var pChecked = "Checked";
            _binding.Bind(chkIff1, pChecked, "Iff1");
            _binding.Bind(chkIff2, pChecked, "Iff2");
            _binding.Bind(chkHalt, pChecked, "Halt");
            _binding.Bind(chkBint, pChecked, "Bint");
            _binding.Bind(chkFlagS, pChecked, "FlagS");
            _binding.Bind(chkFlagZ, pChecked, "FlagZ");
            _binding.Bind(chkFlag5, pChecked, "Flag5");
            _binding.Bind(chkFlagH, pChecked, "FlagH");
            _binding.Bind(chkFlag3, pChecked, "Flag3");
            _binding.Bind(chkFlagV, pChecked, "FlagV");
            _binding.Bind(chkFlagN, pChecked, "FlagN");
            _binding.Bind(chkFlagC, pChecked, "FlagC");
            var pVisible = "Visible";
            _binding.Bind(lblTitleRzx, pVisible, "IsRzxAvailable");
            _binding.Bind(sepRzx, pVisible, "IsRzxAvailable");
            _binding.Bind(lblRzxFetch, pVisible, "IsRzxAvailable");
            _binding.Bind(lblRzxInput, pVisible, "IsRzxAvailable");
            _binding.Bind(lblRzxFrame, pVisible, "IsRzxAvailable");
            _binding.Bind(lblRzxFetchValue, pVisible, "IsRzxAvailable");
            _binding.Bind(lblRzxInputValue, pVisible, "IsRzxAvailable");
            _binding.Bind(lblRzxFrameValue, pVisible, "IsRzxAvailable");
        }

        private bool _isRunning;
        
        public bool IsRunning
        {
            get { return _isRunning; }
            set
            {
                _isRunning = value;
                OnIsRunningChanged();
            }
        }

        private void OnIsRunningChanged()
        {
            var allowEdit = !IsRunning;
            Controls
                .OfType<Control>()
                .ToList()
                .ForEach(arg => arg.Enabled = allowEdit);
            Controls
                .OfType<Control>()
                .ToList()
                .ForEach(arg => arg.BackColor = Color.White);
        }

        #endregion Binding


        #region TextBox Behavior

        private bool _isSelectionNeeded;

        private void txtReg_OnClick(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            //Logger.Debug("txtReg_OnClick: Focused={0}, SelectionStart={1}", textBox.Focused, textBox.SelectionStart);
            if (textBox.Focused && _isSelectionNeeded)
            {
                textBox.SelectAll();
                _isSelectionNeeded = false;
            }
        }

        private void txtReg_OnEnter(object sender, EventArgs e)
        {
            //_isSelectionNeeded = true;
        }

        private void txtReg_OnLeave(object sender, EventArgs e)
        {
            _isSelectionNeeded = true;
        }

        private void txtReg_KeyPress(object sender, KeyPressEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (e.KeyChar == 0x0D)
            {
                e.Handled = true;
                Validate();
            }
        }

        #endregion TextBox Behavior
    }
}
