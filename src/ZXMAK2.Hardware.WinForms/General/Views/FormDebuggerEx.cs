using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Host.Presentation.Interfaces;
using ZXMAK2.Host.WinForms.Views;
using ZXMAK2.Resources;
using ZXMAK2.Hardware.WinForms.General.ViewModels;
using ZXMAK2.Mvvm.BindingTools;
using ZXMAK2.Host.WinForms.BindingTools;
using ZXMAK2.Mvvm;


namespace ZXMAK2.Hardware.WinForms.General.Views
{
    public partial class FormDebuggerEx : FormView, IDebuggerExView
    {
        private readonly BindingService _binding = new BindingService();
        private DebuggerViewModel _dataContext;
        private List<DockContent> _childs = new List<DockContent>();
        private bool _isCloseRequest;
        private bool _isUiRequest;
        private bool _isCloseCalled;


        public FormDebuggerEx(IDebuggable debugTarget)
        {
            _binding.RegisterAdapterFactory<Control>(
                arg => new ControlBindingAdapter(arg));
            _binding.RegisterAdapterFactory<ToolStripItem>(
                arg => new ToolStripItemBindingAdapter(arg));

            _dataContext = new DebuggerViewModel(debugTarget, this);
            _dataContext.Attach();
            _dataContext.ShowRequest += DataContext_OnShowRequest;
            _dataContext.CloseRequest += DataContext_OnCloseRequest;
            
            InitializeComponent();

            LoadImages();
            dockPanel.DocumentStyle = DocumentStyle.DockingWindow;// .DockingMdi;

            var dasm = new FormDisassembly();
            var memr = new FormMemory();
            var regs = new FormRegisters();
            var bpts = new FormBreakpoints();

            // Mono compatible
            //dasm.Show(dockPanel, DockState.Document);
            //regs.Show(dasm.Pane, DockAlignment.Right, 0.24);
            //memr.Show(dasm.Pane, DockAlignment.Bottom, 0.3);
            
            regs.Show(dockPanel, DockState.DockRight);
            bpts.Show(dockPanel, DockState.DockRight);
            dasm.Show(dockPanel, DockState.Document);
            memr.Show(dasm.Pane, DockAlignment.Bottom, 0.3);

            regs.Activate();
            dasm.Activate();

            _childs.Add(dasm);
            _childs.Add(regs);
            _childs.Add(memr);
            _childs.Add(bpts);
            dasm.FormClosed += (s, e) => _childs.Remove(dasm);
            regs.FormClosed += (s, e) => _childs.Remove(regs);
            memr.FormClosed += (s, e) => _childs.Remove(memr);
            bpts.FormClosed += (s, e) => _childs.Remove(bpts);

            dasm.Attach(debugTarget);
            regs.Attach(debugTarget);
            memr.Attach(debugTarget);
            bpts.Attach(debugTarget);

            _binding.DataContext = _dataContext;
            Bind();

            KeyPreview = true;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _dataContext.ShowRequest -= DataContext_OnShowRequest;
            _dataContext.CloseRequest -= DataContext_OnCloseRequest;
            if (CommandClose != null)
            {
                CommandClose.CanExecuteChanged -= CommandClose_OnCanExecuteChanged;
            }
            _dataContext.Detach();
            base.OnFormClosed(e);
            foreach (var child in _childs.ToArray())
            {
                child.Close();
            }
        }

        private void LoadImages()
        {
            Icon = ResourceImages.IconDebugger;

            toolStripContinue.Image = ResourceImages.DebuggerContinue;
            toolStripBreak.Image = ResourceImages.DebuggerBreak;
            toolStripStepInto.Image = ResourceImages.DebuggerStepInto;
            toolStripStepOver.Image = ResourceImages.DebuggerStepOver;
            toolStripStepOut.Image = ResourceImages.DebuggerStepOut;
            toolStripShowNext.Image = ResourceImages.DebuggerShowNext;
            toolStripBreakpoints.Image = ResourceImages.DebuggerShowBreakpoints;

            menuFileClose.Image = ResourceImages.DebuggerClose;
            menuDebugContinue.Image = ResourceImages.DebuggerContinue;
            menuDebugBreak.Image = ResourceImages.DebuggerBreak;
            menuDebugStepInto.Image = ResourceImages.DebuggerStepInto;
            menuDebugStepOver.Image = ResourceImages.DebuggerStepOver;
            menuDebugStepOut.Image = ResourceImages.DebuggerStepOut;
            menuDebugShowNext.Image = ResourceImages.DebuggerShowNext;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.F5 && _dataContext.CommandBreak.CanExecute(null))
            {
                _dataContext.CommandBreak.Execute(null);
                e.Handled = true;
            }
            if (e.KeyCode == Keys.F9 && _dataContext.CommandContinue.CanExecute(null))
            {
                _dataContext.CommandContinue.Execute(null);
                e.Handled = true;
            }
            if (e.KeyCode == Keys.F7 && _dataContext.CommandStepInto.CanExecute(null))
            {
                _dataContext.CommandStepInto.Execute(null);
                e.Handled = true;
            }
            if (e.KeyCode == Keys.F8 && _dataContext.CommandStepOver.CanExecute(null))
            {
                _dataContext.CommandStepOver.Execute(null);
                e.Handled = true;
            }
        }
        
