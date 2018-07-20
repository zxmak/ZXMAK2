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
 *  Date: 10.07.2018
 */
using System;

namespace ZXMAK2.DirectX.DirectSound
{
    // <unmanaged>DSCAPS_FLAGS</unmanaged>	
    [Flags]
    public enum DSCAPS_FLAGS
    {
        // <unmanaged>DSCAPS_PRIMARYMONO</unmanaged>	
        PRIMARYMONO = 1,
        // <unmanaged>DSCAPS_PRIMARYSTEREO</unmanaged>	
        PRIMARYSTEREO = 2,
        // <unmanaged>DSCAPS_PRIMARY8BIT</unmanaged>	
        PRIMARY8BIT = 4,
        // <unmanaged>DSCAPS_PRIMARY16BIT</unmanaged>	
        PRIMARY16BIT = 8,
        // <unmanaged>DSCAPS_CONTINUOUSRATE</unmanaged>	
        CONTINUOUSRATE = 16,
        // <unmanaged>DSCAPS_EMULDRIVER</unmanaged>	
        EMULDRIVER = 32,
        // <unmanaged>DSCAPS_CERTIFIED</unmanaged>	
        CERTIFIED = 64,
        // <unmanaged>DSCAPS_SECONDARYMONO</unmanaged>	
        SECONDARYMONO = 256,
        // <unmanaged>DSCAPS_SECONDARYSTEREO</unmanaged>	
        SECONDARYSTEREO = 512,
        // <unmanaged>DSCAPS_SECONDARY8BIT</unmanaged>	
        SECONDARY8BIT = 1024,
        // <unmanaged>DSCAPS_SECONDARY16BIT</unmanaged>	
        SECONDARY16BIT = 2048,
        // <unmanaged>None</unmanaged>	
        None = 0
    }
}
