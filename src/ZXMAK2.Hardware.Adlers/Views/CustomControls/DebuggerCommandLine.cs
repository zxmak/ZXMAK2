using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ZXMAK2.Hardware.Adlers.Views.CustomControls
{
    public partial class DebuggerCommandLine : TextBox
    {
        [DllImport("user32.dll")]
        static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);
        [DllImport("user32.dll")]
        static extern bool ShowCaret(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool SetCaretPos(int x, int y);
        [DllImport("user32")]
        private extern static int GetCaretPos(out Point p);

        private bool _modeOverwrite;

        public DebuggerCommandLine()
        {
            InitializeComponent();
            _modeOverwrite = false;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        protected override void OnGotFocus(System.EventArgs e)
        {
            DisplayCaret();
            base.OnGotFocus(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if( e.KeyCode == Keys.Insert )
            {
                _modeOverwrite = !_modeOverwrite;
                DisplayCaret();
            }
        }

        private void DisplayCaret()
        {
            // https://msdn.microsoft.com/en-us/library/windows/desktop/ms648398%28v=vs.85%29.aspx
            // http://stackoverflow.com/questions/15307381/getcaretpos-works-improperly
            if (_modeOverwrite)
            {
                CreateCaret(this.Handle, IntPtr.Zero, 8, 2);
                ShowCaret(this.Handle);
            }
            else
            {
                CreateCaret(this.Handle, IntPtr.Zero, 8, this.Height - 1);
                ShowCaret(this.Handle);

                //Point p;
                //GetCaretPos(out p);
                //SetCaretPos(p.X, 12);
            }
        }
    }
}
