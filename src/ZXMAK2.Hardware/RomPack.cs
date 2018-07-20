using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Reflection;
using ZXMAK2.Engine;


namespace ZXMAK2.Hardware
{
    public class RomPack
    {
        private static long GetImageLength(string fileName)
        {
            var folderName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            // override
            var romsFolderName = Path.Combine(folderName, "roms");
            if (Directory.Exists(romsFolderName))
            {
                var romsFileName = Path.Combine(romsFolderName, fileName);
                if (File.Exists(romsFileName))
                {
                    return new FileInfo(romsFileName).Length;
                }
            }

            var pakFileName = Path.Combine(folderName, "ROMS.PAK");

            using (ZipLib.Zip.ZipFile zip = new ZipLib.Zip.ZipFile(pakFileName))
            {
                foreach (ZipLib.Zip.ZipEntry entry in zip)
                {
                    if (entry.IsFile &&
                       entry.CanDecompress &&
                       string.Compare(entry.Name, fileName, true) == 0)
                    {
                        return entry.Size;
                    }
                }
            }
            throw new FileNotFoundException(string.Format("ROM file not found: {0}", fileName));
        }

        private static Stream GetImageStream(string fileName)
        {
            var folderName = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            // override
            var romsFolderName = Path.Combine(folderName, "roms");
            if (Directory.Exists(romsFolderName))
            {
                var romsFileName = Path.Combine(romsFolderName, fileName);
                if (File.Exists(romsFileName))
                {
                    using (var fs = new FileStream(romsFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        return CreateStream(fs, fs.Length);
                    }
                }
            }

            var pakFileName = Path.Combine(folderName, "ROMS.PAK");

            using (ZipLib.Zip.ZipFile zip = new ZipLib.Zip.ZipFile(pakFileName))
            {
                foreach (ZipLib.Zip.ZipEntry entry in zip)
                {
                    if (entry.IsFile &&
                       entry.CanDecompress &&
                       string.Compare(entry.Name, fileName, true) == 0)
                    {
                        using (var s = zip.GetInputStream(entry))
                        {
                            return CreateStream(s, entry.Size);
                        }
                    }
                }
            }
            throw new FileNotFoundException(
                string.Format(
                    "ROM file not found: {0}", 
                    fileName));
        }

        private static Stream CreateStream(Stream stream, long length)
        {
            var fileData = new byte[length];
            stream.Read(fileData, 0, fileData.Length);
            return new MemoryStream(fileData);
        }

        public static Stream GetUlaRomStream(string romName)
        {
            var mapping = new XmlDocument();
            using (var stream = RomPack.GetImageStream("~ula.xml"))
            {
                mapping.Load(stream);
            }
            // "/Mapping/Rom"
            foreach (XmlNode romNode in mapping.DocumentElement.ChildNodes)
            {
                if (string.Compare(romNode.Name, "Rom", true) != 0)
                {
                    continue;
                }
                if (romNode.Attributes["name"] == null ||
                    romNode.Attributes["image"] == null)
                {
                    continue;
                }
                var name = romNode.Attributes["name"].InnerText;
                var image = romNode.Attributes["image"].InnerText;
                if (name != string.Empty &&
                    image != string.Empty &&
                    string.Compare(name, romName, true) == 0)
                {
                    return GetImageStream(image);
                }
            }
            throw new FileNotFoundException(string.Format("ULA ROM file not found: {0}", romName));
        }

        public static IEnumerable<RomPage> GetRomSet(String romSetName)
        {
            var list = new List<RomPage>();
            try
            {
                var mapping = new XmlDocument();
                using (var stream = RomPack.GetImageStream("~mapping.xml"))
                {
                    mapping.Load(stream);
                }
                // "/Mapping/RomSet"
                foreach (XmlNode romSetNode in mapping.DocumentElement.ChildNodes)
                {
                    if (string.Compare(romSetNode.Name, "RomSet", true) != 0)
                    {
                        continue;
                    }
                    var romSet = Utils.GetXmlAttributeAsString(romSetNode, "name", string.Empty);
                    if (romSet == string.Empty ||
                        string.Compare(romSetName, romSet, true) != 0)
                    {
                        continue;
                    }
                    // "Page"
                    foreach (XmlNode pageNode in romSetNode.ChildNodes)
                    {
                        if (string.Compare(pageNode.Name, "Page", true) != 0)
                        {
                            continue;
                        }
                        var pageName = Utils.GetXmlAttributeAsString(pageNode, "name", string.Empty);
                        var pageImage = Utils.GetXmlAttributeAsString(pageNode, "image", string.Empty);
                        if (pageName == string.Empty ||
                            pageImage == string.Empty)
                        {
                            continue;
                        }
                        var fileOffset = 0;
                        var fileLength = (int)RomPack.GetImageLength(pageImage);
                        if (pageNode.Attributes["offset"] != null)
                        {
                            fileOffset = Utils.ParseSpectrumInt(pageNode.Attributes["offset"].InnerText);
                            fileLength -= fileOffset;
                        }
                        if (pageNode.Attributes["length"] != null)
                        {
                            fileLength = Utils.ParseSpectrumInt(pageNode.Attributes["length"].InnerText);
                        }
                        var data = new byte[fileLength];
                        using (var stream = RomPack.GetImageStream(pageImage))
                        {
                            stream.Seek(fileOffset, SeekOrigin.Begin);
                            stream.Read(data, 0, data.Length);
                        }

                        list.Add(new RomPage(pageName, data));
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Load RomSet failed, romSet=\"{0}\"", romSetName);
            }
            return list;
        }

        public static IEnumerable<String> GetRomSetNames()
        {
            var list = new List<String>();
            try
            {
                var mapping = new XmlDocument();
                using (Stream stream = RomPack.GetImageStream("~mapping.xml"))
                {
                    mapping.Load(stream);
                }
                // "/Mapping/RomSet"                
                foreach (XmlNode romSetNode in mapping.DocumentElement.ChildNodes)
                {
                    if (string.Compare(romSetNode.Name, "RomSet", true) != 0)
                    {
                        continue;
                    }
                    var romSet = Utils.GetXmlAttributeAsString(romSetNode, "name", string.Empty);
                    list.Add(romSet);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return list;
        }
    }

    public class RomPage
    {
        public RomPage(String name, byte[] content)
        {
            Name = name;
            Content = content;
        }

        public String Name { get; private set; }
        public byte[] Content { get; private set; }
    }
}
