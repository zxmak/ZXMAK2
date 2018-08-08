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


namespace ZXMAK2.DirectX.Vectors
{
    [StructLayout(LayoutKind.Sequential)]
    public struct D3DCOLOR : IEquatable<D3DCOLOR>
    {
        public uint _value;

        
        public D3DCOLOR(uint value)
        {
            _value = value;
        }

        
        #region Equality

        public static bool operator ==(D3DCOLOR left, D3DCOLOR right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(D3DCOLOR left, D3DCOLOR right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("[D3DCOLOR] 0x{0:x}", _value);
        }

        public override bool Equals(object obj)
        {
            return (obj is D3DCOLOR) && obj.Equals(this);
        }

        public bool Equals(D3DCOLOR other)
        {
            return _value == other._value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        #endregion Equality



        public static explicit operator int(D3DCOLOR color)
        {
            return (int)color._value;
        }

        public static explicit operator uint(D3DCOLOR color)
        {
            return color._value;
        }

        public static implicit operator D3DCOLOR(uint color)
        {
            return new D3DCOLOR(color);
        }

        public static implicit operator D3DCOLOR(int color)
        {
            return new D3DCOLOR((uint)color);
        }


        /// <unmanaged>D3DCOLOR_ARGB</unmanaged>
        public static D3DCOLOR ARGB(byte a, byte r, byte g, byte b)
        {
            return (uint)((a << 24) | (r << 16) | (g << 8) | (b));
        }

        /// <unmanaged>D3DCOLOR_XRGB</unmanaged>
        public static D3DCOLOR XRGB(byte r, byte g, byte b)
        {
            return D3DCOLOR.ARGB(0xff, r, g, b);
        }
    }
}
