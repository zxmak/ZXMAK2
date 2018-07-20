/// Description: Disk track image emulation
/// Author: Alex Makeev
/// Date: 13.04.2007
using System;
using System.Collections.Generic;
using ZXMAK2.Crc;


namespace ZXMAK2.Model.Disk
{
    public class Track
    {
        private readonly long _trackTime; // Z80FQ * 60 / RPM; RPM=300, Z80FQ=3500000
        private long _byteTime;
        private byte[] _trackImage;
        private byte[] _trackClock;
        private readonly List<SectorHeader> _headerList = new List<SectorHeader>();


        public Track(long trackTime)
        {
            _trackTime = trackTime;
            _byteTime = trackTime / 6400;
            if (_byteTime < 1)
            {
                _byteTime = 1;
            }
        }

        public bool sf = false;  // temp for compatibility = need refresh

        public void RefreshHeaders()
        {
            _headerList.Clear();
            if (_trackImage == null)
            {
                return;
            }
            for (var i = 0; i < _trackImage.Length - 8; i++)
            {
                if (_trackImage[i] != 0xA1 ||
                    _trackImage[i + 1] != 0xFE ||
                    !RawTestClock(i))
                {
                    continue;
                }

                var h = new SectorHeader();
                _headerList.Add(h);
                h.idOffset = i + 2;
                h.idTime = h.idOffset * _byteTime;
                h.c = _trackImage[h.idOffset + 0];
                h.s = _trackImage[h.idOffset + 1];
                h.n = _trackImage[h.idOffset + 2];
                h.l = _trackImage[h.idOffset + 3];
                h.crc1 = (ushort)(_trackImage[i + 6] | (_trackImage[i + 7] << 8));
                h.c1 = MakeCrc(i + 1, 5) == h.crc1;
                h.dataOffset = -1; // temp not found
                h.datlen = 0;
                if (h.l > 5)
                {
                    continue;
                }

                var end = _trackImage.Length - 8;  // = min((int)(trklen - 8), i + 8 + 43); // 43-DD, 30-SD
                for (var j = i + 8; j < end; j++)
                {
                    if (_trackImage[j] != 0xA1 ||
                        !RawTestClock(j) ||
                        RawTestClock(j + 1))
                    {
                        continue;
                    }

                    if (_trackImage[j + 1] == 0xF8 || _trackImage[j + 1] == 0xFB)
                    {
                        h.datlen = 128 << h.l;
                        h.dataOffset = j + 2;
                        h.dataTime = h.dataOffset * _byteTime;
                        if ((h.dataOffset + h.datlen + 2) > _trackImage.Length)
                        {
                            h.datlen = _trackImage.Length - h.dataOffset;
                            h.crc2 = (ushort)(MakeCrc(h.dataOffset - 1, h.datlen + 1) ^ 0xFFFF);
                            h.c2 = false;
                        }
                        else
                        {
                            h.crc2 = (ushort)(_trackImage[h.dataOffset + h.datlen] | (_trackImage[h.dataOffset + h.datlen + 1] << 8));
                            h.c2 = MakeCrc(h.dataOffset - 1, h.datlen + 1) == h.crc2;
                        }
                    }
                    break;
                }
            }
        }

        public void AssignImage(byte[] trackImage, byte[] trackClock)
        {
            if (trackImage.Length <= 0 ||
               (trackImage.Length / 8 + (((trackImage.Length & 7) != 0) ? 1 : 0)) != trackClock.Length)
            {
                throw new InvalidOperationException("Invalid track image length!");
            }

            _trackImage = trackImage;
            _trackClock = trackClock;
            _byteTime = _trackTime / trackImage.Length;
            if (_byteTime < 1)
                _byteTime = 1;

            RefreshHeaders();
        }

