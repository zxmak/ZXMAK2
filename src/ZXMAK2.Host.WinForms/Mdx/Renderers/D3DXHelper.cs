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
 *  Description: DirectX helper
 *  Date: 15.07.2018
 */
using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Drawing;
using ZXMAK2.DirectX.Direct3D;
using ZXMAK2.DirectX.Vectors;


namespace ZXMAK2.Host.WinForms.Mdx.Renderers
{
    public class D3DXHelper
    {
        public static uint GetColor(Color color)
        {
            return (uint)color.ToArgb();
        }

        public static Rectangle GetRect(D3DRECT rect)
        {
            return new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        public static D3DRECT GetRawRect(Rectangle rect)
        {
            return new D3DRECT(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }

        public unsafe static void Draw2D(
            D3DXSprite sprite,
            Direct3DTexture9 texture,
            RectangleF dstRect,
            Size srcSize)
        {
            var scale = new D3DXVECTOR2(
                dstRect.Width / (float)srcSize.Width,
                dstRect.Height / (float)srcSize.Height);
            var trans = new D3DXVECTOR2(dstRect.Location.X, dstRect.Location.Y);
            D3DMATRIX m;
            D3DMATRIX.Transformation2D(&m, null, 0f, &scale, null, 0f, &trans);
            sprite.SetTransform(ref m).CheckError();
            sprite.Draw(texture, null, null, null, 0xffffffff).CheckError();
        }


        
        public static D3DXFont CreateFont(Direct3DDevice9 device, Font gdiFont)
        {
            var dc = GetDC(IntPtr.Zero);
            var logPixelsY = GetDeviceCaps(dc, LOGPIXELSY);
            ReleaseDC(IntPtr.Zero, dc);
            var height = (int)((double)gdiFont.Size * (double)logPixelsY / -72d);
            return D3DX9.CreateFont(
                device,
                height,                     // height
                0,                          // width
                gdiFont.Bold ? 700 : 400,   // weight
                0,                          // mipLevels
                gdiFont.Italic,             // italic
                1,                          // charSet
                0,                          // outputPrecision
                0,                          // quality
                0,                          // pitchAndFamily
                gdiFont.Name);              // faceName
        }
        
        
        
        private const int LOGPIXELSX = 88;    /* Logical pixels/inch in X                 */
        private const int LOGPIXELSY = 90;    /* Logical pixels/inch in Y                 */
        
        //int GetDeviceCaps(HDC hdc, int index);
        [SuppressUnmanagedCodeSecurity]
        [DllImport("gdi32")]
        private static extern int GetDeviceCaps(IntPtr hdc, int index);
        
        //HDC GetDC(HWND hWnd);
        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        //int ReleaseDC(HWND hWnd, HDC  hDC);
        [SuppressUnmanagedCodeSecurity]
        [DllImport("user32")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    }
}
