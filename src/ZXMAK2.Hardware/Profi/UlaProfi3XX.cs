using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Hardware.Profi
{
    public class UlaProfi3XX : UlaDeviceBase
    {
        #region Fields

        private bool m_profiMode = false;
        private int m_pageBw = 4;
        private int m_pageClr = 0x38;

        protected ProfiRenderer ProfiRenderer = new ProfiRenderer();

        #endregion Fields


        public UlaProfi3XX()
        {
            Name = "PROFI 3.xx";
        }



        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            base.BusInit(bmgr);
            bmgr.Events.SubscribeRdIo(0x0000, 0x0000, ReadPortAll);
        }

        #endregion

        #region Bus Handlers

        protected override void WriteMem0000(ushort addr, byte value)
        {
            if (m_profiMode && (m_pageBw == m_page0000 || m_pageClr == m_page0000))
                UpdateState((int)(CPU.Tact % FrameTactCount));
            else
                base.WriteMem0000(addr, value);
        }

        protected override void WriteMem4000(ushort addr, byte value)
        {
            if (m_profiMode && (m_pageBw == m_page4000 || m_pageClr == m_page4000))
                UpdateState((int)(CPU.Tact % FrameTactCount));
            else
                base.WriteMem4000(addr, value);
        }

        protected override void WriteMem8000(ushort addr, byte value)
        {
            if (m_profiMode && (m_pageBw == m_page8000 || m_pageClr == m_page8000))
                UpdateState((int)(CPU.Tact % FrameTactCount));
            else
                base.WriteMem8000(addr, value);
        }

        protected override void WriteMemC000(ushort addr, byte value)
        {
            if (m_profiMode && (m_pageBw == m_pageC000 || m_pageClr == m_pageC000))
                UpdateState((int)(CPU.Tact % FrameTactCount));
            else
                base.WriteMemC000(addr, value);
        }

        protected override void WritePortFE(ushort addr, byte value, ref bool handled)
        {
            if ((addr & 0x67) != (0xFE & 0x67) || Memory.DOSEN)
            {
                return;
            }
            base.WritePortFE(addr, value, ref handled);
        }

        protected virtual void ReadPortAll(ushort addr, ref byte value, ref bool handled)
        {
            if ((addr & 0xFF) == 0xFF)
            {
                // Port #FF emulation
                int frameTact = (int)((CPU.Tact - 1) % FrameTactCount);
                Renderer.ReadFreeBus(frameTact, ref value);
            }
        }

        #endregion

        #region UlaDeviceBase

        protected override IMemoryDevice Memory
        {
            set
            {
                base.Memory = value;
                var pageBw = Memory.RamPages.Length > m_pageBw ?
                    Memory.RamPages[m_pageBw] :
                    new byte[0x4000];
                var pageClr = Memory.RamPages.Length > m_pageClr ?
                    Memory.RamPages[m_pageClr] :
                    new byte[0x4000];
                ProfiRenderer.MemoryCpmUlaBw = pageBw;
                ProfiRenderer.MemoryCpmUlaClr = pageClr;
            }
        }

        #endregion


        protected override void OnRendererInit()
        {
            var palette = SpectrumRenderer.CreatePalette();
            SpectrumRenderer.Palette = palette;
            ProfiRenderer.Palette = palette;
            SpectrumRenderer.Params = CreateSpectrumRendererParams();
            ProfiRenderer.Params = CreateProfiRendererParams();
        }

        protected virtual ProfiRendererParams CreateProfiRendererParams()
        {
            return ProfiRenderer.CreateParams();
        }

        protected override SpectrumRendererParams CreateSpectrumRendererParams()
        {
            // PROFI 3.2
            // Total Size:          768 x 312
            // Visible Size:        640 x 240 (64+512+64 x 0+240+0)
            // SYNCGEN: SAMX6 (original)
            var timing = SpectrumRenderer.CreateParams();
            timing.c_frameTactCount = 69888;	// 59904 for profi mode (312x192)
            timing.c_ulaLineTime = 224;
            timing.c_ulaFirstPaperLine = 56;
            timing.c_ulaFirstPaperTact = 39;//42;
            timing.c_ulaBorder4T = false;   // TODO: check?
            timing.c_ulaBorder4Tstage = 1;  // TODO: check?

            timing.c_ulaBorderTop = 32;
            timing.c_ulaBorderBottom = 32;
            timing.c_ulaBorderLeftT = 16;
            timing.c_ulaBorderRightT = 16;

            timing.c_ulaIntBegin = 0;
            timing.c_ulaIntLength = 32 + 7;	// TODO: needs approve
            timing.c_ulaFlashPeriod = 25;   // TODO: check?

            timing.c_ulaWidth = (timing.c_ulaBorderLeftT + 128 + timing.c_ulaBorderRightT) * 2;
            timing.c_ulaHeight = timing.c_ulaBorderTop + 192 + timing.c_ulaBorderBottom;
            return timing;
        }

        public override void SetPageMapping(
            int videoPage, 
            int page0000, 
            int page4000, 
            int page8000, 
            int pageC000)
        {
            base.SetPageMapping(videoPage, page0000, page4000, page8000, pageC000);
            m_profiMode = false;
            Renderer = SpectrumRenderer;
        }

        public void SetPageMappingProfi(
            bool ds80, 
            int videoPage, 
            int page0000, 
            int page4000, 
            int page8000, 
            int pageC000)
        {
            base.SetPageMapping(videoPage, page0000, page4000, page8000, pageC000);
            if (Memory.RamPages.Length < 0x40)
            {
                m_profiMode = false;
                Renderer = SpectrumRenderer;
                return;
            }
            m_profiMode = ds80;
            Renderer = m_profiMode ?
                (IUlaRenderer)ProfiRenderer :
                (IUlaRenderer)SpectrumRenderer;
            bool polek = videoPage == 7;
            m_pageBw = polek ? 0x06 : 0x04;
            m_pageClr = polek ? 0x3A : 0x38;
            ProfiRenderer.MemoryCpmUlaBw = Memory.RamPages[m_pageBw];
            ProfiRenderer.MemoryCpmUlaClr = Memory.RamPages[m_pageClr];
            //_ulaMemory =        polek ? Memory.RamPages[7]    : Memory.RamPages[5];
        }
    }
}
