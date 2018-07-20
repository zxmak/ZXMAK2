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
using System.Linq;
using System.Runtime.InteropServices;
using ZXMAK2.DirectX.Native;


namespace ZXMAK2.DirectX
{
    public class DirectXException : Exception
    {
        public HRESULT ErrorCode { get; private set; }

        public DirectXException(HRESULT hr)
            : base(GetErrorCodeMessage(hr))
        {
            ErrorCode = hr;
        }

        public DirectXException(HRESULT hr, Exception innerException)
            : base(GetErrorCodeMessage(hr), innerException)
        {
            ErrorCode = hr;
        }

        
        public static string GetErrorCodeMessage(HRESULT hr)
        {
            var codeName = GetErrorCodeName(hr);
            codeName = codeName ?? string.Empty;
            if (codeName.Length > 0)
                codeName = "[" + codeName + "]";
            var message = hr.GetDescription();
            message = message ?? string.Empty;
            if (message.Length > 0)
                message = " " + message;
            return string.Format("[0x{0:X8}]{1}{2}", (uint)hr, codeName, message);
        }

        public static string GetErrorCodeName(HRESULT hr)
        {
            var value = (ErrorCode)hr;
            if (!Enum.IsDefined(typeof(ErrorCode), value))
                return null;
            return Convert.ToString(value);
        }
    }
}
