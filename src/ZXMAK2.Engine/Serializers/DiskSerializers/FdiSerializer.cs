/// Description: FDI load helper
/// Author: Alex Makeev
/// Date: 15.04.2007
using System;
using System.IO;
using System.Collections.Generic;

using ZXMAK2.Model.Disk;
using ZXMAK2.Crc;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Serializers.DiskSerializers
{
    public class FdiSerializer : FormatSerializer
    {
        private DiskImage _diskImage;


        public FdiSerializer(DiskImage diskImage)
        {
            _diskImage = diskImage;
        }


        #region FormatSerializer

        public override string FormatGroup { get { return "Disk images"; } }
        public override string FormatName { get { return "FDI disk image"; } }
        public override string FormatExtension { get { return "FDI"; } }

        public override bool CanDeserialize { get { return true; } }

        public override void Deserialize(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            loadData(stream);

            _diskImage.SetPhysics(_cylynderImages.Count, _sideCount);
            for (var cyl = 0; cyl < _cylynderImages.Count; cyl++)
            {
                var cylynder = _cylynderImages[cyl];
                for (var side = 0; side < _diskImage.SideCount; side++)
                {
                    _diskImage.GetTrackImage(cyl, side)
                        .AssignImage(cylynder[side][0], cylynder[side][1]);
                }
            }
            _diskImage.ModifyFlag = ModifyFlag.None;
            _diskImage.Present = true;
        }

        public override void SetSource(string fileName)
        {
            _diskImage.FileName = fileName;
        }

        public override void SetReadOnly(bool readOnly)
        {
            _diskImage.IsWP = readOnly;
        }

        #endregion


        #region private data
        private bool _writeProtect = false;
        private string _description = string.Empty;
        private List<byte[][][]> _cylynderImages = new List<byte[][][]>();
        private int _sideCount = 0;
        #endregion


        private void loadData(Stream stream)
        {
            _cylynderImages.Clear();
            _sideCount = 0;

            if (stream.Length < 14)
            {
                Locator.Resolve<IUserMessage>()
                    .Error("FDI loader\n\nCorrupted disk image!");
                return;
            }

            var hdr1 = new byte[14];
            stream.Read(hdr1, 0, 14);

            if (hdr1[0] != 0x46 ||
                hdr1[1] != 0x44 ||
                hdr1[2] != 0x49)
            {
                Locator.Resolve<IUserMessage>()
                    .Error("FDI loader\n\nInvalid FDI file!");
                return;
            }

            _writeProtect = hdr1[3] != 0;
            var cylCount = hdr1[4] | (hdr1[5] << 8);
            _sideCount = hdr1[6] | (hdr1[7] << 8);

            var descrOffset = hdr1[8] | (hdr1[9] << 8);
            var dataOffset = hdr1[0xA] | (hdr1[0xB] << 8);
            var hdr2len = hdr1[0xC] | (hdr1[0xD] << 8);

            // TODO: check filesize!

            if (hdr2len > 0)
            {
                var hdr2 = new byte[hdr2len];
                stream.Read(hdr2, 0, hdr2len);
            }

            var trackHeaderList = new List<List<FdiSectorHeader>>();
            for (var trk = 0; trk < cylCount; trk++)
            {
                for (var side = 0; side < _sideCount; side++)
                {
                    trackHeaderList.Add(readTrackHeader(stream));
                }
            }

            for (var cyl = 0; cyl < cylCount; cyl++)
            {
                var cylynderData = new byte[_sideCount][][];
                _cylynderImages.Add(cylynderData);

                for (var side = 0; side < _sideCount; side++)
                {
                    var sectorHeaderList = trackHeaderList[cyl * _sideCount + side];
                    // Вычитываем массивы данных
                    for (var sec = 0; sec < sectorHeaderList.Count; sec++)
                    {
                        var sh = sectorHeaderList[sec];

                        if ((sh.Flags & 0x40) != 0)   // нет массива данных?
                            continue;

                        var dataArrayLen = 128 << sh.N;
                        //if ((sh.Flags & 0x01) != 0)      // CRC 128 OK?
                        //   dataArrayLen = 128;
                        //if ((sh.Flags & 0x02) != 0)      // CRC 256 OK?
                        //   dataArrayLen = 256;
                        //if ((sh.Flags & 0x04) != 0)      // CRC 1024 OK?
                        //   dataArrayLen = 1024;
                        //if ((sh.Flags & 0x08) != 0)      // CRC 2048 OK?
                        //   dataArrayLen = 2048;
                        //if ((sh.Flags & 0x10) != 0)      // CRC 4096 OK?
                        //   dataArrayLen = 4096;
                        sh.IsCrcOk = (sh.Flags & 0x1F) != 0;

                        sh.DataArray = new byte[dataArrayLen];
                        stream.Seek(dataOffset + sh.DataOffset, SeekOrigin.Begin);
                        stream.Read(sh.DataArray, 0, dataArrayLen);
                    }

                    // Формируем дорожку
                    cylynderData[side] = generateTrackImage(sectorHeaderList);
                }
            }
        }


        #region private methods

        private byte[][] generateTrackImage(List<FdiSectorHeader> sectorHeaderList)
        {
            var trackImage = new byte[2][];

            // Вычисляем необходимое число байт под данные:
            var imageSize = 6250;

            var secCount = sectorHeaderList.Count;
            var trkdatalen = 0;
            for (var ilsec = 0; ilsec < secCount; ilsec++)
            {
                var hdr = sectorHeaderList[ilsec];

                trkdatalen += 2 + 6;     // for marks:   0xA1, 0xFE, 6bytes
                var slen = 128 << hdr.N;

                if ((hdr.Flags & 0x40) != 0)   // заголовок без массива данных
                {
                    slen = 0;
                }
                else
                {
                    trkdatalen += 4;       // for data header/crc: 0xA1, 0xFB, ...,2bytes
                }

                trkdatalen += slen;
            }

            var freeSpace = imageSize - (trkdatalen + secCount * (3 + 2));  // 3x4E & 2x00 per sector
            var synchroPulseLen = 1; // 1 уже учтен в trkdatalen...
            var firstSpaceLen = 1;
            var secondSpaceLen = 1;
            var thirdSpaceLen = 1;
            var synchroSpaceLen = 1;
            freeSpace -= firstSpaceLen + secondSpaceLen + thirdSpaceLen + synchroSpaceLen;
            if (freeSpace < 0)
            {
                imageSize += -freeSpace;
                freeSpace = 0;
            }
            // Распределяем длины пробелов и синхропромежутка:
            while (freeSpace > 0)
            {
                if (freeSpace >= (secCount * 2)) // Synchro for ADMARK & DATA
                    if (synchroSpaceLen < 12)
                    {
                        synchroSpaceLen++;
                        freeSpace -= secCount * 2;
                    }
                if (freeSpace < secCount) break;

                if (firstSpaceLen < 10) { firstSpaceLen++; freeSpace -= secCount; }
                if (freeSpace < secCount) break;
                if (secondSpaceLen < 22) { secondSpaceLen++; freeSpace -= secCount; }
                if (freeSpace < secCount) break;
                if (thirdSpaceLen < 60) { thirdSpaceLen++; freeSpace -= secCount; }
                if (freeSpace < secCount) break;

                if ((synchroSpaceLen >= 12) && (firstSpaceLen >= 10) &&
                    (secondSpaceLen >= 22) && (thirdSpaceLen >= 60))
                    break;
            }
            // по возможности делаем три синхроимпульса...
            if (freeSpace > (secCount * 2) + 10) { synchroPulseLen++; freeSpace -= secCount; }
            if (freeSpace > (secCount * 2) + 9) synchroPulseLen++;
            if (freeSpace < 0)
            {
                imageSize += -freeSpace;
                freeSpace = 0;
            }


            // Форматируем дорожку...
            trackImage[0] = new byte[imageSize];
            trackImage[1] = new byte[trackImage[0].Length / 8 + (((trackImage[0].Length & 7) != 0) ? 1 : 0)];

            var tptr = 0;
            for (var sec = 0; sec < secCount; sec++)
            {
                var hdr = sectorHeaderList[sec];

                for (var r = 0; r < firstSpaceLen; r++)        // Первый пробел
                {
                    trackImage[0][tptr] = 0x4E;
                    trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                    tptr++;
                }
                for (var r = 0; r < synchroSpaceLen; r++)        // Синхропромежуток
                {
                    trackImage[0][tptr] = 0x00;
                    trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                    tptr++;
                }
                var ptrcrc = tptr;
                for (var r = 0; r < synchroPulseLen; r++)        // Синхроимпульс
                {
                    trackImage[0][tptr] = 0xA1;
                    trackImage[1][tptr / 8] |= (byte)(1 << (tptr & 7));
                    tptr++;
                }
                trackImage[0][tptr] = 0xFE;               // Метка "Адрес"
                trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                tptr++;

                trackImage[0][tptr] = hdr.C;              // cyl
                trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                tptr++;
                trackImage[0][tptr] = hdr.H;              // head
                trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                tptr++;
                trackImage[0][tptr] = hdr.R;              // sector #
                trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                tptr++;
                trackImage[0][tptr] = hdr.N;              // len code
                trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                tptr++;

                ushort vgcrc = CrcVg93.Calculate(trackImage[0], ptrcrc, tptr - ptrcrc);
                trackImage[0][tptr] = (byte)vgcrc;        // crc
                trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                tptr++;
                trackImage[0][tptr] = (byte)(vgcrc >> 8);   // crc
                trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                tptr++;

                for (var r = 0; r < secondSpaceLen; r++)        // Второй пробел
                {
                    trackImage[0][tptr] = 0x4E;
                    trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                    tptr++;
                }
                for (var r = 0; r < synchroSpaceLen; r++)        // Синхропромежуток
                {
                    trackImage[0][tptr] = 0x00;
                    trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                    tptr++;
                }

                byte fdiSectorFlags = hdr.Flags;
                // !!!!!!!!!
                // !WARNING! this feature of FDI format is NOT FULL DOCUMENTED!!!
                // !!!!!!!!!
                //
                //  Flags::bit6 - Возможно, 1 в данном разряде
                //                будет обозначать адресный маркер без области данных.
                //
                if ((fdiSectorFlags & 0x40) == 0) // oh-oh, data area can be not present... ;-) 
                {
                    ptrcrc = tptr;
                    for (var r = 0; r < synchroPulseLen; r++)        // Синхроимпульс
                    {
                        trackImage[0][tptr] = 0xA1;
                        trackImage[1][tptr / 8] |= (byte)(1 << (tptr & 7));
                        tptr++;
                    }

                    if ((fdiSectorFlags & 0x80) != 0)
                    {
                        trackImage[0][tptr] = 0xF8; // Метка "Удаленные данные"
                    }
                    else
                    {
                        trackImage[0][tptr] = 0xFB; // Метка "Данные"
                    }
                    trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                    tptr++;

                    //TODO: sector len from crc flags?
                    var SL = 128 << hdr.N;

                    for (var r = 0; r < SL; r++)        // сектор SL байт
                    {
                        trackImage[0][tptr] = hdr.DataArray[r];
                        trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                        tptr++;
                    }

                    vgcrc = CrcVg93.Calculate(trackImage[0], ptrcrc, tptr - ptrcrc);

                    if ((fdiSectorFlags & 0x3F) == 0)         // CRC not correct?
                    {
                        vgcrc ^= (ushort)0xFFFF;            // oh-oh, high technology... CRC bad... ;-)
                    }

                    trackImage[0][tptr] = (byte)vgcrc;        // crc
                    trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                    tptr++;
                    trackImage[0][tptr] = (byte)(vgcrc >> 8);   // crc
                    trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                    tptr++;
                }


                for (var r = 0; r < thirdSpaceLen; r++)        // Третий пробел
                {
                    trackImage[0][tptr] = 0x4E;
                    trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                    tptr++;
                }
            }
            for (var eoftrk = tptr; eoftrk < trackImage[0].Length; eoftrk++)
            {
                trackImage[0][tptr] = 0x4E;
                trackImage[1][tptr / 8] &= (byte)~(1 << (tptr & 7));
                tptr++;
            }

            return trackImage;
        }

        private List<FdiSectorHeader> readTrackHeader(Stream f)
        {
            var buf = new byte[7];
            var sectorHeaderList = new List<FdiSectorHeader>();

            f.Read(buf, 0, 4);   // data offset in data block
            var dataOffset = buf[0] | (buf[1] << 8) | (buf[2] << 16) | (buf[3] << 24);

            f.Read(buf, 0, 2);   // reserved

            f.Read(buf, 0, 1);   // sector count
            var sectorCount = buf[0];

            for (var i = 0; i < sectorCount; i++)
            {
                var sh = FdiSectorHeader.Deserialize(f, dataOffset);
                sectorHeaderList.Add(sh);
            }
            return sectorHeaderList;
        }

        #endregion

        private class FdiSectorHeader
        {
            public byte C;   // std data CYLYNDER?
            public byte H;   // std data HEAD?
            public byte R;   // std data
            public byte N;   // std data DATA ARRAY LEN

            public byte Flags;      // 012345=crcok(128,256,1024,2048,4096); 6=no data array; 7=0:normal marker/1:deleted marker
            public int DataOffset;  // sector data offset in track data block (dataOffset+trackOffset)
            public byte[] DataArray = null;

            public bool IsCrcOk;

            public static FdiSectorHeader Deserialize(
                Stream stream,
                int dataOffset)
            {
                var buf = new byte[7];
                stream.Read(buf, 0, 7);
                var sh = new FdiSectorHeader();
                sh.C = buf[0];
                sh.H = buf[1];
                sh.R = buf[2];
                sh.N = buf[3];
                sh.Flags = buf[4];
                sh.DataOffset = dataOffset + (buf[5] | (buf[6] << 8));
                return sh;
            }
        }
    }
}
