using System;
using System.IO;


namespace ZXMAK2.Hardware.Circuits
{
    /// <summary>
    /// 24C16 SPI NVRAM emulator
    /// </summary>
    public class NvramChip
    {
        private readonly byte[] m_nvram = new byte[2048];	// 24c16 = 2048 bytes; 24c32 - 4096 bytes

        private int m_address;
        private byte m_datain, m_dataout, m_bitsin, m_bitsout;
        private NVSTATE m_state;
        private byte m_prev;
        private byte m_out_z;
        private byte m_out;

        public void Write(byte val)
        {
            try
            {
                if (((val ^ m_prev) & SCL) != 0) // clock edge, data in/out
                {
                    if ((val & SCL) != 0) // nvram reads SDA
                    {
                        if (m_state == NVSTATE.RD_ACK)
                        {
                            if ((val & SDA) != 0) // no ACK, stop
                            {
                                // goto idle;
                                m_state = NVSTATE.IDLE;
                                m_out_z = 1;
                                return;
                            }
                            // move next byte to host
                            m_state = NVSTATE.SEND_DATA;
                            m_dataout = m_nvram[m_address];
                            m_address = (m_address + 1) & 0x7FF;
                            m_bitsout = 0;
                            //goto exit; // out_z==1;
                            return;
                        }

                        if (((1 << (int)m_state) & ((1 << (int)NVSTATE.RCV_ADDR) | (1 << (int)NVSTATE.RCV_CMD) | (1 << (int)NVSTATE.RCV_DATA))) != 0)
                        {
                            if (m_out_z != 0) // skip nvram ACK before reading
                            {
                                m_datain = (byte)(2 * m_datain + ((val >> SDA_SHIFT_IN) & 1));
                                m_bitsin++;
                            }
                        }

                    }
                    else
                    {		// nvram sets SDA

                        if (m_bitsin == 8) // byte received
                        {
                            m_bitsin = 0;
                            if (m_state == NVSTATE.RCV_CMD)
                            {
                                if ((m_datain & 0xF0) != 0xA0)
                                {
                                    // goto idle;
                                    m_state = NVSTATE.IDLE;
                                    m_out_z = 1;
                                    return;
                                }
                                m_address = (m_address & 0xFF) + ((m_datain << 7) & 0x700);
                                if ((m_datain & 1) != 0)
                                { // read from current address
                                    m_dataout = m_nvram[m_address];
                                    m_address = (m_address + 1) & 0x7FF;
                                    m_bitsout = 0;
                                    m_state = NVSTATE.SEND_DATA;
                                }
                                else
                                {
                                    m_state = NVSTATE.RCV_ADDR;
                                }
                            }
                            else if (m_state == NVSTATE.RCV_ADDR)
                            {
                                m_address = (m_address & 0x700) + m_datain;
                                m_state = NVSTATE.RCV_DATA;
                                m_bitsin = 0;
                            }
                            else if (m_state == NVSTATE.RCV_DATA)
                            {
                                m_nvram[m_address] = m_datain;
                                m_address = (m_address & 0x7F0) + ((m_address + 1) & 0x0F);
                                // state unchanged
                            }

                            // EEPROM always acknowledges
                            m_out = SDA_0;
                            m_out_z = 0;
                            //goto exit;
                            return;
                        }

                        if (m_state == NVSTATE.SEND_DATA)
                        {
                            if (m_bitsout == 8)
                            {
                                m_state = NVSTATE.RD_ACK;
                                m_out_z = 1;
                                //goto exit;
                                return;
                            }
                            m_out = (byte)((m_dataout & 0x80) != 0 ? SDA_1 : SDA_0);
                            m_dataout *= 2;
                            m_bitsout++;
                            m_out_z = 0;
                            //goto exit;
                            return;
                        }
                        m_out_z = 1; // no ACK, reading
                    }
                    //goto exit;
                    return;
                }

                if ((val & SCL) != 0 && ((val ^ m_prev) & SDA) != 0) // start/stop
                {
                    if ((val & SDA) != 0)
                    {	// stop 
                        // goto idle;
                        m_state = NVSTATE.IDLE;
                        m_out_z = 1;
                        return;
                    }
                    else
                    {
                        m_state = NVSTATE.RCV_CMD;
                        m_bitsin = 0; // start
                    }
                    m_out_z = 1;
                }

                // else SDA changed on low SCL
            }
            finally
            {
                if (m_out_z != 0)
                {
                    m_out = (byte)((val & SDA) != 0 ? SDA_1 : SDA_0);
                }
                m_prev = val;
            }
        }

        public byte Read()
        {
            return m_out;
        }

        private enum NVSTATE : byte
        {
            IDLE = 0,
            RCV_CMD = 1,
            RCV_ADDR = 2,
            RCV_DATA = 3,
            SEND_DATA = 4,
            RD_ACK = 5,
        };

        private const int SCL = 0x40,
            SDA = 0x10,
            WP = 0x20,
            SDA_1 = 0xFF,
            SDA_0 = 0xBF,
            SDA_SHIFT_IN = 4;

        public void Load(string fileName)
        {
            try
            {
                for (int i = 0; i < m_nvram.Length; i++)
                {
                    m_nvram[i] = 0x00;
                }
                if (File.Exists(fileName))
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        fs.Read(m_nvram, 0, m_nvram.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void Save(string fileName)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    fs.Write(m_nvram, 0, m_nvram.Length);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
