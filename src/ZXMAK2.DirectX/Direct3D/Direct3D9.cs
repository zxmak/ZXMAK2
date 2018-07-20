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
 *  Description: DirectX native wrapper
 *  Date: 15.07.2018
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ZXMAK2.DirectX.Native;


namespace ZXMAK2.DirectX.Direct3D
{
    /// <unmanaged>IDirect3D9</unmanaged>	
    [Guid("81BDCBCA-64D4-426d-AE8D-AD0147F4275C")]
    public class Direct3D9 : ComObject
    {
        protected const int D3D_SDK_VERSION = 32;
        
        
        public Direct3D9()
            : base(IntPtr.Zero)
        {
            var nativePointer = NativeMethods.Direct3DCreate9_(D3D_SDK_VERSION);
            if (nativePointer == IntPtr.Zero)
            {
                throw new DirectXException(Marshal.GetLastWin32Error());
            }
            base.NativePointer = nativePointer;
        }
        
        public Direct3D9(IntPtr nativePointer)
            : base(nativePointer)
        {
        }

        /// <unmanaged>HRESULT IDirect3D9::CreateDevice([In] unsigned int Adapter,[In] D3DDEVTYPE DeviceType,[In] HWND hFocusWindow,[In] D3DCREATE BehaviorFlags,[In, Buffer] D3DPRESENT_PARAMETERS* pPresentationParameters,[Out, Fast] IDirect3DDevice9** ppReturnedDeviceInterface)</unmanaged>	
        private unsafe HRESULT CreateDevice_(int adapter, D3DDEVTYPE deviceType, IntPtr hFocusWindow, D3DCREATE behaviorFlags, D3DPRESENT_PARAMETERS[] pPresentationParameters, out IntPtr ppReturnedDeviceInterface)
        {
	        var result = IntPtr.Zero;
            HRESULT hr;
            fixed (void* ptr = pPresentationParameters)
	        {
		        //result = calli(System.Int32(System.Void*,System.Int32,System.Int32,System.Void*,System.Int32,System.Void*,System.Void*), this._nativePointer, adapter, deviceType, (void*)hFocusWindow, behaviorFlags, ptr, &zero, *(*(IntPtr*)this._nativePointer + (IntPtr)16 * (IntPtr)sizeof(void*)));
                hr = (HRESULT)NativeHelper.CalliInt32(16, _nativePointer, adapter, (int)deviceType, (void*)hFocusWindow, (int)behaviorFlags, ptr, &result);
	        }
            ppReturnedDeviceInterface = result;
            return hr;
        }
        
        public Direct3DDevice9 CreateDevice(int adapter, D3DDEVTYPE deviceType, IntPtr hFocusWindow, D3DCREATE behaviorFlags, params D3DPRESENT_PARAMETERS[] pPresentationParameters)
        {
            IntPtr nativePointer;
            CreateDevice_(adapter, deviceType, hFocusWindow, behaviorFlags, pPresentationParameters, out nativePointer).CheckError();
            return new Direct3DDevice9(nativePointer);
        }

        /// <unmanaged>HRESULT IDirect3D9::GetDeviceCaps([In] unsigned int Adapter,[In] D3DDEVTYPE DeviceType,[Out] D3DCAPS9* pCaps)</unmanaged>	
        public unsafe HRESULT GetDeviceCaps(int adapter, D3DDEVTYPE deviceType, out D3DCAPS9 caps)
        {
            fixed (void* pCaps = &caps)
            {
                //calli(System.Int32(System.Void*,System.Int32,System.Int32,System.Void*), this._nativePointer, adapter, deviceType, &result, *(*(IntPtr*)this._nativePointer + (IntPtr)14 * (IntPtr)sizeof(void*))).CheckError();
                return (HRESULT)NativeHelper.CalliInt32(14, _nativePointer, (int)adapter, (int)deviceType, (void*)pCaps);
            }
        }

        /// <unmanaged>unsigned int IDirect3D9::GetAdapterCount()</unmanaged>	
        public unsafe int GetAdapterCount()
        {
	        //return calli(System.Int32(System.Void*), this._nativePointer, *(*(IntPtr*)this._nativePointer + (IntPtr)4 * (IntPtr)sizeof(void*)));
            return NativeHelper.CalliInt32(4, _nativePointer);
        }

