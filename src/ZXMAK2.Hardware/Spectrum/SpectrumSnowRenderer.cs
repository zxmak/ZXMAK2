using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Hardware.Spectrum
{
    public class SpectrumSnowRenderer : SpectrumRenderer
    {
        private int m_ulaNoise = 0xA001;

        public int Snow { get; set; }

        public bool IsUlaFetch(int frameTact)
        {
            var do3 = m_ulaAction[frameTact];
            return do3 == UlaAction.BorderAndFetchA1 ||
                do3 == UlaAction.Shift1AndFetchB2 ||
                do3 == UlaAction.Shift1AndFetchA2 ||
                do3 == UlaAction.Shift2AndFetchB1 ||
                do3 == UlaAction.Shift2AndFetchA1;
        }

        public override unsafe void Render(
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
                var ulaDo = m_ulaAction[takt];
                // first do the same thing as in SpectrumRenderer
                switch (ulaDo)
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
                // next, do snow emulation
                if (Snow > 0 && ulaDo == UlaAction.BorderAndFetchB1)
                {
                    Snow--;
                    int addr = m_ulaAddrBw[takt];
                    if ((m_ulaNoise & 0x0F) > 9)
                        addr = (addr & 0x3F00) | (m_ulaNoise & 0x00FF);
                    else if ((m_ulaNoise & 3) != 0)
                        addr = ((addr - 1) & 0xFF) | (addr & 0x3F00);
                    m_fetchB1 = m_memoryPage[addr];
                    m_ulaNoise = (((m_ulaNoise >> 16) ^ (m_ulaNoise >> 13)) & 1) ^ ((m_ulaNoise << 1) + 1);
                }
                if (Snow > 0 && ulaDo == UlaAction.Shift1AndFetchB2)
                {
                    Snow--;
                    int addr = m_ulaAddrBw[takt];
                    if ((m_ulaNoise & 0x0F) > 9)
                        addr = (addr & 0x3F00) | (m_ulaNoise & 0x00FF);
                    else if ((m_ulaNoise & 3) != 0)
                        addr = ((addr - 1) & 0xFF) | (addr & 0x3F00);
                    m_fetchB2 = m_memoryPage[addr];
                    m_ulaNoise = (((m_ulaNoise >> 16) ^ (m_ulaNoise >> 13)) & 1) ^ ((m_ulaNoise << 1) + 1);
                }
                if (Snow > 0 && ulaDo == UlaAction.Shift2AndFetchB1)
                {
                    Snow--;
                    int addr = m_ulaAddrBw[takt];
                    if ((m_ulaNoise & 0x0F) > 9)
                        addr = (addr & 0x3F00) | (m_ulaNoise & 0x00FF);
                    else if ((m_ulaNoise & 3) != 0)
                        addr = ((addr - 1) & 0xFF) | (addr & 0x3F00);
                    m_fetchB1 = m_memoryPage[addr];
                    m_ulaNoise = (((m_ulaNoise >> 16) ^ (m_ulaNoise >> 13)) & 1) ^ ((m_ulaNoise << 1) + 1);
                }
            }
        }

        public override IUlaRenderer Clone()
        {
            var renderer = new SpectrumSnowRenderer();
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
    }
}
