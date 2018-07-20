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
using System.Runtime.InteropServices;


namespace ZXMAK2.DirectX.Direct3D
{
    // d3d9caps.h
    [Flags]
    public enum D3DCAPS : uint
    {
        D3DCAPS_OVERLAY                = 0x00000800,
        D3DCAPS_READ_SCANLINE          = 0x00020000,
    }

    [Flags]
    public enum D3DCAPS2 : uint
    {
        D3DCAPS2_FULLSCREENGAMMA       = 0x00020000,
        D3DCAPS2_CANCALIBRATEGAMMA     = 0x00100000,
        D3DCAPS2_RESERVED              = 0x02000000,
        D3DCAPS2_CANMANAGERESOURCE     = 0x10000000,
        D3DCAPS2_DYNAMICTEXTURES       = 0x20000000,
        D3DCAPS2_CANAUTOGENMIPMAP      = 0x40000000,

/* D3D9Ex only -- */
#if !D3D_DISABLE_9EX

        D3DCAPS2_CANSHARERESOURCE      = 0x80000000,

#endif // !D3D_DISABLE_9EX
/* -- D3D9Ex only */
    }

    [Flags]
    public enum D3DCAPS3 : uint
    {
        D3DCAPS3_RESERVED              = 0x8000001f,

        // Indicates that the device can respect the ALPHABLENDENABLE render state
        // when fullscreen while using the FLIP or DISCARD swap effect.
        // COPY and COPYVSYNC swap effects work whether or not this flag is set.
        D3DCAPS3_ALPHA_FULLSCREEN_FLIP_OR_DISCARD  = 0x00000020,

        // Indicates that the device can perform a gamma correction from 
        // a windowed back buffer containing linear content to the sRGB desktop.
        D3DCAPS3_LINEAR_TO_SRGB_PRESENTATION = 0x00000080,

        D3DCAPS3_COPY_TO_VIDMEM        = 0x00000100, /* Device can acclerate copies from sysmem to local vidmem */
        D3DCAPS3_COPY_TO_SYSTEMMEM     = 0x00000200, /* Device can acclerate copies from local vidmem to sysmem */
        D3DCAPS3_DXVAHD                = 0x00000400,
    }

    [Flags]
    public enum D3DCURSORCAPS
    {
        // Driver supports HW color cursor in at least hi-res modes(height >=400)
        D3DCURSORCAPS_COLOR            = 0x00000001,
        // Driver supports HW cursor also in low-res modes(height < 400)
        D3DCURSORCAPS_LOWRES           = 0x00000002,
    }

