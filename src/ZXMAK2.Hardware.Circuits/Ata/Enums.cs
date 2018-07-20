

namespace ZXMAK2.Hardware.Circuits.Ata
{
    public enum RESET_TYPE 
    { 
        RESET_HARD, 
        RESET_SOFT, 
        RESET_SRST 
    }
    
    public enum ATAPI_INT_REASON : byte
    {
        INT_COD = 0x01,
        INT_IO = 0x02,
        INT_RELEASE = 0x04
    }

    public enum HD_STATUS : byte
    {
        STATUS_BSY = 0x80,
        STATUS_DRDY = 0x40,
        STATUS_DF = 0x20,
        STATUS_DSC = 0x10,
        STATUS_DRQ = 0x08,
        STATUS_CORR = 0x04,
        STATUS_IDX = 0x02,
        STATUS_ERR = 0x01,

        STATUS_NONE = 0,
    }

    public enum HD_ERROR : byte
    {
        ERR_BBK = 0x80,
        ERR_UNC = 0x40,
        ERR_MC = 0x20,
        ERR_IDNF = 0x10,
        ERR_MCR = 0x08,
        ERR_ABRT = 0x04,
        ERR_TK0NF = 0x02,
        ERR_AMNF = 0x01,

        ERR_NONE = 0,
    }

    public enum HD_CONTROL : byte
    {
        CONTROL_SRST = 0x04,
        CONTROL_nIEN = 0x02
    }

    public enum HD_STATE
    {
        S_IDLE = 0, 
        S_READ_ID,
        S_READ_SECTORS, 
        S_VERIFY_SECTORS, 
        S_WRITE_SECTORS, 
        S_FORMAT_TRACK,
        S_RECV_PACKET, 
        S_READ_ATAPI,
        S_MODE_SELECT,
    }

    public enum DEVTYPE 
    { 
        ATA_NONE, 
        ATA_FILEHDD, 
/*
        ATA_NTHDD, 
        ATA_SPTI_CD, 
        ATA_ASPI_CD,
*/ 
        ATA_FILECD 
    }
    
    public enum DEVUSAGE 
    { 
        ATA_OP_ENUM_ONLY, 
        ATA_OP_USE 
    }
}
