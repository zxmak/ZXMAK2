/// Description: Generic ZX Spectrum emulator
/// Author: Alex Makeev
/// Date: 18.03.2008
using System;
using System.Linq;
using System.Collections.Generic;

using ZXMAK2.Engine.Cpu;
using ZXMAK2.Engine.Cpu.Tools;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Entities;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Engine
{
    public sealed class Spectrum : IMachineState, IDisposable
    {
        #region Fields

        private readonly BusManager _bus = new BusManager();
        private readonly List<Breakpoint> _breakpoints = new List<Breakpoint>();

        private long _tactLimitStepOver = 71680 * 5;
        private bool _isRunning = false;
        private int _frameStartTact;

        #endregion Fields


        #region .ctor

        public Spectrum()
        {
            BusManager.FrameReady += () => _frameEmpty = false;
        }

        public void Dispose()
        {
        }

        #endregion .ctor


        #region Properties

        public event EventHandler Breakpoint;
        public event EventHandler UpdateState;

        public CpuUnit CPU
        {
            get { return _bus.Cpu; }
        }

        public BusManager BusManager
        {
            get { return _bus; }
        }

        public int FrameStartTact
        {
            get { return _frameStartTact; }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
            set { _isRunning = value; OnUpdateState(); }
        }

        #endregion Properties


        #region Public

        public void Init()
        {
            _tactLimitStepOver = 71680 * 50 * 5;
            _bus.Init(this, false);
            _bus.Cpu.RST = true;
            _bus.Cpu.ExecCycle();
            _bus.Cpu.RST = false;
            _bus.Cpu.Tact = 0;
        }

        public void RaiseUpdateState()
        {
            OnUpdateState();
        }

        private bool _frameEmpty;

        public void ExecuteFrame()
        {
            var frameTact = _bus.GetFrameTact();
            var cpu = _bus.Cpu;
            var t = cpu.Tact - frameTact + _bus.FrameTactCount;

            _frameEmpty = true;

            while (_frameEmpty) //t > cpu.Tact/* && IsRunning*/)
            {
                // Alex: performance critical block, do not modify!
                _bus.ExecCycle();
                if (_breakpoints.Count == 0 || cpu.HALTED)
                {
                    continue;
                }
                // Alex: end of performance critical block

                if (CheckBreakpoint())
                {
                    int delta1 = (int)(cpu.Tact - t);
                    if (delta1 >= 0)
                        _frameStartTact = delta1;
                    IsRunning = false;
                    OnUpdateState();
                    OnBreakpoint();
                    return;
                }
            }
            var delta = (int)(cpu.Tact - t);
            if (delta >= 0)
            {
                _frameStartTact = delta;
            }
        }

        #endregion Public


        #region Debugger

        public void DebugReset()
        {
            CPU.RST = true;
            OnExecCycle();
            CPU.RST = false;
            OnUpdateState();
        }

        public void DebugNmi()
        {
            BusManager.RequestNmi(BusManager.FrameTactCount * 50);
            OnUpdateState();
        }

        public void DebugStepInto()
        {
            if (IsRunning)
            {
                return;
            }
            StepInto();
            OnUpdateState();
        }

        public void DebugStepOver()
        {
            if (IsRunning)
            {
                return;
            }
            StepOver();
            OnUpdateState();
        }

        public byte DebugReadMemory(ushort addr)
        {
            var memory = _bus.FindDevice<IMemoryDevice>();
            if (memory == null)
            {
                return 0xFF;
            }
            return memory.RDMEM_DBG(addr);
        }

        public void DebugWriteMemory(ushort addr, byte value)
        {
            var memory = _bus.FindDevice<IMemoryDevice>();
            if (memory == null)
            {
                return;
            }
            memory.WRMEM_DBG(addr, value);
            OnUpdateState();
        }

        public void DebugAddBreakpoint(Breakpoint bp)
        {
            _breakpoints.Add(bp);
        }

        public void DebugRemoveBreakpoint(Breakpoint bp)
        {
            _breakpoints.Remove(bp);
        }

        public Breakpoint[] DebugGetBreakpointList()
        {
            return _breakpoints.ToArray();
        }

        public void DebugClearBreakpoints()
        {
            _breakpoints.Clear();
        }

        #endregion Debugger


        #region Private

        private void OnBreakpoint()
        {
            var handler = Breakpoint;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        private void OnUpdateState()
        {
            var handler = UpdateState;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        private bool OnMaxTactExceed(long tactLimit)
        {
            var service = Locator.Resolve<IUserQuery>();
            if (service == null)
            {
                return true;
            }
            var msg = string.Format(
                "{0} tacts executed,\nbut operation not complete!\n\nAre you sure to continue?",
                tactLimit);
            return service.Show(
                msg,
                "Warning",
                DlgButtonSet.YesNo,
                DlgIcon.Question) != DlgResult.Yes;
        }

        private void OnExecCycle()
        {
            int frameTact = _bus.GetFrameTact();
            var cpu = _bus.Cpu;
            long t = cpu.Tact - frameTact + _bus.FrameTactCount;
            _bus.ExecCycle();
            int delta = (int)(cpu.Tact - t);
            if (delta >= 0)
            {
                _frameStartTact = delta;
            }
            if (CheckBreakpoint())
            {
                IsRunning = false;
                OnUpdateState();
                OnBreakpoint();
            }
        }

        private void StepInto()
        {
            do
            {
                OnExecCycle();
            } while (CPU.FX != CpuModeIndex.None || CPU.XFX != CpuModeEx.None);
        }

        private void StepOver()
        {
            var tactLimit = _tactLimitStepOver;
            var t = CPU.Tact;
            var dasmTool = new DasmTool(DebugReadMemory);
            int len;
            var opCodeStr = dasmTool.GetMnemonic(CPU.regs.PC, out len);
            var nextAddr = (ushort)((CPU.regs.PC + len) & 0xFFFF);

            var donotStepOver = opCodeStr.IndexOf("J") >= 0 ||
                opCodeStr.IndexOf("RET") >= 0;

            if (donotStepOver)
            {
                StepInto();
            }
            else
            {
                while (true)
                {
                    if (CPU.Tact - t >= tactLimit)
                    {
                        OnUpdateState();
                        if (OnMaxTactExceed(tactLimit))
                        {
                            break;
                        }
                        else
                        {
                            t = CPU.Tact;
                            tactLimit *= 2;
                        }
                    }

                    StepInto();
                    if (CPU.regs.PC == nextAddr)
                        break;

                    if (CheckBreakpoint())
                    {
                        OnUpdateState();
                        OnBreakpoint();
                        break;
                    }
                }
            }
        }

        private bool CheckBreakpoint()
        {
            return _breakpoints
                .Any(bp => bp.Check(this));
        }

        #endregion Private
    }
}
