using System;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Serializers.ScreenshotSerializers
{
    public class JpgSerializer : ScreenshotSerializerBase
    {
		public JpgSerializer(IUlaDevice ulaDevice)
            : base (ulaDevice)
		{
		}

        public override string FormatExtension { get { return "JPG"; } }

        protected override void Save(Stream stream, Bitmap bmp)
        {
            var jpgEncoder = ImageCodecInfo.GetImageDecoders()
                .FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
            if (jpgEncoder == null)
            {
                Locator.Resolve<IUserMessage>().Error("Could not find JPEG encoder!");
                return;
            }
            using (var eps = new EncoderParameters(1))
            {
                eps.Param[0] = new EncoderParameter(Encoder.Quality, 500L);
                bmp.Save(stream, jpgEncoder, eps);
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.FirstOrDefault(codec => codec.FormatID == format.Guid); 
        }
    }
}
