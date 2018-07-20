

namespace ZXMAK2.Model.Disk
{
    public class SectorHeader
    {
        public byte c, s, n, l;
        public ushort crc1, crc2;
        public bool c1, c2;     // flags: correct CRCs in address and data

        public int idOffset;    // offset to id in track image
        public int dataOffset;  // offset to sector data in track image, -1 for no data
        public int datlen;      // data length

        public long idTime;    // time from begin track to id
        public long dataTime;  // time from begin track to data
    }
}
