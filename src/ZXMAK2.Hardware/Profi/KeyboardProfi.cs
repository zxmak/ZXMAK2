using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;


namespace ZXMAK2.Hardware.Profi
{
    public class KeyboardProfi : BusDeviceBase, IKeyboardDevice
    {
        #region Fields

        private const long ExtKeyMask = 0x10000000000;

        private IMemoryDevice m_memory;
        private IKeyboardState m_keyboardState;

        /// <summary>
        /// Spectrum keyboard state. 
        /// Each 5 bits represents: #7FFE, #BFFE, #DFFE, #EFFE, #F7FE, #FBFE, #FDFE, #FEFE.
        /// </summary>
        private long m_intState = 0;

        #endregion Fields


        public KeyboardProfi()
        {
            Category = BusDeviceCategory.Keyboard;
            Name = "KEYBOARD PROFI";
            Description = "PROFI Keyboard with extended keys\r\nPort: #FE\r\nMask: #01";
        }


        #region IBusDevice

        public override void BusInit(IBusManager bmgr)
        {
            m_memory = bmgr.FindDevice<IMemoryDevice>();
            bmgr.Events.SubscribeRdIo(0x67, 0xFE & 0x67, ReadPortFE);
        }

        public override void BusConnect()
        {
        }

        public override void BusDisconnect()
        {
        }

        #endregion IBusDevice


        #region IKeyboardDevice

        public IKeyboardState KeyboardState
        {
            get { return m_keyboardState; }
            set
            {
                m_keyboardState = value;
                m_intState = scanState(m_keyboardState);
            }
        }

        #endregion


        #region Bus Handlers

        protected virtual void ReadPortFE(ushort addr, ref byte value, ref bool handled)
        {
            if (handled || m_memory.DOSEN)
                return;
            //handled = true;

            value &= (m_intState & ExtKeyMask) != 0 ?
                (byte)0xC0 :
                (byte)0xE0;
            value |= (byte)(ScanKbdPort(addr) & 0x1F);
        }

        #endregion


        /// <summary>
        /// Scans keyboard state for specified port
        /// </summary>
        protected int ScanKbdPort(ushort port)
        {
            byte val = 0x1F;
            int msk = 0x0100;
            for (int i = 0; i < 8; i++, msk <<= 1)
            {
                if ((port & msk) == 0)
                {
                    var state = (int)(m_intState >> (i * 5));
                    state = state & 0x1F;
                    val &= (byte)(state ^ 0x1F);
                }
            }
            return val;
        }

        #region Scan

        private long scanState(IKeyboardState state)
        {
            if (state == null || ((state[Key.LeftAlt] || state[Key.RightAlt]) && state[Key.Return]))
                return 0;
            long value = 0;
            value = (value << 5) | scan_7FFE(state) | value & ExtKeyMask;    // BNM[symbol][space]
            value = (value << 5) | scan_BFFE(state) | value & ExtKeyMask;    // HJKL[enter]
            value = (value << 5) | scan_DFFE(state) | value & ExtKeyMask;    // YUIOP
            value = (value << 5) | scan_EFFE(state) | value & ExtKeyMask;    // 67890
            value = (value << 5) | scan_F7FE(state) | value & ExtKeyMask;    // 54321
            value = (value << 5) | scan_FBFE(state) | value & ExtKeyMask;    // QWERT
            value = (value << 5) | scan_FDFE(state) | value & ExtKeyMask;    // GFDSA
            value = (value << 5) | scan_FEFE(state) | value & ExtKeyMask;    // VCXZ[caps]
            return value;
        }

