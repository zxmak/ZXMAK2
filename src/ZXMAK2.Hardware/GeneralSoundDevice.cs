using System;

using ZXMAK2.Engine.Interfaces;
using System.Threading;
using ZXMAK2.Engine.Z80;
using System.IO;
using System.Reflection;
using ZXMAK2.Logging;


namespace ZXMAK2.Engine
{
    public class GeneralSoundDevice : IBusDevice, ISoundRenderer
    {
        private uint[] m_audioBuffer = new uint[882];
        private Thread m_thread = null;
        private volatile bool m_running = false;

        #region IBusDevice Members

        public string Name { get { return "General Sound"; } }
        public string Description { get { return "General Sound Sound Card Emulator"; } }
        public BusCategory Category { get { return BusCategory.Music; } }

        public void BusConnect(IBusManager bmgr)
        {
            bmgr.SubscribeWRIO(0x00FF, 0x00BB, writeBB);    // GSCOM
            bmgr.SubscribeRDIO(0x00FF, 0x00BB, readBB);     // GSSTAT
            bmgr.SubscribeWRIO(0x00FF, 0x00B3, writeB3);    // GSDAT
            bmgr.SubscribeRDIO(0x00FF, 0x00B3, readB3);     // 
            bmgr.SubscribeWRIO(0x00FF, 0x0033, write33);    // GSCTR
            bmgr.SubscribeEndFrame(endFrame);

            m_rom = new byte[32][];// = new byte[0x80000];  // 524288
            m_ram = new byte[256][];// = new byte[0x400000];  // 4194304
            for (int i = 0; i < m_rom.Length; i++)
                m_rom[i] = new byte[0x4000];
            for (int i = 0; i < m_rom.Length; i++)
                m_ram[i] = new byte[0x4000];

            string fileName = "ZXMAK2.bootgs.rom";
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
                for(int i=0; i < m_rom.Length; i++)
                    stream.Read(m_rom[i], 0, m_rom[i].Length);

            m_thread = new Thread(new ThreadStart(threadProc));
            m_thread.Name = "GeneralSoundDevice Thread";
            m_thread.IsBackground = true;
            m_thread.Start();
            while (!m_running)
                Thread.Sleep(1);
        }

        public void BusDisconnect()
        {
            m_running = false;
            m_event.Set();    
            m_thread.Join();
            m_thread = null;
        }

        #endregion

        #region ISoundRenderer Members

        public uint[] AudioBuffer { get { return m_audioBuffer; } }

        #endregion

        #region IO latch regs

        private byte gsdata_out = 0;
        private byte gsdata_in = 0;
        private byte gsstat = 0;
        private byte gscmd = 0;
        
        private void writeBB(long cpuTact, ushort addr, byte value)
        {
            gscmd = value;
            gsstat |= 0x01;
        }

        private void readBB(long cpuTact, ushort addr, ref byte value)
        {
            value = (byte)(gsstat | 0x7E);
        }

        private void writeB3(long cpuTact, ushort addr, byte value)
        {
            gsdata_out = value;
            gsstat |= 0x80;
        }

        private void readB3(long cpuTact, ushort addr, ref byte value)
        {
            gsstat &= 0x7F;
            value = gsdata_in;
        }

        
        private const int C_GRST = 0x80;    // reset constant to be written into GSCTR
        private const int C_GNMI = 0x40;    // NMI constant to be written into GSCTR
        private const int C_GLED = 0x20;    // LED toggle constant

        private void write33(long cpuTact, ushort addr, byte value)
        {
            m_cpu.RST = (value & C_GRST) != 0;
            m_cpu.NMI = (value & C_GRST) != 0;
            if (m_cpu.RST || m_cpu.NMI)
                m_event.Set();
        }

        #endregion

        
        #region GS Emulation

        private Z80CPU m_cpu = new Z80CPU();
        private byte[][] m_rom;// = new byte[0x80000];  // 524288
        private byte[][] m_ram;// = new byte[0x400000];  // 4194304
        private AutoResetEvent m_event = new AutoResetEvent(false);

        private void endFrame()
        {
            m_event.Set();    
        }

        private const long Z80_FQ = 10000000;
        private const long FPS = 50;
        private long FRAME_LEN = Z80_FQ / FPS;
        
