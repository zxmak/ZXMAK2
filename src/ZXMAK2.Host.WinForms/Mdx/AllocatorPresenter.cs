/* 
 *  Copyright 2008, 2015 Alex Makeev
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
 */
using System;
using System.Linq;
using System.Collections.Generic;
using ZXMAK2.Host.WinForms.Tools;
using System.Threading;
using ZXMAK2.DirectX;
using ZXMAK2.DirectX.Direct3D;
using NativeWindow = System.Windows.Forms.NativeWindow;
using Message = System.Windows.Forms.Message;
using Size=System.Drawing.Size;


namespace ZXMAK2.Host.WinForms.Mdx
{
    public sealed class AllocatorPresenter : IAllocatorPresenter
    {
        #region Fields

        private readonly object _syncRoot = new object();
        private readonly List<IRenderer> _renderers = new List<IRenderer>();
        private readonly HashSet<IRenderer> _loaded = new HashSet<IRenderer>();
        private readonly HashSet<IRenderer> _failed = new HashSet<IRenderer>();
        private readonly HashSet<IRenderer> _good = new HashSet<IRenderer>();
        private SubclassWindow _window;
        private Thread _thread;
        private bool _isRunning;
        private IntPtr _monitorId;
        private Direct3DDevice9 _device;
        private D3DPRESENT_PARAMETERS _d3dpp;
        private Direct3DSwapChain9 _swapChain;
        private Direct3DSurface9 _renderTarget;
        private Size _size;

        #endregion Fields


        #region .ctor

        public AllocatorPresenter()
        {
            RefreshRate = 50;
        }

        public void Dispose()
        {
            Detach();
        }

        #endregion .ctor


        #region IDeviceAllocator

        public event EventHandler PresentCompleted;

        public string ErrorMessage { get; private set; }

        public bool IsRendering { get; private set; }

        public int RefreshRate { get; private set; }
        
        public void Attach(IntPtr hwnd)
        {
            if (hwnd == IntPtr.Zero)
            {
                throw new ArgumentNullException("hwnd");
            }
            lock (_syncRoot)
            {
                if (_window != null)
                {
                    throw new InvalidOperationException("Already attached!");
                }
                ErrorMessage = null;
                IsRendering = false;
                _window = CreateSubclassWindow(hwnd);
                _good.Clear();
                _isRunning = true;
                _thread = new Thread(RenderThreadProc);
                _thread.IsBackground = false;
                _thread.Start();
            }
        }

        public void Detach()
        {
            IsRendering = false;
            Thread thread = null;
            SubclassWindow window = null;
            lock (_syncRoot)
            {
                window = _window;
                _window = null;
                thread = _thread;
                _thread = null;
                _isRunning = false;
            }
            // wait&release outside lock to avoid deadlock
            if (thread != null && thread.IsAlive)
            {
                thread.Join();
            }
            if (window != null)
            {
                window.ReleaseHandle();
            }
        }

        private bool RenderIteration()
        {
            lock (_syncRoot)
            {
                if (_window == null)
                {
                    return false;
                }
                try
                {
                    if (_monitorId != _window.MonitorId)
                    {
                        // we need to recreate device for another adapterId
                        OnLost();
                        OnDeviceDestroy();
                    }
                    OnDeviceCheck();
                    if (_device == null)
                    {
                        Thread.Sleep(1000);
                        return false;
                    }
                    var hr = TestCooperativeLevel();
                    switch (hr)
                    {
                        case ErrorCode.D3D_OK: //Success
                            break;
                        case ErrorCode.D3DERR_DEVICENOTRESET: // appears once after DeviceLost
                            //Logger.Debug("Direct3D: DeviceNotReset");
                            //OnLost();
                            //OnDeviceDestroy();
                            OnDeviceReset();
                            return false;
                        case ErrorCode.D3DERR_DEVICELOST:     // appears continuously when DeviceLost
                            //Logger.Debug("Direct3D: DeviceLost");
                            OnLost();
                            Thread.Sleep(500);
                            return false;
                        default:                        // never seen
                            Logger.Warn("TestCooperativeLevel() = {0}", hr);
                            return false;
                    }

                    // device OK, check resources...
                    OnAcquireCheck();
                    OnLoadCheck();
                    if (_renderTarget == null)
                    {
                        return true;
                    }
                    var wndSize = _window.Size;
                    if (wndSize.Width < 1 ||
                        wndSize.Height < 1 ||
                        !_window.Visible)
                    {
                        return false;
                    }
                    if (_renderTarget == null ||
                        wndSize != _size)
                    {
                        OnLost();
                        return true;
                    }

                    // device & resources OK
                    OnRender();
                    if (_swapChain != null)
                    {
                        _swapChain.Present(0).CheckError(); // Present.DoNotWait // Present.None
                    }
                    else
                    {
                        _device.Present().CheckError();
                    }
                    RefreshRate = Device.DisplayMode.RefreshRate;
                    OnPresentCompleted();
                    IsRendering = true;
                    return true;
                }
                catch (DirectXException ex)
                {
                    if (ex.ErrorCode == ErrorCode.D3DERR_DEVICELOST) // DeviceLostException
                    {
                        Logger.Debug(ex);
                        return false;
                    }
                    else // GraphicsException
                    {
                        Logger.Error(ex);
                        ErrorMessage = string.Format("{0}: {1}", ex.GetType().Name, ex.Message);
                        IsRendering = false;
                        Thread.Sleep(1000);
                        return false;
                    }
                }
            }
        }

