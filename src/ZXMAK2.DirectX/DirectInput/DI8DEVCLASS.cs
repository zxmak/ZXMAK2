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


namespace ZXMAK2.DirectX.DirectInput
{
    public enum DI8DEVCLASS
    {
        /// <unmanaged>DI8DEVCLASS_ALL</unmanaged>
        ALL = 0,
        /// <unmanaged>DI8DEVCLASS_DEVICE</unmanaged>
        DEVICE = 1,
        /// <unmanaged>DI8DEVCLASS_POINTER</unmanaged>
        POINTER = 2,
        /// <unmanaged>DI8DEVCLASS_KEYBOARD</unmanaged>
        KEYBOARD = 3,
        /// <unmanaged>DI8DEVCLASS_GAMECTRL</unmanaged>
        GAMECTRL = 4,
    }
}
