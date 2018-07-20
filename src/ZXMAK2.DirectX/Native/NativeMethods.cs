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
 *  Date: 10.07.2018
 */
using System;
using System.Runtime.InteropServices;
using System.Security;
using ZXMAK2.DirectX.Vectors;


namespace ZXMAK2.DirectX.Native
{
    internal static class NativeMethods
    {
        private const string D3DX9 = "d3dx9_43";
        private const string D3D9 = "d3d9";
        private const string DSound = "dsound";
        private const string DInput8 = "dinput8";
        private const string Kernel32 = "kernel32";

        #region D3DX

        [SuppressUnmanagedCodeSecurity]
        [DllImport(D3DX9, CallingConvention = CallingConvention.StdCall, EntryPoint = "D3DXCreateSprite")]
        public static extern unsafe int D3DXCreateSprite_(void* arg0, void* arg1);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(D3DX9, CallingConvention = CallingConvention.StdCall, EntryPoint = "D3DXCreateTexture")]
        public static extern unsafe int D3DXCreateTexture_(void* arg0, int arg1, int arg2, int arg3, int arg4, int arg5, int arg6, void* arg7);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(D3DX9, CallingConvention = CallingConvention.StdCall, EntryPoint = "D3DXCreateTextureFromFileInMemory")]
        public static extern unsafe int D3DXCreateTextureFromFileInMemory_(void* arg0, void* arg1, int arg2, void* arg3);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(D3DX9, CallingConvention = CallingConvention.StdCall, EntryPoint = "D3DXCreateFontW")]
        public static extern unsafe int D3DXCreateFontW_(void* arg0, int arg1, int arg2, int arg3, int arg4, RawBool arg5, int arg6, int arg7, int arg8, int arg9, void* arg10, void* arg11);


        // Build 2D transformation matrix in XY plane.  NULL arguments are treated as identity.
        // Mout = Msc-1 * Msr-1 * Ms * Msr * Msc * Mrc-1 * Mr * Mrc * Mt
        [SuppressUnmanagedCodeSecurity]
        [DllImport(D3DX9, CallingConvention = CallingConvention.StdCall, EntryPoint = "D3DXMatrixTransformation2D")]
        public static extern unsafe D3DMATRIX* D3DXMatrixTransformation2D(
            D3DMATRIX* pOut, D3DXVECTOR2* pScalingCenter,
            float ScalingRotation, D3DXVECTOR2* pScaling,
            D3DXVECTOR2* pRotationCenter, float Rotation,
            D3DXVECTOR2* pTranslation);

        #endregion D3DX


        #region D3D9

        [SuppressUnmanagedCodeSecurity]
        [DllImport(D3D9, CallingConvention = CallingConvention.StdCall, EntryPoint = "Direct3DCreate9")]
        public static extern IntPtr Direct3DCreate9_(int sDKVersion);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(D3D9, CallingConvention = CallingConvention.StdCall, EntryPoint = "Direct3DCreate9Ex")]
        public unsafe static extern int Direct3DCreate9Ex_(int arg0, void* arg1);

        #endregion D3D9


        #region DirectSound

        public static readonly Guid AllObjects = new Guid("aa114de5-c262-4169-a1c8-23d698cc73b5");

        [SuppressUnmanagedCodeSecurity]
        [DllImport(DSound, CallingConvention = CallingConvention.StdCall, EntryPoint = "DirectSoundCreate8")]
        public unsafe static extern int DirectSoundCreate8_(void* arg0, void* arg1, void* arg2);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(DSound, CallingConvention = CallingConvention.StdCall, EntryPoint = "DirectSoundEnumerateW")]
        public unsafe static extern int DirectSoundEnumerateW_(void* arg0, void* arg1);

        #endregion DirectSound


        #region DirectInput

        [SuppressUnmanagedCodeSecurity]
        [DllImport(DInput8, CallingConvention = CallingConvention.StdCall, EntryPoint = "DirectInput8Create")]
        public unsafe static extern int DirectInput8Create_(void* arg0, int arg1, void* arg2, void* arg3, void* arg4);

        #endregion DirectInput


        #region Kernel32

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Kernel32, CharSet = CharSet.Unicode)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [SuppressUnmanagedCodeSecurity]
        [DllImport(Kernel32)]
        public static extern uint FormatMessageW(
            int dwFlags,
            IntPtr lpSource,
            int dwMessageId,
            int dwLanguageId,
            ref IntPtr lpBuffer,
            int nSize,
            IntPtr Arguments);

        #endregion Kernel32
    }
}
