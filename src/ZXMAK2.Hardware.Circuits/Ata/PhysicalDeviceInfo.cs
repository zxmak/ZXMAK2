

namespace ZXMAK2.Hardware.Circuits.Ata
{
    public class PhysicalDeviceInfo
    {
        public DEVTYPE type;
        public DEVUSAGE usage;
        public uint hdd_size;
        public uint spti_id;
        public uint adapterid, targetid; // ASPI
        public byte[] idsector = new byte[512];
        public string filename;
        public string viewname;
    }
}
