using System;
using System.ComponentModel;
using System.Windows.Forms;
using ZXMAK2.Host.Presentation.Interfaces;


namespace ZXMAK2.Host.WinForms.Views
{
    public class FormView : Form, IView
    {
        public void Show(IMainView parent)
        {
            if (!Visible)
            {
                Show(parent as IWin32Window);
            }
            else
            {
                Show();
                Activate();
            }
        }

        public event EventHandler ViewClosed;
        public event CancelEventHandler ViewClosing;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!e.Cancel && e.CloseReason == CloseReason.UserClosing)
            {
                var handler = ViewClosing;
                if (handler != null)
                {
                    var arg = new CancelEventArgs(e.Cancel);
                    handler(this, arg);
                    e.Cancel = arg.Cancel;
                }
            }
            base.OnFormClosing(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            var handler = ViewClosed;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
            base.OnFormClosed(e);
        }
    }
}
