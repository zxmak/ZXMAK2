using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Hardware.WinForms.General.ViewModels.Entities;
using System.Collections.ObjectModel;

namespace ZXMAK2.Hardware.WinForms.General.ViewModels
{
    public class BreakpointsViewModel : BaseDebuggerViewModel
    {
        private readonly BindingList<BreakpointViewModel> _breakpoints = new BindingList<BreakpointViewModel>();
        
        public BreakpointsViewModel(IDebuggable target, ISynchronizeInvoke synchronizeInvoke)
            : base(target, synchronizeInvoke)
        {
        }


        #region Properties

        public IEnumerable<BreakpointViewModel> Breakpoints 
        {
            get { return _breakpoints; }
        }

        private BreakpointViewModel _selectedBreakpoint;
        
        public BreakpointViewModel SelectedBreakpoint
        {
            get { return _selectedBreakpoint; }
            set { PropertyChangeRef("SelectedBreakpoint", ref _selectedBreakpoint, value); }
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
            // synchronize
            var newItems = Target.GetBreakpointList();
            var oldItems = _breakpoints.Select(arg => arg.Entity);
            var add = newItems
                .Except(oldItems)
                .Select(arg => new BreakpointViewModel(arg))
                .ToList();
            var del = oldItems
                .Except(newItems)
                .Select(arg => _breakpoints.First(bp => bp.Entity == arg))
                .ToList();
            if (SelectedBreakpoint != null &&
                del.Any(arg => arg == SelectedBreakpoint))
            {
                SelectedBreakpoint = null;
            }
            del.ForEach(arg => _breakpoints.Remove(arg));
            add.ForEach(arg => _breakpoints.Add(arg));
        }

        #endregion Private
    }
}
