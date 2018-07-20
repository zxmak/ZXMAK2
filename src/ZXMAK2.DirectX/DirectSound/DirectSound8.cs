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
using ZXMAK2.DirectX.Native;


namespace ZXMAK2.DirectX.DirectSound
{
    // <unmanaged>IDirectSound8</unmanaged>
    [Guid("c50a7e93-f395-4834-9ef6-7fa99de50966")]
    public class DirectSound8 : DirectSoundBase
    {
        public DirectSound8()
            : base(IntPtr.Zero)
        {
            Create8(null, this, null);            
        }

		public DirectSound8(Guid driverGuid) 
            : base(IntPtr.Zero)
		{
			Create8(new Guid?(driverGuid), this, null);
		}

        public DirectSound8(IntPtr nativePtr) 
            : base(nativePtr)
		{
		}

        /// <summary>
        /// Enumerates the DirectSound devices installed in the system.
        /// </summary>
        public static List<DeviceInformation> GetDevices()
        {
            var enumDelegateCallback = new EnumDelegateCallback();
            EnumerateW(enumDelegateCallback.NativePointer, IntPtr.Zero);
            return enumDelegateCallback.Informations;
        }


        private unsafe static void Create8(Guid? cGuidDeviceRef, DirectSound8 dS8Out, ComObject unkOuterRef)
        {
            Guid value;
            if (cGuidDeviceRef.HasValue)
            {
                value = cGuidDeviceRef.Value;
            }
            IntPtr zero = IntPtr.Zero;
            var result = (HRESULT)NativeMethods.DirectSoundCreate8_(cGuidDeviceRef.HasValue ? ((void*)(&value)) : ((void*)IntPtr.Zero), (void*)(&zero), (void*)((unkOuterRef == null) ? IntPtr.Zero : unkOuterRef.NativePointer));
            dS8Out.NativePointer = zero;
            result.CheckError();
        }

        private unsafe static void EnumerateW(FunctionCallback dSEnumCallbackRef, IntPtr contextRef)
        {
            var hr = (HRESULT)NativeMethods.DirectSoundEnumerateW_(dSEnumCallbackRef, (void*)contextRef);
            hr.CheckError();
        }
    }
}
