/* 
 *  Copyright 2008-2018 Alex Makeev
 * 
 *  This file is part of ZXMAK2 (ZX Spectrum virtual machine).
 *
 *  ZXMAK2 is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  ZXMAK2 is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with ZXMAK2.  If not, see <http://www.gnu.org/licenses/>.
 *
 *  Description: ZX Spectrum mouse emulator
 *  Author: Alex Makeev
 *  Date: 26.03.2008, 10.07.2018
 */
using System;
using System.Windows.Forms;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.DirectX;
using ZXMAK2.DirectX.DirectInput;


namespace ZXMAK2.Host.WinForms.Mdx
{
    public sealed class DirectMouse : IHostMouse, IDisposable
    {
        private readonly MouseStateWrapper _state = new MouseStateWrapper();
        private Form _form;
        private IntPtr _hWnd;
        private DirectInputDevice8W _device;
        private bool _active;


        #region .ctor

        public unsafe DirectMouse(Form form)
        {
            _form = form;
            _hWnd = form.Handle;
            if (_device == null)
            {
                using (var dinput = new DirectInput8W())
                {
                    _device = dinput.CreateDevice(SysGuid.GUID_SysMouse, null);
                }
                _device.SetDataFormat(DIDATAFORMAT.c_dfDIMouse).CheckError();

                form.Deactivate += WndDeactivate;
            }
        }

        public void Dispose()
        {
            if (_device != null)
            {
                _active = false;
                var hr = _device.Unacquire();
                if (hr.IsFailure)
                {
                    Logger.Error("DirectMouse.Dispose: {0}", hr);
                }
            }
            Dispose(ref _device);
        }

        #endregion .ctor


        #region IHostMouse

        public IMouseState MouseState
        {
            get { return _state; }
        }

        public bool IsCaptured
        {
            get { return _active; }
        }

        private DIMOUSESTATE _diState;
        public void Scan()
        {
            if (!_active || _device==null)
            {
                return;
            }
            var hr = _device.GetDeviceState(out _diState);
            if (hr.IsSuccess)
            {
                _state.Update(ref _diState);
            }
            else//if(ErrorCode.DIERR_NOTACQUIRED)
            {
                Uncapture();    
            }
        }

        public void Capture()
        {
            if (_device == null || _active)
            {
                return;
            }
            var hr = _device.SetCooperativeLevel(_hWnd, DISCL.EXCLUSIVE | DISCL.FOREGROUND);
            if (hr.IsSuccess)
            {
                hr = _device.Acquire();
                _active = true;
            }
            if (hr.IsFailure)
            {
                Uncapture();
            }
        }

        public void Uncapture()
        {
            if (_device == null)
            {
                return;
            }
            if (_active)
            {
                _device.Unacquire();
            }
            _device.SetCooperativeLevel(_hWnd, DISCL.NONEXCLUSIVE | DISCL.FOREGROUND);
            _active = false;
        }


        #endregion IHostMouse


        #region Private

        private void WndDeactivate(object sender, EventArgs e)
        {
            Uncapture();
        }

        private static void Dispose<T>(ref T disposable)
            where T : IDisposable
        {
            var value = disposable;
            disposable = default(T);
            value.Dispose();
        }

        #endregion Private


        private class MouseStateWrapper : IMouseState
        {
            private int m_x = 128;
            private int m_y = 128;
            private int m_b = 0;

            internal MouseStateWrapper()
            {
            }

            internal void Update(ref DIMOUSESTATE state)
            {
                m_x += state.lX;
                m_y += state.lY;

                m_b = 0;
                if ((state.rgbButton0 & 0x80) != 0) m_b |= 1;
                if ((state.rgbButton1 & 0x80) != 0) m_b |= 2;
                if ((state.rgbButton2 & 0x80) != 0) m_b |= 4;
                if ((state.rgbButton3 & 0x80) != 0) m_b |= 8;
                if ((state.rgbButton4 & 0x80) != 0) m_b |= 16;
                if ((state.rgbButton5 & 0x80) != 0) m_b |= 32;
                if ((state.rgbButton6 & 0x80) != 0) m_b |= 64;
                if ((state.rgbButton7 & 0x80) != 0) m_b |= 128;
            }

            public int X { get { return m_x; } }
            public int Y { get { return m_y; } }
            public int Buttons { get { return m_b; } }
        }
    }
}
