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
    /// <unmanaged>ID3DXSprite</unmanaged>	
    [Guid("ba0b762d-7d28-43ec-b9dc-2f84443b0614")]
    public class D3DXSprite : ComObject
    {
        public D3DXSprite(IntPtr nativePointer)
            : base(nativePointer)
        {
        }


        /// <unmanaged>HRESULT ID3DXSprite::Draw([In] IDirect3DTexture9* pTexture,[In] const void* pSrcRect,[In] const void* pCenter,[In] const void* pPosition,[In] D3DCOLOR Color)</unmanaged>	
        private unsafe HRESULT Draw(Direct3DTexture9 texture, IntPtr srcRect, IntPtr center, IntPtr position, D3DCOLOR color)
        {
	        //calli(System.Int32(System.Void*,System.Void*,System.Void*,System.Void*,System.Void*,SharpDX.Mathematics.Interop.RawColorBGRA), this._nativePointer, (void*)((textureRef == null) ? IntPtr.Zero : textureRef.NativePointer), (void*)srcRectRef, (void*)centerRef, (void*)positionRef, color, *(*(IntPtr*)this._nativePointer + (IntPtr)9 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(9, _nativePointer,
                (void*)(texture == null ? IntPtr.Zero : texture.NativePointer),
                (void*)srcRect,
                (void*)center,
                (void*)position,
                (int)color);
        }


        /// <unmanaged>HRESULT ID3DXSprite::Begin([In] D3DXSPRITE Flags)</unmanaged>	
        public unsafe HRESULT Begin(D3DXSPRITEFLAG flags = 0)
		{
			//calli(System.Int32(System.Void*,System.Int32), this._nativePointer, flags, *(*(IntPtr*)this._nativePointer + (IntPtr)8 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(8, _nativePointer, (int)flags);
		}

        /// <unmanaged>HRESULT ID3DXSprite::End()</unmanaged>	
        public unsafe HRESULT End()
        {
	        //calli(System.Int32(System.Void*), this._nativePointer, *(*(IntPtr*)this._nativePointer + (IntPtr)11 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(11, _nativePointer);
        }

        /// <unmanaged>HRESULT ID3DXSprite::SetTransform([In] const D3DXMATRIX* pTransform)</unmanaged>	
        public unsafe HRESULT SetTransform(ref D3DMATRIX transform)
        {
	        fixed(void* pTransform = &transform)
            {
		        //result = calli(System.Int32(System.Void*,System.Void*), this._nativePointer, ptr, *(*(IntPtr*)this._nativePointer + (IntPtr)5 * (IntPtr)sizeof(void*)));
                return (HRESULT)NativeHelper.CalliInt32(5, _nativePointer, (void*)pTransform);
            }
        }

        public unsafe HRESULT Draw(Direct3DTexture9 texture, D3DRECT? rect, D3DXVECTOR3? center, D3DXVECTOR3? position, D3DCOLOR color)
        {
            D3DRECT vRect = rect.HasValue ? rect.Value : default(D3DRECT);
            D3DXVECTOR3 vCenter = center.HasValue ? center.Value : default(D3DXVECTOR3);
            D3DXVECTOR3 vPosition = position.HasValue ? position.Value : default(D3DXVECTOR3);
            return Draw(
                texture, 
                rect.HasValue ? (IntPtr)(void*)&vRect : IntPtr.Zero, 
                center.HasValue ? (IntPtr)(void*)&vCenter : IntPtr.Zero, 
                position.HasValue ? (IntPtr)(void*)&vPosition : IntPtr.Zero, 
                color);
        }
    }
}
