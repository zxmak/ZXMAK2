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
    /// Flags that refine the scope of the enumeration used by DirectInput8W.GetDevices(DeviceClass,DeviceEnumerationFlags)
    /// and DirectInput8W.GetDevices(DeviceType,DeviceEnumerationFlags)" methods.
    /// </summary>	
    [Flags]
    public enum DIEDFL
    {
        /// <summary>	
        /// All installed devices are enumerated. This is the default behavior.
        /// </summary>	
        /// <unmanaged>DIEDFL_ALLDEVICES</unmanaged>	
        ALLDEVICES = 0,
        /// <summary>	
        /// Only attached and installed devices.
        /// </summary>	
        /// <unmanaged>DIEDFL_ATTACHEDONLY</unmanaged>	
        ATTACHEDONLY = 1,
        /// <summary>	
        /// Only devices that support force feedback.
        /// </summary>	
        /// <unmanaged>DIEDFL_FORCEFEEDBACK</unmanaged>	
        FORCEFEEDBACK = 256,
        /// <summary>	
        /// Include devices that are aliases for other devices. 
        /// </summary>	
        /// <unmanaged>DIEDFL_INCLUDEALIASES</unmanaged>	
        INCLUDEALIASES = 65536,
        /// <summary>	
        /// Include hidden devices. For more information about hidden devices, see <see cref="T:SharpDX.DirectInput.Capabilities" />.
        /// </summary>	
        /// <unmanaged>DIEDFL_INCLUDEPHANTOMS</unmanaged>	
        INCLUDEPHANTOMS = 131072,
        /// <summary>	
        /// Include phantom (placeholder) devices.
        /// </summary>	
        /// <unmanaged>DIEDFL_INCLUDEHIDDEN</unmanaged>	
        INCLUDEHIDDEN = 262144,
    }
}
