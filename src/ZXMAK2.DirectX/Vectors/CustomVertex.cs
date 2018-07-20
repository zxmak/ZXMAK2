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
using ZXMAK2.DirectX.Direct3D;

namespace ZXMAK2.DirectX.Vectors
{
    public sealed class CustomVertex
    {
        public struct TransformedColored
        {
            public float X;
            public float Y;
            public float Z;
            public float Rhw;
            public int Color;

            public const D3DFVF Format = D3DFVF.D3DFVF_XYZRHW | D3DFVF.D3DFVF_DIFFUSE;
            
            public TransformedColored(float xvalue, float yvalue, float zvalue, float rhwvalue, int c)
            {
                X = xvalue;
                Y = yvalue;
                Z = zvalue;
                Rhw = rhwvalue;
                Color = c;
            }
        }

        public struct PositionColored
        {
            public float X;
            public float Y;
            public float Z;
            public int Color;

            public const D3DFVF Format = D3DFVF.D3DFVF_XYZ | D3DFVF.D3DFVF_DIFFUSE;

            public PositionColored(float xvalue, float yvalue, float zvalue, float rhwvalue, int c)
            {
                X = xvalue;
                Y = yvalue;
                Z = zvalue;
                Color = c;
            }
        }
    }
}
