using System;
using System.Windows.Forms;
using System.Drawing;


namespace ZXMAK2.Hardware.WinForms.General
{
    public class DasmPanel : Control
    {
        public DasmPanel()
        {
            TabStop = true;
            //         BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Font = new Font("Courier", 13, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel);
            Size = new System.Drawing.Size(424, 148);
            ControlStyles styles = ControlStyles.Selectable |
                                   ControlStyles.UserPaint |
                                   ControlStyles.ResizeRedraw |
                                   ControlStyles.StandardClick |           // csClickEvents
                                   ControlStyles.UserMouse |               // csCaptureMouse
                                   ControlStyles.ContainerControl |        // csAcceptsControls?
                                   ControlStyles.StandardDoubleClick |     // csDoubleClicks
                //                                ControlStyles.Opaque  |                 // csOpaque
                                   0;
            base.SetStyle(styles, true);

            mouseTimer = new Timer();
            mouseTimer.Enabled = false;
            mouseTimer.Interval = 50;
            mouseTimer.Tick += OnMouseTimer;

            fLineHeight = 1;
            fVisibleLineCount = 0;
            fTopAddress = 0;
            fActiveLine = 0;
            fBreakColor = Color.Red;
            fBreakForeColor = Color.Black;
            UpdateLines();
        }

        public Color BreakpointColor
        {
            get { return fBreakColor; }
            set { fBreakColor = value; Refresh(); }
        }

        public Color BreakpointForeColor
        {
            get { return fBreakForeColor; }
            set { fBreakForeColor = value; Refresh(); }
        }

        public ushort TopAddress
        {
            get { return fTopAddress; }
            set
            {
                fTopAddress = value;
                fActiveLine = 0;
                UpdateLines();
                Invalidate();
            }
        }

        public ushort ActiveAddress
        {
            get { if ((fActiveLine >= 0) && (fActiveLine < fLineCount)) return fADDRS[fActiveLine]; return 0; }
            set
            {
                for (int i = 0; i <= fVisibleLineCount; i++)
                    if (fADDRS[i] == value)
                    {
                        if (fActiveLine != i)
                        {
                            if (i == fVisibleLineCount)
                            {
                                fTopAddress = fADDRS[1];
                                fActiveLine = i - 1;
                            }
                            else
                                fActiveLine = i;
                        }
                        UpdateLines();
                        Refresh();
                        return;
                    }
                TopAddress = value;
            }
        }

        public delegate bool ONCHECKCPU(object Sender, ushort ADDR);
        public delegate void ONGETDATACPU(object Sender, ushort ADDR, int len, out byte[] data);
        public delegate void ONGETDASMCPU(object Sender, ushort ADDR, out string DASM, out int len);
        public delegate void ONCLICKCPU(object Sender, ushort Addr);

        public event ONCHECKCPU CheckBreakpoint;
        public event ONCHECKCPU CheckExecuting;
        public event ONGETDATACPU GetData;
        public event ONGETDASMCPU GetDasm;
        public event ONCLICKCPU BreakpointClick;
        public event ONCLICKCPU DasmClick;



        // private...
        private Timer mouseTimer;

        private ushort fTopAddress;

        private Color fBreakColor;
        private Color fBreakForeColor;
        static private int fGutterWidth = 30;

        private int fLineCount
        {
            get { return fVisibleLineCount + 3; }
        }
        private int fVisibleLineCount;
        private int fLineHeight;
        private int fActiveLine;
        ushort[] fADDRS = null;
        string[] fStrADDRS = null;
        string[] fStrDATAS = null;
        string[] fStrDASMS = null;
        bool[] fBreakpoints = null;
        private Bitmap bitmap = null;

