using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Reflection;


namespace ZXMAK2.Engine
{
    public class MachinesConfig
    {
        private XmlDocument m_config = new XmlDocument();

        public void Load()
        {
            var folderName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fileName = Path.Combine(folderName, "machines.config");
            if (File.Exists(fileName))
            {
                Load(fileName);
            }
        }

        public void Load(string fileName)
        {
            m_config.Load(fileName);
        }

        public IEnumerable<string> GetNames()
        {
            var names = m_config.DocumentElement.ChildNodes
                .OfType<XmlNode>()
                .Where(node => node.Name == "Bus")
                .Select(node => GetAttrString(node, "name"))
                .Where(v => !string.IsNullOrEmpty(v));
            return names;
        }

        public XmlNode GetConfig(string name)
        {
            var configNode = m_config.DocumentElement.ChildNodes
                .OfType<XmlNode>()
                .FirstOrDefault(node => node.Name=="Bus" && GetAttrString(node, "name")==name);
            return configNode;
        }

        public XmlNode GetDefaultConfig()
        {
            var configNode = m_config.DocumentElement.ChildNodes
                .OfType<XmlNode>()
                .FirstOrDefault(node => GetAttrBool(node, "isDefault", false));
            if (configNode != null)
            {
                return configNode;
            }
            var firstName = GetNames().FirstOrDefault();
            if (firstName == null)
            {
                return null;
            }
            return GetConfig(firstName);
        }

        private static string GetAttrString(XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr == null)
            {
                return null;
            }
            return attr.InnerText;
        }

        private static bool GetAttrBool(XmlNode node, string name, bool defValue)
        {
            var attr = node.Attributes[name];
            if (attr == null)
            {
                return defValue;
            }
            return string.Compare(attr.InnerText, "true", true)==0;
        }
    }
}
