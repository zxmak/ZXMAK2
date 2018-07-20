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
    // d3d9.h (split on 2 sections)
    /// <summary>
    /// Flags for IDirect3DSwapChain9::Present
    /// </summary>
    public enum D3DPRESENT
    {
        /// <summary>
        /// Maximum number of back-buffers supported in DX9
        /// </summary>
        /// <unmanaged>D3DPRESENT_BACK_BUFFERS_MAX</unmanaged>
        BACK_BUFFERS_MAX            = 3,

/* D3D9Ex only -- */
#if !D3D_DISABLE_9EX

        /// <summary>
        /// Maximum number of back-buffers supported when apps use CreateDeviceEx
        /// </summary>
        /// <unmanaged>D3DPRESENT_BACK_BUFFERS_MAX_EX</unmanaged>
        BACK_BUFFERS_MAX_EX         = 30,

#endif // !D3D_DISABLE_9EX
/* -- D3D9Ex only */

        /// <unmanaged>D3DPRESENT_DONOTWAIT</unmanaged>
        DONOTWAIT                   = 0x00000001,
        /// <unmanaged>D3DPRESENT_LINEAR_CONTENT</unmanaged>
        LINEAR_CONTENT              = 0x00000002,

/* D3D9Ex only -- */
#if !D3D_DISABLE_9EX

        /// <unmanaged>D3DPRESENT_DONOTFLIP</unmanaged>
        DONOTFLIP                   = 0x00000004,
        /// <unmanaged>D3DPRESENT_FLIPRESTART</unmanaged>
        FLIPRESTART                 = 0x00000008,
        /// <unmanaged>D3DPRESENT_VIDEO_RESTRICT_TO_MONITOR</unmanaged>
        VIDEO_RESTRICT_TO_MONITOR   = 0x00000010,
        /// <unmanaged>D3DPRESENT_UPDATEOVERLAYONLY</unmanaged>
        UPDATEOVERLAYONLY           = 0x00000020,
        /// <unmanaged>D3DPRESENT_HIDEOVERLAY</unmanaged>
        HIDEOVERLAY                 = 0x00000040,
        /// <unmanaged>D3DPRESENT_UPDATECOLORKEY</unmanaged>
        UPDATECOLORKEY              = 0x00000080,
        /// <unmanaged>D3DPRESENT_FORCEIMMEDIATE</unmanaged>
        FORCEIMMEDIATE              = 0x00000100,

#endif // !D3D_DISABLE_9EX
/* -- D3D9Ex only */
    }
}
