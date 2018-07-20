using System;
using System.IO;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;


namespace ZXMAK2.Hardware.Sprinter
{
    public class SprinterRenderer : IUlaRenderer
    {
        public SprinterRenderer()
        {
            VideoData = new FrameVideo(640, 256, 2F);
        }
        
        public IFrameVideo VideoData { get; private set; }
        
        public int FrameLength
        {
            get { return 0x11800 * 6; } //6 - 21MHz
        }

        public int IntLength
        {
            get { return 0x20; }
        }

        public void UpdateBorder(int value)
        {
        }

        public void UpdatePalette(int index, uint value)
        {
        }

        public void ReadFreeBus(int frameTact, ref byte value)
        {
        }

        public unsafe void Render(uint* bufPtr, int startTact, int endTact)
        {
        }

        public void Frame()
        {
        }

        public void LoadScreenData(Stream stream)
        {
        }

        public void SaveScreenData(Stream stream)
        {
        }

        public IUlaRenderer Clone()
        {
            var renderer = new SprinterRenderer();
            return renderer;
        }
    }
}
