using System;
using System.IO;


namespace ZXMAK2.Hardware.Circuits
{
    /// <summary>
    /// RTC IC emulator
    /// </summary>
    public class RtcChip
    {
        private readonly byte[] m_ram = new byte[0x80];
        private RtcChipType m_chipType;
        private byte m_addrMask;
        private byte m_addr;
        private DateTime m_dateTime;
        private bool m_uf;
        private bool m_af;
        private bool m_pf;
        private int m_seconds;


        public RtcChip(RtcChipType chipType)
        {
            m_dateTime = DateTime.Now;
            m_chipType = chipType;
            m_addrMask = 0;
            if (m_chipType == RtcChipType.DS1285)
            {
                m_addrMask = 0x3F;
            }
            if (m_chipType == RtcChipType.DS12885)
            {
                m_addrMask = 0x7F;
            }
        }

        public void Load(string fileName)
        {
            try
            {
                for (var i = 0; i < m_ram.Length; i++)
                {
                    m_ram[i] = (byte)(i <= 0xD ? 0x00 : 0xFF);
                }
                m_ram[0xA] = 0x27;  // enable clock
                m_ram[0xB] = 0x07;  // 24/binary/daylight saving
                if (File.Exists(fileName))
                {
                    using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        fs.Read(m_ram, 0, m_ram.Length);
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
                var fileInfo = new FileInfo(fileName);
                if (fileInfo.Exists && fileInfo.IsReadOnly)
                {
                    Logger.Warn("The CMOS image could not be written to disk because marked with read only attribute: {0}", fileName);
                    return;
                }
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    fs.Write(m_ram, 0, m_ram.Length);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        public void Reset()
        {
            m_addr = 0;
        }

        public void WriteAddr(byte value)
        {
            m_addr = value;
        }

        public void WriteData(byte value)
        {
            var reg = m_addr & m_addrMask;
            if (value > 0xD)
            {
                m_ram[reg] = value;
                return;
            }
            switch (reg)
            {
                case 6:
                    value = (byte)(((value - 1) & 7) % 7);
                    m_ram[reg] = GetDelta((int)m_dateTime.DayOfWeek, value, 7);
                    break;
                default:
                    m_ram[reg] = value;
                    break;
            }
        }

        public void ReadData(ref byte value)
        {
            var reg = m_addr & m_addrMask;

            if (((1 << reg) & ((1 << 0) | (1 << 2) | (1 << 4) | (1 << 6) | (1 << 7) | (1 << 8) | (1 << 9) | (1 << 0xA) | (1 << 0xC))) != 0)
            {
                m_dateTime = DateTime.Now;
                if (m_dateTime.Second != m_seconds)
                {
                    m_uf = true;
                    m_seconds = m_dateTime.Second;
                }
            }
            var uip = !IsSetMode && m_dateTime.Millisecond >= 997;
            switch (reg)
            {
                case 0: value = Bcd(m_dateTime.Second); break;
                case 2: value = Bcd(m_dateTime.Minute); break;
                case 4:
                    int hours = m_dateTime.Hour;
                    var pm = 0x00;
                    if (!IsFormat24)
                    {
                        if (hours == 0)
                        {
                            hours = 12;
                        }
                        else if (hours == 12)
                        {
                            pm = 0x80;
                        }
                        else if (hours > 12)
                        {
                            hours -= 12;
                            pm = 0x80;
                        }
                    }
                    value = (byte)(Bcd(m_dateTime.Hour) | pm);
                    break;
                case 6:
                    var dayOfWeek = 1 + (((int)m_dateTime.DayOfWeek + m_ram[6]) % 7);
                    value = (byte)dayOfWeek;
                    break;
                case 7: value = Bcd(m_dateTime.Day); break;
                case 8: value = Bcd(m_dateTime.Month); break;
                case 9: value = Bcd(m_dateTime.Year % 100); break;
                case 0xA:
                    value = (byte)((m_ram[0xA] & 0x7F) | (uip ? 0x80 : 0));
                    break;
                case 0xB: value = m_ram[0xB]; break;
                case 0xC:
                    value = 0;
                    if (m_uf) value |= 0x10;
                    if (m_af) value |= 0x20;
                    if (m_pf) value |= 0x40;
                    if ((value & (m_ram[0xB] & 0x70)) != 0) value |= 0x80;
                    m_uf = false;
                    m_af = false;
                    m_pf = false;
                    break;
                case 0xD: value = 0x80; break;
                default: value = m_ram[reg]; break;
            }
        }

        private bool IsFormat24
        {
            get { return (m_ram[0x0B] & 2) != 0; }
        }

        private bool IsFormatBcd
        {
            get { return (m_ram[0x0B] & 4) == 0; }
        }

        private bool IsSetMode
        {
            get { return (m_ram[0x0B] & 0x80) != 0; }
        }

        private byte Bcd(int binary)
        {
            if (IsFormatBcd)
            {
                binary = (binary % 10) + 0x10 * ((binary / 10) % 10);
            }
            return (byte)binary;
        }

        private byte GetDelta(int curVal, int newVal, int basis)
        {
            if (newVal < curVal)
            {
                return (byte)(basis + (basis-(curVal - newVal)));
            }
            else
            {
                return (byte)((newVal - curVal) % basis);
            }
        }
    }

    public enum RtcChipType
    {
        /// <summary>
        /// DS1285/MC146818/KR512VI1
        /// </summary>
        DS1285 = 0,
        /// <summary>
        /// DS12885
        /// </summary>
        DS12885,
    }
}
