using System;
using System.Collections.Generic;

using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Hardware.Sprinter
{
    public sealed class SprinterMouse : BusDeviceBase, IMouseDevice
    {
        #region Fields

        private readonly Queue<byte> _queue = new Queue<byte>();

        private IMouseState _mouseState;
        private int _mouseX;
        private int _mouseY;
        private int _mouseBtn;
        private bool _isSwapBtns;

        #endregion Fields

        
        public SprinterMouse()
        {
            Category = BusDeviceCategory.Mouse;
            Name = "MOUSE SPRINTER";
            Description = "Standart Sprinter Mouse";
        }


        public override void BusConnect()
        {
            _isSwapBtns = true;
        }

        public override void BusDisconnect()
        {
        }

        public override void BusInit(IBusManager bmgr)
        {
            //bmgr.SubscribeRDIO(0xffff, 0xfadf, new BusReadIoProc(this.readPortFADF));
            //bmgr.SubscribeRDIO(0xffff, 0xfbdf, new BusReadIoProc(this.readPortFBDF));
            //bmgr.SubscribeRDIO(0xffff, 0xffdf, new BusReadIoProc(this.readPortFFDF));
            bmgr.Events.SubscribeRdIo(0x00ff, 0x001B, new BusReadIoProc(ReadPortMouseState));
            bmgr.Events.SubscribeRdIo(0x00ff, 0x001A, new BusReadIoProc(ReadPortMouseData));
        }

        private void ReadPortMouseState(ushort addr, ref byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;
            value = (byte)((_queue.Count > 0) ? 1 : 0);
        }

        private void ReadPortMouseData(ushort addr, ref byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;

            //value = (byte)((m_kbd_buff.Length > 0) ? 1 : 0);
            if (_queue.Count > 0)
            {
                value = _queue.Dequeue();
            }
            else
            {
                value = 0;
            }
        }

        public IMouseState MouseState
        {
            get { return _mouseState; }
            set
            {
                if (_queue.Count > 3 * 4)   // buffer overflow
                {
                    return;
                }
//                m_mouseState = value;
                /*bool different = false;
                if (value != null)
                {
                    Logger.GetLogger().LogTrace(String.Format("Mouse X = {0}, Y = {1}, Btns = {2}", value.X, value.Y, value.Buttons));
                    if (m_mouseX != value.X)
                    {
//                        m_mouseX = value.X;
                        different = true;
                    }
                    if (m_mouseY!= value.Y)
                    {
                        
//                        m_mouseY = value.Y;
                        different = true;
                    }
                    if (m_mouseBtn != value.Buttons)
                    {
                        m_mouseBtn = value.Buttons;
                        different = true;
                    }
                }*/
/*                if (this.m_mouseState == null)
                {
                    this.m_mouseState = value;
                    different = true;
                }
                else
                {
                    if (this.)
                    if (!((this.m_mouseState.Buttons == value.Buttons) && (this.m_mouseState.Y == value.Y) && (this.m_mouseState.X == value.X)))
                    {
                        this.m_mouseState = value;
                        different = true;
                    }
                }*/
                if (!((_mouseBtn== value.Buttons) && (_mouseY == value.Y) && (_mouseX == value.X)))
                //if (different)
                {
                    byte my;
                    my = (byte)(Math.Abs(_mouseY - value.Y) / 2);
                    if ((_mouseY - value.Y) > 0)
                    {
                        my ^= 0x7f;
                        my |= 128;
                    }
                    byte mx;
                    mx = (byte)(Math.Abs(_mouseX - value.X) / 2);
                    if ((_mouseX - value.X) > 0)
                    {
                        mx ^= 0x7f;
                        mx |= 128;
                    }
                    _mouseX = value.X;
                    _mouseY = value.Y;
                    if (_isSwapBtns)
                    {
                        _mouseBtn = ((value.Buttons & 0x01) << 1) | ((value.Buttons & 0x02) >> 1);
                    } else _mouseBtn = value.Buttons;

                    _mouseState = value;
                    byte b1 = (byte)(64 + ((_mouseBtn & 3) << 4) + (((my) & 192) >> 4) + (((mx) & 192) >> 6));
                    _queue.Enqueue(b1);
                    b1 = (byte)((mx) & 63);
                    _queue.Enqueue(b1);
                    b1 = (byte)((my) & 63);
                    _queue.Enqueue(b1);
                    //Logger.GetLogger().LogTrace(String.Format("Mouse event start, Buffer size = {0}", m_msbuf.Count));
                }
            }
        }
    }
}
