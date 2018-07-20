using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Mvvm;

namespace ZXMAK2.Hardware.WinForms.General.ViewModels
{
    public class DebuggerViewModel : BaseDebuggerViewModel
    {
        public DebuggerViewModel(IDebuggable target, ISynchronizeInvoke synchronizeInvoke)
            : base(target, synchronizeInvoke)
        {
            CommandClose = new CommandDelegate(
                CommandClose_OnExecute,
                CommandClose_OnCanExecute,
                "Close");
            CommandContinue = new CommandDelegate(
                CommandContinue_OnExecute, 
                CommandContinue_OnCanExecute, 
                "Continue");
            CommandBreak = new CommandDelegate(
                CommandBreak_OnExecute,
                CommandBreak_OnCanExecute,
                "Break");
            CommandStepInto = new CommandDelegate(
                CommandStepInto_OnExecute,
                CommandStepInto_OnCanExecute,
                "Step Into");
            CommandStepOver = new CommandDelegate(
                CommandStepOver_OnExecute,
                CommandStepOver_OnCanExecute,
                "Step Over");
            CommandStepOut = new CommandDelegate(
                () => { },
                () => false,
                "Step Out");
        }

        
        public event EventHandler ShowRequest;


        #region Close Behavior

        public event EventHandler CloseRequest;

        private void Close()
        {
            var handler = CloseRequest;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Close Behavior


        #region Properties

        private string _statusText;

        public string StatusText
        {
            get { return _statusText; }
            set { PropertyChangeRef("StatusText", ref _statusText, value); }
        }

        private string _statusTact;

        public string StatusTact
        {
            get { return _statusTact; }
            set { PropertyChangeRef("StatusTact", ref _statusTact, value); }
        }

        #endregion Properties


        #region Commands

        public ICommand CommandClose { get; private set; }
        public ICommand CommandContinue { get; private set; }
        public ICommand CommandBreak { get; private set; }
        public ICommand CommandStepInto { get; private set; }
        public ICommand CommandStepOver { get; private set; }
        public ICommand CommandStepOut { get; private set; }

        
        private bool CommandClose_OnCanExecute()
        {
            return true;
        }

        private void CommandClose_OnExecute()
        {
            if (!CommandClose_OnCanExecute())
            {
                return;
            }
            Close();
        }

        private bool CommandContinue_OnCanExecute()
        {
            return Target != null && !IsRunning;
        }

        private void CommandContinue_OnExecute()
        {
            if (!CommandContinue_OnCanExecute())
            {
                return;
            }
            Target.DoRun();
        }

        private bool CommandBreak_OnCanExecute()
        {
            return Target != null && IsRunning;
        }

        private void CommandBreak_OnExecute()
        {
            if (!CommandBreak_OnCanExecute())
            {
                return;
            }
            Target.DoStop();
        }

        private bool CommandStepInto_OnCanExecute()
        {
            return Target != null && !IsRunning;
        }

        private void CommandStepInto_OnExecute()
        {
            if (!CommandStepInto_OnCanExecute())
            {
                return;
            }
            Target.DoStepInto();
        }

        private bool CommandStepOver_OnCanExecute()
        {
            return Target != null && !IsRunning;
        }

        private void CommandStepOver_OnExecute()
        {
            if (!CommandStepOver_OnCanExecute())
            {
                return;
            }
            Target.DoStepOver();
        }

        #endregion Commands


        #region Private

        protected override void OnTargetStateChanged()
        {
            base.OnTargetStateChanged();
            CommandClose.Update();
            CommandContinue.Update();
            CommandBreak.Update();
            CommandStepInto.Update();
            CommandStepOver.Update();
            CommandStepOut.Update();

            StatusTact = IsRunning ? string.Format("T: - / {0}", Target.FrameTactCount) :
                string.Format("T: {0} / {1}", Target.GetFrameTact(), Target.FrameTactCount);
            StatusText = IsRunning ? "Running" : "Ready";
        }

        protected override void OnTargetBreakpoint()
        {
            base.OnTargetBreakpoint();
            var handler = ShowRequest;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Private
    }
}
