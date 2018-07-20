using System.Xml;


namespace ZXMAK2.Engine.Interfaces
{
    public interface IBus
    {
        T FindDevice<T>() where T : class;
        
        void LoadConfigXml(XmlNode busNode);
        void SaveConfigXml(XmlNode busNode);
    }
}