        private void threadProc()
        {
            m_running = true;
            m_cpu.RDMEM_M1 = RDMEM;
            m_cpu.RDMEM = RDMEM;
            m_cpu.WRMEM = WRMEM;
            m_cpu.RDPORT = RDPORT;
            m_cpu.WRPORT = WRPORT;
            m_cpu.RESET = DUMMY;
            m_cpu.INTACK_M1 = DUMMY;
            m_cpu.NMIACK_M1 = DUMMY;
            m_cpu.RST = true;
            m_cpu.ExecCycle();
            m_cpu.RST = false;
            long next = m_cpu.Tact;
            long nint = m_cpu.Tact;
            initGsbank();
            while (m_running)
            {
                next += FRAME_LEN;
                while (m_cpu.Tact < next)
                {
                    int intTact = (int)(m_cpu.Tact % (FRAME_LEN * 50 / 44100));
                    m_cpu.INT = intTact < 32;
                    m_cpu.ExecCycle();
                }
                m_event.WaitOne();
            }
        }


        private const int PAGE = 0x4000;
        private byte[] m_trash = new byte[PAGE];
        private byte[][] gsbankr;   // bank pointers for read
        private byte[][] gsbankw;   // bank pointers for write
        
        private void initGsbank()
        {
            gsbankr = new byte[4][] { m_rom[0], m_ram[3], m_rom[0], m_rom[1] };
            gsbankw = new byte[4][] { m_trash, m_ram[3], m_trash, m_trash };
        }

        private int ngs_cfg0 = 0x30;
        private int gspage = 0;
        private int ngs_mode_pg1 = 0;

        private const int gs_ram_size = 2048;
        private const int gs_ram_mask = (gs_ram_size - 1) >> 4;

        private void updateMemMapping()
        {
            bool RamRo = (ngs_cfg0 & M_RAMRO) != 0;
            bool NoRom = (ngs_cfg0 & M_NOROM) != 0;
            if(NoRom)
            {
                gsbankr[0] = gsbankw[0] = m_ram[0];
                gsbankr[1] = gsbankw[1] = m_ram[3];
                gsbankr[2] = gsbankw[2] = m_ram[gspage];
                gsbankr[3] = gsbankw[3] = m_ram[ngs_mode_pg1];

                if(RamRo)
                {
                    if(gspage == 0 || gspage == 1) // RAM0 or RAM1 in PG2
                        gsbankw[2] = m_trash;
                    if(ngs_mode_pg1 == 0 || ngs_mode_pg1 == 1) // RAM0 or RAM1 in PG3
                        gsbankw[3] = m_trash;
                }
            }
            else
            {
                gsbankw[0] = gsbankw[2] = gsbankw[3] = m_trash;
                gsbankr[0] = m_rom[0];                                  // ROM0
                gsbankr[1] = gsbankw[1] = m_ram[3];                     // RAM3
                gsbankr[2] = m_rom[gspage & 0x1F];                      // ROMn
                gsbankr[3] = m_rom[ngs_mode_pg1 & 0x1F];                // ROMm
            }
        }


        //uint[] gs_v = new uint[4];
        private byte[] gsvol = new byte[4];
        //private byte[] gsbyte = new byte[4];


        private byte RDMEM(ushort addr)
        {
            return gsbankr[(addr >> 14) & 3][addr & (PAGE - 1)];
        }

        private void WRMEM(ushort addr, byte value)
        {
            gsbankw[(addr >> 14) & 3][addr & (PAGE - 1)] = value;
        }

        private void WRPORT(ushort addr, byte value)
        {
            switch (addr & 0xFF)
            {
                case MPAG:
                    bool ExtMem = (ngs_cfg0 & M_EXPAG) != 0;
                    gspage = rol8(value, 1) & gs_ram_mask & (ExtMem ? 0xFF : 0xFE);

                    if (!ExtMem)
                        ngs_mode_pg1 = (rol8(value, 1) & gs_ram_mask) | 1;
                    updateMemMapping();
                    break;

                case ZXDATRD: gsstat &= 0x7F; break;                    // 0x02
                case ZXDATWR: gsstat |= 0x80; gsdata_in = value; break; // 0x03    
                case CLRCBIT: gsstat &= 0xFE; return;                   // 0x05
                case VOL1:  // 0x06
                case VOL2:  // 0x07
                case VOL3:  // 0x08
                case VOL4:  // 0x09
                    ////flush()
                    int chan = (addr & 0x0F)-6; value &= 0x3F;
                    gsvol[chan] = value;
                    ////         gs_v[chan] = (gsbyte[chan] * gs_vfx[gsvol[chan]]) >> 8;
                    //gs_v[chan] = ((signed char)(gsbyte[chan]-0x80) * (signed)gs_vfx[gsvol[chan]]) /256 + gs_vfx[33]; //!psb
                    break;
                case 0x0A: gsstat = (byte)((gsstat & 0x7F) | (gspage << 7)); break;
                case 0x0B: gsstat = (byte)((gsstat & 0xFE) | ((gsvol[0] >> 5) & 1)); break;


                case GSCFG0:
                    ngs_cfg0 = value & 0x3F;
                    updateMemMapping();
                    break;

                case MPAGEX:
                    ngs_mode_pg1 = rol8(value, 1) & gs_ram_mask;
                    updateMemMapping();
                    break;
                
                default:
                    Logger.GetLogger().LogTrace(string.Format("SKIP GS WRIO #{0:X2},#{1:X2}", addr, value));
                    break;
            }
        }

