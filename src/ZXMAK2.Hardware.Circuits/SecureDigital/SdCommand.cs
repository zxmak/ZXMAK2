

namespace ZXMAK2.Hardware.Circuits.SecureDigital
{
    public enum SdCommand
    {
        INVALID = -1,
        CMD0 = 0,
        GO_IDLE_STATE = CMD0,
        CMD1 = 1,
        SEND_OP_COND = CMD1,
        CMD8 = 8,
        SEND_IF_COND = CMD8,
        CMD9 = 9,
        SEND_CSD = CMD9,
        CMD10 = 10,
        SEND_CID = CMD10,
        CMD12 = 12,
        STOP_TRANSMISSION = CMD12,
        CMD16 = 16,
        SET_BLOCKLEN = CMD16,
        CMD17 = 17,
        READ_SINGLE_BLOCK = CMD17,
        CMD18 = 18,
        READ_MULTIPLE_BLOCK = CMD18,
        CMD24 = 24,
        WRITE_BLOCK = CMD24,
        CMD25 = 25,
        WRITE_MULTIPLE_BLOCK = CMD25,
        CMD55 = 55,
        APP_CMD = CMD55,
        CMD58 = 58,
        READ_OCR = CMD58,
        CMD59 = 59,
        CRC_ON_OFF = CMD59,
        ACMD41 = 41,
        SD_SEND_OP_COND = ACMD41,
    }
}