        private void OnDeviceReset()
        {
            OnLost();   // actually already called in case of DeviceLost
            Logger.Debug("Direct3D: reset");
            _device.Reset(_d3dpp).CheckError();
            OnDeviceInit();
        }

        public void Register(IRenderer renderer)
        {
            ExecuteSynchronized(() => _renderers.Add(renderer));
        }

        public void Unregister(IRenderer renderer)
        {
            ExecuteSynchronized(() =>
            {
                if (_loaded.Contains(renderer))
                {
                    RendererUnload(renderer);
                }
                _renderers.Remove(renderer);
                if (_failed.Contains(renderer))
                {
                    _failed.Remove(renderer);
                }
            });
        }

        #endregion IDeviceAllocator


        #region Internal

        internal Direct3DDevice9 Device
        {
            get { return _device; }
        }

        internal void ExecuteSynchronized(Action action)
        {
            lock (_syncRoot)
            {
                action();
            }
        }

        internal T ExecuteSynchronized<T>(Func<T> action)
        {
            lock (_syncRoot)
            {
                return action();
            }
        }

        #endregion Internal


        #region Private

        private void RenderThreadProc()
        {
            try
            {
                try
                {
                    while (_isRunning)
                    {
                        var success = RenderIteration();
                        Thread.Yield();
                        if (!success && _isRunning)
                        {
                            Thread.Sleep(10);
                        }
                    }
                }
                finally
                {
                    OnLost();
                    OnDeviceDestroy();
                }
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
                if (ex.Message.Contains("d3dx9_"))
                {
                    ErrorMessage = "Look in the browser, install DirectX9 end user runtime and restart emulator";
                    System.Diagnostics.Process.Start("http://www.microsoft.com/download/en/details.aspx?id=35");
                } else
                {
                    ErrorMessage = string.Format("{0}: {1}", ex.GetType(), ex.Message);
                }
            }
        }

