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
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ZXMAK2.DirectX.Native;
using ZXMAK2.DirectX.Vectors;


namespace ZXMAK2.DirectX.Direct3D
{
    /// <unmanaged>IDirect3DDevice9</unmanaged>	
    [Guid("D0223B96-BF7A-43fd-92BD-A43B0D82B9EB")]
    public class Direct3DDevice9 : ComObject
    {
        public Direct3DDevice9(IntPtr nativePointer)
            : base(nativePointer)
        {
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::Clear([In] unsigned int Count,[In, Buffer, Optional] const D3DRECT* pRects,[In] D3DCLEAR Flags,[In] D3DCOLOR Color,[In] float Z,[In] unsigned int Stencil)</unmanaged>	
        private unsafe HRESULT Clear_(int count, D3DRECT[] rectsRef, D3DCLEAR flags, D3DCOLOR color, float z, int stencil)
        {
            fixed (void* pRects = rectsRef)
            {
                //result = calli(System.Int32(System.Void*,System.Int32,System.Void*,System.Int32,SharpDX.Mathematics.Interop.RawColorBGRA,System.Single,System.Int32), this._nativePointer, count, ptr, flags, color, z, stencil, *(*(IntPtr*)this._nativePointer + (IntPtr)43 * (IntPtr)sizeof(void*)));
                return (HRESULT)NativeHelper.CalliInt32(43, _nativePointer, count, pRects, (int)flags, (int)color, z, stencil);
            }
        }

        /// <summary>	
        /// <p>Presents the contents of the next buffer in the sequence of back buffers owned by the device.</p>	
        /// </summary>	
        /// <param name="sourceRectRef"><dd>  <p>Pointer to a value that must be <strong><c>null</c></strong> unless the swap chain was created with SwapEffect.Copy. pSourceRect is a reference to a <strong>RawRectangle</strong> structure containing the source rectangle. If <strong><c>null</c></strong>, the entire source surface is presented. If the rectangle exceeds the source surface, the rectangle is clipped to the source surface. </p> </dd></param>	
        /// <param name="destRectRef"><dd>  <p>Pointer to a value that must be <strong><c>null</c></strong> unless the swap chain was created with SwapEffect.Copy. pDestRect is a reference to a <strong>RawRectangle</strong> structure containing the destination rectangle, in window client coordinates. If <strong><c>null</c></strong>, the entire client area is filled. If the rectangle exceeds the destination client area, the rectangle is clipped to the destination client area. </p> </dd></param>	
        /// <param name="hDestWindowOverride"><dd>  <p>Pointer to a destination window whose client area is taken as the target for this presentation. If this value is <strong><c>null</c></strong>, the runtime uses the <strong>hDeviceWindow</strong> member of <strong>PresentParameters</strong> for the presentation.</p> </dd></param>	
        /// <param name="dirtyRegionRef"><dd>  <p>Value must be <strong><c>null</c></strong> unless the swap chain was created with SwapEffect.Copy. For more information about swap chains, see Flipping Surfaces (Direct3D 9) and <strong>SwapEffect</strong>. If this value is non-<strong><c>null</c></strong>, the contained region is expressed in back buffer coordinates. The rectangles within the region are the minimal set of pixels that need to be updated. This method takes these rectangles into account when optimizing the presentation by copying only the pixels within the region, or some suitably expanded set of rectangles. This is an aid to optimization only, and the application should not rely on the region being copied exactly. The implementation can choose to copy the whole source rectangle.  </p> </dd></param>	
        /// <returns><p>Possible return values include: Success or DeviceRemoved (see D3DERR).</p></returns>	
        /// <remarks>	
        /// <p>If necessary, a stretch operation is applied to transfer the pixels within the source rectangle to the destination rectangle in the client area of the target window. </p><p><strong>Present</strong> will fail, returning InvalidCall, if called between BeginScene and EndScene pairs unless the render target is not the current render target (such as the back buffer you get from creating an additional swap chain). This is a new behavior for Direct3D 9. </p>	
        /// </remarks>	
        /// <unmanaged>HRESULT IDirect3DDevice9::Present([In] const void* pSourceRect,[In] const void* pDestRect,[In] HWND hDestWindowOverride,[In] const void* pDirtyRegion)</unmanaged>	
        private unsafe HRESULT Present_(IntPtr sourceRectRef, IntPtr destRectRef, IntPtr hDestWindowOverride, IntPtr dirtyRegionRef)
        {
	        //calli(System.Int32(System.Void*,System.Void*,System.Void*,System.Void*,System.Void*), this._nativePointer, (void*)sourceRectRef, (void*)destRectRef, (void*)hDestWindowOverride, (void*)dirtyRegionRef, *(*(IntPtr*)this._nativePointer + (IntPtr)17 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(17, _nativePointer, (void*)sourceRectRef, (void*)destRectRef, (void*)hDestWindowOverride, (void*)dirtyRegionRef);
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::CreateTexture([In] unsigned int Width,[In] unsigned int Height,[In] unsigned int Levels,[In] unsigned int Usage,[In] D3DFORMAT Format,[In] D3DPOOL Pool,[Out, Fast] IDirect3DTexture9** ppTexture,[In] void** pSharedHandle)</unmanaged>	
        private unsafe HRESULT CreateTexture_(int width, int height, int levels, D3DUSAGE usage, D3DFORMAT format, D3DPOOL pool, out IntPtr pTexture, ref IntPtr sharedHandleRef)
        {
            fixed (void* ppTexture = &pTexture)
            {
                //result = calli(System.Int32(System.Void*,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Void*,System.Void*), this._nativePointer, width, height, levels, usage, format, pool, &zero, (void*)sharedHandleRef, *(*(IntPtr*)this._nativePointer + (IntPtr)23 * (IntPtr)sizeof(void*)));
                return (HRESULT)NativeHelper.CalliInt32(23, _nativePointer, (int)width, (int)height, (int)levels, (int)usage, (int)format, (int)pool, (void*)ppTexture, (void*)sharedHandleRef);
            }
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::CreateTexture([In] unsigned int Width,[In] unsigned int Height,[In] unsigned int Levels,[In] unsigned int Usage,[In] D3DFORMAT Format,[In] D3DPOOL Pool,[Out, Fast] IDirect3DTexture9** ppTexture,[In] void** pSharedHandle)</unmanaged>	
        private unsafe HRESULT CreateTexture_(int width, int height, int levels, D3DUSAGE usage, D3DFORMAT format, D3DPOOL pool, out IntPtr pTexture)
        {
            fixed (void* ppTexture = &pTexture)
            {
                //result = calli(System.Int32(System.Void*,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Int32,System.Void*,System.Void*), this._nativePointer, width, height, levels, usage, format, pool, &zero, (void*)sharedHandleRef, *(*(IntPtr*)this._nativePointer + (IntPtr)23 * (IntPtr)sizeof(void*)));
                return (HRESULT)NativeHelper.CalliInt32(23, _nativePointer, (int)width, (int)height, (int)levels, (int)usage, (int)format, (int)pool, (void*)ppTexture, (void*)IntPtr.Zero);
            }
        }

        public D3DDISPLAYMODE DisplayMode
        {
            get
            {
                D3DDISPLAYMODE mode;
                var hr = GetDisplayMode(0, out mode);
                hr.CheckError();
                return mode;
            }
        }

        public D3DFVF VertexFormat
        {
            get
            {
                D3DFVF vertexFormat;
                var hr = GetFVF(out vertexFormat);
                hr.CheckError();
                return vertexFormat;
            }
            set { SetFVF(value).CheckError(); }
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::GetSwapChain([In] unsigned int iSwapChain,[Out] IDirect3DSwapChain9** pSwapChain)</unmanaged>	
        public unsafe Direct3DSwapChain9 GetSwapChain(int iSwapChain)
        {
            var nativePointer = IntPtr.Zero;
            //result = calli(System.Int32(System.Void*,System.Int32,System.Void*), this._nativePointer, iSwapChain, &result, *(*(IntPtr*)this._nativePointer + (IntPtr)14 * (IntPtr)sizeof(void*)));
            var hr = (HRESULT)NativeHelper.CalliInt32(14, _nativePointer, (int)iSwapChain, (void*)&nativePointer);
            hr.CheckError();
            if (nativePointer == IntPtr.Zero) return null;
            return new Direct3DSwapChain9(nativePointer);
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::CreateAdditionalSwapChain([In] D3DPRESENT_PARAMETERS* pPresentationParameters,[Out, Fast] IDirect3DSwapChain9** pSwapChain)</unmanaged>	
        public unsafe Direct3DSwapChain9 CreateAdditionalSwapChain(ref D3DPRESENT_PARAMETERS presentationParameters)
        {
            var nativePointer = IntPtr.Zero;
            HRESULT hr;
            fixed (void* pPresentationParameters = &presentationParameters)
            {
                //result = calli(System.Int32(System.Void*,System.Void*,System.Void*), this._nativePointer, ptr, &zero, *(*(IntPtr*)this._nativePointer + (IntPtr)13 * (IntPtr)sizeof(void*)));
                hr = (HRESULT)NativeHelper.CalliInt32(13, _nativePointer, (void*)pPresentationParameters, (void*)&nativePointer);
            }
            hr.CheckError();
            if (nativePointer == IntPtr.Zero) return null;
            return new Direct3DSwapChain9(nativePointer);
        }
        
        public HRESULT Clear(D3DCLEAR clearFlags, D3DCOLOR color, float zdepth, int stencil)
        {
            return Clear_(0, null, clearFlags, color, zdepth, stencil);
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::BeginScene()</unmanaged>	
        public unsafe HRESULT BeginScene()
        {
	        //calli(System.Int32(System.Void*), this._nativePointer, *(*(IntPtr*)this._nativePointer + (IntPtr)41 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(41, _nativePointer);
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::EndScene()</unmanaged>	
        public unsafe HRESULT EndScene()
        {
	        //calli(System.Int32(System.Void*), this._nativePointer, *(*(IntPtr*)this._nativePointer + (IntPtr)42 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(42, _nativePointer);
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::TestCooperativeLevel()</unmanaged>	
        public unsafe HRESULT TestCooperativeLevel()
        {
	        //return calli(System.Int32(System.Void*), this._nativePointer, *(*(IntPtr*)this._nativePointer + (IntPtr)3 * (IntPtr)sizeof(void*)));
            return (HRESULT)NativeHelper.CalliInt32(3, _nativePointer);
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::SetRenderTarget([In] unsigned int RenderTargetIndex,[In] IDirect3DSurface9* pRenderTarget)</unmanaged>	
        public unsafe HRESULT SetRenderTarget(int renderTargetIndex, Direct3DSurface9 renderTargetRef)
        {
	        //calli(System.Int32(System.Void*,System.Int32,System.Void*), this._nativePointer, renderTargetIndex, (void*)((renderTargetRef == null) ? IntPtr.Zero : renderTargetRef.NativePointer), *(*(IntPtr*)this._nativePointer + (IntPtr)37 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(37, _nativePointer, renderTargetIndex, (void*)((renderTargetRef == null) ? IntPtr.Zero : renderTargetRef.NativePointer));
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::SetRenderState([In] D3DRENDERSTATETYPE State,[In] unsigned int Value)</unmanaged>	
        public unsafe HRESULT SetRenderState(D3DRENDERSTATETYPE state, int value)
        {
	        //calli(System.Int32(System.Void*,System.Int32,System.Int32), this._nativePointer, state, value, *(*(IntPtr*)this._nativePointer + (IntPtr)57 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(57, _nativePointer, (int)state, (int)value);
        }

        public unsafe HRESULT SetRenderState(D3DRENDERSTATETYPE renderState, float value)
        {
            return SetRenderState(renderState, *(int*)(&value));
        }

        public HRESULT SetRenderState(D3DRENDERSTATETYPE renderState, bool enable)
        {
            return SetRenderState(renderState, enable ? 1 : 0);
        }
                
        /// <unmanaged>HRESULT IDirect3DDevice9::SetSamplerState([In] unsigned int Sampler,[In] D3DSAMPLERSTATETYPE Type,[In] unsigned int Value)</unmanaged>	
        public unsafe HRESULT SetSamplerState(int sampler, D3DSAMPLERSTATETYPE type, int value)
        {
	        //calli(System.Int32(System.Void*,System.Int32,System.Int32,System.Int32), this._nativePointer, sampler, type, value, *(*(IntPtr*)this._nativePointer + (IntPtr)69 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(69, _nativePointer, (int)sampler, (int)type, (int)value);
        }

        public unsafe HRESULT SetSamplerState(int sampler, D3DSAMPLERSTATETYPE type, float value)
        {
            return SetSamplerState(sampler, type, *(int*)(&value));
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::Reset([In, Out, Params] D3DPRESENT_PARAMETERS* pPresentationParameters)</unmanaged>	
        public unsafe HRESULT Reset(params D3DPRESENT_PARAMETERS[] presentationParametersRef)
        {
	        fixed (void* pPresentationParameters = presentationParametersRef)
	        {
                //hr = calli(System.Int32(System.Void*,System.Void*), this._nativePointer, pPresentationParameters, *(*(IntPtr*)this._nativePointer + (IntPtr)16 * (IntPtr)sizeof(void*)));
                return (HRESULT)NativeHelper.CalliInt32(16, _nativePointer, pPresentationParameters);
	        }
        }

        public HRESULT Present()
        {
            return Present_(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        }

        public Direct3DTexture9 CreateTexture(int width, int height, int levelCount, D3DUSAGE usage, D3DFORMAT format, D3DPOOL pool, ref IntPtr sharedHandle)
        {
            var nativePointer = IntPtr.Zero;
            var hr = CreateTexture_(width, height, levelCount, usage, format, pool, out nativePointer, ref sharedHandle);
            hr.CheckError();
            if (nativePointer == IntPtr.Zero) return null;
            return new Direct3DTexture9(nativePointer);
        }

        public Direct3DTexture9 CreateTexture(int width, int height, int levelCount, D3DUSAGE usage, D3DFORMAT format, D3DPOOL pool)
        {
            var nativePointer = IntPtr.Zero;
            var hr = CreateTexture_(width, height, levelCount, usage, format, pool, out nativePointer);
            hr.CheckError();
            if (nativePointer == IntPtr.Zero) return null;
            return new Direct3DTexture9(nativePointer);
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::SetFVF([In] D3DFVF FVF)</unmanaged>	
        public unsafe HRESULT SetFVF(D3DFVF vertexFormat)
        {
            //calli(System.Int32(System.Void*,System.Int32), this._nativePointer, vertexFormat, *(*(IntPtr*)this._nativePointer + (IntPtr)89 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(89, _nativePointer, (int)vertexFormat);
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::GetFVF([Out] D3DFVF* pFVF)</unmanaged>	
        public unsafe HRESULT GetFVF(out D3DFVF vertexFormat)
        {
	        fixed (void* ptr = &vertexFormat)
	        {
		        //result = calli(System.Int32(System.Void*,System.Void*), this._nativePointer, ptr, *(*(IntPtr*)this._nativePointer + (IntPtr)90 * (IntPtr)sizeof(void*)));
                return (HRESULT)NativeHelper.CalliInt32(90, _nativePointer, (void*)ptr);
	        }
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::SetStreamSource([In] unsigned int StreamNumber,[In] IDirect3DVertexBuffer9* pStreamData,[In] unsigned int OffsetInBytes,[In] unsigned int Stride)</unmanaged>	
        public unsafe HRESULT SetStreamSource(int streamNumber, Direct3DVertexBuffer9 streamData, int offsetInBytes, int stride)
        {
            var pStreamData = (void*)(streamData == null ? IntPtr.Zero : streamData.NativePointer);
            //calli(System.Int32(System.Void*,System.Int32,System.Void*,System.Int32,System.Int32), this._nativePointer, streamNumber, (void*)((streamDataRef == null) ? IntPtr.Zero : streamDataRef.NativePointer), offsetInBytes, stride, *(*(IntPtr*)this._nativePointer + (IntPtr)100 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(100, _nativePointer, (int)streamNumber, (void*)pStreamData, (int)offsetInBytes, (int)stride);
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::DrawPrimitive([In] D3DPRIMITIVETYPE PrimitiveType,[In] unsigned int StartVertex,[In] unsigned int PrimitiveCount)</unmanaged>	
        public unsafe HRESULT DrawPrimitive(D3DPRIMITIVETYPE primitiveType, int startVertex, int primitiveCount)
        {
	        //calli(System.Int32(System.Void*,System.Int32,System.Int32,System.Int32), this._nativePointer, primitiveType, startVertex, primitiveCount, *(*(IntPtr*)this._nativePointer + (IntPtr)81 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(81, _nativePointer, (int)primitiveType, (int)startVertex, (int)primitiveCount);
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::CreateVertexBuffer([In] unsigned int Length,[In] D3DUSAGE Usage,[In] D3DFVF FVF,[In] D3DPOOL Pool,[Out, Fast] IDirect3DVertexBuffer9** ppVertexBuffer,[In] void** pSharedHandle)</unmanaged>	
        public unsafe Direct3DVertexBuffer9 CreateVertexBuffer(int length, D3DUSAGE usage, D3DFVF vertexFormat, D3DPOOL pool)
        {
	        var nativePointer = IntPtr.Zero;
            //result = calli(System.Int32(System.Void*,System.Int32,System.Int32,System.Int32,System.Int32,System.Void*,System.Void*), this._nativePointer, length, usage, vertexFormat, pool, &zero, (void*)sharedHandleRef, *(*(IntPtr*)this._nativePointer + (IntPtr)26 * (IntPtr)sizeof(void*)));
            var hr = (HRESULT)NativeHelper.CalliInt32(26, _nativePointer, (int)length, (int)usage, (int)vertexFormat, (int)pool, (void*)&nativePointer, (void*)IntPtr.Zero);
            hr.CheckError();
            if (nativePointer == IntPtr.Zero) return null;
            return new Direct3DVertexBuffer9(nativePointer);
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::GetDisplayMode([In] unsigned int iSwapChain,[Out] D3DDISPLAYMODE* pMode)</unmanaged>	
        public unsafe HRESULT GetDisplayMode(int iSwapChain, out D3DDISPLAYMODE mode)
        {
            fixed (void* pMode = &mode)
            {
                //calli(System.Int32(System.Void*,System.Int32,System.Void*), this._nativePointer, iSwapChain, &result, *(*(IntPtr*)this._nativePointer + (IntPtr)8 * (IntPtr)sizeof(void*))).CheckError();
                return (HRESULT)NativeHelper.CalliInt32(8, _nativePointer, (int)iSwapChain, (void*)pMode);
            }
        }

        /// <unmanaged>HRESULT IDirect3DDevice9::DrawPrimitiveUP([In] D3DPRIMITIVETYPE PrimitiveType,[In] unsigned int PrimitiveCount,[In] const void* pVertexStreamZeroData,[In] unsigned int VertexStreamZeroStride)</unmanaged>	
        public unsafe HRESULT DrawPrimitiveUP(D3DPRIMITIVETYPE primitiveType, int primitiveCount, void* pVertexStreamZeroData, int vertexStreamZeroStride)
        {
            //calli(System.Int32(System.Void*,System.Int32,System.Int32,System.Void*,System.Int32), this._nativePointer, primitiveType, primitiveCount, (void*)vertexStreamZeroDataRef, vertexStreamZeroStride, *(*(IntPtr*)this._nativePointer + (IntPtr)83 * (IntPtr)sizeof(void*))).CheckError();
            return (HRESULT)NativeHelper.CalliInt32(83, _nativePointer, (int)primitiveType, (int)primitiveCount, (void*)pVertexStreamZeroData, (int)vertexStreamZeroStride);
        }

        public HRESULT DrawUserPrimitives<T>(D3DPRIMITIVETYPE primitiveType, int primitiveCount, T[] data) 
            where T : struct
        {
            return DrawUserPrimitives<T>(primitiveType, 0, primitiveCount, data);
        }

        [MethodImpl(MethodImplOptions.ForwardRef)]
        public extern unsafe HRESULT DrawUserPrimitives<T>(D3DPRIMITIVETYPE primitiveType, int startIndex, int primitiveCount, T[] data) 
            where T : struct;
    }
}
