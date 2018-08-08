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
        private const uint Polynome = 0xedb88320u;

        static readonly uint[] _table;

        static Crc32()
        {
            _table = new uint[256];
            uint temp = 0;
            for (uint i = 0; i < _table.Length; i++)
            {
                temp = i;
                for (int j = 8; j > 0; j--)
                {
                    temp = (temp & 1) == 0 ? temp >> 1 : (uint)((temp >> 1) ^ Polynome);
                }
                _table[i] = temp;
            }
        }

        public static unsafe uint Compute(byte* data, int length)
        {
            uint crc = 0xffffffffu;
            for (int i = 0; i < length; i++)
            {
                var index = (byte)(crc ^ data[i]);
                crc = (uint)((crc >> 8) ^ _table[index]);
            }
            return ~crc;
        }

        public static unsafe uint Compute(byte[] data, int offset, int length)
        {
            if (data.Length >= offset + length)
                throw new IndexOutOfRangeException();
            fixed (byte* pData = data)
            {
                return Compute(pData + offset, length);
            }
        }
    }
}
