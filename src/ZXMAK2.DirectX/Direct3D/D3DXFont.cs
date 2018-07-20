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
using ZXMAK2.DirectX.Vectors;


namespace ZXMAK2.DirectX.Direct3D
{
    /// <unmanaged>ID3DXFont</unmanaged>	
    [Guid("d79dbb70-5f21-4d36-bbc2-ff525c213cdc")]
    public class D3DXFont : ComObject
    {
        public D3DXFont(IntPtr nativePointer)
            : base(nativePointer)
        {
        }

        /// <unmanaged>HRESULT ID3DXFont::GetDevice([Out] IDirect3DDevice9** ppDevice)</unmanaged>	
        private unsafe HRESULT GetDevice(out IntPtr devicePointer)
        {
            fixed (void* pDevicePointer = &devicePointer)
            {
                //result = calli(System.Int32(System.Void*,System.Void*), this._nativePointer, &zero, *(*(IntPtr*)this._nativePointer + (IntPtr)3 * (IntPtr)sizeof(void*)));
                return (HRESULT)NativeHelper.CalliInt32(3, _nativePointer, pDevicePointer);
            }
        }

        public Direct3DDevice9 Device
        {
            get
            {
                var nativePointer = IntPtr.Zero;
                var hr = GetDevice(out nativePointer);
                hr.CheckError();
                if (nativePointer == IntPtr.Zero) return null;
                return new Direct3DDevice9(nativePointer);
            }
        }

        // <unmanaged>HRESULT ID3DXFont::OnLostDevice()</unmanaged>	
        public unsafe HRESULT OnLostDevice()
        {
	        //calli(System.Int32(System.Void*), this._nativePointer, *(*(IntPtr*)this._nativePointer + (IntPtr)16 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(16, _nativePointer);
        }

        /// <unmanaged>HRESULT ID3DXFont::OnResetDevice()</unmanaged>	
        public unsafe HRESULT OnResetDevice()
        {
	        //calli(System.Int32(System.Void*), this._nativePointer, *(*(IntPtr*)this._nativePointer + (IntPtr)17 * (IntPtr)sizeof(void*))).CheckError();
            return NativeHelper.CalliInt32(17, _nativePointer);
        }

        /// <unmanaged>HDC ID3DXFont::GetDC()</unmanaged>	
        public unsafe IntPtr GetDeviceContext()
        {
	        //return calli(System.IntPtr(System.Void*), this._nativePointer, *(*(IntPtr*)this._nativePointer + (IntPtr)8 * (IntPtr)sizeof(void*)));
            return (IntPtr)NativeHelper.CalliInt32(8, _nativePointer);
        }

        /// <unmanaged>int ID3DXFont::DrawTextW([In] ID3DXSprite* pSprite,[In] const wchar_t* pString,[In] int Count,[In] void* pRect,[In] unsigned int Format,[In] D3DCOLOR Color)</unmanaged>	
        private unsafe int DrawText(D3DXSprite sprite, string text, int count, IntPtr rectRef, int format, uint color)
        {
	        var pString = Marshal.StringToHGlobalUni(text);
            //result = calli(System.Int32(System.Void*,System.Void*,System.Void*,System.Int32,System.Void*,System.Int32,SharpDX.Mathematics.Interop.RawColorBGRA), this._nativePointer, (void*)((spriteRef == null) ? IntPtr.Zero : spriteRef.NativePointer), (void*)intPtr, count, (void*)rectRef, format, color, *(*(IntPtr*)this._nativePointer + (IntPtr)15 * (IntPtr)sizeof(void*)));
            var result = NativeHelper.CalliInt32(15, _nativePointer, 
                (void*)(sprite == null ? IntPtr.Zero : sprite.NativePointer), 
                (void*)pString, 
                (int)count, 
                (void*)rectRef, 
                (int)format, 
                (int)color);
	        Marshal.FreeHGlobal(pString);
            return result;
        }

        public unsafe D3DRECT MeasureText(D3DXSprite sprite, string text, DT drawFlags)
        {
            D3DRECT rect;
            int color = -1;
            int format = (int)(drawFlags | DT.DT_CALCRECT);
            DrawText(sprite, text, text.Length, (IntPtr)(void*)&rect, format, *(uint*)&color);
            return rect;
        }

        public unsafe int DrawText(D3DXSprite sprite, string text, D3DRECT rect, DT drawFlags, uint color)
        {
            var result = DrawText(sprite, text, text.Length, (IntPtr)(void*)&rect, (int)drawFlags, color);
            if (result == 0)
            {
                //"Draw failed"
                throw new DirectXException(0);
            }
            return result;
        }
    }
}
