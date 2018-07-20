using System;
using System.Linq;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Host.Entities
{
    public class FrameInfo : IFrameInfo
    {
        public IIconDescriptor[] Icons { get; private set; }

        public int StartTact { get; private set; }
        public double UpdateTime { get; private set; }
        public bool IsRefresh { get; private set; }
        public int SampleRate { get; private set; }

        public FrameInfo(
            IIconDescriptor[] icons, 
            int startTact,
            double instantUpdateTime,
            int sampleRate,
            bool isRefresh)
        {
            Icons = icons;
            StartTact = startTact;
            UpdateTime = instantUpdateTime;
            SampleRate = sampleRate;
            IsRefresh = isRefresh;
        }
    }
}
