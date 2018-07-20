using System;
using System.Windows.Forms;


namespace ZXMAK2.Host.WinForms.Controls
{
    /// <summary>
    /// Fixed version of ToolStrip control.
    /// It allows click-through functionality.
    /// http://blogs.msdn.com/b/rickbrew/archive/2006/01/09/511003.aspx
    /// </summary>
    public class ToolStripEx : ToolStrip
    {
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            const int WM_MOUSEACTIVATE = 0x0021;
            const int MA_ACTIVATE = 1;
            const int MA_ACTIVATEANDEAT = 2;

            if (m.Msg == WM_MOUSEACTIVATE &&
                m.Result == (IntPtr)MA_ACTIVATEANDEAT)
            {
                m.Result = (IntPtr)MA_ACTIVATE;
            }
        }
    }
}
