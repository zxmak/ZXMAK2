using System;
using System.IO;

using ZXMAK2.Model.Disk;
using ZXMAK2.Serializers.DiskSerializers;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Entities;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Serializers
{
    public class DiskLoadManager : SerializeManager
    {
        public DiskLoadManager(DiskImage diskImage)
        {
            AddSerializer(new UdiSerializer(diskImage));
            AddSerializer(new FdiSerializer(diskImage));
            AddSerializer(new Td0Serializer(diskImage));
            AddSerializer(new TrdSerializer(diskImage));
            AddSerializer(new ImgSerializer(diskImage));
            AddSerializer(new ProSerializer(diskImage));
            AddSerializer(new QdiSerializer(diskImage));
            AddSerializer(new SclSerializer(diskImage));
            AddSerializer(new HobetaSerializer(diskImage));

            diskImage.LoadDisk += LoadDisk;
            diskImage.SaveDisk += SaveDisk;
        }

        private void LoadDisk(DiskImage image, bool readOnly)
        {
            if (!string.IsNullOrEmpty(image.FileName))
            {
                OpenFileName(image.FileName, readOnly);
            }
            else
            {
                image.SetPhysics(80, 2);
                image.FormatTrdos();
            }
        }

        private void SaveDisk(DiskImage image)
        {
            if (image.IsWP)
            {
                Logger.Error("Write protected disk was changed! Autosave canceled");
                return;
            }

            var fileName = image.FileName;
            if (!string.IsNullOrEmpty(fileName))
            {
                var serializer = GetSerializer(Path.GetExtension(fileName));
                if (serializer == null || !serializer.CanSerialize)
                    fileName = string.Empty;
            }
            if (string.IsNullOrEmpty(fileName))
            {
                string folderName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                folderName = Path.Combine(folderName, "Images");
                for (int i = 0; i < 10001; i++)
                {
                    fileName = string.Format("zxmak2image{0:D3}{1}", i, GetDefaultExtension());
                    fileName = Path.Combine(folderName, fileName);
                    if (!File.Exists(fileName))
                        break;
                    fileName = string.Empty;
                }
            }

            var qr = DlgResult.No;
            var service = Locator.Resolve<IUserQuery>();
            if (service != null)
            {
                var msg = string.Format(
                    "Do you want to save disk changes to {0}",
                    Path.GetFileName(fileName));
                qr = service.Show(
                    msg, 
                    "Attention!", 
                    DlgButtonSet.YesNo,
                    DlgIcon.Question);
            }
            if (qr == DlgResult.Yes)
            {
                //if (Path.GetExtension(_fileName).ToUpper() == ".SCL")
                //{
                //   _fileName = Path.ChangeExtension(_fileName, ".TRD");
                //   if (File.Exists(_fileName))
                //      _fileName = string.Empty;
                //}

                if (string.IsNullOrEmpty(fileName))
                {
                    Locator.Resolve<IUserMessage>()
                        .Warning("Can't save disk image!\nNo space on HDD!");
                }
                else
                {
                    string folderName = Path.GetDirectoryName(fileName);
                    if (!Directory.Exists(folderName))
                        Directory.CreateDirectory(folderName);
                    image.FileName = fileName;
                    SaveFileName(image.FileName);
                    Locator.Resolve<IUserMessage>().Info(
                        "Disk image successfuly saved!\n{0}", 
                        image.FileName);
                }
            }
        }
    }
}
