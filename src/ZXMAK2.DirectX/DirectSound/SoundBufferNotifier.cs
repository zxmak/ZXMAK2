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
using System.Runtime.InteropServices;
using ZXMAK2.DirectX.Native;


namespace ZXMAK2.DirectX.DirectSound
{
    // <unmanaged>IDirectSoundNotify</unmanaged>	
    [Guid("b0210783-89cd-11d0-af08-00a0c925cd16")]
    internal class SoundBufferNotifier : ComObject
    {
        public SoundBufferNotifier(IntPtr nativePtr)
            : base(nativePtr)
        {
        }

        public unsafe void SetNotificationPositions(int positionNotifies, DSBPOSITIONNOTIFY[] cPositionNotifiesRef)
		{
			DSBPOSITIONNOTIFY.__Native[] array = new DSBPOSITIONNOTIFY.__Native[cPositionNotifiesRef.Length];
			for (int i = 0; i < cPositionNotifiesRef.Length; i++)
			{
				cPositionNotifiesRef[i].__MarshalTo(ref array[i]);
			}
			HRESULT result;
			fixed (DSBPOSITIONNOTIFY.__Native* ptr = array)
			{
				//result = calli(System.Int32(System.Void*,System.Int32,System.Void*), this._nativePointer, positionNotifies, ptr, *(*(IntPtr*)this._nativePointer + (IntPtr)3 * (IntPtr)sizeof(void*)));
                result = (HRESULT)NativeHelper.CalliInt32(3, _nativePointer, positionNotifies, (void*)ptr);
			}
			for (int j = 0; j < cPositionNotifiesRef.Length; j++)
			{
				cPositionNotifiesRef[j].__MarshalFree(ref array[j]);
			}
			result.CheckError();
		}
    }
}
