using System;
using System.IO;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;
using ZXMAK2.Engine.Interfaces;

namespace ZXMAK2.Hardware.Atm
{
    public class AtmTxtRendererParams
    {
        public int c_ulaLineTime;
        public int c_ulaFirstPaperLine;
        public int c_ulaFirstPaperTact;
        public int c_frameTactCount;

        public int c_ulaBorderTop;
        public int c_ulaBorderBottom;
        public int c_ulaBorderLeftT;
        public int c_ulaBorderRightT;

        public int c_ulaIntBegin;
        public int c_ulaIntLength;

        public int c_ulaWidth;
        public int c_ulaHeight;
    }

    public class AtmTxtRenderer : IUlaRenderer
    {
        private AtmTxtRendererParams m_params;
        private uint[] m_palette;
        private readonly byte[] m_ulaSgen = new byte[256 * 8];

        private int[] m_videoOffset;
        private int[] m_memoryMask;
        private int[] m_ulaAddrTXT640BW;
        private int[] m_ulaAddrTXT640AT;
        private int[] m_ulaAddrTXT640CG;

        private readonly uint[] m_ink = new uint[0x100];
        private readonly uint[] m_paper = new uint[0x100];
        protected UlaAction[] m_ulaAction;


        protected byte[] m_memoryPageAt;
        protected byte[] m_memoryPageBw;
        protected int m_borderIndex = 0;    // current border index
        protected uint m_borderColor = 0;   // current border color


        #region IUlaRenderer

        public IFrameVideo VideoData { get; private set; }

        public virtual int FrameLength
        {
            get { return Params.c_frameTactCount; }
        }

        public virtual int IntLength
        {
            get { return Params.c_ulaIntLength; }
        }

        public virtual void UpdateBorder(int value)
        {
            m_borderIndex = value;
            m_borderColor = Palette[m_borderIndex & 0x0F];
        }

        public virtual void UpdatePalette(int index, uint value)
        {
            Palette[index] = value;
            UpdateBorder(m_borderIndex);
            // TODO: remove palette index substitution to remove OnPaletteChanged
            OnPaletteChanged();
        }

        public virtual void ReadFreeBus(int frameTact, ref byte value)
        {
            // not implemented
        }

        public unsafe virtual void Render(
            uint* bufPtr,
            int startTact,
            int endTact)
        {
            if (bufPtr == null || m_ulaAction == null)
                return;
            if (endTact > Params.c_frameTactCount)
                endTact = Params.c_frameTactCount;
            if (startTact > Params.c_frameTactCount)
                startTact = Params.c_frameTactCount;
            for (int tact = startTact; tact < endTact; tact++)
            {
                switch (m_ulaAction[tact])
                {
                    case UlaAction.None:
                        break;
                    case UlaAction.Border:
                        {
                            var offset = m_videoOffset[tact];
                            bufPtr[offset + 0] = m_borderColor;
                            bufPtr[offset + 1] = m_borderColor;
                            bufPtr[offset + 2] = m_borderColor;
                            bufPtr[offset + 3] = m_borderColor;
                        }
                        break;
                    case UlaAction.Paper:
                        {
                            var addrBw = m_ulaAddrTXT640BW[tact];
                            var addrAt = m_ulaAddrTXT640AT[tact];
                            var addrCg = m_ulaAddrTXT640CG[tact];
                            var bw = m_ulaSgen[(m_memoryPageBw[addrBw] << 3) + addrCg];
                            var at = m_memoryPageAt[addrAt];
                            var ink = m_ink[at];
                            var paper = m_paper[at];
                            var offset = m_videoOffset[tact];
                            var mask = m_memoryMask[tact];
                            for (var i = 0; i < 4; i++, mask >>= 1)
                            {
                                bufPtr[offset + i] = (bw & mask) != 0 ?
                                    ink :
                                    paper;
                            }
                        }
                        break;
                }
            }
        }

        public virtual void Frame()
        {
        }

        public virtual void LoadScreenData(Stream stream)
        {
            stream.Read(MemoryPageAt, 0, 0x4000);
            stream.Read(MemoryPageBw, 0, 0x4000);
        }

        public virtual void SaveScreenData(Stream stream)
        {
            stream.Write(MemoryPageAt, 0, 0x4000);
            stream.Write(MemoryPageBw, 0, 0x4000);
        }

        public virtual IUlaRenderer Clone()
        {
            var renderer = new AtmTxtRenderer();
            renderer.Params = this.Params;
            renderer.Palette = this.Palette;
            renderer.MemoryPageAt = this.MemoryPageAt;
            renderer.MemoryPageBw = this.MemoryPageBw;
            renderer.UpdateBorder(this.m_borderIndex);
            return renderer;
        }

        #endregion IUlaRenderer

        #region Public

        public AtmTxtRendererParams Params
        {
            get { return m_params; }
            set { ValidateParams(value); m_params = value; OnParamsChanged(); }
        }

        public uint[] Palette
        {
            get { return m_palette; }
            set { m_palette = value; UpdateBorder(m_borderIndex); OnPaletteChanged(); }
        }

