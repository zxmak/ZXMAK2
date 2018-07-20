using System;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Hardware.Spectrum
{
    public class UlaSpectrum48_Early : UlaDeviceBase
    {
        public UlaSpectrum48_Early()
        {
            Name = "ZX Spectrum 48 [early model]";
        }
        

        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            base.BusInit(bmgr);
            bmgr.Events.SubscribeRdMem(0xC000, 0x4000, ReadMem4000);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0x4000, ReadMem4000);

            bmgr.Events.SubscribeRdNoMreq(0xC000, 0x4000, ContendNoMreq);
            bmgr.Events.SubscribeWrNoMreq(0xC000, 0x4000, ContendNoMreq);

            bmgr.Events.SubscribeRdIo(0x0000, 0x0000, ReadPortAll);
            bmgr.Events.SubscribeWrIo(0x0000, 0x0000, WritePortAll);
        }

        #endregion

        public override bool IsEarlyTimings
        {
            get { return true; }
        }

        protected override SpectrumRendererParams CreateSpectrumRendererParams()
        {
            // ZX Spectrum 48
            // Total Size:          448 x 312
            // Visible Size:        352 x 303 (48+256+48 x 55+192+56)
            var timing = SpectrumRenderer.CreateParams();
            timing.c_frameTactCount = 69888;
            timing.c_ulaLineTime = 224;
            timing.c_ulaFirstPaperLine = 64;
            timing.c_ulaFirstPaperTact = 64;      // 64 [40sync+24border+128scr+32border]
            timing.c_ulaBorder4T = true;
            timing.c_ulaBorder4Tstage = 0;

            timing.c_ulaBorderTop = 32;      //55 (at least 48=border, other=retrace or border)
            timing.c_ulaBorderBottom = 32;   //56
            timing.c_ulaBorderLeftT = 16;    //16T
            timing.c_ulaBorderRightT = 16;   //32T

            timing.c_ulaIntBegin = 64;
            timing.c_ulaIntLength = 32;    // according to fuse

            timing.c_ulaWidth = (timing.c_ulaBorderLeftT + 128 + timing.c_ulaBorderRightT) * 2;
            timing.c_ulaHeight = (timing.c_ulaBorderTop + 192 + timing.c_ulaBorderBottom);
            return timing;
        }


        #region Bus Handlers

        protected override void WriteMem4000(ushort addr, byte value)
        {
            contendMemory();
            base.WriteMem4000(addr, value);
        }

        protected void ReadMem4000(ushort addr, ref byte value)
        {
            contendMemory();
        }

        #region The same as 128

        protected void ContendNoMreq(ushort addr)
        {
            if (IsContended(addr))
                contendMemory();
        }

        protected override void WritePortFE(ushort addr, byte value, ref bool handled)
        {
        }

        private void WritePortAll(ushort addr, byte value, ref bool handled)
        {
            contendPortEarly(addr);
            contendPortLate(addr);
            if ((addr & 0x0001) == 0)
            {
                int frameTact = (int)((CPU.Tact - 2) % FrameTactCount);
                UpdateState(frameTact);
                PortFE = value;
            }
        }

        private void ReadPortAll(ushort addr, ref byte value, ref bool handled)
        {
            contendPortEarly(addr);
            contendPortLate(addr);
            int frameTact = (int)((CPU.Tact - 1) % FrameTactCount);
            base.ReadPortFF(frameTact, ref value);
        }

        #endregion

        #endregion

        private bool IsContended(int addr)
        {
            int test = addr & 0xC000;
            return (test == 0x4000);
        }

        private bool IsPortUla(int addr)
        {
            return (addr & 1) == 0;
        }

        #region The same as 128

        private void contendMemory()
        {
            int frameTact = (int)(CPU.Tact % FrameTactCount);
            CPU.Tact += m_contention[frameTact];
        }

        private void contendPortEarly(int addr)
        {
            if (IsContended(addr))
            {
                int frameTact = (int)(CPU.Tact % FrameTactCount);
                CPU.Tact += m_contention[frameTact];
            }
        }

        private void contendPortLate(int addr)
        {
            int shift = 1;
            int frameTact = (int)((CPU.Tact + shift) % FrameTactCount);

            if (IsPortUla(addr))
            {
                CPU.Tact += m_contention[frameTact];
            }
            else if (IsContended(addr))
            {
                CPU.Tact += m_contention[frameTact]; frameTact += m_contention[frameTact]; frameTact++; frameTact %= FrameTactCount;
                CPU.Tact += m_contention[frameTact]; frameTact += m_contention[frameTact]; frameTact++; frameTact %= FrameTactCount;
                CPU.Tact += m_contention[frameTact]; frameTact += m_contention[frameTact]; frameTact++; frameTact %= FrameTactCount;
            }
        }

        protected override void OnTimingChanged()
        {
            base.OnTimingChanged();
            m_contention = CreateContentionTable(
                SpectrumRenderer.Params,
                new int[] { 6, 5, 4, 3, 2, 1, 0, 0, });
        }

        public static int[] CreateContentionTable(
            SpectrumRendererParams timing,
            int[] byteContention)
        {
            // build early model table...
            var contention = new int[timing.c_frameTactCount];
            for (int t = 0; t < timing.c_frameTactCount; t++)
            {
                int shifted = (t + 1) + timing.c_ulaIntBegin;
                // check overflow
                if (shifted < 0)
                    shifted += timing.c_frameTactCount;
                shifted %= timing.c_frameTactCount;

                contention[t] = 0;
                int line = shifted / timing.c_ulaLineTime;
                int pix = shifted % timing.c_ulaLineTime;
                if (line < timing.c_ulaFirstPaperLine || line >= (timing.c_ulaFirstPaperLine + 192))
                {
                    contention[t] = 0;
                    continue;
                }
                int scrPix = pix - timing.c_ulaFirstPaperTact;
                if (scrPix < 0 || scrPix >= 128)
                {
                    contention[t] = 0;
                    continue;
                }
                int pixByte = scrPix % 8;

                contention[t] = byteContention[pixByte];
            }
            return contention;
        }

        private int[] m_contention;

        #endregion

        //protected override void EndFrame()
        //{
        //    base.EndFrame();
        //    if (IsKeyPressed(System.Windows.Forms.Keys.F1))
        //    {
        //        c_ulaBorder4T = true;
        //        c_ulaBorder4Tstage = 0;
        //        OnTimingChanged();
        //    }
        //    if (IsKeyPressed(System.Windows.Forms.Keys.F2))
        //    {
        //        c_ulaBorder4T = true;
        //        c_ulaBorder4Tstage = 1;
        //        OnTimingChanged();
        //    }
        //    if (IsKeyPressed(System.Windows.Forms.Keys.F3))
        //    {
        //        c_ulaBorder4T = true;
        //        c_ulaBorder4Tstage = 2;
        //        OnTimingChanged();
        //    }
        //    if (IsKeyPressed(System.Windows.Forms.Keys.F4))
        //    {
        //        c_ulaBorder4T = true;
        //        c_ulaBorder4Tstage = 3;
        //        OnTimingChanged();
        //    }
        //    if (IsKeyPressed(System.Windows.Forms.Keys.F5))
        //    {
        //        c_ulaBorder4T = false;
        //        OnTimingChanged();
        //    }
        //}
        //private static bool IsKeyPressed(System.Windows.Forms.Keys key)
        //{
        //    return (GetKeyState((int)key) & 0xFF00) != 0;
        //}
        //[System.Runtime.InteropServices.DllImport("user32")]
        //private static extern short GetKeyState(int vKey);
    }

    public class UlaSpectrum48 : UlaSpectrum48_Early
    {
        public UlaSpectrum48()
        {
            Name = "ZX Spectrum 48 [late model]";
        }
        
        public override bool IsEarlyTimings
        {
            get { return false; }
        }

        protected override SpectrumRendererParams CreateSpectrumRendererParams()
        {
            var timing = base.CreateSpectrumRendererParams();
            timing.c_ulaIntLength += 1;
            timing.c_ulaFirstPaperTact += 1;
            timing.c_ulaBorder4Tstage = (timing.c_ulaBorder4Tstage + 1) & 3;
            return timing;
        }
    }
}