        public void DrawLines(Graphics g, int x, int y, int wid, int hei)
        {
            if ((Height <= 0) || (Width <= 0)) return;
            if (!Visible) return;
            if ((bitmap == null) || (bitmap.Width != wid) || (bitmap.Height != hei))
                bitmap = new Bitmap(wid, hei);

            using (Graphics gp = Graphics.FromImage(bitmap))
            {
                int wa = (int)gp.MeasureString("DDDD", this.Font).Width;               // "DDDD" width (addr)
                int wd = (int)gp.MeasureString("DDDDDDDDDDDDDDDD", this.Font).Width;   // "DDDDDDDDDDDDDDDD" width (data)
                int wtab = 8;
                int wsp = 8;

                int CurrentY = 0;
                Color ink;
                Color paper;

                gp.FillRectangle(new SolidBrush(BackColor), 0, 0, bitmap.Width, bitmap.Height);

                for (int line = 0; line < fVisibleLineCount; line++)
                {
                    ink = ForeColor;
                    paper = BackColor;

                    bool breakLine = fBreakpoints[line];
                    bool execLine = false;
                    if (CheckExecuting != null)
                        execLine = CheckExecuting(this, fADDRS[line]);

                    Rectangle liner = new Rectangle(fGutterWidth, CurrentY, bitmap.Width, fLineHeight);

                    if (breakLine)
                    {
                        ink = fBreakForeColor;
                        paper = fBreakColor;
                        gp.FillRectangle(new SolidBrush(paper), liner);
                    }
                    if (line == fActiveLine)
                    {
                        if (Focused)
                        {
                            ink = Color.White;
                            paper = Color.Navy;
                        }
                        else
                        {
                            ink = Color.Silver;
                            paper = Color.Gray;
                        }
                        gp.FillRectangle(new SolidBrush(paper), liner);
                        /*
                        if ((line == fActiveLine) && Focused) // doted border around selected line...
                        {
                           Point[] lins = new Point[5] 
                           { 
                              new Point(fGutterWidth, CurrentY), new Point(fGutterWidth, CurrentY+fLineHeight-1),
                              new Point(bitmap.Width-1, CurrentY+fLineHeight-1), new Point(bitmap.Width-1, CurrentY),
                              new Point(fGutterWidth, CurrentY) 
                           };
                           gp.DrawLines(new Pen(Color.Yellow), lins);
                        }
                        */
                    }
                    #region Draw gutter icons...
                    if (execLine)      // execarrow icon
                    {
                        int r = 4;    // base
                        int cx = 2 + r;
                        int cy = CurrentY + (fLineHeight / 2);
                        Point[] arr = new Point[7] { new Point(cx,cy-5), new Point(cx, cy-2),
                                    new Point(cx-3, cy-2),
                                    new Point(cx-3, cy+2), new Point(cx, cy+2),
                                    new Point(cx, cy+5),
                                    new Point(cx+5, cy) };
                        gp.FillPolygon(new SolidBrush(Color.Lime), arr);
                        gp.DrawPolygon(new Pen(Color.Black), arr);
                        Point[] shine = new Point[5] { new Point(cx-2, cy+1), new Point(cx-2, cy-1),
                                      new Point(cx+1, cy-1), new Point(cx+1, cy-3),
                                      new Point(cx+4, cy) };
                        gp.DrawLines(new Pen(Color.Yellow), shine);
                    }
                    if (breakLine)  // breakpoint icon
                    {
                        int r = 4;    // half radius
                        Rectangle bpRect;
                        int cx = 2 + r;
                        int cy = CurrentY + (fLineHeight / 2);
                        if (!execLine)
                        {
                            bpRect = new Rectangle(cx - r, cy - r, /*cx +*/ r + r + 1, /*cy +*/ r + r + 1);
                        }
                        else
                        {
                            cx += 16;
                            bpRect = new Rectangle(cx - r, cy - r, /*cx +*/ r + r + 1, /*cy +*/ r + r + 1);
                        }
                        gp.FillEllipse(new SolidBrush(fBreakColor), bpRect);
                        gp.DrawEllipse(new Pen(Color.Black), bpRect);
                    }
                    #endregion


                    gp.DrawString(fStrADDRS[line], this.Font, new SolidBrush(ink), fGutterWidth + wsp, CurrentY);
                    gp.DrawString(fStrDATAS[line], this.Font, new SolidBrush(ink), fGutterWidth + wsp + wa + wtab, CurrentY);
                    gp.DrawString(fStrDASMS[line], this.Font, new SolidBrush(ink), fGutterWidth + wsp + wa + wtab + wd + wtab, CurrentY);

                    CurrentY += fLineHeight;
                }
            }
            g.DrawImageUnscaled(bitmap, x, y);// DrawImage(bitmap, x, y);
        }

