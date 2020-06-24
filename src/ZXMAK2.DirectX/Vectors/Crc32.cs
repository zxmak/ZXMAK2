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
 *  Date: 08.08.2018
 */
using System;


namespace ZXMAK2.DirectX.Vectors
{
    // Helper for entity hash calculation
    internal class Crc32
    {
        static readonly uint[] _table = new uint[256];

        static Crc32()
        {
            for (uint i = 0; i < _table.Length; i++)
            {
                uint temp = i;
                for (int j = 8; j > 0; j--)
                {
                    uint msk = 0 - (temp & 1);
                    temp >>= 1;
                    temp ^= msk & 0xedb88320u;
                }
                _table[i] = temp;
            }
        }

        public static unsafe uint Compute(byte* data, int length)
        {
            uint crc = 0xffffffffu;
            for (int i = 0; i < length; i++)
            {
                crc = (crc >> 8) ^ _table[(byte)(crc ^ data[i])];
            }
            return ~crc;
        }

        public static unsafe uint Compute(byte[] data, int offset, int length)
        {
            if (data.Length > offset + length)
                throw new IndexOutOfRangeException();
            fixed (byte* pData = data)
            {
                return Compute(pData + offset, length);
            }
        }
    }
}
