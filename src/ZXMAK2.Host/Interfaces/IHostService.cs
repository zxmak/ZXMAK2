using System;
using ZXMAK2.Host.Entities;


namespace ZXMAK2.Host.Interfaces
{
    public interface IHostService : IDisposable
    {
        IHostKeyboard Keyboard { get; }
        IHostMouse Mouse { get; }
        IHostJoystick Joystick { get; }
        SyncSource SyncSource { get; set; }
        int SampleRate { get; }
        bool IsCaptured { get; }
        IMediaRecorder MediaRecorder { get; set; }


        bool CheckSyncSourceSupported(SyncSource value);
        void PushFrame(
            IFrameInfo infoFrame,
            IFrameVideo videoFrame, 
            IFrameSound soundFrame);
        void CancelPush();
        void Capture();
        void Uncapture();
    }
}
