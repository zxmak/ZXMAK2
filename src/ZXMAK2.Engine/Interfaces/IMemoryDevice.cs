using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Engine.Interfaces
{
    public interface IMemoryDevice
    {
        byte RDMEM_DBG(ushort addr);
        void WRMEM_DBG(ushort addr, byte value);

        byte[][] RamPages { get; }
        byte[][] RomPages { get; }
        int GetRomIndex(RomId rom);

        byte CMR0 { get; set; }
        byte CMR1 { get; set; }

        bool DOSEN { get; set; }
        bool SYSEN { get; set; }

        #region Comment
        /// <summary>
        /// Returns true when memory should be saved to 48K snapshot (save RAM pages from Map48 only)
        /// </summary>
        #endregion
        bool IsMap48 { get; }
        int[] Map48 { get; }

        #region Comment
        /// <summary>
        /// Returns True when ROM 48 is mapped to window #0000...#3FFF
        /// </summary>
        #endregion
        bool IsRom48 { get; }
    }
}
