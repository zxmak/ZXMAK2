using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using ZXMAK2.Engine.Interfaces;

namespace ZXMAK2.Serializers.ScreenshotSerializers
{
    public class PngSerializer : ScreenshotSerializerBase
    {
        public PngSerializer(IUlaDevice ulaDevice)
            : base(ulaDevice)
        {
        }

        public override string FormatExtension { get { return "PNG"; } }

        protected override void Save(Stream stream, Bitmap bmp)
        {
            bmp.Save(stream, ImageFormat.Png);
            
            //ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            //System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
            //EncoderParameters eps = new EncoderParameters(1);
            //eps.Param[0] = new EncoderParameter(encoder, 500L);
            //bmp.Save(stream, jgpEncoder, eps);
        }

        //private static ImageCodecInfo GetEncoder(ImageFormat format)
        //{
        //    ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
        //    foreach (ImageCodecInfo codec in codecs)
        //        if (codec.FormatID == format.Guid)
        //            return codec;
        //    return null;
        //}
    }
}
