using System;
using System.IO;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;

namespace ZXMAK2.Hardware
{
    public class SpectrumRendererParams
    {
        public int c_frameTactCount;
        public int c_ulaLineTime;
        public int c_ulaFirstPaperLine;
        public int c_ulaFirstPaperTact;
        public bool c_ulaBorder4T;
        public int c_ulaBorder4Tstage;

        public int c_ulaBorderTop;
        public int c_ulaBorderBottom;
        public int c_ulaBorderLeftT;
        public int c_ulaBorderRightT;

        public int c_ulaIntBegin;
        public int c_ulaIntLength;
        public int c_ulaFlashPeriod;

        public int c_ulaWidth;
        public int c_ulaHeight;
    }

    public class SpectrumRenderer : IUlaRenderer
    {
        private SpectrumRendererParams m_params;
        private uint[] m_palette;

        protected int[] m_ulaLineOffset;
        protected int[] m_ulaAddrBw;
        protected int[] m_ulaAddrAt;
        protected UlaAction[] m_ulaAction;
        protected readonly uint[] m_ulaInk = new uint[256 * 2];
        protected readonly uint[] m_ulaPaper = new uint[256 * 2];

        protected byte[] m_memoryPage;              // current video ram bank
        protected int m_flashState = 0;            // flash attr state (0/256)
        protected int m_flashCounter = 0;          // flash attr counter
        protected int m_borderIndex = 0;            // current border value
        protected uint m_borderColor = 0;           // current border color

        protected int m_fetchB1;
        protected int m_fetchA1;
        protected int m_fetchB2;
        protected int m_fetchA2;
        protected uint m_fetchInk;
        protected uint m_fetchPaper;
        protected uint m_fetchBorder;


        #region IUlaRenderer

        public IFrameVideo VideoData { get; private set; }

        public int FrameLength
        {
            get { return Params.c_frameTactCount; }
        }

        public int IntLength
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
            switch (m_ulaAction[frameTact])
            {
                case UlaAction.BorderAndFetchB1:
                case UlaAction.Shift1AndFetchB2:
                case UlaAction.Shift2AndFetchB1:
                    value = m_memoryPage[m_ulaAddrBw[frameTact]];
                    break;
                case UlaAction.BorderAndFetchA1:
                case UlaAction.Shift1AndFetchA2:
                case UlaAction.Shift2AndFetchA1:
                    value = m_memoryPage[m_ulaAddrAt[frameTact]];
                    break;
            }
        }

