// (c) 2013 Eltaron
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;
using ZXMAK2.DirectX;
using ZxmakKey = ZXMAK2.Host.Entities.Key;
using ZXMAK2.DirectX.DirectInput;


namespace ZXMAK2.Host.WinForms.Mdx
{
    public sealed class DirectJoystick : IHostJoystick, IDisposable
    {
        #region Fields

        private const string KeyboardNumpadId = "keyboard";

        private readonly Dictionary<string, IJoystickState> _states = new Dictionary<string, IJoystickState>();
        private readonly Dictionary<string, DirectInputDevice8W> _devices = new Dictionary<string, DirectInputDevice8W>();
        private readonly Dictionary<string, bool> _acquired = new Dictionary<string, bool>();
        private Form _form;
        private IntPtr _hwnd;
        private IJoystickState _numpadState;

        #endregion Fields


        #region Public

        public DirectJoystick(Form form)
        {
            _form = form;
            _hwnd = form.Handle;
            _form.Activated += WndActivated;
            _form.Deactivate += WndDeactivate;
        }

        public void Scan()
        {
            var guidList = _devices.Keys;
            foreach (var guid in guidList)
            {
                ActivateDevice(guid);
                _states[guid] = ScanDevice(guid);
            }
            if (IsKeyboardStateRequired)
            {
                var isUp = KeyboardState[ZxmakKey.NumPad8];
                var isDown = KeyboardState[ZxmakKey.NumPad2];
                var isLeft = KeyboardState[ZxmakKey.NumPad4];
                var isRight = KeyboardState[ZxmakKey.NumPad6];
                var isFire = KeyboardState[ZxmakKey.NumPad5] ||
                    KeyboardState[ZxmakKey.NumPad0];
                _numpadState = new StateWrapper(
                    isLeft,
                    isRight,
                    isUp,
                    isDown,
                    isFire);
            }
        }

        public void Dispose()
        {
            if (_form != null)
            {
                _form.Activated -= WndActivated;
                _form.Deactivate -= WndDeactivate;
            }
            foreach (var guid in _devices.Keys)
            {
                ReleaseHostDevice(guid);
            }
        }

