using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZXMAK2.Hardware.Adlers.Views.CustomControls
{
    public partial class ProgressBarBackgroundProcess : ProgressBar
    {
        readonly object _sync = new object();

        Action _doworkAction = null;
        Action _onCompletedAction = null;
        Action _cancelActionGUI = null;
        int _maxProgressBarValue = -1;

        private BackgroundWorker _backgroundWorker = new BackgroundWorker();

        private bool _isStarted = false;
        //private bool _isCanceled = false;

        public ProgressBarBackgroundProcess()
        {
            InitializeComponent();

            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.DoWork += new DoWorkEventHandler(_backgroundWorker_DoWork);
            _backgroundWorker.ProgressChanged += new ProgressChangedEventHandler(_backgroundWorker_ProgressChanged);
            _backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_backgroundWorker_RunWorkerCompleted);
        }
        public void Init(int i_maxValue, Action i_doworkAction, Action i_cancelActionGUI)
        {
            BackgroundWorker _backgroundWorker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            _doworkAction = i_doworkAction;
            _cancelActionGUI = i_cancelActionGUI;
            this.Value = 0;
            _maxProgressBarValue = i_maxValue;
        }

        public void Start()
        {
            lock (_sync)
            {
                if (_doworkAction == null || _maxProgressBarValue == -1 /*|| _cancelActionGUI == null*/)
                    throw new Exception("Assembler: Process not initialized");

                this.Maximum = _maxProgressBarValue;

                if (_isStarted)
                    _backgroundWorker.CancelAsync();

                this.Value = 0;
                _isStarted = true;
                _backgroundWorker.RunWorkerAsync();
            }
        }

        public void Finish()
        {
            lock (_sync)
            {
                if (!_isStarted)
                    throw new Exception("Assembler: Process not started!");

                if (_backgroundWorker.CancellationPending == false) //already canceled by user?
                    _backgroundWorker.CancelAsync();
                this.Value = this.Maximum;
                _isStarted = false;

                if (_onCompletedAction != null)
                    _onCompletedAction();
            }
            _doworkAction = _cancelActionGUI = _onCompletedAction = null;
        }
        public void CancelProcessManual()
        {
            lock (_sync)
                _backgroundWorker.CancelAsync();
        }
        public bool IsTerminateSignalReceived()
        {
            return _backgroundWorker.CancellationPending;
        }
        public bool IsWorking()
        {
            return _isStarted;
        }

        public void IncreaseCounter()
        {
            if (_backgroundWorker.CancellationPending) //already canceled by user?
                return;
            if (this.Value < this.Maximum && this._maxProgressBarValue != -1)
            {
                ProcessInvokeRequired(this, () => this.Value++); //RunOnNewThread(this, () => this.Value++);
                //Application.DoEvents();
            }
        }
        #region Background worker
        public BackgroundWorker GetBackgroundWorker()
        {
            return _backgroundWorker;
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _doworkAction();
        }

        private void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Value++;
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Finish();
        }

        public void SetOnFinishedAction(Action i_onCompletedAction)
        {
            _onCompletedAction = i_onCompletedAction;
        }

        //run async without progress bar
        public void RunOnNewThread(Control i_rootControl, Action i_action)
        {
            Action actionBackground = new Action(() =>
            {
                ProgressBarBackgroundProcess.ProcessInvokeRequired(i_rootControl, i_action);
            });

            Thread newThread = new Thread(actionBackground.Invoke);
            newThread.Start();
        }


        public static void ProcessInvokeRequired(Control i_rootControl, Action i_action)
        {
            if (i_rootControl.InvokeRequired)
                i_rootControl.Invoke(new Action(() =>
                {
                    i_action();
                }));
            else
            {
                i_action();
            }
        }
        #endregion Background worker
    }
}