        #region Close Behavior

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            //Logger.Debug("OnFormClosing: reason={0}, isRequest={1}, isUi={2}", e.CloseReason, _isCloseRequest, _isUiRequest);
            if (!_isCloseRequest &&
                CommandClose != null)
            {
                _isCloseCalled = false;
                _isUiRequest = true;
                var canClose = CommandClose.CanExecute(null);
                if (canClose)
                {
                    CommandClose.Execute(null);
                    canClose = _isCloseCalled;
                }
                _isUiRequest = false;
                if (!canClose)
                {
                    // WARN: Mono runtime has a bug, so if the user will
                    // close parent window, it will be closed although Cancel=true.
                    // In such case, attempt to show the window will cause 
                    // fatal Mono runtime bug (it requires OS reboot).
                    // So, we call it async to avoid such issues.
                    e.Cancel = true;
                    // show & highlight blocking window
                    BeginInvoke(new Action(() =>
                        {
                            Show();
                            WindowState = FormWindowState.Normal;
                            Activate();
                        }), null);
                }
            }
            base.OnFormClosing(e);
        }
        
        private void DataContext_OnCloseRequest(object sender, EventArgs e)
        {
            if (_isUiRequest)
            {
                _isCloseCalled = true;
                return;
            }
            _isCloseRequest = true;
            Close();
            _isCloseRequest = false;
        }

        private void DataContext_OnShowRequest(object sender, EventArgs e)
        {
            Show();
        }

        #endregion Close Behavior


        private void Bind()
        {
            _binding.Bind(this, "CommandClose", "CommandClose");

            BindCommand(menuDebugContinue, "CommandContinue");
            BindCommand(toolStripContinue, "CommandContinue");
            BindCommand(menuDebugBreak, "CommandBreak");
            BindCommand(toolStripBreak, "CommandBreak");
            BindCommand(menuDebugStepInto, "CommandStepInto");
            BindCommand(toolStripStepInto, "CommandStepInto");
            BindCommand(menuDebugStepOver, "CommandStepOver");
            BindCommand(toolStripStepOver, "CommandStepOver");
            BindCommand(menuDebugStepOut, "CommandStepOut");
            BindCommand(toolStripStepOut, "CommandStepOut");
            BindCommand(menuFileClose, "CommandClose");

            _binding.Bind(toolStripStatus, "Text", "StatusText");
            _binding.Bind(toolStripStatusTact, "Text", "StatusTact");
            _binding.Bind(statusStrip, "BackColor", "IsRunning", Converters.BoolToStatusBackColor);
            _binding.Bind(statusStrip, "ForeColor", "IsRunning", Converters.BoolToStatusForeColor);
        }

        private void BindCommand(ToolStripItem target, string path)
        {
            _binding.Bind(target, "Command", path);
            _binding.Bind(target, "Text", path + ".Text");
            _binding.Bind(target, "Checked", path + ".Checked");
        }

        private ICommand _commandClose;
        
        public ICommand CommandClose
        {
            get { return _commandClose; }
            set
            {
                if (_commandClose != null)
                {
                    _commandClose.CanExecuteChanged -= CommandClose_OnCanExecuteChanged;
                }
                _commandClose = value;
                if (_commandClose != null)
                {
                    _commandClose.CanExecuteChanged += CommandClose_OnCanExecuteChanged;
                    CommandClose_OnCanExecuteChanged(_commandClose, EventArgs.Empty);
                }
            }
        }

        private void CommandClose_OnCanExecuteChanged(object sender, EventArgs e)
        {
            var canExecute = CommandClose==null || CommandClose.CanExecute(null);
            if (canExecute)
            {
                NativeMethods.EnableCloseButton(Handle);
            }
            else
            {
                NativeMethods.DisableCloseButton(Handle);
            }
        }
    }

    [SuppressUnmanagedCodeSecurity]
    internal class NativeMethods
    {
        private static readonly Dictionary<IntPtr, int> s_windowSystemMenuHandle = new Dictionary<IntPtr, int>();

        public static bool IsWinApiNotAvailable { get; private set; }

        public static void DisableCloseButton(IntPtr hWnd)
        {
            if (IsWinApiNotAvailable)
            {
                return;
            }
            try
            {
                if (s_windowSystemMenuHandle.ContainsKey(hWnd))
                {
                    return;
                }
                var hMenu = GetSystemMenu(hWnd, false);
                s_windowSystemMenuHandle[hWnd] = hMenu;
                DeleteMenu(hMenu, 6, 1024);
            }
            catch (EntryPointNotFoundException ex)
            {
                Logger.Warn(ex);
                IsWinApiNotAvailable = true;
            }
        }

        public static void EnableCloseButton(IntPtr hWnd)
        {
            if (IsWinApiNotAvailable)
            {
                return;
            }
            try
            {
                if (!s_windowSystemMenuHandle.ContainsKey(hWnd))
                {
                    return;
                }
                var hMenu = GetSystemMenu(hWnd, true);
                DeleteMenu(hMenu, 6, 1024);
                s_windowSystemMenuHandle.Remove(hWnd);
            }
            catch (EntryPointNotFoundException ex)
            {
                Logger.Warn(ex);
                IsWinApiNotAvailable = true;
            }
        }    

        [DllImport("user32", SetLastError=true)]
        private static extern int GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32", SetLastError = true)]
        private static extern bool DeleteMenu(int hMenu, int uPosition, int uFlags);
    }
}
