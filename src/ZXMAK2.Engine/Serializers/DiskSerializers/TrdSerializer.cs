using System;
using System.IO;

using ZXMAK2.Model.Disk;


namespace ZXMAK2.Serializers.DiskSerializers
{
    public class TrdSerializer : FormatSerializer
    {
        #region private data

        private DiskImage _diskImage;

        #endregion


        public TrdSerializer(DiskImage diskImage)
        {
            _diskImage = diskImage;
        }


        #region FormatSerializer

        public override string FormatGroup { get { return "Disk images"; } }
        public override string FormatName { get { return "TRD disk image"; } }
        public override string FormatExtension { get { return "TRD"; } }

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
            saveToStream(stream);
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



        private void loadFromStream(Stream stream)
        {
            int cylCount = (int)stream.Length / (256 * 16 * 2);
            if ((stream.Length % (256 * 16 * 2)) > 0)
                _diskImage.SetPhysics(cylCount + 1, 2);
            else
                _diskImage.SetPhysics(cylCount, 2);
            _diskImage.FormatTrdos();

            int i = 0;
            while (stream.Position < stream.Length)
            {
                byte[] snbuf = new byte[256];
                stream.Read(snbuf, 0, 256);
                _diskImage.WriteLogicalSector(i >> 13, (i >> 12) & 1, ((i >> 8) & 0x0F) + 1, snbuf);
                i += 0x100;
            }
        }

        private void saveToStream(Stream stream)
        {
            for (int i = 0; i < 256 * 16 * 2 * _diskImage.CylynderCount; i += 0x100)
            {
                byte[] snbuf = new byte[256];
                _diskImage.ReadLogicalSector(i >> 13, (i >> 12) & 1, ((i >> 8) & 0x0F) + 1, snbuf);
                stream.Write(snbuf, 0, 256);
            }
        }
    }
}
