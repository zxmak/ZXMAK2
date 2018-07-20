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


namespace ZXMAK2.DirectX.Direct3D
{
    /// <unmanaged>IDirect3DTexture9</unmanaged>	
    [Guid("85C31227-3DE5-4f00-9B3A-F11AC38C18B5")]
    public class Direct3DTexture9 : Direct3DBaseTexture9
    {
        public Direct3DTexture9(IntPtr nativePointer)
            : base(nativePointer)
        {
        }


        /// <unmanaged>HRESULT IDirect3DTexture9::GetSurfaceLevel([In] unsigned int Level,[Out] IDirect3DSurface9** ppSurfaceLevel)</unmanaged>	
        public unsafe Direct3DSurface9 GetSurfaceLevel(int level)
        {
	        var nativePointer = IntPtr.Zero;
            //result = calli(System.Int32(System.Void*,System.Int32,System.Void*), this._nativePointer, level, &zero, *(*(IntPtr*)this._nativePointer + (IntPtr)18 * (IntPtr)sizeof(void*)));
            var hr = (HRESULT)NativeHelper.CalliInt32(18, _nativePointer, (int)level, (void*)&nativePointer);
            hr.CheckError();
            if (nativePointer == IntPtr.Zero) return null;
            return new Direct3DSurface9(nativePointer);
        }


        /// <unmanaged>HRESULT IDirect3DTexture9::LockRect([In] unsigned int Level,[Out] D3DLOCKED_RECT* pLockedRect,[In] const void* pRect,[In] D3DLOCK Flags)</unmanaged>	
        public unsafe HRESULT LockRectangle(int level, out D3DLOCKED_RECT lockedRect, IntPtr pRect, D3DLOCK flags)
		{
			lockedRect = default(D3DLOCKED_RECT);
            fixed (void* pLockedRect = &lockedRect)
			{
				//result = calli(System.Int32(System.Void*,System.Int32,System.Void*,System.Void*,System.Int32), this._nativePointer, level, ptr, (void*)rectRef, flags, *(*(IntPtr*)this._nativePointer + (IntPtr)19 * (IntPtr)sizeof(void*)));
                return (HRESULT)NativeHelper.CalliInt32(19, _nativePointer, (int)level, (void*)pLockedRect, (void*)pRect, (int)flags);
			}
		}

        public D3DLOCKED_RECT LockRectangle(int level, D3DLOCK flags)
        {
            D3DLOCKED_RECT lockedRect;
            var hr = LockRectangle(level, out lockedRect, IntPtr.Zero, flags);
            hr.CheckError();
            return lockedRect;
        }

        /// <unmanaged>HRESULT IDirect3DTexture9::UnlockRect([In] unsigned int Level)</unmanaged>	
        public unsafe HRESULT UnlockRectangle(int level)
		{
			//calli(System.Int32(System.Void*,System.Int32), this._nativePointer, level, *(*(IntPtr*)this._nativePointer + (IntPtr)20 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(20, _nativePointer, (int)level);
		}
    }
}
