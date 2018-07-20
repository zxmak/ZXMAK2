using System;
using System.Xml;
using System.Globalization;
using System.IO;


namespace ZXMAK2.Hardware.Circuits
{
    internal class Utils
    {
        #region Xml Helpers

        public static void SetXmlAttribute(XmlNode node, string name, Int32 value)
        {
            var attr = node.OwnerDocument.CreateAttribute(name);
            attr.Value = value.ToString(CultureInfo.InvariantCulture);
            node.Attributes.Append(attr);
        }

        public static void SetXmlAttribute(XmlNode node, string name, UInt32 value)
        {
            var attr = node.OwnerDocument.CreateAttribute(name);
            attr.Value = value.ToString(CultureInfo.InvariantCulture);
            node.Attributes.Append(attr);
        }

        public static void SetXmlAttribute(XmlNode node, string name, string value)
        {
            var attr = node.OwnerDocument.CreateAttribute(name);
            attr.Value = value;
            node.Attributes.Append(attr);
        }

        public static void SetXmlAttribute(XmlNode node, string name, bool value)
        {
            var attr = node.OwnerDocument.CreateAttribute(name);
            attr.Value = value.ToString();
            node.Attributes.Append(attr);
        }

        public static Int32 GetXmlAttributeAsInt32(XmlNode itemNode, string name, int defValue)
        {
            var result = defValue;
            if (itemNode.Attributes[name] != null)
            {
                if (Int32.TryParse(itemNode.Attributes[name].InnerText, out result))
                {
                    return result;
                }
            }
            return defValue;
        }

        public static UInt32 GetXmlAttributeAsUInt32(XmlNode itemNode, string name, uint defValue)
        {
            var result = defValue;
            if (itemNode.Attributes[name] != null)
            {
                if (UInt32.TryParse(itemNode.Attributes[name].InnerText, out result))
                {
                    return result;
                }
            }
            return defValue;
        }

        public static bool GetXmlAttributeAsBool(XmlNode itemNode, string name, bool defValue)
        {
            bool result = defValue;
            if (itemNode.Attributes[name] != null)
                if (bool.TryParse(itemNode.Attributes[name].InnerText, out result))
                    return result;
            return defValue;
        }

        public static String GetXmlAttributeAsString(XmlNode itemNode, string name, string defValue)
        {
            if (itemNode.Attributes[name] != null)
                return itemNode.Attributes[name].InnerText;
            return defValue;
        }

        #endregion


        public static string GetFullPathFromRelativePath(string relFileName, string rootPath)
        {
            // TODO: rewrite with safe version http://stackoverflow.com/questions/275689/how-to-get-relative-path-from-absolute-path
            string current = Directory.GetCurrentDirectory();
            try
            {
                Directory.SetCurrentDirectory(rootPath);
                return Path.GetFullPath(relFileName);
            }
            finally
            {
                Directory.SetCurrentDirectory(current);
            }
        }
    }
}
