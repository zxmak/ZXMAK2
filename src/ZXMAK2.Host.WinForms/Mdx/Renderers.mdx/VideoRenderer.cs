/* 
 *  Copyright 2008, 2015 Alex Makeev
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
 */
using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;
using ZXMAK2.Host.WinForms.Controls;
using ZXMAK2.Host.WinForms.Tools;
using ZXMAK2.Host.Presentation.Interfaces;


namespace ZXMAK2.Host.WinForms.Mdx.Renderers
{
    public class VideoRenderer : RendererBase
    {
        #region Constants

        private const byte MimicTvRatio = 4;      // mask size 1/x of pixel
        private const byte MimicTvAlpha = 0x90;   // mask alpha

        #endregion Constants


        #region Fields

        private readonly ConcurrentQueue<IFrameVideo> _showQueue = new ConcurrentQueue<IFrameVideo>();
        private readonly ConcurrentQueue<IFrameVideo> _updateQueue = new ConcurrentQueue<IFrameVideo>();
        private IFrameVideo _lastVideoData;
        private int[] _lastBuffer = new int[0];    // noflick


        private Size _frameSize;
        private SizeF _frameSizeNormalized;
        private int _textureStride;
        private int _textureMaskTvStride;

        private Sprite _sprite;
        private Sprite _spriteTv;
        private Texture _texture0;
        private Texture _textureMaskTv;

        #endregion Fields


        #region .ctor

        public VideoRenderer(AllocatorPresenter allocator)
            : base(allocator)
        {
            _updateQueue.Enqueue(new FrameVideo(1, 1, 1));
            _updateQueue.Enqueue(new FrameVideo(1, 1, 1));
            _updateQueue.Enqueue(new FrameVideo(1, 1, 1));
        }

        #endregion .ctor


        #region RenderBase

        protected override void LoadSynchronized()
        {
            base.LoadSynchronized();
            _sprite = new Sprite(Allocator.Device);
            _spriteTv = new Sprite(Allocator.Device);
        }

        protected override void UnloadSynchronized()
        {
            base.UnloadSynchronized();
            if (_texture0 != null)
            {
                _texture0.Dispose();
                _texture0 = null;
            }
            if (_textureMaskTv != null)
            {
                _textureMaskTv.Dispose();
                _textureMaskTv = null;
            }
            if (_sprite != null)
            {
                _sprite.Dispose();
                _sprite = null;
            }
            if (_spriteTv != null)
            {
                _spriteTv.Dispose();
                _spriteTv = null;
            }
        }

        protected override void RenderSynchronized(int width, int height)
        {
            base.RenderSynchronized(width, height);
            IFrameVideo videoData;
            if (_showQueue.TryDequeue(out videoData))
            {
                if (_lastVideoData != null)
                {
                    _updateQueue.Enqueue(_lastVideoData);
                }
                _lastVideoData = videoData;
            }
            videoData = _lastVideoData;
            if (videoData == null)
            {
                return;
            }
            if (_frameSize != videoData.Size || _texture0 == null)
            {
                UpdateTextureSize(videoData.Size);
            }
            _frameSizeNormalized = new SizeF(_frameSize.Width, _frameSize.Height * videoData.Ratio);
            UpdateTextureData(videoData);

            var size = new Size(width, height);
            var dstRect = ScaleHelper.GetDestinationRect(ScaleMode, size, _frameSizeNormalized);
            RenderSprite(_sprite, _texture0, _frameSize, dstRect, AntiAlias);
            if (MimicTv)
            {
                RenderSprite(_spriteTv, _textureMaskTv, new Size(_frameSize.Width, (int)(_frameSize.Height * MimicTvRatio+0.5F)), dstRect, true);
            }
        }

        #endregion RenderBase


        #region Public

        public bool AntiAlias { get; set; }
        public bool MimicTv { get; set; }
        public ScaleMode ScaleMode { get; set; }
        public VideoFilter VideoFilter { get; set; }

        public void Update(IFrameVideo videoData)
        {
            IFrameVideo clone;
            if (!_updateQueue.TryDequeue(out clone))
            {
                return;
            }
            if (clone.Size != videoData.Size ||
                clone.Ratio != videoData.Ratio)
            {
                clone = new FrameVideo(videoData.Size, videoData.Ratio);
            }
            Array.Copy(
                videoData.Buffer, 
                clone.Buffer, 
                clone.Buffer.Length);
            if (VideoFilter == VideoFilter.NoFlick)
            {
                FilterNoFlick(
                    clone.Buffer, 
                    clone.Size.Width, 
                    clone.Size.Height);
            }
            _showQueue.Enqueue(clone);
        }

        #endregion Public


        #region Private

