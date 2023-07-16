using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Model.Disk;


namespace ZXMAK2.Serializers.DiskSerializers
{
    public class HfeSerializer : FormatSerializer
    {
        #region private data

        // readonly byte[] MARK_C2 = new byte[] { 0x52, 0x24 };
        readonly byte[] MARK_A1_ADDRESS = new byte[] { 0x44, 0x89, 0x44, 0x89, 0x44, 0x89, 0x55, 0x54 };
        private const string HXCPICFE = "HXCPICFE"; // magic signature
        
        private DiskImage _diskImage;

        #endregion


        public HfeSerializer(DiskImage diskImage)
        {
            _diskImage = diskImage;
        }


        #region FormatSerializer

        public override string FormatGroup => "Disk images";
        public override string FormatName => "HxC emulator image";
        public override string FormatExtension => "HFE";

        public override bool CanDeserialize => true;
        public override bool CanSerialize => true;

        public override void Deserialize(Stream stream)
        {
            if (LoadFromStream(stream))
            {
                _diskImage.ModifyFlag = ModifyFlag.None;
                _diskImage.Present = true;
            }
        }

        public override void Serialize(Stream stream)
        {
            SaveToStream(stream);
            _diskImage.ModifyFlag = ModifyFlag.None;
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

        private bool LoadFromStream(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                var signature = new string(reader.ReadChars(8));
                if (signature != HXCPICFE)
                {
                    Locator.Resolve<IUserMessage>()
                        .Error($"HFE loader\n\nInvalid HFE signature: {signature}");
                    return false;
                }

                reader.ReadByte(); // format revision (ignore)
                var numberOfTracks = reader.ReadByte();
                var numberOfSides = reader.ReadByte();
                var trackEncoding = reader.ReadByte();
                
                // We support only IBM MFM DD floppies (ISOIBM_MFM_ENCODING, which is 0)
                if (trackEncoding != 0)
                {
                    Locator.Resolve<IUserMessage>()
                        .Error($"HFE loader\n\nInvalid track encoding: {trackEncoding}");
                    return false;
                }
                
                _diskImage.SetPhysics(numberOfTracks, numberOfSides);
                
                // We read 12 bytes (8 + 1 + 1 + 1 + 1), so 500 is left until header end
                reader.ReadBytes(500);
                
                // Now there is 'numberOfTracks' items with track pointers
                var trackOffsets = new List<Tuple<ushort, ushort>>();
                for (int _ = 0; _ < numberOfTracks; _++)
                {
                    var trackOffset = reader.ReadUInt16();
                    var trackLen = reader.ReadUInt16();
                    trackOffsets.Add(new Tuple<ushort, ushort>(trackOffset, trackLen));
                }

                // Put list in order of offset increment (most likely it already is)
                trackOffsets = trackOffsets.OrderBy(t => t.Item1).ToList();
                
                // Where are we now in file? 512 bytes header + 4 bytes per each track
                var currentPos = 512 + 2 * sizeof(UInt16) * numberOfTracks;

                // Parse tracks according to the stored offsets and lengths
                foreach (var tp in trackOffsets)
                {
                    var requiredPos = tp.Item1 * 512;
                    if (currentPos > requiredPos)
                    {
                        Locator.Resolve<IUserMessage>().Error($"HFE loader\n\nInvalid file contents");
                        return false;
                    }

                    // Fast forward to the track data
                    reader.ReadBytes(requiredPos - currentPos);
                    currentPos = requiredPos;

                    var cylinderData = reader.ReadBytes(tp.Item2);
                    currentPos += tp.Item2;
                    
                    // "Track data" consist of blocks (512b), where first half is side 0 and second half is side 1
                    // So it is actually the interleaved cylinder data. We need to split it to two tracks.
                    var trackBlocksCount = Convert.ToInt32(Math.Ceiling(tp.Item2 / 512f));
                    var trackDataSide0 = new byte[256 * trackBlocksCount];
                    var trackDataSide1 = new byte[256 * trackBlocksCount];
                    for (int i = 0; i < trackBlocksCount; i++)
                    {
                        var block0 = cylinderData.Skip(512 * i).Take(256);
                        var block1 = cylinderData.Skip(512 * i + 256).Take(256);

                        var pos0 = 256 * i;
                        var pos1 = pos0;

                        foreach (var b in block0)
                            trackDataSide0[pos0++] = b;
                        
                        foreach (var b in block1)
                            trackDataSide1[pos1++] = b;
                    }

                    foreach (var track in new[] { trackDataSide0, trackDataSide1 }.Take(numberOfSides))
                    {
                        var mfm = new MfmCoder(track);

                        // Detect if clock is odd or even bit
                        var a1Pos = mfm.Find(MARK_A1_ADDRESS);

                        // Is track formatted? (otherwise there are no sector headers and we'll be unable to find A1)
                        if (a1Pos != -1)
                        {
                            // Skip 1 bit if there is a garbage ahead of the first clock
                            if ((a1Pos & 1) == 1)
                                mfm.ReadBit();

                            var dataAndClock = mfm.SplitDataAndClock();
                            var data = dataAndClock.Item1;
                            var clock = dataAndClock.Item2;

                            // Now a1Pos is pointer in bytes
                            a1Pos >>= 4;

                            var trackNumber = data[a1Pos + 4];
                            var sideNumber = data[a1Pos + 5];
                            
                            // Now we should convert full array of clock bits to the reduced one, where the only thing,
                            // which is stored, is a sync flag for data byte. 0 when the byte is normal and 1 if byte is
                            // C2/A1 mark

                            var reducedClock = MfmCoder.ReduceClockArray(data, clock);
                            
                            _diskImage.GetTrackImage(trackNumber, sideNumber).AssignImage(data, reducedClock);
                        }
                    }
                }
            }

            return true;
        }