        /// <summary>
        /// #7FFE: BNM[symbol][space]    +'
        /// </summary>
        private static long scan_7FFE(IKeyboardState state)
        {
            var res = 0L;
            if (state[Key.Space]) res |= 1;
            if (state[Key.RightShift] || state[Key.LeftShift]) res |= 2;
            if (state[Key.M]) res |= 4;
            if (state[Key.N]) res |= 8;
            if (state[Key.B]) res |= 16;

            if (state[Key.LeftAlt]) res |= 2;       // SS+ENTER
            if (state[Key.RightAlt]) res |= 2 | 1;  // SS+SPACE
            if (state[Key.F11]) res |= 2;           // SS+Q
            if (state[Key.F12]) res |= 2;           // SS+W
            if (state[Key.BackSlash]) res |= 2;     // SS+D
            if (state[Key.LeftBracket]) res |= 2;   // SS+Y
            if (state[Key.RightBracket]) res |= 2;  // SS+U

            if (state[Key.F2]) res |= 16 | ExtKeyMask;      // B + b6
            if (state[Key.PageUp]) res |= 4 | ExtKeyMask;   // M + b6
            if (state[Key.PageDown]) res |= 8 | ExtKeyMask; // N + b6

            if (state[Key.CapsLock] || state[Key.NumPadPlus] || state[Key.NumPadMinus] || state[Key.NumPadStar] || state[Key.NumPadSlash])
                res |= 2;                                                            // numpad CapsLock +-*/
            if (state[Key.Period] || state[Key.Comma] || state[Key.SemiColon] ||
                state[Key.Apostrophe] || state[Key.Slash] ||
                state[Key.Minus] || state[Key.Equals] || state[Key.LeftBracket] ||
                state[Key.RightBracket])
                res |= 2;                                                            // SS for .,;"/-=[]

            if (state[Key.NumPadStar]) res |= 16;                                    // * = SS+B
            if (!state[Key.RightShift] && !state[Key.LeftShift])
            {
                if (state[Key.Period]) res |= 4;
                if (state[Key.Comma]) res |= 8;
            }
            return res;
        }

        /// <summary>
        /// #BFFE: HJKL[enter]
        /// </summary>
        private static long scan_BFFE(IKeyboardState state)
        {
            var res = 0L;
            if (state[Key.Return]) res |= 1;
            if (state[Key.L]) res |= 2;
            if (state[Key.K]) res |= 4;
            if (state[Key.J]) res |= 8;
            if (state[Key.H]) res |= 16;

            if (state[Key.LeftAlt]) res |= 1;       // SS+ENTER

            if (state[Key.F8]) res |= 16 | ExtKeyMask;  // H + b6
            if (state[Key.F10]) res |= 8 | ExtKeyMask;  // J + b6
            if (state[Key.Home]) res |= 4 | ExtKeyMask; // K + b6
            if (state[Key.End]) res |= 2 | ExtKeyMask;  // L + b6

            if (state[Key.NumPadEnter]) res |= 1;
            if (state[Key.NumPadMinus]) res |= 8;
            if (state[Key.NumPadPlus]) res |= 4;
            if (state[Key.RightShift] || state[Key.LeftShift])
            {
                if (state[Key.Equals]) res |= 4;
            }
            else
            {
                if (state[Key.Minus]) res |= 8;
                if (state[Key.Equals]) res |= 2;
            }
            return res;
        }

        /// <summary>
        /// #DFFE: YUIOP    +",'
        /// </summary>
        private static long scan_DFFE(IKeyboardState state)
        {
            var res = 0L;
            if (state[Key.P]) res |= 1;
            if (state[Key.O]) res |= 2;
            if (state[Key.I]) res |= 4;
            if (state[Key.U]) res |= 8;
            if (state[Key.Y]) res |= 16;

            if (state[Key.LeftBracket]) res |= 16;      // SS+Y
            if (state[Key.RightBracket]) res |= 8;      // SS+U

            if (state[Key.F9]) res |= 4 | ExtKeyMask;       // I + b6
            if (state[Key.Insert]) res |= 2 | ExtKeyMask;   // O + b6
            if (state[Key.Delete]) res |= 1 | ExtKeyMask;   // P + b6

            if (state[Key.Tab]) res |= 4;
            if (state[Key.RightShift] || state[Key.LeftShift])
            {
                if (state[Key.Apostrophe]) res |= 1;            // " = SS+P
            }
            else
            {
                if (state[Key.SemiColon]) res |= 2;             // ; = SS+O
            }
            return res;
        }

