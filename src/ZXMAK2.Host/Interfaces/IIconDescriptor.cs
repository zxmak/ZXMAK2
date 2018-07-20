using System;
using System.IO;
using System.Drawing;


namespace ZXMAK2.Host.Interfaces
{
    public interface IIconDescriptor
    {
        string Name { get; }
        bool Visible { get; }
        Size Size { get; }

        Stream GetImageStream();
    }
}
