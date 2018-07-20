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
    /// <unmanaged>IDirect3DSwapChain9</unmanaged>	
    [Guid("794950F2-ADFC-458a-905E-10A10B0B503B")]
    public class Direct3DSwapChain9 : ComObject
    {
        public Direct3DSwapChain9(IntPtr nativePointer)
            : base(nativePointer)
        {
        }

        
        /// <unmanaged>HRESULT IDirect3DSwapChain9::GetBackBuffer([In] unsigned int iBackBuffer,[In] D3DBACKBUFFER_TYPE Type,[Out] IDirect3DSurface9** ppBackBuffer)</unmanaged>	
        private unsafe HRESULT GetBackBuffer(int iBackBuffer, D3DBACKBUFFER_TYPE type, out IntPtr pBackBuffer)
        {
            fixed (void* ppBackBuffer = &pBackBuffer)
            {
                //result = calli(System.Int32(System.Void*,System.Int32,System.Int32,System.Void*), this._nativePointer, iBackBuffer, type, &zero, *(*(IntPtr*)this._nativePointer + (IntPtr)5 * (IntPtr)sizeof(void*)));
                return (HRESULT)NativeHelper.CalliInt32(5, _nativePointer, (int)iBackBuffer, (int)type, ppBackBuffer);
            }
        }

        /// <unmanaged>HRESULT IDirect3DSwapChain9::Present([In, Optional] const void* pSourceRect,[InOut, Optional] const void* pDestRect,[In] HWND hDestWindowOverride,[In] const RGNDATA* pDirtyRegion,[In] unsigned int dwFlags)</unmanaged>	
        private unsafe HRESULT Present(IntPtr sourceRectRef, IntPtr destRectRef, IntPtr hDestWindowOverride, IntPtr dirtyRegionRef, int dwFlags)
        {
	        //calli(System.Int32(System.Void*,System.Void*,System.Void*,System.Void*,System.Void*,System.Int32), this._nativePointer, (void*)sourceRectRef, (void*)destRectRef, (void*)hDestWindowOverride, (void*)dirtyRegionRef, dwFlags, *(*(IntPtr*)this._nativePointer + (IntPtr)3 * (IntPtr)sizeof(void*))).CheckError();
            return NativeHelper.CalliInt32(3, _nativePointer, (void*)sourceRectRef, (void*)destRectRef, (void*)hDestWindowOverride, (void*)dirtyRegionRef, (int)dwFlags);
        }



        public Direct3DSurface9 GetBackBuffer(int iBackBuffer, D3DBACKBUFFER_TYPE type)
        {
            var nativePointer = IntPtr.Zero;
            var hr = GetBackBuffer(iBackBuffer, type, out nativePointer);
            hr.CheckError();
            if (nativePointer == IntPtr.Zero) return null;
            return new Direct3DSurface9(nativePointer);
        }

        public Direct3DSurface9 GetBackBuffer(int iBackBuffer)
        {
            return GetBackBuffer(iBackBuffer, D3DBACKBUFFER_TYPE.D3DBACKBUFFER_TYPE_MONO);
        }

        public HRESULT Present(D3DPRESENT presentFlags)
        {
            return Present(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, (int)presentFlags);
        }

        /// <unmanaged>HRESULT IDirect3DSwapChain9::GetRasterStatus([Out] D3DRASTER_STATUS* pRasterStatus)</unmanaged>	
        public unsafe HRESULT GetRasterStatus(out D3DRASTER_STATUS rasterStatus)
        {
            fixed (void* pRasterStatus = &rasterStatus)
            {
                //result = calli(System.Int32(System.Void*,System.Void*), this._nativePointer, ptr, *(*(IntPtr*)this._nativePointer + (IntPtr)6 * (IntPtr)sizeof(void*)));
                return (HRESULT)NativeHelper.CalliInt32(6, _nativePointer, (void*)pRasterStatus);
            }
        }
    }
}
