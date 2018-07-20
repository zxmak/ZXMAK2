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
    // d3dx9core.h
    public enum D3DXSPRITEFLAG
    {
        NONE = 0,
        D3DXSPRITE_DONOTSAVESTATE              = (1 << 0),
        D3DXSPRITE_DONOTMODIFY_RENDERSTATE     = (1 << 1),
        D3DXSPRITE_OBJECTSPACE                 = (1 << 2),
        D3DXSPRITE_BILLBOARD                   = (1 << 3),
        D3DXSPRITE_ALPHABLEND                  = (1 << 4),
        D3DXSPRITE_SORT_TEXTURE                = (1 << 5),
        D3DXSPRITE_SORT_DEPTH_FRONTTOBACK      = (1 << 6),
        D3DXSPRITE_SORT_DEPTH_BACKTOFRONT      = (1 << 7),
        D3DXSPRITE_DO_NOT_ADDREF_TEXTURE       = (1 << 8),
    }
}
