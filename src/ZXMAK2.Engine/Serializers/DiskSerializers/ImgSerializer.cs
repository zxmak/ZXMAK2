// IMG serializer
// Author: zx.pk.ru\Дмитрий
using ZXMAK2.Model.Disk;


namespace ZXMAK2.Serializers.DiskSerializers
{
    /// <summary>
    /// SPRINTER disk image
    /// </summary>
    public class ImgSerializer : SectorImageSerializerBase
    {
        public ImgSerializer(DiskImage diskImage)
            : base(diskImage)
        {
        }
        
        public override string FormatExtension
        {
            get { return "IMG"; }
        }
        
        protected override int SectorSizeCode
        {
            get { return 2; }
        }

        protected override int[] GetSectorMap(int cyl, int head)
        {
            return new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
        }
    }
}
