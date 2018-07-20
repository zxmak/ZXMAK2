using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Model.Tape.Interfaces;
using ZXMAK2.Model.Tape.Entities;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Serializers.TapeSerializers
{
    public class CswSerializer : FormatSerializer
    {
        private ITapeDevice _tape;


        public CswSerializer(ITapeDevice tape)
        {
            _tape = tape;
        }


        #region FormatSerializer

        public override string FormatGroup { get { return "Tape images"; } }
        public override string FormatName { get { return "CSW image"; } }
        public override string FormatExtension { get { return "CSW"; } }

        public override bool CanDeserialize { get { return true; } }

        public override void Deserialize(Stream stream)
        {
            _tape.Blocks.Clear();
            var blocks = Load(stream, _tape.TactsPerSecond);
            if (blocks != null)
            {
                _tape.Blocks.AddRange(blocks);
            }
            _tape.Reset();
        }

        private static IEnumerable<ITapeBlock> Load(Stream stream, int tactsPerSecond)
        {
            try
            {
                var hdr = new byte[0x34];
                stream.Read(hdr, 0, 0x20);

                var txtInfo = string.Empty;
                if (Encoding.ASCII.GetString(hdr, 0, 22) != "Compressed Square Wave")
                {
                    Locator.Resolve<IUserMessage>()
                        .Error("CSW loader\n\nInvalid CSW file, identifier not found!");
                    return null;
                }
                var version = hdr[0x17];
                if (version > 2)
                {
                    Locator.Resolve<IUserMessage>()
                        .Error("CSW loader\n\nFormat CSW V{0}.{1} not supported!", hdr[0x17], hdr[0x18]);
                    return null;
                }
                if (version == 2)  // CSW V2
                {
                    stream.Read(hdr, 0x20, 0x14);

                    var binDesc = new List<byte>();
                    for (var i = 0; i < 16 && hdr[0x24 + i] != 0; i++)
                    {
                        binDesc.Add(hdr[0x24 + i]);
                    }
                    txtInfo = Encoding.ASCII.GetString(binDesc.ToArray());

                    var extHdr = new byte[hdr[0x23]];
                    stream.Read(extHdr, 0, extHdr.Length);
                }
                var cswSampleRate = version == 2 ?
                    BitConverter.ToInt32(hdr, 0x19) :
                    BitConverter.ToUInt16(hdr, 0x19);
                var cswCompressionType = version == 2 ?
                    hdr[0x21] :
                    hdr[0x1B];
                // Flags b0: initial polarity; if set, the signal starts at logical high
                var cswFlags = version == 2 ?
                    hdr[0x22] :
                    hdr[0x1C];
                var cswPulseCount = version == 2 ?
                    BitConverter.ToInt32(hdr, 0x1D) :
                    (int)stream.Length - 0x20;
                var rleData = LoadRleData(
                    stream,
                    cswCompressionType,
                    cswPulseCount);

                var ratio = tactsPerSecond / (double)cswSampleRate; // usually 3.5mhz / 44khz

                var list = new List<TapeBlock>();
                var pulses = new List<int>();
                var blockTime = 0;
                var blockCounter = 0;
                foreach (var rle in rleData)
                {
                    var len = (int)Math.Round(rle * ratio, MidpointRounding.AwayFromZero);
                    pulses.Add(len);

                    blockTime += len;
                    if (blockTime >= tactsPerSecond * 2)
                    {
                        var tb = new TapeBlock();
                        tb.Description = string.Format("CSW-{0:D3}", blockCounter++);
                        tb.Periods = new List<int>(pulses);
                        list.Add(tb);
                        blockTime = 0;
                        pulses.Clear();
                    }
                }
                if (pulses.Count > 0)
                {
                    var tb = new TapeBlock();
                    tb.Description = string.Format("CSW-{0:D3}", blockCounter++);
                    tb.Periods = new List<int>(pulses);
                    list.Add(tb);
                }

                if (txtInfo != string.Empty)
                {
                    var desc = new TapeBlock();
                    desc.Periods = new List<int>();
                    desc.Description = string.Format("[{0}]", txtInfo);
                    list.Insert(0, desc);
                }
                return list;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                Locator.Resolve<IUserMessage>()
                    .Error("CSW loader\n\n{0}", ex.Message);
                return null;
            }
        }

        #endregion


        #region Private

        private static IEnumerable<int> LoadRleData(
            Stream stream,
            byte type,
            int dataSize)
        {
            if (type == 2)   // Z-RLE
            {
                stream = new ZipLib.Zip.Compression.Streams.InflaterInputStream(stream);
            }
            else if (type != 1)
            {
                throw new NotSupportedException(
                    string.Format(
                        "Unknown compression type: 0x{0:X2}!", 
                        type));
            }
            var list = new List<int>();
            while ((type == 1 && stream.Position < stream.Length) ||
                (type == 2 && list.Count < dataSize))
            {
                var buf = new byte[4];
                var read = stream.Read(buf, 0, 1);
                if (read != 1)
                {
                    throw new EndOfStreamException("Unexpected end of stream");
                }
                if (buf[0] == 0)
                {
                    read = stream.Read(buf, 0, 4);
                    if (read != 4)
                    {
                        throw new EndOfStreamException("Unexpected end of stream");
                    }
                }
                var rle =
                    (buf[0] << 0) |
                    (buf[1] << 8) |
                    (buf[2] << 16) |
                    (buf[3] << 24);
                list.Add(rle);
            }
            return list;
        }

        #endregion Private
    }
}
