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
    // <unmanaged>SPEAKER_FLAGS</unmanaged>	
    [Flags]
    public enum SPEAKER_FLAGS
    {
        // <unmanaged>SPEAKER_FRONT_LEFT</unmanaged>	
        FRONT_LEFT = 1,
        // <unmanaged>SPEAKER_FRONT_RIGHT</unmanaged>	
        FRONT_RIGHT = 2,
        // <unmanaged>SPEAKER_FRONT_CENTER</unmanaged>	
        FRONT_CENTER = 4,
        // <unmanaged>SPEAKER_LOW_FREQUENCY</unmanaged>	
        LOW_FREQUENCY = 8,
        // <unmanaged>SPEAKER_BACK_LEFT</unmanaged>	
        BACK_LEFT = 16,
        // <unmanaged>SPEAKER_BACK_RIGHT</unmanaged>	
        BACK_RIGHT = 32,
        // <unmanaged>SPEAKER_FRONT_LEFT_OF_CENTER</unmanaged>	
        FRONT_LEFT_OF_CENTER = 64,
        // <unmanaged>SPEAKER_FRONT_RIGHT_OF_CENTER</unmanaged>	
        FRONT_RIGHT_OF_CENTER = 128,
        // <unmanaged>SPEAKER_BACK_CENTER</unmanaged>	
        BACK_CENTER = 256,
        // <unmanaged>SPEAKER_SIDE_LEFT</unmanaged>	
        SIDE_LEFT = 512,
        // <unmanaged>SPEAKER_SIDE_RIGHT</unmanaged>	
        SIDE_RIGHT = 1024,
        // <unmanaged>SPEAKER_TOP_CENTER</unmanaged>	
        TOP_CENTER = 2048,
        // <unmanaged>SPEAKER_TOP_FRONT_LEFT</unmanaged>	
        TOP_FRONT_LEFT = 4096,
        // <unmanaged>SPEAKER_TOP_FRONT_CENTER</unmanaged>	
        TOP_FRONT_CENTER = 8192,
        // <unmanaged>SPEAKER_TOP_FRONT_RIGHT</unmanaged>	
        TOP_FRONT_RIGHT = 16384,
        // <unmanaged>SPEAKER_TOP_BACK_LEFT</unmanaged>	
        TOP_BACK_LEFT = 32768,
        // <unmanaged>SPEAKER_TOP_BACK_CENTER</unmanaged>	
        TOP_BACK_CENTER = 65536,
        // <unmanaged>SPEAKER_TOP_BACK_RIGHT</unmanaged>	
        TOP_BACK_RIGHT = 131072,
        // <unmanaged>SPEAKER_RESERVED</unmanaged>	
        RESERVED = 2147221504,
        // <unmanaged>SPEAKER_ALL</unmanaged>	
        ALL = -2147483648,
        // <unmanaged>SPEAKER_MONO</unmanaged>	
        MONO = 4,
        // <unmanaged>SPEAKER_STEREO</unmanaged>	
        STEREO = 3,
        // <unmanaged>SPEAKER_2POINT1</unmanaged>	
        TWO_POINT_ONE = 11,
        // <unmanaged>SPEAKER_SURROUND</unmanaged>	
        SURROUND = 263,
        // <unmanaged>SPEAKER_QUAD</unmanaged>	
        QUAD = 51,
        // <unmanaged>SPEAKER_4POINT1</unmanaged>	
        FOUR_POINT_ONE = 59,
        // <unmanaged>SPEAKER_5POINT1</unmanaged>	
        FIVE_POINT_ONE = 63,
        // <unmanaged>SPEAKER_7POINT1</unmanaged>	
        SEVEN_POINT_ONE = 255,
        // <unmanaged>SPEAKER_5POINT1_SURROUND</unmanaged>	
        FIVE_POINT_ONE_SURROUND = 1551,
        // <unmanaged>SPEAKER_7POINT1_SURROUND</unmanaged>	
        SEVEN_POINT_ONE_SURROUND = 1599,
        // <unmanaged>None</unmanaged>	
        None = 0
    }
}
