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
    [DebuggerDisplay("X: {X}, Y: {Y}, Z: {Z}")]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct D3DXVECTOR3 : IEquatable<D3DXVECTOR3>
    {
        public float X;
        public float Y;
        public float Z;

        
        public D3DXVECTOR3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }


        #region Equality

        public static bool operator ==(D3DXVECTOR3 left, D3DXVECTOR3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(D3DXVECTOR3 left, D3DXVECTOR3 right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("[D3DXVECTOR3] {0}; {1}; {2}", X, Y, Z);
        }

        public override bool Equals(object obj)
        {
            return (obj is D3DXVECTOR3) && obj.Equals(this);
        }

        public bool Equals(D3DXVECTOR3 other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() +
                (Y.GetHashCode() << 16) +
                (Z.GetHashCode() << 7);
        }

        #endregion Equality


        public static readonly D3DXVECTOR3 Zero = new D3DXVECTOR3(0, 0, 0);
    }
}
