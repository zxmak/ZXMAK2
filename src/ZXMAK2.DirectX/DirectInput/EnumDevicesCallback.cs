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

namespace ZXMAK2.DirectX.DirectInput
{
    internal class EnumDevicesCallback
    {
        private readonly IntPtr _nativePointer;
        private readonly DirectInputEnumDevicesDelegate _callback;

        public EnumDevicesCallback()
        {
            unsafe
            {
                _callback = new DirectInputEnumDevicesDelegate(DirectInputEnumDevicesImpl);
                _nativePointer = Marshal.GetFunctionPointerForDelegate(_callback);
                DeviceInstances = new List<DIDEVICEINSTANCE>();
            }
        }

        public IntPtr NativePointer
        {
            get { return _nativePointer; }
        }

        public List<DIDEVICEINSTANCE> DeviceInstances { get; private set; }

        // typedef BOOL (FAR PASCAL * LPDIENUMDEVICESCALLBACKW)(LPCDIDEVICEINSTANCEW, LPVOID);
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private unsafe delegate int DirectInputEnumDevicesDelegate(void* deviceInstance, IntPtr data);
        
        private unsafe int DirectInputEnumDevicesImpl(void* deviceInstance, IntPtr data)
        {
            var pNative = (DIDEVICEINSTANCE.NATIVE*)deviceInstance;
            DeviceInstances.Add(new DIDEVICEINSTANCE(pNative));
            // Return true to continue iterating
            return 1;
        }
    }
}
