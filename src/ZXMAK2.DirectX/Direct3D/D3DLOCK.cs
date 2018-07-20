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
    public enum D3DLOCK
    {
        NONE = 0,
        /// <unmanaged>D3DLOCK_READONLY</unmanaged>
        D3DLOCK_READONLY           = 0x00000010,
        /// <unmanaged>D3DLOCK_DISCARD</unmanaged>
        D3DLOCK_DISCARD            = 0x00002000,
        /// <unmanaged>D3DLOCK_NOOVERWRITE</unmanaged>
        D3DLOCK_NOOVERWRITE        = 0x00001000,
        /// <unmanaged>D3DLOCK_NOSYSLOCK</unmanaged>
        D3DLOCK_NOSYSLOCK          = 0x00000800,
        /// <unmanaged>D3DLOCK_DONOTWAIT</unmanaged>
        D3DLOCK_DONOTWAIT          = 0x00004000,                  

        /// <unmanaged>D3DLOCK_NO_DIRTY_UPDATE</unmanaged>
        D3DLOCK_NO_DIRTY_UPDATE    = 0x00008000,
    }
}