        private void OnPresentCompleted()
        {
            var handler = PresentCompleted;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void OnDeviceCheck()
        {
            if (_device != null)
            {
                return;
            }
            _size = _window.Size;
            _d3dpp = CreatePresentParams(_window.Handle);

            _monitorId = _window.MonitorId;
            var adapterId = _window.AdapterId;
            using (var d3d = new Direct3D9())
            {
                D3DCAPS9 caps;
                d3d.GetDeviceCaps(adapterId, D3DDEVTYPE.D3DDEVTYPE_HAL, out caps).CheckError();
                var flags = D3DCREATE.D3DCREATE_NOWINDOWCHANGES | D3DCREATE.D3DCREATE_FPU_PRESERVE;// CreateFlags.MultiThreaded;
                if ((caps.DevCaps & D3DDEVCAPS.D3DDEVCAPS_HWTRANSFORMANDLIGHT) != 0)
                    flags |= D3DCREATE.D3DCREATE_HARDWARE_VERTEXPROCESSING;
                else
                    flags |= D3DCREATE.D3DCREATE_SOFTWARE_VERTEXPROCESSING;

                Logger.Debug("Direct3D: create Device, threadId={0}", Thread.CurrentThread.ManagedThreadId);
                _device = d3d.CreateDevice(
                    adapterId,
                    D3DDEVTYPE.D3DDEVTYPE_HAL,
                    _window.Handle,
                    flags,
                    _d3dpp);
            }
            //TODO: check
            //_device.DeviceResizing += (s, e) => e.Cancel = true;
            //_device.DeviceReset += Device_OnDeviceReset;

            OnDeviceInit();
        }

        private void OnDeviceDestroy()
        {
            if (_device != null)
            {
                Logger.Debug("Direct3D: dispose Device, threadId={0}", Thread.CurrentThread.ManagedThreadId);
                _device.Dispose();
                _device = null;
            }
        }

        private void OnDeviceInit()
        {
            // Set default sampler/renderer state
            _device.SetSamplerState(0, D3DSAMPLERSTATETYPE.D3DSAMP_MAGFILTER, (int)D3DTEXTUREFILTERTYPE.D3DTEXF_LINEAR);
            _device.SetSamplerState(0, D3DSAMPLERSTATETYPE.D3DSAMP_MINFILTER, (int)D3DTEXTUREFILTERTYPE.D3DTEXF_LINEAR);
            _device.SetRenderState(D3DRENDERSTATETYPE.D3DRS_LIGHTING, false);
            _device.SetRenderState(D3DRENDERSTATETYPE.D3DRS_ZENABLE, true);
            _device.SetRenderState(D3DRENDERSTATETYPE.D3DRS_DITHERENABLE, true);
            _device.SetRenderState(D3DRENDERSTATETYPE.D3DRS_CULLMODE, 1);

            _device.SetRenderState(D3DRENDERSTATETYPE.D3DRS_ALPHABLENDENABLE, true);
            _device.SetRenderState(D3DRENDERSTATETYPE.D3DRS_SRCBLEND, (int)D3DBLEND.D3DBLEND_SRCALPHA);
            _device.SetRenderState(D3DRENDERSTATETYPE.D3DRS_DESTBLEND, (int)D3DBLEND.D3DBLEND_INVSRCALPHA);
            //_device.SetRenderState(RenderStates.BlendOperation, (int)BlendOperation.Add);
        }

        private void OnLoadCheck()
        {
            if (_renderTarget == null)
            {
                return;
            }
            _loaded
                .Except(_renderers)
                .ToList()
                .ForEach(RendererUnload);
            _renderers
                .Except(_failed)
                .Except(_loaded)
                .ToList()
                .ForEach(RendererLoad);
        }

        private void OnUnload()
        {
            _loaded
                .ToList()
                .ForEach(RendererUnload);
        }

        private void RendererLoad(IRenderer renderer)
        {
            if (_loaded.Contains(renderer) || 
                _failed.Contains(renderer))
            {
                return;
            }
            _loaded.Add(renderer);
            try
            {
                renderer.Load();
                _good.Add(renderer);
            }
            catch
            {
                _failed.Add(renderer);
                _loaded.Remove(renderer);
                throw;
            }
        }

        private void RendererUnload(IRenderer renderer)
        {
            if (!_loaded.Contains(renderer))
            {
                return;
            }
            _loaded.Remove(renderer);
            try
            {
                renderer.Unload();
            }
            catch
            {
                _failed.Add(renderer);
                throw;
            }
        }

        private void OnAcquireCheck()
        {
            if (_device == null || 
                _renderTarget != null || 
                _swapChain != null)
            {
                return;
            }
            var d3dpp = (D3DPRESENT_PARAMETERS)_d3dpp;//.Clone();
            _size = _window.Size;
            d3dpp.hDeviceWindow = _window.Handle;
            d3dpp.BackBufferWidth = _size.Width;
            d3dpp.BackBufferHeight = _size.Height;


            if (_size.Width <= 0 || _size.Height <= 0)
            {
                return;
            }
            var failed = false;
            try
            {
                _swapChain = _device.CreateAdditionalSwapChain(ref d3dpp);
                _renderTarget = _swapChain.GetBackBuffer(0, D3DBACKBUFFER_TYPE.D3DBACKBUFFER_TYPE_MONO);
                //_renderTarget = _device.GetRenderTarget(0);
            }
            catch (Exception ex)
            {
                failed = true;
                ErrorMessage = string.Format("{0}: {1}", ex.GetType().Name, ex.Message);
                throw;
            }
            finally
            {
                if (failed)
                {
                    OnLost();
                }
            }
        }

        private void OnLost()
        {
            OnUnload();
            if (_renderTarget != null)
            {
                _renderTarget.Dispose();
                _renderTarget = null;
            }
            if (_swapChain != null)
            {
                _swapChain.Dispose();
                _swapChain = null;
            }
        }

        private unsafe void OnRender()
        {
            _device.SetRenderTarget(0, _renderTarget).CheckError();
            _device.Clear(D3DCLEAR.TARGET, 0, 1.0f, 0).CheckError();
            _device.BeginScene().CheckError();
            try
            {
                var renderers = _good.ToArray();
                foreach (var renderer in renderers)
                {
                    try
                    {
                        renderer.Render(_size.Width, _size.Height);
                    }
                    catch
                    {
                        _good.Remove(renderer);
                        _failed.Add(renderer);
                        throw;
                    }
                }
            }
            finally
            {
                _device.EndScene().CheckError();
            }
        }

        private ErrorCode TestCooperativeLevel()
        {
            if (_device == null)
            {
                return ErrorCode.D3DERR_NOTAVAILABLE;
            }
            return (ErrorCode)_device.TestCooperativeLevel();
        }

        private D3DPRESENT_PARAMETERS CreatePresentParams(IntPtr hwnd)
        {
            var format = D3DFORMAT.D3DFMT_A8R8G8B8;   // mode.Format;

            var d3dpp = new D3DPRESENT_PARAMETERS();
            d3dpp.hDeviceWindow = hwnd;
            d3dpp.Windowed = true;
            d3dpp.PresentationInterval = D3DPRESENT_INTERVAL.D3DPRESENT_INTERVAL_ONE;// PresentInterval.One;//vbSync ? PresentInterval.One : PresentInterval.Immediate;//PresentInterval.Default;
            //make sure you are NOT using flipping if you are in windowed mode. 
            //In windowed mode, you share the current video mode of the applications running. 
            //Unfortunately, you have to use the slower blitting process.
            d3dpp.SwapEffect = D3DSWAPEFFECT.D3DSWAPEFFECT_DISCARD;//vbSync ? SwapEffect.Flip : SwapEffect.Discard;//SwapEffect.Discard;
            d3dpp.BackBufferCount = 1;
            d3dpp.BackBufferFormat = format;
            d3dpp.BackBufferWidth = _size.Width > 0 ? _size.Width : 1;
            d3dpp.BackBufferHeight = _size.Height > 0 ? _size.Height : 1;
            d3dpp.EnableAutoDepthStencil = false;
            //d3dpp.MultiSample = MultiSampleType.NonMaskable;
            d3dpp.Flags = D3DPRESENTFLAG.VIDEO; // PresentFlag.DeviceClip == single display mode
            return d3dpp;
        }

        private SubclassWindow CreateSubclassWindow(IntPtr hwnd)
        {
            var window = new SubclassWindow(this);
            var failed = false;
            try
            {
                window.AssignHandle(hwnd);
                return window;
            }
            catch (Exception ex)
            {
                failed = true;
                ErrorMessage = string.Format("{0}: {1}", ex.GetType().Name, ex.Message);
                throw;
            }
            finally
            {
                if (failed)
                {
                    window.ReleaseHandle();
                }
            }
        }

        #endregion Private


        #region SubclassWindow

        private sealed class SubclassWindow : NativeWindow
        {
            private const int WM_WINDOWPOSCHANGING = 0x0046;
            private const int WM_WINDOWPOSCHANGED = 0x0047;
            //private const int WM_MOVING = 0x0216;
            //private const int WM_PAINT = 0x000F;
            //private const int WM_SIZE = 0x0005;
            private const int MONITOR_DEFAULTTONEAREST = 2;
            private const int GA_ROOT = 2;

            private readonly AllocatorPresenter _allocator;
            private readonly SubclassWindowRoot _windowRoot;

            public SubclassWindow(AllocatorPresenter allocator)
            {
                _allocator = allocator;
                _windowRoot = new SubclassWindowRoot(this);
            }

            public IntPtr MonitorId { get; private set; }
            public int AdapterId { get; private set; }
            public string AdapterName { get; private set; }
            public string AdapterDriver { get; private set; }

            public Size Size
            {
                get { return NativeMethods.GetWindowRect(Handle).Size; }
            }

            public bool Visible
            {
                get { return NativeMethods.IsWindowVisible(Handle); }
            }


            protected override void OnHandleChange()
            {
                base.OnHandleChange();
                if (Handle != IntPtr.Zero)
                {
                    var root = NativeMethods.GetAncestor(Handle, GA_ROOT);
                    _windowRoot.AssignHandle(root);
                }
                else
                {
                    _windowRoot.ReleaseHandle();
                    AdapterId = 0;
                    MonitorId = IntPtr.Zero;
                    return;
                }
                CheckAdapter();
            }

            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case WM_WINDOWPOSCHANGING:
                        lock (_allocator._syncRoot)
                        {
                            base.WndProc(ref m);
                        }
                        CheckAdapter();
                        return;
                }
                base.WndProc(ref m);
            }

            
            #region Private

