using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities.Tools;
using ZxmakKey = ZXMAK2.Host.Entities.Key;
using XnaKey = Microsoft.Xna.Framework.Input.Keys;


namespace ZXMAK2.Host.Xna4.Xna
{
    public sealed class XnaKeyboard : IHostKeyboard, IKeyboardState
    {
        private KeyboardStateMapper<XnaKey> m_mapper = new KeyboardStateMapper<XnaKey>();
        private readonly Dictionary<ZxmakKey, bool> m_state = new Dictionary<ZxmakKey, bool>();


        public XnaKeyboard()
        {
            m_mapper.LoadMapFromString(
                global::ZXMAK2.Host.Xna4.Properties.Resources.Keyboard_Xna);
        }

        public void Dispose()
        {
        }

        public void Update(KeyboardState state)
        {
            foreach (var key in m_mapper.Keys)
            {
                m_state[key] = state[m_mapper[key]] == KeyState.Down;
            }
        }


        #region IKeyboardState

        public bool this[ZxmakKey key]
        {
            get { return m_state.ContainsKey(key) && m_state[key]; }
        }

        #endregion IKeyboardState


        #region IHostKeyboard

        public IKeyboardState State
        {
            get { return this; }
        }

        public void Scan()
        {
        }

        //public void LoadConfiguration(string fileName)
        //{
        //    using (var reader = (TextReader)new StreamReader(fileName))
        //    {
        //        var xml = reader.ReadToEnd();
        //        var mapper = new KeyboardStateMapper<XnaKey>();
        //        mapper.LoadMapFromString(xml);
        //        m_mapper = mapper;
        //    }
        //}

        #endregion IHostKeyboard
    }
}
