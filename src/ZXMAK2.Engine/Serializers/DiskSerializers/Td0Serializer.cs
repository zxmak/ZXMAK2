/// Description: TD0 format serializer
/// Author: Alex Makeev
/// Date: 18.04.2008
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using ZXMAK2.Model.Disk;
using ZXMAK2.Crc;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Serializers.DiskSerializers
{
    public class Td0Serializer : FormatSerializer
    {
        private readonly DiskImage _diskImage;

        public Td0Serializer(DiskImage diskImage)
        {
            _diskImage = diskImage;
        }

        #region FormatSerializer

        public override string FormatGroup { get { return "Disk images"; } }
        public override string FormatName { get { return "TD0 disk image"; } }
        public override string FormatExtension { get { return "TD0"; } }

        public override bool CanDeserialize { get { return true; } }

        public override void Deserialize(Stream stream)
        {
            loadData(stream);
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


        #region private

        private bool loadData(Stream stream)
        {
            var mainHdr = TD0_MAIN_HEADER.Deserialize(stream);
            if (mainHdr == null)
            {
                return false;
            }
            if (mainHdr.Ver > 21 || mainHdr.Ver < 10)           // 1.0 <= version <= 2.1...
            {
                Locator.Resolve<IUserMessage>()
                    .Error("TD0 loader\n\nFormat version is not supported [0x{0:X2}]", mainHdr.Ver);
                return false;
            }
            if (mainHdr.DataDOS != 0)
            {
                Locator.Resolve<IUserMessage>()
                    .Error("TD0 loader\n\n'DOS Allocated sectors were copied' is not supported!");
                return false;
            }

            var dataStream = stream;
            if (mainHdr.IsAdvandcedCompression)
            {
                if (mainHdr.Ver < 20)    // unsupported Old Advanced compression
                {
                    Locator.Resolve<IUserMessage>()
                        .Error("TD0 loader\n\nOld Advanced compression is not implemented!");
                    return false;
                }
                dataStream = new LzssHuffmanStream(stream);
            }


            #region debug info
            //string state = "Main header loaded, ";
            //if (dataStream is LzssHuffmanStream)
            //    state += "compressed+";
            //else
            //    state += "compressed-";
            //state += ", ";
            //if ((mainHdr.Info & 0x80) != 0)
            //    state += "info+";
            //else
            //    state += "info-";
            //DialogProvider.ShowWarning(state, "TD0 loader");
            #endregion

            var description = string.Empty;
            if ((mainHdr.Info & 0x80) != 0)
            {
                var tmp = new byte[4];
                dataStream.Read(tmp, 0, 2);						// crc
                dataStream.Read(tmp, 2, 2);						// length

                var info = new byte[getUInt16(tmp, 2) + 10];
                for (int i = 0; i < 4; i++)
                {
                    info[i] = tmp[i];
                }

                dataStream.Read(info, 4, 6);					// year,month,day,hour,min,sec (year is relative to 1900)
                dataStream.Read(info, 10, info.Length - 10);	// description

                if (CrcTd0.Calculate(info, 2, 8 + getUInt16(info, 2)) != getUInt16(info, 0))
                {
                    Locator.Resolve<IUserMessage>()
                        .Warning("TD0 loader\n\nInfo crc wrong!");
                }
                // combine lines splitted by zero
                var builder = new StringBuilder();
                int begin = 10, end = 10;
                for (; end < info.Length; end++)
                {
                    if (info[end] == 0 && end > begin)
                    {
                        builder.Append(Encoding.ASCII.GetString(info, begin, end - begin));
                        builder.Append("\n");
                        begin = end + 1;
                    }
                }
                description = builder.ToString();
            }

            var cylCount = -1;
            var sideCount = -1;
            var trackList = new List<TD0_TRACK>();
            for (; ; )
            {
                var track = TD0_TRACK.Deserialize(dataStream);
                if (track.SectorCount == 0xFF)
                {
                    break;
                }
                trackList.Add(track);

                if (cylCount < track.Cylinder) cylCount = track.Cylinder;
                if (sideCount < track.Side) sideCount = track.Side;
            }
            cylCount++;
            sideCount++;

            if (cylCount < 1 || sideCount < 1)
            {
                Locator.Resolve<IUserMessage>()
                    .Error("TD0 loader\n\nInvalid disk structure");
                return false;
            }

            _diskImage.SetPhysics(cylCount, sideCount);
            foreach (TD0_TRACK trk in trackList)
            {
                _diskImage.GetTrackImage(trk.Cylinder, trk.Side).AssignSectors(trk.SectorList);
            }
            _diskImage.Description = description;
            return true;
        }

        #endregion

        #region td0 structs

        private class TD0_MAIN_HEADER			// 12 bytes
        {
            private readonly byte[] _buffer = new byte[12];

            public TD0_MAIN_HEADER()
            {
                for (int i = 0; i < _buffer.Length; i++)
                    _buffer[i] = 0;
            }

            //public ushort ID		// +0:  0x4454["TD"] - 'Normal'; 0x6474["td"] - packed LZH ('New Advanced data compression')
            //{
            //    get { return getUInt16(_buffer, 0); }
            //    set { setUint16(_buffer, 0, value); }
            //}
            //public byte __t;		// +2:  = 0x00   (id null terminator?)
            //public byte __1;		// +3:  ???
            public byte Ver		// +4:  Source version  (1.0 -> 10, ..., 2.1 -> 21)
            {
                get { return _buffer[4]; }
                set { _buffer[4] = value; }
            }
            //public byte __2;		// +5: 0x00 (except for akai)???
            public byte DiskType	// +6:  Source disk type
            {
                get { return _buffer[6]; }
                set { _buffer[6] = value; }
            }
            public byte Info		// +7:  D7-наличие image info; D0-D6 - буква дисковода
            {
                get { return _buffer[7]; }
                set { _buffer[7] = value; }
            }
            public byte DataDOS	// +8:  if(=0)'All sectors were copied', else'DOS Allocated sectors were copied'
            {
                get { return _buffer[8]; }
                set { _buffer[8] = value; }
            }
            public byte ChkdSides	// +9:  if(=1)'One side was checked', else'Both sides were checked'
            {
                get { return _buffer[9]; }
                set { _buffer[9] = value; }
            }
            //public ushort CRC		// +A:  CRC хидера TD0_MAIN_HEADER (кроме байт с CRC)
            //{
            //    get { return getUInt16(_buffer, 0xA); }
            //    set { setUint16(_buffer, 0xA, value); }
            //}
            //public ushort CalculateCRC()
            //{
            //    return CalculateTD0CRC(_buffer, 0, _buffer.Length-2);
            //}

            public bool IsAdvandcedCompression { get { return getUInt16(_buffer, 0) == 0x6474; } } // "td"

            public static TD0_MAIN_HEADER Deserialize(Stream stream)
            {
                TD0_MAIN_HEADER result = new TD0_MAIN_HEADER();
                stream.Read(result._buffer, 0, result._buffer.Length);

                ushort ID = getUInt16(result._buffer, 0);
                if (ID != 0x4454 && ID != 0x6474) // "TD"/"td"
                {
                    Logger.Error("TD0 loader: Invalid header ID");
                    Locator.Resolve<IUserMessage>()
                        .Error("TD0 loader\n\nInvalid header ID");
                    return null;
                }

                ushort crc = CrcTd0.Calculate(result._buffer, 0, result._buffer.Length - 2);
                ushort stampcrc = getUInt16(result._buffer, 0xA);
                if (stampcrc != crc)
                {
                    Logger.Warn("TD0 loader: Main header had bad CRC=0x" + crc.ToString("X4") + " (stamp crc=0x" + stampcrc.ToString("X4") + ")");
                    Locator.Resolve<IUserMessage>()
                        .Warning("TD0 loader\n\nWrong main header CRC");
                }
                return result;
            }

            //public void Serialize(Stream stream)
            //{
            //    stream.Write(_buffer, 0, _buffer.Length);
            //}
        }

        private class TD0_TRACK					// 4 bytes+sectors
        {
            private readonly byte[] _rawData = new byte[4];
            private readonly List<Sector> _sectorList = new List<Sector>();

            public int SectorCount
            {
                get { return _rawData[0]; }
            }

            public int Cylinder
            {
                get { return _rawData[1]; }
            }

            public int Side
            {
                get { return _rawData[2]; }
            }

            // last 1 byte: low byte of crc
            public List<Sector> SectorList
            {
                get { return _sectorList; }
            }

            public static TD0_TRACK Deserialize(Stream stream)
            {
                var hdr = new TD0_TRACK();
                stream.Read(hdr._rawData, 0, 4);
                if (hdr._rawData[0] != 0xFF)			// 0xFF - terminator
                {
                    var crc = CrcTd0.Calculate(hdr._rawData, 0, 3);
                    if (hdr._rawData[3] != (crc & 0xFF))
                    {
                        Logger.Warn("TD0 loader: Track header had bad CRC=0x" + crc.ToString("X4") + " (stamp crc=0x" + hdr._rawData[3].ToString("X2") + ") [CYL:0x" + hdr._rawData[1].ToString("X2") + ";SIDE:" + hdr._rawData[2].ToString("X2"));
                        Locator.Resolve<IUserMessage>()
                            .Warning("TD0 loader\n\nTrack header had bad CRC");
                    }

                    var sectors = new List<Sector>(hdr.SectorCount);
                    for (int s = 0; s < hdr.SectorCount; s++)
                    {
                        hdr._sectorList.Add(TD0_SECTOR.Deserialize(stream));
                    }
                }
                return hdr;
            }
        }

        [Flags]
        private enum SectorFlags
        {
            /// <summary>
            /// Sector was duplicated within a track.
            /// The meaning of some of these bits was taken  from  early  Teledisk
            /// documentation,  and may not be accurate - For example,  I've  seen
            /// images where sectors were duplicated within a track and the 01 bit
            /// was NOT set.
            /// </summary>
            DuplicatedWithinTrack = 0x01,
            /// <summary>
            /// Sector was read with a CRC error
            /// </summary>
            BadCrc = 0x02,
            /// <summary>
            /// Sector has a "deleted-data" address mark
            /// </summary>
            DeletedData = 0x04,
            /// <summary>
            /// Sector data was skipped based on DOS allocation.
            /// Bit values 20 or 10 indicate  that  NO  SECTOR  DATA  BLOCK	FOLLOWS.
            /// </summary>
            NoDataBlockDOS = 0x10,
            /// <summary>
            /// Sector had an ID field but not data.
            /// Bit values 20 or 10 indicate  that  NO  SECTOR  DATA  BLOCK	FOLLOWS.
            /// </summary>
            NoDataBlock = 0x20,
            /// <summary>
            /// Sector had data but no ID field (bogus header)
            /// </summary>
            NoAddressBlock = 0x40,
        }

        private class TD0_SECTOR : Sector
        {
            private TD0_SECTOR() { }

            private byte[] _admark;
            private byte[] _data;

            public override bool AdPresent
            {
                get { return (Td0Flags & SectorFlags.NoAddressBlock) == 0; }
            }

            public override bool DataPresent
            {
                get { return (Td0Flags & (SectorFlags.NoDataBlock | SectorFlags.NoDataBlockDOS)) == 0; }
            }

            public override bool DataDeleteMark
            {
                get { return (Td0Flags & SectorFlags.DeletedData) != 0; }
            }

            public override byte[] Data { get { return _data; } }
            public override byte C { get { return _admark[0]; } }
            public override byte H { get { return _admark[1]; } }
            public override byte R { get { return _admark[2]; } }
            public override byte N { get { return _admark[3]; } }

            public SectorFlags Td0Flags
            {
                get { return (SectorFlags)_admark[4]; }
            }

            public static TD0_SECTOR Deserialize(Stream stream)
            {
                var sector = new TD0_SECTOR();

                // C,H,R,N,Flags,hdrcrc low,hdrcrc high
                var adm = new byte[6];
                stream.Read(adm, 0, 6);
                sector._admark = adm;

                // data size low, data size high, encoding method, rawdata,...
                var datahdr = new byte[2];
                stream.Read(datahdr, 0, 2);
                var rawdata = new byte[getUInt16(datahdr, 0)];
                stream.Read(rawdata, 0, rawdata.Length);
                sector._data = unpackData(rawdata);

                var crc = CrcTd0.Calculate(sector._data, 0, sector._data.Length);
                if (adm[5] != (crc & 0xFF))
                {
                    Logger.Warn(
                        "TD0 loader: Sector data had bad CRC=0x{0:X4} (stamp crc=0x{1:X2}) [C:{2:X2};H:{3:X2};R:{4:X2};N:{5:X2}",
                        crc,
                        adm[5],
                        sector.C,
                        sector.H,
                        sector.R,
                        sector.N);
                    Locator.Resolve<IUserMessage>()
                        .Warning("TD0 loader\n\nSector data had bad CRC");
                }

                sector.SetAdCrc(true);
                sector.SetDataCrc((sector.Td0Flags & SectorFlags.BadCrc) == 0);
                return sector;
            }

            private static byte[] unpackData(byte[] buffer)
            {
                var result = new List<byte>();
                switch (buffer[0])
                {
                    case 0:
                        for (int i = 1; i < buffer.Length; i++)
                        {
                            result.Add(buffer[i]);
                        }
                        break;
                    case 1:
                        {
                            var n = getUInt16(buffer, 1);
                            for (var i = 0; i < n; i++)
                            {
                                result.Add(buffer[3]);
                                result.Add(buffer[4]);
                            }
                        }
                        break;
                    case 2:
                        var index = 1;
                        do
                        {
                            switch (buffer[index++])
                            {
                                case 0:
                                    {
                                        var n = buffer[index++];
                                        for (var i = 0; i < n; i++)
                                        {
                                            result.Add(buffer[index++]);
                                        }
                                    }
                                    break;
                                case 1:
                                    {
                                        var n = buffer[index++];
                                        for (var i = 0; i < n; i++)
                                        {
                                            result.Add(buffer[index]);
                                            result.Add(buffer[index + 1]);
                                        }
                                        index += 2;
                                    }
                                    break;
                                default:
                                    Logger.Warn("Unknown sector encoding!");
                                    Locator.Resolve<IUserMessage>()
                                        .Warning("TD0 loader\n\nUnknown sector encoding!");
                                    index = buffer.Length;
                                    break;
                            }
                        } while (index < buffer.Length);
                        break;
                }
                return result.ToArray();
            }
        }

        #endregion
    }
}
