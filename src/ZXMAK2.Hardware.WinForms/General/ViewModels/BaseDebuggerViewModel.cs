using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Mvvm;

namespace ZXMAK2.Hardware.WinForms.General.ViewModels
{
    public abstract class BaseDebuggerViewModel : BaseViewModel
    {
        private readonly ISynchronizeInvoke _synchronizeInvoke;
        
        
        protected BaseDebuggerViewModel(IDebuggable target, ISynchronizeInvoke synchronizeInvoke)
        {
            Target = target;
            _synchronizeInvoke = synchronizeInvoke;
        }

        public void Attach()
        {
            Target.UpdateState += Target_OnUpdateState;
            Target.Breakpoint += Target_OnBreakpoint;
            OnTargetStateChanged();
        }

        public void Detach()
        {
            Target.UpdateState -= Target_OnUpdateState;
            Target.Breakpoint -= Target_OnBreakpoint;
        }


        #region Properties

        private bool _isRunning;
        
        public bool IsRunning
        {
            get { return _isRunning; }
            private set { PropertyChangeVal("IsRunning", ref _isRunning, value); }
        }

        #endregion Properties


        #region Private

        protected IDebuggable Target { get; private set; }

        protected void InvokeAsync(Action action)
        {
            _synchronizeInvoke.BeginInvoke(action, null);
        }

        private void Target_OnUpdateState(object sender, EventArgs e)
        {
            InvokeAsync(OnTargetStateChanged);
        }

        private void Target_OnBreakpoint(object sender, EventArgs e)
        {
            InvokeAsync(OnTargetBreakpoint);
        }

        protected virtual void OnTargetStateChanged()
        {
            IsRunning = Target == null || Target.IsRunning;
        }

        protected virtual void OnTargetBreakpoint()
        {
        }

        #endregion Private
    }
}
