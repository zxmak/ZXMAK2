using System;
using System.IO;


namespace ZXMAK2.Hardware.Circuits.SecureDigital
{
    /// <summary>
    /// SD Card emulator
    /// Written by ZEK
    /// </summary>
    public class SdCard
    {
        #region Fields

        private readonly byte[] cid;
        private readonly byte[] csd;
        private readonly byte[] buff;

        private SdState currState;
        private SdCommand cmd;

        private UInt32 argCnt;
        private UInt32 ocrCnt;
        private UInt32 r7_Cnt;


        private UInt32 cidCnt;
        private UInt32 csdCnt;

        private bool appCmd;

        private int dataBlockLen;
        private UInt32 dataCnt;
        private UInt32 wrPos;

        private UInt32 arg;

        private FileStream fstream;

        #endregion Fields


        public SdCard()
        {
            cid = new byte[16] { 0x00, (byte)'U', (byte)'S', (byte)'U', (byte)'S', (byte)'3', (byte)'7', (byte)'6', 0x03, 0x12, 0x34, 0x56, 0x78, 0x00, 0xC1, 0x0F };
            csd = new byte[16] { 0x00, 0x0E, 0x00, 0x32, 0x5B, 0x59, 0x03, 0xFF, 0xED, 0xB7, 0xBF, 0xBF, 0x06, 0x40, 0x00, 0xF5 };

            buff = new byte[4096];
        }

        public void Open(string fname)
        {
            if (fstream != null)
                Close();

            if (File.Exists(fname))
            {
                fstream = File.Open(fname, FileMode.Open, FileAccess.ReadWrite);
                Reset();
            }
        }

        public void Close()
        {
            if (fstream != null)
            {
                fstream.Flush();
                fstream.Close();
                fstream.Dispose();
                fstream = null;
            }
        }

        public void Reset()
        {
            currState = SdState.IDLE;
            argCnt = 0;
            cmd = SdCommand.INVALID;
            dataBlockLen = 512;
            dataCnt = 0;

            csdCnt = 0;
            cidCnt = 0;
            ocrCnt = 0;
            r7_Cnt = 0;

            appCmd = false;
            wrPos = UInt32.MaxValue;
        }

        public void Wr(byte val)
        {
            SdState NextState = SdState.IDLE;

            if (fstream == null)
                return;

            switch (currState)
            {
                case SdState.IDLE:
                    {
                        if ((val & 0xC0) != 0x40)
                            break;

                        cmd = (SdCommand)(val & 0x3F);
                        if (!appCmd)
                        {
                            switch (cmd)
                            {

                                case SdCommand.SEND_CSD:
                                    csdCnt = 0;
                                    break;

                                case SdCommand.SEND_CID:
                                    cidCnt = 0;
                                    break;

                                case SdCommand.READ_OCR:
                                    ocrCnt = 0;
                                    break;

                                case SdCommand.SEND_IF_COND:
                                    r7_Cnt = 0;
                                    break;
                            }
                        }
                        NextState = SdState.RD_ARG;
                        argCnt = 0;
                    }
                    break;

                case SdState.RD_ARG:
                    NextState = SdState.RD_ARG;
                    SetByte(ref arg, 3 - argCnt++, val);

                    if (argCnt == 4)
                    {
                        if (!appCmd)
                        {
                            switch (cmd)
                            {
                                case SdCommand.SET_BLOCKLEN:
                                    if (arg <= 4096) dataBlockLen = (int)arg;
                                    break;

                                case SdCommand.READ_SINGLE_BLOCK:
                                    fstream.Seek(arg, SeekOrigin.Begin);
                                    fstream.Read(buff, 0, dataBlockLen);

                                    break;

                                case SdCommand.READ_MULTIPLE_BLOCK:

                                    fstream.Seek(arg, SeekOrigin.Begin);
                                    fstream.Read(buff, 0, dataBlockLen);
                                    break;

                                case SdCommand.WRITE_BLOCK:

                                    break;

                                case SdCommand.WRITE_MULTIPLE_BLOCK:
                                    wrPos = arg;

                                    break;
                            }
                        }

                        NextState = SdState.RD_CRC;
                        argCnt = 0;
                    }
                    break;

                case SdState.RD_CRC:

                    NextState = GetRespondType();
                    break;

                case SdState.RD_DATA_SIG:
                    if (val == 0xFE) // Проверка сигнатуры данных
                    {
                        dataCnt = 0;
                        NextState = SdState.RD_DATA;
                    }
                    else
                        NextState = SdState.RD_DATA_SIG;
                    break;

                case SdState.RD_DATA_SIG_MUL:
                    switch (val)
                    {
                        case 0xFC: // Проверка сигнатуры данных

                            dataCnt = 0;
                            NextState = SdState.RD_DATA_MUL;
                            break;
                        case 0xFD: // Окончание передачи

                            dataCnt = 0;
                            NextState = SdState.IDLE;
                            break;
                        default:
                            NextState = SdState.RD_DATA_SIG_MUL;
                            break;
                    }
                    break;

                case SdState.RD_DATA: // Прием данных в буфер
                    {

                        buff[dataCnt++] = val;
                        NextState = SdState.RD_DATA;
                        if (dataCnt == dataBlockLen) // Запись данных в SD карту
                        {
                            dataCnt = 0;
                            fstream.Seek(arg, SeekOrigin.Begin);
                            fstream.Write(buff, 0, dataBlockLen);


                            NextState = SdState.WR_DATA_RESP;
                        }
                    }
                    break;

                case SdState.RD_DATA_MUL: // Прием данных в буфер
                    {

                        buff[dataCnt++] = val;
                        NextState = SdState.RD_DATA_MUL;
                        if (dataCnt == dataBlockLen) // Запись данных в SD карту
                        {
                            dataCnt = 0;

                            fstream.Seek(wrPos, SeekOrigin.Begin);
                            fstream.Write(buff, 0, dataBlockLen);


                            wrPos += (uint)dataBlockLen;
                            NextState = SdState.RD_DATA_SIG_MUL;
                        }
                    }
                    break;

                case SdState.RD_CRC16_1: // Чтение старшего байта CRC16
                    NextState = SdState.RD_CRC16_2;
                    break;

                case SdState.RD_CRC16_2: // Чтение младшего байта CRC16
                    NextState = SdState.WR_DATA_RESP;
                    break;

                default:
                    return;
            }
            currState = NextState;
        }

