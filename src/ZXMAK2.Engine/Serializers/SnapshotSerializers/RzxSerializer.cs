using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using System.IO.Compression;


namespace ZXMAK2.Serializers.SnapshotSerializers
{
    public class RzxSerializer : SnapshotSerializerBase
    {
        public RzxSerializer(Spectrum spec)
            : base(spec)
        {
        }

        public override string FormatExtension { get { return "RZX"; } }

        public override bool CanDeserialize { get { return true; } }
        public override bool CanSerialize { get { return false; } }

        public override void Deserialize(Stream stream)
        {
            var reader = CreateRzxSource(stream);
            _spec.BusManager.RzxHandler.Play(reader);
            var ula = _spec.BusManager.FindDevice<IUlaDevice>();
            ula.ForceRedrawFrame();
            _spec.RaiseUpdateState();
        }

        private IRzxFrameSource CreateRzxSource(Stream stream)
        {
            var data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            var dataStream = new MemoryStream(data);
            try
            {
                return new RzxBlockReader(_spec, dataStream);
            }
            catch
            {
                dataStream.Dispose();
                throw;
            }
        }
    }

    public sealed class RzxBlockReader : IRzxFrameSource
    {
        #region Constants

        private const string RZX_SIGNATURE = "RZX!";

        #endregion


        private readonly Spectrum m_spectrum;
        private Stream m_stream;
        private UInt32 m_flags;
        private int m_majorRevision;
        private int m_minorRevision;


        public RzxBlockReader(Spectrum spectrum, Stream stream)
        {
            m_spectrum = spectrum;
            m_stream = stream;
            CheckEof();
            Open();
        }

        public void Dispose()
        {
        }

        private void Open()
        {
            if (IsEndOfFile)
            {
                return;
            }
            var header = new byte[0x0A];
            m_stream.Read(header, 0, header.Length);
            if (Encoding.ASCII.GetString(header, 0, 4) != RZX_SIGNATURE)
            {
                throw new InvalidDataException("Invalid RZX file");
            }
            m_majorRevision = header[4];
            m_minorRevision = header[5];
            m_flags = BitConverter.ToUInt32(header, 6);
        }

        public bool IsSigned { get { return (m_flags & 1) != 0; } }
        public bool IsEndOfFile { get; private set; }

        public RzxBlock ReadBlock()
        {
            CheckEof();
            if (IsEndOfFile)
            {
                return null;
            }
            int blockId = m_stream.ReadByte();
            if (blockId < 0 || blockId > 255)
                RaiseUnexpectedEndException();
            byte[] dataBuf = new byte[4];
            if (m_stream.Read(dataBuf, 0, dataBuf.Length) != dataBuf.Length)
                RaiseUnexpectedEndException();
            Int32 blockLength = BitConverter.ToInt32(dataBuf, 0) - 5;
            if (blockLength < 0)
                throw new InvalidDataException(string.Format("Invalid block length={0}", blockLength));
            byte[] blockData = new byte[blockLength];
            if (m_stream.Read(blockData, 0, blockData.Length) != blockData.Length)
                RaiseUnexpectedEndException();
            return RzxBlock.Create(blockId, blockData);
        }

        private void CheckEof()
        {
            if (m_stream == null)
            {
                return;
            }
            IsEndOfFile = m_stream.Position >= m_stream.Length;
            if (IsEndOfFile)
            {
                var stream = m_stream;
                m_stream = null;
                stream.Dispose();
            }
        }

        private void RaiseUnexpectedEndException()
        {
            throw new InvalidDataException(string.Format("Unexpected end of stream file at position {0}", m_stream.Position));
        }

        public RzxFrame[] GetNextFrameArray()
        {
            while (true)
            {
                RzxBlock block = ReadBlock();
                if (block == null)
                    return null;
                RzxBlockRecording rzxRec = block as RzxBlockRecording;
                if (rzxRec != null)
                    return loadRec(rzxRec);
                RzxBlockSnapshot rzxSnap = block as RzxBlockSnapshot;
                if (rzxSnap != null)
                    loadSnap(rzxSnap);
            }
        }

