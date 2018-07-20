using ZXMAK2.Model.Disk;


namespace ZXMAK2.Serializers.DiskSerializers
{
    /// <summary>
    /// QUORUM disk image 80 track with 5 x 1024kb sectors = 819200kb
    /// Track 000 Side 0 1,2,3,4,5
    /// Track 001 Side 1 1,2,3,4,5
    /// ...
    /// Track 159 Side 1 1,2,3,4,5
    /// </summary>
    public class QdiSerializer : SectorImageSerializerBase
    {
        public QdiSerializer(DiskImage diskImage)
            : base(diskImage)
        {
        }

        public override string FormatExtension
        {
            get { return "QDI"; }
        }

        protected override int SectorSizeCode
        {
            get { return 3; }
        }

        protected override int[] GetSectorMap(int cyl, int head)
        {
            return new[] { 1, 2, 3, 4, 5 };
        }
    }
}