        private void RenderSprite(
            Sprite sprite, 
            Texture texture,
            Size srcSize,
            RectangleF dstRect, 
            bool antiAlias)
        {
            var srcRect = new Rectangle(
                0, 
                0, 
                srcSize.Width, 
                srcSize.Height);
            sprite.Begin(SpriteFlags.None);
            try
            {
                if (!antiAlias)
                {
                    Allocator.Device.SetSamplerState(0, SamplerStageStates.MinFilter, (int)TextureFilter.Point);
                    Allocator.Device.SetSamplerState(0, SamplerStageStates.MagFilter, (int)TextureFilter.Point);
                    Allocator.Device.SetSamplerState(0, SamplerStageStates.MipFilter, (int)TextureFilter.Point);
                }
                sprite.Draw2D(
                   texture,
                   srcRect,
                   dstRect.Size,
                   dstRect.Location,
                   -1);
            }
            finally
            {
                sprite.End();
            }
        }

        private void UpdateTextureSize(Size size)
        {
            if (_texture0 != null)
            {
                _texture0.Dispose();
                _texture0 = null;
            }
            if (_textureMaskTv != null)
            {
                _textureMaskTv.Dispose();
                _textureMaskTv = null;
            }

            _frameSize = size;
            var maxSize = Math.Max(size.Width, size.Height);
            var potSize = ScaleHelper.GetPotSize(maxSize);
            _texture0 = new Texture(
                Allocator.Device,
                potSize,
                potSize,
                1,
                Usage.None,
                Format.X8R8G8B8,
                Pool.Managed);
            _textureStride = potSize;

            var maskSizeTv = new Size(size.Width, size.Height * MimicTvRatio);
            var maxSizeTv = Math.Max(maskSizeTv.Width, maskSizeTv.Height);
            var potSizeTv = ScaleHelper.GetPotSize(maxSizeTv);
            _textureMaskTv = new Texture(
                Allocator.Device,
                potSizeTv,
                potSizeTv,
                1,
                Usage.None, Format.A8R8G8B8, Pool.Managed);
            _textureMaskTvStride = potSizeTv;
            using (var gs = _textureMaskTv.LockRectangle(0, LockFlags.None))
            {
                var pixelColor = 0;
                var gapColor = MimicTvAlpha << 24;
                unsafe
                {
                    var pdst = (int*)gs.InternalData.ToPointer();
                    for (var y = 0; y < maskSizeTv.Height; y++)
                    {
                        pdst += potSizeTv;
                        var color = (y % MimicTvRatio) != (MimicTvRatio - 1) ? pixelColor : gapColor;
                        for (var x = 0; x < maskSizeTv.Width; x++)
                        {
                            pdst[x] = color;
                        }
                    }
                }
            }
            _textureMaskTv.UnlockRectangle(0);
        }

        private void UpdateTextureData(IFrameVideo videoData)
        {
            if (_texture0 == null)
            {
                return;
            }
            using (var gs = _texture0.LockRectangle(0, LockFlags.None))
            {
                unsafe
                {
                    fixed (int* srcPtr = videoData.Buffer)
                    {
                        NativeMethods.CopyStride(
                            (int*)gs.InternalData.ToPointer(),
                            srcPtr,
                            _frameSize.Width,
                            _frameSize.Height,
                            _textureStride);
                    }
                }
            }
            _texture0.UnlockRectangle(0);
        }

        private void FilterNoFlick(
            int[] buffer,
            int width,
            int height)
        {
            var size = height * width;
            if (_lastBuffer.Length < size)
            {
                _lastBuffer = new int[size];
            }
            unsafe
            {
                fixed (int* pSrcBuffer1 = buffer)
                fixed (int* pSrcBuffer2 = _lastBuffer)
                {
                    var pSrcArray1 = pSrcBuffer1;
                    var pSrcArray2 = pSrcBuffer2;
                    for (var y = 0; y < height; y++)
                    {
                        for (var i = 0; i < width; i++)
                        {
                            var src1 = pSrcArray1[i];
                            var src2 = pSrcArray2[i];
                            var r1 = (((src1 >> 16) & 0xFF) + ((src2 >> 16) & 0xFF)) / 2;
                            var g1 = (((src1 >> 8) & 0xFF) + ((src2 >> 8) & 0xFF)) / 2;
                            var b1 = (((src1 >> 0) & 0xFF) + ((src2 >> 0) & 0xFF)) / 2;
                            pSrcArray2[i] = src1;
                            pSrcArray1[i] = -16777216 | (r1 << 16) | (g1 << 8) | b1;
                        }
                        pSrcArray1 += width;
                        pSrcArray2 += width;
                    }
                }
            }
        }

        #endregion Private
    }
}
