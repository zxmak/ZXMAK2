//#define Debug
using System;

using ZXMAK2.Engine.Attributes;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Hardware.Sprinter
{
/*
        WingLion:
        А с распределением памяти ситуация такая. Порты страниц - переключают адреса 
        "виртуальных" страниц скорпиона независимо от того, что в этот момент 
        подключено в нулевую банку. С третьей банкой наоборот - запись в порт 
        страницы меняет адрес той страницы, какая установлена портами 7FFD,1FFD, 
        т.е. записав что-то в порт PAGE3 нужно помнить, какая страница стояла а 
        адреса #C000. Например, если нулевая, то поменяется страница и в адресе 
        #0000, если там было установлено ОЗУ.

        В неспектрумовских режимах Спринтера, спектрумовские порты отключаются, и 
        остается только спринтеровское управление страницами.
*/
    public sealed class SprinterMMU : MemoryBase
    {
        #region Fields

        private readonly byte[][] _cramPages = new byte[0x04][]; //кэш
        private readonly byte[][] _vramPages = new byte[0x10][]; //видео-рам
        private readonly byte[] _scorpionPages = new byte[8];
        private readonly byte[] _pages = new byte[16];

        private byte _page0;
        private byte _page1;
        private byte _page2;
        private byte _page3;
        private bool _cache;
        private byte _sysPort;
        //Video RAM
        private byte _portY;
        private byte _portVideoMode;
        private byte _portScr;
        private byte _vblock4000;
        private byte _vblockC000;
        private bool _sys; //ROM Expansion
        private bool _romA16; //ARAM16 (RA16)
        private byte _romIndex; //Номер страницы ПЗУ
        //private byte m_cacheindex; //Номер страницы Кэша
        private bool _firstRead = true;

        //Акселератор
        private bool _accEnable;
        private bool _accOn;

        //private bool _accWaitCmd;
        //private bool _accWaitData;

        private AccelCMD _accMode;
        private AccelSubCMD _accSubMode;

        private int _accBufSize;
        private byte[] _accBuf = new byte[256];
#if Debug
        private ushort _opaddr;
#endif
        private SprinterULA _ulaSprinter;
        private SprinterFdd _sprinterBdi;
        private CovoxBlaster _covoxBlaster;

        #endregion Fields


        public SprinterMMU()
            : base("Sprinter", 0x10, 0x0100)
        {
            Name = "Sprinter RAM";
            Description = "Sprinter 4Mb RAM + 64Kb Cache + 256Kb VRAM Manager";
        }


        #region MemoryBase

        public override bool IsMap48
        {
            get { return false; }
        }

        protected override void Init(int romPageCount, int ramPageCount)
        {
            base.Init(romPageCount, ramPageCount);
            for (int i = 0; i < this._vramPages.Length; i++)
            {
                this._vramPages[i] = new byte[0x4000];
            }
            for (int i = 0; i < this._cramPages.Length; i++)
            {
                this._cramPages[i] = new byte[0x4000];
            }
        }

        public override int GetRomIndex(RomId romId)
        {
            switch (romId)
            {
                // It seems like not used
                case RomId.ROM_128: return 0;
                case RomId.ROM_SOS: return 1;
                case RomId.ROM_DOS: return 2;
                case RomId.ROM_SYS: return 3;
            }
            Logger.Error("Unknown RomName: {0}", romId);
            throw new InvalidOperationException("Unknown RomName");
        }

        #endregion MemoryBase


        #region  BusDeviceBase

        public override void BusInit(IBusManager bmgr)
        {
            base.BusInit(bmgr); // should be first because Accelerator depends on it

            _ulaSprinter = bmgr.FindDevice<SprinterULA>();
            _sprinterBdi = bmgr.FindDevice<SprinterFdd>();
            _covoxBlaster = bmgr.FindDevice<CovoxBlaster>();
            if (_ulaSprinter != null)
            {
                _ulaSprinter.VRAM = _vramPages;
            }

            bmgr.Events.SubscribeWrIo(0x0000, 0x0000, this.WriteIoDcp);  //write to DCP Port
            bmgr.Events.SubscribeRdIo(0x0000, 0x0000, this.ReadIoDcp);    //read from DCP port
            bmgr.Events.SubscribeWrIo(0x00ff, 0x0082, this.WritePort82);  //write PAGE0
            bmgr.Events.SubscribeRdIo(0x00ff, 0x0082, this.ReadPort82);    //read PAGE0
            bmgr.Events.SubscribeWrIo(0x00ff, 0x00A2, this.WritePortA2);  //write PAGE1
            bmgr.Events.SubscribeRdIo(0x00ff, 0x00A2, this.ReadPortA2);    //read PAGE1
            bmgr.Events.SubscribeWrIo(0x00ff, 0x00C2, this.WritePortC2);  //write PAGE2
            bmgr.Events.SubscribeRdIo(0x00ff, 0x00C2, this.ReadPortC2);    //read PAGE2
            bmgr.Events.SubscribeWrIo(0x00ff, 0x00E2, this.WritePortE2);  //write PAGE3
            bmgr.Events.SubscribeRdIo(0x00ff, 0x00E2, this.ReadPortE2);    //read PAGE3
            bmgr.Events.SubscribeWrIo(0xd027, 0x5025, this.WritePort7FFD);    //write 7FFDh
            bmgr.Events.SubscribeWrIo(0xd027, 0x1025, this.WritePort1FFD);  //write 1FFDh
            bmgr.Events.SubscribeRdIo(0x00ff, 0x00fb, this.ReadPortFB);  //read FBh  Open Cash
            bmgr.Events.SubscribeRdIo(0x00ff, 0x007b, this.ReadPort7B);  //read 7Bh  Close Cash
            bmgr.Events.SubscribeWrIo(0x00BD, 0x003c, this.WritePort7C);  //write 7Ch
            bmgr.Events.SubscribeWrIo(0x00FF, 0x0089, this.WritePort89);  //write PORTY
            bmgr.Events.SubscribeWrIo(0x00FF, 0x00C9, this.WritePortC9);  //write C9h
            bmgr.Events.SubscribeWrIo(0x00FF, 0x00E9, this.WritePortE9);  //write E9h
            bmgr.Events.SubscribeRdIo(0x00ff, 0x0089, this.ReadPort89);  //read PORTY
            bmgr.Events.SubscribeRdIo(0x00ff, 0x00E9, this.ReadPortE9);  //read E9h
            bmgr.Events.SubscribeRdIo(0x00ff, 0x00C9, this.ReadPortC9);  //read C9h
            
            // fix call overhead (methods marked with new)
            bmgr.Events.SubscribeWrMem(0xC000, 0x0000, this.WriteMem0000);  //write 
            bmgr.Events.SubscribeWrMem(0xC000, 0x4000, this.WriteMem4000);  //write 
            bmgr.Events.SubscribeWrMem(0xC000, 0x8000, this.WriteMem8000);  //write 
            bmgr.Events.SubscribeWrMem(0xC000, 0xC000, this.WriteMemC000);  //write
            bmgr.Events.SubscribeRdMem(0xC000, 0x0000, this.ReadMem0000);  //read
            bmgr.Events.SubscribeRdMem(0xC000, 0x8000, this.ReadMem8000);  //read
            bmgr.Events.SubscribeRdMem(0xC000, 0xC000, this.ReadMemC000);  //read
            bmgr.Events.SubscribeRdMem(0xC000, 0x4000, this.ReadMem4000);  //read

            bmgr.Events.SubscribeRdIo(0xFFFF, 0, this.ReadPort00);  //read 0
            bmgr.Events.SubscribeRdMemM1(0x0000, 0x0000, this.Accelerator);
            bmgr.Events.SubscribeRdMem(0x0000, 0x0000, this.AccelRead);
            bmgr.Events.SubscribeWrMem(0x0000, 0x0000, this.AccelWrite);

#if Debug
            bmgr.Events.SubscribeRdMemM1(0x0000, 0x0000, this.readRamM1);  //read operator from memory
#endif
            bmgr.Events.SubscribeReset(ResetBus);
        }

        protected override void UpdateMapping()
        {
            if (((this.CMR1 & 0x01) == 1) && (this._sys))
            //if ((this.CMR1 & 0x01) == 1)
            {
                //Вкл ОЗУ в 0й сектор адресного пространства (вместо ПЗУ)
                base.MapRead0000 = this.RamPages[this.PAGE0];
                base.MapWrite0000 = this.MapRead0000;
            }
            else
            {

                if (_cache && this._sys)
                {
                    base.MapRead0000 = this.CachePages[0];
                    base.MapWrite0000 = base.MapRead0000;
                }
                else
                {
                    //Добавить выдачу страниц ПЗУ
                    //byte rom_exp = (byte)((this.m_rom_exp == true) ? 8 : 0);
                    //int index = (this.CMR0 & 0x10) >> 4;
                    //index = index | ((this.DOSEN) ? 0:1); // 1 bit (RA14)
                    //index = index | ( ((this.DOSEN) ? 1:0) & ((this.CMR1 & 1) & )

                    //int videoPage = ((this.CMR0 & 8) == 0) ? 5 : 7;
                    //if (((this.CMR1 & 0x01) == 0) && (!this.m_sys))
                    _romIndex = 0;
                    //if (((this.CMR1 & 0x01) == 1) || (!this.m_sys))
                    //      m_romindex = 8;

                    _romIndex += (byte)((this._romA16) ? 0 : 8);

                    //m_romindex = (byte)((this.m_sys == false) ? 8 : 0);
                    /*
                    if (((this.CMR1 & 1) == 0) || (!this.m_sys)) 
                    {
                        if (this.m_romA16) m_romindex += 4;
                    }
                    */
                    //if (this.m_romA16 && !(((this.CMR1 & 1) == 1) && this.m_sys)) m_romindex += 4;

                    base.MapRead0000 = this.RomPages[_romIndex];
                    base.MapWrite0000 = this.m_trashPage;
                }
            }

            base.MapRead4000 = this.RamPages[this.PAGE1];
            base.MapRead8000 = this.RamPages[this.PAGE2];
            base.MapReadC000 = this.RamPages[this.PAGE3];
            if ((PAGE1 >= 0x50) && (PAGE1 <= 0x5F))
            {
                base.MapWrite4000 = m_trashPage;
            }
            else base.MapWrite4000 = base.MapRead4000;
            if ((PAGE2 >= 0x50) && (PAGE2 <= 0x5F))
            {
                base.MapWrite8000 = m_trashPage;
            }
            else base.MapWrite8000 = base.MapRead8000;

            if ((PAGE3 >= 0x50) && (PAGE3 <= 0x5F))
            {
                base.MapWriteC000 = m_trashPage;
            }
            else base.MapWriteC000 = base.MapReadC000;
            base.Map48[0] = 0;
            base.Map48[1] = this.PAGE1;
            base.Map48[2] = this.PAGE2;
            base.Map48[3] = this.PAGE3;
        }

        private void ResetBus()
        {
            this.CMR0 = 0;
            this.CMR1 = 0;
            this.PAGE1 = 5;
            this.PAGE0 = 0;
            this.PAGE2 = 2;
            this.PAGE3 = 0x40;
            this._cache = false;
            this._romIndex = 0;
            this._sys = false;
            this._romA16 = false;
            _firstRead = true;
            //надо false и сделать обработку порта 204Eh
            _accEnable = true;
            _accOn = false;
            _accMode = AccelCMD.Off;
            UpdateMapping();
        }

        #endregion  BusDeviceBase


        #region Accelerator

        private void AccelRead(ushort addr, ref byte value)
        {
            if (_accEnable && _accOn && (_accMode != AccelCMD.Off))
            {
                switch (_accMode)
                {
                    case AccelCMD.On:           //LD D,D
                        {
                            if (value != 0) _accBufSize = value;
                            else _accBufSize = 256;
                        } break;

                    case AccelCMD.CopyBlok:     //LD L,L
                        {
                            for (int i = 0; i < _accBufSize; i++)
                            {
                                byte tmp = 0;
                                switch ((addr + i) & 0xc000)
                                {
                                    case 0x0000: this.ReadMem0000((ushort)(addr + i), ref tmp); break;
                                    case 0x4000: this.ReadMem4000((ushort)(addr + i), ref tmp); break;
                                    case 0x8000: this.ReadMem8000((ushort)(addr + i), ref tmp); break;
                                    case 0xc000: this.ReadMemC000((ushort)(addr + i), ref tmp); break;
                                }
                                switch (_accSubMode)
                                {

                                    case AccelSubCMD.None:      //CopyBlok
                                        {
                                            _accBuf[i] = tmp;//RDMEM_DBG((ushort)(addr + i));
                                        } break;
                                    case AccelSubCMD.XORBlok:      //XOR (HL)
                                        {
                                            _accBuf[i] ^= tmp;

                                        } break;
                                    case AccelSubCMD.ORBlok:       //OR (HL)
                                        {
                                            _accBuf[i] |= tmp;

                                        } break;
                                    case AccelSubCMD.ANDBlok:      //AND (HL)
                                        {
                                            _accBuf[i] &= tmp;
                                        } break;
                                }

                            }
                            _accSubMode = AccelSubCMD.None;
                        } break;

                    case AccelCMD.GrCopyBlok:   //LD A,A
                        {
                            for (int i = 0; i < _accBufSize; i++)
                            {
                                byte tmp = 0;
                                switch (addr & 0xc000)
                                {
                                    case 0x0000: this.ReadMem0000(addr, ref tmp); break;
                                    case 0x4000: this.ReadMem4000(addr, ref tmp); break;
                                    case 0x8000: this.ReadMem8000(addr, ref tmp); break;
                                    case 0xc000: this.ReadMemC000(addr, ref tmp); break;
                                }
                                switch (_accSubMode)
                                {
                                    case AccelSubCMD.None: _accBuf[i] = tmp;//RDMEM_DBG((ushort)(addr + i));
                                        break;
                                    case AccelSubCMD.XORBlok: _accBuf[i] ^= tmp; break;
                                    case AccelSubCMD.ORBlok: _accBuf[i] |= tmp; break;
                                    case AccelSubCMD.ANDBlok: _accBuf[i] &= tmp; break;
                                }
                                //m_acc_buf[i] = VRamPages[(m_port_y & 0xf0) >> 4][(m_port_y & 0x0f) * 1024 + (addr & 0x3FF)]; //this.RDMEM_DBG(addr);
                                _portY++;
                            }
                            _accSubMode = AccelSubCMD.None;
                        } break;
                    case AccelCMD.GrFill:   //LD E,E
                        {
                            for (int i = 0; i < _accBufSize; i++)
                                _portY++;
                        } break;

                }
            }
        }

        private void AccelWrite(ushort addr, byte value)
        {
            if (_accEnable && _accOn && (_accMode != AccelCMD.Off))
            {
                switch (_accMode)
                {
                    case AccelCMD.On:           //LD D,D
                        {
                            if (value != 0) _accBufSize = value;
                            else _accBufSize = 256;
                        } break;
                    case AccelCMD.GrCopyBlok:   //LD A,A
                        {
                            for (int i = 0; i < _accBufSize; i++)
                            {
                                switch (addr & 0xc000)
                                {
                                    case 0x0000: this.WriteMem0000(addr, _accBuf[i]); break;
                                    case 0x4000: this.WriteMem4000(addr, _accBuf[i]); break;
                                    case 0x8000: this.WriteMem8000(addr, _accBuf[i]); break;
                                    case 0xc000: this.WriteMemC000(addr, _accBuf[i]); break;
                                }
                                //                                VRamPages[(m_port_y & 0xf0) >> 4][(m_port_y & 0x0f) * 1024 + (addr & 0x3FF)] = m_acc_buf[i];
                                _portY++;
                            }
                        } break;
                    case AccelCMD.CopyBlok: //LD L,L
                        {
                            for (int i = 0; i < _accBufSize; i++)
                            {
                                switch ((addr + i) & 0xc000)
                                {
                                    case 0x0000: this.WriteMem0000((ushort)(addr + i), _accBuf[i]); break;
                                    case 0x4000: this.WriteMem4000((ushort)(addr + i), _accBuf[i]); break;
                                    case 0x8000: this.WriteMem8000((ushort)(addr + i), _accBuf[i]); break;
                                    case 0xc000: this.WriteMemC000((ushort)(addr + i), _accBuf[i]); break;

                                }
                                //WRMEM_DBG((ushort)(addr + i), m_acc_buf[i]);

                                //                                m_acc_buf[i] = RDMEM_DBG((ushort)(addr + i));
                            }
                        } break;

                    case AccelCMD.Fill:     //LD C,C
                        {
                            for (int i = 0; i < _accBufSize; i++)
                            {
                                switch ((addr + i) & 0xc000)
                                {
                                    case 0x0000: this.WriteMem0000((ushort)(addr + i), value); break;
                                    case 0x4000: this.WriteMem4000((ushort)(addr + i), value); break;
                                    case 0x8000: this.WriteMem8000((ushort)(addr + i), value); break;
                                    case 0xc000: this.WriteMemC000((ushort)(addr + i), value); break;
                                }
                                //this.WRMEM_DBG((ushort)(addr + i), value);
                            }
                        } break;
                    case AccelCMD.GrFill:   //LD E,E
                        {
                            for (int i = 0; i < _accBufSize; i++)
                            {
                                switch (addr & 0xc000)
                                {
                                    case 0x0000: this.WriteMem0000(addr, value); break;
                                    case 0x4000: this.WriteMem4000(addr, value); break;
                                    case 0x8000: this.WriteMem8000(addr, value); break;
                                    case 0xc000: this.WriteMemC000(addr, value); break;
                                }
                                //VRamPages[(m_port_y & 0xf0) >> 4][(m_port_y & 0x0f) * 1024 + (addr & 0x03FF)] = value;
                                _portY++;
                            }
                        } break;
                }
            }
        }

        /*        
        private void GetAccelDATA(ushort addr, ref byte value)
        {
            if (_accEnable && _accOn && _accWaitData)
            {
            }
        }
        */

        private void Accelerator(ushort addr, ref byte value)
        {
            if (!_accEnable || value < 0x40)
            {
                return;
            }
            switch (value)//this.RDMEM_DBG(addr))
            {
                //Accelerator off - ld b,b
                case 0x40:
                    {
                        //_accOn = false;
                        //_accWaitCmd = false;
                        _accMode = AccelCMD.Off;
                        _accSubMode = AccelSubCMD.None;
                    }
                    break;
                //Accelerator on - ld d,d
                case 0x52:
                    {
                        _accOn = true;
                        //_accWaitCmd = true;
                        _accMode = AccelCMD.On;
                        _accSubMode = AccelSubCMD.None;
                    }
                    break;
                case 0x49:
                    {
                        _accOn = true;
                        _accMode = AccelCMD.Fill;
                        _accSubMode = AccelSubCMD.None;
                    }
                    break;
                case 0x5B:
                    {
                        _accOn = true;
                        _accMode = AccelCMD.GrFill;
                        _accSubMode = AccelSubCMD.None;
                    }
                    break;
                case 0x64:
                    {
                        _accOn = true;
                        _accMode = AccelCMD.Reserved;
                        _accSubMode = AccelSubCMD.None;
                    }
                    break;
                case 0x6D:
                    {
                        _accOn = true;
                        _accMode = AccelCMD.CopyBlok;
                        _accSubMode = AccelSubCMD.None;
                    }
                    break;
                case 0x7F:
                    {
                        _accOn = true;
                        _accMode = AccelCMD.GrCopyBlok;
                        _accSubMode = AccelSubCMD.None;
                    }
                    break;
                case 0xAE:
                    {
                        _accOn = true;
                        _accSubMode = AccelSubCMD.XORBlok;
                    } break;
                case 0xB6:
                    {
                        _accOn = true;
                        _accSubMode = AccelSubCMD.ORBlok;
                    } break;
                case 0xA6:
                    {
                        _accOn = true;
                        _accSubMode = AccelSubCMD.ANDBlok;
                    } break;
            }
        }
        
        #endregion


        #region I/O Handlers

        //Обработчик записи в порт с расшифровкой по DCP-странице
        private void WriteIoDcp(ushort addr, byte value, ref bool handled)
        {
            if (handled)
                return;

            ushort dcpadr = (ushort)((this.DOSEN ? 2048 : 0) + (addr & 0x67) + (addr & 0x2000 >> 8) + (addr & 0xc000 >> 7));
#if Debug
                LogPort(addr, value);
#endif
            //            switch (m_ramPages[0x40][dcpadr])
            if ((RamPages[0x40][dcpadr] >= 0xF0) && (RamPages[0x40][dcpadr] <= 0xFF))
            {
                _scorpionPages[(RamPages[0x40][dcpadr] & 0x0f)] = value;
                handled = true;
            }
            else
            {
                if ((RamPages[0x40][dcpadr] >= 0xE0) && (RamPages[0x40][dcpadr] <= 0xEF))
                {
                    _pages[RamPages[0x40][dcpadr] & 0x0f] = value;
                    handled = true;
                }

                /*                    switch (m_ramPages[0x40][dcpadr])
                                    {
                                        case DCPports.dcpROMSysAlt: 
                                    }*/
            }
#if Debug
                if (handled)
                    LogPort(addr, value);
#endif
        }

        //Обработчик чтения из порта с расшифровкой по DCP-странице
        private void ReadIoDcp(ushort addr, ref byte value, ref bool handled)
        {
            if (handled)
                return;

            ushort dcpadr = (ushort)((this.DOSEN ? 2048 : 0) + (addr & 0x67) + (addr & 0x2000 >> 8) + (addr & 0xc000 >> 7) + 1024);
#if Debug
                LogPort(addr, value);
#endif
            //            switch (m_ramPages[0x40][dcpadr])
            if ((RamPages[0x40][dcpadr] >= 0xF0) && (RamPages[0x40][dcpadr] <= 0xFF))
            {
                value = _scorpionPages[(RamPages[0x40][dcpadr] & 0x0f)];
                handled = true;
            }
            else
            {
                if ((RamPages[0x40][dcpadr] >= 0xE0) && (RamPages[0x40][dcpadr] <= 0xEF))
                {
                    value = _pages[RamPages[0x40][dcpadr] & 0x0f];
                    handled = true;
                }

                /*                    switch (m_ramPages[0x40][dcpadr])
                                    {
                                        case DCPports.dcpROMSysAlt: 
                                    }*/
            }
#if Debug
                if (handled)
                    LogPort(addr, value);
#endif
        }

        // TODO: need to fix (new)

        private new void WriteMem0000(ushort addr, byte value)
        {
            if (_accMode != AccelCMD.Off && _covoxBlaster != null && _page0 == 0xFD)
            {
                // accel only
                _covoxBlaster.WriteMemory(addr, value);
            }
            if ((_page0 >= 0x50) && (_page0 <= 0x5f))
            {
                //открыта видеостраница, пишем в нее
                ushort vaddr = (ushort)(addr & 0x03ff);
                //00111100 00000000
                byte line = (byte)(_portY + ((addr & 0x3c00) >> 10));
                byte vpage = (byte)((line & 0xF0) >> 4);

                // Номер страницы ВидеоОЗУ:
                // 0x50 - нормальная запись
                // 3 бит номера - разрешение записи байта #FF в видео ОЗУ
                // 2 бит номера - разрешение записи в основное ОЗУ
                if (((_page0 & 8) == 0) || (((_page0 & 8) != 0) && (value != 0xFF)))
                    _vramPages[vpage][((line & 0x0f) * 1024) + vaddr] = value;
                if ((_page0 & 4) == 0)
                {
                    RamPages[0x50 + vpage][((line & 0x0f) * 1024) + vaddr] = value;
                    //   this.MapWrite8000[addr & 0x3fff] = value;
                }
            }
            else
            {
                this.MapWrite0000[addr & 0x3fff] = value;
            }
        }

        private new void ReadMem0000(ushort addr, ref byte value)
        {
            if ((_page0 >= 0x50) && (_page0 <= 0x5f))
            {
                //открыта видеостраница, пишем в нее
                ushort vaddr = (ushort)(addr & 0x03ff);
                //00111100 00000000
                byte line = (byte)(_portY + ((addr & 0x3c00) >> 10));
                byte vpage = (byte)((line & 0xF0) >> 4);

                // Номер страницы ВидеоОЗУ:
                // 0x50 - нормальная запись
                // 3 бит номера - разрешение записи байта #FF в видео ОЗУ
                // 2 бит номера - разрешение записи в основное ОЗУ
                value = RamPages[0x50 + vpage][((line & 0x0f) * 1024) + vaddr];
                //m_vramPages[vpage][((line & 0x0f) * 1024) + vaddr];
            }
            else
            {
                base.ReadMem0000(addr, ref value);
                //value = this.MapReadC000[addr & 0x3fff];
            }
        }

        private new void WriteMem4000(ushort addr, byte value)
        {
            if (_accMode != AccelCMD.Off && _covoxBlaster != null && _page1 == 0xFD)
            {
                // accel only
                _covoxBlaster.WriteMemory(addr, value);
            }
            if ((_page1 >= 0x50) && (_page1 <= 0x5f))
            {
                //открыта видеостраница, пишем в нее
                ushort vaddr = (ushort)(addr & 0x03ff);
                //00111100 00000000
                byte line = (byte)(_portY + ((addr & 0x3c00) >> 10));
                byte vpage = (byte)((line & 0xF0) >> 4);

                // Номер страницы ВидеоОЗУ:
                // 0x50 - нормальная запись
                // 3 бит номера - разрешение записи байта #FF в видео ОЗУ
                // 2 бит номера - разрешение записи в основное ОЗУ
                if (((_page1 & 8) == 0) || (((_page1 & 8) != 0) && (value != 0xFF)))
                    _vramPages[vpage][((line & 0x0f) * 1024) + vaddr] = value;
                if ((_page1 & 4) == 0)
                {
                    //   this.MapWrite4000[addr & 0x3fff] = value;
                    RamPages[0x50 + vpage][((line & 0x0f) * 1024) + vaddr] = value;
                }
            }
            else
            {
                this.MapWrite4000[addr & 0x3fff] = value;
                //видео-страница закрыта, значит выводим в спектрумовском режиме
                if ((_portY & 64) == 0)
                {
                    //бит 6==0, значит вывод в видео-озу в спектрумовском режиме разрешен
                    //необходимо транспонировать адрес спектрумовского экрана в адрес спринтеровского озу
                    byte vrampg = (byte)((addr & 0xf0) >> 4);
                    ushort vrampg_row = (ushort)((addr & 0x0f) * 1024);
                    ushort vrampg_col = (ushort)((addr & 0x1f00) >> 8);

                    if ((addr & 0x3fff) <= 0x1fff)
                    {
                        _vramPages[vrampg][vrampg_row + vrampg_col + (_vblock4000 * 32)] = value;
                    }
                    else
                    {
                        if ((_portY & 128) == 0)
                            _vramPages[vrampg][vrampg_row + vrampg_col + (((_vblock4000 + 1) & 31) * 32)] = value;
                    }

                }
            }
        }

        private new void WriteMem8000(ushort addr, byte value)
        {
            if (_accMode != AccelCMD.Off && _covoxBlaster != null && _page2 == 0xFD)
            {
                // accel only
                _covoxBlaster.WriteMemory(addr, value);
            }
            if ((_page2 >= 0x50) && (_page2 <= 0x5f))
            {
                //открыта видеостраница, пишем в нее
                ushort vaddr = (ushort)(addr & 0x03ff);
                //00111100 00000000
                byte line = (byte)(_portY + ((addr & 0x3c00) >> 10));
                byte vpage = (byte)((line & 0xF0) >> 4);

                // Номер страницы ВидеоОЗУ:
                // 0x50 - нормальная запись
                // 3 бит номера - разрешение записи байта #FF в видео ОЗУ
                // 2 бит номера - разрешение записи в основное ОЗУ
                if (((_page2 & 8) == 0) || (((_page2 & 8) != 0) && (value != 0xFF)))
                    _vramPages[vpage][((line & 0x0f) * 1024) + vaddr] = value;
                if ((_page2 & 4) == 0)
                {
                    RamPages[0x50 + vpage][((line & 0x0f) * 1024) + vaddr] = value;
                    //   this.MapWrite8000[addr & 0x3fff] = value;
                }
            }
            else
            {
                this.MapWrite8000[addr & 0x3fff] = value;
            }
        }

        private new void WriteMemC000(ushort addr, byte value)
        {
            if (_accMode != AccelCMD.Off && _covoxBlaster != null && _page3 == 0xFD)
            {
                // accel only
                _covoxBlaster.WriteMemory(addr, value);
            }
            if ((_page3 >= 0x50) && (_page3 <= 0x5f))
            {
                //открыта видеостраница, пишем в нее
                ushort vaddr = (ushort)(addr & 0x03ff);
                //00111100 00000000
                byte line = (byte)(_portY + ((addr & 0x3c00) >> 10));
                byte vpage = (byte)((line & 0xF0) >> 4);

                // Номер страницы ВидеоОЗУ:
                // 0x50 - нормальная запись
                // 3 бит номера - разрешение записи байта #FF в видео ОЗУ
                // 2 бит номера - разрешение записи в основное ОЗУ
                if (((_page3 & 8) == 0) || (((_page3 & 8) != 0) && (value != 0xFF)))
                    _vramPages[vpage][((line & 0x0f) * 1024) + vaddr] = value;
                if ((_page3 & 4) == 0)
                {
                    RamPages[0x50 + vpage][((line & 0x0f) * 1024) + vaddr] = value;
                    //= value;
                }
            }
            else
            {
                //Нет проверки на открытие спектрумовской графической страницы!!!


                this.MapWriteC000[addr & 0x3fff] = value;
                if (((this.CMR0 & 7) == 5) || ((this.CMR0 & 7) == 7))
                {
                    int blok = ((this.CMR0 & 7) == 5) ? _vblock4000 : _vblockC000;
                    //видео-страница закрыта, значит выводим в спектрумовском режиме если открыты стр 5 или 7
                    if ((_portY & 64) == 0)
                    {
                        //бит 6==0, значит вывод в видео-озу в спектрумовском режиме разрешен
                        //необходимо транспонировать адрес спектрумовского экрана в адрес спринтеровского озу
                        byte vrampg = (byte)((addr & 0xf0) >> 4);
                        ushort vrampg_row = (ushort)((addr & 0x0f) * 1024);
                        ushort vrampg_col = (ushort)((addr & 0x1f00) >> 8);

                        if ((addr & 0x3fff) <= 0x1fff)
                        {
                            _vramPages[vrampg][vrampg_row + vrampg_col + (blok * 32)] = value;
                        }
                        else
                        {
                            if ((_portY & 128) == 0)
                                _vramPages[vrampg][vrampg_row + vrampg_col + (((blok + 1) & 31) * 32)] = value;
                        }

                    }
                }
            }
        }

        private new void ReadMemC000(ushort addr, ref byte value)
        {
            if ((_page3 >= 0x50) && (_page3 <= 0x5f))
            {
                //открыта видеостраница, пишем в нее
                ushort vaddr = (ushort)(addr & 0x03ff);
                //00111100 00000000
                byte line = (byte)(_portY + ((addr & 0x3c00) >> 10));
                byte vpage = (byte)((line & 0xF0) >> 4);

                // Номер страницы ВидеоОЗУ:
                // 0x50 - нормальная запись
                // 3 бит номера - разрешение записи байта #FF в видео ОЗУ
                // 2 бит номера - разрешение записи в основное ОЗУ
                value = RamPages[0x50 + vpage][((line & 0x0f) * 1024) + vaddr];
                //m_vramPages[vpage][((line & 0x0f) * 1024) + vaddr];
            }
            else
            {
                base.ReadMemC000(addr, ref value);
                //value = this.MapReadC000[addr & 0x3fff];
            }
        }

        private new void ReadMem8000(ushort addr, ref byte value)
        {
            if ((_page2 >= 0x50) && (_page2 <= 0x5f))
            {
                //открыта видеостраница, пишем в нее
                ushort vaddr = (ushort)(addr & 0x03ff);
                //00111100 00000000
                byte line = (byte)(_portY + ((addr & 0x3c00) >> 10));
                byte vpage = (byte)((line & 0xF0) >> 4);

                // Номер страницы ВидеоОЗУ:
                // 0x50 - нормальная запись
                // 3 бит номера - разрешение записи байта #FF в видео ОЗУ
                // 2 бит номера - разрешение записи в основное ОЗУ
                value = RamPages[0x50 + vpage][((line & 0x0f) * 1024) + vaddr];
                //m_vramPages[vpage][((line & 0x0f) * 1024) + vaddr];
            }
            else
            {
                base.ReadMem8000(addr, ref value);
                //value = this.MapReadC000[addr & 0x3fff];
            }
        }

        private new void ReadMem4000(ushort addr, ref byte value)
        {
            if ((_page1 >= 0x50) && (_page1 <= 0x5f))
            {
                //открыта видеостраница, пишем в нее
                ushort vaddr = (ushort)(addr & 0x03ff);
                //00111100 00000000
                byte line = (byte)(_portY + ((addr & 0x3c00) >> 10));
                byte vpage = (byte)((line & 0xF0) >> 4);

                // Номер страницы ВидеоОЗУ:
                // 0x50 - нормальная запись
                // 3 бит номера - разрешение записи байта #FF в видео ОЗУ
                // 2 бит номера - разрешение записи в основное ОЗУ
                value = RamPages[0x50 + vpage][((line & 0x0f) * 1024) + vaddr];
                //m_vramPages[vpage][((line & 0x0f) * 1024) + vaddr];
            }
            else
            {
                base.ReadMem4000(addr, ref value);
                //value = this.MapReadC000[addr & 0x3fff];
            }
        }

#if Debug
        private void readRamM1(ushort addr, ref byte value)
        {
            _opaddr = addr;
        }

        private void LogPort(ushort port, byte value)
        {
            Logger.Info("#{0:X4}: Write to port #{1:X4} value #{2:X2}", _opaddr, port, value);
        }
#endif

        private void ReadPort00(ushort addr, ref byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;
            value = 0;
        }

        private void WritePort89(ushort addr, byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;

            _portY = value;
            if (_ulaSprinter != null)
            {
                _ulaSprinter.RGADR = value;
            }

            //Перепроверить!!! тут в TASM происходит глюк непонятный
            /*                if ((this.CMR0 & 2) == 0)
                            {
                                m_vblok4000 = (byte)(value & 31);
                                m_vblokC000 = (byte)((m_vblok4000 + 1) & 31);//((value & 31) + 1)
                            }
                            else
                            {
                                m_vblokC000 = (byte)(value & 31);
                                m_vblok4000 = (byte)((m_vblokC000 + 1) & 31);//((value & 31) + 1);
                            }*/
            _vblock4000 = (byte)(value & 31);
            _vblockC000 = (byte)((value & 31) ^ 1);
#if Debug
                LogPort(addr, value);
#endif
        }

        private void ReadPort89(ushort addr, ref byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;
            value = _portY;
        }

        private void WritePortC9(ushort addr, byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;
            
            _portVideoMode = value;
            if (_ulaSprinter != null)
            {
                _ulaSprinter.RGMOD = value;
            }
#if Debug
                LogPort(addr, value);
#endif
        }

        private void ReadPortC9(ushort addr, ref byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;
            value = _portVideoMode;
        }

        private void WritePortE9(ushort addr, byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;

            _portScr = value;
#if Debug
                LogPort(addr, value);
#endif
        }

        private void ReadPortE9(ushort addr, ref byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;
            value = _portScr;
        }

        private void WritePort7C(ushort addr, byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;

            _sysPort = value;
            //Если в порт 0x7C/0x3C записано значение 01, то включается ROM Expansion
            if ((addr & 64) == 64) this._romA16 = (value & 0x03) == 0x01;//) ? true : false;
            //                Logger.GetLogger().LogMessage(String.Format("Write to port #{0:X4} value #{1:X2}", addr, value));

            //                this.lb.Items.Add(String.Format("Write to port #{0:X4} value #{1:X2}", addr, value));
            _sys = (addr & 64) == 0;//) ? true : false;
#if Debug
                LogPort(addr, value);
                Logger.Info("State: SYS - {0}, AROM16 - {1}", _sys, _romA16);
#endif

            //                this.lb.Items.Add(String.Format("State: SYS - {0}, AROM16 - {1}", this.m_sys, this.m_romA16));
            if ((value & 0x04) != 0 && _sprinterBdi != null)
            {
                _sprinterBdi.OpenPorts = ((value & 0x1C) == 0x1C ? true : false);
            }
            this.UpdateMapping();

            //                if ((value & 0x03) == 1) this.m_rom_exp = true;
            //                    else this.m_rom_exp = false;
            //                this.m_1ffd = value;
        }

        private void ReadPortFB(ushort addr, ref byte value, ref bool handled)
        {
            if (handled)
                return;
            
            //Включение кеша
            this._cache = true;
            this.UpdateMapping();
        }

        private void ReadPort7B(ushort addr, ref byte value, ref bool handled)
        {
            if (handled)
                return;

            //Выключение кеша
            this._cache = false;
            this.UpdateMapping();
        }

        private void WritePort1FFD(ushort addr, byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;
            
            this.CMR1 = value;
            this.UpdateMapping();
#if Debug
                LogPort(addr, value);
#endif
        }

        private void WritePort7FFD(ushort addr, byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;

            this.CMR0 = value;
            this.UpdateMapping();
#if Debug
                LogPort(addr, value);
#endif
        }

        private void ReadPort82(ushort addr, ref byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;

            if (_firstRead)
            {
                this.PAGE0 = 0x40;
                this.UpdateMapping();
                _firstRead = false;
            }
            value = this.PAGE0;
        }

        private void WritePort82(ushort addr, byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;

            this.PAGE0 = value;
            this.UpdateMapping();
#if Debug
                LogPort(addr, value);
#endif
        }

        private void ReadPortA2(ushort addr, ref byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;

            if (_firstRead)
            {
                this.PAGE1 = 0x40;
                this.UpdateMapping();
                _firstRead = false;
            }
            value = this.PAGE1;
        }

        private void WritePortA2(ushort addr, byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;

            this.PAGE1 = value;
            this.UpdateMapping();
#if Debug
                LogPort(addr, value);
#endif
        }

        private void ReadPortC2(ushort addr, ref byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;

            if (_firstRead)
            {
                this.PAGE2 = 0x40;
                this.UpdateMapping();
                _firstRead = false;
            }
            value = this.PAGE2;
        }

        private void WritePortC2(ushort addr, byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;

            this.PAGE2 = value;
            this.UpdateMapping();
#if Debug
                LogPort(addr, value);
#endif
        }

        private void ReadPortE2(ushort addr, ref byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;

            if (_firstRead)
            {
                this.PAGE3 = 0x40;
                this.UpdateMapping();
                _firstRead = false;
            }
            //  MessageBox.Show("Reading from PAGE3 Port: "+Convert.ToString(this.PAGE3));
            value = this.PAGE3;
        }

        private void WritePortE2(ushort addr, byte value, ref bool handled)
        {
            if (handled)
                return;
            handled = true;

            this.PAGE3 = value;
            this.UpdateMapping();
#if Debug
                LogPort(addr, value);
#endif

            //  MessageBox.Show("Write to PAGE3 Port: " + Convert.ToString(value));
        }

        #endregion I/O Handlers


        #region RAM/ROM

        public byte[][] VRamPages
        {
            get { return _vramPages; }
        }

        public byte[][] CachePages
        {
            get { return _cramPages; }
        }

        #endregion RAM/ROM


        #region Properties

        [HardwareValue("PAGE0", Description = "Port 82 (PAGE0)")]
        public byte PAGE0
        {
            get { return _page0; }
            set
            {
                if (_page0 != value)
                {
                    _page0 = value;
                    this.UpdateMapping();
                }
            }
        }

        [HardwareValue("PAGE1", Description = "Port A2(PAGE1)")]
        public byte PAGE1
        {
            get { return _page1; }
            set
            {
                if (_page1 != value)
                {
                    _page1 = value;
                    this.UpdateMapping();
                }
            }
        }

        [HardwareValue("PAGE2", Description = "Port C2 (PAGE2)")]
        public byte PAGE2
        {
            get { return _page2; }
            set
            {
                if (_page2 != value)
                {
                    _page2 = value;
                    this.UpdateMapping();
                }
            }
        }

        [HardwareValue("PAGE3", Description = "Port E2 (PAGE3)")]
        public byte PAGE3
        {
            get { return _page3; }
            set
            {
                if (_page3 != value)
                {
                    _page3 = value;
                    this.UpdateMapping();
                }
            }
        }

        [HardwareValue("SYSPORT", Description = "Port XX7C/XX3C")]
        public byte SYSPORT
        {
            get { return _sysPort; }
        }

        [HardwareValue("RGADR", Description = "Port 89 (RGADR)")]
        public byte RGADR
        {
            get { return _portY; }
        }

        [HardwareValue("RGMOD", Description = "Port C9 (RGMOD)")]
        public byte RGMOD
        {
            get { return _portVideoMode; }
        }

        [HardwareValue("SYS", Description = "Variable SYS")]
        public bool SYS
        {
            get { return _sys; }
            set
            {
                _sys = value;
                UpdateMapping();
            }
        }

        [HardwareValue("RA16", Description = "Variable RA16")]
        public bool RA16
        {
            get { return _romA16; }
            set
            {
                _romA16 = value;
                UpdateMapping();
            }
        }
        
        #endregion Properties
    }
}
