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
    public enum D3DPRESENTFLAG
    {
        /// <unmanaged>D3DPRESENTFLAG_LOCKABLE_BACKBUFFER</unmanaged>
        LOCKABLE_BACKBUFFER                 = 0x00000001,
        /// <unmanaged>D3DPRESENTFLAG_DISCARD_DEPTHSTENCIL</unmanaged>
        DISCARD_DEPTHSTENCIL                = 0x00000002,
        /// <unmanaged>D3DPRESENTFLAG_DEVICECLIP</unmanaged>
        DEVICECLIP                          = 0x00000004,
        /// <unmanaged>D3DPRESENTFLAG_VIDEO</unmanaged>
        VIDEO                               = 0x00000010,

/* D3D9Ex only -- */
#if !D3D_DISABLE_9EX

        /// <unmanaged>D3DPRESENTFLAG_NOAUTOROTATE</unmanaged>
        NOAUTOROTATE                        = 0x00000020,
        /// <unmanaged>D3DPRESENTFLAG_UNPRUNEDMODE</unmanaged>
        UNPRUNEDMODE                        = 0x00000040,
        /// <unmanaged>D3DPRESENTFLAG_OVERLAY_LIMITEDRGB</unmanaged>
        OVERLAY_LIMITEDRGB                  = 0x00000080,
        /// <unmanaged>D3DPRESENTFLAG_OVERLAY_YCbCr_BT709</unmanaged>
        OVERLAY_YCbCr_BT709                 = 0x00000100,
        /// <unmanaged>D3DPRESENTFLAG_OVERLAY_YCbCr_xvYCC</unmanaged>
        OVERLAY_YCbCr_xvYCC                 = 0x00000200,
        /// <unmanaged>D3DPRESENTFLAG_RESTRICTED_CONTENT</unmanaged>
        RESTRICTED_CONTENT                  = 0x00000400,
        /// <unmanaged>D3DPRESENTFLAG_RESTRICT_SHARED_RESOURCE_DRIVER</unmanaged>
        RESTRICT_SHARED_RESOURCE_DRIVER     = 0x00000800,

#endif // !D3D_DISABLE_9EX
/* -- D3D9Ex only */
        
    }
}
