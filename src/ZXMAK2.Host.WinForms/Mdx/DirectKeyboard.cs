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
 *  Description: ZX Spectrum keyboard emulator
 *  Author: Alex Makeev
 *  Date: 26.03.2008, 10.07.2018
 */
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities.Tools;
using ZXMAK2.Host.WinForms.Tools;
using ZXMAK2.DirectX;
using ZXMAK2.DirectX.DirectInput;
using ZxmakKey = ZXMAK2.Host.Entities.Key;
using MdxKey = ZXMAK2.DirectX.DirectInput.Key;


namespace ZXMAK2.Host.WinForms.Mdx
{
    public sealed class DirectKeyboard : IHostKeyboard, IKeyboardState
    {
        private readonly Form _form;
        private readonly IntPtr _hWnd;
        private DirectInputDevice8W _device;
        private KeyboardStateMapper<MdxKey> _mapper = new KeyboardStateMapper<MdxKey>();
        private readonly Dictionary<ZxmakKey, bool> _state = new Dictionary<ZxmakKey, bool>();
        private bool _isAcquired;



        public unsafe DirectKeyboard(Form form)
        {
            if (form == null)
            {
                throw new ArgumentNullException("form");
            }
            _form = form;
            _hWnd = form.Handle;
            using (var dinput = new DirectInput8W())
            {
                _device = dinput.CreateDevice(SysGuid.GUID_SysKeyboard, null);
            }
            _device.SetDataFormat(DIDATAFORMAT.c_dfDIKeyboard).CheckError();
            _device.SetCooperativeLevel(_hWnd, DISCL.NONEXCLUSIVE | DISCL.FOREGROUND).CheckError();
            form.Deactivate += WndDeactivate;
            TryAcquire();
            _mapper.LoadMapFromString(
                global::ZXMAK2.Host.WinForms.Properties.Resources.Keyboard_Mdx);
        }

        public void Dispose()
        {
            if (_device == null)
            {
                return;
            }
            // TODO: sync needed
            var device = _device;
            _device = null;
            
            _form.Deactivate -= WndDeactivate;
            if (_isAcquired)
            {
                _isAcquired = false;
                var hr = device.Unacquire();
                if (hr.IsFailure)
                {
                    Logger.Error("DirectKeyboard.Dispose: {0}", hr);
                }
            }
            device.Dispose();
        }


        #region IKeyboardState

        public bool this[ZxmakKey key]
        {
            get { return _state.ContainsKey(key) && _state[key]; }
        }

        #endregion IKeyboardState


        #region IHostKeyboard

        public IKeyboardState State
        {
            get { return this; }
        }

        private byte[] _diState = new byte[256];
        public void Scan()
        {
            if (_device == null || (!_isAcquired && !TryAcquire()))
            {
                foreach (var key in _mapper.Keys)
                {
                    _state[key] = false;
                }
                return;
            }
            var hr = _device.GetDeviceState(_diState);
            if (hr.IsSuccess)
            {
                foreach (var key in _mapper.Keys)
                {
                    _state[key] = _diState[(int)_mapper[key]] != 0;
                }
            }
            else if (hr == ErrorCode.DIERR_NOTACQUIRED)
            {
                // TODO: sync needed
            }
            else if (hr == ErrorCode.DIERR_INPUTLOST)
            {
                WndDeactivate(null, null);
            }
            else
            {
                Logger.Error("DirectKeyboard.Scan: {0}", hr);
                WndDeactivate(null, null);
            }
        }

        #endregion IHostKeyboard


        #region Private

        private bool TryAcquire()
        {
            if (_device == null || 
                _hWnd != NativeMethods.GetForegroundWindow())
            {
                return false;
            }
            var hr = _device.Acquire();
            _isAcquired = true;
            if (hr.IsSuccess)
            {
                return true;
            }
            if (hr != ErrorCode.DIERR_OTHERAPPHASPRIO &&
                hr != ErrorCode.DIERR_INPUTLOST)
            {
                Logger.Error("DirectKeyboard.TryAcquire: {0}", hr);
            }
            return false;
        }

        private void WndDeactivate(object sender, EventArgs e)
        {
            if (_device == null || !_isAcquired)
            {
                return;
            }
            _isAcquired = false;
            var hr = _device.Unacquire();
            if (hr.IsSuccess)
            {
                return;
            }
            Logger.Error("DirectKeyboard.WndDeactivate: {0}", hr);
        }

        #endregion Private
    }
}