        public virtual unsafe void Render(
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
            // cache params...
            var c_ulaBorder4T = Params.c_ulaBorder4T;
            var c_ulaBorder4Tstage = Params.c_ulaBorder4Tstage;

            for (int takt = startTact; takt < endTact; takt++)
            {
                if (!c_ulaBorder4T || (takt & 3) == c_ulaBorder4Tstage)
                {
                    m_fetchBorder = m_borderColor;
                }
                switch (m_ulaAction[takt])
                {
                    case UlaAction.None:
                        break;
                    case UlaAction.Border:
                        {
                            var offset = m_ulaLineOffset[takt];
                            bufPtr[offset] = m_fetchBorder;
                            bufPtr[offset + 1] = m_fetchBorder;
                        }
                        break;
                    case UlaAction.BorderAndFetchB1:
                        {
                            var offset = m_ulaLineOffset[takt];
                            bufPtr[offset] = m_fetchBorder;
                            bufPtr[offset + 1] = m_fetchBorder;
                            m_fetchB1 = m_memoryPage[m_ulaAddrBw[takt]];
                        }
                        break;
                    case UlaAction.BorderAndFetchA1:
                        {
                            var offset = m_ulaLineOffset[takt];
                            bufPtr[offset] = m_fetchBorder;
                            bufPtr[offset + 1] = m_fetchBorder;
                            m_fetchA1 = m_memoryPage[m_ulaAddrAt[takt]];
                            m_fetchInk = m_ulaInk[m_fetchA1 + m_flashState];
                            m_fetchPaper = m_ulaPaper[m_fetchA1 + m_flashState];
                        }
                        break;
                    case UlaAction.Shift1AndFetchB2:
                        {
                            var offset = m_ulaLineOffset[takt];
                            bufPtr[offset] = ((m_fetchB1 & 0x80) != 0) ? m_fetchInk : m_fetchPaper;
                            bufPtr[offset + 1] = ((m_fetchB1 & 0x40) != 0) ? m_fetchInk : m_fetchPaper;
                            m_fetchB1 <<= 2;
                            m_fetchB2 = m_memoryPage[m_ulaAddrBw[takt]];
                        }
                        break;
                    case UlaAction.Shift1AndFetchA2:
                        {
                            var offset = m_ulaLineOffset[takt];
                            bufPtr[offset] = ((m_fetchB1 & 0x80) != 0) ? m_fetchInk : m_fetchPaper;
                            bufPtr[offset + 1] = ((m_fetchB1 & 0x40) != 0) ? m_fetchInk : m_fetchPaper;
                            m_fetchB1 <<= 2;
                            m_fetchA2 = m_memoryPage[m_ulaAddrAt[takt]];
                        }
                        break;
                    case UlaAction.Shift1:
                        {
                            var offset = m_ulaLineOffset[takt];
                            bufPtr[offset] = ((m_fetchB1 & 0x80) != 0) ? m_fetchInk : m_fetchPaper;
                            bufPtr[offset + 1] = ((m_fetchB1 & 0x40) != 0) ? m_fetchInk : m_fetchPaper;
                            m_fetchB1 <<= 2;
                        }
                        break;
                    case UlaAction.Shift1Last:
                        {
                            var offset = m_ulaLineOffset[takt];
                            bufPtr[offset] = ((m_fetchB1 & 0x80) != 0) ? m_fetchInk : m_fetchPaper;
                            bufPtr[offset + 1] = ((m_fetchB1 & 0x40) != 0) ? m_fetchInk : m_fetchPaper;
                            m_fetchB1 <<= 2;
                            m_fetchInk = m_ulaInk[m_fetchA2 + m_flashState];
                            m_fetchPaper = m_ulaPaper[m_fetchA2 + m_flashState];
                        }
                        break;
                    case UlaAction.Shift2:
                        {
                            var offset = m_ulaLineOffset[takt];
                            bufPtr[offset] = ((m_fetchB2 & 0x80) != 0) ? m_fetchInk : m_fetchPaper;
                            bufPtr[offset + 1] = ((m_fetchB2 & 0x40) != 0) ? m_fetchInk : m_fetchPaper;
                            m_fetchB2 <<= 2;
                        }
                        break;
                    case UlaAction.Shift2AndFetchB1:
                        {
                            var offset = m_ulaLineOffset[takt];
                            bufPtr[offset] = ((m_fetchB2 & 0x80) != 0) ? m_fetchInk : m_fetchPaper;
                            bufPtr[offset + 1] = ((m_fetchB2 & 0x40) != 0) ? m_fetchInk : m_fetchPaper;
                            m_fetchB2 <<= 2;
                            m_fetchB1 = m_memoryPage[m_ulaAddrBw[takt]];
                        }
                        break;
                    case UlaAction.Shift2AndFetchA1:
                        {
                            var offset = m_ulaLineOffset[takt];
                            bufPtr[offset] = ((m_fetchB2 & 0x80) != 0) ? m_fetchInk : m_fetchPaper;
                            bufPtr[offset + 1] = ((m_fetchB2 & 0x40) != 0) ? m_fetchInk : m_fetchPaper;
                            m_fetchB2 <<= 2;
                            m_fetchA1 = m_memoryPage[m_ulaAddrAt[takt]];
                            m_fetchInk = m_ulaInk[m_fetchA1 + m_flashState];
                            m_fetchPaper = m_ulaPaper[m_fetchA1 + m_flashState];
                        }
                        break;
                }
            }
        }

        public virtual void Frame()
        {
            m_flashCounter++;
            if (m_flashCounter >= Params.c_ulaFlashPeriod)
            {
                m_flashState ^= 256;
                m_flashCounter = 0;
            }
        }

