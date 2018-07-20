/* 
 *  Copyright 2015 Alex Makeev
 * 
 *  This file is part of ZXMAK2 (ZX Spectrum virtual machine).
 *
 *  ZXMAK2 is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  ZXMAK2 is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with ZXMAK2.  If not, see <http://www.gnu.org/licenses/>.
 *  
 * 
 */
using System;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Cpu;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Hardware.Sprinter
{
    public class CovoxBlaster : SoundDeviceBase
    {
        #region Constants

        private const int MODE_CBL = 0x80;      // COVOX-Blaster on (если 0 то обычный режим COVOX)
        private const int MODE_STEREO = 0x40;   // STEREO-mode on
        private const int MODE_16BIT = 0x20;    // 16bit-mode on
        private const int MODE_INTRQ = 0x10;    // Interrupt on - включение прерываний

        #endregion Constants


        #region Fields

        private readonly byte[] _ram = new byte[0x400];
        private int _cnt;
        private double _lastTime;

        private byte _port4e;
        private int _divider;
        private bool _isBlaster;
        private bool _is16bit;
        private bool _isStereo;
        private bool _isIntEnabled;
        private bool _isInt;
        private int _step = 1;
        private int _cntShift;
        private int _addrMask;

        private double _sampleRateHz;
        private double _tick;
        private int m_mult;

        private CpuUnit _cpu;

        #endregion Fields


        public CovoxBlaster()
        {
            Category = BusDeviceCategory.Sound;
            Name = "COVOX BLASTER";
            Description = "SPRINTER COVOX BLASTER";
            OnProcessConfigChange();
        }

        
        #region SoundDeviceBase

        public override void BusInit(IBusManager bmgr)
        {
            base.BusInit(bmgr);
            _cpu = bmgr.CPU;
            bmgr.Events.SubscribeWrIo(0x00FF, 0x004e, WritePort4e);
            bmgr.Events.SubscribeRdIo(0x00FF, 0x004e, ReadPort4e);
            bmgr.Events.SubscribeWrIo(0x00FF, 0x004F, WritePort4f);
            bmgr.Events.SubscribeWrIo(0x00FF, 0x00FB, WritePortFb);
            bmgr.Events.SubscribeRdIo(0x00FF, 0x00FE, ReadPortFe);
            bmgr.Events.SubscribePreCycle(CheckInt);
            bmgr.Events.SubscribeReset(ResetBus);
        }

        protected override void OnProcessConfigChange()
        {
            base.OnProcessConfigChange();

            // process volume change...
            m_mult = (ushort.MaxValue * Volume) / (100 * 0xFF);
        }

        protected override void OnBeginFrame()
        {
            base.OnBeginFrame();
            _lastTime = 0D;
        }

        protected override void OnEndFrame()
        {
            Flush(1D);
            base.OnEndFrame();
        }

        protected override void OnConfigLoad(XmlNode node)
        {
            base.OnConfigLoad(node);
            LogIo = Utils.GetXmlAttributeAsBool(node, "logIo", false);
        }

        protected override void OnConfigSave(XmlNode node)
        {
            base.OnConfigSave(node);
            Utils.SetXmlAttribute(node, "logIo", LogIo);
        }

        private void ResetBus()
        {
            var handled = false;
            WritePort4e(0x4E, 0x00, ref handled);
        }

        #endregion SoundDeviceBase


        #region I/O Handlers

        private void WritePort4e(ushort addr, byte value, ref bool handled)
        {
            //if (handled)
            //    return;
            handled = true;

            if (_port4e == value)
            {
                return;
            }
            if (LogIo)
            {
                Logger.Debug("CovoxBlaster: mode #{0:X2} => #{1:X2} (PC=#{2:X4}, BC={3:X4})", _port4e, value, _cpu.LPC, _cpu.regs.BC);
            }
            Flush(GetFrameTime());
            _port4e = value;
            _divider = GetDivider(_port4e & 0x0F);
            _isBlaster = (_port4e & MODE_CBL) != 0;
            _addrMask = (_port4e & MODE_16BIT) != 0 ? 0x1FF : 0xFF;
            if ((_port4e & MODE_INTRQ) == 0)
            {
                _isIntEnabled = false;
                _isInt = false;
            }
        }

        private void ReadPort4e(ushort addr, ref byte value, ref bool handled)
        {
            //if (handled)
            //    return;
            handled = true;
            value = _port4e;
        }

        private void WritePortFb(ushort addr, byte value, ref bool handled)
        {
            //if (handled)
            //    return;
            //handled = true;

            if (!_isBlaster)
            {
                // Covox mode
                var dac = (ushort)(value * m_mult);
                UpdateDac(dac, dac);
            }
        }

        public void WriteMemory(ushort addr, byte value)
        {
            //if ((_port4e & MODE_INTRQ) == 0)
            //{
            //    return;
            //}
            Flush(GetFrameTime());
            if (LogIo)
            {
                Logger.Debug("CovoxBlaster: WR* #{0:X2} (PC=#{1:X4}, addr={2:X4})", value, _cpu.LPC, addr);
            }
            _ram[addr & _addrMask] = value;
            _isInt = false;
        }

        private void WritePort4f(ushort addr, byte value, ref bool handled)
        {
            //if (handled)
            //    return;
            //handled = true;

            // CovoxBlaster mode
            Flush(GetFrameTime());
            if (LogIo)
            {
                Logger.Debug("CovoxBlaster: WR #{0:X2} (PC=#{1:X4}, BC={2:X4})", value, _cpu.LPC, _cpu.regs.BC);
            }
            var wraddr = addr >> 8;
            if ((_port4e & MODE_16BIT) != 0)
            {
                wraddr |= ~_cnt & 0x100;
            }
            _ram[wraddr] = value;
            _isInt = false;
        }

        private void ReadPortFe(ushort addr, ref byte value, ref bool handled)
        {
            //if (handled)
            //    return;
            //handled = true;

            Flush(GetFrameTime());
            value &= 0x3F;

            if (_isBlaster && _isInt)
            {
                value |= 0x80;
            }
            //Logger.Debug("IN #FE: {0}   pc={1:X4}", value >> 7, _cpu.LPC);
        }

        #endregion I/O Handlers


        #region Properties

        public bool LogIo { get; set; }

        #endregion Properties


        #region Private

        private void Flush(double frameTime)
        {
            if (_divider == 0)
            {
                _lastTime = frameTime;
                return;
            }
            if (!_isBlaster)
            {
                _cnt = 0;
            }
            for (var time = _lastTime; time < frameTime && time <= 1D; time += _tick)
            {
                Step();
                Fetch(time);
                _lastTime = time + _tick;
            }
        }

        private void Fetch(double time)
        {
            if (_divider == 1)
            {
                return;
            }
            if (_is16bit)
            {
                var left = (ushort)_ram[_cnt];
                left |= (ushort)(_ram[(_cnt + 1) & 0x1FF] << 8);
                var right = left;
                if (_isStereo)
                {
                    right = (ushort)_ram[(_cnt + 2) & 0x1FF];
                    right |= (ushort)(_ram[(_cnt + 3) & 0x1FF] << 8);
                }
                UpdateDac(time, (short)left, (short)right);
            }
            else
            {
                var left = (ushort)(_ram[_cnt] << 8);
                var right = left;
                if (_isStereo)
                {
                    right = (ushort)(_ram[(_cnt + 1) & 0xFF] << 8);
                }
                UpdateDac(time, left, right);
            }
        }

        private void Step()
        {
            if (_cnt == 0)
            {
                Carry();
            }
            var cnt = _cnt + _step;
            if (cnt >= (0x100<<_cntShift) || cnt < 0)
            {
                cnt = 0;
            }
            if (((cnt ^ _cnt) & (0x80<<_cntShift)) != 0)
            {
                // actually INT linked to the bit D6?
                _isInt = true;
            }
            _cnt = cnt;
            if (_cnt == 0)
            {
                Carry();
            }
        }

        private void Carry()
        {
            _cnt = 0;
            _is16bit = (_port4e & MODE_16BIT) != 0;
            _isStereo = (_port4e & MODE_STEREO) != 0;
            _isIntEnabled = (_port4e & MODE_INTRQ) != 0;
            _cntShift = _is16bit ? 1 : 0;
            _step = _isStereo ? 2 : 1;
            _step <<= _cntShift;
            if (_divider == 0)
            {
                _sampleRateHz = double.NaN;
                _tick = double.NaN;
            }
            const double frequency = 218750D;
            _sampleRateHz = frequency / (double)(_divider + 1);
            _tick = 50D / _sampleRateHz;
        }

        public void CheckInt()
        {
            // interrupt issue with 0x0F after startup (flashing cursor, etc)
            if (!_isBlaster || _divider <= 1)
            {
                return;
            }
            Flush(GetFrameTime());
            if ((_cnt & 0x40) != 0)
            {
                _isInt = false;
            }
            if (_isIntEnabled && _isInt)
            {
                _cpu.INT = true;
            }
        }

        private static int GetDivider(int code)
        {
            switch (code)
            {
                // это старые режимы -- не использовать!
                // (old modes, do not use it!)
                case 0: return 13;    // 16khz
                case 1: return 9;     // 22khz

                case 8: return 27;    // 7.8125  kHz
                case 9: return 19;    // 10.9375 kHz
                case 10: return 13;   // 15.625  kHz
                case 11: return 9;    // 21.875  kHz
                case 12: return 6;    // 31.25   kHz
                case 13: return 4;    // 43.75   kHz
                case 14: return 3;    // 54.6875 kHz
                case 15: return 1;    // 109.375 kHz
                default: return 0;    // reserved
            }
        }

        #endregion Private
    }
}
