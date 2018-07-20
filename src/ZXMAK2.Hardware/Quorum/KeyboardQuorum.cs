using System;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Engine.Cpu;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;


namespace ZXMAK2.Hardware.Quorum
{
    public class KeyboardQuorum : BusDeviceBase, IKeyboardDevice
    {
        private long m_intState;
        private UInt64 m_extState;
        private IKeyboardState m_keyboardState;
        private IBusManager m_busManager;
        private CpuUnit m_cpu;
        private bool m_nmiTriggered;


        public KeyboardQuorum()
        {
            Category = BusDeviceCategory.Keyboard;
            Name = "KEYBOARD QUORUM";
            Description = "Quorum extended keyboard\n\n(c) Eltaron";
        }


        public override void BusConnect()
        {
        }

        public override void BusDisconnect()
        {
        }

        public override void BusInit(IBusManager bmgr)
        {
            m_busManager = bmgr;
            bmgr.Events.SubscribeRdIo(0x99, 0x98, readPortFE);
            bmgr.Events.SubscribeRdIo(0x99, 0x18, readPort7E);

            m_cpu = bmgr.CPU;
        }

        private void readPortFE(ushort addr, ref byte value, ref bool handled)
        {
            value = (byte)(value & 0xe0);
            value = (byte)(value | ((byte)(this.scanFEPort(addr) & 0x1f)));
        }

        private void readPort7E(ushort addr, ref byte value, ref bool handled)
        {
            // Quorum ROM contains modificated keyboard procedures so we MUST handle 0x7E port
            // otherwise we'll get no keyboard at all, even 0xFE one

            value = (byte)(value & 0xC0);
            value = (byte)(value | ((byte)(this.scan7EPort(addr) & 0x3f)));
        }

        private int scanFEPort(ushort port)
        {
            byte num = 0x1f;
            int num2 = 0x100;
            int num3 = 0;
            while (num3 < 8)
            {
                if ((port & num2) == 0)
                {
                    num = (byte)(num & ((byte)(((this.m_intState >> (num3 * 5)) ^ 0x1f) & 0x1f)));
                }
                num3++;
                num2 = num2 << 1;
            }
            return num;
        }

        private int scan7EPort(ushort port)
        {
            byte num = 0x3f;
            int num2 = 0x100;
            int num3 = 0;
            while (num3 < 8)
            {
                if ((port & num2) == 0)
                {
                    num = (byte)(num & ((byte)(((this.m_extState >> (num3 * 6)) ^ 0x3f) & 0x3f)));
                }
                num3++;
                num2 = num2 << 1;
            }
            return num;
        }

        IKeyboardState IKeyboardDevice.KeyboardState
        {
            get
            {
                return m_keyboardState;
            }
            set
            {
                m_keyboardState = value;
                this.m_intState = scanState(this.m_keyboardState);
                this.m_extState = scanExtState(this.m_keyboardState);
            }
        }

        private static long scanState(IKeyboardState state)
        {
            if (((state == null) || state[Key.LeftAlt]) || state[Key.RightAlt])
            {
                return 0;
            }
            long num = 0;
            num = (num << 5) | scan_7FFE(state);
            num = (num << 5) | scan_BFFE(state);
            num = (num << 5) | scan_DFFE(state);
            num = (num << 5) | scan_EFFE(state);
            num = (num << 5) | scan_F7FE(state);
            num = (num << 5) | scan_FBFE(state);
            num = (num << 5) | scan_FDFE(state);
            return ((num << 5) | scan_FEFE(state));
        }

        private ulong scanExtState(IKeyboardState state)
        {
            if (((state == null) || state[Key.LeftAlt]) || state[Key.RightAlt])
            {
                return 0;
            }
            ulong num = 0;
            if (state[Key.F11])
            {
                if (!m_nmiTriggered)
                {
                    m_nmiTriggered = true;
                    m_busManager.Events.RequestNmi(50000);
                }
            }
            else
            {
                m_nmiTriggered = false;
            }
            if (state[Key.F12])
            {
                m_cpu.RESET();
            }
            num = (num << 6) | scan_7F7E(state);
            num = (num << 6) | scan_BF7E(state);
            num = (num << 6) | scan_DF7E(state);
            num = (num << 6) | scan_EF7E(state);
            num = (num << 6) | scan_F77E(state);
            num = (num << 6) | scan_FB7E(state);
            num = (num << 6) | scan_FD7E(state);
            return ((num << 6) | scan_FE7E(state));
        }


