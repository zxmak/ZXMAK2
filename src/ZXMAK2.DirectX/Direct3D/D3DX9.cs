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
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using ZXMAK2.DirectX.Direct3D;
using ZXMAK2.DirectX.Native;
using ZXMAK2.DirectX.Vectors;


namespace ZXMAK2.DirectX.Direct3D
{
    public static class D3DX9
    {
        public unsafe static D3DXSprite CreateSprite(Direct3DDevice9 device)
        {
            var nativePointer = IntPtr.Zero;
            var hr = (HRESULT)NativeMethods.D3DXCreateSprite_(
                (void*)(device == null ? IntPtr.Zero : device.NativePointer),
                (void*)&nativePointer);
            hr.CheckError();
            if (nativePointer == null) return null;
            return new D3DXSprite(nativePointer);
        }

        public unsafe static Direct3DTexture9 CreateTexture(Direct3DDevice9 device, int width, int height, int mipLevels, int usage, D3DFORMAT format, D3DPOOL pool)
        {
            var nativePointer = IntPtr.Zero;
            var hr = (HRESULT)NativeMethods.D3DXCreateTexture_(
                (void*)((device == null) ? IntPtr.Zero : device.NativePointer),
                width, height, mipLevels,
                (int)usage, (int)format, (int)pool,
                (void*)&nativePointer);
            hr.CheckError();
            if (nativePointer == null) return null;
            return new Direct3DTexture9(nativePointer);
        }

        public unsafe static Direct3DTexture9 CreateTextureFromFileInMemory(Direct3DDevice9 device, byte[] data)
        {
            var nativePointer = IntPtr.Zero;
            HRESULT hr;
            fixed (void* pData = data)
            {
                hr = NativeMethods.D3DXCreateTextureFromFileInMemory_(
                    (void*)(device == null ? IntPtr.Zero : device.NativePointer),
                    (void*)pData,
                    (int)data.Length,
                    (void*)&nativePointer);
            }
            hr.CheckError();
            if (nativePointer == IntPtr.Zero) return null;
            return new Direct3DTexture9(nativePointer);
        }

        public static Direct3DTexture9 CreateTextureFromStream(Direct3DDevice9 device, Stream stream)
        {
            var length = (int)stream.Length;
            var data = new byte[length];
	        var read = 0;
	        if (length > 0)
	        {
		        do
		        {
			        read += stream.Read(data, read, length - read);
		        }
		        while (read < length);
            }
            return CreateTextureFromFileInMemory(device, data);
        }


        public unsafe static D3DXFont CreateFont(Direct3DDevice9 device, int height, int width, int weight, int mipLevels, RawBool italic, int charSet, int outputPrecision, int quality, int pitchAndFamily, string faceName)
        {
            var pFaceName = Marshal.StringToHGlobalUni(faceName);
            var nativePointer = IntPtr.Zero;
            var hr = (HRESULT)NativeMethods.D3DXCreateFontW_(
                (void*)(device == null ? IntPtr.Zero : device.NativePointer), 
                height, width, weight, mipLevels, italic, charSet, outputPrecision, quality, pitchAndFamily,
                (void*)pFaceName, 
                (void*)&nativePointer);
            Marshal.FreeHGlobal(pFaceName);
            hr.CheckError();
            if (nativePointer == null) return null;
            return new D3DXFont(nativePointer);
        }    
    }
}