        public void UpdateLines()
        {
            fADDRS = new ushort[fLineCount];
            fStrADDRS = new string[fLineCount];
            fStrDATAS = new string[fLineCount];
            fStrDASMS = new string[fLineCount];
            fBreakpoints = new bool[fLineCount];

            ushort CurADDR = fTopAddress;
            for (int i = 0; i < fLineCount; i++)
            {
                fADDRS[i] = CurADDR;

                fStrADDRS[i] = CurADDR.ToString("X4");
                string dasm;
                int len;
                byte[] data;

                if (GetDasm != null)
                    GetDasm(this, CurADDR, out dasm, out len);
                else
                {
                    dasm = "???";
                    len = 1;
                }
                if (GetData != null)
                    GetData(this, CurADDR, len, out data);
                else
                    data = new byte[] { 0x00 };

                fStrDASMS[i] = dasm;
                string sdata = "";
                int maxdata = data.Length;
                if (maxdata > 7) maxdata = 7;
                for (int j = 0; j < maxdata; j++)
                    sdata += data[j].ToString("X2");
                if (maxdata < data.Length)
                    sdata += "..";
                fStrDATAS[i] = sdata;
                if (CheckBreakpoint != null)
                {
                    if (CheckBreakpoint(this, CurADDR))
                        fBreakpoints[i] = true;
                }
                else
                    fBreakpoints[i] = false;
                CurADDR += (ushort)len;
            }
        }

        private void ControlUp()
        {
            fActiveLine--;
            if (fActiveLine < 0)
            {
                fActiveLine++;
                fTopAddress = (ushort)(fADDRS[0] - 1);
                UpdateLines();
            }
        }

        private void ControlDown()
        {
            fActiveLine++;
            if (fActiveLine >= fVisibleLineCount)
            {
                fTopAddress = fADDRS[1];
                fActiveLine--;
                UpdateLines();
            }
        }

        private void ControlPageUp()
        {
            for (int i = 0; i < (fVisibleLineCount - 1); i++)
            {
                fTopAddress--;
                UpdateLines();
            }
        }

        private void ControlPageDown()
        {
            if (fVisibleLineCount > 0)
            {
                fTopAddress = fADDRS[fVisibleLineCount - 1];
                UpdateLines();
            }
            fTopAddress = fADDRS[1];
            UpdateLines();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    ControlDown();
                    Invalidate();
                    break;
                case Keys.Up:
                    ControlUp();
                    Invalidate();
                    break;
                case Keys.PageDown:
                    ControlPageDown();
                    Invalidate();
                    break;
                case Keys.PageUp:
                    ControlPageUp();
                    Invalidate();
                    break;
                case Keys.Enter:
                    if (DasmClick != null)
                        DasmClick(this, fADDRS[fActiveLine]);
                    UpdateLines();
                    Refresh();
                    break;
            }
            base.OnKeyDown(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                int nl = (e.Y - 1) / fLineHeight;
                if (nl < fVisibleLineCount)
                    if (nl != fActiveLine)
                    {
                        fActiveLine = nl;
                        Invalidate();
                    }
                mouseTimer.Enabled = true;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                int nl = (e.Y - 1) / fLineHeight;
                if (nl < 0) return;
                if (nl < fVisibleLineCount)
                    if (nl != fActiveLine)
                    {
                        fActiveLine = nl;
                        Invalidate();
                    }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            int delta = -e.Delta / 120;  //WHEEL_DELTA=120
            if (delta < 0)
                for (int i = 0; i < -delta; i++)
                    ControlUp();
            else
                for (int i = 0; i < delta; i++)
                    ControlDown();
            Invalidate();

            base.OnMouseWheel(e);
        }

        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            mouseTimer.Enabled = false;
            base.OnMouseCaptureChanged(e);
        }