        #region FE-keyboard

        private static byte scan_7FFE(IKeyboardState state)
        {
            byte num = 0;
            if (state[Key.Space])
            {
                num = (byte)(num | 1);
            }
            if (state[Key.RightShift])
            {
                num = (byte)(num | 2);
            }
            if (state[Key.M])
            {
                num = (byte)(num | 4);
            }
            if (state[Key.N])
            {
                num = (byte)(num | 8);
            }
            if (state[Key.B])
            {
                num = (byte)(num | 0x10);
            }
            return num;
        }

        private static byte scan_BFFE(IKeyboardState state)
        {
            byte num = 0;
            if (state[Key.Return])
            {
                num = (byte)(num | 1);
            }
            if (state[Key.L])
            {
                num = (byte)(num | 2);
            }
            if (state[Key.K])
            {
                num = (byte)(num | 4);
            }
            if (state[Key.J])
            {
                num = (byte)(num | 8);
            }
            if (state[Key.H])
            {
                num = (byte)(num | 0x10);
            }
            return num;
        }

        private static byte scan_DFFE(IKeyboardState state)
        {
            byte num = 0;
            if (state[Key.P])
            {
                num = (byte)(num | 1);
            }
            if (state[Key.O])
            {
                num = (byte)(num | 2);
            }
            if (state[Key.I])
            {
                num = (byte)(num | 4);
            }
            if (state[Key.U])
            {
                num = (byte)(num | 8);
            }
            if (state[Key.Y])
            {
                num = (byte)(num | 0x10);
            }
            return num;
        }

        private static byte scan_EFFE(IKeyboardState state)
        {
            byte num = 0;
            if (state[Key.D0])
            {
                num = (byte)(num | 1);
            }
            if (state[Key.D9])
            {
                num = (byte)(num | 2);
            }
            if (state[Key.D8] || state[Key.RightArrow])
            {
                num = (byte)(num | 4);
            }
            if (state[Key.D7] || state[Key.UpArrow])
            {
                num = (byte)(num | 8);
            }
            if (state[Key.D6] || state[Key.DownArrow])
            {
                num = (byte)(num | 0x10);
            }
            return num;
        }

        private static byte scan_F7FE(IKeyboardState state)
        {
            byte num = 0;
            if (state[Key.D1])
            {
                num = (byte)(num | 1);
            }
            if (state[Key.D2])
            {
                num = (byte)(num | 2);
            }
            if (state[Key.D3])
            {
                num = (byte)(num | 4);
            }
            if (state[Key.D4])
            {
                num = (byte)(num | 8);
            }
            if (state[Key.D5] || state[Key.LeftArrow])
            {
                num = (byte)(num | 0x10);
            }
            return num;
        }

        private static byte scan_FBFE(IKeyboardState state)
        {
            byte num = 0;
            if (state[Key.Q])
            {
                num = (byte)(num | 1);
            }
            if (state[Key.W])
            {
                num = (byte)(num | 2);
            }
            if (state[Key.E])
            {
                num = (byte)(num | 4);
            }
            if (state[Key.R])
            {
                num = (byte)(num | 8);
            }
            if (state[Key.T])
            {
                num = (byte)(num | 0x10);
            }
            return num;
        }

        private static byte scan_FDFE(IKeyboardState state)
        {
            byte num = 0;
            if (state[Key.A])
            {
                num = (byte)(num | 1);
            }
            if (state[Key.S])
            {
                num = (byte)(num | 2);
            }
            if (state[Key.D])
            {
                num = (byte)(num | 4);
            }
            if (state[Key.F])
            {
                num = (byte)(num | 8);
            }
            if (state[Key.G])
            {
                num = (byte)(num | 0x10);
            }
            return num;
        }

        private static byte scan_FEFE(IKeyboardState state)
        {
            byte num = 0;
            if (state[Key.LeftShift])
            {
                num = (byte)(num | 1);
            }
            if (state[Key.Z])
            {
                num = (byte)(num | 2);
            }
            if (state[Key.X])
            {
                num = (byte)(num | 4);
            }
            if (state[Key.C])
            {
                num = (byte)(num | 8);
            }
            if (state[Key.V])
            {
                num = (byte)(num | 0x10);
            }
            return num;
        }
        #endregion

        #region 7E-keyboard (Quorum)

