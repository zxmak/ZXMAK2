using ZXMAK2.Crc;


namespace ZXMAK2.Model.Disk
{
    public abstract class Sector
    {
        private ushort _adCrc = 0xFFFF;
        private ushort _dataCrc = 0xFFFF;

        public abstract bool AdPresent { get; }
        public abstract bool DataPresent { get; }
        public virtual bool DataDeleteMark { get { return false; } }

        public abstract byte[] Data { get; }

        public abstract byte C { get; }
        public abstract byte H { get; }
        public abstract byte R { get; }
        public abstract byte N { get; }

        public virtual int AdSyncCount { get { return 3; } }
        public virtual int DataSyncCount { get { return 3; } }


        public void SetAdCrc(bool valid)
        {
            var adBlock = CreateAdBlock()[0];
            ushort crc = 0xFFFF;
            if (adBlock.Length > 0)
            {
                crc = CrcVg93.Update(crc, adBlock, 0, adBlock.Length - 2);
            }
            if (!valid)
            {
                crc = (ushort)(crc ^ 0xFFFF);
            }
            _adCrc = crc;
        }

        public void SetDataCrc(bool valid)
        {
            var dataBlock = CreateDataBlock()[0];
            ushort crc = 0xFFFF;
            if (dataBlock.Length > 0)
            {
                crc = CrcVg93.Update(crc, dataBlock, 0, dataBlock.Length - 2);
            }
            if (!valid)
            {
                crc = (ushort)(crc ^ 0xFFFF);
            }
            _dataCrc = crc;
        }

        public byte[][] CreateAdBlock()
        {
            if (!AdPresent)
            {
                return new[] { new byte[0], new byte[0] };
            }
            var ptr = 0;
            var data = new byte[2][];
            data[0] = new byte[GetAdBlockSize()];
            data[1] = new byte[data[0].Length / 8 + (((data[0].Length & 7) != 0) ? 1 : 0)];

            // synchropulse
            for (var i = 0; i < AdSyncCount; i++)
            {
                data[0][ptr] = 0xA1;
                data[1][ptr / 8] |= (byte)(1 << (ptr & 7));
                ptr++;
            }

            // address mark
            data[0][ptr] = 0xFE;
            data[1][ptr / 8] &= (byte)~(1 << (ptr & 7));//|= (byte)(1 << (ptr & 7));
            ptr++;

            // address data
            var adData = new byte[4] { C, H, R, N };
            for (var i = 0; i < 4; i++)
            {
                data[0][ptr] = adData[i];
                data[1][ptr / 8] &= (byte)~(1 << (ptr & 7));
                ptr++;
            }

            // crc (low part)
            data[0][ptr] = (byte)_adCrc;
            data[1][ptr / 8] &= (byte)~(1 << (ptr & 7));
            ptr++;

            // crc (high part)
            data[0][ptr] = (byte)(_adCrc >> 8);
            data[1][ptr / 8] &= (byte)~(1 << (ptr & 7));
            ptr++;
            return data;
        }

        public byte[][] CreateDataBlock()
        {
            if (!DataPresent)
            {
                return new[] { new byte[0], new byte[0] };
            }
            var ptr = 0;
            var data = new byte[2][];
            data[0] = new byte[GetDataBlockSize()];
            data[1] = new byte[data[0].Length / 8 + (((data[0].Length & 7) != 0) ? 1 : 0)];

            // synchropulse
            for (var i = 0; i < DataSyncCount; i++)
            {
                data[0][ptr] = 0xA1;
                data[1][ptr / 8] |= (byte)(1 << (ptr & 7));
                ptr++;
            }

            // Deleted/Normal data mark
            if (DataDeleteMark)
            {
                data[0][ptr] = 0xF8;
            }
            else
            {
                data[0][ptr] = 0xFB;
            }
            data[1][ptr / 8] &= (byte)~(1 << (ptr & 7));//|= (byte)(1 << (ptr & 7));
            ptr++;

            // data array
            for (var i = 0; i < Data.Length; i++)
            {
                data[0][ptr] = Data[i];
                data[1][ptr / 8] &= (byte)~(1 << (ptr & 7));
                ptr++;
            }

            // crc (low part)
            data[0][ptr] = (byte)_dataCrc;
            data[1][ptr / 8] &= (byte)~(1 << (ptr & 7));
            ptr++;

            // crc (high part)
            data[0][ptr] = (byte)(_dataCrc >> 8);
            data[1][ptr / 8] &= (byte)~(1 << (ptr & 7));
            ptr++;
            return data;
        }

        public int GetAdBlockSize()
        {
            if (AdPresent)
            {
                return AdSyncCount + 4 + 3;
            }
            return 0;
        }

        public int GetDataBlockSize()
        {
            if (DataPresent)
            {
                return DataSyncCount + Data.Length + 3;
            }
            return 0;
        }
    }
}
