using System;
using System.IO;
using System.Text;

using ZXMAK2.Model.Disk;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Entities;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Serializers.DiskSerializers
{
    public class SclSerializer : HobetaSerializer
    {
        public SclSerializer(DiskImage diskImage)
            : base(diskImage)
        {
        }


        #region FormatSerializer

        public override string FormatName { get { return "SCL disk image"; } }
        public override string FormatExtension { get { return "SCL"; } }
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


        private void loadFromStream(Stream stream)
        {
            if (stream.Length < 9 || stream.Length > 2544 * 256 + 9)
            {
                Locator.Resolve<IUserMessage>()
                    .Error("SCL loader\n\nInvalid SCL file size!");
                return;
            }

            byte[] fbuf = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(fbuf, 0, (int)stream.Length);

            // TODO:check first 8 bytes "SINCLAIR"
            if (Encoding.ASCII.GetString(fbuf, 0, 8) != "SINCLAIR")
            {
                Locator.Resolve<IUserMessage>()
                    .Error("SCL loader\n\nCorrupted SCL file!");
                return;
            }

            //            if (fbuf.Length >= (9 + (0x100 + 14) * fbuf[8]))  
            //               throw new InvalidDataException("Corrupt *.SCL file!");

            bool needFormat = true;
            if (_diskImage.IsConnected && _diskImage.Present)
            {
                var service = Locator.Resolve<IUserQuery>();
                DlgResult dlgRes = DlgResult.No;
                if (service != null)
                {
                    dlgRes = service.Show(
                        "Do you want to append file(s) to existing disk?\n\nPlease click 'Yes' to append file(s).\nOr click 'No' to create new disk...",
                        "SCL loader",
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

            int size;
            for (int i = size = 0; i < fbuf[8]; i++)
                size += fbuf[9 + 14 * i + 13];
            if (size > 2544)
            {
                byte[] s9 = new byte[256];
                _diskImage.ReadLogicalSector(0, 0, 9, s9);
                s9[0xE5] = (byte)size;              // free sec
                s9[0xE6] = (byte)(size >> 8);
                _diskImage.WriteLogicalSector(0, 0, 9, s9);
            }
            int dataIndex = 9 + 14 * fbuf[8];
            for (int i = 0; i < fbuf[8]; i++)
            {
                if (!addFile(fbuf, 9 + 14 * i, dataIndex))
                    return;
                dataIndex += fbuf[9 + 14 * i + 13] * 0x100;
            }
        }
    }
}
