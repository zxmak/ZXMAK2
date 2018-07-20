using System;


namespace ZXMAK2.Crc
{
    public sealed class CrcVg93
    {
        public static readonly UInt16 InitValue = 0xFFFF;

        // TODO: check why used by FdiSerializer (because current engine uses precalc 3xA1)
        public static UInt16 Calculate(
            byte[] data,
            int startIndex,
            int size)
        {
            // 0xFFFF - init value
            return Update(InitValue, data, startIndex, size);
        }

        public static UInt16 Calc3xA1(
            byte[] data,
            int startIndex,
            int size)
        {
            // 0xCDB4 - precalculated value for 0xA1,0xA1,0xA1
            return Update(0xCDB4, data, startIndex, size);
        }

        public static UInt16 Update(
            ushort value,
            byte[] buffer,
            int startIndex,
            int length)
        {
            int crc = value;
            while (length-- > 0)
            {
                crc ^= buffer[startIndex++] << 8;
                for (int j = 8; j != 0; j--) // todo: rewrite with pre-calc'ed table
                {
                    if (((crc *= 2) & 0x10000) != 0) crc ^= 0x1021; // bit representation of x^12+x^5+1
                }
            }
            crc = ((crc & 0xFF00) >> 8) | ((crc & 0x00FF) << 8);
            return (UInt16)crc;
        }
    }
}
