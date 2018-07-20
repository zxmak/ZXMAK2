using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZXMAK2.Host.Entities;
using ZXMAK2.Host.Entities.Tools;
using ZXMAK2.Host.Interfaces;

namespace ZXMAK2.Host.WinForms.Controls
{
    public sealed class WinFormsKeyboard : IHostKeyboard, IKeyboardState
    {
        private readonly Form _form;
        private readonly KeyboardStateMapper<Keys> _mapper = new KeyboardStateMapper<Keys>();
        private readonly Dictionary<Key, bool> _state = new Dictionary<Key, bool>();

        public WinFormsKeyboard(Form form)
        {
            if (form == null)
            {
                throw new ArgumentNullException("form");
            }
            _form = form;
            _mapper.LoadMapFromString(
                global::ZXMAK2.Host.WinForms.Properties.Resources.Keyboard_WinForms);
            foreach (var key in _mapper.Keys)
            {
                _state[key] = false;
            }
            _form.KeyDown += Form_OnKeyDown;
            _form.KeyUp += Form_OnKeyUp;
        }

        public void Dispose()
        {
        }


        #region IHostKeyboard

        public void Scan()
        {
        }

        public IKeyboardState State
        {
            get { return this; }
        }

        #endregion IHostKeyboard


        #region IKeyboardState

        public bool this[Key key]
        {
            get { return _state.ContainsKey(key) && _state[key]; }
        }

        #endregion IKeyboardState


        #region Private

        private void Form_OnKeyDown(object sender, KeyEventArgs e)
        {
            //Logger.Debug("KeyCode={0}, KeyValue={1}, Modifiers={2}", e.KeyCode, e.KeyValue, e.Modifiers);
            foreach (var key in _mapper.Keys.ToArray())
            {
                if (_mapper[key] == e.KeyCode)
                {
                    _state[key] = true;
                    //Logger.Debug("DN: {0}", key);
                }
            }
        }

        private void Form_OnKeyUp(object sender, KeyEventArgs e)
        {
            foreach (var key in _mapper.Keys.ToArray())
            {
                if (_mapper[key] == e.KeyCode)
                {
                    _state[key] = false;
                    //Logger.Debug("UP: {0}", key);
                }
            }
        }

        #endregion Private
    }
}
