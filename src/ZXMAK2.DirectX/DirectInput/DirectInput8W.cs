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
using System.Runtime.InteropServices;
using ZXMAK2.DirectX.Native;


namespace ZXMAK2.DirectX.DirectInput
{
    /// <unmanaged>IDirectInput8W</unmanaged>	
    [Guid("bf798031-483a-4da2-aa99-5d64ed369700")]
    public class DirectInput8W : ComObject
    {
        private const int DIRECTINPUT_VERSION = 0x0800;
        
        public unsafe DirectInput8W()
            : base(IntPtr.Zero)
        {
            IntPtr nativePointer;
            var hInstance = NativeMethods.GetModuleHandle(null);
            var riid = typeof(DirectInput8W).GUID;
            var result = (HRESULT)NativeMethods.DirectInput8Create_(
                (void*)hInstance,
                DIRECTINPUT_VERSION,
                (void*)&riid,
                (void*)&nativePointer,
                (void*)IntPtr.Zero);
            result.CheckError();
            base.NativePointer = nativePointer;
        }

        public DirectInput8W(IntPtr nativePointer)
            : base(nativePointer)
        {
        }

        /// <unmanaged>HRESULT IDirectInput8W::CreateDevice([In] const GUID arg0,[Out] void** arg1,[In] IUnknown* arg2)</unmanaged>	
        public unsafe DirectInputDevice8W CreateDevice(Guid deviceGuid, ComObject arg2)
		{
            IntPtr nativePointer;
			//result = calli(System.Int32(System.Void*,System.Void*,System.Void*,System.Void*), this._nativePointer, &arg0, ptr, (void*)((arg2 == null) ? IntPtr.Zero : arg2.NativePointer), *(*(IntPtr*)this._nativePointer + (IntPtr)3 * (IntPtr)sizeof(void*)));
            var result = (HRESULT)NativeHelper.CalliInt32(3, _nativePointer, (void*)&deviceGuid, (void*)&nativePointer, (void*)((arg2 == null) ? IntPtr.Zero : arg2.NativePointer));
			result.CheckError();
            return new DirectInputDevice8W(nativePointer);
		}

        /// <unmanaged>HRESULT IDirectInput8W::EnumDevices([In] unsigned int arg0,[In] __function__stdcall* arg1,[In] void* arg2,[In] DIEDFL arg3)</unmanaged>	
        internal unsafe HRESULT EnumDevices(int arg0, FunctionCallback arg1, IntPtr arg2, DIEDFL arg3)
		{
			//calli(System.Int32(System.Void*,System.Int32,System.Void*,System.Void*,System.Int32), this._nativePointer, arg0, arg1, (void*)arg2, arg3, *(*(IntPtr*)this._nativePointer + (IntPtr)4 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(4, _nativePointer, arg0, (void*)arg1, (void*)arg2, (int)arg3);
		}

        public IList<DIDEVICEINSTANCE> EnumDevices(DI8DEVCLASS deviceClass, DIEDFL deviceEnumFlags)
        {
            var callback = new EnumDevicesCallback();
            this.EnumDevices((int)deviceClass, callback.NativePointer, IntPtr.Zero, deviceEnumFlags).CheckError();
            return callback.DeviceInstances;
        }

        public IList<DIDEVICEINSTANCE> EnumDevices(DI8DEVTYPE deviceType, DIEDFL deviceEnumFlags)
        {
            var callback = new EnumDevicesCallback();
            this.EnumDevices((int)deviceType, callback.NativePointer, IntPtr.Zero, deviceEnumFlags);
            return callback.DeviceInstances;
        }

        /// <unmanaged>HRESULT IDirectInput8W::GetDeviceStatus([In] const GUID&amp; arg0)</unmanaged>	
        public unsafe HRESULT GetDeviceStatus(Guid arg0)
		{
			//return calli(System.Int32(System.Void*,System.Void*), this._nativePointer, &arg0, *(*(IntPtr*)this._nativePointer + (IntPtr)5 * (IntPtr)sizeof(void*)));
            return (HRESULT)NativeHelper.CalliInt32(5, _nativePointer, &arg0);
		}

        /// <unmanaged>HRESULT IDirectInput8W::RunControlPanel([In] HWND arg0,[In] unsigned int arg1)</unmanaged>	
        public unsafe void RunControlPanel(IntPtr hwnd, int arg1)
		{
            //calli(System.Int32(System.Void*,System.Void*,System.Int32), this._nativePointer, (void*)hwnd, arg1, *(*(IntPtr*)this._nativePointer + (IntPtr)6 * (IntPtr)sizeof(void*))).CheckError();
            var hr = (HRESULT)NativeHelper.CalliInt32(6, _nativePointer, (void*)hwnd, arg1);
            hr.CheckError();
		}

        /// <unmanaged>HRESULT IDirectInput8W::FindDevice([In] const GUID arg0,[In] const wchar_t* arg1,[Out] GUID* arg2)</unmanaged>	
        public unsafe Guid FindDevice(Guid arg0, string arg1)
		{
            IntPtr pArg1 = Marshal.StringToHGlobalUni(arg1);
			Guid result = default(Guid);
			//Result result2 = calli(System.Int32(System.Void*,System.Void*,System.Void*,System.Void*), this._nativePointer, &arg0, (void*)intPtr, &result, *(*(IntPtr*)this._nativePointer + (IntPtr)8 * (IntPtr)sizeof(void*)));
            var hr = (HRESULT)NativeHelper.CalliInt32(8, _nativePointer, &arg0, (void*)pArg1, &result);
			Marshal.FreeHGlobal(pArg1);
			hr.CheckError();
			return result;
		}
    }
}
