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
using System.Runtime.InteropServices;
using ZXMAK2.DirectX.Native;


namespace ZXMAK2.DirectX.DirectInput
{
    
    /// <unmanaged>IDirectInputDevice8W</unmanaged>	
	[Guid("54d41081-dc15-4833-a41b-748f73a38179")]
	public class DirectInputDevice8W : ComObject
    {
        public DirectInputDevice8W(IntPtr nativePointer)
            : base(nativePointer)
        {
        }


        /// <unmanaged>HRESULT IDirectInputDevice8W::Acquire()</unmanaged>	
        public unsafe HRESULT Acquire()
        {
	        //calli(System.Int32(System.Void*), this._nativePointer, *(*(IntPtr*)this._nativePointer + (IntPtr)7 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(7, _nativePointer);
        }

        /// <unmanaged>HRESULT IDirectInputDevice8W::Unacquire()</unmanaged>	
        public unsafe HRESULT Unacquire()
        {
	        //calli(System.Int32(System.Void*), this._nativePointer, *(*(IntPtr*)this._nativePointer + (IntPtr)8 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(8, _nativePointer);
        }

        /// <unmanaged>HRESULT IDirectInputDevice8W::SetDataFormat([In] const DIDATAFORMAT* arg0)</unmanaged>	
        public unsafe HRESULT SetDataFormat(DIDATAFORMAT dataFormat)
        {
            //Result result = calli(System.Int32(System.Void*,System.Void*), this._nativePointer, &_Native, *(*(IntPtr*)this._nativePointer + (IntPtr)11 * (IntPtr)sizeof(void*)));
            return (HRESULT)NativeHelper.CalliInt32(11, _nativePointer, (void*)&dataFormat);
        }

        /// <unmanaged>HRESULT IDirectInputDevice8W::SetCooperativeLevel([In] HWND arg0,[In] DISCL arg1)</unmanaged>	
        public unsafe HRESULT SetCooperativeLevel(IntPtr hwnd, DISCL flags)
        {
	        //calli(System.Int32(System.Void*,System.Void*,System.Int32), this._nativePointer, (void*)arg0, arg1, *(*(IntPtr*)this._nativePointer + (IntPtr)13 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(13, _nativePointer, (void*)hwnd, (int)flags);
        }


        /// <unmanaged>HRESULT IDirectInputDevice8W::GetDeviceState([In] unsigned int arg0,[In] void* arg1)</unmanaged>	
        public unsafe HRESULT GetDeviceState(int arg0, void* arg1)
        {
	        //calli(System.Int32(System.Void*,System.Int32,System.Void*), this._nativePointer, arg0, (void*)arg1, *(*(IntPtr*)this._nativePointer + (IntPtr)9 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(9, _nativePointer, (int)arg0, (void*)arg1);
        }

        public unsafe HRESULT GetDeviceState(out DIMOUSESTATE mouseState)
        {
            fixed (void* pState = &mouseState)
            {
                return GetDeviceState(sizeof(DIMOUSESTATE), pState);
            }
        }

        public unsafe HRESULT GetDeviceState(byte[] state)
        {
            fixed (byte* pState = state)
            {
                return GetDeviceState(state.Length, pState);
            }
        }

        public unsafe HRESULT GetDeviceState(out DIJOYSTATE joyState)
        {
            fixed (void* pState = &joyState)
            {
                return GetDeviceState(sizeof(DIJOYSTATE), pState);
            }
        }

        /// <unmanaged>HRESULT IDirectInputDevice8W::Poll()</unmanaged>	
        public unsafe HRESULT Poll()
        {
	        //calli(System.Int32(System.Void*), this._nativePointer, *(*(IntPtr*)this._nativePointer + (IntPtr)25 * (IntPtr)sizeof(void*))).CheckError();
            return NativeHelper.CalliInt32(25, _nativePointer);
        }

        /// <unmanaged>HRESULT IDirectInputDevice8W::GetDeviceInfo([Out] DIDEVICEINSTANCEW* arg0)</unmanaged>	
        public unsafe DIDEVICEINSTANCE GetInformation()
        {
	        var result = new DIDEVICEINSTANCE.NATIVE();
            result.dwSize = sizeof(DIDEVICEINSTANCE.NATIVE);
            //Result result = calli(System.Int32(System.Void*,System.Void*), this._nativePointer, &_Native, *(*(IntPtr*)this._nativePointer + (IntPtr)15 * (IntPtr)sizeof(void*)));
            var hr = (HRESULT)NativeHelper.CalliInt32(15, _nativePointer, &result);
            hr.CheckError();
            return new DIDEVICEINSTANCE(&result);
        }
    }
}
