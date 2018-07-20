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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace ZXMAK2.DirectX.DirectInput
{
    // dinput.h / DIDEVICEINSTANCEW
    // x64 sizeof = 1100
    // x86 sizeof = 1100
    public class DIDEVICEINSTANCE
    {
        public readonly int dwSize;
        public Guid guidInstance;
        public Guid guidProduct;
        public int dwDevType;
        public string tszInstanceName;
        public string tszProductName;

        public Guid guidFFDriver;
        public ushort wUsagePage;
        public ushort wUsage;


        public unsafe DIDEVICEINSTANCE()
        {
            dwSize = sizeof(NATIVE);
        }

        internal unsafe DIDEVICEINSTANCE(NATIVE* di)
        {
            dwSize = di->dwSize;
            guidInstance = di->guidInstance;
            guidProduct = di->guidProduct;
            dwDevType = di->dwDevType;
            tszInstanceName = di->tszInstanceName.GetStringUni();
            tszProductName = di->tszProductName.GetStringUni();
            guidFFDriver = di->guidFFDriver;
            wUsagePage = di->wUsagePage;
            wUsage = di->wUsage;
        }

        internal unsafe void MarshalTo(NATIVE* di)
        {
            di->dwSize = dwSize;
            di->guidInstance = guidInstance;
            di->guidProduct = guidProduct;
            di->dwDevType = dwDevType;
            di->tszInstanceName.SetStringUni(tszInstanceName);
            di->tszProductName.SetStringUni(tszProductName);
            di->guidFFDriver = guidFFDriver;
            di->wUsagePage = wUsagePage;
            di->wUsage = wUsage;
        }



        [StructLayout(LayoutKind.Sequential)]
        internal struct NATIVE
        {
            public int dwSize;                      // DWORD
            public Guid guidInstance;               // GUID
            public Guid guidProduct;                // GUID
            public int dwDevType;                   // DWORD
            public tszString260 tszInstanceName;    // WCHAR   tszInstanceName[MAX_PATH=260]; 
            public tszString260 tszProductName;     // WCHAR   tszProductName[MAX_PATH=260];
//#if(DIRECTINPUT_VERSION >= 0x0500)
            public Guid guidFFDriver;               // GUID
            public ushort wUsagePage;               // WORD
            public ushort wUsage;                   // WORD
//#endif /* DIRECTINPUT_VERSION >= 0x0500 */
        }

        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        internal struct tszString260
        {
            private tszString20 v00;
            private tszString20 v01;
            private tszString20 v02;
            private tszString20 v03;
            private tszString20 v04;
            private tszString20 v05;
            private tszString20 v06;
            private tszString20 v07;
            private tszString20 v08;
            private tszString20 v09;
            private tszString20 v10;
            private tszString20 v11;
            private tszString20 v12;

            public unsafe string GetStringUni()
            {
                fixed (void* pStrUni = &this)
                {
                    char* pUni = (char*)pStrUni;
                    var maxLength = sizeof(DIDEVICEINSTANCE.tszString260);                    
                    var length = 0;
                    for (; length < maxLength && pUni[length] != 0; length++) ;
                    return Marshal.PtrToStringUni((IntPtr)pStrUni, length);
                }
            }

            public unsafe void SetStringUni(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    fixed (void* pDst = &this)
                    {
                        *(ushort*)pDst = (ushort)0;
                    }
                    return;
                }
                fixed (char* pStr = value)
                {
                    var maxLength = sizeof(DIDEVICEINSTANCE.tszString260);
                    var length = Math.Min(value.Length * 2, maxLength);
                    char* pSrc = pStr;
                    if (pSrc != null)
                    {
                        pSrc += RuntimeHelpers.OffsetToStringData / 2;
                    }
                    fixed (void* pDst = &this)
                    {
                        Native.NativeHelper.CPBLK(pDst, pSrc, length);
                    }
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack=2)]
        internal struct tszString20
        {
            private ushort v00;
            private ushort v01;
            private ushort v02;
            private ushort v03;
            private ushort v04;
            private ushort v05;
            private ushort v06;
            private ushort v07;
            private ushort v08;
            private ushort v09;
            private ushort v10;
            private ushort v11;
            private ushort v12;
            private ushort v13;
            private ushort v14;
            private ushort v15;
            private ushort v16;
            private ushort v17;
            private ushort v18;
            private ushort v19;
        }
    }
}
