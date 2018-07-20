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


namespace ZXMAK2.DirectX.DirectSound
{
    // https://ccrma.stanford.edu/software/stk/Misc/dsound.h
    public static class DSB
    {
        public const int DSBFREQUENCY_MIN            = 100;
        public const int DSBFREQUENCY_MAX            = 100000;
        public const int DSBFREQUENCY_ORIGINAL       = 0;

        public const int DSBPAN_LEFT                 = -10000;
        public const int DSBPAN_CENTER               = 0;
        public const int DSBPAN_RIGHT                = 10000;

        // SoundBuffer.SetVolume
        public const int DSBVOLUME_MIN               = -10000;
        public const int DSBVOLUME_MAX               = 0;

        public const int DSBSIZE_MIN                 = 4;
        public const int DSBSIZE_MAX                 = 0x0FFFFFFF;
    }
}
