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


namespace ZXMAK2.DirectX.Direct3D
{
    // d3d9types.h
    public enum D3DFVF
    {
        D3DFVF_RESERVED0        = 0x001,
        D3DFVF_POSITION_MASK    = 0x400E,
        D3DFVF_XYZ              = 0x002,
        D3DFVF_XYZRHW           = 0x004,
        D3DFVF_XYZB1            = 0x006,
        D3DFVF_XYZB2            = 0x008,
        D3DFVF_XYZB3            = 0x00a,
        D3DFVF_XYZB4            = 0x00c,
        D3DFVF_XYZB5            = 0x00e,
        D3DFVF_XYZW             = 0x4002,

        D3DFVF_NORMAL           = 0x010,
        D3DFVF_PSIZE            = 0x020,
        D3DFVF_DIFFUSE          = 0x040,
        D3DFVF_SPECULAR         = 0x080,

        D3DFVF_TEXCOUNT_MASK    = 0xf00,
        D3DFVF_TEXCOUNT_SHIFT   = 8,
        D3DFVF_TEX0             = 0x000,
        D3DFVF_TEX1             = 0x100,
        D3DFVF_TEX2             = 0x200,
        D3DFVF_TEX3             = 0x300,
        D3DFVF_TEX4             = 0x400,
        D3DFVF_TEX5             = 0x500,
        D3DFVF_TEX6             = 0x600,
        D3DFVF_TEX7             = 0x700,
        D3DFVF_TEX8             = 0x800,

        D3DFVF_LASTBETA_UBYTE4   = 0x1000,
        D3DFVF_LASTBETA_D3DCOLOR = 0x8000,

        D3DFVF_RESERVED2         = 0x6000,  // 2 reserved bits
    }
}