    [Flags]
    public enum D3DDEVCAPS
    {
        D3DDEVCAPS_EXECUTESYSTEMMEMORY  =0x00000010, /* Device can use execute buffers from system memory */
        D3DDEVCAPS_EXECUTEVIDEOMEMORY   =0x00000020, /* Device can use execute buffers from video memory */
        D3DDEVCAPS_TLVERTEXSYSTEMMEMORY =0x00000040, /* Device can use TL buffers from system memory */
        D3DDEVCAPS_TLVERTEXVIDEOMEMORY  =0x00000080, /* Device can use TL buffers from video memory */
        D3DDEVCAPS_TEXTURESYSTEMMEMORY  =0x00000100, /* Device can texture from system memory */
        D3DDEVCAPS_TEXTUREVIDEOMEMORY   =0x00000200, /* Device can texture from device memory */
        D3DDEVCAPS_DRAWPRIMTLVERTEX     =0x00000400, /* Device can draw TLVERTEX primitives */
        D3DDEVCAPS_CANRENDERAFTERFLIP   =0x00000800, /* Device can render without waiting for flip to complete */
        D3DDEVCAPS_TEXTURENONLOCALVIDMEM =0x00001000, /* Device can texture from nonlocal video memory */
        D3DDEVCAPS_DRAWPRIMITIVES2      =0x00002000, /* Device can support DrawPrimitives2 */
        D3DDEVCAPS_SEPARATETEXTUREMEMORIES =0x00004000, /* Device is texturing from separate memory pools */
        D3DDEVCAPS_DRAWPRIMITIVES2EX    =0x00008000, /* Device can support Extended DrawPrimitives2 i.e. DX7 compliant driver*/
        D3DDEVCAPS_HWTRANSFORMANDLIGHT  =0x00010000, /* Device can support transformation and lighting in hardware and DRAWPRIMITIVES2EX must be also */
        D3DDEVCAPS_CANBLTSYSTONONLOCAL  =0x00020000, /* Device supports a Tex Blt from system memory to non-local vidmem */
        D3DDEVCAPS_HWRASTERIZATION      =0x00080000, /* Device has HW acceleration for rasterization */
        D3DDEVCAPS_PUREDEVICE           =0x00100000, /* Device supports D3DCREATE_PUREDEVICE */
        D3DDEVCAPS_QUINTICRTPATCHES     =0x00200000, /* Device supports quintic Beziers and BSplines */
        D3DDEVCAPS_RTPATCHES            =0x00400000, /* Device supports Rect and Tri patches */
        D3DDEVCAPS_RTPATCHHANDLEZERO    =0x00800000, /* Indicates that RT Patches may be drawn efficiently using handle 0 */
        D3DDEVCAPS_NPATCHES             =0x01000000, /* Device supports N-Patches */
    }

    [Flags]
    public enum D3DPMISCCAPS
    {
        D3DPMISCCAPS_MASKZ              =0x00000002,
        D3DPMISCCAPS_CULLNONE           =0x00000010,
        D3DPMISCCAPS_CULLCW             =0x00000020,
        D3DPMISCCAPS_CULLCCW            =0x00000040,
        D3DPMISCCAPS_COLORWRITEENABLE   =0x00000080,
        D3DPMISCCAPS_CLIPPLANESCALEDPOINTS =0x00000100, /* Device correctly clips scaled points to clip planes */
        D3DPMISCCAPS_CLIPTLVERTS        =0x00000200, /* device will clip post-transformed vertex primitives */
        D3DPMISCCAPS_TSSARGTEMP         =0x00000400, /* device supports D3DTA_TEMP for temporary register */
        D3DPMISCCAPS_BLENDOP            =0x00000800, /* device supports D3DRS_BLENDOP */
        D3DPMISCCAPS_NULLREFERENCE      =0x00001000, /* Reference Device that doesnt render */
        D3DPMISCCAPS_INDEPENDENTWRITEMASKS     =0x00004000, /* Device supports independent write masks for MET or MRT */
        D3DPMISCCAPS_PERSTAGECONSTANT   =0x00008000, /* Device supports per-stage constants */
        D3DPMISCCAPS_FOGANDSPECULARALPHA   =0x00010000, /* Device supports separate fog and specular alpha (many devices
                                                          use the specular alpha channel to store fog factor) */
        D3DPMISCCAPS_SEPARATEALPHABLEND         =0x00020000, /* Device supports separate blend settings for the alpha channel */
        D3DPMISCCAPS_MRTINDEPENDENTBITDEPTHS    =0x00040000, /* Device supports different bit depths for MRT */
        D3DPMISCCAPS_MRTPOSTPIXELSHADERBLENDING =0x00080000, /* Device supports post-pixel shader operations for MRT */
        D3DPMISCCAPS_FOGVERTEXCLAMPED           =0x00100000, /* Device clamps fog blend factor per vertex */

/* D3D9Ex only -- */
#if !D3D_DISABLE_9EX

        D3DPMISCCAPS_POSTBLENDSRGBCONVERT       =0x00200000, /* Indicates device can perform conversion to sRGB after blending. */

#endif // !D3D_DISABLE_9EX
/* -- D3D9Ex only */
    }

