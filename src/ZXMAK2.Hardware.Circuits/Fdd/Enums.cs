using System;


namespace ZXMAK2.Hardware.Circuits.Fdd
{
    public enum WD93REG
    {
        #region Comment
        /// <summary>
        /// COMMAND/STATUS register (port #1F)
        /// </summary>
        #endregion
        CMD = 0,

        #region Comment
        /// <summary>
        /// TRACK register (port #3F)
        /// </summary>
        #endregion
        TRK = 1,
        
        #region Comment
        /// <summary>
        /// SECTOR register (port #5F)
        /// </summary>
        #endregion
        SEC = 2,
        
        #region Comment
        /// <summary>
        /// DATA register (port #7F)
        /// </summary>
        #endregion
        DAT = 3,
        
        #region Comment
        /// <summary>
        /// BETA128 register (port #FF)
        /// </summary>
        #endregion
        SYS = 4,
    }

    public enum WDSTATE : byte
    {
        S_IDLE = 0,
        S_WAIT,

        S_DELAY_BEFORE_CMD,
        S_CMD_RW,
        S_FOUND_NEXT_ID,
        S_RDSEC,
        S_READ,
        S_WRSEC,
        S_WRITE,
        S_WRTRACK,
        S_WR_TRACK_DATA,

        S_TYPE1_CMD,
        S_STEP,
        S_SEEKSTART,
        S_SEEK,
        S_VERIFY,

        S_RESET,
    }

    public enum BETA_STATUS : byte
    {
        NONE = 0x00,
        DRQ = 0x40,
        INTRQ = 0x80,
    }

    [Flags]
    public enum WD_STATUS
    {
        WDS_NONE = 0,
        WDS_BUSY = 0x01,
        WDS_INDEX = 0x02,
        WDS_DRQ = 0x02,
        WDS_TRK00 = 0x04,
        WDS_LOST = 0x04,
        WDS_CRCERR = 0x08,
        WDS_NOTFOUND = 0x10,
        WDS_SEEKERR = 0x10,
        WDS_RECORDT = 0x20,
        WDS_HEADL = 0x20,
        WDS_WRFAULT = 0x20,
        WDS_WRITEP = 0x40,
        WDS_NOTRDY = 0x80
    }

    [Flags]
    public enum WD_SYS
    {
        SYS_HLT = 0x08,
        SYS_RST = 0x04,
    }
}
