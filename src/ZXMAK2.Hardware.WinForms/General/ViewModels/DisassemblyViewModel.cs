using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Mvvm;
using ZXMAK2.Engine.Cpu.Tools;
using ZXMAK2.Engine.Entities;

namespace ZXMAK2.Hardware.WinForms.General.ViewModels
{
    public class DisassemblyViewModel : BaseDebuggerViewModel
    {
        private readonly DasmTool _dasmTool;
        private readonly TimingTool _timingTool;


        public DisassemblyViewModel(IDebuggable target, ISynchronizeInvoke synchronizeInvoke)
            : base(target, synchronizeInvoke)
        {
            _dasmTool = new DasmTool(target.ReadMemory);
            _timingTool = new TimingTool(target.CPU, target.ReadMemory);
            CommandSetBreakpoint = new CommandDelegate(
                CommandSetBreakpoint_OnExecute,
                CommandSetBreakpoint_OnCanExecute);
        }


        #region Properties

        private ushort? _activeAddress;
        
        public ushort? ActiveAddress
        {
            get { return _activeAddress; }
            set { PropertyChangeNul("ActiveAddress", ref _activeAddress, value); }
        }

        #endregion Properties

        
        #region Commands

        public ICommand CommandSetBreakpoint { get; private set; }

        private bool CommandSetBreakpoint_OnCanExecute(object arg)
        {
            return !IsRunning && arg is ushort;
        }

        private void CommandSetBreakpoint_OnExecute(object obj)
        {
            if (!CommandSetBreakpoint_OnCanExecute(obj))
            {
                return;
            }
            var addr = (ushort)Convert.ToInt32(obj);
            var item = Target.GetBreakpointList()
                .FirstOrDefault(arg => arg.Address.HasValue && arg.Address == addr);
            if (item != null)
            {
                Target.RemoveBreakpoint(item);
            }
            else
            {
                Target.AddBreakpoint(new Breakpoint(addr));
            }
            Target.RaiseUpdateState();
        }
        
        #endregion Commands


        #region Private

        protected override void OnTargetStateChanged()
        {
            base.OnTargetStateChanged();
            ActiveAddress = IsRunning ? (ushort?)null : Target.CPU.regs.PC;
        }

        public byte[] GetData(ushort addr, int len)
        {
            var data = new byte[len];
            Target.ReadMemory(addr, data, 0, len);
            return data;
        }

        public void GetDisassembly(ushort addr, out string dasm, out int len)
        {
            var mnemonic = _dasmTool.GetMnemonic(addr, out len);
            var timing = _timingTool.GetTimingString(addr);
            dasm = string.Format("{0,-24} ; {1}", mnemonic, timing);
        }

        public bool CheckBreakpoint(ushort addr)
        {
            return Target.GetBreakpointList()
                .Any(arg => arg.Address.HasValue && arg.Address == addr);
        }

        #endregion Private
    }
}
