using System;


namespace ZXMAK2.Crc
{
    public sealed class CrcUdi
    {
        private const UInt32 Polynom = 0xEDB88320;

        public static readonly UInt32 InitValue = 0xFFFFFFFF;

        public static UInt32 Calculate(
            byte[] buffer,
            int startIndex,
            int size)
        {
            return Update(InitValue, buffer, startIndex, size);
        }

        public static UInt32 Update(
            UInt32 value,
            byte[] buffer,
            int startIndex,
            int size)
        {
            var crc = (Int32)value;
            for (var i = 0; i < size; i++)
            {
                crc ^= -1 ^ buffer[startIndex + i];
                for (int k = 8; k-- != 0; )
                {
                    var temp = -(crc & 1);
                    crc >>= 1;
                    crc ^= temp & unchecked((int)Polynom);
                }
                crc ^= -1;
            }
            return (UInt32)crc;
        }
    }
}
