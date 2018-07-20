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
    public enum D3DSWAPEFFECT
    {
        D3DSWAPEFFECT_DISCARD           = 1,
        D3DSWAPEFFECT_FLIP              = 2,
        D3DSWAPEFFECT_COPY              = 3,

/* D3D9Ex only -- */
#if !D3D_DISABLE_9EX
        D3DSWAPEFFECT_OVERLAY           = 4,
        D3DSWAPEFFECT_FLIPEX            = 5,
#endif // !D3D_DISABLE_9EX
/* -- D3D9Ex only */

        D3DSWAPEFFECT_FORCE_DWORD       = 0x7fffffff,
    }
}
