using System;
using System.Collections.Generic;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Hardware.ZXBYTE
{
    public class UlaByte_Late : UlaDeviceBase
    {
        #region Fields

        private readonly byte[] m_dd10 = new byte[0x200];
        private readonly byte[] m_dd11 = new byte[0x200];
        private int[] m_contention;

        #endregion Fields


        public UlaByte_Late()
        {
            Name = "BYTE [late model]";
            Description = "BYTE [late model]\r\nVersion 1.3";
            InitStaticTables();
        }

        
        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            base.BusInit(bmgr);
            bmgr.Events.SubscribeRdMem(0xC000, 0x4000, ReadMem4000);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0x4000, ReadMem4000);
            bmgr.Events.SubscribeRdNoMreq(0xC000, 0x4000, NoMreq4000);
            bmgr.Events.SubscribeWrNoMreq(0xC000, 0x4000, NoMreq4000);
        }

        #endregion

        #region Bus Handlers

        protected override void WriteMem4000(ushort addr, byte value)
        {
            int frameTact = (int)(CPU.Tact % FrameTactCount);
            CPU.Tact += m_contention[frameTact];
            base.WriteMem4000(addr, value);
        }

        protected void ReadMem4000(ushort addr, ref byte value)
        {
            int frameTact = (int)(CPU.Tact % FrameTactCount);
            CPU.Tact += m_contention[frameTact];
        }

        protected void NoMreq4000(ushort addr)
        {
            int frameTact = (int)(CPU.Tact % FrameTactCount);
            CPU.Tact += m_contention[frameTact];
        }

        #endregion


        protected override SpectrumRendererParams CreateSpectrumRendererParams()
        {
            // Байт
            // Total Size:          448 x 312
            // Visible Size:        320 x 240 (32+256+32 x 24+192+24)
            var timing = SpectrumRenderer.CreateParams();
            timing.c_frameTactCount = 69888;
            timing.c_ulaLineTime = 224;
            timing.c_ulaFirstPaperLine = 64;
            timing.c_ulaFirstPaperTact = 56;
            timing.c_ulaBorder4T = true;
            timing.c_ulaBorder4Tstage = 0;

            timing.c_ulaBorderTop = 32;
            timing.c_ulaBorderBottom = 32;
            timing.c_ulaBorderLeftT = 16;
            timing.c_ulaBorderRightT = 16;

            timing.c_ulaIntBegin = 0;
            timing.c_ulaIntLength = 33;
            timing.c_ulaFlashPeriod = 25;

            timing.c_ulaWidth = (timing.c_ulaBorderLeftT + 128 + timing.c_ulaBorderRightT) * 2;
            timing.c_ulaHeight = (timing.c_ulaBorderTop + 192 + timing.c_ulaBorderBottom);
            return timing;
        }

        protected override void WritePortFE(ushort addr, byte value, ref bool handled)
        {
            if ((addr & 0x34) == (0xFE & 0x34))
            {
                base.WritePortFE(addr, value, ref handled);
            }
        }

        protected override void OnTimingChanged()
        {
            base.OnTimingChanged();
            //m_contention = UlaSpectrum48.CreateContentionTable(
            //    SpectrumRenderer.Params,
            //    new int[] { 6, 5, 4, 3, 2, 1, 0, 0, });

            m_contention = CreateContentionTable(
                SpectrumRenderer.Params,
                m_dd10,
                m_dd11);
        }

        private static int[] CreateContentionTable(
            SpectrumRendererParams rendererParams,
            byte[] romDd10,
            byte[] romDd11)
        {
            // Simulate full ULA signals according to DD10/DD11 ROM
            // Catch INT and then start scanning until second INT...
            // Store contentions to the list and then check frame length
            const int dd3dd4set = 0x280 >> 1;
            const int dd5dd6set = 0x0F0;
            var dd10addr = dd3dd4set;
            var dd11addr = dd5dd6set;
            var lastHsync = (romDd10[dd3dd4set & 0x1FF] & 0x01) != 0;
            var intgt = true;
            var scanning = false;
            var scanning2 = false;
            var contention = new List<int>();
            var scanTact = 0;
            var contIndex = 0;
            while (true)
            {
                {
                    //var vsync = (romDd11[dd11addr & 0x1FF] & 1) != 0;
                    //var pre56 = (romDd11[dd11addr & 0x1FF] & 2) != 0;
                    var hlock = (romDd11[dd11addr & 0x1FF] & 0x10) != 0;
                    //var vretr = (romDd11[dd11addr & 0x1FF] & 0x20) != 0;
                    var bus75 = (romDd11[dd11addr & 0x1FF] & 0x80) != 0;

                    var dd10val = romDd10[dd10addr & 0x1FF];
                    //var dd11val = romDd11[dd11addr & 0x1FF];
                    // D0 - ССИ (horizontal sync pulse)
                    var hsync = (dd10val & 1) != 0;
                    // D1 - preload DD3/DD4
                    //var pre34 = (dd10val & 2) != 0;
                    // D2 - BUS20 = A1 for DD38-DD41 (vram address generator)
                    // D3 - RAS
                    // D4 - CAS
                    // D5 - BUS23 = block CLK when BUS23=1 and mem access #4000-7FFF
                    var blclk = (dd10val & 0x20) != 0;
                    // D6 - BUS24 = attr/pixel latch, BUS24=0 -> attr latch
                    //var latch = (dd10val & 0x40) != 0;
                    // D7 - BUS142 = horizontal retrace
                    var hretr = (dd10val & 0x80) != 0;

                    if (hretr)                      // TM2 - always set on S=0
                    {
                        intgt = true;
                    }
                    else if (hsync && !lastHsync)   // TM2 - capture D on UP CLK transition
                    {
                        intgt = false;
                    }
                    var intrq = intgt | bus75;

                    //var bus23 = blclk && !hlock;

                    lastHsync = hsync;

                    if (!scanning && !intrq)
                    {
                        scanning = true;
                    }
                    if (scanning && intrq)
                    {
                        scanning2 = true;
                    }
                    if (scanning2 && !intrq)
                    {
                        break;
                    }
                    if (scanning)
                    {
                        contention.Add(0);
                        if (blclk && !hlock)
                        {
                            for (var j = contIndex; j <= scanTact; j++)
                            {
                                contention[j]++;
                            }
                        }
                        else
                        {
                            contIndex = scanTact + 1;
                        }
                        scanTact++;
                    }
                }
                // Counters DD3/DD4, DD5/DD6 simulation
                for (var j = 0; j < 2; j++)
                {
                    // DD2=#E=>#F -> UP-DOWN transition on DD3-C
                    // DD2=#F=>#0 -> DOWN-UP transition on DD3-C
                    // DD3/DD4-C DOWN-UP transition = increment or load
                    var pre34 = (romDd10[dd10addr & 0x1FF] & 2) == 0;
                    var lastVblank = (romDd10[dd10addr & 0x1FF] & 0x80) == 0;
                    dd10addr++;
                    var dd3c = (dd10addr & 7) == 0;
                    var dd4c = dd3c && (dd10addr & 0x78) == 0;
                    if (dd3c && pre34)
                    {
                        dd10addr = (dd10addr & 0x187) | (dd3dd4set & 0x078);
                    }
                    if (dd4c && pre34)
                    {
                        dd10addr = (dd10addr & 0x07F) | (dd3dd4set & 0x180);
                    }
                    // DD5/DD6 - load always when PE=0
                    // increment on +1 DOWN-UP transition
                    var pre56 = (romDd11[dd11addr & 0x1FF] & 2) == 0;
                    var vblank = (romDd10[dd10addr & 0x1FF] & 0x80) == 0;
                    if (pre56)
                    {
                        dd11addr = (dd11addr & 0x100) | (dd5dd6set & 0xFF);
                    }
                    else if (vblank && !lastVblank)
                    {
                        dd11addr++;
                    }
                }
            }
            if (contention.Count != rendererParams.c_frameTactCount)
            {
                throw new ArgumentException("Invalid frame length!");
            }
            return contention.ToArray();
        }

        private void InitStaticTables()
        {
            try
            {
                using (var stream = RomPack.GetUlaRomStream("ZXBYTE-DD10"))
                {
                    stream.Read(m_dd10, 0, m_dd10.Length);
                }
                using (var stream = RomPack.GetUlaRomStream("ZXBYTE-DD11"))
                {
                    stream.Read(m_dd11, 0, m_dd10.Length);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }

    public class UlaByte_Early : UlaByte_Late
    {
        public UlaByte_Early()
        {
            Name = "BYTE [early model]";
            Description = "BYTE [early model]\r\nVersion 1.0";
        }

        protected override SpectrumRendererParams CreateSpectrumRendererParams()
        {
            // Байт
            // Total Size:          448 x 312
            // Visible Size:        320 x 240 (32+256+32 x 24+192+24)
            var timing = base.CreateSpectrumRendererParams();

            timing.c_ulaIntBegin = -53 * timing.c_ulaLineTime; // shift all timings on 53 lines (c_ulaFirstPaperLine = 64+53)

            return timing;
        }
    }
}
