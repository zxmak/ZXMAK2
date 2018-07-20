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
    /// <summary>
    /// State enumerants for per-sampler texture processing.
    /// </summary>
    public enum D3DSAMPLERSTATETYPE
    {
        /// <summary>
        /// D3DTEXTUREADDRESS for U coordinate
        /// </summary>
        D3DSAMP_ADDRESSU = 1,  
        /// <summary>
        /// D3DTEXTUREADDRESS for V coordinate
        /// </summary>
        D3DSAMP_ADDRESSV = 2,
        /// <summary>
        /// D3DTEXTUREADDRESS for W coordinate
        /// </summary>
        D3DSAMP_ADDRESSW = 3,
        /// <summary>
        /// D3DCOLOR
        /// </summary>
        D3DSAMP_BORDERCOLOR = 4,
        /// <summary>
        /// D3DTEXTUREFILTER filter to use for magnification
        /// </summary>
        D3DSAMP_MAGFILTER = 5,
        /// <summary>
        /// D3DTEXTUREFILTER filter to use for minification
        /// </summary>
        D3DSAMP_MINFILTER = 6,  
        /// <summary>
        /// D3DTEXTUREFILTER filter to use between mipmaps during minification
        /// </summary>
        D3DSAMP_MIPFILTER = 7,
        /// <summary>
        /// float Mipmap LOD bias
        /// </summary>
        D3DSAMP_MIPMAPLODBIAS = 8,
        /// <summary>
        /// DWORD 0..(n-1) LOD index of largest map to use (0 == largest)
        /// </summary>
        D3DSAMP_MAXMIPLEVEL = 9,
        /// <summary>
        /// DWORD maximum anisotropy
        /// </summary>
        D3DSAMP_MAXANISOTROPY = 10,
        /// <summary>
        /// Default = 0 (which means Gamma 1.0, no correction required.) else correct for Gamma = 2.2
        /// </summary>
        D3DSAMP_SRGBTEXTURE = 11,
        /// <summary>
        /// When multi-element texture is assigned to sampler, this indicates which element index to use.  Default = 0.
        /// </summary>
        D3DSAMP_ELEMENTINDEX = 12,
        /// <summary>
        /// Offset in vertices in the pre-sampled displacement map. Only valid for D3DDMAPSAMPLER sampler
        /// </summary>
        D3DSAMP_DMAPOFFSET = 13,
        /// <summary>
        /// force 32-bit size enum
        /// </summary>
        D3DSAMP_FORCE_DWORD = 0x7fffffff, /* force 32-bit size enum */
    }
}
