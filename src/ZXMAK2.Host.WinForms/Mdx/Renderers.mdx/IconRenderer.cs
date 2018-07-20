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
using System.Drawing;
using System.Collections.Generic;
using Microsoft.DirectX.Direct3D;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.WinForms.Tools;

namespace ZXMAK2.Host.WinForms.Mdx.Renderers
{
    public class IconRenderer : RendererBase
    {
        #region Fields

        private readonly Dictionary<IIconDescriptor, IconTextureWrapper> _iconTextures = new Dictionary<IIconDescriptor, IconTextureWrapper>();
        private Sprite _spriteIcon;

        #endregion Fields


        #region .ctor

        public IconRenderer(AllocatorPresenter allocator)
            : base(allocator)
        {
        }

        #endregion .ctor


        #region RendererBase

        protected override void LoadSynchronized()
        {
            base.LoadSynchronized();
            _spriteIcon = new Sprite(Allocator.Device);
        }

        protected override void UnloadSynchronized()
        {
            base.UnloadSynchronized();
            if (_spriteIcon != null)
            {
                _spriteIcon.Dispose();
                _spriteIcon = null;
            }
            foreach (var icon in _iconTextures.Values)
            {
                icon.Dispose();
            }
        }

        protected override void RenderSynchronized(int width, int height)
        {
            var size = new Size(width, height);
            var visibleIcons = _iconTextures.Values
                .Where(icon => icon.Visible);
            foreach (var icon in visibleIcons)
            {
                icon.LoadResources(Allocator.Device);
            }
            var potSize = ScaleHelper.GetPotSize(32);
            var iconSize = new SizeF(potSize, potSize);
            var iconNumber = 1;
            foreach (var iconTexture in visibleIcons)
            {
                var iconRect = new Rectangle(new Point(0, 0), iconTexture.Size);
                var iconPos = new PointF(
                    size.Width - iconSize.Width * iconNumber,
                    0);
                _spriteIcon.Begin(SpriteFlags.AlphaBlend);
                try
                {
                    _spriteIcon.Draw2D(
                       iconTexture.Texture,
                       iconRect,
                       iconSize,
                       iconPos,
                       -1);
                }
                finally
                {
                    _spriteIcon.End();
                }
                iconNumber++;
            }
        }

        #endregion RendererBase


        #region Public

        public void Update(IIconDescriptor[] icons)
        {
            var nonUsed = _iconTextures.Keys.ToList();
            foreach (var id in icons)
            {
                var iconTexture = default(IconTextureWrapper);
                if (!_iconTextures.ContainsKey(id))
                {
                    iconTexture = new IconTextureWrapper(id);
                    Allocator.ExecuteSynchronized(() => _iconTextures.Add(id, iconTexture));
                }
                else
                {
                    iconTexture = _iconTextures[id];
                }
                iconTexture.Visible = id.Visible;
                nonUsed.Remove(id);
            }
            if (nonUsed.Count > 0)
            {
                Allocator.ExecuteSynchronized(() =>
                {
                    foreach (var id in nonUsed)
                    {
                        _iconTextures[id].Dispose();
                        _iconTextures.Remove(id);
                    }
                });
            }
        }

        #endregion Public


        #region TextureWrapper

        private class IconTextureWrapper : IDisposable
        {
            private IIconDescriptor m_iconDesc;

            public Texture Texture;
            public bool Visible;

            public IconTextureWrapper(IIconDescriptor iconDesc)
            {
                m_iconDesc = iconDesc;
                Visible = iconDesc.Visible;
            }

            public void Dispose()
            {
                UnloadResources();
            }

            public Size Size
            {
                get { return m_iconDesc.Size; }
            }

            public void LoadResources(Device device)
            {
                if (device == null || Texture != null)
                {
                    return;
                }
                using (var stream = m_iconDesc.GetImageStream())
                {
                    Texture = TextureLoader.FromStream(device, stream);
                }
            }

            public void UnloadResources()
            {
                var texture = Texture;
                Texture = null;
                if (texture != null)
                {
                    texture.Dispose();
                }
            }
        }

        #endregion TextureWrapper
    }
}
