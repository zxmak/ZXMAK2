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
    [StructLayout(LayoutKind.Sequential)]
    public struct D3DCAPS9
    {
        /* Device Info */
        public D3DDEVTYPE DeviceType;                       // D3DDEVTYPE
        public int AdapterOrdinal;                          // UINT

        /* Caps from DX7 Draw */
        public D3DCAPS Caps;                                // DWORD
        public D3DCAPS2 Caps2;                              // DWORD
        public D3DCAPS3 Caps3;                              // DWORD
        public D3DPRESENT_INTERVAL PresentationIntervals;   // DWORD

        /* Cursor Caps */
        public D3DCURSORCAPS CursorCaps;                    // DWORD

        /* 3D Device Caps */
        public D3DDEVCAPS DevCaps;                          // DWORD

        public D3DPMISCCAPS PrimitiveMiscCaps;              // DWORD
        public D3DPRASTERCAPS RasterCaps;                   // DWORD
        public D3DPCMPCAPS ZCmpCaps;                        // DWORD
        public D3DPBLENDCAPS SrcBlendCaps;                  // DWORD
        public D3DPBLENDCAPS DestBlendCaps;                 // DWORD
        public D3DPCMPCAPS AlphaCmpCaps;                    // DWORD
        public D3DPSHADECAPS ShadeCaps;                     // DWORD
        public D3DPTEXTURECAPS TextureCaps;                 // DWORD
        public D3DPTFILTERCAPS TextureFilterCaps;           // DWORD // D3DPTFILTERCAPS for IDirect3DTexture9's
        public D3DPTFILTERCAPS CubeTextureFilterCaps;       // DWORD // D3DPTFILTERCAPS for IDirect3DCubeTexture9's
        public D3DPTFILTERCAPS VolumeTextureFilterCaps;     // DWORD // D3DPTFILTERCAPS for IDirect3DVolumeTexture9's
        public D3DPTADDRESSCAPS TextureAddressCaps;         // DWORD // D3DPTADDRESSCAPS for IDirect3DTexture9's
        public D3DPTADDRESSCAPS VolumeTextureAddressCaps;   // DWORD // D3DPTADDRESSCAPS for IDirect3DVolumeTexture9's

        public D3DLINECAPS LineCaps;                        // DWORD // D3DLINECAPS

        public int MaxTextureWidth, MaxTextureHeight;       // DWORD
        public int MaxVolumeExtent;                         // DWORD

        public int MaxTextureRepeat;                        // DWORD
        public int MaxTextureAspectRatio;                   // DWORD
        public int MaxAnisotropy;                           // DWORD
        public float MaxVertexW;                            // float

        public float GuardBandLeft;                         // float
        public float GuardBandTop;                          // float
        public float GuardBandRight;                        // float
        public float GuardBandBottom;                       // float

        public float ExtentsAdjust;                         // float
        public D3DSTENCILCAPS StencilCaps;                  // DWORD

        public D3DFVFCAPS FVFCaps;                          // DWORD
        public D3DTEXOPCAPS TextureOpCaps;                  // DWORD
        public int MaxTextureBlendStages;                   // DWORD
        public int MaxSimultaneousTextures;                 // DWORD

        public D3DVTXPCAPS VertexProcessingCaps;            // DWORD
        public int MaxActiveLights;                         // DWORD
        public int MaxUserClipPlanes;                       // DWORD
        public int MaxVertexBlendMatrices;                  // DWORD
        public int MaxVertexBlendMatrixIndex;               // DWORD

        public float MaxPointSize;                          // float

        public int MaxPrimitiveCount;           // DWORD    // max number of primitives per DrawPrimitive call
        public int MaxVertexIndex;              // DWORD
        public int MaxStreams;                  // DWORD
        public int MaxStreamStride;             // DWORD    // max stride for SetStreamSource

        //new Version(this.VertexShaderVersion_ >> 8 & 255, this.VertexShaderVersion_ & 255);
        public int VertexShaderVersion;         // DWORD
        public int MaxVertexShaderConst;        // DWORD    // number of vertex shader constant registers

        //new Version(this.PixelShaderVersion_ >> 8 & 255, this.PixelShaderVersion_ & 255);
        public int PixelShaderVersion;          // DWORD
        public float PixelShader1xMaxValue;     // float    // max value storable in registers of ps.1.x shaders

        // Here are the DX9 specific ones
        public D3DDEVCAPS2 DevCaps2;            // DWORD

        public float MaxNpatchTessellationLevel;    // float
        public int Reserved5;                       // DWORD

        public int MasterAdapterOrdinal;        // UINT // ordinal of master adaptor for adapter group
        public int AdapterOrdinalInGroup;       // UINT // ordinal inside the adapter group
        public int NumberOfAdaptersInGroup;     // UINT // number of adapters in this adapter group (only if master)
        public D3DDTCAPS DeclTypes;             // DWORD// Data types, supported in vertex declarations
        public int NumSimultaneousRTs;          // DWORD// Will be at least 1
        public D3DPTFILTERCAPS StretchRectFilterCaps;   // DWORD// Filter caps supported by StretchRect
        public D3DVSHADERCAPS2_0 VS20Caps;
        public D3DPSHADERCAPS2_0 PS20Caps;
        public D3DPTFILTERCAPS VertexTextureFilterCaps; // DWORD// D3DPTFILTERCAPS for IDirect3DTexture9's for texture, used in vertex shaders
        public int MaxVShaderInstructionsExecuted;      // DWORD// maximum number of vertex shader instructions that can be executed
        public int MaxPShaderInstructionsExecuted;      // DWORD// maximum number of pixel shader instructions that can be executed
        public int MaxVertexShader30InstructionSlots;   // DWORD
        public int MaxPixelShader30InstructionSlots;    // DWORD
    }
}
