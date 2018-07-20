using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Presentation.Interfaces;
using ZXMAK2.Host.WinForms.Mdx;
using ZXMAK2.Host.WinForms.Mdx.Renderers;
using ZXMAK2.Host.WinForms.Tools;

namespace ZXMAK2.Host.WinForms.Controls
{
    public class RenderVideo : Control, IHostVideo
    {
        private readonly IAllocatorPresenter _allocator;
        private readonly VideoRenderer _videoLayer;
        private readonly OsdRenderer _osdLayer;
        private readonly IconRenderer _iconLayer;
        private readonly FrameResampler _frameResampler = new FrameResampler(50);
        private readonly AutoResetEvent _frameEvent = new AutoResetEvent(false);
        private readonly AutoResetEvent _cancelEvent = new AutoResetEvent(false);
        private Bitmap _slowSurface;

        #region .ctor

        public RenderVideo()
        {
            SetStyle(
                ControlStyles.Opaque | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, 
                true);

            var allocator = new AllocatorPresenter();
            _allocator = allocator;
            _videoLayer = new VideoRenderer(allocator);
            _osdLayer = new OsdRenderer(allocator);
            _iconLayer = new IconRenderer(allocator);
            _videoLayer.IsVisible = true;

            _allocator.Register(_videoLayer);
            _allocator.Register(_osdLayer);
            _allocator.Register(_iconLayer);

            _allocator.PresentCompleted += AllocatorPresenter_OnPresentCompleted;

            IsSyncSupported = true;
            VideoFilter = VideoFilter.None;
            ScaleMode = ScaleMode.FixedPixelSize;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _allocator.Dispose();
                _frameEvent.Dispose();
                _cancelEvent.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion .ctor


        #region Properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MimicTv
        {
            get { return _videoLayer.MimicTv; }
            set { _videoLayer.MimicTv = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AntiAlias
        {
            get { return _videoLayer.AntiAlias; }
            set { _videoLayer.AntiAlias = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ScaleMode ScaleMode
        {
            get { return _videoLayer.ScaleMode; }
            set { _videoLayer.ScaleMode = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public VideoFilter VideoFilter
        {
            get { return _videoLayer.VideoFilter; }
            set { _videoLayer.VideoFilter = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DebugInfo
        {
            get { return _osdLayer.IsVisible; }
            set { _osdLayer.IsVisible = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DisplayIcon 
        {
            get { return _iconLayer.IsVisible; }
            set { _iconLayer.IsVisible = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSyncSupported { get; private set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Size FrameSize
        {
            get { return _osdLayer.FrameSize; }
            private set { _osdLayer.FrameSize = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsRunning
        {
            get { return _osdLayer.IsRunning; }
            set { _osdLayer.IsRunning = value; }
        }

        public void InitWnd()
        {
            try
            {
                _allocator.Attach(Handle);
            }
            catch (Exception ex)    // process error on Mono
            {
                Logger.Error(ex);
            }
        }

        public void FreeWnd()
        {
            _allocator.Dispose();
        }

        #endregion Properties


        #region IHostVideo

        public bool IsSynchronized { get; set; }

        public void CancelWait()
        {
            _cancelEvent.Set();
        }

        public void PushFrame(IFrameInfo info, IFrameVideo frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame");
            }
            if (_frameResampler.SourceRate > 0 && IsSynchronized && !info.IsRefresh)
            {
                do
                {
                    var waitEvents = new[] 
                    { 
                        _frameEvent, 
                        _cancelEvent 
                    };
                    if (WaitHandle.WaitAny(waitEvents) != 0)
                    {
                        return;
                    }
                } while (!_frameResampler.Next());
            }
            _osdLayer.FrameStartTact = info.StartTact;
            _osdLayer.SampleRate = info.SampleRate;
            if (!info.IsRefresh)
            {
                _osdLayer.UpdateFrame(info.UpdateTime);
            }
            FrameSize = new Size(
                frame.Size.Width,
                (int)(frame.Size.Height * frame.Ratio + 0.5F));
            _videoLayer.Update(frame);
            _iconLayer.Update(info.Icons);
            
            // slow GDI rendering...
            if (!_allocator.IsRendering)
            {
                UpdateGdi(frame);
            }
        }

        private readonly object _slowRenderSync = new object();

        private void UpdateGdi(IFrameVideo frame)
        {
            lock (_slowRenderSync)
            {
                if (_slowSurface == null || _slowSurface.Size != frame.Size)
                {
                    if (_slowSurface != null)
                    {
                        _slowSurface.Dispose();
                        _slowSurface = null;
                    }
                    _slowSurface = new Bitmap(frame.Size.Width, frame.Size.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                }
                //var im = new Bitmap(columns, rows, stride,
                //                            PixelFormat.Format8bppIndexed,
                //                            Marshal.UnsafeAddrOfPinnedArrayElement(newbytes, 0));
                var data = _slowSurface.LockBits(
                    new Rectangle(0, 0, frame.Size.Width, frame.Size.Height),
                    System.Drawing.Imaging.ImageLockMode.WriteOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                try
                {
                    unsafe
                    {
                        var stride = data.Stride;
                        int* pDst = (int*)data.Scan0;
                        fixed (int* pSrc = frame.Buffer)
                        {
                            for (var y = 0; y < frame.Size.Height; y++)
                            {
                                var offsetSrc = y * frame.Size.Width;
                                var offsetDst = y * frame.Size.Width;
                                for (var x = 0; x < frame.Size.Width; x++)
                                {
                                    pDst[offsetDst + x] = pSrc[offsetSrc + x];
                                }
                            }
                        }
                    }
                }
                finally
                {
                    _slowSurface.UnlockBits(data);
                }
            }
            Invalidate();
        }

        private void RenderGdi(Graphics g, Rectangle rect)
        {
            lock (_slowRenderSync)
            {
                if (_slowSurface != null)
                {
                    g.InterpolationMode = AntiAlias ? System.Drawing.Drawing2D.InterpolationMode.Bilinear : System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    
                    var drawRect = Rectangle.Round(ScaleHelper.GetDestinationRect(ScaleMode, rect.Size, _slowSurface.Size));
                    drawRect = new Rectangle(
                        drawRect.Left + rect.Left, 
                        drawRect.Top + rect.Top, 
                        drawRect.Width, 
                        drawRect.Height);
                    g.DrawImage(_slowSurface, drawRect);
                    
                    var region = new Region(rect);
                    region.Exclude(drawRect);
                    g.FillRegion(Brushes.Black, region);
                }
            }
        }

        #endregion IHostVideo

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_allocator.IsRendering)
            {
                return;
            }
            var padding = 5;
            var textHeight = 0;
            using (var font = new System.Drawing.Font(Font.FontFamily, 10))
            {
                var iconWidth = SystemIcons.Warning.Width;
                var iconHeight = SystemIcons.Warning.Height;
                var width = ClientSize.Width - (padding + iconWidth + padding);
                var height = ClientSize.Height - (padding + padding);
                width = width < padding ? padding : width;
                height = height < padding ? padding : height;
                var text = _allocator.ErrorMessage ?? "Loading...";
                var size = e.Graphics.MeasureString(
                    text,
                    font,
                    width);
                textHeight = (int)(size.Height + 0.5F);
                textHeight = textHeight < iconHeight ? iconHeight : textHeight;
                textHeight += padding * 2;
                var fillRect = new Rectangle(0, 0, ClientSize.Width, textHeight);
                var textRect = new Rectangle(
                    padding + iconWidth + padding,
                    padding, 
                    width, 
                    textHeight);
                e.Graphics.FillRectangle(Brushes.Black, fillRect);
                e.Graphics.DrawImage(SystemIcons.Warning.ToBitmap(), new Point(padding, padding));
                e.Graphics.DrawString(
                    text,
                    font,
                    Brushes.White,
                    textRect);
            }
            var spaceRect = new Rectangle(new Point(0, textHeight), new Size(ClientSize.Width, ClientSize.Height-textHeight));
            RenderGdi(e.Graphics, spaceRect);
        }

        private void AllocatorPresenter_OnPresentCompleted(object sender, EventArgs e)
        {
            _osdLayer.UpdatePresent();
            _frameEvent.Set();
            _frameResampler.SourceRate = _allocator.RefreshRate;
        }
    }
}
