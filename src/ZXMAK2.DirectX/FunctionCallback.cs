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
using System.Globalization;
using System.Runtime.InteropServices;


namespace ZXMAK2.DirectX
{
    [StructLayout(LayoutKind.Sequential)]
    public class FunctionCallback
    {
        public IntPtr Pointer;

        public FunctionCallback(IntPtr pointer)
        {
            this.Pointer = pointer;
        }

        public unsafe FunctionCallback(void* pointer)
        {
            this.Pointer = new IntPtr(pointer);
        }

        public static explicit operator IntPtr(FunctionCallback value)
        {
            return value.Pointer;
        }

        public static implicit operator FunctionCallback(IntPtr value)
        {
            return new FunctionCallback(value);
        }

        public unsafe static implicit operator void*(FunctionCallback value)
        {
            return (void*)value.Pointer;
        }

        public unsafe static explicit operator FunctionCallback(void* value)
        {
            return new FunctionCallback(value);
        }

        public override string ToString()
        {
            return string.Format("[FunctionCallback] {0};", this.Pointer); 
        }

        public override int GetHashCode()
        {
            return this.Pointer.ToInt32();
        }

        public bool Equals(FunctionCallback other)
        {
            return this.Pointer == other.Pointer;
        }

        public override bool Equals(object value)
        {
            return value != null && 
                value.GetType() == typeof(FunctionCallback) && 
                this.Equals((FunctionCallback)value);
        }
    }
}
