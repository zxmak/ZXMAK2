using System;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Hardware.Scorpion
{
    public class UlaScorpion : UlaDeviceBase
    {
        public UlaScorpion()
        {
            Name = "Scorpion [Yellow]";
        }

        
        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            base.BusInit(bmgr);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0x4000, busRDM1);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0x8000, busRDM1);
            bmgr.Events.SubscribeRdMemM1(0xC000, 0xC000, busRDM1);
        }

        #endregion

        protected override SpectrumRendererParams CreateSpectrumRendererParams()
        {
            // Scorpion [Yellow PCB]
            // Total Size:          448 x 312
            // Visible Size:        320 x 240 (32+256+32 x 24+192+24)
            var timing = SpectrumRenderer.CreateParams();
            timing.c_frameTactCount = 69888;//+
            timing.c_ulaLineTime = 224;
            timing.c_ulaFirstPaperLine = 64;
            timing.c_ulaFirstPaperTact = 61;// 64;      // 64 [40sync+24border+128scr+32border]
            timing.c_ulaBorder4T = true;
            timing.c_ulaBorder4Tstage = 0;

            timing.c_ulaBorderTop = 32;//64;
            timing.c_ulaBorderBottom = 32;// 40;
            timing.c_ulaBorderLeftT = 16;// 24;  //24
            timing.c_ulaBorderRightT = 16;// 24; //32

            timing.c_ulaIntBegin = 61;
            timing.c_ulaIntLength = 32;    // according to fuse

            timing.c_ulaWidth = (timing.c_ulaBorderLeftT + 128 + timing.c_ulaBorderRightT) * 2;
            timing.c_ulaHeight = (timing.c_ulaBorderTop + 192 + timing.c_ulaBorderBottom);
            return timing;
        }

        #region Bus Handlers

        private void busRDM1(ushort addr, ref byte value)
        {
            CPU.Tact += CPU.Tact & 1;
        }

        #endregion

        #region UlaDeviceBase

        protected override void WritePortFE(ushort addr, byte value, ref bool handled)
        {
            if (!Memory.DOSEN && (addr & 0x23) == (0xFE & 0x23))
                base.WritePortFE(addr, value, ref handled);
        }

        #endregion
    }

    public class UlaScorpionEx : UlaScorpion
    {
        public UlaScorpionEx()
        {
            Name = "Scorpion [Yellow - extended border]";
        }

        
        protected override SpectrumRendererParams CreateSpectrumRendererParams()
        {
            // Scorpion [Yellow PCB]
            // Total Size:          448 x 312
            // Visible Size:        352 x 296 (48+256+48 x 64+192+40)
            var timing = base.CreateSpectrumRendererParams();
            timing.c_ulaBorderTop = 64;
            timing.c_ulaBorderBottom = 40;
            timing.c_ulaBorderLeftT = 24;  //24
            timing.c_ulaBorderRightT = 24; //32

            timing.c_ulaWidth = (timing.c_ulaBorderLeftT + 128 + timing.c_ulaBorderRightT) * 2;
            timing.c_ulaHeight = (timing.c_ulaBorderTop + 192 + timing.c_ulaBorderBottom);
            return timing;
        }
    }
}
