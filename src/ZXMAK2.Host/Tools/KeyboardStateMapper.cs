using System;
using System.Xml;
using System.Collections.Generic;


namespace ZXMAK2.Host.Entities.Tools
{
    public class KeyboardStateMapper<T>
        where T : struct
    {
        private readonly Dictionary<Key, T> m_map = new Dictionary<Key, T>();

        public IEnumerable<Key> Keys { get { return m_map.Keys; } }


        public T this[Key key]
        {
            get { return m_map[key]; }
        }


        #region Map

        public void LoadMap(string fileName)
        {
            var xml = new XmlDocument();
            xml.Load(fileName);
            LoadMapFromXml(xml);
        }
        
        public void LoadMapFromString(string xmlString)
        {
            var xml = new XmlDocument();
            xml.LoadXml(xmlString);
            LoadMapFromXml(xml.DocumentElement);
        }
        
        public void LoadMapFromXml(XmlNode xml)
        {
            m_map.Clear();
            try
            {
                foreach (XmlNode node in xml.ChildNodes)
                {
                    if (string.Compare(node.Name, "Key", true) != 0 ||
                        node.Attributes["Name"] == null ||
                        node.Attributes["Value"] == null)
                    {
                        continue;
                    }
                    var name = node.Attributes["Name"].Value.Trim();
                    var value = node.Attributes["Value"].Value.Trim();
                    if (name == string.Empty || value == string.Empty)
                    {
                        continue;
                    }
                    try
                    {
                        Key emuKey = (Key)Enum.Parse(typeof(Key), name);
                        T devKey = (T)Enum.Parse(typeof(T), value);
                        if (m_map.ContainsKey(emuKey))
                        {
                            Logger.Warn(
                                "Duplicate keyboard mapping \"{0}\", \"{1}\"",
                                name,
                                value);
                            continue;
                        }
                        m_map[emuKey] = devKey;
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(
                            "Invalid keyboard mapping \"{0}\", \"{1}\" ({2})",
                            name,
                            value,
                            ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion Map
    }
}
