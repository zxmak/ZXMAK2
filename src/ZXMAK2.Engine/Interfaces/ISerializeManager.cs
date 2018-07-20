using System.IO;
using System.Collections.Generic;


namespace ZXMAK2.Engine.Interfaces
{
    public interface ISerializeManager
    {
        string GetOpenExtFilter();
        string GetSaveExtFilter();
        string SaveFileName(string fileName);
        string OpenFileStream(string fileName, Stream fileStream);
        string OpenFileName(string fileName, bool wp);

        bool CheckCanOpenFileName(string fileName);
        bool CheckCanOpenFileStream(string fileName, Stream fileStream);
        bool CheckCanSaveFileName(string fileName);

        string GetDefaultExtension();

        void Clear();
        void AddSerializer(IFormatSerializer serializer);
        IFormatSerializer GetSerializer(string ext);
        IEnumerable<IFormatSerializer> GetSerializers();
    }
}
