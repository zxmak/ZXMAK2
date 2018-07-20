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
    /// <unmanaged>IDirect3D9Ex</unmanaged>	
    [Guid("02177241-69FC-400C-8FF1-93A44DF6861D")]
    public class Direct3D9Ex : Direct3D9
    {
        public unsafe Direct3D9Ex() 
            : base(IntPtr.Zero)
        {
            IntPtr nativePointer;
            var hr = (HRESULT)NativeMethods.Direct3DCreate9Ex_(D3D_SDK_VERSION, (void*)&nativePointer);
            hr.CheckError();
            base.NativePointer = nativePointer;
        }
    }
}
