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
    // winuser.h
    public enum DT
    {
        DT_TOP                      = 0x00000000,
        DT_LEFT                     = 0x00000000,
        DT_CENTER                   = 0x00000001,
        DT_RIGHT                    = 0x00000002,
        DT_VCENTER                  = 0x00000004,
        DT_BOTTOM                   = 0x00000008,
        DT_WORDBREAK                = 0x00000010,
        DT_SINGLELINE               = 0x00000020,
        DT_EXPANDTABS               = 0x00000040,
        //DT_TABSTOP                  = 0x00000080,
        DT_NOCLIP                   = 0x00000100,
        //DT_EXTERNALLEADING          = 0x00000200,
        DT_CALCRECT                 = 0x00000400,
        //DT_NOPREFIX                 = 0x00000800,
        //DT_INTERNAL                 = 0x00001000,
    
        DT_RTLREADING               = 0x00020000,
    }
}
