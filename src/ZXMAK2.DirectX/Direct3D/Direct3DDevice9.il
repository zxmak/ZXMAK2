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
.class public auto ansi beforefieldinit ZXMAK2.DirectX.Direct3D.Direct3DDevice9
	extends ZXMAK2.DirectX.ComObject
{
    
    .method public hidebysig 
	    instance valuetype ZXMAK2.DirectX.HRESULT DrawUserPrimitives<valuetype .ctor ([mscorlib]System.ValueType) T> (
		    valuetype ZXMAK2.DirectX.Direct3D.D3DPRIMITIVETYPE primitiveType,
		    int32 startIndex,
		    int32 primitiveCount,
		    !!T[] data) cil managed 
    {
        .maxstack   5
	    .locals     ([0] !!T& pinned)
	    ldarg.0
	    ldarg.1
	    ldarg.3
	    ldarg.s     data
	    ldarg.2
	    ldelema     !!T
	    stloc.0
	    ldloc.0
        sizeof      !!T
        call        instance valuetype ZXMAK2.DirectX.HRESULT ZXMAK2.DirectX.Direct3D.Direct3DDevice9::DrawPrimitiveUP(valuetype ZXMAK2.DirectX.Direct3D.D3DPRIMITIVETYPE, int32, void*, int32)
	    ret
    } // end of method Direct3DDevice9::DrawUserPrimitives


} // end of class ZXMAK2.DirectX.Direct3D.Direct3DDevice9