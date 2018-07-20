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
    // <unmanaged>IDirectSoundBuffer8</unmanaged>	
    [Guid("6825a449-7524-4d82-920f-50e36ab3ab1e")]
    public class SecondarySoundBuffer : SoundBuffer
    {
        public SecondarySoundBuffer(IntPtr nativePtr)
            : base(nativePtr)
        {
        }

        // <unmanaged>HRESULT IDirectSoundBuffer8::SetFX([In] unsigned int dwEffectsCount,[In, Buffer, Optional] DSEFFECTDESC* pDSFXDesc,[InOut, Buffer] DSOUND_ENUM_0* pdwResultCodes)</unmanaged>	
        internal unsafe void SetEffect(int effectsCount, DSEFFECTDESC[] dSFXDescRef, DSFX_I3DL2_MATERIAL_PRESET[] resultCodesRef)
		{
			DSEFFECTDESC.__Native[] array = (dSFXDescRef == null) ? null : new DSEFFECTDESC.__Native[dSFXDescRef.Length];
			if (dSFXDescRef != null)
			{
				for (int i = 0; i < dSFXDescRef.Length; i++)
				{
					dSFXDescRef[i].__MarshalTo(ref array[i]);
				}
			}
			HRESULT result;
			fixed (DSEFFECTDESC.__Native* ptr = array)
            fixed (DSFX_I3DL2_MATERIAL_PRESET* ptr2 = resultCodesRef)
			{
                //result = calli(System.Int32(System.Void*,System.Int32,System.Void*,System.Void*), this._nativePointer, effectsCount, ptr, ptr2, *(*(IntPtr*)this._nativePointer + (IntPtr)21 * (IntPtr)sizeof(void*)));
                result = (HRESULT)NativeHelper.CalliInt32(21, _nativePointer, effectsCount, (void*)ptr, (void*)ptr2);
			}
			if (dSFXDescRef != null)
			{
				for (int j = 0; j < dSFXDescRef.Length; j++)
				{
					dSFXDescRef[j].__MarshalFree(ref array[j]);
				}
			}
			result.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer8::AcquireResources([In] unsigned int dwFlags,[In] unsigned int dwEffectsCount,[Out, Buffer] unsigned int* pdwResultCodes)</unmanaged>	
        public unsafe void AcquireResources(int flags, int effectsCount, int[] resultCodesRef)
		{
			
            HRESULT result;
			fixed (int* ptr = resultCodesRef)
			{
				//result = calli(System.Int32(System.Void*,System.Int32,System.Int32,System.Void*), this._nativePointer, flags, effectsCount, ptr, *(*(IntPtr*)this._nativePointer + (IntPtr)22 * (IntPtr)sizeof(void*)));
                result = (HRESULT)NativeHelper.CalliInt32(22, _nativePointer, flags, effectsCount, (void*)ptr);
			}
			result.CheckError();
		}

        // <unmanaged>HRESULT IDirectSoundBuffer8::GetObjectInPath([In] const GUID&amp; rguidObject,[In] unsigned int dwIndex,[In] const GUID&amp; rguidInterface,[Out] void** ppObject)</unmanaged>	
        internal unsafe void GetEffect(Guid rguidObject, int index, Guid rguidInterface, out IntPtr objectOut)
		{
            IntPtr pObject;
            //result = calli(System.Int32(System.Void*,System.Void*,System.Int32,System.Void*,System.Void*), this._nativePointer, &rguidObject, index, &rguidInterface, ptr, *(*(IntPtr*)this._nativePointer + (IntPtr)23 * (IntPtr)sizeof(void*)));
            var result = (HRESULT)NativeHelper.CalliInt32(23, _nativePointer, (void*)&rguidObject, index, (void*)&rguidInterface, (void*)&pObject);
            objectOut = pObject;
			result.CheckError();
		}

        public SecondarySoundBuffer(DirectSoundBase dSound, DSBUFFERDESC bufferDescription)
            : base(IntPtr.Zero)
        {
            IntPtr intPtr;
            dSound.CreateSoundBuffer(bufferDescription, out intPtr, null);
            base.NativePointer = intPtr;
            base.QuerySelfInterfaceFrom<SecondarySoundBuffer>(this);
            Marshal.Release(intPtr);
        }

        // <unmanaged>HRESULT IDirectSoundCaptureBuffer8::GetObjectInPath([In] GUID* rguidObject,[None] int dwIndex,[In] GUID* rguidInterface,[Out] void** ppObject)</unmanaged>
        public T GetEffect<T>(int index) where T : ComObject
        {
            IntPtr comObjectPtr;
            this.GetEffect(NativeMethods.AllObjects, index, typeof(T).GUID, out comObjectPtr);
            //return CppObject.FromPointer<T>(comObjectPtr);
            if (comObjectPtr == IntPtr.Zero)
                return default(T);
            return (T)((object)Activator.CreateInstance(typeof(T), new object[] { comObjectPtr }));
        }

        // <unmanaged>HRESULT IDirectSoundBuffer8::SetFX([None] int dwEffectsCount,[In, Buffer, Optional] LPDSEFFECTDESC pDSFXDesc,[Out, Buffer, Optional] int* pdwResultCodes)</unmanaged>
        public DSFX_I3DL2_MATERIAL_PRESET[] SetEffect(Guid[] effects)
        {
            if (effects == null || effects.Length == 0)
            {
                this.SetEffect(0, null, new DSFX_I3DL2_MATERIAL_PRESET[1]);
                return new DSFX_I3DL2_MATERIAL_PRESET[0];
            }
            var array = new DSEFFECTDESC[effects.Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new DSEFFECTDESC();
                array[i].dwFlags = 0;
                array[i].guidDSFXClass = effects[i];
            }
            var array2 = new DSFX_I3DL2_MATERIAL_PRESET[effects.Length];
            this.SetEffect(effects.Length, array, array2);
            return array2;
        }
    }
}
