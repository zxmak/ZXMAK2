using System;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;
using AForge.Video.FFMPEG;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Media.Tools;
using ZXMAK2.Logging;
using System.Reflection;


namespace ZXMAK2.Host.Media
{
    public sealed class VideoRecorder : IMediaRecorder
    {
        private readonly string _fileName;
        private readonly int _width;
        private readonly int _height;
        private readonly VideoFileWriter _writer;
        private readonly Bitmap _bitmap;
        private Bitmap _bitmapRaw;
        private readonly AutoResetEvent _eventFrame = new AutoResetEvent(false);
        private readonly AutoResetEvent _eventCancel = new AutoResetEvent(false);
        private readonly ConcurrentQueue<MediaFrame> _queue = new ConcurrentQueue<MediaFrame>();
        private Thread _threadRecord;


        public VideoRecorder(string fileName, int width, int height)
        {
            CheckUnmanagedReference("avcodec-53.dll");
            CheckUnmanagedReference("avformat-53.dll");
            CheckUnmanagedReference("avutil-51.dll");
            CheckUnmanagedReference("swscale-2.dll");

            _fileName = Path.GetFullPath(fileName);
            _width = width;
            _height = height;
            _writer = new VideoFileWriter();
            try
            {
                var ext = Path.GetExtension(_fileName).ToUpperInvariant();
                var codec = GetCodec(ext);
                _writer.Open(_fileName, _width, _height, 50, codec, 200000000);
                _bitmap = new Bitmap(_width, _height, PixelFormat.Format32bppRgb);
            }
            catch
            {
                _writer.Dispose();
                throw;
            }
            _threadRecord = new Thread(RecordProc);
            _threadRecord.Name = "Record";
            _threadRecord.IsBackground = true;
            _threadRecord.Start();
        }

        private VideoCodec GetCodec(string ext)
        {
            if (ext == ".MPEG")
            {
                return VideoCodec.MPEG2;
            }
            if (ext == ".MP4")
            {
                return VideoCodec.MPEG4;
            }
            return VideoCodec.MPEG4;
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
            _bitmap.Dispose();
            if (_bitmapRaw != null)
            {
                _bitmapRaw.Dispose();
            }
            _eventFrame.Dispose();
            _eventCancel.Dispose();
            _writer.Close();
        }

        public void PushFrame(
            IFrameInfo infoFrame,
            IFrameVideo videoFrame, 
            IFrameSound soundFrame)
            //IVideoFrame videoFrame, ISoundFrame soundFrame)
        {
            if (_threadRecord == null)
            {
                return;
            }
            var frame = new MediaFrame();
            frame.Width = videoFrame.Size.Width;
            frame.Height = videoFrame.Size.Height;
            frame.Ratio = videoFrame.Ratio;
            frame.Image = new int[frame.Width * frame.Height];
            Array.Copy(videoFrame.Buffer, frame.Image, frame.Image.Length);
            
            frame.SampleRate = soundFrame.SampleRate;
            //frame.Audio = new uint[frame.SampleRate / 50];
            //Array.Copy(soundFrame.Buffer, frame.Audio, frame.Audio.Length);
            _queue.Enqueue(frame);
            _eventFrame.Set();
        }

        private void RecordProc()
        {
            try
            {
                MediaFrame frame;
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

        private void WriteFrame(MediaFrame frame)
        {
            MakeBitmapRaw(frame);
            if (_bitmapRaw.Width == _width &&
                _bitmapRaw.Height == _height)
            {
                _writer.WriteVideoFrame(_bitmapRaw);
            }
            else
            {
                using (var g = Graphics.FromImage(_bitmap))
                {
                    var srcRect = new Rectangle(0, 0, _bitmapRaw.Width, _bitmapRaw.Height);
                    var dstRect = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);
                    g.DrawImage(_bitmapRaw, dstRect, srcRect, GraphicsUnit.Pixel);
                }
                _writer.WriteVideoFrame(_bitmap);
            }
        }

        private void MakeBitmapRaw(MediaFrame frame)
        {
            if (_bitmapRaw != null &&
                (_bitmapRaw.Width != frame.Width || _bitmapRaw.Height != frame.Height))
            {
                _bitmapRaw.Dispose();
                _bitmapRaw = null;
            }
            if (_bitmapRaw == null)
            {
                _bitmapRaw = new Bitmap(frame.Width, frame.Height, PixelFormat.Format32bppRgb);
            }
            var rect = new Rectangle(0, 0, _bitmapRaw.Width, _bitmapRaw.Height);
            var data = _bitmapRaw.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            var height = _bitmapRaw.Height;
            unsafe
            {
                var lineSize = frame.Width << 2;
                var strideWords = data.Stride >> 2;
                var pDst = (int*)data.Scan0.ToPointer();
                fixed (int* pSrc = frame.Image)
                {
                    var pSrcPtr = pSrc;
                    var pDstPtr = pDst;
                    for (var y = 0; y < height; y++)
                    {
                        NativeMethods.CopyMemory(pDstPtr, pSrcPtr, lineSize);
                        pSrcPtr += frame.Width;
                        pDstPtr += strideWords;
                    }
                }
            }
            _bitmapRaw.UnlockBits(data);
        }

        private class MediaFrame
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public float Ratio { get; set; }
            public int SampleRate { get; set; }
            public int[] Image { get; set; }
            public uint[] Audio { get; set; }
        }

        private static void CheckUnmanagedReference(string fileName)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!File.Exists(Path.Combine(path, fileName)))
            {
                throw new FileNotFoundException(string.Format("{0} is missing!", fileName));
            }
        }
    }
}
