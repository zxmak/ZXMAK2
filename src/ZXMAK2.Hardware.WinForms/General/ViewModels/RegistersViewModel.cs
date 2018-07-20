using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Mvvm;
using ZXMAK2.Mvvm.Attributes;
using System.Reflection;

namespace ZXMAK2.Hardware.WinForms.General.ViewModels
{
    public class RegistersViewModel : BaseDebuggerViewModel
    {
        private static readonly List<PropertyInfo> _regProps = typeof(RegistersViewModel)
            .GetProperties()
            .Where(pi => pi.DeclaringType == typeof(RegistersViewModel))
            .Where(pi => !pi.Name.StartsWith("Flag"))
            .ToList();
        private Dictionary<PropertyInfo, object> _propertyCache = new Dictionary<PropertyInfo, object>();

        
        public RegistersViewModel(IDebuggable target, ISynchronizeInvoke synchronizeInvoke)
            : base (target, synchronizeInvoke)
        {
        }

        
        #region Properties

        public ushort Pc
        {
            get { return Target.CPU.regs.PC; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("Pc", ref Target.CPU.regs.PC, value);
                Target.RaiseUpdateState();
            }
        }

        public ushort Sp
        {
            get { return Target.CPU.regs.SP; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("Sp", ref Target.CPU.regs.SP, value);
                Target.RaiseUpdateState();
            }
        }

        public ushort Ir
        {
            get { return Target.CPU.regs.IR; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("Ir", ref Target.CPU.regs.IR, value);
                Target.RaiseUpdateState();
            }
        }

        public int Im
        {
            get { return Target.CPU.IM; }
            set 
            {
                if (Target.IsRunning) return;
                if (value >= 0 && value <= 2)
                {
                    PropertyChangeVal("Im", ref Target.CPU.IM, (byte)(value % 3));
                }
                else
                {
                    OnPropertyChanged("Im");
                }
                Target.RaiseUpdateState();
            }
        }

        public ushort Wz
        {
            get { return Target.CPU.regs.MW; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("Wz", ref Target.CPU.regs.MW, value);
                Target.RaiseUpdateState();
            }
        }

        public ushort Lpc
        {
            get { return Target.CPU.LPC; }
        }

        public ushort Af
        {
            get { return Target.CPU.regs.AF; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("Af", ref Target.CPU.regs.AF, value);
                Target.RaiseUpdateState();
            }
        }

        public ushort Af_
        {
            get { return Target.CPU.regs._AF; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("Af_", ref Target.CPU.regs._AF, value);
                Target.RaiseUpdateState();
            }
        }

        public ushort Hl
        {
            get { return Target.CPU.regs.HL; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("Hl", ref Target.CPU.regs.HL, value);
                Target.RaiseUpdateState();
            }
        }

        public ushort Hl_
        {
            get { return Target.CPU.regs._HL; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("Hl_", ref Target.CPU.regs._HL, value);
                Target.RaiseUpdateState();
            }
        }

        public ushort De
        {
            get { return Target.CPU.regs.DE; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("De", ref Target.CPU.regs.DE, value);
                Target.RaiseUpdateState();
            }
        }

        public ushort De_
        {
            get { return Target.CPU.regs._DE; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("De_", ref Target.CPU.regs._DE, value);
                Target.RaiseUpdateState();
            }
        }

        public ushort Bc
        {
            get { return Target.CPU.regs.BC; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("Bc", ref Target.CPU.regs.BC, value);
                Target.RaiseUpdateState();
            }
        }

        public ushort Bc_
        {
            get { return Target.CPU.regs._BC; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("Bc_", ref Target.CPU.regs._BC, value);
                Target.RaiseUpdateState();
            }
        }

        public ushort Ix
        {
            get { return Target.CPU.regs.IX; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("Ix", ref Target.CPU.regs.IX, value);
                Target.RaiseUpdateState();
            }
        }

        public ushort Iy
        {
            get { return Target.CPU.regs.IY; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("Iy", ref Target.CPU.regs.IY, value);
                Target.RaiseUpdateState();
            }
        }

