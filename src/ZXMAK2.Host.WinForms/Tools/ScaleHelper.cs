using System;
using System.Drawing;
using ZXMAK2.Host.Presentation.Interfaces;


namespace ZXMAK2.Host.WinForms.Tools
{
    public static class ScaleHelper
    {
        /// <summary>
        /// Calculate destination rect according to scale mode
        /// </summary>
        /// <param name="scaleMode">Scale mode</param>
        /// <param name="wndSize">Available size</param>
        /// <param name="dstSize">Requested size</param>
        /// <returns></returns>
        public static RectangleF GetDestinationRect(
            ScaleMode scaleMode, 
            SizeF wndSize,
            SizeF dstSize)
        {
            if (dstSize.Width <= 0 || dstSize.Height <= 0)
            {
                return new RectangleF(new PointF(0, 0), dstSize);
            }
            var rx = wndSize.Width / dstSize.Width;
            var ry = wndSize.Height / dstSize.Height;
            if (scaleMode == ScaleMode.SquarePixelSize)
            {
                rx = (float)Math.Floor(rx);
                ry = (float)Math.Floor(ry);
                rx = ry = Math.Min(rx, ry);
                rx = rx < 1F ? 1F : rx;
                ry = ry < 1F ? 1F : ry;
            }
            if (scaleMode == ScaleMode.FixedPixelSize)
            {
                rx = (float)Math.Floor(rx);
                ry = (float)Math.Floor(ry);
                rx = rx < 1F ? 1F : rx;
                ry = ry < 1F ? 1F : ry;
            }
            else if (scaleMode == ScaleMode.KeepProportion)
            {
                if (rx > ry)
                {
                    rx = (wndSize.Width * ry / rx) / dstSize.Width;
                }
                else if (rx < ry)
                {
                    ry = (wndSize.Height * rx / ry) / dstSize.Height;
                }
            }
            dstSize = new SizeF(
                dstSize.Width * rx,
                dstSize.Height * ry);
            var dstPos = new PointF(
                (float)Math.Floor((wndSize.Width - dstSize.Width) / 2F),
                (float)Math.Floor((wndSize.Height - dstSize.Height) / 2F));
            return new RectangleF(dstPos, dstSize);
        }

        public static int GetPotSize(int size)
        {
            // Create POT texture (e.g. 512x512) to render NPOT image (e.g. 320x240),
            // because NPOT textures is not supported on some videocards
            var potSize = 0;
            for (var power = 1; potSize < size; power++)
            {
                potSize = Pow(2, power);
            }
            return potSize;
        }

        private static int Pow(int value, int power)
        {
            var result = value;
            for (var i = 0; i < power; i++)
            {
                result *= value;
            }
            return result;
        }
    }
}
