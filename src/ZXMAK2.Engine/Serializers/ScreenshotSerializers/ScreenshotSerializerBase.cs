using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Serializers.ScreenshotSerializers
{
    public abstract class ScreenshotSerializerBase : FormatSerializer
    {
        protected IUlaDevice m_ulaDevice;

        public ScreenshotSerializerBase(IUlaDevice ulaDevice)
		{
            m_ulaDevice = ulaDevice;
		}

        #region FormatSerializer

        public override string FormatGroup { get { return "Screenshots"; } }
        public override string FormatName { get { return string.Format("{0} screenshot", FormatExtension); } }

        public override bool CanSerialize { get { return true; } }

        public unsafe override void Serialize(Stream stream)
        {
            var videoData = m_ulaDevice.VideoData;
            using (var bmp = new Bitmap(videoData.Size.Width, videoData.Size.Height, PixelFormat.Format32bppArgb))
            {
                var size = videoData.Size;
                var bmpData = bmp.LockBits(new Rectangle(0, 0, size.Width, size.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
                try
                {
                    Marshal.Copy(videoData.Buffer, 0, bmpData.Scan0, size.Width * size.Height);
                }
                finally
                {
                    bmp.UnlockBits(bmpData);
                }

                var sizeScale = new Size(
                    size.Width, 
                    (int)((float)size.Height * videoData.Ratio));
                using (var bmpScale = new Bitmap(sizeScale.Width, sizeScale.Height, bmp.PixelFormat))
                {
                    using (var g = Graphics.FromImage(bmpScale))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                        // -0.5 pixel shift to avoid shift after scale (looks like bug in Graphics)
                        var dstRect = new RectangleF(-0.5F, -0.5F, sizeScale.Width, sizeScale.Height);
                        var srcRect = new RectangleF(-0.5F, -0.5F, size.Width, size.Height);
                        g.DrawImage(bmp, dstRect, srcRect, GraphicsUnit.Pixel);
                    }
                    Save(stream, bmpScale);
                }
            }
        }

        #endregion

        protected abstract void Save(Stream stream, Bitmap bmp);
    }
}
