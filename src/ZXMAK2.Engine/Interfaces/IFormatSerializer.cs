using System.IO;


namespace ZXMAK2.Engine.Interfaces
{
    public interface IFormatSerializer
    {
        string FormatGroup { get; }
        string FormatExtension { get; }
        string FormatName { get; }
        bool CanDeserialize { get; }
        bool CanSerialize { get; }

        void Deserialize(Stream stream);
        void Serialize(Stream stream);

        void SetSource(string fileName);
        void SetReadOnly(bool readOnly);
    }
}
