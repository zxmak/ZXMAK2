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
.class private abstract auto ansi sealed beforefieldinit ZXMAK2.DirectX.Native.NativeHelper
	extends [mscorlib]System.Object
{
    .method public hidebysig static 
		void INITBLK (
			void* dst,
			uint8 'value',
			int32 length) cil managed 
	{
        .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = (01 00 00 00)
        .maxstack   3
        ldarg.0
        ldarg.1
        ldarg.2
        initblk
        ret
	} // end of method NativeHelper::INITBLK    


    .method public hidebysig static 
        void CPBLK (
            void* dst,
            void* src,
            int32 length) cil managed
    {
        .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = (01 00 00 00)
        .maxstack   3
        ldarg.s     dst
        ldarg.s     src
        ldarg.s     length
        cpblk
        ret
    } // end of method NativeHelper::CPBLK
    

    .method public hidebysig static 
	    int32 SizeOf<valuetype .ctor(System.ValueType) T> () cil managed 
    {
	    .maxstack 8
	    sizeof !!T
	    ret
    } // end of method NativeHelper::SizeOf
    

    //I00
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer) cil managed
	{
		.maxstack   4
		ldarg.1		        // native pointer
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*)
		ret
	} // end of method NativeHelper::CalliInt32

    
    //I01
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        void* arg0,
        void* arg1) cil managed
	{
		.maxstack   6
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, void*, void*)
		ret
	} // end of method NativeHelper::CalliInt32

    //I02
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        void* arg0,
        void* arg1,
        void* arg2) cil managed
	{
		.maxstack   7
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, void*, void*, void*)
		ret
	} // end of method NativeHelper::CalliInt32


    //I03
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        void* arg0,
        int32 arg1) cil managed
	{
		.maxstack   6
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, void*, int32)
		ret
	} // end of method NativeHelper::CalliInt32

    
    //I04
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        void* arg0,
        int32 arg1,
        void* arg2) cil managed
	{
		.maxstack   7
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, void*, int32, void*)
		ret
	} // end of method NativeHelper::CalliInt32


    //I05
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        void* arg0) cil managed
	{
		.maxstack   5
		ldarg.1		        // native pointer
        ldarg.2             // arg0
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, void*)
		ret
	} // end of method NativeHelper::CalliInt32


    //I06
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        int32 arg0, 
        int32 arg1, 
        void* arg2, 
        void* arg3, 
        void* arg4, 
        void* arg5, 
        int32 arg6) cil managed
	{
		.maxstack   11
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
        ldarg.s     arg3
        ldarg.s     arg4
        ldarg.s     arg5
        ldarg.s     arg6
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, int32, int32, void*, void*, void*, void*, int32)
		ret
	} // end of method NativeHelper::CalliInt32


    //I06-2
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        int32 arg0, 
        int32 arg1, 
        int32 arg2, 
        int32 arg3, 
        int32 arg4, 
        int32 arg5, 
        void* arg6,
        void* arg7) cil managed
	{
		.maxstack   12
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
        ldarg.s     arg3
        ldarg.s     arg4
        ldarg.s     arg5
        ldarg.s     arg6
        ldarg.s     arg7
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, int32, int32, int32, int32, int32, int32, void*, void*)
		ret
	} // end of method NativeHelper::CalliInt32


    //I07
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        int32 arg0,
        int32 arg1,
        int32 arg2) cil managed
	{
		.maxstack   7
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, int32, int32, int32)
		ret
	} // end of method NativeHelper::CalliInt32

    
    //I08
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        int32 arg0) cil managed
	{
		.maxstack   5
		ldarg.1		        // native pointer
        ldarg.2             // arg0
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, int32)
		ret
	} // end of method NativeHelper::CalliInt32


    //I09
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        void* arg0,
        int32 arg1,
        void* arg2,
        int32 arg3) cil managed
	{
		.maxstack   8
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
        ldarg.s     arg3
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, void*, int32, void*, int32)
		ret
	} // end of method NativeHelper::CalliInt32


    //I10
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        int32 arg0,
        void* arg1) cil managed
	{
		.maxstack   6
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, int32, void*)
		ret
	} // end of method NativeHelper::CalliInt32


    //I11
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        int32 arg0,
        void* arg1,
        void* arg2) cil managed
	{
		.maxstack   7
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, int32, void*, void*)
		ret
	} // end of method NativeHelper::CalliInt32


    //I11-2
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        int32 arg0,
        void* arg1,
        void* arg2,
        int32 arg3) cil managed
	{
		.maxstack   8
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
        ldarg.s     arg3
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, int32, void*, void*, int32)
		ret
	} // end of method NativeHelper::CalliInt32


    //I12
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        int32 arg0,
        int32 arg1,
        void* arg2) cil managed
	{
		.maxstack   7
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, int32, int32, void*)
		ret
	} // end of method NativeHelper::CalliInt32


    //I12-2
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        int32 arg0,
        int32 arg1) cil managed
	{
		.maxstack   6
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, int32, int32)
		ret
	} // end of method NativeHelper::CalliInt32


    //I13
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        void* arg0,
        int32 arg1,
        void* arg2,
        void* arg3) cil managed
	{
		.maxstack   8
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
        ldarg.s     arg3
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, void*, int32, void*, void*)
		ret
	} // end of method NativeHelper::CalliInt32


    //I14
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        int32 arg0,
        int32 arg1,
        void* arg2,
        int32 arg3,
        void* arg4,
        void* arg5) cil managed
	{
		.maxstack   10
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
        ldarg.s     arg3
        ldarg.s     arg4
        ldarg.s     arg5
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, int32, int32, void*, int32, void*, void*)
		ret
	} // end of method NativeHelper::CalliInt32

    
    //I15
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        int32 arg0, 
        void* arg1, 
        int32 arg2, 
        int32 arg3, 
        float32 arg4, 
        int32 arg5) cil managed
	{
		.maxstack   10
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
        ldarg.s     arg3
        ldarg.s     arg4
        ldarg.s     arg5
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, int32, void*, int32, int32, float32, int32)
		ret
	} // end of method NativeHelper::CalliInt32


    //I16
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        void* arg0,
        void* arg1,
        void* arg2,
        void* arg3) cil managed
	{
		.maxstack   8
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
        ldarg.s     arg3
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, void*, void*, void*, void*)
		ret
	} // end of method NativeHelper::CalliInt32


    //I17
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        void* arg0,
        void* arg1,
        void* arg2,
        void* arg3,
        int32 arg4) cil managed
	{
		.maxstack   9
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
        ldarg.s     arg3
        ldarg.s     arg4
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, void*, void*, void*, void*, int32)
		ret
	} // end of method NativeHelper::CalliInt32


    //I18
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        void* arg0, 
        void* arg1, 
        int32 arg2, 
        void* arg3, 
        int32 arg4, 
        int32 arg5) cil managed
	{
		.maxstack   10
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
        ldarg.s     arg3
        ldarg.s     arg4
        ldarg.s     arg5
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, void*,void*,int32,void*,int32,int32)
		ret
	} // end of method NativeHelper::CalliInt32


    //I19
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        int32 arg0,
        int32 arg1,
        void* arg2,
        int32 arg3) cil managed
	{
		.maxstack   8
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
        ldarg.s     arg3
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, int32, int32, void*, int32)
		ret
	} // end of method NativeHelper::CalliInt32

    
    //I20
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        int32 arg0,
        int32 arg1,
        int32 arg2,
        void* arg3) cil managed
	{
		.maxstack   8
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
        ldarg.s     arg3
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, int32,int32,int32,void*)
		ret
	} // end of method NativeHelper::CalliInt32


    //I21
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        int32 arg0,
        void* arg1,
        int32 arg2,
        int32 arg3) cil managed
	{
		.maxstack   8
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
        ldarg.s     arg3
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, int32,void*,int32,int32)
		ret
	} // end of method NativeHelper::CalliInt32


    //I22
	.method public hidebysig static int32 CalliInt32(
		int32 slot,
		void* nativePointer,
        int32 arg0, 
        int32 arg1, 
        int32 arg2, 
        int32 arg3, 
        void* arg4, 
        void* arg5) cil managed
	{
		.maxstack   10
		ldarg.1		        // native pointer
        ldarg.2             // arg0
        ldarg.3             // arg1
        ldarg.s     arg2
        ldarg.s     arg3
        ldarg.s     arg4
        ldarg.s     arg5
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*, int32,int32,int32,int32,void*,void*)
		ret
	} // end of method NativeHelper::CalliInt32


    // IP00
	.method public hidebysig static native int CalliIntPtr(
		int32 slot,
		void* nativePointer) cil managed
	{
		.maxstack   4
		ldarg.1		        // native pointer
		ldarg.1             // native pointer
		ldind.i
		ldarg.0             // slot
		conv.i
		sizeof	    void*
		mul
		add
		ldind.i
		calli       unmanaged stdcall native int(void*)
		ret
	} // end of method NativeHelper::CalliIntPtr

} // end of class ZXMAK2.DirectX.Native.NativeHelper
