

namespace ZXMAK2.Hardware.Circuits.SecureDigital
{
    public enum SdState
    {
        IDLE,
        RD_ARG,
        RD_CRC,
        R1,
        R1b,
        R2,
        R3,
        R7,
        STARTBLOCK,
        DELAY_S,
        WR_DATA,
        WR_CRC16_1,
        WR_CRC16_2,
        RD_DATA_SIG,
        RD_DATA,
        RD_DATA_MUL,
        RD_CRC16_1,
        RD_CRC16_2,
        WR_DATA_RESP,
        RD_DATA_SIG_MUL
    }
}
