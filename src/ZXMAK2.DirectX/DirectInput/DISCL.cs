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


namespace ZXMAK2.DirectX.DirectInput
{
    /// <summary>	
    /// Flags that specify the cooperative level to associate with the input device. This flag is used by <see cref="!:Device.SetCooperativeLevel(System.Windows.Forms.Control,SharpDX.DirectInput.CooperativeLevel)" /> method.
    /// </summary>	
    /// <unmanaged>DISCL</unmanaged>	
    [Flags]
    public enum DISCL
    {
        /// <summary>	
        /// The application is requesting an exclusive access to the device. If the exclusive access is authorized, no other instance of the device can get an exclusive access to the device while it is acquired. Note that non-exclusive access to the input device is always authorized, even when another application has an exclusive access.  In exclusive mode, an application that acquires the mouse or keyboard device must unacquire the input device when it receives a windows event message WM_ENTERSIZEMOVE or WM_ENTERMENULOOP. Otherwise, the user won't be able to access to the menu or move and resize the window.
        /// </summary>	
        /// <unmanaged>DISCL_EXCLUSIVE</unmanaged>	
        EXCLUSIVE = 1,
        /// <summary>	
        /// The application is requesting a non-exclusive access to the device. There is no interference even if another application is using the same device. 
        /// </summary>	
        /// <unmanaged>DISCL_NONEXCLUSIVE</unmanaged>	
        NONEXCLUSIVE = 2,
        /// <summary>	
        /// The application is requesting a foreground access to the device. If the foreground access is authorized and the associated window moves to the background, the device is automatically unacquired.
        /// </summary>	
        /// <unmanaged>DISCL_FOREGROUND</unmanaged>	
        FOREGROUND = 4,
        /// <summary>	
        /// The application is requesting a background access to the device. If background access is authorized, the device can be acquired even when the associated window is not the active window.
        /// </summary>	
        /// <unmanaged>DISCL_BACKGROUND</unmanaged>	
        BACKGROUND = 8,
        /// <summary>	
        /// The application is requesting to disable the Windows logo key effect. When this flag is set, the user cannot perturbate the application. However, when the default action mapping UI is displayed, the Windows logo key is operating as long as that UI is opened. Consequently, this flag has no effect in this situation.
        /// </summary>	
        /// <unmanaged>DISCL_NOWINKEY</unmanaged>	
        NOWINKEY = 16
    }
}
