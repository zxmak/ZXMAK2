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
    // <unmanaged>DSBCAPS_FLAGS</unmanaged>	
    [Flags]
    public enum DSBCAPS_FLAGS
    {
        // <unmanaged>DSBCAPS_PRIMARYBUFFER</unmanaged>	
        PRIMARYBUFFER = 1,
        // <unmanaged>DSBCAPS_STATIC</unmanaged>	
        STATIC = 2,
        // <unmanaged>DSBCAPS_LOCHARDWARE</unmanaged>	
        LOCHARDWARE = 4,
        // <unmanaged>DSBCAPS_LOCSOFTWARE</unmanaged>	
        LOCSOFTWARE = 8,
        // <unmanaged>DSBCAPS_CTRL3D</unmanaged>	
        CTRL3D = 16,
        // <unmanaged>DSBCAPS_CTRLFREQUENCY</unmanaged>	
        CTRLFREQUENCY = 32,
        // <unmanaged>DSBCAPS_CTRLPAN</unmanaged>	
        CTRLPAN = 64,
        // <unmanaged>DSBCAPS_CTRLVOLUME</unmanaged>	
        CTRLVOLUME = 128,
        // <unmanaged>DSBCAPS_CTRLPOSITIONNOTIFY</unmanaged>	
        CTRLPOSITIONNOTIFY = 256,
        // <unmanaged>DSBCAPS_CTRLFX</unmanaged>	
        CTRLFX = 512,
        // <unmanaged>DSBCAPS_STICKYFOCUS</unmanaged>	
        STICKYFOCUS = 16384,
        // <unmanaged>DSBCAPS_GLOBALFOCUS</unmanaged>	
        GLOBALFOCUS = 32768,
        // <unmanaged>DSBCAPS_GETCURRENTPOSITION2</unmanaged>	
        GETCURRENTPOSITION2 = 65536,
        // <unmanaged>DSBCAPS_MUTE3DATMAXDISTANCE</unmanaged>	
        MUTE3DATMAXDISTANCE = 131072,
        // <unmanaged>DSBCAPS_LOCDEFER</unmanaged>	
        LOCDEFER = 262144,
        // <unmanaged>DSBCAPS_TRUEPLAYPOSITION</unmanaged>	
        TRUEPLAYPOSITION = 524288,
        // <unmanaged>None</unmanaged>	
        None = 0
    }
}
