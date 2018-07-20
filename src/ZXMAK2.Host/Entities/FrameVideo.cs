using System;
using System.Drawing;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Host.Entities
{
    public class FrameVideo : IFrameVideo
    {
        public FrameVideo(Size size, float ratio)
        {
            Buffer = new int[size.Width * size.Height];
            Size = size;
            Ratio = ratio;
        }

        public FrameVideo(int width, int height, float ratio)
            : this (new Size(width, height), ratio)
        {
        }
        
        public int[] Buffer { get; private set; }
        public Size Size { get; private set; }
        public float Ratio { get; private set; }
    }
}
