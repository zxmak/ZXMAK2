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
    public enum DI8DEVTYPE
    {
        /// <unmanaged>DI8DEVTYPE_DEVICE</unmanaged>
        DEVICE = 0x11,
        /// <unmanaged>DI8DEVTYPE_MOUSE</unmanaged>
        MOUSE = 0x12,
        /// <unmanaged>DI8DEVTYPE_KEYBOARD</unmanaged>
        KEYBOARD = 0x13,
        /// <unmanaged>DI8DEVTYPE_JOYSTICK</unmanaged>
        JOYSTICK = 0x14,
        /// <unmanaged>DI8DEVTYPE_GAMEPAD</unmanaged>
        GAMEPAD = 0x15,
        /// <unmanaged>DI8DEVTYPE_DRIVING</unmanaged>
        DRIVING = 0x16,
        /// <unmanaged>DI8DEVTYPE_FLIGHT</unmanaged>
        FLIGHT = 0x17,
        /// <unmanaged>DI8DEVTYPE_1STPERSON</unmanaged>
        FIRSTPERSON = 0x18,
        /// <unmanaged>DI8DEVTYPE_DEVICECTRL</unmanaged>
        DEVICECTRL = 0x19,
        /// <unmanaged>DI8DEVTYPE_SCREENPOINTER</unmanaged>
        SCREENPOINTER = 0x1A,
        /// <unmanaged>DI8DEVTYPE_REMOTE</unmanaged>
        REMOTE = 0x1B,
        /// <unmanaged>DI8DEVTYPE_SUPPLEMENTAL</unmanaged>
        SUPPLEMENTAL = 0x1C,
    }
}