        private SdState GetRespondType()
        {
            if (!appCmd)
            {
                switch (cmd)
                {
                    case SdCommand.APP_CMD:
                        appCmd = true;
                        return SdState.R1;
                    case SdCommand.GO_IDLE_STATE:
                    case SdCommand.SEND_OP_COND:
                    case SdCommand.SET_BLOCKLEN:
                    case SdCommand.READ_SINGLE_BLOCK:
                    case SdCommand.READ_MULTIPLE_BLOCK:
                    case SdCommand.CRC_ON_OFF:
                    case SdCommand.STOP_TRANSMISSION:
                    case SdCommand.SEND_CSD:
                    case SdCommand.SEND_CID:
                        return SdState.R1;
                    case SdCommand.READ_OCR:
                        return SdState.R3;
                    case SdCommand.SEND_IF_COND:
                        return SdState.R7;

                    case SdCommand.WRITE_BLOCK:
                        return SdState.RD_DATA_SIG;
                    case SdCommand.WRITE_MULTIPLE_BLOCK:
                        return SdState.RD_DATA_SIG_MUL;
                }
            }
            else
            {
                appCmd = false;
                switch (cmd)
                {
                    case SdCommand.SD_SEND_OP_COND:
                        return SdState.R1;
                }
            }

            cmd = SdCommand.INVALID;
            return SdState.R1;
        }

        //ArgArr[3 - ArgCnt++] = Val;
        private void SetByte(ref uint arg, uint bnum, byte val)
        {
            UInt32 mask = (UInt32)((0x000000FF << (byte)(bnum * 8)) ^ 0xFFFFFFFF);
            arg &= mask;
            arg |= (UInt32)(val << (byte)(bnum * 8));
        }

