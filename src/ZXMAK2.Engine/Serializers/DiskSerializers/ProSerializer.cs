using ZXMAK2.Model.Disk;


namespace ZXMAK2.Serializers.DiskSerializers
{
    /// <summary>
    /// PROFI disk image 80 track with 5 x 1024kb sectors = 819200kb
    /// Track 000 Side 0 1,2,3,4,9
    /// Track 001 Side 1 1,2,3,4,5
    /// ...
    /// Track 159 Side 1 1,2,3,4,5
    /// </summary>
    public class ProSerializer : SectorImageSerializerBase
    {
        public ProSerializer(DiskImage diskImage)
            : base(diskImage)
        {
        }
        
        public override string FormatExtension
        {
            get { return "PRO"; }
        }

        protected override int SectorSizeCode
        {
            get { return 3; }   // 0x400
        }
        
        protected override int[] GetSectorMap(int cyl, int head)
        {
            if (cyl == 0 && head == 0)
            {
                return new[] { 1, 2, 3, 4, 9 };
            }
            else
            {
                return new[] { 1, 2, 3, 4, 5 };
            }
        }
    }
}
