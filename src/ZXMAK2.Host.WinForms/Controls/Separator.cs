using System;
using System.Windows.Forms;
using System.Drawing;


namespace ZXMAK2.Host.WinForms.Controls
{
    public class Separator : Control
    {
        private Orientation m_orientation;
        private ContentAlignment m_alignment;

        public Separator()
        {
            SetStyle(ControlStyles.ResizeRedraw, true);
            m_orientation = Orientation.Horizontal;
            m_alignment = ContentAlignment.MiddleCenter;
            OnResize(EventArgs.Empty);
        }

        public Orientation Orientation
        {
            get { return m_orientation; }
            set { if (m_orientation != value) { m_orientation = value; Size = new Size(Height, Width); } }
        }

        public ContentAlignment Alignment
        {
            get { return m_alignment; }
            set { m_alignment = value; Invalidate(); }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            int minSize = 6;
            Size size = Orientation == Orientation.Horizontal ?
                new Size(Width < minSize ? minSize : Width, minSize) :
                new Size(minSize, Height < minSize ? minSize : Height);
            if (Size != size)
                Size = size;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Orientation == Orientation.Horizontal)
            {
                var y = 0;
                if (Alignment == ContentAlignment.MiddleLeft || Alignment == ContentAlignment.MiddleCenter || Alignment == ContentAlignment.MiddleRight)
                {
                    y = Height / 2;
                }
                if (Alignment == ContentAlignment.BottomLeft || Alignment == ContentAlignment.BottomCenter || Alignment == ContentAlignment.BottomRight)
                {
                    y = Height - 2;
                }
                e.Graphics.DrawLine(
                    SystemPens.ControlDark, 
                    0, 
                    y,
                    Width - 1, 
                    y);
                e.Graphics.DrawLine(
                    SystemPens.ControlLightLight, 
                    0,
                    y + 1,
                    Width - 1,
                    y + 1);
            }
            else
            {
                var x = 0;
                if (Alignment == ContentAlignment.TopCenter || Alignment == ContentAlignment.MiddleCenter || Alignment == ContentAlignment.BottomCenter)
                {
                    x = Width / 2;
                }
                if (Alignment == ContentAlignment.TopRight || Alignment == ContentAlignment.MiddleRight || Alignment == ContentAlignment.BottomRight)
                {
                    x = Width - 2;
                }
                e.Graphics.DrawLine(
                    SystemPens.ControlDark, 
                    x, 
                    0,
                    x,
                    Height - 1);
                e.Graphics.DrawLine(
                    SystemPens.ControlLightLight,
                    x + 1, 
                    0,
                    x + 1,
                    Height - 1);
            }
        }
    }
}