        public byte Rd()
        {
            if (fstream == null) return 0xFF;

            switch (cmd)
            {
                case SdCommand.GO_IDLE_STATE:
                    if (currState == SdState.R1)
                    {
                        //            Cmd = CMD.INVALID;
                        currState = SdState.IDLE;
                        return 1;
                    }
                    break;
                case SdCommand.SEND_OP_COND:
                    if (currState == SdState.R1)
                    {
                        //            Cmd = CMD.INVALID;
                        currState = SdState.IDLE;
                        return 0;
                    }
                    break;
                case SdCommand.SET_BLOCKLEN:
                    if (currState == SdState.R1)
                    {
                        //            Cmd = CMD.INVALID;
                        currState = SdState.IDLE;
                        return 0;
                    }
                    break;
                case SdCommand.SEND_IF_COND:
                    if (currState == SdState.R7)
                    {
                        switch (r7_Cnt++)
                        {
                            case 0: return 0x01; // R1
                            case 1: return 0x00;
                            case 2: return 0x00;
                            case 3: return 0x01;
                            default:
                                currState = SdState.IDLE;
                                r7_Cnt = 0;
                                return (byte)(arg & 0xFF); // echo-back
                        }
                    }
                    break;

                case SdCommand.READ_OCR:
                    if (currState == SdState.R3)
                    {
                        switch (ocrCnt++)
                        {
                            case 0: return 0x00; // R1
                            case 1: return 0x80;
                            case 2: return 0xFF;
                            case 3: return 0x80;
                            default:
                                currState = SdState.IDLE;
                                ocrCnt = 0;
                                return 0x00;
                        }
                    }
                    break;

                case SdCommand.APP_CMD:
                    if (currState == SdState.R1)
                    {
                        currState = SdState.IDLE;
                        return 0;
                    }
                    break;

                case SdCommand.SD_SEND_OP_COND:
                    if (currState == SdState.R1)
                    {
                        currState = SdState.IDLE;
                        return 0;
                    }
                    break;

                case SdCommand.CRC_ON_OFF:
                    if (currState == SdState.R1)
                    {
                        currState = SdState.IDLE;
                        return 0;
                    }
                    break;

                case SdCommand.STOP_TRANSMISSION:
                    switch (currState)
                    {
                        case SdState.R1:
                            dataCnt = 0;
                            currState = SdState.R1b;
                            return 0;
                        case SdState.R1b:
                            currState = SdState.IDLE;
                            return 0xFF;
                    }
                    break;

                case SdCommand.READ_SINGLE_BLOCK:
                    switch (currState)
                    {
                        case SdState.R1:
                            currState = SdState.DELAY_S;
                            return 0;
                        case SdState.DELAY_S:
                            currState = SdState.STARTBLOCK;
                            return 0xFF;
                        case SdState.STARTBLOCK:
                            currState = SdState.WR_DATA;
                            dataCnt = 0;
                            return 0xFE;
                        case SdState.WR_DATA:
                            {
                                byte Val = buff[dataCnt++];
                                if (dataCnt == dataBlockLen)
                                {
                                    dataCnt = 0;
                                    currState = SdState.WR_CRC16_1;
                                }
                                return Val;
                            }
                        case SdState.WR_CRC16_1:
                            currState = SdState.WR_CRC16_2;
                            return 0xFF; // crc
                        case SdState.WR_CRC16_2:
                            currState = SdState.IDLE;
                            cmd = SdCommand.INVALID;
                            return 0xFF; // crc
                    }
                    //        Cmd = CMD.INVALID;
                    break;

                case SdCommand.READ_MULTIPLE_BLOCK:
                    switch (currState)
                    {
                        case SdState.R1:
                            currState = SdState.DELAY_S;
                            return 0;
                        case SdState.DELAY_S:
                            currState = SdState.STARTBLOCK;
                            return 0xFF;
                        case SdState.STARTBLOCK:
                            currState = SdState.IDLE;
                            dataCnt = 0;
                            return 0xFE;
                        case SdState.IDLE:
                            {
                                if (dataCnt < dataBlockLen)
                                {
                                    byte Val = buff[dataCnt++];
                                    if (dataCnt == dataBlockLen)
                                        fstream.Read(buff, 0, dataBlockLen);
                                    return Val;
                                }
                                else if (dataCnt > (dataBlockLen + 8))
                                {
                                    dataCnt = 0;
                                    return 0xFE; // next startblock
                                }
                                else
                                {
                                    dataCnt++;
                                    return 0xFF; // crc & pause
                                }
                            }


                    }
                    break;

                case SdCommand.SEND_CSD:
                    switch (currState)
                    {
                        case SdState.R1:
                            currState = SdState.DELAY_S;
                            return 0;
                        case SdState.DELAY_S:
                            currState = SdState.STARTBLOCK;
                            return 0xFF;
                        case SdState.STARTBLOCK:
                            currState = SdState.WR_DATA;
                            return 0xFE;
                        case SdState.WR_DATA:
                            {
                                byte Val = csd[csdCnt++];
                                if (csdCnt == 16)
                                {
                                    csdCnt = 0;
                                    currState = SdState.IDLE;
                                    cmd = SdCommand.INVALID;
                                }
                                return Val;
                            }
                    }
                    //        Cmd = CMD.INVALID;
                    break;

                case SdCommand.SEND_CID:
                    switch (currState)
                    {
                        case SdState.R1:
                            currState = SdState.DELAY_S;
                            return 0;
                        case SdState.DELAY_S:
                            currState = SdState.STARTBLOCK;
                            return 0xFF;
                        case SdState.STARTBLOCK:
                            currState = SdState.WR_DATA;
                            return 0xFE;
                        case SdState.WR_DATA:
                            {
                                byte Val = cid[cidCnt++];
                                if (cidCnt == 16)
                                {
                                    cidCnt = 0;
                                    currState = SdState.IDLE;
                                    cmd = SdCommand.INVALID;
                                }
                                return Val;
                            }
                    }
                    //        Cmd = CMD.INVALID;
                    break;

                case SdCommand.WRITE_BLOCK:
                    //        printf(__FUNCTION__" cmd=0x%X, St=0x%X\n", Cmd, CurrState);
                    switch (currState)
                    {
                        case SdState.R1:
                            currState = SdState.RD_DATA_SIG;
                            return 0xFE;

                        case SdState.WR_DATA_RESP:
                            {
                                currState = SdState.IDLE;
                                byte Resp = (((byte)SdStatus.DATA_ACCEPTED) << 1) | 1;
                                return Resp;
                            }
                    }
                    break;

                case SdCommand.WRITE_MULTIPLE_BLOCK:
                    switch (currState)
                    {
                        case SdState.R1:
                            currState = SdState.RD_DATA_SIG_MUL;
                            return 0xFE;
                        case SdState.WR_DATA_RESP:
                            {
                                currState = SdState.RD_DATA_SIG_MUL;
                                byte Resp = (((byte)SdStatus.DATA_ACCEPTED) << 1) | 1;
                                return Resp;
                            }
                    }
                    break;
            }

            if (currState == SdState.R1) // CMD.INVALID
            {
                currState = SdState.IDLE;
                return 0x05;
            }

            return 0xFF;
        }
    }
}
