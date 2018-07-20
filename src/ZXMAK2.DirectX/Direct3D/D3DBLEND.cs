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
    public enum D3DBLEND
    {
        D3DBLEND_ZERO               = 1,
        D3DBLEND_ONE                = 2,
        D3DBLEND_SRCCOLOR           = 3,
        D3DBLEND_INVSRCCOLOR        = 4,
        D3DBLEND_SRCALPHA           = 5,
        D3DBLEND_INVSRCALPHA        = 6,
        D3DBLEND_DESTALPHA          = 7,
        D3DBLEND_INVDESTALPHA       = 8,
        D3DBLEND_DESTCOLOR          = 9,
        D3DBLEND_INVDESTCOLOR       = 10,
        D3DBLEND_SRCALPHASAT        = 11,
        D3DBLEND_BOTHSRCALPHA       = 12,
        D3DBLEND_BOTHINVSRCALPHA    = 13,
        D3DBLEND_BLENDFACTOR        = 14, /* Only supported if D3DPBLENDCAPS_BLENDFACTOR is on */
        D3DBLEND_INVBLENDFACTOR     = 15, /* Only supported if D3DPBLENDCAPS_BLENDFACTOR is on */
/* D3D9Ex only -- */
#if !D3D_DISABLE_9EX

        D3DBLEND_SRCCOLOR2          = 16,
        D3DBLEND_INVSRCCOLOR2       = 17,

#endif // !D3D_DISABLE_9EX
/* -- D3D9Ex only */
        D3DBLEND_FORCE_DWORD        = 0x7fffffff, /* force 32-bit size enum */
    }
}
