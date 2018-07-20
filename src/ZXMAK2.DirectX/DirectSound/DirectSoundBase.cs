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
    // <unmanaged>IDirectSound</unmanaged>
    [Guid("279afa83-4981-11ce-a521-0020af0be560")]
    public class DirectSoundBase : ComObject
    {
        public DirectSoundBase(IntPtr nativePointer)
            : base(nativePointer)
        {
        }

        // <unmanaged>HRESULT IDirectSound::GetCaps([Out] DSCAPS* pDSCaps)</unmanaged>
        public DSCAPS Capabilities
        {
            get
            {
                DSCAPS result;
                this.GetCapabilities(out result);
                return result;
            }
        }

        // <unmanaged>HRESULT IDirectSound::CreateSoundBuffer([In] const DSBUFFERDESC* pcDSBufferDesc,[Out] void** ppDSBuffer,[In] IUnknown* pUnkOuter)</unmanaged>	
        internal unsafe void CreateSoundBuffer(DSBUFFERDESC cDSBufferDescRef, out IntPtr dSBufferOut, ComObject unkOuterRef)
        {
            DSBUFFERDESC.__Native _Native = DSBUFFERDESC.__NewNative();
            cDSBufferDescRef.__MarshalTo(ref _Native);
            IntPtr pDSBuffer;
            //result = calli(System.Int32(System.Void*,System.Void*,System.Void*,System.Void*), this._nativePointer, &_Native, ptr, (void*)((unkOuterRef == null) ? IntPtr.Zero : unkOuterRef.NativePointer), *(*(IntPtr*)this._nativePointer + (IntPtr)3 * (IntPtr)sizeof(void*)));
            var result = (HRESULT)NativeHelper.CalliInt32(3, _nativePointer, &_Native, &pDSBuffer, (void*)((unkOuterRef == null) ? IntPtr.Zero : unkOuterRef.NativePointer));
            dSBufferOut = pDSBuffer;
            cDSBufferDescRef.__MarshalFree(ref _Native);
            result.CheckError();
        }

        // <unmanaged>HRESULT IDirectSound::GetCaps([Out] DSCAPS* pDSCaps)</unmanaged>	
        internal unsafe void GetCapabilities(out DSCAPS dSCapsRef)
		{
			DSCAPS.__Native _Native = DSCAPS.__NewNative();
			//Result result = calli(System.Int32(System.Void*,System.Void*), this._nativePointer, &_Native, *(*(IntPtr*)this._nativePointer + (IntPtr)4 * (IntPtr)sizeof(void*)));
            var result = (HRESULT)NativeHelper.CalliInt32(4, _nativePointer, &_Native);
			dSCapsRef = new DSCAPS();
			dSCapsRef.__MarshalFrom(ref _Native);
			result.CheckError();
		}

        // <unmanaged>HRESULT IDirectSound::DuplicateSoundBuffer([In] IDirectSoundBuffer* pDSBufferOriginal,[Out] void** ppDSBufferDuplicate)</unmanaged>	
        internal unsafe HRESULT DuplicateSoundBuffer(SoundBuffer dSBufferOriginalRef, out IntPtr dSBufferDuplicateOut)
		{
			IntPtr pDSBufferDuplicate;
            //result = calli(System.Int32(System.Void*,System.Void*,System.Void*), this._nativePointer, (void*)((dSBufferOriginalRef == null) ? IntPtr.Zero : dSBufferOriginalRef.NativePointer), ptr, *(*(IntPtr*)this._nativePointer + (IntPtr)5 * (IntPtr)sizeof(void*)));
            var result = (HRESULT)NativeHelper.CalliInt32(5, _nativePointer, (void*)((dSBufferOriginalRef == null) ? IntPtr.Zero : dSBufferOriginalRef.NativePointer), &pDSBufferDuplicate);
            dSBufferDuplicateOut = pDSBufferDuplicate;
			return result;
		}

        // <unmanaged>HRESULT IDirectSound::SetCooperativeLevel([In] HWND hwnd,[In] DSSCL_ENUM dwLevel)</unmanaged>	
        public unsafe void SetCooperativeLevel(IntPtr hwnd, DSSCL level)
        {
            //calli(System.Int32(System.Void*,System.Void*,System.Int32), this._nativePointer, (void*)hwnd, level, *(*(IntPtr*)this._nativePointer + (IntPtr)6 * (IntPtr)sizeof(void*))).CheckError();
            var hr = (HRESULT)NativeHelper.CalliInt32(6, this._nativePointer, (void*)hwnd, (int)level);
            hr.CheckError();
        }

        // <unmanaged>HRESULT IDirectSound::Compact()</unmanaged>	
        public unsafe void Compact()
		{
			//calli(System.Int32(System.Void*), this._nativePointer, *(*(IntPtr*)this._nativePointer + (IntPtr)7 * (IntPtr)sizeof(void*))).CheckError();
            var hr = (HRESULT)NativeHelper.CalliInt32(7, _nativePointer);
            hr.CheckError();
		}

        // <unmanaged>HRESULT IDirectSound::GetSpeakerConfig([Out] unsigned int* pdwSpeakerConfig)</unmanaged>	
        internal unsafe void GetSpeakerConfiguration(out int speakerConfigRef)
		{
            int dwSpeakerConfig;
            //result = calli(System.Int32(System.Void*,System.Void*), this._nativePointer, ptr, *(*(IntPtr*)this._nativePointer + (IntPtr)8 * (IntPtr)sizeof(void*)));
            var result = (HRESULT)NativeHelper.CalliInt32(8, _nativePointer, &dwSpeakerConfig);
            speakerConfigRef = dwSpeakerConfig;
			result.CheckError();
		}

        // <unmanaged>HRESULT IDirectSound::SetSpeakerConfig([In] unsigned int dwSpeakerConfig)</unmanaged>	
        internal unsafe void SetSpeakerConfiguration(int speakerConfig)
		{
			//calli(System.Int32(System.Void*,System.Int32), this._nativePointer, speakerConfig, *(*(IntPtr*)this._nativePointer + (IntPtr)9 * (IntPtr)sizeof(void*))).CheckError();
            var hr = (HRESULT)NativeHelper.CalliInt32(9, _nativePointer, speakerConfig); 
		}
    }
}
