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


namespace ZXMAK2.DirectX.DirectInput
{
    // x64 sizeof = 80
    // x86 sizeof = 80
    [StructLayout(LayoutKind.Sequential)]
    public struct DIJOYSTATE
    {
        public int lX;              // LONG
        public int lY;              // LONG
        public int lZ;              // LONG
        public int lRx;             // LONG
        public int lRy;             // LONG
        public int lRz;             // LONG
        public int rglSlider0;      // int rglSlider[2]
        public int rglSlider1;
        public uint rgdwPOV0;       // DWORD rgdwPOV[4];
        public uint rgdwPOV1;
        public uint rgdwPOV2;
        public uint rgdwPOV3;
        public byte rgbButtons0;    // BYTE rgbButtons[32];
        public byte rgbButtons1;
        public byte rgbButtons2;
        public byte rgbButtons3;
        public byte rgbButtons4;
        public byte rgbButtons5;
        public byte rgbButtons6;
        public byte rgbButtons7;
        public byte rgbButtons8;
        public byte rgbButtons9;
        public byte rgbButtons10;
        public byte rgbButtons11;
        public byte rgbButtons12;
        public byte rgbButtons13;
        public byte rgbButtons14;
        public byte rgbButtons15;
        public byte rgbButtons16;
        public byte rgbButtons17;
        public byte rgbButtons18;
        public byte rgbButtons19;
        public byte rgbButtons20;
        public byte rgbButtons21;
        public byte rgbButtons22;
        public byte rgbButtons23;
        public byte rgbButtons24;
        public byte rgbButtons25;
        public byte rgbButtons26;
        public byte rgbButtons27;
        public byte rgbButtons28;
        public byte rgbButtons29;
        public byte rgbButtons30;
        public byte rgbButtons31;

        
        public unsafe byte[] GetButtons()
        {
            var offset = 8 * 4 + 4 * 4; // rgbButtons0 offset
            var length = sizeof(DIJOYSTATE) - offset;
            var button = new byte[length];
            fixed (void* pStruct = &this)
            fixed (void* pButton = button)
            {
                NativeHelper.CPBLK(pButton, (byte*)pStruct + offset, length);            
            }
            return button;
        }
    }
}
