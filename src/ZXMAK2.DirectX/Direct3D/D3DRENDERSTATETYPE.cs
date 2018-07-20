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
    public enum D3DRENDERSTATETYPE
    {
        D3DRS_ZENABLE = 7,    /* D3DZBUFFERTYPE (or TRUE/FALSE for legacy) */
        D3DRS_FILLMODE = 8,    /* D3DFILLMODE */
        D3DRS_SHADEMODE = 9,    /* D3DSHADEMODE */
        D3DRS_ZWRITEENABLE = 14,   /* TRUE to enable z writes */
        D3DRS_ALPHATESTENABLE = 15,   /* TRUE to enable alpha tests */
        D3DRS_LASTPIXEL = 16,   /* TRUE for last-pixel on lines */
        D3DRS_SRCBLEND = 19,   /* D3DBLEND */
        D3DRS_DESTBLEND = 20,   /* D3DBLEND */
        D3DRS_CULLMODE = 22,   /* D3DCULL */
        D3DRS_ZFUNC = 23,   /* D3DCMPFUNC */
        D3DRS_ALPHAREF = 24,   /* D3DFIXED */
        D3DRS_ALPHAFUNC = 25,   /* D3DCMPFUNC */
        D3DRS_DITHERENABLE = 26,   /* TRUE to enable dithering */
        D3DRS_ALPHABLENDENABLE = 27,   /* TRUE to enable alpha blending */
        D3DRS_FOGENABLE = 28,   /* TRUE to enable fog blending */
        D3DRS_SPECULARENABLE = 29,   /* TRUE to enable specular */
        D3DRS_FOGCOLOR = 34,   /* D3DCOLOR */
        D3DRS_FOGTABLEMODE = 35,   /* D3DFOGMODE */
        D3DRS_FOGSTART = 36,   /* Fog start (for both vertex and pixel fog) */
        D3DRS_FOGEND = 37,   /* Fog end      */
        D3DRS_FOGDENSITY = 38,   /* Fog density  */
        D3DRS_RANGEFOGENABLE = 48,   /* Enables range-based fog */
        D3DRS_STENCILENABLE = 52,   /* BOOL enable/disable stenciling */
        D3DRS_STENCILFAIL = 53,   /* D3DSTENCILOP to do if stencil test fails */
        D3DRS_STENCILZFAIL = 54,   /* D3DSTENCILOP to do if stencil test passes and Z test fails */
        D3DRS_STENCILPASS = 55,   /* D3DSTENCILOP to do if both stencil and Z tests pass */
        D3DRS_STENCILFUNC = 56,   /* D3DCMPFUNC fn.  Stencil Test passes if ((ref & mask) stencilfn (stencil & mask)) is true */
        D3DRS_STENCILREF = 57,   /* Reference value used in stencil test */
        D3DRS_STENCILMASK = 58,   /* Mask value used in stencil test */
        D3DRS_STENCILWRITEMASK = 59,   /* Write mask applied to values written to stencil buffer */
        D3DRS_TEXTUREFACTOR = 60,   /* D3DCOLOR used for multi-texture blend */
        D3DRS_WRAP0 = 128,  /* wrap for 1st texture coord. set */
        D3DRS_WRAP1 = 129,  /* wrap for 2nd texture coord. set */
        D3DRS_WRAP2 = 130,  /* wrap for 3rd texture coord. set */
        D3DRS_WRAP3 = 131,  /* wrap for 4th texture coord. set */
        D3DRS_WRAP4 = 132,  /* wrap for 5th texture coord. set */
        D3DRS_WRAP5 = 133,  /* wrap for 6th texture coord. set */
        D3DRS_WRAP6 = 134,  /* wrap for 7th texture coord. set */
        D3DRS_WRAP7 = 135,  /* wrap for 8th texture coord. set */
        D3DRS_CLIPPING = 136,
        D3DRS_LIGHTING = 137,
        D3DRS_AMBIENT = 139,
        D3DRS_FOGVERTEXMODE = 140,
        D3DRS_COLORVERTEX = 141,
        D3DRS_LOCALVIEWER = 142,
        D3DRS_NORMALIZENORMALS = 143,
        D3DRS_DIFFUSEMATERIALSOURCE = 145,
        D3DRS_SPECULARMATERIALSOURCE = 146,
        D3DRS_AMBIENTMATERIALSOURCE = 147,
        D3DRS_EMISSIVEMATERIALSOURCE = 148,
        D3DRS_VERTEXBLEND = 151,
        D3DRS_CLIPPLANEENABLE = 152,
        D3DRS_POINTSIZE = 154,   /* float point size */
        D3DRS_POINTSIZE_MIN = 155,   /* float point size min threshold */
        D3DRS_POINTSPRITEENABLE = 156,   /* BOOL point texture coord control */
        D3DRS_POINTSCALEENABLE = 157,   /* BOOL point size scale enable */
        D3DRS_POINTSCALE_A = 158,   /* float point attenuation A value */
        D3DRS_POINTSCALE_B = 159,   /* float point attenuation B value */
        D3DRS_POINTSCALE_C = 160,   /* float point attenuation C value */
        D3DRS_MULTISAMPLEANTIALIAS = 161,  // BOOL - set to do FSAA with multisample buffer
        D3DRS_MULTISAMPLEMASK = 162,  // DWORD - per-sample enable/disable
        D3DRS_PATCHEDGESTYLE = 163,  // Sets whether patch edges will use float style tessellation
        D3DRS_DEBUGMONITORTOKEN = 165,  // DEBUG ONLY - token to debug monitor
        D3DRS_POINTSIZE_MAX = 166,   /* float point size max threshold */
        D3DRS_INDEXEDVERTEXBLENDENABLE = 167,
        D3DRS_COLORWRITEENABLE = 168,  // per-channel write enable
        D3DRS_TWEENFACTOR = 170,   // float tween factor
        D3DRS_BLENDOP = 171,   // D3DBLENDOP setting
        D3DRS_POSITIONDEGREE = 172,   // NPatch position interpolation degree. D3DDEGREE_LINEAR or D3DDEGREE_CUBIC (default)
        D3DRS_NORMALDEGREE = 173,   // NPatch normal interpolation degree. D3DDEGREE_LINEAR (default) or D3DDEGREE_QUADRATIC
        D3DRS_SCISSORTESTENABLE = 174,
        D3DRS_SLOPESCALEDEPTHBIAS = 175,
        D3DRS_ANTIALIASEDLINEENABLE = 176,
        D3DRS_MINTESSELLATIONLEVEL = 178,
        D3DRS_MAXTESSELLATIONLEVEL = 179,
        D3DRS_ADAPTIVETESS_X = 180,
        D3DRS_ADAPTIVETESS_Y = 181,
        D3DRS_ADAPTIVETESS_Z = 182,
        D3DRS_ADAPTIVETESS_W = 183,
        D3DRS_ENABLEADAPTIVETESSELLATION = 184,
        D3DRS_TWOSIDEDSTENCILMODE = 185,   /* BOOL enable/disable 2 sided stenciling */
        D3DRS_CCW_STENCILFAIL = 186,   /* D3DSTENCILOP to do if ccw stencil test fails */
        D3DRS_CCW_STENCILZFAIL = 187,   /* D3DSTENCILOP to do if ccw stencil test passes and Z test fails */
        D3DRS_CCW_STENCILPASS = 188,   /* D3DSTENCILOP to do if both ccw stencil and Z tests pass */
        D3DRS_CCW_STENCILFUNC = 189,   /* D3DCMPFUNC fn.  ccw Stencil Test passes if ((ref & mask) stencilfn (stencil & mask)) is true */
        D3DRS_COLORWRITEENABLE1 = 190,   /* Additional ColorWriteEnables for the devices that support D3DPMISCCAPS_INDEPENDENTWRITEMASKS */
        D3DRS_COLORWRITEENABLE2 = 191,   /* Additional ColorWriteEnables for the devices that support D3DPMISCCAPS_INDEPENDENTWRITEMASKS */
        D3DRS_COLORWRITEENABLE3 = 192,   /* Additional ColorWriteEnables for the devices that support D3DPMISCCAPS_INDEPENDENTWRITEMASKS */
        D3DRS_BLENDFACTOR = 193,   /* D3DCOLOR used for a constant blend factor during alpha blending for devices that support D3DPBLENDCAPS_BLENDFACTOR */
        D3DRS_SRGBWRITEENABLE = 194,   /* Enable rendertarget writes to be DE-linearized to SRGB (for formats that expose D3DUSAGE_QUERY_SRGBWRITE) */
        D3DRS_DEPTHBIAS = 195,
        D3DRS_WRAP8 = 198,   /* Additional wrap states for vs_3_0+ attributes with D3DDECLUSAGE_TEXCOORD */
        D3DRS_WRAP9 = 199,
        D3DRS_WRAP10 = 200,
        D3DRS_WRAP11 = 201,
        D3DRS_WRAP12 = 202,
        D3DRS_WRAP13 = 203,
        D3DRS_WRAP14 = 204,
        D3DRS_WRAP15 = 205,
        D3DRS_SEPARATEALPHABLENDENABLE = 206,  /* TRUE to enable a separate blending function for the alpha channel */
        D3DRS_SRCBLENDALPHA = 207,  /* SRC blend factor for the alpha channel when D3DRS_SEPARATEDESTALPHAENABLE is TRUE */
        D3DRS_DESTBLENDALPHA = 208,  /* DST blend factor for the alpha channel when D3DRS_SEPARATEDESTALPHAENABLE is TRUE */
        D3DRS_BLENDOPALPHA = 209,  /* Blending operation for the alpha channel when D3DRS_SEPARATEDESTALPHAENABLE is TRUE */


        D3DRS_FORCE_DWORD = 0x7fffffff, /* force 32-bit size enum */
    }
}
