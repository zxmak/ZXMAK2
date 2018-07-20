using System;
using System.IO;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;
using ZXMAK2.Engine.Interfaces;

namespace ZXMAK2.Hardware.Atm
{
    public class EvoA16Renderer : IUlaRenderer
    {
        private SpectrumRendererParams m_params;
        private uint[] m_palette;
        private byte[] m_atm_pal = new byte[16];

        private int[] m_videoOffset;
        private int[] m_memoryOffset;
        private bool[] m_memoryPage;
        private readonly uint[] m_ink0 = new uint[0x100];
        private readonly uint[] m_ink1 = new uint[0x100];
        protected UlaAction[] m_ulaAction;


        protected byte[] m_memoryPage0;
        protected byte[] m_memoryPage1;
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
                            bufPtr[offset] = m_borderColor;
                            bufPtr[offset + 1] = m_borderColor;
                        }
                        break;
                    case UlaAction.Paper:
                        {
                            var addr = m_memoryOffset[tact];
                            var bw = m_memoryPage[tact] ?
                                m_memoryPage0[addr] :
                                m_memoryPage1[addr];
                            var offset = m_videoOffset[tact];
                            bufPtr[offset] = m_ink0[bw];
                            bufPtr[offset + 1] = m_ink1[bw];
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
            stream.Read(MemoryPage0, 0, 0x4000);
            stream.Read(MemoryPage1, 0, 0x4000);
        }

        public virtual void SaveScreenData(Stream stream)
        {
            stream.Write(MemoryPage0, 0, 0x4000);
            stream.Write(MemoryPage1, 0, 0x4000);
        }

        public virtual IUlaRenderer Clone()
        {
            var renderer = new EvoA16Renderer();
            renderer.Params = this.Params;
            renderer.Palette = this.Palette;
            renderer.MemoryPage0 = this.MemoryPage0;
            renderer.MemoryPage1 = this.MemoryPage1;
            renderer.UpdateBorder(this.m_borderIndex);
            return renderer;
        }

        #endregion IUlaRenderer

        #region Public

        public SpectrumRendererParams Params
        {
            get { return m_params; }
            set { ValidateParams(value); m_params = value; OnParamsChanged(); }
        }

        public uint[] Palette
        {
            get { return m_palette; }
            set { m_palette = value; UpdateBorder(m_borderIndex); OnPaletteChanged(); }
        }

        public byte[] MemoryPage0
        {
            get { return m_memoryPage0; }
            set { m_memoryPage0 = value; }
        }

        public byte[] MemoryPage1
        {
            get { return m_memoryPage1; }
            set { m_memoryPage1 = value; }
        }

        #endregion


        public EvoA16Renderer()
        {
            Params = SpectrumRenderer.CreateParams();
            Palette = SpectrumRenderer.CreatePalette();
        }

        public static void ValidateParams(SpectrumRendererParams timing)
        {
            //...
        }

        protected virtual void OnParamsChanged()
        {
            VideoData = new FrameVideo(Params.c_ulaWidth, Params.c_ulaHeight, 1F);
            m_ulaAction = new UlaAction[Params.c_frameTactCount];
            m_videoOffset = new int[Params.c_frameTactCount];
            m_memoryOffset = new int[Params.c_frameTactCount];
            m_memoryPage = new bool[Params.c_frameTactCount];
            for (var tact = 0; tact < m_ulaAction.Length; tact++)
            {
                var tvy = tact / Params.c_ulaLineTime;
                var tvx = tact - (tvy * Params.c_ulaLineTime);
                var zy = tvy - (Params.c_ulaFirstPaperLine - Params.c_ulaBorderTop);
                var zx = tvx - (Params.c_ulaFirstPaperTact - Params.c_ulaBorderLeftT);
                var y = zy - Params.c_ulaBorderTop;
                var x = zx - Params.c_ulaBorderLeftT;
                if (y >= 0 && y < 192 && x >= 0 && x < 128)
                {
                    m_ulaAction[tact] = UlaAction.Paper;
                    m_videoOffset[tact] = Params.c_ulaWidth * zy + 2 * zx;
                    switch (x & 3)
                    {
                        case 0: m_memoryOffset[tact] = 0x0000 + CalcTableAddrBw(x, y); break; //  y * 128 / 4 + x / 4
                        case 1: m_memoryOffset[tact] = 0x0000 + CalcTableAddrBw(x, y); break;
                        case 2: m_memoryOffset[tact] = 0x2000 + CalcTableAddrBw(x, y); break;
                        case 3: m_memoryOffset[tact] = 0x2000 + CalcTableAddrBw(x, y); break;
                    }
                    m_memoryPage[tact] = (x & 1) == 0;
                }
                else if (zy >= 0 &&
                    zy < (192 + Params.c_ulaBorderTop + Params.c_ulaBorderBottom) &&
                    zx >= 0 &&
                    zx < (128 + Params.c_ulaBorderLeftT + Params.c_ulaBorderRightT))
                {
                    m_ulaAction[tact] = UlaAction.Border;
                    m_videoOffset[tact] = Params.c_ulaWidth * zy + 2 * zx;
                }
                else
                {
                    m_ulaAction[tact] = UlaAction.None;
                }
            }
        }

        /// <summary>
        /// Calculate pixel fetch address
        /// </summary>
        /// <param name="sx">Pixel area tact (x/2)</param>
        /// <param name="sy">Pixel area line</param>
        protected virtual int CalcTableAddrBw(int sx, int sy)
        {
            sx >>= 2;
            var vp = sx | (sy << 5);
            return (vp & 0x181F) | ((vp & 0x0700) >> 3) | ((vp & 0x00E0) << 3);
        }

        protected virtual void OnPaletteChanged()
        {
            for (int at = 0; at < 256; at++)
            {
                var ulaInk = (at & 7) | ((at & 0x40) >> 3);
                var ulaPaper = ((at >> 3) & 7) | ((at & 0x80) >> 4);
                m_ink0[at] = Palette[ulaInk];
                m_ink1[at] = Palette[ulaPaper];
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
