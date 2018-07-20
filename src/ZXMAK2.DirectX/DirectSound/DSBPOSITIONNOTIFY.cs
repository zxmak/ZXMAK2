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
 *  Date: 10.07.2018
 */
using System;
using System.Threading;


namespace ZXMAK2.DirectX.DirectSound
{
    // <unmanaged>DSBPOSITIONNOTIFY</unmanaged>	
    public class DSBPOSITIONNOTIFY
    {
        internal struct __Native
        {
            // <unmanaged>unsigned int dwOffset</unmanaged>	
            public int dwOffset;

            // <unmanaged>void* hEventNotify</unmanaged>	
            public IntPtr hEventNotify;

            internal void __MarshalFree()
            {
            }
        }

        public int dwOffset;

        public WaitHandle WaitHandle
        {
            get;
            set;
        }

        internal void __MarshalFree(ref DSBPOSITIONNOTIFY.__Native @ref)
        {
            @ref.__MarshalFree();
        }

        internal void __MarshalTo(ref DSBPOSITIONNOTIFY.__Native @ref)
        {
            @ref.dwOffset = this.dwOffset;
            @ref.hEventNotify = this.WaitHandle.SafeWaitHandle.DangerousGetHandle();
        }
    }
}