        private void OnMouseTimer(object sender, EventArgs e)
        {
            Point mE = PointToClient(MousePosition);
            int nl = (mE.Y - 1);
            if (nl < 0)
            {
                ControlUp();
                Invalidate();
            }
            if (nl >= (fVisibleLineCount * fLineHeight))
            {
                ControlDown();
                Invalidate();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //         base.OnPaintBackground(e);
            /*
                     Pen penBlack = new Pen(Color.Black);
                     Pen penSilver = new Pen(Color.Ivory);
                     e.Graphics.DrawLine(penBlack, 0, 0, Width - 1, 0);
                     e.Graphics.DrawLine(penBlack, 0, 0, 0, Height - 1);

                     e.Graphics.DrawLine(penSilver, 0, Height - 1, Width - 1, Height - 1);
                     e.Graphics.DrawLine(penSilver, Width - 1, Height - 1, Width - 1, 0);
             */
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //         base.OnPaint(e);
            //         fAntispacing = this.Font.GetHeight(e.Graphics);
            //         int lh = (int)Math.Ceiling((double)this.Font.GetHeight(e.Graphics)*((double)this.Font.FontFamily.GetEmHeight(this.Font.Style) / (double)this.Font.FontFamily.GetLineSpacing(this.Font.Style)));
            //         fAntispacing = (fAntispacing - (float)lh)/2f;
            int lh = (int)e.Graphics.MeasureString("3D,", this.Font).Height;
            int lc = ((Height - 2) / lh);
            if (lc < 0) lc = 0;
            if ((fVisibleLineCount != lc) || (fLineHeight != lh))
            {
                fLineHeight = lh;
                fVisibleLineCount = lc;
                UpdateLines();
            }
            // chk...
            if ((fActiveLine >= fVisibleLineCount) && (fVisibleLineCount > 0))
            {
                fActiveLine = fVisibleLineCount - 1;
                Invalidate();
            }
            else if ((fActiveLine < 0) && (fVisibleLineCount > 0))
            {
                fActiveLine = 0;
                Invalidate();
            }
            DrawLines(e.Graphics, 0, 0, ClientRectangle.Width, ClientRectangle.Height);// Width - 2, Height - 2);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            Keys keys1 = keyData & Keys.KeyCode;
            switch (keys1)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Right:
                case Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                int nl = e.Y / fLineHeight;
                if (nl < fVisibleLineCount)
                {
                    if (e.X <= fGutterWidth)
                    {
                        if (BreakpointClick != null)
                            BreakpointClick(this, fADDRS[nl]);
                        UpdateLines();
                        Refresh();
                    }
                    else
                    {
                        if (DasmClick != null)
                            DasmClick(this, fADDRS[nl]);
                        UpdateLines();
                        Refresh();
                    }
                }
            }
            base.OnMouseDoubleClick(e);
        }
    }

    public class DataPanel : Control
    {
        public DataPanel()
        {
            TabStop = true;
            this.Font = new Font("Courier", 13, System.Drawing.FontStyle.Regular, GraphicsUnit.Pixel);
            Size = new System.Drawing.Size(424, 99);
            ControlStyles styles = ControlStyles.Selectable |
                                   ControlStyles.UserPaint |
                                   ControlStyles.ResizeRedraw |
                                   ControlStyles.StandardClick |           // csClickEvents
                                   ControlStyles.UserMouse |               // csCaptureMouse
                                   ControlStyles.ContainerControl |        // csAcceptsControls?
                                   ControlStyles.StandardDoubleClick |     // csDoubleClicks
                                   0;
            base.SetStyle(styles, true);

            mouseTimer = new Timer();
            mouseTimer.Enabled = false;
            mouseTimer.Interval = 50;
            mouseTimer.Tick += OnMouseTimer;

            fLineHeight = 1;
            fVisibleLineCount = 0;
            fTopAddress = 0;
            fActiveLine = 0;
            fActiveColumn = 0;
            fColCount = 8;
            UpdateLines();
        }

        public ushort TopAddress
        {
            get { return fTopAddress; }
            set
            {
                fTopAddress = value;
                fActiveLine = 0;
                UpdateLines();
                Invalidate();
            }
        }

        public int ColCount
        {
            get { return fColCount; }
            set
            {
                fColCount = value;
                UpdateLines();
                Invalidate();
            }
        }

        public delegate void ONGETDATACPU(object Sender, ushort ADDR, int len, out byte[] data);
        public delegate void ONCLICKCPU(object Sender, ushort Addr);

        public event ONGETDATACPU GetData;
        public event ONCLICKCPU DataClick;


        // private...
        private Timer mouseTimer;
        private ushort fTopAddress;

        static private int fGutterWidth = 30;
        private int fColCount;
        private int fLineCount
        {
            get { return fVisibleLineCount + 3; }
        }
        private int fVisibleLineCount;
        private int fLineHeight;
        private int fActiveLine;
        private int fActiveColumn;
        ushort[] fADDRS = null;
        byte[][] fBytesDATAS = null;
        private Bitmap bitmap = null;
        private int wa = 0;
        private int wd = 0;
        private int wtab = 0;
        private int wsp = 0;
        private int wsymb = 0;


        public void DrawLines(Graphics g, int x, int y, int wid, int hei)
        {
            if ((Height <= 0) || (Width <= 0)) return;
            if (!Visible) return;
            if ((bitmap == null) || (bitmap.Width != wid) || (bitmap.Height != hei))
                bitmap = new Bitmap(wid, hei);

            using (Graphics gp = Graphics.FromImage(bitmap))
            {
                int wdsp = 2;
                wa = (int)gp.MeasureString("DDDD", this.Font).Width;             // "DDDD" width (addr)
                wd = (int)gp.MeasureString("DD", this.Font).Width + wdsp * 2;        // "DD" width (data)
                wsymb = (int)gp.MeasureString("D", this.Font).Width;
                wtab = 8;
                wsp = 8;

                int CurrentY = 0;
                Color ink;
                Color paper;

                gp.FillRectangle(new SolidBrush(BackColor), 0, 0, bitmap.Width, bitmap.Height);


                for (int line = 0; line < fVisibleLineCount; line++)
                {
                    ink = ForeColor;
                    paper = BackColor;

                    gp.DrawString(fADDRS[line].ToString("X4"), this.Font, new SolidBrush(ink), fGutterWidth + wsp, CurrentY);

                    for (int col = 0; col < fColCount; col++)
                    {
                        ink = ForeColor;
                        paper = BackColor;
                        if ((line == fActiveLine) && (col == fActiveColumn))
                        {
                            if (Focused)
                            {
                                ink = Color.White;
                                paper = Color.Navy;
                            }
                            else
                            {
                                ink = Color.Silver;
                                paper = Color.Gray;
                            }
                            gp.FillRectangle(new SolidBrush(paper), new Rectangle(fGutterWidth + wsp + wa + wtab + (col * wd), CurrentY, wd, fLineHeight));
                            gp.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(fGutterWidth + wsp + wa + wtab + (fColCount * wd) + wtab + (col * wsymb), CurrentY, wsymb, fLineHeight));
                            /*
                            if (Focused) // doted border around selected line...
                            {
                               Point[] lins = new Point[5] 
                               { 
                                  new Point(fGutterWidth + wsp + wa + wtab + (col * wd), CurrentY), 
                                  new Point(fGutterWidth + wsp + wa + wtab + (col * wd), CurrentY+fLineHeight-1),
                                  new Point(fGutterWidth + wsp + wa + wtab + (col * wd)+wd-1, CurrentY+fLineHeight-1), 
                                  new Point(fGutterWidth + wsp + wa + wtab + (col * wd)+wd-1, CurrentY),
                                  new Point(fGutterWidth + wsp + wa + wtab + (col * wd), CurrentY) 
                               };
                               gp.DrawLines(new Pen(Color.Yellow), lins);
                            }
                            */
                        }
                        gp.DrawString(fBytesDATAS[line][col].ToString("X2"), this.Font, new SolidBrush(ink), fGutterWidth + wsp + wa + wtab + (col * wd) + wdsp, CurrentY);
                        string sch = new String(zxencode[fBytesDATAS[line][col]], 1);
                        gp.DrawString(sch, this.Font, new SolidBrush(ink), fGutterWidth + wsp + wa + wtab + (fColCount * wd) + wtab + (col * wsymb), CurrentY);
                    }
                    CurrentY += fLineHeight;
                }
            }
            g.DrawImageUnscaled(bitmap, x, y);// DrawImage(bitmap, x, y);
        }

        public void UpdateLines()
        {
            fADDRS = new ushort[fLineCount];
            fBytesDATAS = new byte[fLineCount][];

            ushort CurADDR = fTopAddress;
            for (int i = 0; i < fLineCount; i++)
            {
                fADDRS[i] = CurADDR;

                if (GetData != null)
                    GetData(this, CurADDR, fColCount, out fBytesDATAS[i]);
                else
                {
                    fBytesDATAS[i] = new byte[fColCount];
                    for (int j = 0; j < fColCount; j++)
                        fBytesDATAS[i][j] = (byte)((fTopAddress + i * fColCount + j) & 0xFF);
                }
                CurADDR += (ushort)fColCount;
            }
        }

        private void ControlUp()
        {
            fActiveLine--;
            if (fActiveLine < 0)
            {
                fActiveLine++;
                fTopAddress -= (ushort)fColCount;
                UpdateLines();
            }
        }

        private void ControlDown()
        {
            fActiveLine++;
            if (fActiveLine >= fVisibleLineCount)
            {
                fTopAddress += (ushort)fColCount;
                fActiveLine--;
                UpdateLines();
            }
        }

        private void ControlLeft()
        {
            fActiveColumn--;
            if (fActiveColumn < 0)
            {
                fActiveColumn = fColCount - 1;
                ControlUp();
            }
            else
                UpdateLines();
        }

        private void ControlRight()
        {
            fActiveColumn++;
            if (fActiveColumn >= fColCount)
            {
                fActiveColumn = 0;
                ControlDown();
            }
            else
                UpdateLines();
        }

        private void ControlPageUp()
        {
            fTopAddress -= (ushort)(fColCount * fVisibleLineCount);
            UpdateLines();
        }

        private void ControlPageDown()
        {
            fTopAddress += (ushort)(fColCount * fVisibleLineCount);
            UpdateLines();
        }


        protected override void OnMouseDown(MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                int nl = (e.Y - 1) / fLineHeight;
                if ((nl < fVisibleLineCount) && (nl >= 0))
                {
                    if (nl != fActiveLine)
                    {
                        fActiveLine = nl;
                        Invalidate();
                    }
                }
                int nc;
                if (wd >= 0)
                {
                    nc = ((e.X - 1) - (fGutterWidth + wsp + wa + wtab));
                    if (nc >= 0) nc /= wd;
                    else nc = -1;
                }
                else
                    nc = 0;
                if ((nc < fColCount) && (nc >= 0))
                {
                    if (nc != fActiveColumn)
                    {
                        fActiveColumn = nc;
                        Invalidate();
                    }
                }
                mouseTimer.Enabled = true;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                int nl = (e.Y - 1) / fLineHeight;
                if ((nl < fVisibleLineCount) && (nl >= 0))
                {
                    if (nl != fActiveLine)
                    {
                        fActiveLine = nl;
                        Invalidate();
                    }
                }
                int nc;
                if (wd >= 0)
                {
                    nc = ((e.X - 1) - (fGutterWidth + wsp + wa + wtab));
                    if (nc >= 0) nc /= wd;
                    else nc = -1;
                }
                else
                    nc = 0;
                if ((nc < fColCount) && (nc >= 0))
                {
                    if (nc != fActiveColumn)
                    {
                        fActiveColumn = nc;
                        Invalidate();
                    }
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            int delta = -e.Delta / 120;  //WHEEL_DELTA=120
            if (delta < 0)
                for (int i = 0; i < -delta; i++)
                    ControlUp();
            else
                for (int i = 0; i < delta; i++)
                    ControlDown();
            Invalidate();

            base.OnMouseWheel(e);
        }

        protected override void OnMouseCaptureChanged(EventArgs e)
        {
            mouseTimer.Enabled = false;
            base.OnMouseCaptureChanged(e);
        }

        private void OnMouseTimer(object sender, EventArgs e)
        {
            Point mE = PointToClient(MousePosition);
            int nl = (mE.Y - 1);
            if (nl < 0)
            {
                ControlUp();
                Invalidate();
            }
            if (nl >= (fVisibleLineCount * fLineHeight))
            {
                ControlDown();
                Invalidate();
            }

            int nc;
            if (wd >= 0)
            {
                nc = ((mE.X - 1) - (fGutterWidth + wsp + wa + wtab));
                if (nc >= 0) nc /= wd;
                else nc = -1;
            }
            else
                nc = 0;
            if ((nc < fColCount) && (nc >= 0))
            {
                if (nc != fActiveColumn)
                {
                    fActiveColumn = nc;
                    Invalidate();
                }
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) != 0)
            {
                int nl = e.Y / fLineHeight;
                int nc;
                if (wd >= 0)
                    nc = ((e.X - 1) - (fGutterWidth + wsp + wa + wtab)) / wd;
                else
                    nc = 0;
                if ((nl < fVisibleLineCount) && (nl >= 0))
                {
                    if ((nc < fColCount) && (nc >= 0))
                    {
                        if (DataClick != null)
                            DataClick(this, (ushort)(fADDRS[nl] + nc));
                        UpdateLines();
                        Refresh();
                    }
                }
            }
            base.OnMouseDoubleClick(e);
        }


        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Invalidate();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            Keys keys1 = keyData & Keys.KeyCode;
            switch (keys1)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Right:
                case Keys.Down:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Down:
                    ControlDown();
                    Invalidate();
                    break;
                case Keys.Up:
                    ControlUp();
                    Invalidate();
                    break;
                case Keys.Left:
                    ControlLeft();
                    Invalidate();
                    break;
                case Keys.Right:
                    ControlRight();
                    Invalidate();
                    break;
                case Keys.PageDown:
                    ControlPageDown();
                    Invalidate();
                    break;
                case Keys.PageUp:
                    ControlPageUp();
                    Invalidate();
                    break;
                case Keys.Enter:
                    if (fVisibleLineCount > 0)
                    {
                        if (DataClick != null)
                            DataClick(this, (ushort)(fADDRS[fActiveLine] + fActiveColumn));
                    }
                    UpdateLines();
                    Refresh();
                    break;
            }
            base.OnKeyDown(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int lh = (int)e.Graphics.MeasureString("3D,", this.Font).Height;
            int lc = ((Height - 2) / lh);
            if (lc < 0) lc = 0;
            if ((fVisibleLineCount != lc) || (fLineHeight != lh))
            {
                fLineHeight = lh;
                fVisibleLineCount = lc;
                UpdateLines();
            }
            // chk...
            if ((fActiveLine >= fVisibleLineCount) && (fVisibleLineCount > 0))
            {
                fActiveLine = fVisibleLineCount - 1;
                //            Invalidate();
            }
            else if ((fActiveLine < 0) && (fVisibleLineCount > 0))
            {
                fActiveLine = 0;
                //            Invalidate();
            }

            DrawLines(e.Graphics, 0, 0, ClientRectangle.Width, ClientRectangle.Height);// Width - 2, Height - 2);
        }

        static char[] zxencode = new char[256]
        {
         ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',  // 00..0F
         ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',  // 10..1F
         ' ','!','"','#','$','%','&','\'','(',')','*','+',',','-','.','/', // 20..2F
         '0','1','2','3','4','5','6','7','8','9',':',';','<','=','>','?',  // 30..3F
         '@','A','B','C','D','E','F','G','H','I','J','K','L','M','N','O',  // 40..4F
         'P','Q','R','S','T','U','V','W','X','Y','Z','[','\\',']','↑','_', // 50..5F
         '₤','a','b','c','d','e','f','g','h','i','j','k','l','m','n','o',  // 60..6F
         'p','q','r','s','t','u','v','w','x','y','z','{','|','}','~','©',  // 70..7F

         ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',  // 80..8F
         ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',  // 90..9F
         ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',  // A0..AF
         ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',  // B0..BF
         ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',  // C0..CF
         ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',  // D0..DF
         ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',  // E0..EF
         ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',  // F0..FF
        };
    }
}