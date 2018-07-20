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
 *  Date: 10.07.2018
 */
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace ZXMAK2.DirectX.DirectSound
{
    internal class EnumDelegateCallback
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DirectSoundEnumDelegate(IntPtr guid, IntPtr description, IntPtr module, IntPtr lpContext);

        private IntPtr _nativePointer;

        private EnumDelegateCallback.DirectSoundEnumDelegate _callback;

        public IntPtr NativePointer
        {
            get { return this._nativePointer; }
        }

        public List<DeviceInformation> Informations
        {
            get;
            private set;
        }

        public EnumDelegateCallback()
        {
            this._callback = new EnumDelegateCallback.DirectSoundEnumDelegate(this.DirectSoundEnumImpl);
            this._nativePointer = Marshal.GetFunctionPointerForDelegate(this._callback);
            this.Informations = new List<DeviceInformation>();
        }

        private unsafe int DirectSoundEnumImpl(IntPtr guidPtr, IntPtr description, IntPtr module, IntPtr lpContext)
        {
            Guid driverGuid;
            if (guidPtr == IntPtr.Zero)
            {
                driverGuid = Guid.Empty;
            }
            else
            {
                driverGuid = *(Guid*)((void*)guidPtr);
            }
            this.Informations.Add(new DeviceInformation(driverGuid, Marshal.PtrToStringUni(description), Marshal.PtrToStringUni(module)));
            return 1;
        }
    }
}
