using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Serializers.ScreenshotSerializers
{
    public class BmpSerializer : ScreenshotSerializerBase
    {
        public BmpSerializer(IUlaDevice ulaDevice)
            : base(ulaDevice)
        {
        }

        public override string FormatExtension { get { return "BMP"; } }

        protected override void Save(Stream stream, Bitmap bmp)
        {
            bmp.Save(stream, ImageFormat.Bmp);
        }
    }
}
