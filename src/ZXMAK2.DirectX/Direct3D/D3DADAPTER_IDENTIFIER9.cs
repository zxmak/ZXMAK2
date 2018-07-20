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
using ZXMAK2.DirectX.Native;


namespace ZXMAK2.DirectX.Direct3D
{
    // d3d9types.h
    public class D3DADAPTER_IDENTIFIER9
    {
        public string Driver;
        public string Description;
        public string DeviceName;
        public long DriverVersion;
        public int VendorId;
        public int DeviceId;
        public int SubSysId;
        public int Revision;
        public Guid DeviceIdentifier;
        public int WHQLLevel;

        
        internal unsafe D3DADAPTER_IDENTIFIER9(NATIVE* ai)
        {
            MarshalFrom(ai);
        }

        internal unsafe void MarshalTo(NATIVE* ai)
        {
            ai->Driver.SetStringAnsi(this.Driver);
            ai->Description.SetStringAnsi(this.Description);
            ai->DeviceName.SetStringAnsi(this.DeviceName);
            ai->DriverVersion = this.DriverVersion;
            ai->VendorId = this.VendorId;
            ai->DeviceId = this.DeviceId;
            ai->SubSysId = this.SubSysId;
            ai->Revision = this.Revision;
            ai->DeviceIdentifier = this.DeviceIdentifier;
            ai->WHQLLevel = this.WHQLLevel;
        }

        internal unsafe void MarshalFrom(NATIVE* ai)
        {
            this.Driver = ai->Driver.GetStringAnsi();
            this.Description = ai->Description.GetStringAnsi();
            this.DeviceName = ai->DeviceName.GetStringAnsi();
            this.DriverVersion = ai->DriverVersion;
            this.VendorId = ai->VendorId;
            this.DeviceId = ai->DeviceId;
            this.SubSysId = ai->SubSysId;
            this.Revision = ai->Revision;
            this.DeviceIdentifier = ai->DeviceIdentifier;
            this.WHQLLevel = ai->WHQLLevel;
        }


        [StructLayout(LayoutKind.Sequential)]
        internal struct NATIVE
        {
                                                    // MAX_DEVICE_IDENTIFIER_STRING = 512
            public tszString512 Driver;             // char Driver[MAX_DEVICE_IDENTIFIER_STRING]
            public tszString512 Description;        // char Description[MAX_DEVICE_IDENTIFIER_STRING]
            
            // Device name for GDI (ex. \\.\DISPLAY1)
            public tszString32 DeviceName;          // char DeviceName[32];

            // new Version((int)(this.DriverVersion >> 48) & 65535, (int)(this.DriverVersion >> 32) & 65535, (int)(this.DriverVersion >> 16) & 65535, (int)this.DriverVersion & 65535);
            public long DriverVersion;              // LARGE_INTEGER
            public int VendorId;                    // DWORD
            public int DeviceId;                    // DWORD
            public int SubSysId;                    // DWORD
            public int Revision;                    // DWORD
            public Guid DeviceIdentifier;           // GUID
            public int WHQLLevel;                   // DWORD
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        internal struct tszString512
        {
            private tszString32 v00;
            private tszString32 v01;
            private tszString32 v02;
            private tszString32 v03;
            private tszString32 v04;
            private tszString32 v05;
            private tszString32 v06;
            private tszString32 v07;
            private tszString32 v08;
            private tszString32 v09;
            private tszString32 v10;
            private tszString32 v11;
            private tszString32 v12;
            private tszString32 v13;
            private tszString32 v14;
            private tszString32 v15;

            public unsafe string GetStringAnsi()
            {
                fixed (void* pStrSrc = &this)
                {
                    byte* pSrc = (byte*)pStrSrc;
                    var maxLength = Marshal.SizeOf(this);
                    var length = 0;
                    for (; length < maxLength && pSrc[length] != 0; length++) ;
                    return Marshal.PtrToStringAnsi((IntPtr)pSrc, length);
                }
            }

            public unsafe void SetStringAnsi(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    fixed (void* pDst = &this)
                    {
                        *(byte*)pDst = (byte)0;
                    }
                    return;
                }
                var maxLength = Marshal.SizeOf(this);
                var length = Math.Min(value.Length, maxLength);
                var pSrc = Marshal.StringToHGlobalAnsi(value);
                fixed (void* pDst = &this)
                {
                    NativeHelper.CPBLK(pDst, (void*)pSrc, length);
                }
                Marshal.FreeHGlobal(pSrc);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        internal struct tszString32
        {
            private tszString8 v00;
            private tszString8 v01;
            private tszString8 v02;
            private tszString8 v03;

            public unsafe string GetStringAnsi()
            {
                fixed (void* pStrSrc = &this)
                {
                    byte* pSrc = (byte*)pStrSrc;
                    var maxLength = Marshal.SizeOf(this);
                    var length = 0;
                    for (; length < maxLength && pSrc[length] != 0; length++) ;
                    return Marshal.PtrToStringAnsi((IntPtr)pSrc, length);
                }
            }

            public unsafe void SetStringAnsi(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    fixed (void* pDst = &this)
                    {
                        *(byte*)pDst = (byte)0;
                    }
                    return;
                }
                var maxLength = Marshal.SizeOf(this);
                var length = Math.Min(value.Length, maxLength);
                var pSrc = Marshal.StringToHGlobalAnsi(value);
                fixed (void* pDst = &this)
                {
                    NativeHelper.CPBLK(pDst, (void*)pSrc, length);
                }
                Marshal.FreeHGlobal(pSrc);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack=1)]
        internal struct tszString8
        {
            private byte v00;
            private byte v01;
            private byte v02;
            private byte v03;
            private byte v04;
            private byte v05;
            private byte v06;
            private byte v07;
        }
    }
}
