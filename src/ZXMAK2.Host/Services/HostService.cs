using System;
using System.Linq;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;


namespace ZXMAK2.Host.Services
{
    public class HostService : IHostService
    {
        #region Fields

        private SyncSource m_syncSource;
        private TimeSync m_timeSync;
        private IHostVideo m_video;
        private IHostSound m_sound;
        private IHostKeyboard m_keyboard;
        private IHostMouse m_mouse;
        private IHostJoystick m_joystick;
        private IMediaRecorder m_mediaRecorder;

        #endregion Fields


        #region .ctor

        public HostService(
            IHostVideo hostVideo,
            IHostSound hostSound,
            IHostKeyboard hostKeyboard,
            IHostMouse hostMouse,
            IHostJoystick hostJoystick)
        {
            m_video = hostVideo;
            m_sound = hostSound;
            m_keyboard = hostKeyboard;
            m_mouse = hostMouse;
            m_joystick = hostJoystick;
            m_timeSync = new TimeSync();
            UpdateSyncSource();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Dispose(ref m_timeSync);
                Dispose(ref m_sound);
                Dispose(ref m_keyboard);
                Dispose(ref m_mouse);
                Dispose(ref m_joystick);
                Dispose(ref m_mediaRecorder);
            }
        }

        #endregion .ctor


        #region IHost

        public IHostKeyboard Keyboard { get { return m_keyboard; } }
        public IHostMouse Mouse { get { return m_mouse; } }
        public IHostJoystick Joystick { get { return m_joystick; } }

        public SyncSource SyncSource 
        {
            get { return m_syncSource; }
            set
            {
                m_syncSource = value;
                UpdateSyncSource();
            }
        }

        public int SampleRate 
        {
            get { return m_sound != null ? m_sound.SampleRate : 22050; }
        }


        public bool IsCaptured
        {
            get { return m_mouse != null && m_mouse.IsCaptured; }
        }

        public IMediaRecorder MediaRecorder 
        {
            get { return m_mediaRecorder; }
            set { m_mediaRecorder = value; }
        }


        public bool CheckSyncSourceSupported(SyncSource value)
        {
            switch (value)
            {
                case SyncSource.None:
                    return true;
                case SyncSource.Time:
                    var timeSync = m_timeSync;
                    return timeSync != null && timeSync.IsSyncSupported;
                case SyncSource.Sound:
                    var sound = m_sound;
                    return sound != null && sound.IsSyncSupported;
                case SyncSource.Video:
                    var video = m_video;
                    return video != null && video.IsSyncSupported;
                default:
                    return false;
            }
        }

        public void PushFrame(
            IFrameInfo infoFrame,
            IFrameVideo videoFrame,
            IFrameSound soundFrame)
        {
            var timeSync = m_timeSync;
            var sound = m_sound;
            var video = m_video;
            if (infoFrame.IsRefresh)
            {
                // request from UI, so we don't need sound and sync
                if (video != null && videoFrame != null)
                {
                    video.PushFrame(infoFrame, videoFrame);
                }
                return;
            }
            if (SyncSource == SyncSource.Time && timeSync != null)
            {
                timeSync.WaitFrame();
            }
            if (video != null && videoFrame != null)
            {
                video.PushFrame(infoFrame, videoFrame);
            }
            if (sound != null && soundFrame != null)
            {
                sound.PushFrame(infoFrame, soundFrame);
            }
            var mediaRecorder = MediaRecorder;
            if (mediaRecorder != null && 
                videoFrame != null && 
                soundFrame != null)
            {
                mediaRecorder.PushFrame(infoFrame, videoFrame, soundFrame);
            }
        }

        public void CancelPush()
        {
            var timeSync = m_timeSync;
            if (timeSync != null)
            {
                timeSync.CancelWait();
            }
            var video = m_video;
            if (video != null)
            {
                video.CancelWait();
            }
            var sound = m_sound;
            if (sound != null)
            {
                sound.CancelWait();
            }
        }

        public void Capture()
        {
            if (m_mouse == null)
            {
                return;
            }
            m_mouse.Capture();
        }

        public void Uncapture()
        {
            if (m_mouse == null)
            {
                return;
            }
            m_mouse.Uncapture();
        }

        #endregion IHost


        #region Private

        private void UpdateSyncSource()
        {
            var sound = m_sound;
            if (sound != null)
            {
                sound.IsSynchronized = m_syncSource == SyncSource.Sound;
            }
            var video = m_video;
            if (video != null)
            {
                video.IsSynchronized = m_syncSource == SyncSource.Video;
            }
        }

        private static void Dispose<T>(ref T fieldRef)
            where T : IDisposable
        {
            var value = fieldRef;
            if (value == null)
            {
                return;
            }
            fieldRef = default(T);
            value.Dispose();
        }

        #endregion Private
    }
}
