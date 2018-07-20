using System;
using System.IO;


namespace ZXMAK2.Serializers.DiskSerializers
{
    public sealed class LzssHuffmanStream : Stream
    {
        #region Constants

        private const string _invalidCallString = "Not supported operation";

        #region LZSS Parameters

        private const int N = 4096;    /* Size of string buffer */
        private const int F = 60;    /* Size of look-ahead buffer */
        private const int THRESHOLD = 2;
        //private const int NIL = N;    /* End of tree's node  */

        #endregion

        #region Huffman coding parameters

        private const int N_CHAR = 256 - THRESHOLD + F; /* character code (= 0..N_CHAR-1) */
        private const int T = N_CHAR * 2 - 1;			/* Size of table */
        private const int R = T - 1;					/* root position */
        private const int MAX_FREQ = 0x8000;			/* update when cumulative frequency reaches to this value */

        #endregion
        
        #endregion Constants


        #region Fields
        
        private readonly Stream _input;

        // LZSS stuff?...
        private readonly byte[] text_buf = new byte[N + F - 1];
        //private short match_position, match_length;
        //private readonly short[] lson = new short[N + 1];
        //private readonly short[] rson = new short[N + 257];
        //private readonly short[] dad = new short[N + 1];

        // Huffman coding stuff?...
        private readonly ushort[] freq = new ushort[T + 1];    /* cumulative freq table */
        /*
         * pointing parent nodes.
         * area [T..(T + N_CHAR - 1)] are pointers for leaves
         */
        private readonly short[] prnt = new short[T + N_CHAR];
        /* pointing children nodes (son[], son[] + 1)*/
        private readonly short[] son = new short[T];


        // next four variables were local in original main()
        // we need to save these values between calls to Decode()
        public ushort _r;
        public ushort _bufcnt, _bufndx, _bufpos;	// string buffer

        #endregion Fields

        public LzssHuffmanStream(Stream stream)
        {
            _input = stream;
            InitDecode();
        }