        public bool Iff1
        {
            get { return Target.CPU.IFF1; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("Iff1", ref Target.CPU.IFF1, value);
                Target.RaiseUpdateState();
            }
        }

        public bool Iff2
        {
            get { return Target.CPU.IFF2; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("Iff2", ref Target.CPU.IFF2, value);
                Target.RaiseUpdateState();
            }
        }

        public bool Halt
        {
            get { return Target.CPU.HALTED; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("Halt", ref Target.CPU.HALTED, value);
                Target.RaiseUpdateState();
            }
        }

        public bool Bint
        {
            get { return Target.CPU.BINT; }
            set 
            {
                if (Target.IsRunning) return;
                PropertyChangeVal("Bint", ref Target.CPU.BINT, value);
                Target.RaiseUpdateState();
            }
        }

        
        #region Flags

        [DependsOnProperty("Af")]
        public bool FlagS
        {
            get { return (Af & 0x80) != 0; }
            set { Af = (ushort)((Af & ~0x80) | (value ? 0x80 : 0)); }
        }

        [DependsOnProperty("Af")]
        public bool FlagZ
        {
            get { return (Af & 0x40) != 0; }
            set { Af = (ushort)((Af & ~0x40) | (value ? 0x40 : 0)); }
        }

        [DependsOnProperty("Af")]
        public bool Flag5
        {
            get { return (Af & 0x20) != 0; }
            set { Af = (ushort)((Af & ~0x20) | (value ? 0x20 : 0)); }
        }

        [DependsOnProperty("Af")]
        public bool FlagH
        {
            get { return (Af & 0x10) != 0; }
            set { Af = (ushort)((Af & ~0x10) | (value ? 0x10 : 0)); }
        }

        [DependsOnProperty("Af")]
        public bool Flag3
        {
            get { return (Af & 0x08) != 0; }
            set { Af = (ushort)((Af & ~0x08) | (value ? 0x08 : 0)); }
        }

        [DependsOnProperty("Af")]
        public bool FlagV
        {
            get { return (Af & 0x04) != 0; }
            set { Af = (ushort)((Af & ~0x04) | (value ? 0x04 : 0)); }
        }

        [DependsOnProperty("Af")]
        public bool FlagN
        {
            get { return (Af & 0x02) != 0; }
            set { Af = (ushort)((Af & ~0x02) | (value ? 0x02 : 0)); }
        }

        [DependsOnProperty("Af")]
        public bool FlagC
        {
            get { return (Af & 0x01) != 0; }
            set { Af = (ushort)((Af & ~0x01) | (value ? 0x01 : 0)); }
        }

        #endregion Flags


        public bool IsRzxAvailable
        {
            get { return Target.RzxState.IsPlayback; }
        }
        
        public string RzxFetch
        {
            get { return string.Format("{0} / {1}", Target.RzxState.Fetch, Target.RzxState.FetchCount); }
        }

        public string RzxInput
        {
            get { return string.Format("{0} / {1}", Target.RzxState.Input, Target.RzxState.InputCount); }
        }

        public string RzxFrame
        {
            get { return string.Format("{0} / {1}", Target.RzxState.Frame, Target.RzxState.FrameCount); }
        }

        #endregion Properties


        #region Private

        protected override void OnTargetStateChanged()
        {
            base.OnTargetStateChanged();
            if (IsRunning)
            {
                return;
            }
            _regProps
                .ForEach(pi => OnPropertyChanged(pi.Name));
            // Check implemented on binding level
            //// In order to improve UI performance,
            //// we need to eliminate redundant notifications.
            //// So, we raise notification if value is really changed
            //foreach (var pi in _regProps)
            //{
            //    var value = pi.GetValue(this, null);
            //    if (_propertyCache.ContainsKey(pi) &&
            //        !object.Equals(_propertyCache[pi], value))
            //    {
            //        OnPropertyChanged(pi.Name);
            //    }
            //    _propertyCache[pi] = value;
            //}
        }

        #endregion Private
    }
}