        public byte[] MemoryPageAt
        {
            get { return m_memoryPageAt; }
            set { m_memoryPageAt = value; }
        }

        public byte[] MemoryPageBw
        {
            get { return m_memoryPageBw; }
            set { m_memoryPageBw = value; }
        }

        public void WriteSgen(int addr, byte value)
        {
            m_ulaSgen[addr & 0x7FF] = value;
        }

        #endregion


        public AtmTxtRenderer()
        {
            InitStaticTables();
            Params = CreateParams();
            Palette = SpectrumRenderer.CreatePalette();
        }

        /// <summary>
        /// Create default renderer params (ATM1 v4.50)
        /// </summary>
        public static AtmTxtRendererParams CreateParams()
        {
            // ATM1 v4.50
            // Total Size:          640 x 200 (TXT: 80 x 25)
            var timing = new AtmTxtRendererParams();
            timing.c_frameTactCount = 69888;
            timing.c_ulaLineTime = 224;
            timing.c_ulaFirstPaperLine = 56;
            timing.c_ulaFirstPaperTact = 32;

            timing.c_ulaBorderTop = 28;
            timing.c_ulaBorderBottom = 28;
            timing.c_ulaBorderLeftT = 0;
            timing.c_ulaBorderRightT = 0;

            timing.c_ulaIntLength = 32;

            timing.c_ulaWidth = (timing.c_ulaBorderLeftT + 160 + timing.c_ulaBorderRightT) * 4;
            timing.c_ulaHeight = timing.c_ulaBorderTop + 200 + timing.c_ulaBorderBottom;
            return timing;
        }

        public static void ValidateParams(AtmTxtRendererParams timing)
        {
            //...
        }

        protected virtual void OnParamsChanged()
        {
            VideoData = new FrameVideo(Params.c_ulaWidth, Params.c_ulaHeight, 2F);
            m_ulaAction = new UlaAction[Params.c_frameTactCount];
            m_videoOffset = new int[Params.c_frameTactCount];
            m_memoryMask = new int[Params.c_frameTactCount];
            m_ulaAddrTXT640BW = new int[Params.c_frameTactCount];
            m_ulaAddrTXT640AT = new int[Params.c_frameTactCount];
            m_ulaAddrTXT640CG = new int[Params.c_frameTactCount];
            for (var tact = 0; tact < m_ulaAction.Length; tact++)
            {
                var tvy = tact / Params.c_ulaLineTime;
                var tvx = tact - (tvy * Params.c_ulaLineTime);
                var zy = tvy - (Params.c_ulaFirstPaperLine - Params.c_ulaBorderTop);
                var zx = tvx - (Params.c_ulaFirstPaperTact - Params.c_ulaBorderLeftT);
                var y = zy - Params.c_ulaBorderTop;
                var x = zx - Params.c_ulaBorderLeftT;
                if (y >= 0 && y < 200 && x >= 0 && x < 160)
                {
                    m_ulaAction[tact] = UlaAction.Paper;
                    m_videoOffset[tact] = Params.c_ulaWidth * zy + 4 * zx;
                    m_memoryMask[tact] = (x & 1) == 0 ? 0x80 : 0x08;
                    x = x / 2;
                    var pageOffsetBw = (x & 1) == 0 ? 0x01C0 : 0x21C0;
                    var pageOffsetAt = ((x + 1) & 1) == 0 ? 0x01C0 : 0x21C0;
                    pageOffsetBw += x >> 1;
                    pageOffsetAt += (x + 1) >> 1;
                    pageOffsetBw += (y >> 3) * 64;
                    pageOffsetAt += (y >> 3) * 64;
                    m_ulaAddrTXT640BW[tact] = pageOffsetBw;
                    m_ulaAddrTXT640AT[tact] = pageOffsetAt;
                    m_ulaAddrTXT640CG[tact] = y & 7;
                }
                else if (zy >= 0 &&
                    zy < (200 + Params.c_ulaBorderTop + Params.c_ulaBorderBottom) &&
                    zx >= 0 &&
                    zx < (160 + Params.c_ulaBorderLeftT + Params.c_ulaBorderRightT))
                {
                    m_ulaAction[tact] = UlaAction.Border;
                    m_videoOffset[tact] = Params.c_ulaWidth * zy + 4 * zx;
                }
                else
                {
                    m_ulaAction[tact] = UlaAction.None;
                }
            }
        }

        protected virtual void OnPaletteChanged()
        {
            for (int at = 0; at < 256; at++)
            {
                var ulaInk = (at & 7) | ((at & 0x40) >> 3);
                var ulaPaper = ((at >> 3) & 7) | ((at & 0x80) >> 4);
                m_ink[at] = Palette[ulaInk];
                m_paper[at] = Palette[ulaPaper];
            }
        }

        private void InitStaticTables()
        {
            try
            {
                using (var stream = RomPack.GetUlaRomStream("ATM-SGEN"))
                {
                    stream.Read(m_ulaSgen, 0, m_ulaSgen.Length);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }


        protected enum UlaAction
        {
            None = 0,
            Border,
            Paper,
        }
    }
}
