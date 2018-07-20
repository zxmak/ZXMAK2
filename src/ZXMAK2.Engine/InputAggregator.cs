using System;
using System.Collections.Generic;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Host.Entities;


namespace ZXMAK2.Engine
{
    public sealed class InputAggregator : IDisposable
    {
        private readonly IHostKeyboard m_hostKeyboard;
        private readonly IHostMouse m_hostMouse;
        private readonly IHostJoystick m_hostJoystick;
        
        private readonly IKeyboardDevice[] m_keyboards;
        private readonly IMouseDevice[] m_mouses;
        private readonly IJoystickDevice[] m_joysticks;


        public InputAggregator(
            IHostService host,
            IKeyboardDevice[] keyboards,
            IMouseDevice[] mouses,
            IJoystickDevice[] joysticks)
        {
            m_hostKeyboard = host.Keyboard;
            m_hostMouse = host.Mouse;
            m_hostJoystick = host.Joystick;
            
            m_keyboards = keyboards;
            m_mouses = mouses;
            m_joysticks = joysticks;
            Capture();
        }

        public void Dispose()
        {
            Release();
        }

        private void Capture()
        {
            if (m_hostJoystick == null)
            {
                return;
            }
            var list = new List<string>();
            foreach (var j in m_joysticks)
            {
                if (!list.Contains(j.HostId))
                {
                    list.Add(j.HostId);
                    m_hostJoystick.CaptureHostDevice(j.HostId);
                }
            }
        }

        private void Release()
        {
            if (m_hostJoystick == null)
            {
                return;
            }
            var list = new List<string>();
            foreach (var j in m_joysticks)
            {
                if (!list.Contains(j.HostId))
                {
                    list.Add(j.HostId);
                    m_hostJoystick.ReleaseHostDevice(j.HostId);
                }
            }
        }

        public void Scan()
        {
            if (m_hostKeyboard != null &&
                (m_keyboards.Length > 0 || m_hostJoystick.IsKeyboardStateRequired))
            {
                m_hostKeyboard.Scan();
                var state = m_hostKeyboard.State;
                //if (!((state[Key.LeftAlt] || state[Key.RightAlt]) && state[Key.Return]))
                {
                    // lock for Alt+Enter?
                    // TODO: implement system key lock (set from ui)
                    foreach (var kbd in m_keyboards)
                    {
                        kbd.KeyboardState = m_hostKeyboard.State;
                    }
                }
                if (m_hostJoystick != null)
                {
                    m_hostJoystick.KeyboardState = m_hostKeyboard.State;
                }
            }
            if (m_hostMouse != null && m_mouses.Length > 0)
            {
                m_hostMouse.Scan();
                foreach (var mouse in m_mouses)
                {
                    mouse.MouseState = m_hostMouse.MouseState;
                }
            }
            if (m_hostJoystick != null && m_joysticks.Length > 0)
            {
                m_hostJoystick.Scan();
                foreach (var j in m_joysticks)
                {
                    j.JoystickState = m_hostJoystick.GetState(j.HostId);
                }
            }
        }
    }
}