    [Flags]
    public enum D3DPRASTERCAPS
    {
        D3DPRASTERCAPS_DITHER                 =0x00000001,
        D3DPRASTERCAPS_ZTEST                  =0x00000010,
        D3DPRASTERCAPS_FOGVERTEX              =0x00000080,
        D3DPRASTERCAPS_FOGTABLE               =0x00000100,
        D3DPRASTERCAPS_MIPMAPLODBIAS          =0x00002000,
        D3DPRASTERCAPS_ZBUFFERLESSHSR         =0x00008000,
        D3DPRASTERCAPS_FOGRANGE               =0x00010000,
        D3DPRASTERCAPS_ANISOTROPY             =0x00020000,
        D3DPRASTERCAPS_WBUFFER                =0x00040000,
        D3DPRASTERCAPS_WFOG                   =0x00100000,
        D3DPRASTERCAPS_ZFOG                   =0x00200000,
        D3DPRASTERCAPS_COLORPERSPECTIVE       =0x00400000, /* Device iterates colors perspective correct */
        D3DPRASTERCAPS_SCISSORTEST            =0x01000000,
        D3DPRASTERCAPS_SLOPESCALEDEPTHBIAS    =0x02000000,
        D3DPRASTERCAPS_DEPTHBIAS              =0x04000000, 
        D3DPRASTERCAPS_MULTISAMPLE_TOGGLE     =0x08000000,
    }

    [Flags]
    public enum D3DPCMPCAPS
    {
        D3DPCMPCAPS_NEVER               =0x00000001,
        D3DPCMPCAPS_LESS                =0x00000002,
        D3DPCMPCAPS_EQUAL               =0x00000004,
        D3DPCMPCAPS_LESSEQUAL           =0x00000008,
        D3DPCMPCAPS_GREATER             =0x00000010,
        D3DPCMPCAPS_NOTEQUAL            =0x00000020,
        D3DPCMPCAPS_GREATEREQUAL        =0x00000040,
        D3DPCMPCAPS_ALWAYS              =0x00000080,
    }

    [Flags]
    public enum D3DPBLENDCAPS
    {
        D3DPBLENDCAPS_ZERO              =0x00000001,
        D3DPBLENDCAPS_ONE               =0x00000002,
        D3DPBLENDCAPS_SRCCOLOR          =0x00000004,
        D3DPBLENDCAPS_INVSRCCOLOR       =0x00000008,
        D3DPBLENDCAPS_SRCALPHA          =0x00000010,
        D3DPBLENDCAPS_INVSRCALPHA       =0x00000020,
        D3DPBLENDCAPS_DESTALPHA         =0x00000040,
        D3DPBLENDCAPS_INVDESTALPHA      =0x00000080,
        D3DPBLENDCAPS_DESTCOLOR         =0x00000100,
        D3DPBLENDCAPS_INVDESTCOLOR      =0x00000200,
        D3DPBLENDCAPS_SRCALPHASAT       =0x00000400,
        D3DPBLENDCAPS_BOTHSRCALPHA      =0x00000800,
        D3DPBLENDCAPS_BOTHINVSRCALPHA   =0x00001000,
        D3DPBLENDCAPS_BLENDFACTOR       =0x00002000, /* Supports both D3DBLEND_BLENDFACTOR and D3DBLEND_INVBLENDFACTOR */
/* D3D9Ex only -- */
#if !D3D_DISABLE_9EX

        D3DPBLENDCAPS_SRCCOLOR2         =0x00004000,
        D3DPBLENDCAPS_INVSRCCOLOR2      =0x00008000,

#endif // !D3D_DISABLE_9EX
/* -- D3D9Ex only */
    }

    [Flags]
    public enum D3DPSHADECAPS
    {
        D3DPSHADECAPS_COLORGOURAUDRGB       =0x00000008,
        D3DPSHADECAPS_SPECULARGOURAUDRGB    =0x00000200,
        D3DPSHADECAPS_ALPHAGOURAUDBLEND     =0x00004000,
        D3DPSHADECAPS_FOGGOURAUD            =0x00080000,
    }

