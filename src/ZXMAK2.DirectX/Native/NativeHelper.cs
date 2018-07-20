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
using System.Runtime.CompilerServices;


namespace ZXMAK2.DirectX.Native
{
    public static class NativeHelper
    {
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern void INITBLK(void* dst, byte value, int length);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern void CPBLK(void* dst, void* src, int length);

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public static extern int SizeOf<T>() where T : struct;

        
        //I00
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer);
        //I01
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            void* arg0,
            void* arg1);
        //I02
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            void* arg0,
            void* arg1,
            void* arg2);
        //I03
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            void* arg0,
            int arg1);

        //I04
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            void* arg0,
            int arg1,
            void* arg2);

        //I05
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            void* arg0);

        //I06
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot, 
            void* nativePointer, 
            int arg0, 
            int arg1, 
            void* arg2, 
            void* arg3, 
            void* arg4, 
            void* arg5, 
            int arg6);

        //I06-2
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            int arg0,
            int arg1,
            int arg2,
            int arg3,
            int arg4,
            int arg5,
            void* arg6,
            void* arg7);

        //I07
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot, 
            void* nativePointer, 
            int arg0, 
            int arg1, 
            int arg2);

        //I08
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot, 
            void* nativePointer, 
            int arg0);

        //I09
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot, 
            void* nativePointer, 
            void* arg0, 
            int arg1, 
            void* arg2, 
            int arg3);
        //I10
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot, 
            void* nativePointer, 
            int arg0, 
            void* arg1);

        //I11
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot, 
            void* nativePointer, 
            int arg0, 
            void* arg1, 
            void* arg2);

        //I11-2
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            int arg0,
            void* arg1,
            void* arg2,
            int arg3);

        //I12
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot, 
            void* nativePointer, 
            int arg0, 
            int arg1, 
            void* arg2);

        //I12-2
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            int arg0,
            int arg1);

        //I13
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot, 
            void* nativePointer, 
            void* arg0, 
            int arg1, 
            void* arg2, 
            void* arg3);

        //I14
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            int arg0,
            int arg1,
            void* arg2,
            int arg3,
            void* arg4,
            void* arg5);

        //I15
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            int arg0,
            void* arg1,
            int arg2,
            int arg3,
            float arg4,
            int arg5);

        //I16
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            void* arg0,
            void* arg1,
            void* arg2,
            void* arg3);

        //I17
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            void* arg0,
            void* arg1,
            void* arg2,
            void* arg3,
            int arg4);

        //I18
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            void* arg0,
            void* arg1,
            int arg2,
            void* arg3,
            int arg4,
            int arg5);

        //I19
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            int arg0,
            int arg1,
            void* arg2,
            int arg3);

        //I20
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            int arg0,
            int arg1,
            int arg2,
            void* arg3);

        //I21
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            int arg0,
            void* arg1,
            int arg2,
            int arg3);

        //I22
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern int CalliInt32(
            int slot,
            void* nativePointer,
            //int32,int32,int32,int32,void*,void*
            int arg0,
            int arg1,
            int arg2,
            int arg3,
            void* arg4,
            void* arg5);


        //IP00
        [MethodImpl(MethodImplOptions.ForwardRef)]
        public unsafe static extern IntPtr CalliIntPtr(
            int slot, void* nativePointer);

    }
}
