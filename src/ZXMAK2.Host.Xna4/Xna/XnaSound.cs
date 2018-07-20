using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework.Audio;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Xna4.Tools;


namespace ZXMAK2.Host.Xna4.Xna
{
    public unsafe class XnaSound : IHostSound, IDisposable
    {
        private readonly DynamicSoundEffectInstance m_soundEffect;
        private readonly AutoResetEvent m_frameEvent = new AutoResetEvent(false);
        private readonly ManualResetEvent m_waitEvent = new ManualResetEvent(true);
        private readonly int m_bufferLength;
        private readonly int m_sampleRate;
        private readonly Queue<byte[]> m_playQueue = new Queue<byte[]>();
        private readonly Queue<byte[]> m_fillQueue = new Queue<byte[]>();
        private bool m_isCancel;
        private bool m_isDisposed;


        public XnaSound(int sampleRate, int bufferCount)
        {
            const int frameRate = 50;
            const int channelCount = 2;
            if ((sampleRate % frameRate) != 0)
            {
                throw new ArgumentOutOfRangeException("sampleRate", "Sample rate must be a multiple of 50!");
            }
            m_sampleRate = sampleRate;
            m_bufferLength = (sampleRate / frameRate) * channelCount * 2;

            m_soundEffect = new DynamicSoundEffectInstance(
                sampleRate, 
                channelCount == 1 ? AudioChannels.Mono : AudioChannels.Stereo);
            
            var needSize = m_soundEffect.GetSampleSizeInBytes(
                TimeSpan.FromMilliseconds(20));
            var bufferCountReal = needSize * bufferCount / m_bufferLength;
            if (bufferCountReal < bufferCount)
            {
                bufferCountReal = bufferCount;
            }
            for (var i = 0; i < bufferCountReal; i++)
            {
                m_fillQueue.Enqueue(new byte[m_bufferLength]);
            }
            m_soundEffect.BufferNeeded += SoundEffect_OnBufferNeeded;
            m_soundEffect.Play();
        }

        public void Dispose()
        {
            if (m_isDisposed)
            {
                return;
            }
            m_isDisposed = true;
            m_soundEffect.Stop();
            CancelWait();
            m_soundEffect.Dispose();
            m_waitEvent.Dispose();
            m_frameEvent.Dispose();
        }

        
        #region IHostSound

        public int SampleRate
        {
            get { return m_sampleRate; }
        }

        public bool IsSyncSupported
        {
            get { return true; }
        }

        public bool IsSynchronized { get; set; }

        private void WaitFrame()
        {
            m_waitEvent.Reset();
            try
            {
                if (m_isCancel)
                {
                    return;
                }
                lock (m_soundEffect)
                {
                    if (m_fillQueue.Count > 0)
                    {
                        return;
                    }
                }
                m_frameEvent.WaitOne(100);
            }
            finally
            {
                m_waitEvent.Set();
            }
        }

        public void CancelWait()
        {
            m_isCancel = true;
            Thread.MemoryBarrier();
            m_waitEvent.WaitOne();
            m_isCancel = false;
            Thread.MemoryBarrier();
        }

        public void PushFrame(IFrameInfo info, IFrameSound frame)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            if (frame == null)
            {
                throw new ArgumentNullException("frame");
            }
            if (IsSynchronized)
            {
                WaitFrame();
            }
            var buffer = LockBuffer();
            if (buffer == null)
            {
                return;
            }
            var srcBuffer = frame.GetBuffer();
            fixed (uint* pSrc = srcBuffer)
            fixed (byte* pbDst = buffer)
            {
                NativeMethods.CopyMemory((uint*)pbDst, pSrc, buffer.Length);
            }
            UnlockBuffer(buffer);
        }

        #endregion IHostSound


        #region Private

        private byte[] LockBuffer()
        {
            lock (m_soundEffect)
            {
                if (m_fillQueue.Count == 0)
                {
                    return null;
                }
                return m_fillQueue.Dequeue();
            }
        }

        private void UnlockBuffer(byte[] buffer)
        {
            lock (m_soundEffect)
            {
                m_soundEffect.SubmitBuffer(buffer);
                m_playQueue.Enqueue(buffer);
            }
        }

        private void SoundEffect_OnBufferNeeded(object sender, EventArgs e)
        {
            m_frameEvent.Set();
            lock (m_soundEffect)
            {
                if (m_playQueue.Count == 0)
                {
                    return;
                }
                m_fillQueue.Enqueue(m_playQueue.Dequeue());
            }
        }

        #endregion Private
    }
}
