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
using System;


namespace ZXMAK2.DirectX.Direct3D
{
    // d3d9types.h
    [Flags]
    public enum D3DUSAGE
    {
        NONE = 0,
        /// <unmanaged>D3DUSAGE_RENDERTARGET</unmanaged>
        RENDERTARGET                    =(0x00000001),
        /// <unmanaged>D3DUSAGE_DEPTHSTENCIL</unmanaged>
        DEPTHSTENCIL                    =(0x00000002),
        /// <unmanaged>D3DUSAGE_DYNAMIC</unmanaged>
        DYNAMIC                         =(0x00000200),

/* D3D9Ex only -- */
#if !D3D_DISABLE_9EX

        /// <unmanaged>D3DUSAGE_NONSECURE</unmanaged>
        NONSECURE                       = (0x00800000),

#endif // !D3D_DISABLE_9EX
/* -- D3D9Ex only */

        // When passed to CheckDeviceFormat, D3DUSAGE_AUTOGENMIPMAP may return
        // D3DOK_NOAUTOGEN if the device doesn't support autogeneration for that format.
        // D3DOK_NOAUTOGEN is a success code, not a failure code... the SUCCEEDED and FAILED macros
        // will return true and false respectively for this code.
        /// <unmanaged>D3DUSAGE_AUTOGENMIPMAP</unmanaged>
        AUTOGENMIPMAP                   = (0x00000400),
        /// <unmanaged>D3DUSAGE_DMAP</unmanaged>
        DMAP                            = (0x00004000),

        // The following usages are valid only for querying CheckDeviceFormat
        /// <unmanaged>D3DUSAGE_QUERY_LEGACYBUMPMAP</unmanaged>
        QUERY_LEGACYBUMPMAP             = (0x00008000),
        /// <unmanaged>D3DUSAGE_QUERY_SRGBREAD</unmanaged>
        QUERY_SRGBREAD                  = (0x00010000),
        /// <unmanaged>D3DUSAGE_QUERY_FILTER</unmanaged>
        QUERY_FILTER                    = (0x00020000),
        /// <unmanaged>D3DUSAGE_QUERY_SRGBWRITE</unmanaged>
        QUERY_SRGBWRITE                 = (0x00040000),
        /// <unmanaged>D3DUSAGE_QUERY_POSTPIXELSHADER_BLENDING</unmanaged>
        QUERY_POSTPIXELSHADER_BLENDING  = (0x00080000),
        /// <unmanaged>D3DUSAGE_QUERY_VERTEXTEXTURE</unmanaged>
        QUERY_VERTEXTEXTURE             = (0x00100000),
        /// <unmanaged>D3DUSAGE_QUERY_WRAPANDMIP</unmanaged>
        QUERY_WRAPANDMIP	            = (0x00200000),

        /* Usages for Vertex/Index buffers */
        /// <unmanaged>D3DUSAGE_WRITEONLY</unmanaged>
        WRITEONLY                       = (0x00000008),
        /// <unmanaged>D3DUSAGE_SOFTWAREPROCESSING</unmanaged>
        SOFTWAREPROCESSING              = (0x00000010),
        /// <unmanaged>D3DUSAGE_DONOTCLIP</unmanaged>
        DONOTCLIP                       = (0x00000020),
        /// <unmanaged>D3DUSAGE_POINTS</unmanaged>
        POINTS                          = (0x00000040),
        /// <unmanaged>D3DUSAGE_RTPATCHES</unmanaged>
        RTPATCHES                       = (0x00000080),
        /// <unmanaged>D3DUSAGE_NPATCHES</unmanaged>
        NPATCHES                        = (0x00000100),

/* D3D9Ex only -- */
#if !D3D_DISABLE_9EX

        /// <unmanaged>D3DUSAGE_TEXTAPI</unmanaged>
        TEXTAPI                         = (0x10000000),
        /// <unmanaged>D3DUSAGE_RESTRICTED_CONTENT</unmanaged>
        RESTRICTED_CONTENT              = (0x00000800),
        /// <unmanaged>D3DUSAGE_RESTRICT_SHARED_RESOURCE</unmanaged>
        RESTRICT_SHARED_RESOURCE        = (0x00002000),
        /// <unmanaged>D3DUSAGE_RESTRICT_SHARED_RESOURCE_DRIVER</unmanaged>
        RESTRICT_SHARED_RESOURCE_DRIVER = (0x00001000),

#endif // !D3D_DISABLE_9EX
/* -- D3D9Ex only */
    }
}