    [Flags]
    public enum D3DPTEXTURECAPS
    {
        D3DPTEXTURECAPS_PERSPECTIVE         =0x00000001, /* Perspective-correct texturing is supported */
        D3DPTEXTURECAPS_POW2                =0x00000002, /* Power-of-2 texture dimensions are required - applies to non-Cube/Volume textures only. */
        D3DPTEXTURECAPS_ALPHA               =0x00000004, /* Alpha in texture pixels is supported */
        D3DPTEXTURECAPS_SQUAREONLY          =0x00000020, /* Only square textures are supported */
        D3DPTEXTURECAPS_TEXREPEATNOTSCALEDBYSIZE =0x00000040, /* Texture indices are not scaled by the texture size prior to interpolation */
        D3DPTEXTURECAPS_ALPHAPALETTE        =0x00000080, /* Device can draw alpha from texture palettes */
        // Device can use non-POW2 textures if:
        //  1) D3DTEXTURE_ADDRESS is set to CLAMP for this texture's stage
        //  2) D3DRS_WRAP(N) is zero for this texture's coordinates
        //  3) mip mapping is not enabled (use magnification filter only)
        D3DPTEXTURECAPS_NONPOW2CONDITIONAL  =0x00000100,
        D3DPTEXTURECAPS_PROJECTED           =0x00000400, /* Device can do D3DTTFF_PROJECTED */
        D3DPTEXTURECAPS_CUBEMAP             =0x00000800, /* Device can do cubemap textures */
        D3DPTEXTURECAPS_VOLUMEMAP           =0x00002000, /* Device can do volume textures */
        D3DPTEXTURECAPS_MIPMAP              =0x00004000, /* Device can do mipmapped textures */
        D3DPTEXTURECAPS_MIPVOLUMEMAP        =0x00008000, /* Device can do mipmapped volume textures */
        D3DPTEXTURECAPS_MIPCUBEMAP          =0x00010000, /* Device can do mipmapped cube maps */
        D3DPTEXTURECAPS_CUBEMAP_POW2        =0x00020000, /* Device requires that cubemaps be power-of-2 dimension */
        D3DPTEXTURECAPS_VOLUMEMAP_POW2      =0x00040000, /* Device requires that volume maps be power-of-2 dimension */
        D3DPTEXTURECAPS_NOPROJECTEDBUMPENV = 0x00200000, /* Device does not support projected bump env lookup operation in programmable and fixed function pixel shaders */
    }

    [Flags]
    public enum D3DPTFILTERCAPS
    {
        D3DPTFILTERCAPS_MINFPOINT           =0x00000100, /* Min Filter */
        D3DPTFILTERCAPS_MINFLINEAR          =0x00000200,
        D3DPTFILTERCAPS_MINFANISOTROPIC     =0x00000400,
        D3DPTFILTERCAPS_MINFPYRAMIDALQUAD   =0x00000800,
        D3DPTFILTERCAPS_MINFGAUSSIANQUAD    =0x00001000,
        D3DPTFILTERCAPS_MIPFPOINT           =0x00010000, /* Mip Filter */
        D3DPTFILTERCAPS_MIPFLINEAR          =0x00020000,

/* D3D9Ex only -- */
#if !D3D_DISABLE_9EX

        D3DPTFILTERCAPS_CONVOLUTIONMONO     =0x00040000, /* Min and Mag for the convolution mono filter */

#endif // !D3D_DISABLE_9EX
/* -- D3D9Ex only */

        D3DPTFILTERCAPS_MAGFPOINT           =0x01000000, /* Mag Filter */
        D3DPTFILTERCAPS_MAGFLINEAR          =0x02000000,
        D3DPTFILTERCAPS_MAGFANISOTROPIC     =0x04000000,
        D3DPTFILTERCAPS_MAGFPYRAMIDALQUAD   =0x08000000,
        D3DPTFILTERCAPS_MAGFGAUSSIANQUAD    =0x10000000,
    }

