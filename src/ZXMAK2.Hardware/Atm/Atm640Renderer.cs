using System;
using System.IO;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;
using ZXMAK2.Engine.Interfaces;

namespace ZXMAK2.Hardware.Atm
{
    public class Atm640RendererParams
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

    public class Atm640Renderer : IUlaRenderer
    {
        private Atm640RendererParams m_params;
        private uint[] m_palette;

        private int[] m_videoOffset;
        private int[] m_memoryOffset;
        private int[] m_memoryMask;
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
                            var addr = m_memoryOffset[tact];
                            var bw = m_memoryPageBw[addr];
                            var at = m_memoryPageAt[addr];
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
            var renderer = new Atm640Renderer();
            renderer.Params = this.Params;
            renderer.Palette = this.Palette;
            renderer.MemoryPageAt = this.MemoryPageAt;
            renderer.MemoryPageBw = this.MemoryPageBw;
            renderer.UpdateBorder(this.m_borderIndex);
            return renderer;
        }

        #endregion IUlaRenderer

        #region Public

        public Atm640RendererParams Params
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

        #endregion


        public Atm640Renderer()
        {
            Params = CreateParams();
            Palette = SpectrumRenderer.CreatePalette();
        }

        /// <summary>
        /// Create default renderer params (ATM1 v4.50)
        /// </summary>
        public static Atm640RendererParams CreateParams()
        {
            // ATM1 v4.50
            // Total Size:          640 x 200
            var timing = new Atm640RendererParams();
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

        public static void ValidateParams(Atm640RendererParams timing)
        {
            //...
        }

        protected virtual void OnParamsChanged()
        {
            VideoData = new FrameVideo(Params.c_ulaWidth, Params.c_ulaHeight, 2F);
            m_ulaAction = new UlaAction[Params.c_frameTactCount];
            m_videoOffset = new int[Params.c_frameTactCount];
            m_memoryOffset = new int[Params.c_frameTactCount];
            m_memoryMask = new int[Params.c_frameTactCount];
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
                    x /= 2;
                    m_memoryOffset[tact] = (y * 80 + x) / 2 + 0x2000 * ((y * 80 + x) & 1);
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

        protected enum UlaAction
        {
            None = 0,
            Border,
            Paper,
        }
    }
}
