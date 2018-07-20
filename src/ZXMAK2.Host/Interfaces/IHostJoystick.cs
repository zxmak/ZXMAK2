using System;
using System.Collections.Generic;


namespace ZXMAK2.Host.Interfaces
{
    public interface IHostJoystick : IDisposable
    {
        void CaptureHostDevice(string hostId);
        void ReleaseHostDevice(string hostId);
        void Scan();
        IJoystickState GetState(string hostId);
        IKeyboardState KeyboardState { set; }
        bool IsKeyboardStateRequired { get; }
        IEnumerable<IHostDeviceInfo> GetAvailableJoysticks();
    }
}