            private void CheckAdapter()
            {
                var hMonitor = NativeMethods.MonitorFromWindow(Handle, MONITOR_DEFAULTTONEAREST);
                if (MonitorId == hMonitor)
                {
                    return;
                }
                MonitorId = hMonitor;
                Logger.Debug("MonitorFromWindow: 0x{0}", MonitorId.ToString("X"));

                var adapter = GetAdapterId(Handle);
                if (adapter == null)
                {
                    AdapterId = 0;
                    Logger.Debug("AdapterId: {0}", AdapterId);
                }
                else
                {
                    AdapterId = adapter.Adapter;
                    Logger.Debug(
                        "AdapterId: {0} [\"{1}\", \"{2}\"]",
                        AdapterId,
                        adapter.Details.DeviceName,
                        adapter.Details.Description);
                }
            }

            private AdapterInformation GetAdapterId(IntPtr hwnd)
            {
                using (var d3d = new Direct3D9())
                {
                    var screen = System.Windows.Forms.Screen.FromHandle(hwnd);
                    if (screen == null)
                    {
                        Logger.Warn("SubclassWindow.GetAdapterId: Screen.FromHandle({0}) == null", Handle);
                        return d3d.GetAdapters().First();
                    }
                    var deviceName = FixWinXpDeviceName(screen.DeviceName);
                    var adapterInfo = d3d.GetAdapters()
                        .FirstOrDefault(ai => ai.Details.DeviceName == deviceName);
                    if (adapterInfo == null)
                    {
                        // happens when display just connected on the fly
                        // so it is missing from Manager.Adapters
                        Logger.Warn("SubclassWindow.GetAdapterId: adapter not found!");
                        Logger.Debug("Screen.DeviceName: \"{0}\"", screen.DeviceName);
                        Logger.Debug("Fixed DeviceName: \"{0}\"", deviceName);
                        var list = d3d.GetAdapters().ToList();
                        list.ForEach(ai => Logger.Debug("Manager.Adapters[{0}].DeviceName: \"{1}\"", ai.Adapter, ai.Details.DeviceName));
                        return list.First();
                    }
                    adapterInfo = adapterInfo ?? d3d.GetAdapters().First();
                    return adapterInfo;
                }
            }

            private string FixWinXpDeviceName(string name)
            {
                // http://stackoverflow.com/questions/12319903/system-windows-forms-screen-devicename-gives-garbage-on-windows-xp
                if (name == null)
                {
                    return null;
                }
                var endIndex = name.IndexOf((char)0);
                if (endIndex > 0)
                {
                    return name.Substring(0, endIndex);
                }
                return name;
            }

            #endregion Private


            private sealed class SubclassWindowRoot : NativeWindow
            {
                private readonly SubclassWindow _child;

                public SubclassWindowRoot(SubclassWindow child)
                {
                    _child = child;
                }

                protected override void WndProc(ref Message m)
                {
                    switch (m.Msg)
                    {
                        case WM_WINDOWPOSCHANGING:
                            lock (_child._allocator._syncRoot)
                            {
                                base.WndProc(ref m);
                            }
                            return;
                        case WM_WINDOWPOSCHANGED:
                            _child.CheckAdapter();
                            break;
                    }
                    base.WndProc(ref m);
                }
            }
        }

        #endregion SubclassWindow
    }
}
