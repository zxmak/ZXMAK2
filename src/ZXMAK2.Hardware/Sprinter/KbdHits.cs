using System;
using System.Collections.Generic;


namespace ZXMAK2.Hardware.Sprinter
{
    public class KbdHitsItem
    {
        private int m_code;
        private int m_frames;
        private bool m_first;

        public int Code
        {
            get
            {
                return m_code;
            }
            set
            {
                m_code = value;
            }
        }
        public int Frames
        {
            get
            {
                return m_frames;
            }
            set
            {
                m_frames = value;
            }
        }
        public bool First
        {
            get
            {
                return m_first;
            }
            set
            {
                m_first= value;
            }
        }
        public KbdHitsItem(int Code) {
            m_code = Code;
            m_first=true;
            m_frames=0;
        }
    }

    class KbdHits
    {
        private List<KbdHitsItem> m_items = new List<KbdHitsItem>();

        public int Count
        {
            get
            {
                return m_items.Count;
            }
        }
        public List<KbdHitsItem> Items
        {
            get
            {
                return m_items;
            }
        }
        public bool Contains(int Code)
        {
            bool found = false;
            foreach (KbdHitsItem kbdi in m_items)
            {
                if (kbdi.Code == Code)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        public int Index(int Code)
        {
            int numb = -1, num1 = 0;
            foreach (KbdHitsItem kbdi in m_items)
            {
                if (kbdi.Code == Code)
                {
                    numb = num1;
                    break;
                }
                num1++;
            }
            return numb;
        }

        public void Add(int Code)
        {
            KbdHitsItem kbdi = new KbdHitsItem(Code);
            m_items.Add(kbdi);
        }
        public void Del(int Code)
        {
            if (Index(Code)>=0) {
                m_items.RemoveAt(Index(Code));
            }
        }
        public void Remove(int Index)
        {
            if (Index < m_items.Count)
            {
                m_items.RemoveAt(Index);
            }
        }
    }
    
}
