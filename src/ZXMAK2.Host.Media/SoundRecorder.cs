using ZXMAK2.Host.Interfaces;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;
using ZXMAK2.Host.Media.Audio;
using System;


namespace ZXMAK2.Host.Media
{
    public class SoundRecorder : IMediaRecorder
    {
        private Thread _threadRecord;
        private readonly string _fileName;
        private readonly int _sampleRate;
        private readonly WavSampleWriter _writer;
        private readonly AutoResetEvent _eventFrame = new AutoResetEvent(false);
        private readonly AutoResetEvent _eventCancel = new AutoResetEvent(false);
        private readonly ConcurrentQueue<uint[]> _queue = new ConcurrentQueue<uint[]>();



        public SoundRecorder(string fileName, int sampleRate)
        {
            _fileName = Path.GetFullPath(fileName);
            _sampleRate = sampleRate;
            _writer = new WavSampleWriter(fileName, sampleRate);

            _threadRecord = new Thread(RecordProc);
            _threadRecord.Name = "Record";
            _threadRecord.IsBackground = true;
            _threadRecord.Start();
        }

        public void Dispose()
        {
            var thread = _threadRecord;
            _threadRecord = null;
            if (thread == null)
            {
                return;
            }
            _eventCancel.Set();
            thread.Join();
            _eventFrame.Dispose();
            _eventCancel.Dispose();
            _writer.Dispose();
        }

        public void PushFrame(IFrameInfo info, IFrameVideo videoFrame, IFrameSound soundFrame)
        {
            if (_threadRecord == null)
            {
                return;
            }
            var bufferSrc = soundFrame.GetBuffer();
            var bufferWr = new uint[bufferSrc.Length];
            Array.Copy(bufferSrc, bufferWr, bufferSrc.Length);
            _queue.Enqueue(bufferWr);
            _eventFrame.Set();
        }

        private void RecordProc()
        {
            try
            {
                uint[] frame;
                while (_threadRecord != null)
                {
                    if (_queue.TryDequeue(out frame))
                    {
                        WriteFrame(frame);
                    }
                    else
                    {
                        WaitHandle.WaitAny(new[] { _eventFrame, _eventCancel });
                    }
                }
                // flush
                while (_queue.TryDequeue(out frame))
                {
                    WriteFrame(frame);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void WriteFrame(uint[] frame)
        {
            _writer.Write(frame, 0, frame.Length);
        }
    }
}
