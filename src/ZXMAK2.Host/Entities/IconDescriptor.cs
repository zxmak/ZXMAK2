using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using ZXMAK2.Host.Interfaces;

namespace ZXMAK2.Host.Entities
{
    public class IconDescriptor : IIconDescriptor
	{
		private readonly byte[] m_iconData;

        public string Name { get; private set; }
        public Size Size { get; private set; }
		public bool Visible { get; set; }

        public IconDescriptor(string iconName, Image iconImage)
        {
            Name = iconName;
            Size = iconImage.Size;
            using (var stream = new MemoryStream())
            {
                iconImage.Save(stream, ImageFormat.Png);
                m_iconData = stream.ToArray();
            }
        }
        
        public IconDescriptor(string iconName, Stream iconStream)
		{
            if (iconStream == null)
            {
                throw new FileNotFoundException(
                    string.Format(
                        "Icon stream '{0}' not found",
                        iconName));
            }
            Name = iconName;
			m_iconData = new byte[iconStream.Length];
			iconStream.Read(m_iconData, 0, m_iconData.Length);
			using (var stream = GetImageStream())
            using (var bitmap = new Bitmap(stream))
            {
                Size = bitmap.Size;
            }
		}

		public Stream GetImageStream()
		{
			return new MemoryStream(m_iconData);
		}
	}
}