        private RzxFrame[] loadRec(RzxBlockRecording rzxRec)
        {
            int remainingTacts = m_spectrum.BusManager.FrameTactCount - m_spectrum.BusManager.GetFrameTact();
            m_spectrum.CPU.Tact += remainingTacts + rzxRec.StartTact;
            return rzxRec.GetFrameArray();
        }

        private void loadSnap(RzxBlockSnapshot rzxSnap)
        {
            Logger.Debug("RZX-LOAD-SNAP: {0}", rzxSnap.Extension);
            var fs = m_spectrum.BusManager.LoadManager.GetSerializer("." + rzxSnap.Extension);
            if (fs.CanDeserialize)
            {
                using (Stream stream = rzxSnap.GetSnapshotStream())
                    fs.Deserialize(stream);
            }
        }
    }

    public class RzxBlock
    {
        private readonly int m_id;
        private readonly byte[] m_rawData;

        public virtual int Id { get { return m_id; } }
        public virtual byte[] RawData { get { return m_rawData; } }

        public RzxBlock(int id, byte[] rawData)
        {
            m_id = id;
            m_rawData = rawData;
        }

        protected void RaiseInvalidIdException()
        {
            throw new ArgumentException(string.Format("Invalid {0} Id", this.GetType().Name));
        }

        protected static Stream GetDataStream(byte[] rawData, int offset, int length, bool compressed)
        {
            var stream = new MemoryStream();
            try
            {
                stream.Write(rawData, offset, length);
                stream.Seek(0, SeekOrigin.Begin);
                if (!compressed)
                {
                    return stream;
                }
                stream.ReadByte();
                stream.ReadByte();
                return new DeflateStream(stream, CompressionMode.Decompress, false);
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }

        protected static byte[] GetBytesAsciiZ(byte[] array, int offset, int length)
        {
            List<byte> list = new List<byte>();
            for (int i = 0; i < length; i++)
            {
                byte c = array[i + offset];
                if (c == 0)
                    break;
                list.Add(c);
            }
            return list.ToArray();
        }

        public static RzxBlock Create(int blockId, byte[] blockData)
        {
            if (blockId == RzxBlockSnapshot.ID)
                return new RzxBlockSnapshot(blockId, blockData);
            else if (blockId == RzxBlockRecording.ID)
                return new RzxBlockRecording(blockId, blockData);
            else if (blockId == RzxBlockCreatorInfo.ID)
                return new RzxBlockCreatorInfo(blockId, blockData);
            else if (blockId == RzxBlockSecurityInfo.ID)
                return new RzxBlockSecurityInfo(blockId, blockData);
            else if (blockId == RzxBlockSecuritySign.ID)
                return new RzxBlockSecuritySign(blockId, blockData);
            else return new RzxBlock(blockId, blockData);
        }
    }

    public class RzxBlockSnapshot : RzxBlock
    {
        public static readonly int ID = 0x30;

        public RzxBlockSnapshot(int id, byte[] rawData)
            : base(id, rawData)
        {
            if (id != ID)
            {
                RaiseInvalidIdException();
            }
        }

        public Stream GetSnapshotStream()
        {
            byte[] snapData = new byte[UncompressedLength];
            if (IsExternal)
            {
                UInt32 fileCrc = BitConverter.ToUInt32(RawData, 12);
                string fileName = Encoding.ASCII.GetString(GetBytesAsciiZ(RawData, 12 + 4, RawData.Length - (12 + 4)));
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    fs.Read(snapData, 0, snapData.Length);
                //if (fileCrc!=0)
                //{
                //    ZipLib.Checksums.Crc32 crc = new ZipLib.Checksums.Crc32();
                //    crc.Reset();
                //    crc.Update(snapData);
                //    if(crc.Value!=fileCrc)
                //    {
                //        throw new InvalidDataException("Snapshot CRC failed");
                //    }
                //}
            }
            else
            {
                using (Stream stream = GetDataStream(RawData, 12, RawData.Length - 12, IsCompressed))
                    stream.Read(snapData, 0, snapData.Length);
            }
            return new MemoryStream(snapData);
        }

