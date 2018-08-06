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
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace ZXMAK2.DirectX.Vectors
{
    [DebuggerDisplay("X: {X}, Y: {Y}")]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct D3DXVECTOR2 : IEquatable<D3DXVECTOR2>
    {
        public float X;
        public float Y;

        public D3DXVECTOR2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }


        public static bool operator ==(D3DXVECTOR2 left, D3DXVECTOR2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(D3DXVECTOR2 left, D3DXVECTOR2 right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("[D3DXVECTOR2] {0}; {1}", X, Y);
        }

        public override bool Equals(object obj)
        {
            return (obj is D3DXVECTOR2) && obj.Equals(this);
        }

        public bool Equals(D3DXVECTOR2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() +
                (Y.GetHashCode() << 16);
        }


        public static readonly D3DXVECTOR2 Empty = new D3DXVECTOR2(0, 0);
    }
}
