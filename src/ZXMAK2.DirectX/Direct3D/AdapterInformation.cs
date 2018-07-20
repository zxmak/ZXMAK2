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
using System.Collections.Generic;


namespace ZXMAK2.DirectX.Direct3D
{
    public class AdapterInformation
    {
        private readonly Direct3D9 _direct3d;
        
        
        internal AdapterInformation(Direct3D9 direct3d, int adapter)
        {
            _direct3d = direct3d;
            this.Adapter = adapter;
            this.Details = direct3d.GetAdapterIdentifier(adapter, 0);
        }


        public int Adapter { get; private set; }
        public D3DADAPTER_IDENTIFIER9 Details { get; private set; }

        public IntPtr Monitor
        {
            get { return _direct3d.GetAdapterMonitor(this.Adapter); }
        }

        public D3DCAPS9 GetCaps(D3DDEVTYPE type)
        {
            var caps = new D3DCAPS9();
            var hr = _direct3d.GetDeviceCaps(this.Adapter, type, out caps);
            hr.CheckError();
            return caps;
        }

        public IList<D3DDISPLAYMODE> GetDisplayModes(D3DFORMAT format)
        {
            var modeCount = _direct3d.GetAdapterModeCount(this.Adapter, format);
            var list = new List<D3DDISPLAYMODE>(modeCount);
            for (var i = 0; i < modeCount; i++)
            {
                var mode = new D3DDISPLAYMODE();
                var hr = _direct3d.EnumAdapterModes(this.Adapter, format, i, out mode);
                hr.CheckError();
                list.Add(mode);
            }
            return list;
        }
    }
}
