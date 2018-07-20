using System;
using System.Xml;
using System.IO;
using System.Reflection;


namespace ZXMAK2.Hardware.Circuits.Ata
{
    public class AtaDeviceInfo
    {
        private const string DefaultSerial = "00000000001234567890";
        private const string DefaultModel = "ZXMAK2 HDD IMAGE";
        
        public string FileName { get; private set; }
        public uint Cylinders { get; private set; }
        public uint Heads { get; private set; }
        public uint Sectors { get; private set; }
        public uint Lba { get; private set; }
        public bool ReadOnly { get; private set; }
        public bool IsCdrom { get; private set; }

        public string SerialNumber { get; private set; }        // 20 chars
        public string FirmwareRevision { get; private set; }    // 8 chars
        public string ModelNumber { get; private set; }         // 40 chars

        
        public AtaDeviceInfo()
        {
            Cylinders = 20; 
            Heads = 16;
            Sectors = 63; 
            Lba = 20160;
            ReadOnly = true;
            IsCdrom = false;
            SerialNumber = DefaultSerial;
            FirmwareRevision = GetVersion();
            ModelNumber = DefaultModel;
        }
        
        public void Save(string fileName)
        {
            XmlDocument xml = new XmlDocument();
            XmlNode root = xml.AppendChild(xml.CreateElement("IdeDiskDescriptor"));
            XmlNode imageNode = root.AppendChild(xml.CreateElement("Image"));
            string imageFile = FileName ?? string.Empty;
            if (imageFile != string.Empty &&
                Path.GetDirectoryName(imageFile) == Path.GetDirectoryName(fileName))
            {
                imageFile = Path.GetFileName(imageFile);
            }
            Utils.SetXmlAttribute(imageNode, "fileName", imageFile);
            Utils.SetXmlAttribute(imageNode, "isReadOnly", ReadOnly);
            Utils.SetXmlAttribute(imageNode, "isCdrom", IsCdrom);
            Utils.SetXmlAttribute(imageNode, "serial", SerialNumber);
            Utils.SetXmlAttribute(imageNode, "revision", FirmwareRevision);
            Utils.SetXmlAttribute(imageNode, "model", ModelNumber);
            XmlNode geometryNode = root.AppendChild(xml.CreateElement("Geometry"));
            Utils.SetXmlAttribute(geometryNode, "cylinders", Cylinders);
            Utils.SetXmlAttribute(geometryNode, "heads", Heads);
            Utils.SetXmlAttribute(geometryNode, "sectors", Sectors);
            Utils.SetXmlAttribute(geometryNode, "lba", Lba);
            xml.Save(fileName);
        }

        public void Load(string fileName)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(fileName);
            XmlNode root = xml["IdeDiskDescriptor"];
            XmlNode imageNode = root["Image"];
            XmlNode geometryNode = root["Geometry"];
            FileName = Utils.GetXmlAttributeAsString(imageNode, "fileName", FileName ?? string.Empty);
            if (FileName != string.Empty && !Path.IsPathRooted(FileName))
                FileName = Utils.GetFullPathFromRelativePath(FileName, Path.GetDirectoryName(fileName));
            SerialNumber = Utils.GetXmlAttributeAsString(imageNode, "serial", SerialNumber);
            FirmwareRevision = Utils.GetXmlAttributeAsString(imageNode, "revision", FirmwareRevision);
            ModelNumber = Utils.GetXmlAttributeAsString(imageNode, "model", ModelNumber);
            IsCdrom = Utils.GetXmlAttributeAsBool(imageNode, "isCdrom", false);
            ReadOnly = Utils.GetXmlAttributeAsBool(imageNode, "isReadOnly", true);
            Cylinders = Utils.GetXmlAttributeAsUInt32(geometryNode, "cylinders", Cylinders);
            Heads = Utils.GetXmlAttributeAsUInt32(geometryNode, "heads", Heads);
            Sectors = Utils.GetXmlAttributeAsUInt32(geometryNode, "sectors", Sectors);
            Lba = Utils.GetXmlAttributeAsUInt32(geometryNode, "lba", Lba);
        }

        private static string GetVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.Revision.ToString();
        }
    }
}