    [Flags]
    public enum D3DPTADDRESSCAPS
    {
        D3DPTADDRESSCAPS_WRAP           =0x00000001,
        D3DPTADDRESSCAPS_MIRROR         =0x00000002,
        D3DPTADDRESSCAPS_CLAMP          =0x00000004,
        D3DPTADDRESSCAPS_BORDER         =0x00000008,
        D3DPTADDRESSCAPS_INDEPENDENTUV  =0x00000010,
        D3DPTADDRESSCAPS_MIRRORONCE     =0x00000020,
    }

    [Flags]
    public enum D3DLINECAPS
    {
        D3DLINECAPS_TEXTURE             =0x00000001,
        D3DLINECAPS_ZTEST               =0x00000002,
        D3DLINECAPS_BLEND               =0x00000004,
        D3DLINECAPS_ALPHACMP            =0x00000008,
        D3DLINECAPS_FOG                 =0x00000010,
        D3DLINECAPS_ANTIALIAS           =0x00000020,
    }

    [Flags]
    public enum D3DSTENCILCAPS
    {
        D3DSTENCILCAPS_KEEP             =0x00000001,
        D3DSTENCILCAPS_ZERO             =0x00000002,
        D3DSTENCILCAPS_REPLACE          =0x00000004,
        D3DSTENCILCAPS_INCRSAT          =0x00000008,
        D3DSTENCILCAPS_DECRSAT          =0x00000010,
        D3DSTENCILCAPS_INVERT           =0x00000020,
        D3DSTENCILCAPS_INCR             =0x00000040,
        D3DSTENCILCAPS_DECR             =0x00000080,
        D3DSTENCILCAPS_TWOSIDED         =0x00000100,
    }

    [Flags]
    public enum D3DFVFCAPS
    {
        D3DFVFCAPS_TEXCOORDCOUNTMASK    =0x0000ffff, /* mask for texture coordinate count field */
        D3DFVFCAPS_DONOTSTRIPELEMENTS   =0x00080000, /* Device prefers that vertex elements not be stripped */
        D3DFVFCAPS_PSIZE                =0x00100000, /* Device can receive point size */
    }

    [Flags]
    public enum D3DTEXOPCAPS
    {
        D3DTEXOPCAPS_DISABLE                    =0x00000001,
        D3DTEXOPCAPS_SELECTARG1                 =0x00000002,
        D3DTEXOPCAPS_SELECTARG2                 =0x00000004,
        D3DTEXOPCAPS_MODULATE                   =0x00000008,
        D3DTEXOPCAPS_MODULATE2X                 =0x00000010,
        D3DTEXOPCAPS_MODULATE4X                 =0x00000020,
        D3DTEXOPCAPS_ADD                        =0x00000040,
        D3DTEXOPCAPS_ADDSIGNED                  =0x00000080,
        D3DTEXOPCAPS_ADDSIGNED2X                =0x00000100,
        D3DTEXOPCAPS_SUBTRACT                   =0x00000200,
        D3DTEXOPCAPS_ADDSMOOTH                  =0x00000400,
        D3DTEXOPCAPS_BLENDDIFFUSEALPHA          =0x00000800,
        D3DTEXOPCAPS_BLENDTEXTUREALPHA          =0x00001000,
        D3DTEXOPCAPS_BLENDFACTORALPHA           =0x00002000,
        D3DTEXOPCAPS_BLENDTEXTUREALPHAPM        =0x00004000,
        D3DTEXOPCAPS_BLENDCURRENTALPHA          =0x00008000,
        D3DTEXOPCAPS_PREMODULATE                =0x00010000,
        D3DTEXOPCAPS_MODULATEALPHA_ADDCOLOR     =0x00020000,
        D3DTEXOPCAPS_MODULATECOLOR_ADDALPHA     =0x00040000,
        D3DTEXOPCAPS_MODULATEINVALPHA_ADDCOLOR  =0x00080000,
        D3DTEXOPCAPS_MODULATEINVCOLOR_ADDALPHA  =0x00100000,
        D3DTEXOPCAPS_BUMPENVMAP                 =0x00200000,
        D3DTEXOPCAPS_BUMPENVMAPLUMINANCE        =0x00400000,
        D3DTEXOPCAPS_DOTPRODUCT3                =0x00800000,
        D3DTEXOPCAPS_MULTIPLYADD                =0x01000000,
        D3DTEXOPCAPS_LERP                       =0x02000000,
    }

