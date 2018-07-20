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
using ZXMAK2.Host.Media.Audio;


namespace ZXMAK2.Host.Media
{
    public sealed class MediaRecorder : IMediaRecorder
    {
        private readonly string _fileNameMP4;
        private readonly string _fileNameWAV;
        private readonly int _width;
        private readonly int _height;
        private readonly int _sampleRate;
        private readonly VideoFileWriter _writerMP4;
        private readonly WavSampleWriter _writerWAV;
        private readonly Bitmap _bitmap;
        private Bitmap _bitmapRaw;
        private readonly AutoResetEvent _eventFrame = new AutoResetEvent(false);
        private readonly AutoResetEvent _eventCancel = new AutoResetEvent(false);
        private readonly ConcurrentQueue<MediaFrame> _queueMP4 = new ConcurrentQueue<MediaFrame>();
        private readonly ConcurrentQueue<uint[]> _queueWAV = new ConcurrentQueue<uint[]>();
        private Thread _threadRecord;


        public MediaRecorder(string fileName, int width, int height, int sampleRate)
        {
            CheckUnmanagedReference("avcodec-53.dll");
            CheckUnmanagedReference("avformat-53.dll");
            CheckUnmanagedReference("avutil-51.dll");
            CheckUnmanagedReference("swscale-2.dll");

            _fileNameMP4 = Path.GetFullPath(fileName);
            _fileNameWAV = Path.ChangeExtension(_fileNameMP4, ".wav");
            _width = width;
            _height = height;
            _sampleRate = sampleRate;
            _writerMP4 = new VideoFileWriter();
            _writerWAV = new WavSampleWriter(_fileNameWAV, sampleRate);
            try
            {
                var ext = Path.GetExtension(_fileNameMP4).ToUpperInvariant();
                var codec = GetCodec(ext);
                _writerMP4.Open(_fileNameMP4, _width, _height, 50, codec, 200000000);//200000000);
                _bitmap = new Bitmap(_width, _height, PixelFormat.Format32bppRgb);
            }
            catch
            {
                _writerMP4.Dispose();
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
            _writerMP4.Close();
            _writerWAV.Dispose();
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

            frame.SampleRate = _sampleRate;// soundFrame.SampleRate;
            //frame.Audio = new uint[frame.SampleRate / 50];
            //Array.Copy(soundFrame.Buffer, frame.Audio, frame.Audio.Length);
            _queueMP4.Enqueue(frame);

            // Process audio...
            var bufferSrc = soundFrame.GetBuffer();
            var bufferWr = new uint[bufferSrc.Length];
            Array.Copy(bufferSrc, bufferWr, bufferSrc.Length);
            _queueWAV.Enqueue(bufferWr);

            _eventFrame.Set();
        }

        private void RecordProc()
        {
            try
            {
                MediaFrame frameMP4;
                uint[] frameWAV;
                while (_threadRecord != null)
                {
                    var isWritten = false;
                    if (_queueMP4.TryDequeue(out frameMP4))
                    {
                        WriteFrameMP4(frameMP4);
                        isWritten = true;
                    }
                    if (_queueWAV.TryDequeue(out frameWAV))
                    {
                        WriteFrameWAV(frameWAV);
                        isWritten = true;
                    }

                    if (!isWritten)
                    {
                        WaitHandle.WaitAny(new[] { _eventFrame, _eventCancel });
                    }
                }
                // flush
                while (_queueMP4.TryDequeue(out frameMP4))
                {
                    WriteFrameMP4(frameMP4);
                }
                while (_queueWAV.TryDequeue(out frameWAV))
                {
                    WriteFrameWAV(frameWAV);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void WriteFrameWAV(uint[] frame)
        {
            _writerWAV.Write(frame, 0, frame.Length);
        }

        private void WriteFrameMP4(MediaFrame frame)
        {
            MakeBitmapRaw(frame);
            if (_bitmapRaw.Width == _width &&
                _bitmapRaw.Height == _height)
            {
                _writerMP4.WriteVideoFrame(_bitmapRaw);
            }
            else
            {
                using (var g = Graphics.FromImage(_bitmap))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    var srcRect = new Rectangle(0, 0, _bitmapRaw.Width, _bitmapRaw.Height);
                    var dstRect = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);
                    g.DrawImage(_bitmapRaw, dstRect, srcRect, GraphicsUnit.Pixel);
                }
                _writerMP4.WriteVideoFrame(_bitmap);
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