        public int UncompressedLength { get { return BitConverter.ToInt32(RawData, 8); } }
        public string Extension { get { return Encoding.ASCII.GetString(GetBytesAsciiZ(RawData, 4, 4)); } }

        public bool IsExternal { get { return (RawData[0] & 1) != 0; } }
        public bool IsCompressed { get { return (RawData[0] & 2) != 0; } }
    }

    public class RzxBlockRecording : RzxBlock
    {
        public static readonly int ID = 0x80;

        public RzxBlockRecording(int id, byte[] rawData)
            : base(id, rawData)
        {
            if (id != ID)
            {
                RaiseInvalidIdException();
            }
        }

        public RzxFrame[] GetFrameArray()
        {
            if (IsEncrypted)
            {
                throw new NotSupportedException("Encrypted RZX frames are not supported");
            }
            List<RzxFrame> rzxData = new List<RzxFrame>();
            using (Stream dataStream = GetDataStream(RawData, 13, RawData.Length - 13, IsCompressed))
            {
                BinaryReader reader = new BinaryReader(dataStream);
                byte[] ioData = null;
                for (int i = 0; i < FrameCount; i++)
                {
                    UInt16 counter = reader.ReadUInt16();
                    UInt16 readsCount = reader.ReadUInt16();

                    if (readsCount != 0xFFFF) // not repeated frame
                    {
                        ioData = reader.ReadBytes(readsCount);
                    }
                    else if (ioData == null)
                    {
                        throw new InvalidDataException("Repeated IO frame can not be the first frame in RzxBlockRecording");
                    }

                    rzxData.Add(new RzxFrame(counter, ioData));
                }
            }
            return rzxData.ToArray();
        }

        public int FrameCount { get { return BitConverter.ToInt32(RawData, 0); } }
        public int StartTact { get { return BitConverter.ToInt32(RawData, 5); } }
        //public int Flags { get { return BitConverter.ToInt32(RawData, 9); } }

        public bool IsEncrypted { get { return (RawData[9] & 1) != 0; } }
        public bool IsCompressed { get { return (RawData[9] & 2) != 0; } }
    }

    public class RzxBlockCreatorInfo : RzxBlock
    {
        public static readonly int ID = 0x10;

        public RzxBlockCreatorInfo(int id, byte[] rawData)
            : base(id, rawData)
        {
            if (id != ID)
            {
                RaiseInvalidIdException();
            }
        }

        public string CreatorId
        {
            get { return Encoding.ASCII.GetString(GetBytesAsciiZ(RawData, 0, 20)); }
        }

        public int CreatorMajorVersion
        {
            get { return BitConverter.ToInt16(RawData, 20); }
        }

        public int CreatorMinorVersion
        {
            get { return BitConverter.ToInt16(RawData, 22); }
        }
    }

    public class RzxBlockSecurityInfo : RzxBlock
    {
        public static readonly int ID = 0x20;

        public RzxBlockSecurityInfo(int id, byte[] rawData)
            : base(id, rawData)
        {
            if (id != ID)
            {
                RaiseInvalidIdException();
            }
        }

        public UInt32 KeyId { get { return BitConverter.ToUInt32(RawData, 0); } }
        public Int32 WeekCode { get { return BitConverter.ToInt32(RawData, 4); } }
    }

    public class RzxBlockSecuritySign : RzxBlock
    {
        public static readonly int ID = 0x21;

        public RzxBlockSecuritySign(int id, byte[] rawData)
            : base(id, rawData)
        {
            if (id != ID)
            {
                RaiseInvalidIdException();
            }
        }
    }
}