        /// <summary>
        /// #EFFE: 67890    +down,up,right, bksp
        /// </summary>
        private static long scan_EFFE(IKeyboardState state)
        {
            var res = 0L;
            if (state[Key.D0]) res |= 1;
            if (state[Key.D9]) res |= 2;
            if (state[Key.D8]) res |= 4;
            if (state[Key.D7]) res |= 8;
            if (state[Key.D6]) res |= 16;

            if (state[Key.RightArrow]) res |= 4;
            if (state[Key.UpArrow]) res |= 8;
            if (state[Key.DownArrow]) res |= 16;
            if (state[Key.BackSpace]) res |= 1;
            if (state[Key.RightShift] || state[Key.LeftShift])
            {
                if (state[Key.Minus]) res |= 1;                 // _ = SS+0
            }
            else
            {
                if (state[Key.Apostrophe]) res |= 8;            // ' = SS+7
            }
            return res;
        }

        /// <summary>
        /// #F7FE: 54321    +left
        /// </summary>
        private static long scan_F7FE(IKeyboardState state)
        {
            var res = 0L;
            if (state[Key.D1]) res |= 1;
            if (state[Key.D2]) res |= 2;
            if (state[Key.D3]) res |= 4;
            if (state[Key.D4]) res |= 8;
            if (state[Key.D5]) res |= 16;

            if (state[Key.Escape]) res |= 1;
            if (state[Key.LeftArrow]) res |= 16;
            return res;
        }

        /// <summary>
        /// #FBFE: QWERT    +period,comma
        /// </summary>
        private static long scan_FBFE(IKeyboardState state)
        {
            var res = 0L;
            if (state[Key.Q]) res |= 1;
            if (state[Key.W]) res |= 2;
            if (state[Key.E]) res |= 4;
            if (state[Key.R]) res |= 8;
            if (state[Key.T]) res |= 16;

            if (state[Key.F11]) res |= 1;           // SS+Q
            if (state[Key.F12]) res |= 2;           // SS+W

            if (state[Key.F5]) res |= 4 | ExtKeyMask;   // E + b6

            if (state[Key.RightShift] || state[Key.LeftShift])
            {
                if (state[Key.Period]) res |= 16;
                if (state[Key.Comma]) res |= 8;
            }
            return res;
        }

        /// <summary>
        /// #FDFE: GFDSA
        /// </summary>
        private static long scan_FDFE(IKeyboardState state)
        {
            var res = 0L;
            if (state[Key.A]) res |= 1;
            if (state[Key.S]) res |= 2;
            if (state[Key.D]) res |= 4;
            if (state[Key.F]) res |= 8;
            if (state[Key.G]) res |= 16;

            if (state[Key.BackSlash]) res |= 4;     // SS+D

            if (state[Key.F1]) res |= 1 | ExtKeyMask;   // A + b6
            if (state[Key.F4]) res |= 4 | ExtKeyMask;   // D + b6
            if (state[Key.F6]) res |= 8 | ExtKeyMask;   // F + b6
            if (state[Key.F7]) res |= 16 | ExtKeyMask;  // G + b6
            return res;
        }

        /// <summary>
        /// #FEFE: VCXZ[caps]     +left,right,up,down,bksp
        /// </summary>
        private static long scan_FEFE(IKeyboardState state)
        {
            var res = 0L;
            if (state[Key.LeftControl] || state[Key.RightControl]) res |= 1;
            if (state[Key.Z]) res |= 2;
            if (state[Key.X]) res |= 4;
            if (state[Key.C]) res |= 8;
            if (state[Key.V]) res |= 16;

            if (state[Key.F3]) res |= 8 | ExtKeyMask;      // C + b6

            if (state[Key.Escape]) res |= 1;
            if (state[Key.LeftArrow]) res |= 1;
            if (state[Key.RightArrow]) res |= 1;
            if (state[Key.UpArrow]) res |= 1;
            if (state[Key.DownArrow]) res |= 1;
            if (state[Key.BackSpace]) res |= 1;
            if (state[Key.CapsLock]) res |= 1;
            if (state[Key.Tab]) res |= 1;
            if (state[Key.NumPadSlash]) res |= 16;
            if (state[Key.RightShift] || state[Key.LeftShift])
            {
                if (state[Key.SemiColon]) res |= 2;
                if (state[Key.Slash]) res |= 8;
            }
            else
            {
                if (state[Key.Slash]) res |= 16;
            }
            return res;
        }

        #endregion
    }
}