        public void CaptureHostDevice(string hostId)
        {
            try
            {
                if (hostId == string.Empty)
                {
                    return;
                }
                if (hostId == KeyboardNumpadId)
                {
                    IsKeyboardStateRequired = true;
                    return;
                }
                using (var di = new DirectInput8W())
                {
                    var list = di.EnumDevices(DI8DEVCLASS.GAMECTRL, DIEDFL.ATTACHEDONLY);
                    foreach (var deviceInstance in list)
                    {
                        if (string.Compare(
                            GetDeviceId(deviceInstance.guidInstance),
                            hostId,
                            true) != 0)
                        {
                            continue;
                        }
                        var joystick = di.CreateDevice(deviceInstance.guidInstance, null);
                        try
                        {
                            joystick.SetDataFormat(DIDATAFORMAT.c_dfDIJoystick).CheckError();
                            //joystick.SetCooperativeLevel(_hwnd, DISCL.BACKGROUND | DISCL.NONEXCLUSIVE).CheckError();
                            // someone replaced hwnd with null to fix app close hung (when MDX was used)
                            joystick.SetCooperativeLevel(IntPtr.Zero, DISCL.BACKGROUND | DISCL.NONEXCLUSIVE).CheckError();
                            joystick.Acquire().CheckError();
                            _devices.Add(hostId, joystick);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                            joystick.Dispose();
                            continue;
                        }
                        ActivateDevice(hostId);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void ReleaseHostDevice(string hostId)
        {
            try
            {
                if (hostId == KeyboardNumpadId)
                {
                    IsKeyboardStateRequired = false;
                    return;
                }
                if (!_devices.ContainsKey(hostId))
                {
                    return;
                }
                var device = _devices[hostId];
                DeactivateDevice(hostId);
                try
                {
                    device.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
                _devices.Remove(hostId);
                if (_states.ContainsKey(hostId))
                {
                    _states.Remove(hostId);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public IJoystickState GetState(string hostId)
        {
            if (_states.ContainsKey(hostId))
            {
                return _states[hostId];
            }
            if (hostId == KeyboardNumpadId)
            {
                return _numpadState;
            }
            return StateWrapper.Empty;
        }

        public IKeyboardState KeyboardState { get; set; }
        public bool IsKeyboardStateRequired { get; private set; }

        public IEnumerable<IHostDeviceInfo> GetAvailableJoysticks()
        {
            var list = new List<IHostDeviceInfo>();
            try
            {
                using (var di = new DirectInput8W())
                {
                    var devList = di.EnumDevices(DI8DEVCLASS.GAMECTRL, DIEDFL.ATTACHEDONLY);
                    foreach (var deviceInstance in devList)
                    {
                        var hdi = new HostDeviceInfo(
                            deviceInstance.tszInstanceName,
                            GetDeviceId(deviceInstance.guidInstance));
                        list.Add(hdi);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            list.Sort();
            list.Insert(
                0,
                new HostDeviceInfo("Keyboard Numpad", KeyboardNumpadId));
            list.Insert(
                0,
                new HostDeviceInfo("None", string.Empty));
            return list;
        }

        #endregion Public


        #region Private

        private static string GetDeviceId(Guid guid)
        {
            return guid.ToString(null, CultureInfo.InvariantCulture);
        }

        private void WndActivated(object sender, EventArgs e)
        {
            var guidList = _devices.Keys;
            foreach (var guid in guidList)
            {
                ActivateDevice(guid);
            }
        }

        private void WndDeactivate(object sender, EventArgs e)
        {
            var guidList = _devices.Keys;
            foreach (var guid in guidList)
            {
                DeactivateDevice(guid);
            }
        }

        private void ActivateDevice(string guid)
        {
            try
            {
                var device = _devices[guid];
                var acquired = _acquired.ContainsKey(guid) &&
                    _acquired[guid];
                if (!acquired)
                {
                    device.Acquire().CheckError();
                }
                _acquired[guid] = true;
            }
            catch
            {
                _acquired[guid] = false;
            }
        }

        private void DeactivateDevice(string guid)
        {
            try
            {
                _acquired[guid] = false;
                _devices[guid].Unacquire().CheckError();
            }
            catch
            {
            }
        }

        private DIJOYSTATE _diState;
        private IJoystickState ScanDevice(string hostId)
        {
            try
            {
                if (!_acquired.ContainsKey(hostId) || 
                    !_acquired[hostId])
                {
                    return StateWrapper.Empty;
                }
                var device = _devices[hostId];

                // axisTolerance check is needed because of little fluctuation of axis values even when nothing is pressed.
                int axisTolerance = 0x1000; // Should this be taken from joystick device somehow?
                ushort center = 0x7FFF;

                var hr = device.Poll();
                if (hr.IsSuccess) 
                    hr = device.GetDeviceState(out _diState);
                if (hr.IsSuccess)
                {
                    var diState = _diState;

                    var isDown = diState.lY > center && diState.lY - center > axisTolerance;
                    var isUp = diState.lY < center && center - diState.lY > axisTolerance;
                    var isRight = diState.lX > center && diState.lX - center > axisTolerance;
                    var isLeft = diState.lX < center && center - diState.lX > axisTolerance;
                    var isFire = false;

                    var buttons = diState.GetButtons();
                    foreach (var button in buttons)
                    {
                        // fire = any key pressed
                        isFire |= (button & 0x80) != 0;
                    }

                    return new StateWrapper(
                        isLeft,
                        isRight,
                        isUp,
                        isDown,
                        isFire);
                }
                else if (hr == ErrorCode.DIERR_NOTACQUIRED)
                {
                    _acquired[hostId] = false;
                }
                else if (hr == ErrorCode.DIERR_INPUTLOST)
                {
                    _acquired[hostId] = false;
                    //ReleaseHostDevice(hostId);
                }
                else
                {
                    Logger.Error("[joy.ScanDevice] {0}", hr);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return StateWrapper.Empty;
        }

        private class StateWrapper : IJoystickState
        {
            #region Static

            private static readonly StateWrapper s_empty = new StateWrapper(false, false, false, false, false);

            public static StateWrapper Empty
            {
                get { return s_empty; }
            }

            #endregion


            #region Public

            public StateWrapper(
                bool isLeft,
                bool isRight,
                bool isUp,
                bool isDown,
                bool isFire)
            {
                IsLeft = isLeft;
                IsRight = isRight;
                IsUp = isUp;
                IsDown = isDown;
                IsFire = isFire;
            }

            public StateWrapper()
                : this(false, false, false, false, false)
            {
            }

            public bool IsLeft { get; private set; }
            public bool IsRight { get; private set; }
            public bool IsUp { get; private set; }
            public bool IsDown { get; private set; }
            public bool IsFire { get; private set; }

            #endregion
        }

        #endregion Private
    }
}
