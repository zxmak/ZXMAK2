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
    [DebuggerDisplay("Left: {Left}, Top: {Top}, Right: {Right}, Bottom: {Bottom}")]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct D3DRECT : IEquatable<D3DRECT>
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;


        public bool IsEmpty
        {
            get 
            {
                return this.Left == 0 && 
                    this.Top == 0 && 
                    this.Right == 0 && 
                    this.Bottom == 0;
            }
        }

        public int Width 
        { 
            get { return Left <= Right ? Right - Left : Left - Right; } 
        }

        public int Height
        {
            get { return Top <= Bottom ? Bottom - Top : Top - Bottom; }
        }

        public D3DRECT(int left, int top, int right, int bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }


        public static bool operator ==(D3DRECT left, D3DRECT right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(D3DRECT left, D3DRECT right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return string.Format("[RawRectangle] {0}; {1}", Left, Right, Top, Bottom);
        }

        public override bool Equals(object obj)
        {
            return (obj is D3DRECT) && obj.Equals(this);
        }

        public bool Equals(D3DRECT other)
        {
            return Left == other.Left && 
                Top == other.Top &&
                Right == other.Right &&
                Bottom == other.Bottom;
        }

        public override int GetHashCode()
        {
            return Left.GetHashCode() +
                (Right.GetHashCode() << 3) +
                (Top.GetHashCode() << 5) +
                (Bottom.GetHashCode() << 7);
        }

        public static readonly D3DRECT Empty = new D3DRECT(0, 0, 0, 0);
    }
}
