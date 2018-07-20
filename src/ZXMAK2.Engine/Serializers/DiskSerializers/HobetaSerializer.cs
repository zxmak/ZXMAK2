using System;
using System.IO;

using ZXMAK2.Model.Disk;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Entities;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Serializers.DiskSerializers
{
    public class HobetaSerializer : FormatSerializer
    {
        #region private data

        protected DiskImage _diskImage;

        #endregion


        public HobetaSerializer(DiskImage diskImage)
        {
            _diskImage = diskImage;
        }


        #region FormatSerializer

        public override string FormatGroup { get { return "Disk images"; } }
        public override string FormatName { get { return "Hobeta disk image"; } }
        public override string FormatExtension { get { return "$"; } }

        public override bool CanDeserialize { get { return true; } }

        public override void Deserialize(Stream stream)
        {
            loadFromStream(stream);
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
        private void loadFromStream(Stream stream)
        {
            if (stream.Length < 15)
            {
                Locator.Resolve<IUserMessage>()
                    .Error("HOBETA loader\n\nInvalid HOBETA file size");
                return;
            }

            byte[] fbuf = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(fbuf, 0, (int)stream.Length);

            if (fbuf[14] * 256 + 17 != fbuf.Length || fbuf[13] != 0 || fbuf[14] == 0)
            {
                Locator.Resolve<IUserMessage>()
                    .Error("HOBETA loader\n\nInvalid HOBETA file!");
                return;
            }

            bool needFormat = true;
            if (_diskImage.IsConnected && _diskImage.Present)
            {
                var service = Locator.Resolve<IUserQuery>();
                var dlgRes = DlgResult.No;
                if (service != null)
                {
                    dlgRes = service.Show(
                        "Do you want to append file(s) to existing disk?\n\nPlease click 'Yes' to append file(s).\nOr click 'No' to create new disk...",
                        "HOBETA loader",
                        DlgButtonSet.YesNoCancel,
                        DlgIcon.Question);
                }
                if (dlgRes == DlgResult.Cancel)
                    return;
                if (dlgRes == DlgResult.Yes)
                    needFormat = false;
            }
            if (needFormat)
            {
                _diskImage.Format();
            }

            fbuf[13] = fbuf[14];
            addFile(fbuf, 0, 0x11);
            //_diskImage.FileName = "UNKNOWN.$Z";
        }

        protected bool addFile(byte[] buf, int hdrIndex, int dataIndex)
        {
            byte[] s9 = new byte[256];
            _diskImage.ReadLogicalSector(0, 0, 9, s9);
            int len = buf[hdrIndex + 13];
            int pos = s9[0xE4] * 0x10;

            byte[] dir = new byte[256];
            _diskImage.ReadLogicalSector(0, 0, 1 + pos / 0x100, dir);
            if ((s9[0xE5] | (s9[0xE6] << 8)) < len)   // disk full
            {
                Locator.Resolve<IUserMessage>()
                    .Error("HOBETA loader\n\nDisk full! Create empty disk and repeat operation!");
                return false;
            }
            for (int i = 0; i < 14; i++)
                dir[(pos & 0xFF) + i] = buf[hdrIndex + i];
            ushort x = (ushort)(s9[0xE1] | (s9[0xE2] << 8));
            dir[(pos & 0xFF) + 14] = (byte)x;
            dir[(pos & 0xFF) + 15] = (byte)(x >> 8);

            _diskImage.WriteLogicalSector(0, 0, 1 + pos / 0x100, dir);

            pos = s9[0xE1] + 16 * s9[0xE2];
            s9[0xE1] = (byte)((pos + len) & 0x0F);
            s9[0xE2] = (byte)((pos + len) >> 4);
            s9[0xE4]++;

            x = (ushort)(s9[0xE5] | (s9[0xE6] << 8));
            x -= (ushort)len;
            s9[0xE5] = (byte)x;
            s9[0xE6] = (byte)(x >> 8);

            _diskImage.WriteLogicalSector(0, 0, 9, s9);

            // goto next track
            for (int i = 0; i < len; i++, pos++)
            {
                for (int j = 0; j < 0x100 && (dataIndex + i * 0x100 + j) < buf.Length; j++)
                    s9[j] = buf[dataIndex + i * 0x100 + j];
                _diskImage.WriteLogicalSector(pos / 32, (pos / 16) & 1, (pos & 0x0F) + 1, s9);
            }
            return true;
        }
        #endregion
    }
}