        private static byte scan_FE7E(IKeyboardState state)
        {
            byte num = 0;
            if (state[Key.PageUp])
            {
                num = (byte)(num | 0x1);
            }
            if (state[Key.PageDown])
            {
                num = (byte)(num | 0x2);
            }
            if (state[Key.NumPad1])
            {
                num = (byte)(num | 0x8);
            }
            if (state[Key.NumPad2])
            {
                num = (byte)(num | 0x10);
            }
            if (state[Key.Period])
            {
                num = (byte)(num | 0x20);
            }
            return num;
        }

        private static byte scan_FD7E(IKeyboardState state)
        {
            byte num = 0;
            if (state[Key.CapsLock])
            {
                num = (byte)(num | 1);
            }
            if (state[Key.F2])
            {
                num = (byte)(num | 2);
            }
            if (state[Key.Grave]) // ^ arrow
            {
                num = (byte)(num | 4);
            }
            if (state[Key.NumPad4])
            {
                num = (byte)(num | 8);
            }
            if (state[Key.Apostrophe])
            {
                num = (byte)(num | 0x10);
            }
            if (state[Key.NumPad6])
            {
                num = (byte)(num | 0x20);
            }
            return num;
        }
        private static byte scan_FB7E(IKeyboardState state)
        {
            byte num = 0;
            if (state[Key.Tab])
            {
                num = (byte)(num | 1);
            }
            if (state[Key.F4])
            {
                num = (byte)(num | 2);
            }
            if (state[Key.NumPad7])
            {
                num = (byte)(num | 8);
            }
            if (state[Key.NumPad5])
            {
                num = (byte)(num | 0x10);
            }
            if (state[Key.NumPad9])
            {
                num = (byte)(num | 0x20);
            }
            return num;
        }
        private static byte scan_F77E(IKeyboardState state)
        {
            byte num = 0;
            if (state[Key.Escape]) // E-mode
            {
                num = (byte)(num | 1);
            }
            if (state[Key.F5])
            {
                num = (byte)(num | 2);
            }
            if (state[Key.Delete])
            {
                num = (byte)(num | 4);
            }
            if (state[Key.NumPadSlash])
            {
                num = (byte)(num | 8);
            }
            if (state[Key.NumPad8])
            {
                num = (byte)(num | 0x10);
            }
            if (state[Key.NumPadMinus])
            {
                num = (byte)(num | 0x20);
            }
            return num;
        }

        private static byte scan_EF7E(IKeyboardState state)
        {
            byte num = 0;
            if (state[Key.Minus])
            {
                num = (byte)(num | 1);
            }
            if (state[Key.Equals])
            {
                num = (byte)(num | 4);
            }
            if (state[Key.BackSpace])
            {
                num = (byte)(num | 8);
            }
            if (state[Key.NumPadStar])
            {
                num = (byte)(num | 0x10);
            }
            if (state[Key.F6])
            {
                num = (byte)(num | 0x20);
            }
            return num;
        }

        private static byte scan_DF7E(IKeyboardState state)
        {
            byte num = 0;
            if (state[Key.SemiColon])
            {
                num = (byte)(num | 1);
            }
            if (state[Key.F3])
            {
                num = (byte)(num | 2);
            }
            if (state[Key.BackSlash])
            {
                num = (byte)(num | 4);
            }
            if (state[Key.RightBracket])
            {
                num = (byte)(num | 0x10);
            }
            if (state[Key.LeftBracket])
            {
                num = (byte)(num | 0x20);
            }
            return num;
        }

        private static byte scan_BF7E(IKeyboardState state)
        {
            byte num = 0;
            if (state[Key.Comma])
            {
                num = (byte)(num | 1);
            }
            if (state[Key.Slash])
            {
                num = (byte)(num | 0x10);
            }
            if (state[Key.NumPad3])
            {
                num = (byte)(num | 0x20);
            }
            return num;
        }

        private static byte scan_7F7E(IKeyboardState state)
        {
            byte num = 0;
            if (state[Key.F1])
            {
                num = (byte)(num | 2);
            }
            if (state[Key.NumPad0])
            {
                num = (byte)(num | 8);
            }
            if (state[Key.NumPadPeriod])
            {
                num = (byte)(num | 0x10);
            }
            if (state[Key.NumPadPlus])
            {
                num = (byte)(num | 0x20);
            }
            return num;
        }

        #endregion
    }
}
