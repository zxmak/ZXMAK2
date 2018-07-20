using System;
using System.IO;
using System.Text;

using ZXMAK2.Model.Disk;
using ZXMAK2.Crc;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Serializers.DiskSerializers
{
    public class UdiSerializer : FormatSerializer
    {
        #region private data

        private DiskImage _diskImage;

        #endregion


        public UdiSerializer(DiskImage diskImage)
        {
            _diskImage = diskImage;
        }


        #region FormatSerializer

        public override string FormatGroup { get { return "Disk images"; } }
        public override string FormatName { get { return "UDI disk image"; } }
        public override string FormatExtension { get { return "UDI"; } }

        public override bool CanDeserialize { get { return true; } }
        public override bool CanSerialize { get { return true; } }

        public override void Deserialize(Stream stream)
        {
            loadFromStream(stream);
            _diskImage.ModifyFlag = ModifyFlag.None;
            _diskImage.Present = true;
        }

        public override void Serialize(Stream stream)
        {
            byte[] udi;
            using (MemoryStream tmpStream = new MemoryStream())
            {
                saveToStream(tmpStream);
                udi = tmpStream.ToArray();
            }
            stream.Write(udi, 0, udi.Length);
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


        #region private methods

        private void loadFromStream(Stream stream)
        {
            var crc = CrcUdi.InitValue;
            var hdr = new byte[16];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(hdr, 0, hdr.Length);
            crc = CrcUdi.Update(crc, hdr, 0, hdr.Length);

            if (Encoding.ASCII.GetString(hdr, 0, 4) != "UDI!") // check "UDI!"
            {
                Locator.Resolve<IUserMessage>()
                    .Error("UDI loader\n\nUnknown *.UDI file identifier!");
                return;
            }

            var size = hdr[4] | hdr[5] << 8 | hdr[6] << 16 | hdr[7] << 24;
            if (stream.Length != (size + 4))
            {
                Locator.Resolve<IUserMessage>()
                    .Error("UDI loader\n\nCorrupt *.UDI file!");
                return;
            }
            if (hdr[8] != 0)
            {
                Locator.Resolve<IUserMessage>()
                    .Error("UDI loader\n\nUnsupported *.UDI format version!");
                return;
            }

            var cylCount = hdr[9] + 1;
            var sideCount = hdr[0x0A] + 1;
            var hdr2len = hdr[0x0C] | hdr[0x0D] << 8 | hdr[0x0E] << 16 | hdr[0x0F] << 24;

            _diskImage.SetPhysics(cylCount, sideCount);

            if (hdr2len > 0)
            {
                byte[] hdr2 = new byte[hdr2len];
                stream.Read(hdr2, 0, hdr2len);
                crc = CrcUdi.Update(crc, hdr2, 0, hdr2.Length);
            }
            //            f.Seek(16 + hdr2len, SeekOrigin.Begin);
            for (var cyl = 0; cyl < cylCount; cyl++)
            {
                for (var side = 0; side < sideCount; side++)
                {
                    stream.Read(hdr, 0, 3);
                    crc = CrcUdi.Update(crc, hdr, 0, 3);
                    var trackType = hdr[0];
                    var trackSize = hdr[1] | hdr[2] << 8;
                    var image = new byte[2][];
                    image[0] = new byte[trackSize];
                    image[1] = new byte[trackSize / 8 + (((trackSize & 7) != 0) ? 1 : 0)];
                    stream.Read(image[0], 0, image[0].Length);
                    crc = CrcUdi.Update(crc, image[0], 0, image[0].Length);
                    stream.Read(image[1], 0, image[1].Length);
                    crc = CrcUdi.Update(crc, image[1], 0, image[1].Length);
                    _diskImage.GetTrackImage(cyl, side)
                        .AssignImage(image[0], image[1]);
                }
            }

            // check CRC
            stream.Read(hdr, 0, 4);
            var stampCrc = (UInt32)(hdr[0] | hdr[1] << 8 | hdr[2] << 16 | hdr[3] << 24);
            if (stampCrc != crc)
            {
                Locator.Resolve<IUserMessage>()
                    .Warning(
                        "UDI loader\n\nCRC ERROR:\nStamp: {0:X8}\nReal: {1:X8}", 
                        stampCrc, 
                        crc);
            }
        }

        private void saveToStream(Stream stream)
        {
            stream.SetLength(0);
            stream.Seek(0, SeekOrigin.Begin);

            //var crc = CrcUdi.InitValue;
            var hdr = new byte[16];
            hdr[0] = 0x55;
            hdr[1] = 0x44;
            hdr[2] = 0x49;
            hdr[3] = 0x21;
            // size item (4,5,6,7) later
            hdr[0x08] = 0;
            hdr[0x09] = (byte)(_diskImage.CylynderCount - 1);
            hdr[0x0A] = (byte)(_diskImage.SideCount - 1);
            var hdr2len = 0;
            hdr[0x0C] = (byte)hdr2len;
            hdr[0x0D] = (byte)(hdr2len >> 8);
            hdr[0x0E] = (byte)(hdr2len >> 16);
            hdr[0x0F] = (byte)(hdr2len >> 24);
            stream.Write(hdr, 0, 16);

            if (hdr2len > 0)
            {
                byte[] hdr2 = new byte[hdr2len];
                stream.Write(hdr2, 0, hdr2len);
            }

            for (var cyl = 0; cyl < _diskImage.CylynderCount; cyl++)
            {
                for (var side = 0; side < _diskImage.SideCount; side++)
                {
                    hdr[0] = 0x00; // std MFM track
                    hdr[1] = (byte)_diskImage.GetTrackImage(cyl, side).trklen;
                    hdr[2] = (byte)(_diskImage.GetTrackImage(cyl, side).trklen >> 8);
                    stream.Write(hdr, 0, 3);
                    var image = _diskImage.GetTrackImage(cyl, side).RawImage;
                    stream.Write(image[0], 0, image[0].Length);
                    stream.Write(image[1], 0, image[1].Length);

                    //if(cyl==0&&side==0)
                    //{   
                    //   using (FileStream fdbg = new FileStream("00d.dbg", FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                    //      fdbg.Write(image[0], 0, image[0].Length);
                    //   using (FileStream fdbg = new FileStream("00c.dbg", FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
                    //      fdbg.Write(image[1], 0, image[1].Length);
                    //}
                }
            }

            // set length
            var len = stream.Length;
            hdr[0] = (byte)len;
            hdr[1] = (byte)(len >> 8);
            hdr[2] = (byte)(len >> 16);
            hdr[3] = (byte)(len >> 24);
            stream.Seek(4, SeekOrigin.Begin);
            stream.Write(hdr, 0, 4);

            // set CRC
            stream.Seek(0, SeekOrigin.Begin);
            var tmp = new byte[len];
            stream.Read(tmp, 0, (int)len);
            var crc = CrcUdi.Calculate(tmp, 0, (int)len);
            hdr[0] = (byte)crc;
            hdr[1] = (byte)(crc >> 8);
            hdr[2] = (byte)(crc >> 16);
            hdr[3] = (byte)(crc >> 24);
            stream.Write(hdr, 0, 4);
        }

        #endregion
    }
}