    [Flags]
    public enum D3DVTXPCAPS
    {
        D3DVTXPCAPS_TEXGEN              =0x00000001, /* device can do texgen */
        D3DVTXPCAPS_MATERIALSOURCE7     =0x00000002, /* device can do DX7-level colormaterialsource ops */
        D3DVTXPCAPS_DIRECTIONALLIGHTS   =0x00000008, /* device can do directional lights */
        D3DVTXPCAPS_POSITIONALLIGHTS    =0x00000010, /* device can do positional lights (includes point and spot) */
        D3DVTXPCAPS_LOCALVIEWER         =0x00000020, /* device can do local viewer */
        D3DVTXPCAPS_TWEENING            =0x00000040, /* device can do vertex tweening */
        D3DVTXPCAPS_TEXGEN_SPHEREMAP    =0x00000100, /* device supports D3DTSS_TCI_SPHEREMAP */
        D3DVTXPCAPS_NO_TEXGEN_NONLOCALVIEWER   =0x00000200, /* device does not support TexGen in non-local viewer mode */
    }

    [Flags]
    public enum D3DDEVCAPS2
    {
        D3DDEVCAPS2_STREAMOFFSET                        =0x00000001, /* Device supports offsets in streams. Must be set by DX9 drivers */
        D3DDEVCAPS2_DMAPNPATCH                          =0x00000002, /* Device supports displacement maps for N-Patches*/
        D3DDEVCAPS2_ADAPTIVETESSRTPATCH                 =0x00000004, /* Device supports adaptive tesselation of RT-patches*/
        D3DDEVCAPS2_ADAPTIVETESSNPATCH                  =0x00000008, /* Device supports adaptive tesselation of N-patches*/
        D3DDEVCAPS2_CAN_STRETCHRECT_FROM_TEXTURES       =0x00000010, /* Device supports StretchRect calls with a texture as the source*/
        D3DDEVCAPS2_PRESAMPLEDDMAPNPATCH                =0x00000020, /* Device supports presampled displacement maps for N-Patches */
        D3DDEVCAPS2_VERTEXELEMENTSCANSHARESTREAMOFFSET  =0x00000040, /* Vertex elements in a vertex declaration can share the same stream offset */
    }

    [Flags]
    public enum D3DDTCAPS
    {
        D3DDTCAPS_UBYTE4     =0x00000001,
        D3DDTCAPS_UBYTE4N    =0x00000002,
        D3DDTCAPS_SHORT2N    =0x00000004,
        D3DDTCAPS_SHORT4N    =0x00000008,
        D3DDTCAPS_USHORT2N   =0x00000010,
        D3DDTCAPS_USHORT4N   =0x00000020,
        D3DDTCAPS_UDEC3      =0x00000040,
        D3DDTCAPS_DEC3N      =0x00000080,
        D3DDTCAPS_FLOAT16_2  =0x00000100,
        D3DDTCAPS_FLOAT16_4  =0x00000200,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3DVSHADERCAPS2_0
    {
        public int Caps;                        // DWORD
        public int DynamicFlowControlDepth;     // INT
        public int NumTemps;                    // INT
        public int StaticFlowControlDepth;      // INT
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct D3DPSHADERCAPS2_0
    {
        public int Caps;                        // DWORD
        public int DynamicFlowControlDepth;     // INT
        public int NumTemps;                    // INT
        public int StaticFlowControlDepth;      // INT
        public int NumInstructionSlots;         // INT
    }
}
