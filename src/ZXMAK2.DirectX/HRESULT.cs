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
using System.Diagnostics;
using System.Runtime.InteropServices;
using ZXMAK2.DirectX.Native;


namespace ZXMAK2.DirectX
{
    [StructLayout(LayoutKind.Sequential)]
    public struct HRESULT : IEquatable<HRESULT>
    {
        private readonly int Code;


        public HRESULT(int code)
        {
            Code = code;
        }

        public HRESULT(uint code)
        {
            Code = unchecked((int)code);
        }

        public bool IsSuccess
        {
            get { return Code >= 0; }
        }

        public bool IsFailure
        {
            get { return Code < 0; }
        }

        public string GetName()
        {
            var value = (ErrorCode)Code;
            if (!Enum.IsDefined(typeof(ErrorCode), value))
                return null;
            return Convert.ToString(value);
        }

        public string GetDescription()
        {
            var lpBuffer = IntPtr.Zero;
            NativeMethods.FormatMessageW(
                4864,
                IntPtr.Zero,
                (int)Code,
                0,
                ref lpBuffer,
                0,
                IntPtr.Zero);
            var result = Marshal.PtrToStringUni(lpBuffer);
            Marshal.FreeHGlobal(lpBuffer);
            return result;
        }

        [DebuggerStepThrough]
        public void CheckError()
        {
            if (IsSuccess)
                return;
            throw new DirectXException(this);
        }

        public override string ToString()
        {
            return string.Format("[HRESULT] 0x{0:X8} {1}", Code, GetName());
        }

        public override bool Equals(object obj)
        {
            return (obj is HRESULT) && this.Equals((HRESULT)this);
        }

        public bool Equals(HRESULT other)
        {
            return Code == other.Code;
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        public static bool operator ==(HRESULT hr1, HRESULT hr2)
        {
            return hr1.Equals(hr2);
        }

        public static bool operator !=(HRESULT hr1, HRESULT hr2)
        {
            return !hr1.Equals(hr2);
        }


        public static explicit operator int(HRESULT result)
        {
            return result.Code;
        }

        public static explicit operator uint(HRESULT result)
        {
            return unchecked((uint)result.Code);
        }

        public static explicit operator ErrorCode(HRESULT result)
        {
            return (ErrorCode)unchecked((uint)result.Code);
        }

        public static implicit operator HRESULT(int result)
        {
            return new HRESULT(result);
        }

        public static implicit operator HRESULT(uint result)
        {
            return new HRESULT(result);
        }

        public static implicit operator HRESULT(ErrorCode result)
        {
            return new HRESULT((uint)result);
        }
    }
}