        public virtual void LoadScreenData(Stream stream)
        {
            stream.Read(MemoryPage, 0, 6912);
        }

        public virtual void SaveScreenData(Stream stream)
        {
            stream.Write(MemoryPage, 0, 6912);
        }

        public virtual IUlaRenderer Clone()
        {
            var renderer = new SpectrumRenderer();
            renderer.Params = this.Params;
            renderer.Palette = this.Palette;
            renderer.MemoryPage = this.MemoryPage;
            renderer.m_flashState = this.m_flashState;
            renderer.m_flashCounter = this.m_flashCounter;
            renderer.UpdateBorder(this.m_borderIndex);
            renderer.m_fetchB1 = this.m_fetchB1;
            renderer.m_fetchA1 = this.m_fetchA1;
            renderer.m_fetchB2 = this.m_fetchB2;
            renderer.m_fetchA2 = this.m_fetchA2;
            renderer.m_fetchInk = this.m_fetchInk;
            renderer.m_fetchPaper = this.m_fetchPaper;
            renderer.m_fetchBorder = m_fetchBorder;
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

        public byte[] MemoryPage
        {
            get { return m_memoryPage; }
            set { m_memoryPage = value; }
        }

        #endregion


        public SpectrumRenderer()
        {
            //Params = CreateParams();
            //Palette = CreatePalette();
        }

        /// <summary>
        /// Create default renderer params (Pentagon 128K)
        /// </summary>
        public static SpectrumRendererParams CreateParams()
        {
            // Pentagon 128K
            // Total Size:          448 x 320
            // Visible Size:        320 x 240 (32+256+32 x 24+192+24)
            var timing = new SpectrumRendererParams();
            timing.c_frameTactCount = 71680;
            timing.c_ulaLineTime = 224;
            timing.c_ulaFirstPaperLine = 80;
            timing.c_ulaFirstPaperTact = 65;// 68;      // 68 [32sync+36border+128scr+28border]
            timing.c_ulaBorder4T = false;
            timing.c_ulaBorder4Tstage = 1;

            timing.c_ulaBorderTop = 32;
            timing.c_ulaBorderBottom = 32;
            timing.c_ulaBorderLeftT = 16;
            timing.c_ulaBorderRightT = 16;

            timing.c_ulaIntBegin = 0;
            timing.c_ulaIntLength = 32;
            timing.c_ulaFlashPeriod = 25;

            timing.c_ulaWidth = (timing.c_ulaBorderLeftT + 128 + timing.c_ulaBorderRightT) * 2;
            timing.c_ulaHeight = timing.c_ulaBorderTop + 192 + timing.c_ulaBorderBottom;
            return timing;
        }

        /// <summary>
        /// Create default palette
        /// </summary>
        public static uint[] CreatePalette()
        {
            return new uint[16]
            { 
                0xFF000000, 0xFF0000AA, 0xFFAA0000, 0xFFAA00AA, 
                0xFF00AA00, 0xFF00AAAA, 0xFFAAAA00, 0xFFAAAAAA,
                0xFF000000, 0xFF0000FF, 0xFFFF0000, 0xFFFF00FF, 
                0xFF00FF00, 0xFF00FFFF, 0xFFFFFF00, 0xFFFFFFFF,
            };
        }

        public static void ValidateParams(SpectrumRendererParams timing)
        {
            if (timing.c_ulaIntBegin <= -timing.c_frameTactCount ||
                timing.c_ulaIntBegin >= timing.c_frameTactCount)
            {
                throw new ArgumentException("ulaIntBegin");
            }
            if (timing.c_ulaIntLength <= -timing.c_frameTactCount ||
                timing.c_ulaIntLength >= timing.c_frameTactCount)
            {
                throw new ArgumentException("ulaIntLength");
            }
            if (timing.c_ulaWidth != (timing.c_ulaBorderLeftT + 128 + timing.c_ulaBorderRightT) * 2 ||
                timing.c_ulaHeight != (timing.c_ulaBorderTop + 192 + timing.c_ulaBorderBottom))
            {
                throw new ArgumentException("width/height");
            }
            if (timing.c_ulaLineTime < 128)
            {
                throw new ArgumentException("ulaLineTime");
            }
            if (timing.c_frameTactCount < timing.c_ulaLineTime * 192)
            {
                throw new ArgumentException("frameTactCount/ulaLineTime");
            }
            //...
        }

        protected virtual void OnParamsChanged()
        {
            // rebuild tables...
            VideoData = new FrameVideo(Params.c_ulaWidth, Params.c_ulaHeight, 1F);
            m_ulaLineOffset = new int[Params.c_frameTactCount];
            m_ulaAddrBw = new int[Params.c_frameTactCount];
            m_ulaAddrAt = new int[Params.c_frameTactCount];
            m_ulaAction = new UlaAction[Params.c_frameTactCount];

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
            //{
            //    var xml = new System.Xml.XmlDocument();
            //    var root = xml.AppendChild(xml.CreateElement("ULA"));
            //    for (var i = 0; i < Params.c_frameTactCount; i++)
            //    {
            //        var xe = xml.CreateElement("Item");
            //        xe.SetAttribute("tact", i.ToString("D5"));
            //        xe.SetAttribute("offset", m_ulaLineOffset[i].ToString("D6"));
            //        xe.SetAttribute("y", (m_ulaLineOffset[i] / Params.c_ulaWidth).ToString("D3"));
            //        xe.SetAttribute("x", (m_ulaLineOffset[i] % Params.c_ulaWidth).ToString("D3"));
            //        xe.SetAttribute("addrBw", m_ulaAddrBw[i].ToString("X4"));
            //        xe.SetAttribute("addrAt", m_ulaAddrAt[i].ToString("X4"));
            //        xe.SetAttribute("action", m_ulaAction[i].ToString());
            //        root.AppendChild(xe);
            //    }
            //    xml.Save("_ulaAction.xml");
            //}
        }

        /// <summary>
        /// Calculate table item
        /// </summary>
        /// <param name="item">item number</param>
        /// <param name="line">TV line number</param>
        /// <param name="pix">TV pixel pair number</param>
        private void CalcTableItem(int item, int line, int pix)
        {
            m_ulaAction[item] = UlaAction.None;
            int pitchWidth = Params.c_ulaWidth;

            int scrPix = pix - Params.c_ulaFirstPaperTact;
            int scrLin = line - Params.c_ulaFirstPaperLine;

            if ((line >= (Params.c_ulaFirstPaperLine - Params.c_ulaBorderTop)) && (line < (Params.c_ulaFirstPaperLine + 192 + Params.c_ulaBorderBottom)) &&
                (pix >= (Params.c_ulaFirstPaperTact - Params.c_ulaBorderLeftT)) && (pix < (Params.c_ulaFirstPaperTact + 128 + Params.c_ulaBorderRightT)))
            {
                // visibleArea (vertical)
                if ((line >= Params.c_ulaFirstPaperLine) && (line < (Params.c_ulaFirstPaperLine + 192)) &&
                    (pix >= Params.c_ulaFirstPaperTact) && (pix < (Params.c_ulaFirstPaperTact + 128)))
                {
                    // pixel area
                    switch (scrPix & 7)
                    {
                        case 0:
                            m_ulaAction[item] = UlaAction.Shift1AndFetchB2;   // shift 1 + fetch B2
                            // +4 = prefetch!
                            m_ulaAddrBw[item] = CalcTableAddrBw(scrPix + 4, scrLin);
                            break;
                        case 1:
                            m_ulaAction[item] = UlaAction.Shift1AndFetchA2;   // shift 1 + fetch A2
                            // +3 = prefetch!
                            m_ulaAddrAt[item] = CalcTableAddrAt(scrPix + 3, scrLin);
                            break;
                        case 2:
                            m_ulaAction[item] = UlaAction.Shift1;   // shift 1
                            break;
                        case 3:
                            m_ulaAction[item] = UlaAction.Shift1Last;   // shift 1 (last)
                            break;
                        case 4:
                            m_ulaAction[item] = UlaAction.Shift2;   // shift 2
                            break;
                        case 5:
                            m_ulaAction[item] = UlaAction.Shift2;   // shift 2
                            break;
                        case 6:
                            if (pix < (Params.c_ulaFirstPaperTact + 128 - 2))
                            {
                                m_ulaAction[item] = UlaAction.Shift2AndFetchB1;   // shift 2 + fetch B2
                            }
                            else
                            {
                                m_ulaAction[item] = UlaAction.Shift2;             // shift 2
                            }

                            // +2 = prefetch!
                            m_ulaAddrBw[item] = CalcTableAddrBw(scrPix + 2, scrLin);
                            break;
                        case 7:
                            if (pix < (Params.c_ulaFirstPaperTact + 128 - 2))
                            {
                                //???
                                m_ulaAction[item] = UlaAction.Shift2AndFetchA1;   // shift 2 + fetch A2
                            }
                            else
                            {
                                m_ulaAction[item] = UlaAction.Shift2;             // shift 2
                            }

                            // +1 = prefetch!
                            m_ulaAddrAt[item] = CalcTableAddrAt(scrPix + 1, scrLin);
                            break;
                    }
                }
                else if ((line >= Params.c_ulaFirstPaperLine) && (line < (Params.c_ulaFirstPaperLine + 192)) &&
                         (pix == (Params.c_ulaFirstPaperTact - 2)))  // border & fetch B1
                {
                    m_ulaAction[item] = UlaAction.BorderAndFetchB1; // border & fetch B1
                    // +2 = prefetch!
                    m_ulaAddrBw[item] = CalcTableAddrBw(scrPix + 2, scrLin);
                }
                else if ((line >= Params.c_ulaFirstPaperLine) && (line < (Params.c_ulaFirstPaperLine + 192)) &&
                         (pix == (Params.c_ulaFirstPaperTact - 1)))  // border & fetch A1
                {
                    m_ulaAction[item] = UlaAction.BorderAndFetchA1; // border & fetch A1
                    // +1 = prefetch!
                    m_ulaAddrAt[item] = CalcTableAddrAt(scrPix + 1, scrLin);
                }
                else
                {
                    m_ulaAction[item] = UlaAction.Border; // border
                }

                int wy = line - (Params.c_ulaFirstPaperLine - Params.c_ulaBorderTop);
                int wx = (pix - (Params.c_ulaFirstPaperTact - Params.c_ulaBorderLeftT)) * 2;
                m_ulaLineOffset[item] = wy * pitchWidth + wx;
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

        /// <summary>
        /// Calculate attribute fetch address
        /// </summary>
        /// <param name="sx">Pixel area tact (x/2)</param>
        /// <param name="sy">Pixel area line</param>
        protected virtual int CalcTableAddrAt(int sx, int sy)
        {
            sx >>= 2;
            var ap = sx | ((sy >> 3) << 5);
            return 6144 + ap;
        }

        protected virtual void OnPaletteChanged()
        {
            for (int atd = 0; atd < 256; atd++)
            {
                m_ulaInk[atd] = Palette[(atd & 7) + ((atd & 0x40) >> 3)];
                m_ulaPaper[atd] = Palette[((atd >> 3) & 7) + ((atd & 0x40) >> 3)];
                if ((atd & 0x80) != 0)
                {
                    m_ulaInk[atd + 256] = Palette[((atd >> 3) & 7) + ((atd & 0x40) >> 3)];
                    m_ulaPaper[atd + 256] = Palette[(atd & 7) + ((atd & 0x40) >> 3)];
                }
                else
                {
                    m_ulaInk[atd + 256] = Palette[(atd & 7) + ((atd & 0x40) >> 3)];
                    m_ulaPaper[atd + 256] = Palette[((atd >> 3) & 7) + ((atd & 0x40) >> 3)];
                }
            }
        }


        protected enum UlaAction
        {
            None = 0,
            Border,
            BorderAndFetchB1,
            BorderAndFetchA1,
            Shift1AndFetchB2,
            Shift1AndFetchA2,
            Shift1,
            Shift1Last,
            Shift2,
            Shift2AndFetchB1,
            Shift2AndFetchA1,
        }
    }
}