        private byte RDPORT(ushort addr)
        {
            byte value = 0xFF;
            switch (addr & 0xFF)
            {
                case ZXCMD: value = gscmd; break;   // 0x01
                case ZXDATRD: gsstat &= 0x7F; value = gsdata_out; break;
                case ZXDATWR: gsstat |= 0x80; gsdata_in = 0xFF; value = 0xFF; break;
                case 0x04: value = gsstat; break;
                case CLRCBIT: gsstat &= 0xFE; value = 0xFF; break;
                case DAMNPORT1: gsstat = (byte)((gsstat & 0x7F) | (gspage << 7)); value = 0xFF; break;
                case DAMNPORT2: gsstat = (byte)((gsstat & 0xFE) | (gsvol[0] >> 5)); value = 0xFF; break;

                case GSCFG0: value = (byte)ngs_cfg0; break;
            }
            return value;
        }


        private byte rol8(byte val, int shift)
        {
            int shifted = val << (shift&7);
            return (byte)((shifted & 0xFF) | (shifted >> 8));
        }

        private const int MPAG = 0x00;      // ; write-only, Memory PAGe port (big pages at 8000-FFFF or small at 8000-BFFF)
        private const int MPAGEX = 0x10;    // ; write-only, Memory PAGe EXtended (only small pages at C000-FFFF)

        private const int ZXCMD=0x01;   // read-only, ZX CoMmanD port: here is the byte written by ZX into GSCOM
        private const int ZXDATRD=0x02; // read-only, ZX DATa ReaD: a byte written by ZX into GSDAT appears here;
                                        // upon reading this port, data bit is cleared
        private const int ZXDATWR=0x03; // write-only, ZX DATa WRite: a byte written here is available for ZX in GSDAT;
                                        // upon writing here, data bit is set
        private const int ZXSTAT=0x04;  // read-only, read ZX STATus: command and data bits. positions are defined by *_CBIT and *_DBIT above
        private const int CLRCBIT = 0x05; // read-write, upon either reading or writing this port, the Command BIT is CLeaRed
        
        private const int VOL1 = 0x06;  // write-only, volumes for sound channels 1-8
        private const int VOL2 = 0x07;
        private const int VOL3 = 0x08;
        private const int VOL4 = 0x09;
        private const int VOL5 = 0x16;
        private const int VOL6 = 0x17;
        private const int VOL7 = 0x18;
        private const int VOL8 = 0x19;

        //; following two ports are useless and very odd. They have been made just because they were on the original GS and for that
        //; strange case when somebody too crazy have used them. Nevertheless, DO NOT USE THEM! They can disappear or even radically
        //; change functionality in future firmware releases.
        private const int DAMNPORT1 = 0x0A;     // ; writing or reading this port sets data bit to the inverse of bit 0 into MPAG port
        private const int DAMNPORT2 = 0x0B;     // ; the same as DAMNPORT1, but instead command bit involved, which is made equal to 5th bit of VOL4

        private const int GSCFG0 = 0x0F;        // ; read-write, GS ConFiG port 0: acts as memory cell, reads previously written value. Bits and fields follow:
        private const int M_NOROM = 1;          // ; =0 - there is ROM everywhere except 4000-7FFF, =1 - the RAM is all around
        private const int M_RAMRO = 2;          // ; =1 - ram absolute addresses 0000-7FFF (zeroth big page) are write-protected
        private const int M_8CHANS = 4;         // ; =1 - 8 channels mode
        private const int M_EXPAG = 8;          // ; =1 - extended paging: both MPAG and MPAGEX are used to switch two memory windows
        private const int M_CKSEL0 = 0x10;      // ;these bits should be set according to the C_**MHZ constants below
        private const int M_CKSEL1 = 0x20;
//C_10MHZ		equ	#30
//C_12MHZ		equ	#10
//C_20MHZ		equ	#20
//C_24MHZ		equ	#00
        private const int M_PAN4CH = 0x40;      // ; =1 - 4 channels, panning (every channel is on left and right with two volumes)
        private const int M_SETNCLR = 0x80;

        //private const int SCTRL = 0x11;         //   ;Serial ConTRoL: read-write, read: current state of below bits, write - see GS_info
        //private const int SSTAT = 0x12;         //   ;Serial STATus: read-only, reads state of below bits

        private void DUMMY()
        {
        }

        #endregion
    }
}
