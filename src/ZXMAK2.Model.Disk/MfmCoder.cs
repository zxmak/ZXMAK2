using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ZXMAK2.Model.Disk
{
    public class MfmCoder
    {
        private readonly BitArray _data;
        private int _pos = 0;

        public MfmCoder(byte[] data)
        {
            _data = new BitArray(data);
        }

        public byte ReadBit()
        {
            if (_pos >= _data.Length)
                return 0;

            return (byte)(_data[_pos++] ? 1 : 0);
        }

        public int Find(byte[] pattern)
        {
            var window = BitsToBytes(_data, _pos, pattern.Length, true).ToArray();
            var srcPos = _pos + pattern.Length * 8;

            bool found = false;
            while (!(found = pattern.SequenceEqual(window)) && srcPos != _data.Length)
            {
                var msb = (byte)(_data[srcPos++] ? 1 : 0);
                for (var i = pattern.Length - 1; i >= 0; i--)
                {
                    var newMsb = (byte)((window[i] & 0x80) >> 7);
                    window[i] <<= 1;
                    window[i] |= msb;
                    msb = newMsb;
                }
            }

            if (found)
                return srcPos - pattern.Length * 8;

            return -1;
        }
        
        private static IEnumerable<byte> BitsToBytes(BitArray bits, int startPos = 0, int byteCount = -1, bool msb = false)
        {
            int bitCount = 7;
            int outByte = 0;

            var maxBit = byteCount > 0 ? (startPos + 8 * byteCount) : bits.Length;
            for (int i = startPos; i < maxBit; i++)
            {
                if (bits[i])
                    outByte |= msb ? 1 << bitCount : 1 << (7 - bitCount);
                if (bitCount == 0)
                {
                    yield return (byte) outByte;
                    bitCount = 8;
                    outByte = 0;
                }
                bitCount--;
            }
            // Last partially decoded byte
            if (bitCount < 7)
                yield return (byte) outByte;
        }

        public Tuple<byte[], byte[]> SplitDataAndClock()
        {
            var length = Convert.ToInt32((_data.Length - _pos) / 2f);
            var data = new BitArray(length);
            var clock = new BitArray(length);

            var outputPos = 0;
            for (int i = _pos; i + 1 < _data.Length; i+=2)
            {
                clock[outputPos] = _data[i];
                data[outputPos] = _data[i + 1];
                outputPos++;
            }

            var bytesCount = Convert.ToInt32(Math.Ceiling(data.Length / 8f));
            var dataBytes = BitsToBytes(data, 0, bytesCount, true).ToArray();
            var clockBytes = BitsToBytes(clock, 0, bytesCount, true).ToArray();

            return new Tuple<byte[], byte[]>(dataBytes, clockBytes);
        }

        public static byte[] CombineDataAndClock(byte[] data, byte[] clock)
        {
            var dataArray = new BitArray(data);
            var clockArray = new BitArray(clock);
            var result = new BitArray(dataArray.Length * 2);

            for (int i = 0; i < dataArray.Length; i++)
            {
                var pos = (i & ~7) | (7 - (i & 7));
                result[2 * i] = clockArray[pos];
                result[2 * i + 1] = dataArray[pos];
            }

            return BitsToBytes(result).ToArray();
        }
        
        public static byte[] GenerateMfmClock(byte[] data)
        {
            var clock = new byte[data.Length];
            var dataBits = new BitArray(data);
            var clockBits = new BitArray(clock);

            var prev = false;
            for (int i = 0; i < dataBits.Length; i++)
            {
                // Since first bit in array is 0-th bit of first byte
                var pos = (i & ~7) | (7 - (i & 7));
                
                var dataBit = dataBits[pos];
                clockBits[pos] = !dataBit && !prev;
                prev = dataBit;
            }

            return BitsToBytes(clockBits).ToArray();
        }
        
        public static byte[] ReduceClockArray(byte[] data, byte[] clock)
        {
            // Calculate correct MFM clock for a data bytes
            var normalMfm = GenerateMfmClock(data);

            var result = new BitArray(data.Length);

            // And compare it with what we have. Difference will be at C2/A1 marks
            for (int i = 0; i < data.Length; i++)
                result[i] = clock[i] != normalMfm[i];

            return BitsToBytes(result).ToArray();
        }

        public static byte[] ExpandClockArray(byte[] data, byte[] clock)
        {
            var resultClock = GenerateMfmClock(data);
            Func<int, bool> rawTestClock = pos => (clock[pos / 8] & (1 << (pos & 7))) != 0;
            for (int i = 0; i < data.Length; i++)
            {
                if ((data[i] == 0xA1 || data[i] == 0xC2) && rawTestClock(i))
                {
                    if (data[i] == 0xA1) // fix clock for "special" A1
                        resultClock[i] ^= (1 << 2);
                    else // ...or for C2
                        resultClock[i] ^= (1 << 3);
                }
            }

            return resultClock;
        }
    }
}