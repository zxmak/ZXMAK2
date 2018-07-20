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


namespace ZXMAK2.DirectX.DirectInput
{
    // dinput.h
    // x64 sizeof = 32
    // x86 sizeof = 24
    [StructLayout(LayoutKind.Sequential)]
    public struct DIDATAFORMAT
    {
        public readonly int dwSize;               // DWORD
        public readonly int dwObjSize;            // DWORD
        public readonly DIDF dwFlags;             // DWORD
        public readonly int dwDataSize;           // DWORD
        public readonly int dwNumObjs;            // DWORD
        public readonly IntPtr rgodf;             // LPDIOBJECTDATAFORMAT


        public DIDATAFORMAT(int size, int objSize, DIDF flags, int dataSize, int numObjs, DIOBJECTDATAFORMAT[] objArray)
        {
            if (numObjs != objArray.Length)
                throw new ArgumentException();
            dwSize = size;
            dwObjSize = objSize;
            dwFlags = flags;
            dwDataSize = dataSize;
            dwNumObjs = numObjs;
            rgodf = GCHandle.Alloc(objArray, GCHandleType.Pinned).AddrOfPinnedObject();
        }


        // http://xaos.sourceforge.net/dxguid.c
        //public unsafe static readonly DIDATAFORMAT c_dfDIMouse = new DIDATAFORMAT(
        //    sizeof(DIDATAFORMAT), sizeof(DIOBJECTDATAFORMAT), 
        //    (DIDF)2, 16, 7, DIOBJECTDATAFORMAT.rgodf_c_dfDIMouse);
        public unsafe static readonly DIDATAFORMAT c_dfDIMouse = new DIDATAFORMAT(
            sizeof(DIDATAFORMAT), sizeof(DIOBJECTDATAFORMAT), 
            (DIDF)2, 20, 11, DIOBJECTDATAFORMAT.rgodf_c_dfDIMouse);

        public unsafe static readonly DIDATAFORMAT c_dfDIKeyboard = new DIDATAFORMAT(
            sizeof(DIDATAFORMAT), sizeof(DIOBJECTDATAFORMAT), 
            (DIDF)2, 256, 256, DIOBJECTDATAFORMAT.rgodf_c_dfDIKeyboard);
        public unsafe static readonly DIDATAFORMAT c_dfDIJoystick = new DIDATAFORMAT(
            sizeof(DIDATAFORMAT), sizeof(DIOBJECTDATAFORMAT), 
            (DIDF)1, 80, 44, DIOBJECTDATAFORMAT.rgodf_c_dfDIJoystick);
        //public unsafe static readonly DIDATAFORMAT c_dfDIJoystick2 = new DIDATAFORMAT(
        //    sizeof(DIDATAFORMAT), sizeof(DIOBJECTDATAFORMAT), 
        //    (DIDF)1, 272, 164, DIOBJECTDATAFORMAT.rgodf_c_dfDIJoystick2);

    }
}
