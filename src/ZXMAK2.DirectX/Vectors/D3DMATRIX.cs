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
using ZXMAK2.DirectX.Native;


namespace ZXMAK2.DirectX.Vectors
{
    // d3d9types.h
    [StructLayout(LayoutKind.Sequential)]
    public struct D3DMATRIX
    {
        public float _11, _12, _13, _14;
        public float _21, _22, _23, _24;
        public float _31, _32, _33, _34;
        public float _41, _42, _43, _44;

        public D3DMATRIX(
            float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34,
            float m41, float m42, float m43, float m44)
        {
            _11 = m11; _12 = m12; _13 = m13; _14 = m14;
            _21 = m21; _22 = m22; _23 = m23; _24 = m24;
            _31 = m31; _32 = m32; _33 = m33; _34 = m34;
            _41 = m41; _42 = m42; _43 = m43; _44 = m44;
        }

        public static unsafe D3DMATRIX* Transformation2D(
            D3DMATRIX* pOut, D3DXVECTOR2* pScalingCenter,
            float ScalingRotation, D3DXVECTOR2* pScaling,
            D3DXVECTOR2* pRotationCenter, float Rotation,
            D3DXVECTOR2* pTranslation)
        {
            return NativeMethods.D3DXMatrixTransformation2D(
                pOut, pScalingCenter,
                ScalingRotation, pScaling,
                pRotationCenter, Rotation,
                pTranslation);
        }
    }
}