        #region Stream

        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return false; } }
        public override bool CanWrite { get { return false; } }

        #region not supported methods
        public override long Length { get { throw new InvalidOperationException(_invalidCallString); } }

        public override long Position
        {
            get { throw new InvalidOperationException(_invalidCallString); }
            set { throw new InvalidOperationException(_invalidCallString); }
        }

        public override void Flush()
        {
            throw new InvalidOperationException(_invalidCallString);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new InvalidOperationException(_invalidCallString);
        }

        public override void SetLength(long value)
        {
            throw new InvalidOperationException(_invalidCallString);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException(_invalidCallString);
        }
        #endregion

        public override int Read(byte[] buffer, int offset, int length)
        {
            return Decode(buffer, offset, length);
        }

        #endregion


        #region Private

        private void InitDecode()
        {
            _bitCounter = 0;
            //_ibufcnt = _ibufndx = 0; // input buffer is empty
            _bufcnt = 0;
            StartHuff();
            for (var i = 0; i < N - F; i++)
            {
                text_buf[i] = 0x20;	// ' '
            }
            _r = N - F;
        }

        private void StartHuff()
        {
            var i = 0;
            for (; i < N_CHAR; i++)
            {
                freq[i] = 1;
                son[i] = (short)(i + T);
                prnt[i + T] = (short)i;
            }
            i = 0; 
            var j = N_CHAR;
            while (j <= R)
            {
                freq[j] = (ushort)(freq[i] + freq[i + 1]);
                son[j] = (short)i;
                prnt[i] = prnt[i + 1] = (short)j;
                i += 2; j++;
            }
            freq[T] = 0xffff;
            prnt[R] = 0;
        }

        private int Decode(byte[] buf, int startIndex, int len)	/* Decoding/Uncompressing */
        {
            short c, pos;
            int count;  // was an unsigned long, seems unnecessary
            for (count = 0; count < len; )
            {
                if (_bufcnt == 0)
                {
                    if ((c = DecodeChar(_input)) < 0)
                    {
                        // fatal error
                        return (count);
                    }
                    if (c < 256)
                    {
                        buf[startIndex++] = (byte)c;
                        text_buf[_r++] = (byte)c;
                        _r &= (N - 1);
                        count++;
                    }
                    else
                    {
                        if ((pos = DecodePosition(_input)) < 0)
                        {
                            // fatal error
                            return (count);
                        }
                        _bufpos = (ushort)((_r - pos - 1) & (N - 1));
                        _bufcnt = (ushort)(c - 255 + THRESHOLD);
                        _bufndx = 0;
                    }
                }
                else
                { // still chars from last string
                    while (_bufndx < _bufcnt && count < len)
                    {
                        c = text_buf[(_bufpos + _bufndx) & (N - 1)];
                        buf[startIndex++] = (byte)c;
                        _bufndx++;
                        text_buf[_r++] = (byte)c;
                        _r &= (N - 1);
                        count++;
                    }
                    // reset bufcnt after copy string from text_buf[]
                    if (_bufndx >= _bufcnt)
                        _bufndx = _bufcnt = 0;
                }
            }
            return count; // count == len, success
        }

        private short DecodeChar(Stream stream)
        {
            int ret;
            ushort c;

            c = (ushort)son[R];

            /*
             * start searching tree from the root to leaves.
             * choose node #(son[]) if input bit == 0
             * else choose #(son[]+1) (input bit == 1)
             */
            while (c < T)
            {
                if ((ret = GetBit(stream)) < 0)
                    return -1;
                c += (ushort)ret;
                c = (ushort)son[c];
            }
            c -= T;
            update(c);
            return (short)c;
        }

        private short DecodePosition(Stream stream)
        {
            short bit;
            ushort i, j, c;

            /* decode upper 6 bits from given table */
            if ((bit = (short)GetByte(stream)) < 0)
                return -1;
            i = (ushort)bit;
            c = (ushort)(s_dCode[i] << 6);
            j = s_dLen[i];

            /* input lower 6 bits directly */
            j -= 2;
            while (j-- != 0)
            {
                if ((bit = (short)GetBit(stream)) < 0)
                    return -1;
                i = (ushort)((i << 1) + bit);
            }
            return (short)(c | i & 0x3f);
        }

        private int _bitCounter = 0;
        private int _bitValue = 0;
        private int _bitMask = 0;

        private int GetBit(Stream stream)
        {
            if (_bitCounter < 1)
            {
                _bitValue = stream.ReadByte();
                if (_bitValue < 0) return -1;
                _bitMask = 0x80;
                _bitCounter = 8;
            }
            int value = _bitValue & _bitMask;
            _bitMask >>= 1;
            _bitCounter--;
            return (value != 0) ? 1 : 0;
        }

        private int GetByte(Stream stream)
        {
            int value = 0;
            for (int i = 0; i < 8; i++)
            {
                int bit = GetBit(stream);
                if (bit < 0) return -1;
                value = (value << 1) | bit;
            }
            return value;
        }

        /// <summary>update freq tree</summary>
        private void update(int c)
        {
            int i, j, k, l;

            if (freq[R] == MAX_FREQ)
            {
                reconst();
            }
            c = prnt[c + T];
            do
            {
                k = ++freq[c];

                /* swap nodes to keep the tree freq-ordered */
                if (k > freq[l = c + 1])
                {
                    // original: while (k > freq[++l]) ;
                    do
                    {
                        l++;
                    } while (k > freq[l]);

                    l--;
                    freq[c] = freq[l];
                    freq[l] = (ushort)k;

                    i = son[c];
                    prnt[i] = (short)l;
                    if (i < T) prnt[i + 1] = (short)l;

                    j = son[l];
                    son[l] = (short)i;

                    prnt[j] = (short)c;
                    if (j < T) prnt[j + 1] = (short)c;
                    son[c] = (short)j;

                    c = l;
                }
            } while ((c = prnt[c]) != 0);    /* do it until reaching the root */
        }

        /// <summary>reconstruct freq tree</summary>
        private void reconst()
        {
            short i, j, k;
            ushort f, l;
            /* halven cumulative freq for leaf nodes */
            j = 0;
            for (i = 0; i < T; i++)
            {
                if (son[i] >= T)
                {
                    freq[j] = (ushort)((freq[i] + 1) / 2);
                    son[j] = son[i];
                    j++;
                }
            }
            /* make a tree : first, connect children nodes */
            for (i = 0, j = N_CHAR; j < T; i += 2, j++)
            {
                k = (short)(i + 1);
                f = freq[j] = (ushort)(freq[i] + freq[k]);

                // original: for (k = (short)(j - 1); f < freq[k]; k--) ;
                k = (short)(j - 1);
                while (f < freq[k])
                {
                    k--;
                }

                k++;
                l = (ushort)((j - k) * 2);

                movmem(freq, k, freq, k + 1, l);//movmem(&freq[k], &freq[k + 1], l);
                freq[k] = f;
                movmem(son, k, son, k + 1, l);//movmem(&son[k], &son[k + 1], l);
                son[k] = i;
            }
            /* connect parent nodes */
            for (i = 0; i < T; i++)
            {
                if ((k = son[i]) >= T)
                {
                    prnt[k] = i;
                }
                else
                {
                    prnt[k] = prnt[k + 1] = i;
                }
            }
        }

        #endregion Private


        #region Helpers

        // Borland C++ style movmem()
        // void movmem(void *src, void *dest, unsigned length)
        private static void movmem(Array src, int srcIndex, Array dest, int destIndex, int length)
        {
            if (srcIndex > destIndex)
            {
                while (length-- > 0)
                {
                    dest.SetValue(src.GetValue(srcIndex++), destIndex++);
                }
            }
            else if (srcIndex < destIndex)
            {
                srcIndex += length;
                destIndex += length;
                while (length-- > 0)
                {
                    dest.SetValue(src.GetValue(--srcIndex), --destIndex);
                }
            }
        }

        #endregion Helpers


        #region Tables

        /*
		 * Tables for encoding/decoding upper 6 bits of
		 * sliding dictionary pointer
		 */
        private static readonly byte[] s_dCode = new byte[256] 
		{
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
			0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
			0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
			0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02,
			0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02, 0x02,
			0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03,
			0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03,
			0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04,
			0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05,
			0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06,
			0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07,
			0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
			0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09, 0x09,
			0x0A, 0x0A, 0x0A, 0x0A, 0x0A, 0x0A, 0x0A, 0x0A,
			0x0B, 0x0B, 0x0B, 0x0B, 0x0B, 0x0B, 0x0B, 0x0B,
			0x0C, 0x0C, 0x0C, 0x0C, 0x0D, 0x0D, 0x0D, 0x0D,
			0x0E, 0x0E, 0x0E, 0x0E, 0x0F, 0x0F, 0x0F, 0x0F,
			0x10, 0x10, 0x10, 0x10, 0x11, 0x11, 0x11, 0x11,
			0x12, 0x12, 0x12, 0x12, 0x13, 0x13, 0x13, 0x13,
			0x14, 0x14, 0x14, 0x14, 0x15, 0x15, 0x15, 0x15,
			0x16, 0x16, 0x16, 0x16, 0x17, 0x17, 0x17, 0x17,
			0x18, 0x18, 0x19, 0x19, 0x1A, 0x1A, 0x1B, 0x1B,
			0x1C, 0x1C, 0x1D, 0x1D, 0x1E, 0x1E, 0x1F, 0x1F,
			0x20, 0x20, 0x21, 0x21, 0x22, 0x22, 0x23, 0x23,
			0x24, 0x24, 0x25, 0x25, 0x26, 0x26, 0x27, 0x27,
			0x28, 0x28, 0x29, 0x29, 0x2A, 0x2A, 0x2B, 0x2B,
			0x2C, 0x2C, 0x2D, 0x2D, 0x2E, 0x2E, 0x2F, 0x2F,
			0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
			0x38, 0x39, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F,
		};

        private static readonly byte[] s_dLen = new byte[256] 
		{
			0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03,
			0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03,
			0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03,
			0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03, 0x03,
			0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04,
			0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04,
			0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04,
			0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04,
			0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04,
			0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04,
			0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05,
			0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05,
			0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05,
			0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05,
			0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05,
			0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05,
			0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05,
			0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05, 0x05,
			0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06,
			0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06,
			0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06,
			0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06,
			0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06,
			0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06, 0x06,
			0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07,
			0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07,
			0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07,
			0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07,
			0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07,
			0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07, 0x07,
			0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
			0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
		};

        #endregion Tables
    }
}
