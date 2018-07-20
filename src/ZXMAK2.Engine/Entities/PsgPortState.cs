

namespace ZXMAK2.Engine.Entities
{
    public class PsgPortState
    {
        private byte m_outState = 0;
        private byte m_oldOutState = 0;
        private byte m_inState = 0;
        private bool m_dirOut = true;

        public bool DirOut
        {
            get { return m_dirOut; }
            set { m_dirOut = value; }
        }

        public byte OutState
        {
            get { return m_outState; }
            set { m_oldOutState = m_outState; m_outState = value; }
        }

        public byte OldOutState
        {
            get { return m_oldOutState; }
        }

        public byte InState
        {
            get { return m_inState; }
            set { m_inState = value; }
        }

        public PsgPortState(byte value)
        {
            m_outState = value;
            m_oldOutState = value;
            m_inState = value;
        }
    }
}
