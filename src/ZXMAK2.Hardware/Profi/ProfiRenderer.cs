using System;
using System.IO;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;

namespace ZXMAK2.Hardware.Profi
{
    public class ProfiRendererParams
    {
        public int c_ulaLineTime;		// tacts per line
        public int c_ulaFirstPaperLine;	// tact for left top pixel
        public int c_ulaFirstPaperTact;		// tact for left pixel in line
        public int c_frameTactCount;	        // 59904 for profi mode (312x192)

        public int c_ulaBorderTop;
        public int c_ulaBorderBottom;
        public int c_ulaBorderLeftT;
        public int c_ulaBorderRightT;

        public int c_ulaIntBegin;
        public int c_ulaIntLength;

        public bool c_ulaProfiColor;

        public int c_ulaWidth;
        public int c_ulaHeight;
    }

    public class ProfiRenderer : IUlaRenderer
    {
        private ProfiRendererParams m_params;
        private uint[] m_palette;

        private UlaAction[] m_ulaAction;
        private int[] m_ulaBwOffset;
        private int[] m_ulaVideoOffset;
        private readonly uint[] m_ulaProfiInk = new uint[0x100];
        private readonly uint[] m_ulaProfiPaper = new uint[0x100];

        protected byte[] m_memoryCpmUlaBw;   // current b/w video ram for CPM mode
        protected byte[] m_memoryCpmUlaClr;	// current color video ram for CPM mode
        protected int m_borderIndex = 0;        // current border index
        protected uint m_borderColorInk = 0;    // current border color
        protected uint m_borderColorPaper = 0;  // current border color (inverse)

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
            m_borderColorInk = Palette[m_borderIndex & 7];
            m_borderColorPaper = Palette[(~m_borderIndex) & 7];
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
            uint* buffer,
            int startTact,
            int endTact)
        {
            if (buffer == null || m_ulaAction == null)
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
                    case UlaAction.Profi32_0:
                        {
                            var bufOffset = m_ulaVideoOffset[tact];
                            buffer[bufOffset + 0] = m_borderColorPaper;
                            buffer[bufOffset + 1] = m_borderColorPaper;
                            buffer[bufOffset + 2] = m_borderColorPaper;
                            buffer[bufOffset + 3] = m_borderColorPaper;
                        }
                        break;
                    case UlaAction.Profi32_1_BNW:
                        {
                            var offset = m_ulaBwOffset[tact];
                            var shr = m_memoryCpmUlaBw[offset];
                            var bufOffset = m_ulaVideoOffset[tact];
                            buffer[bufOffset + 0] = ((shr & 0x80) != 0) ? m_borderColorInk : m_borderColorPaper;
                            buffer[bufOffset + 1] = ((shr & 0x40) != 0) ? m_borderColorInk : m_borderColorPaper;
                            buffer[bufOffset + 2] = ((shr & 0x20) != 0) ? m_borderColorInk : m_borderColorPaper;
                            buffer[bufOffset + 3] = ((shr & 0x10) != 0) ? m_borderColorInk : m_borderColorPaper;
                        }
                        break;
                    case UlaAction.Profi32_1_CLR:
                        {
                            var offset = m_ulaBwOffset[tact];
                            var shr = m_memoryCpmUlaBw[offset];
                            var attr = m_memoryCpmUlaClr[offset];
                            var ink = m_ulaProfiInk[attr]; //Palette[(attr & 7) | ((attr >> 3)&0x8)]; // attr];
                            var paper = m_ulaProfiPaper[attr]; //Palette[((attr >> 3) & 7) | ((attr >> 4) & 0x8)]; // attr | ((attr & 0x80) >> 1)];
                            var bufOffset = m_ulaVideoOffset[tact];
                            buffer[bufOffset + 0] = ((shr & 0x80) != 0) ? ink : paper;
                            buffer[bufOffset + 1] = ((shr & 0x40) != 0) ? ink : paper;
                            buffer[bufOffset + 2] = ((shr & 0x20) != 0) ? ink : paper;
                            buffer[bufOffset + 3] = ((shr & 0x10) != 0) ? ink : paper;
                        }
                        break;
                    case UlaAction.Profi32_2_BNW:
                        {
                            var offset = m_ulaBwOffset[tact];
                            var shr = m_memoryCpmUlaBw[offset];
                            var bufOffset = m_ulaVideoOffset[tact];
                            buffer[bufOffset + 0] = ((shr & 0x08) != 0) ? m_borderColorInk : m_borderColorPaper;
                            buffer[bufOffset + 1] = ((shr & 0x04) != 0) ? m_borderColorInk : m_borderColorPaper;
                            buffer[bufOffset + 2] = ((shr & 0x02) != 0) ? m_borderColorInk : m_borderColorPaper;
                            buffer[bufOffset + 3] = ((shr & 0x01) != 0) ? m_borderColorInk : m_borderColorPaper;
                        }
                        break;
                    case UlaAction.Profi32_2_CLR:
                        {
                            var offset = m_ulaBwOffset[tact];
                            var shr = m_memoryCpmUlaBw[offset];
                            var attr = m_memoryCpmUlaClr[offset];
                            var ink = m_ulaProfiInk[attr]; //Palette[(attr & 7) | ((attr >> 3)&0x8)]; // attr];
                            var paper = m_ulaProfiPaper[attr]; //Palette[((attr >> 3) & 7) | ((attr >> 4) & 0x8)]; // attr | ((attr & 0x80) >> 1)];
                            var bufOffset = m_ulaVideoOffset[tact];
                            buffer[bufOffset + 0] = ((shr & 0x08) != 0) ? ink : paper;
                            buffer[bufOffset + 1] = ((shr & 0x04) != 0) ? ink : paper;
                            buffer[bufOffset + 2] = ((shr & 0x02) != 0) ? ink : paper;
                            buffer[bufOffset + 3] = ((shr & 0x01) != 0) ? ink : paper;
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
            stream.Read(MemoryCpmUlaBw, 0, 0x4000);
            stream.Read(MemoryCpmUlaClr, 0, 0x4000);
        }

        public virtual void SaveScreenData(Stream stream)
        {
            stream.Write(MemoryCpmUlaBw, 0, 0x4000);
            stream.Write(MemoryCpmUlaClr, 0, 0x4000);
        }

        public virtual IUlaRenderer Clone()
        {
            var renderer = new ProfiRenderer();
            renderer.Params = this.Params;
            renderer.Palette = this.Palette;
            renderer.MemoryCpmUlaBw = this.MemoryCpmUlaBw;
            renderer.MemoryCpmUlaClr = this.MemoryCpmUlaClr;
            renderer.UpdateBorder(this.m_borderIndex);
            return renderer;
        }

        #endregion IUlaRenderer


        #region Public

        public ProfiRendererParams Params
        {
            get { return m_params; }
            set { ValidateParams(value); m_params = value; OnParamsChanged(); }
        }

        public uint[] Palette
        {
            get { return m_palette; }
            set { m_palette = value; UpdateBorder(m_borderIndex); OnPaletteChanged(); }
        }

        public byte[] MemoryCpmUlaBw
        {
            get { return m_memoryCpmUlaBw; }
            set { m_memoryCpmUlaBw = value; }
        }

        public byte[] MemoryCpmUlaClr
        {
            get { return m_memoryCpmUlaClr; }
            set { m_memoryCpmUlaClr = value; }
        }

        #endregion Public


        public ProfiRenderer()
        {
            Params = CreateParams();
            Palette = SpectrumRenderer.CreatePalette();
        }

        /// <summary>
        /// Create default renderer params (PROFI 3.2)
        /// </summary>
        public static ProfiRendererParams CreateParams()
        {
            // PROFI 3.2
            // Total Size:          768 x 312
            // Visible Size:        640 x 240 (64+512+64 x 0+240+0)
            // SYNCGEN: SAMX6 (original)
            var timing = new ProfiRendererParams();
            timing.c_frameTactCount = 69888;	// 59904 for profi mode (312x192)

            timing.c_ulaIntBegin = 16+3;
            timing.c_ulaIntLength = 32 + 7;	// TODO: needs approve

            // profi mode timings...
            timing.c_ulaLineTime = 192;			// tacts per line
            timing.c_ulaFirstPaperLine = 72;    // 192	// tact for left top pixel
            timing.c_ulaFirstPaperTact = 8+16;			// tact for left pixel in line
            timing.c_ulaBorderTop = 8;
            timing.c_ulaBorderBottom = 8;
            timing.c_ulaBorderLeftT = 16;			// real 3.xx=6
            timing.c_ulaBorderRightT = 16;		    // real 3.xx=10
            timing.c_ulaProfiColor = false;

            timing.c_ulaWidth = (timing.c_ulaBorderLeftT + 128 + timing.c_ulaBorderRightT) * 4;
            timing.c_ulaHeight = timing.c_ulaBorderTop + 240 + timing.c_ulaBorderBottom;
            return timing;
        }


        public static void ValidateParams(ProfiRendererParams value)
        {
            //throw new NotImplementedException();
        }

        private void OnParamsChanged()
        {
            // rebuild tables...
            VideoData = new FrameVideo(Params.c_ulaWidth, Params.c_ulaHeight, 2F);
            m_ulaAction = new UlaAction[Params.c_frameTactCount];
            m_ulaBwOffset = new int[Params.c_frameTactCount];
            m_ulaVideoOffset = new int[Params.c_frameTactCount];

            for (var tact = 0; tact < Params.c_frameTactCount; tact++)
            {
                var tactScreen = tact + Params.c_ulaIntBegin;
                if (tactScreen < 0)
                {
                    tactScreen += Params.c_frameTactCount;
                }
                else if (tactScreen >= Params.c_frameTactCount)
                {
                    tactScreen -= Params.c_frameTactCount;
                }
                CalcTableItem(
                    tact,
                    tactScreen / Params.c_ulaLineTime,
                    tactScreen % Params.c_ulaLineTime);
            }
        }

        private void CalcTableItem(int item, int line, int lineTact)
        {
            m_ulaAction[item] = UlaAction.None;
            line -= Params.c_ulaFirstPaperLine - Params.c_ulaBorderTop;
            lineTact -= Params.c_ulaFirstPaperTact - Params.c_ulaBorderLeftT;
            if (line < 0 || 
                line >= (Params.c_ulaBorderTop+240+Params.c_ulaBorderBottom))
            {
                return;
            }
            if (lineTact < 0 || 
                lineTact >= (Params.c_ulaBorderLeftT + 128 + Params.c_ulaBorderRightT))
            {
                return;
            }
            if (line < Params.c_ulaBorderTop || 
                line >= (Params.c_ulaBorderTop+240))
            {
                // top/bottom border
                m_ulaVideoOffset[item] = line * Params.c_ulaWidth + lineTact * 4;
                if (m_ulaVideoOffset[item] < 0 ||
                    m_ulaVideoOffset[item] >= (1024 * 768 - 4))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "m_ulaVideoOffset[{0}] = {1} (top/bottom border)",
                            item,
                            m_ulaVideoOffset[item]));
                }
                m_ulaAction[item] = UlaAction.Profi32_0;
                return;
            }

            if (lineTact < Params.c_ulaBorderLeftT || 
                lineTact >= (Params.c_ulaBorderLeftT + 128))
            {
                // left/right border
                m_ulaVideoOffset[item] = line * Params.c_ulaWidth + lineTact * 4;
                if (m_ulaVideoOffset[item] < 0 || 
                    m_ulaVideoOffset[item] >= (1024 * 768 - 4))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "m_ulaVideoOffset[{0}] = {1} (left/right border)",
                            item,
                            m_ulaVideoOffset[item]));
                }
                m_ulaAction[item] = UlaAction.Profi32_0;
                return;
            }
            lineTact -= Params.c_ulaBorderLeftT;

            int x4 = lineTact;// -_ulaProfiLineBeginTact;
            if (x4 < 0 || x4 >= 512 / 4)
            {
                m_ulaAction[item] = UlaAction.None;
                return;
            }

            m_ulaVideoOffset[item] = line * Params.c_ulaWidth + Params.c_ulaBorderLeftT * 4 + x4 * 4;
            if (m_ulaVideoOffset[item] < 0 || 
                m_ulaVideoOffset[item] >= (1024 * 768 - 4))
            {
                throw new InvalidOperationException(
                    string.Format(
                        "m_ulaVideoOffset[{0}] = {1}",
                        item,
                        m_ulaVideoOffset[item]));
            }

            line -= Params.c_ulaBorderTop;
            var pixCoff = 2048 * (line >> 6) +
                256 * (line & 0x07) +
                ((line & 0x38) << 2);

            if ((x4 & 2) == 0)
                m_ulaBwOffset[item] = pixCoff + x4 / 4 + 8192;
            else
                m_ulaBwOffset[item] = pixCoff + x4 / 4;


            if ((x4 & 1) == 0)
                m_ulaAction[item] = Params.c_ulaProfiColor ?
                    UlaAction.Profi32_1_CLR :
                    UlaAction.Profi32_1_BNW;
            else
                m_ulaAction[item] = Params.c_ulaProfiColor ?
                    UlaAction.Profi32_2_CLR :
                    UlaAction.Profi32_2_BNW;
        }

        private void OnPaletteChanged()
        {
            for (int i = 0; i < 0x100; i++)
            {
                m_ulaProfiInk[i] = Palette[(i & 7) | ((i >> 3) & 0x8)];
                m_ulaProfiPaper[i] = Palette[((i >> 3) & 7) | ((i >> 4) & 0x8)];
            }
        }


        protected enum UlaAction
        {
            None = 0,
            Profi32_0,
            Profi32_1_CLR,
            Profi32_2_CLR,
            Profi32_1_BNW,
            Profi32_2_BNW,
        }
    }
}
