

using System;
namespace ZXMAK2.Host.WinForms.Tools
{
    public class FrameResampler
    {
        private readonly double _targetRate;
        private double _sourceRate;
        private double _ratio;
        

        public FrameResampler(double targetRate)
        {
            _targetRate = targetRate;
        }

        public double SourceRate
        {
            get { return _sourceRate; }
            set 
            {
                if (_sourceRate == value)
                {
                    return;
                }
                _sourceRate = value;
                _ratio = _targetRate / _sourceRate;
            }
        }

        public double Time { get; private set; }

        /// <summary>
        /// Switch to the next source frame, returns true when target frame should be updated
        /// </summary>
        public bool Next()
        {
            var time = Time + _ratio;
            var isSkipped = time < 1D;
            if (time >= 1D)
            {
                time -= Math.Floor(time);
                if (time >= 1D)
                {
                    time = 1D;
                }
            }
            Time = time;
            return !isSkipped;
        }
    }
}