        private void SaveToStream(Stream stream)
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(Encoding.ASCII.GetBytes(HXCPICFE));
                writer.Write((byte)0); // format revision 0
                writer.Write((byte)_diskImage.CylynderCount);
                writer.Write((byte)_diskImage.SideCount);
                writer.Write((byte)0); // ISOIBM_MFM_ENCODING
                writer.Write((UInt16)250); // Bit Rate in kbps
                writer.Write((UInt16)300); // Rotation per minute
                writer.Write((byte)0); // IBMPC_DD_FLOPPYMOD
                writer.Write((byte)0); // dnu
                writer.Write((short)1); // Track lookup table is at 512 bytes
                
                // 20 bytes written. We need to append padding to 512 bytes
                
                var remainingHeaderData = new byte[512 - 20];
                writer.Write(remainingHeaderData);

                var hfeTracks = new List<byte[]>();
                
                for (int cyl = 0; cyl < _diskImage.CylynderCount; cyl++)
                {
                    var track0 = _diskImage.GetTrackImage(cyl, 0);
                    var track1 = _diskImage.SideCount > 1 ? _diskImage.GetTrackImage(cyl, 1) : null;

                    var track0Data = track0.RawImage[0];
                    var track0Clock = MfmCoder.ExpandClockArray(track0Data, track0.RawImage[1]);
                    var track0Mfm = MfmCoder.CombineDataAndClock(track0Data, track0Clock);

                    byte[] track1Mfm;
                    if (track1 != null)
                    {
                        var track1Data = track1.RawImage[0];
                        var track1Clock = MfmCoder.ExpandClockArray(track1Data, track1.RawImage[1]);
                        track1Mfm = MfmCoder.CombineDataAndClock(track1Data, track1Clock);
                    }
                    else
                    {
                        // Even for single sided drives ne need to follow the track format, where 2nd side is present
                        // So replacing it with 0
                        track1Mfm = new byte[track0Mfm.Length];
                    }
                    
                    var trackBlocksCount = Convert.ToInt32(Math.Ceiling(Math.Max(track0Mfm.Length, track1Mfm.Length) / 256f));
                    var hfeTrackData = new byte[trackBlocksCount * 512];
                    for (int i = 0; i < trackBlocksCount; i++)
                    {
                        track0Mfm.Skip(i * 256).Take(256).ToArray().CopyTo(hfeTrackData, i * 512);
                        track1Mfm.Skip(i * 256).Take(256).ToArray().CopyTo(hfeTrackData, i * 512 + 256);
                    }
                    hfeTracks.Add(hfeTrackData);
                }

                var trackOffsetsDataSize = hfeTracks.Count * 2 * sizeof(UInt16);
                var trackOffset512BlockSize = Convert.ToInt32(Math.Ceiling(trackOffsetsDataSize / 512f));

                var offset = 1 + trackOffset512BlockSize;
                foreach (var hfeTrack in hfeTracks)
                {
                    writer.Write((UInt16)offset);
                    writer.Write((UInt16)hfeTrack.Length);
                    offset += hfeTrack.Length / 512;
                }
                
                // Fill gap to next 512 byte block
                writer.Write(new byte[trackOffset512BlockSize * 512 - trackOffsetsDataSize]);
                
                foreach (var hfeTrack in hfeTracks)
                    writer.Write(hfeTrack);
            }
        }
    }
}