        /// <unmanaged>HRESULT IDirect3D9::GetAdapterIdentifier([In] unsigned int Adapter,[In] unsigned int Flags,[Out] D3DADAPTER_IDENTIFIER9* pIdentifier)</unmanaged>	
        public unsafe D3DADAPTER_IDENTIFIER9 GetAdapterIdentifier(int adapter, int flags)
        {
	        var native = new D3DADAPTER_IDENTIFIER9.NATIVE();
            //result = calli(System.Int32(System.Void*,System.Int32,System.Int32,System.Void*), this._nativePointer, adapter, flags, &_Native, *(*(IntPtr*)this._nativePointer + (IntPtr)5 * (IntPtr)sizeof(void*)));
            var hr = (HRESULT)NativeHelper.CalliInt32(5, _nativePointer, (int)adapter, (int)flags, (void*)&native);
            hr.CheckError();
            return new D3DADAPTER_IDENTIFIER9(&native);
        }

        /// <unmanaged>HRESULT IDirect3D9::GetAdapterDisplayMode([In] unsigned int Adapter,[Out] D3DDISPLAYMODE* pMode)</unmanaged>	
        public unsafe HRESULT GetAdapterDisplayMode(int adapter, out D3DDISPLAYMODE mode)
        {
	        fixed(void* pMode = &mode)
            {
                // calli(System.Int32(System.Void*,System.Int32,System.Void*), this._nativePointer, adapter, &result, *(*(IntPtr*)this._nativePointer + (IntPtr)8 * (IntPtr)sizeof(void*))).CheckError();
                return (HRESULT)NativeHelper.CalliInt32(8, _nativePointer, (int)adapter, (void*)pMode);
            }
        }

        /// <unmanaged>HMONITOR IDirect3D9::GetAdapterMonitor([In] unsigned int Adapter)</unmanaged>	
        public unsafe IntPtr GetAdapterMonitor(int adapter)
        {
	        //return calli(System.IntPtr(System.Void*,System.Int32), this._nativePointer, adapter, *(*(IntPtr*)this._nativePointer + (IntPtr)15 * (IntPtr)sizeof(void*)));
            return (IntPtr)NativeHelper.CalliInt32(15, _nativePointer, (int)adapter);
        }

        /// <unmanaged>unsigned int IDirect3D9::GetAdapterModeCount([In] unsigned int Adapter,[In] D3DFORMAT Format)</unmanaged>	
        public unsafe int GetAdapterModeCount(int adapter, D3DFORMAT format)
        {
	        //return calli(System.Int32(System.Void*,System.Int32,System.Int32), this._nativePointer, adapter, format, *(*(IntPtr*)this._nativePointer + (IntPtr)6 * (IntPtr)sizeof(void*)));
            return NativeHelper.CalliInt32(6, _nativePointer, (int)adapter, (int)format);
        }

        /// <unmanaged>HRESULT IDirect3D9::EnumAdapterModes([In] unsigned int Adapter,[In] D3DFORMAT Format,[In] unsigned int Mode,[Out] D3DDISPLAYMODE* pMode)</unmanaged>	
        public unsafe HRESULT EnumAdapterModes(int adapter, D3DFORMAT format, int mode, out D3DDISPLAYMODE d3dMode)
        {
            fixed (void* pMode = &d3dMode)
            {
                //calli(System.Int32(System.Void*,System.Int32,System.Int32,System.Int32,System.Void*), this._nativePointer, adapter, format, mode, &result, *(*(IntPtr*)this._nativePointer + (IntPtr)7 * (IntPtr)sizeof(void*))).CheckError();
                return (HRESULT)NativeHelper.CalliInt32(7, _nativePointer, (int)adapter, (int)format, (int)mode, (void*)pMode);
            }
        }

        public IList<AdapterInformation> GetAdapters()
        {
            var adapterCount = GetAdapterCount();
            var list = new List<AdapterInformation>(adapterCount);
            for (var i = 0; i < adapterCount; i++)
            {
                list.Add(new AdapterInformation(this, i));    
            }
            return list;
        }
    }
}
