using System;
using ZXMAK2.Engine.Interfaces;

// Contended memory info links:
//  +3: http://scratchpad.wikia.com/wiki/Contended_memory#Instruction_breakdown
// 128: http://www.worldofspectrum.org/faq/reference/128kreference.htm
//  48: http://www.worldofspectrum.org/faq/reference/48kreference.htm
// http://www.zxdesign.info/dynamicRam.shtml
// examples: http://zxm.speccy.cz/realspec/

namespace ZXMAK2.Hardware.Spectrum
{
    public class UlaPlus3 : UlaDeviceBase
    {
        public UlaPlus3()
        {
            Name = "ZX Spectrum +2A/+3";
        }
        
        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            base.BusInit(bmgr);
            bmgr.Events.SubscribeRdMem(0xC000, 0x4000, ReadMem4000);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0x4000, ReadMem4000);
            bmgr.Events.SubscribeRdMem(0xC000, 0xC000, ReadMemC000);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0xC000, ReadMemC000);

            bmgr.Events.SubscribeRdIo(0x0000, 0x0000, ReadPortAll);
            bmgr.Events.SubscribeWrIo(0x0000, 0x0000, WritePortAll);
        }

        #endregion

        protected override SpectrumRendererParams CreateSpectrumRendererParams()
        {
            // ZX Spectrum +3
            // Total Size:          456 x 311
            // Visible Size:        352 x 303 (48+256+48 x 55+192+56)
            var timing = SpectrumRenderer.CreateParams();
            timing.c_frameTactCount = 70908;
            timing.c_ulaLineTime = 228;
            timing.c_ulaFirstPaperLine = 63;
            timing.c_ulaFirstPaperTact = 64;      // 64 [40sync+24border+128scr+32border]
            timing.c_ulaBorder4T = true;
            timing.c_ulaBorder4Tstage = 2;

            timing.c_ulaBorderTop = 32;      //55
            timing.c_ulaBorderBottom = 32;   //56
            timing.c_ulaBorderLeftT = 16;    //16T
            timing.c_ulaBorderRightT = 16;   //32T

            timing.c_ulaIntBegin = 64 + 2;
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

        protected override void WriteMemC000(ushort addr, byte value)
        {
            if ((m_pageC000 & 1) != 0)
                contendMemory();
            base.WriteMemC000(addr, value);
        }

        protected void ReadMem4000(ushort addr, ref byte value)
        {
            contendMemory();
        }

        protected void ReadMemC000(ushort addr, ref byte value)
        {
            if ((m_pageC000 & 1) != 0)
                contendMemory();
        }

        #region The same as 48

        protected override void WritePortFE(ushort addr, byte value, ref bool handled)
        {
        }

        private void WritePortAll(ushort addr, byte value, ref bool handled)
        {
            if ((addr & 0x0001) == 0)
            {
                int frameTact = (int)((CPU.Tact - 2) % FrameTactCount);
                UpdateState(frameTact);
                PortFE = value;
            }
        }

        private void ReadPortAll(ushort addr, ref byte value, ref bool handled)
        {
            int frameTact = (int)((CPU.Tact - 1) % FrameTactCount);
            base.ReadPortFF(frameTact, ref value);
        }

        #endregion

        #endregion

        #region The same as 48

        private void contendMemory()
        {
            int frameTact = (int)(CPU.Tact % FrameTactCount);
            CPU.Tact += m_contention[frameTact];
        }

        protected override void OnTimingChanged()
        {
            base.OnTimingChanged();
            m_contention = UlaSpectrum48.CreateContentionTable(
                SpectrumRenderer.Params,
                new int[] { 1, 0, 7, 6, 5, 4, 3, 2, });
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
}
