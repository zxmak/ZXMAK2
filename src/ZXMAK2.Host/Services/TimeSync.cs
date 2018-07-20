using System;
using System.Threading;
using System.Diagnostics;


namespace ZXMAK2.Host.Services
{
    public sealed class TimeSync : IDisposable
    {
        private readonly ManualResetEvent _waitEvent = new ManualResetEvent(true);
        private long _lastTimeStamp;
        private bool _isCancel;


        public TimeSync()
        {
            _lastTimeStamp = Stopwatch.GetTimestamp();
        }

        public void Dispose()
        {
            CancelWait();
            _waitEvent.Dispose();
        }

        public bool IsSyncSupported
        {
            get { return true; }
        }

        public void WaitFrame()
        {
            _waitEvent.Reset();
            try
            {
                if (_isCancel)
                {
                    return;
                }
                var frequency = Stopwatch.Frequency;
                var time50 = frequency / 50;
                var stamp = Stopwatch.GetTimestamp();
                var time = stamp - _lastTimeStamp;
                if (time < time50)
                {
                    var delay = (int)(((time50 - time) * 1000) / frequency);
                    if (delay > 5 && delay < 40)
                    {
                        Thread.Sleep(delay - 1);
                    }
                }
                while (true)
                {
                    stamp = Stopwatch.GetTimestamp();
                    time = stamp - _lastTimeStamp;
                    if (time >= time50)
                    {
                        break;
                    }
                    Thread.SpinWait(1);
                }
                if (time > time50 * 2)
                {
                    // resync
                    _lastTimeStamp = stamp;
                }
                else
                {
                    _lastTimeStamp += time50;
                }
            }
            finally
            {
                _waitEvent.Set();
            }
        }

        public void CancelWait()
        {
            _isCancel = true;
            Thread.MemoryBarrier();
            _waitEvent.WaitOne();
            _isCancel = false;
            Thread.MemoryBarrier();
        }
    }
}