        /// <summary>
        /// Build default sectoring track image and assign it
        /// </summary>
        public void AssignSectors(List<Sector> sectorList)
        {
            var trackImage = new byte[2][];

            #region Calculate required track size...

            var imageSize = 6250;		// recommended track size

            var secCount = sectorList.Count;
            var trkdatalen = 0;
            foreach (var s in sectorList)
            {
                trkdatalen += s.GetAdBlockSize();
                trkdatalen += s.GetDataBlockSize();
            }

            var freeSpace = imageSize - (trkdatalen + secCount * (3 + 2));  // 3x4E & 2x00 per sector
            var firstGapLen = 1;
            var secondGapLen = 1;
            var thirdGapLen = 1;
            var synchroPauseLen = 1;

            freeSpace -= firstGapLen + secondGapLen + thirdGapLen + synchroPauseLen;
            if (freeSpace < 0)
            {
                imageSize += -freeSpace;	// expand track size
                freeSpace = 0;
            }

            // distribute gaps len and synchropauses length
            while (freeSpace > 0)
            {
                if (freeSpace >= (secCount * 2)) // Synchro for ADMARK & DATA
                {
                    if (synchroPauseLen < 12)
                    {
                        synchroPauseLen++;
                        freeSpace -= secCount * 2;
                    }
                }
                if (freeSpace < secCount)
                {
                    break;
                }

                if (firstGapLen < 10) { firstGapLen++; freeSpace -= secCount; }
                if (freeSpace < secCount) break;
                if (secondGapLen < 22) { secondGapLen++; freeSpace -= secCount; }
                if (freeSpace < secCount) break;
                if (thirdGapLen < 60) { thirdGapLen++; freeSpace -= secCount; }
                if (freeSpace < secCount) break;

                if ((synchroPauseLen >= 12) && (firstGapLen >= 10) &&
                    (secondGapLen >= 22) && (thirdGapLen >= 60))
                {
                    break;
                }
            }

            if (freeSpace < 0)
            {
                imageSize += -freeSpace;	// small expand track size
                freeSpace = 0;
            }

            #endregion

            #region Format track...
            trackImage[0] = new byte[imageSize];
            trackImage[1] = new byte[trackImage[0].Length / 8 + (((trackImage[0].Length & 7) != 0) ? 1 : 0)];

            var tptr = 0;
            foreach (var sector in sectorList)
            {
                for (var r = 0; r < firstGapLen; r++)			// First gap
                {
                    trackImage[0][tptr] = 0x4E;
                    trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                    tptr++;
                }
                for (var r = 0; r < synchroPauseLen; r++)       // Synchropause
                {
                    trackImage[0][tptr] = 0x00;
                    trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                    tptr++;
                }
                if (sector.AdPresent)						// address block
                {
                    var block = sector.CreateAdBlock();
                    for (var r = 0; r < sector.GetAdBlockSize(); r++)
                    {
                        trackImage[0][tptr] = block[0][r];
                        if ((block[1][r / 8] & (1 << (r & 7))) != 0)
                            trackImage[1][tptr / 8] |= (byte)(1 << (tptr & 7));
                        else
                            trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                        tptr++;
                    }
                }
                for (var r = 0; r < secondGapLen; r++)		// Second gap
                {
                    trackImage[0][tptr] = 0x4E;
                    trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                    tptr++;
                }
                for (var r = 0; r < synchroPauseLen; r++)		// Synchropause
                {
                    trackImage[0][tptr] = 0x00;
                    trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                    tptr++;
                }
                if (sector.DataPresent)						// data array block
                {
                    var block = sector.CreateDataBlock();
                    for (var r = 0; r < sector.GetDataBlockSize(); r++)
                    {
                        trackImage[0][tptr] = block[0][r];
                        if ((block[1][r / 8] & (1 << (r & 7))) != 0)
                            trackImage[1][tptr / 8] |= (byte)(1 << (tptr & 7));
                        else
                            trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                        tptr++;
                    }
                }
                for (var r = 0; r < thirdGapLen; r++)        // Third gap
                {
                    trackImage[0][tptr] = 0x4E;
                    trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                    tptr++;
                }
            }
            // unused track space
            for (var eoftrk = tptr; eoftrk < trackImage[0].Length; eoftrk++)
            {
                trackImage[0][tptr] = 0x4E;
                trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                tptr++;
            }
            #endregion

            AssignImage(trackImage[0], trackImage[1]);
        }

        public bool RawTestClock(int pos)
        {
            if (_trackClock == null)
            {
                return false;
            }
            return (_trackClock[pos / 8] & (1 << (pos & 7))) != 0;
        }

        public void RawWrite(int pos, byte value, bool clock)
        {
            if (_trackImage == null)
            {
                return;
            }
            _trackImage[pos] = value;
            if (clock)
            {
                _trackClock[pos / 8] |= (byte)(1 << (pos & 7));
            }
            else
            {
                _trackClock[pos / 8] &= (byte)(~(1 << (pos & 7)));
            }
        }

        public byte RawRead(int pos)
        {
            if (_trackImage == null)
            {
                return 0;
            }
            return _trackImage[pos];
        }

        /// <summary>
        /// RawLength
        /// </summary>
        public int trklen
        {
            get
            {
                if (_trackImage == null)
                {
                    return 6400;
                }
                return _trackImage.Length;
            }
        }

        public byte[][] RawImage
        {
            get { return new[] { _trackImage, _trackClock }; }
        }

        /// <summary>
        /// ByteTime
        /// </summary>
        public long ts_byte
        {
            get { return _byteTime; }
        }

        public List<SectorHeader> HeaderList
        {
            get { return _headerList; }
        }

        //public int GetHeadPos(long time)
        //{
        //   if (_trackImage == null)
        //      return (int)(_trackTime / 2);
        //   int pos = (int)((time % _trackTime) / _byteTime);
        //   if (pos > _trackImage.Length)
        //      pos = 0;
        //   return pos;
        //}

        #region UTILS

        /// <summary>
        /// Calculate WD1793 CRC, using initial value for 0xA1,0xA1,0xA1 !!!
        /// </summary>
        /// <param name="startIndex">Start index in track image</param>
        /// <param name="size">Size of block</param>
        public ushort MakeCrc(int startIndex, int size)
        {
            if (_trackImage == null)
            {
                return 0;
            }
            return CrcVg93.Calc3xA1(_trackImage, startIndex, size);
        }

        #endregion
    }
}
