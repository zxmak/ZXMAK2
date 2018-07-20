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
    //d3d9.h
    public enum D3DCREATE
    {
        /// <unmanaged>D3DCREATE_FPU_PRESERVE</unmanaged>
        D3DCREATE_FPU_PRESERVE                 = 0x00000002,
        /// <unmanaged>D3DCREATE_MULTITHREADED</unmanaged>
        D3DCREATE_MULTITHREADED                = 0x00000004,

        /// <unmanaged>D3DCREATE_PUREDEVICE</unmanaged>
        D3DCREATE_PUREDEVICE                   = 0x00000010,
        /// <unmanaged>D3DCREATE_SOFTWARE_VERTEXPROCESSING</unmanaged>
        D3DCREATE_SOFTWARE_VERTEXPROCESSING    = 0x00000020,
        /// <unmanaged>D3DCREATE_HARDWARE_VERTEXPROCESSING</unmanaged>
        D3DCREATE_HARDWARE_VERTEXPROCESSING    = 0x00000040,
        /// <unmanaged>D3DCREATE_MIXED_VERTEXPROCESSING</unmanaged>
        D3DCREATE_MIXED_VERTEXPROCESSING       = 0x00000080,

        /// <unmanaged>D3DCREATE_DISABLE_DRIVER_MANAGEMENT</unmanaged>
        D3DCREATE_DISABLE_DRIVER_MANAGEMENT    = 0x00000100,
        /// <unmanaged>D3DCREATE_ADAPTERGROUP_DEVICE</unmanaged>
        D3DCREATE_ADAPTERGROUP_DEVICE          = 0x00000200,
        /// <unmanaged>D3DCREATE_DISABLE_DRIVER_MANAGEMENT_EX</unmanaged>
        D3DCREATE_DISABLE_DRIVER_MANAGEMENT_EX = 0x00000400,

        // This flag causes the D3D runtime not to alter the focus 
        // window in any way. Use with caution- the burden of supporting
        // focus management events (alt-tab, etc.) falls on the 
        // application, and appropriate responses (switching display
        // mode, etc.) should be coded.
        /// <unmanaged>D3DCREATE_NOWINDOWCHANGES</unmanaged>
        D3DCREATE_NOWINDOWCHANGES			   = 0x00000800,

/* D3D9Ex only -- */
#if !D3D_DISABLE_9EX

        // Disable multithreading for software vertex processing
        /// <unmanaged>D3DCREATE_DISABLE_PSGP_THREADING</unmanaged>
        D3DCREATE_DISABLE_PSGP_THREADING       = 0x00002000,
        // This flag enables present statistics on device.
        /// <unmanaged>D3DCREATE_ENABLE_PRESENTSTATS</unmanaged>
        D3DCREATE_ENABLE_PRESENTSTATS          = 0x00004000,
        // This flag disables printscreen support in the runtime for this device
        /// <unmanaged>D3DCREATE_DISABLE_PRINTSCREEN</unmanaged>
        D3DCREATE_DISABLE_PRINTSCREEN          = 0x00008000,

        /// <unmanaged>D3DCREATE_SCREENSAVER</unmanaged>
        D3DCREATE_SCREENSAVER                  = 0x10000000,

#endif // !D3D_DISABLE_9EX
/* -- D3D9Ex only */
    }
}
