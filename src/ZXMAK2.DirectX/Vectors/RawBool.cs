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
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct RawBool : IEquatable<RawBool>
    {
        private int _boolValue;


        public static implicit operator bool(RawBool booleanValue)
        {
            return booleanValue._boolValue != 0;
        }

        public static implicit operator RawBool(bool boolValue)
        {
            return new RawBool(boolValue);
        }

        public RawBool(bool boolValue)
        {
            _boolValue = boolValue ? 1 : 0;
        }

        public static bool operator ==(RawBool left, RawBool right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RawBool left, RawBool right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("[RawBool] {0};", (bool)this);
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is RawBool && obj.Equals(this);
            //return obj != null && obj is RawBool && this.Equals((RawBool)obj);
        }

        public bool Equals(RawBool other)
        {
            return this._boolValue == other._boolValue;
        }

        public override int GetHashCode()
        {
            return _boolValue.GetHashCode();
        }
    }
}
