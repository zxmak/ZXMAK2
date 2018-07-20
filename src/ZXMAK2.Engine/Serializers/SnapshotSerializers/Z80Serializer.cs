using System;
using System.IO;

using ZXMAK2.Engine.Cpu;
using ZXMAK2.Engine;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Serializers.SnapshotSerializers
{
    public class Z80Serializer : SnapshotSerializerBase
	{
		public Z80Serializer(Spectrum spec)
            : base(spec)
		{
		}

		#region FormatSerializer

		public override string FormatExtension { get { return "Z80"; } }

		public override bool CanDeserialize { get { return true; } }
		public override bool CanSerialize { get { return true; } }

		public override void Deserialize(Stream stream)
		{
			loadFromStream(stream);
            UpdateState();
        }

		public override void Serialize(Stream stream)
		{
			saveToStream(stream);
		}

		#endregion


		#region private

		private void loadFromStream(Stream stream)
		{
			byte[] hdr = new byte[30];
			byte[] hdr1 = new byte[25];
			int version = 1;

			stream.Read(hdr, 0, 30); // 30 bytes

			if (hdr[Z80HDR_FLAGS] == 0xFF)
				hdr[Z80HDR_FLAGS] = 0x01; // Because of compatibility, if byte 12 is 255, it has to be regarded as being 1.

			if (getUInt16(hdr, Z80HDR_PC) == 0)  // if Version >= 1.45 ( 2.01 or 3.0 )
			{
				version = 2;
				stream.Read(hdr1, 0, 25);

				if (hdr1[Z80HDR1_EXTSIZE] == 54)   // if Version is 3.0
				{
					version = 3;
					byte[] bhdr2 = new byte[31];
					stream.Read(bhdr2, 0, 31);
				}
                else if (hdr1[Z80HDR1_EXTSIZE] == 55)
                {
					version = 3;
					byte[] bhdr2 = new byte[31];
					stream.Read(bhdr2, 0, 31);
                    /*p1FFD = (byte)*/ stream.ReadByte();
                }
                else if (hdr1[Z80HDR1_EXTSIZE] != 23)
                {
                    var msg = string.Format(
                        "Z80 format version not recognized!\n" +
                        "(ExtensionSize = {0},\n" +
                        "supported only ExtensionSize={{0(old format), 23, 54}})",
                        hdr1[Z80HDR1_EXTSIZE]);
                    Logger.Warn("{0}", msg);
                    Locator.Resolve<IUserMessage>()
                        .Error("Z80 loader\n\n{0}", msg);
                    return;
                }
			}

            InitStd128K();

            var memory = _spec.BusManager.FindDevice<IMemoryDevice>();
            var ula = _spec.BusManager.FindDevice<IUlaDevice>();

			// Load registers:
			_spec.CPU.regs.A = hdr[Z80HDR_A];
			_spec.CPU.regs.F = hdr[Z80HDR_F];
			_spec.CPU.regs.HL = getUInt16(hdr, Z80HDR_HL);
			_spec.CPU.regs.DE = getUInt16(hdr, Z80HDR_DE);
			_spec.CPU.regs.BC = getUInt16(hdr, Z80HDR_BC);
			_spec.CPU.regs._AF = (ushort)(hdr[Z80HDR_A_] * 0x100 + hdr[Z80HDR_F_]);
			_spec.CPU.regs._HL = getUInt16(hdr, Z80HDR_HL_);
			_spec.CPU.regs._DE = getUInt16(hdr, Z80HDR_DE_);
			_spec.CPU.regs._BC = getUInt16(hdr, Z80HDR_BC_);
			_spec.CPU.regs.IX = getUInt16(hdr, Z80HDR_IX);
			_spec.CPU.regs.IY = getUInt16(hdr, Z80HDR_IY);
			_spec.CPU.regs.SP = getUInt16(hdr, Z80HDR_SP);
			_spec.CPU.regs.I = hdr[Z80HDR_I];
			_spec.CPU.regs.R = (byte)(hdr[Z80HDR_R7F] | (((hdr[Z80HDR_FLAGS] & 1) != 0) ? 0x80 : 0x00));
			_spec.CPU.regs.PC = (version == 1) ? getUInt16(hdr, Z80HDR_PC) : getUInt16(hdr1, Z80HDR1_PC);

			_spec.CPU.BINT = false;
			_spec.CPU.XFX = CpuModeEx.None;
			_spec.CPU.FX = CpuModeIndex.None;
			_spec.CPU.HALTED = false;

			// CPU.Status...
			_spec.CPU.IFF1 = hdr[Z80HDR_IFF1] != 0;
			_spec.CPU.IFF2 = hdr[Z80HDR_IFF2] != 0;
			switch (hdr[Z80HDR_CONFIG] & 3)
			{
				case 0: _spec.CPU.IM = 0; break;
				case 1: _spec.CPU.IM = 1; break;
				case 2: _spec.CPU.IM = 2; break;
				default: _spec.CPU.IM = 0; break;
			}

			// Others...
			ula.PortFE = (byte)((hdr[Z80HDR_FLAGS] >> 1) & 0x07);

			if (version > 1)
			{
                if (hdr1[Z80HDR1_IF1PAGED] == 0xFF)
                {
                    Logger.Warn("Z80Serializer.loadFromStream: Interface I not implemented, but Interface I ROM required!");
                }

				// Load AY-3-8910 registers
                LoadAyState(hdr1);
			}

			bool dataCompressed = (hdr[Z80HDR_FLAGS] & 0x20) != 0;
			bool mode128 = false;

			if (version == 2)
			{
                Logger.Debug("Z80HDR1_HWMODE: 0x{0:X2} [{1}]",
                    hdr1[Z80HDR1_HWMODE],
                    GetModelNameV2(hdr1[Z80HDR1_HWMODE]));
                switch (hdr1[Z80HDR1_HWMODE])
				{
					case 0:  // 48k
					case 1:  // 48k + If.1
                        mode128 = false;
						break;
					case 2:  // SamRam
						Logger.Warn("Z80Serializer.loadFromStream: SamRam not implemented!");
                        mode128 = true;
						break;
					case 3:  // 128k
					case 4:  // 128k + If.1
                    case 7:  // Spectrum +3
					case 9:  // [ext] Pentagon (128K)
					case 10: // [ext] Scorpion (256K)
						mode128 = true;
                        break;
                    default:
						Logger.Warn(
                            "Z80Serializer.loadFromStream: Unrecognized ZX Spectrum config! (0x{0:X2})",
                            hdr1[Z80HDR1_HWMODE]);
						mode128 = true;
						break;
				}
			}
			if (version == 3)
			{
                Logger.Debug("Z80HDR1_HWMODE: 0x{0:X2} [{1}]",
                    hdr1[Z80HDR1_HWMODE],
                    GetModelNameV3(hdr1[Z80HDR1_HWMODE]));
                switch (hdr1[Z80HDR1_HWMODE])
				{
					case 0:  // 48k
					case 1:  // 48k + If.1
					case 2:  // SamRam
						Logger.Warn("Z80Serializer.loadFromStream: SamRam not implemented!");
						mode128 = false;
						break;
					case 3:  // 48k + M.G.T.
						mode128 = false;
						break;
					case 4:  // 128k
					case 5:  // 128k + If.1
					case 6:  // 128k + M.G.T.
                    case 7:  // Spectrum +3
                    case 9:  // [ext] Pentagon (128K)
					case 10: // [ext] Scorpion (256K)
						mode128 = true;
						break;
					default:
						Logger.Warn(
                            "Z80Serializer.loadFromStream: Unrecognized ZX Spectrum config! (0x{0:X2})",
                            hdr1[Z80HDR1_HWMODE]);
						mode128 = true;
						break;
				}
			}

			// Set 7FFD...
			byte p7FFD = 0x30;	// Lock 48K page 0
			if (version == 1)
			{
				p7FFD = 0x30;	// Lock 48K page 0
			}
			else
			{
				if (mode128)
					p7FFD = hdr1[Z80HDR1_SR7FFD];
				else
					p7FFD = 0x30; // Lock 48K page 0
			}
			memory.CMR0 = p7FFD;
            //memory.CMR1 = p1FFD;


			// load rampages
			if (version == 1)
			{
				int comprSize = (int)(stream.Length - stream.Position);
				if (comprSize < 0) comprSize = 0;

				byte[] buf = new byte[comprSize + 1024];
				stream.Read(buf, 0, comprSize);

				byte[] memdump = new byte[0x1FFFF];

				if (dataCompressed) 
				{	
					DecompressZ80(memdump, buf, 0xC000);
				}
				else
				{	
					for (int i = 0; i < 0xC000; i++)
						memdump[i] = buf[i];
				}

				int currPage = p7FFD & 0x07;
				for (int i = 0; i < 0x4000; i++)
					memory.RamPages[5][i] = memdump[i];
				for (int i = 0; i < 0x4000; i++)
					memory.RamPages[2][i] = memdump[i + 0x4000];
				for (int i = 0; i < 0x4000; i++)
					memory.RamPages[currPage][i] = memdump[i + 0x8000];
			}
			else
			{
				byte[] bitbuf = new byte[4];

				int blockSize = 0;   //WORD
				int blockNumber = 0; //byte
				byte[] block = new byte[129000];
				byte[] rawdata = new byte[0x4000];

				while (stream.Position < stream.Length)
				{
					stream.Read(bitbuf, 0, 2);
					blockSize = getUInt16(bitbuf, 0);

					stream.Read(bitbuf, 0, 1);
					blockNumber = bitbuf[0];

                    if (blockSize!=0xFFFF)
                    {
                        stream.Read(block, 0, blockSize);
                        DecompressZ80(rawdata, block, 0x4000);
                    }
                    else
                    {
                        blockSize = 0x4000;
                        stream.Read(rawdata, 0, blockSize);
                    }


					if (blockNumber >= 3 && blockNumber <= 10 && mode128)
					{
						for (int i = 0; i < 0x4000; i++)
							memory.RamPages[(blockNumber-3)&7][i] = rawdata[i];
					}
					else if (((blockNumber == 4) || (blockNumber == 5) || (blockNumber == 8)) && (!mode128))
					{
						int page48 = p7FFD & 0x07;
						if (blockNumber == 8) page48 = 5;
						if (blockNumber == 4) page48 = 2;

						for (int i = 0; i < 0x4000; i++)
							memory.RamPages[page48][i] = rawdata[i];
					}
					else if (blockNumber == 0)
					{
                        //int rom48index = memory.GetRomIndex(RomName.ROM_SOS);
                        //for (int i = 0; i < 0x4000; i++)
                        //    memory.RomPages[rom48index][i] = rawdata[i];
                        Logger.Warn("Z80Serializer.loadFromStream: Skip ROM 48K image!");
					}
					else if (blockNumber == 2)
					{
                        //int rom128index = memory.GetRomIndex(RomName.ROM_128);
                        //for (int i = 0; i < 0x4000; i++)
                        //    memory.RomPages[rom128index][i] = rawdata[i];
                        Logger.Warn("Z80Serializer.loadFromStream: Skip ROM 128K image!");
					}
				}
			}
		}

        private void LoadAyState(byte[] hdr1)
        {
            var aydev = _spec.BusManager.FindDevice<IPsgDevice>();
            if (aydev == null)
            {
                return;
            }
            for (var i = 0; i < 16; i++)
            {
                aydev.SetReg(i, hdr1[Z80HDR1_AYSTATE + i]);
            }
        }

		private void saveToStream(Stream stream)
		{
            var memory = _spec.BusManager.FindDevice<IMemoryDevice>();
            var ula = _spec.BusManager.FindDevice<IUlaDevice>();

			// TODO: align to nonprefix+halt!!!
			//while (_spec.CPU.FX != OPFX.NONE || _spec.CPU.XFX != OPXFX.NONE)
			//    _spec.CPU.ExecCycle();

			byte[] hdr = new byte[30];
			byte[] hdr1 = new byte[25];

			setUint16(hdr, Z80HDR_PC, 0);          // Extended header present... (only if 128K)
			setUint16(hdr1, Z80HDR1_EXTSIZE, 23);  // Z80 V2.01
            
			// Save regs:
			hdr[Z80HDR_A] = _spec.CPU.regs.A;
			hdr[Z80HDR_F] = _spec.CPU.regs.F;
			setUint16(hdr, Z80HDR_HL, _spec.CPU.regs.HL);
			setUint16(hdr, Z80HDR_DE, _spec.CPU.regs.DE);
			setUint16(hdr, Z80HDR_BC, _spec.CPU.regs.BC);

			hdr[Z80HDR_A_] = (byte)(_spec.CPU.regs._AF >> 8);
			hdr[Z80HDR_F_] = (byte)_spec.CPU.regs._AF;
			setUint16(hdr, Z80HDR_HL_, _spec.CPU.regs._HL);
			setUint16(hdr, Z80HDR_DE_, _spec.CPU.regs._DE);
			setUint16(hdr, Z80HDR_BC_, _spec.CPU.regs._BC);

			setUint16(hdr, Z80HDR_IX, _spec.CPU.regs.IX);
			setUint16(hdr, Z80HDR_IY, _spec.CPU.regs.IY);
			setUint16(hdr, Z80HDR_SP, _spec.CPU.regs.SP);

            hdr[Z80HDR_FLAGS] = 0; //clear

            hdr[Z80HDR_I] = _spec.CPU.regs.I;
			hdr[Z80HDR_R7F] = (byte)(_spec.CPU.regs.R & 0x7F);
			if ((_spec.CPU.regs.R & 0x80) != 0) 
				hdr[Z80HDR_FLAGS] |= 0x01;

			hdr[Z80HDR_FLAGS] |= (byte)((ula.PortFE & 7) << 1);
			hdr[Z80HDR_FLAGS] |= 0x20; // compression

			setUint16(hdr1, Z80HDR1_PC, _spec.CPU.regs.PC);

			// CPU.Status...
			if (_spec.CPU.IFF1) hdr[Z80HDR_IFF1] = 0xFF;
			else hdr[Z80HDR_IFF1] = 0x00;
			if (_spec.CPU.IFF2) hdr[Z80HDR_IFF2] = 0xFF;
			else hdr[Z80HDR_IFF2] = 0x00;

			hdr[Z80HDR_CONFIG] = _spec.CPU.IM;
			if (_spec.CPU.IM > 2) 
				hdr[Z80HDR_CONFIG] = 0x00;
			//   hdr[Z80HDR_CONFIG] |= 0x04; // ?? эмуляция 2-го выпуска спектрума

			bool mode128 = !memory.IsMap48;		// 48K / 128K ?

			if (!mode128)
				setUint16(hdr, Z80HDR_PC, _spec.CPU.regs.PC);   // if 48K use V1.45

			hdr1[Z80HDR1_HWMODE] = 0x03;				// 0-48K, 3-128K , but use V1.45 for 48K

			byte p7FFD = memory.CMR0;//0x30;
			hdr1[Z80HDR1_SR7FFD] = p7FFD;
			hdr1[Z80HDR1_IF1PAGED] = 0x00;
			hdr1[Z80HDR1_STUFF] = 0x03;                  // R & LDIR emulation enable

			hdr1[Z80HDR1_7FFD] = 0x0E;       // ?? dont know what is it, but other emuls doing that (128K only)

			// Save AY registers
            var aydev = _spec.BusManager.FindDevice<IPsgDevice>();
			if (aydev != null)
			{
				for (var i = 0; i < 16; i++)
				{
					hdr1[Z80HDR1_AYSTATE + i] = aydev.GetReg(i);
				}
			}
			else
			{
				for (int i = 0; i < 16; i++)
					hdr1[Z80HDR1_AYSTATE + i] = 0xFF;
			}

			// Save headers and memory dumps...
			int blockSize = 0;
			int blockNumber = 0;
			byte[] block = new byte[200000];

			if (!mode128)         // For 48K
			{
				byte[] ram48 = new byte[0xFFFF];
				
				for (int i = 0; i < 0x4000; i++)
					ram48[i+0x0000] = memory.RamPages[memory.Map48[1]][i];
				for (int i = 0; i < 0x4000; i++)
					ram48[i+0x4000] = memory.RamPages[memory.Map48[2]][i];
				for (int i = 0; i < 0x4000; i++)
					ram48[i + 0x8000] = memory.RamPages[memory.Map48[3]][i];
				
				blockSize = CompressZ80(block, ram48, 0x4000 * 3);
				if ((blockSize + 4) >= (0x4000 * 3))       // Disable compression in case when compression is not effective
				{
					hdr[Z80HDR_FLAGS] &= unchecked((byte)~0x20);
					blockSize = 0x4000 * 3;
					for (int i = 0; i < blockSize; i++)
						block[i] = ram48[i];
				}
				// Save header V1.45...
				stream.Write(hdr, 0, 30);

				// Save 48K block...
				stream.Write(block, 0, blockSize);

				if ((hdr[Z80HDR_FLAGS] & 0x20) != 0) // in case when block is compressed write end-marker
				{
					byte[] endmark = new byte[4] { 0x00, 0xED, 0xED, 0x00 };
					stream.Write(endmark, 0, 4);
				}
			}
			else                 // for 128K
			{
				// Save headers V2.01...
				stream.Write(hdr, 0, 30);
				stream.Write(hdr1, 0, 25);    // V2.01 if 128K

				for (int i = 0; i < 8; i++)
				{
					byte[] blockData = memory.RamPages[i];
					blockSize = CompressZ80(block, blockData, 0x4000);
					blockNumber = (i & 0x07) + 3;
					stream.Write(getBytes(blockSize), 0, 2);
					stream.Write(getBytes(blockNumber), 0, 1);
					stream.Write(block, 0, blockSize);
				}
			}
		}

        private static string GetModelNameV2(byte id)
        {
            switch (id)
            {
                case 0: return "48k";
                case 1: return "48k + If.1";
                case 2: return "SamRam";
                case 3: return "128k";
                case 4: return "128k + If.1";
                case 7: return "Spectrum +3";
                case 8: return "Spectrum +3m";
                case 9: return "Pentagon (128K)";
                case 10: return "Scorpion (256K)";
                case 11: return "Didaktik-Kompakt";
                case 12: return "Spectrum +2";
                case 13: return "Spectrum +2A";
                case 14: return "TC2048";
                case 15: return "TC2068";
                case 128: return "TS2068";
                default: return "Unknown";
            }
        }

        private static string GetModelNameV3(byte id)
        {
            switch (id)
            {
                case 0: return "48k";
                case 1: return "48k + If.1";
                case 2: return "SamRam";
                case 3: return "48k + M.G.T.";
                case 4: return "128k";
                case 5: return "128k + If.1";
                case 6: return "128k + M.G.T.";
                case 7: return "Spectrum +3";
                case 8: return "Spectrum +3m";
                case 9: return "Pentagon (128K)";
                case 10: return "Scorpion (256K)";
                case 11: return "Didaktik-Kompakt";
                case 12: return "Spectrum +2";
                case 13: return "Spectrum +2A";
                case 14: return "TC2048";
                case 15: return "TC2068";
                case 128: return "TS2068";
                default: return "Unknown";
            }
        }

		#endregion

		#region struct offsets
		private const int Z80HDR_A = 0;
		private const int Z80HDR_F = 1;
		private const int Z80HDR_BC = 2;
		private const int Z80HDR_HL = 4;
		private const int Z80HDR_PC = 6;   // for New Format == 0 !
		private const int Z80HDR_SP = 8;
		private const int Z80HDR_I = 10;
		private const int Z80HDR_R7F = 11;
		private const int Z80HDR_FLAGS = 12;
		// if(Flags==0xFF) Flags = 0x01
		//                        Bit 0  : Равен 7-му биту регистра R
		//                        Bit 1-3: Цвет бордюра
		//                        Bit 4  : 1- впечатан БЕЙСИК SamRam
		//                        Bit 5  : 1- блок данных компрессирован
		//                        Bit 6-7: Не значащие
		private const int Z80HDR_DE = 13;
		private const int Z80HDR_BC_ = 15;
		private const int Z80HDR_DE_ = 17;
		private const int Z80HDR_HL_ = 19;
		private const int Z80HDR_A_ = 21;
		private const int Z80HDR_F_ = 22;
		private const int Z80HDR_IY = 23;
		private const int Z80HDR_IX = 25;
		private const int Z80HDR_IFF1 = 27;  // if(0) DI
		private const int Z80HDR_IFF2 = 28;
		private const int Z80HDR_CONFIG = 29;
		//                        Bit 0-1: Режим прерываний (0, 1 или 2)
		//                        Bit 2  : 1=эмуляция 2-го выпуска Спектрума
		//                        Bit 3  : 1=Двойная частота прерываний
		//                        Bit 4-5: 1=Высокая видеосинхронизация
		//                                 3=Низкая видеосинхронизация
		//                                 0,2=Нормальная
		//                        Bit 6-7: 0=Курсор/Protek/AGF- джойстик
		//                                 1=Кемпстон-джойстик
		//                                 2=Sinclair-левый джойстик
		//                                 3=Sinclair-правый джойстик

		private const int Z80HDR1_EXTSIZE = 0;  // Длина дополнительного блока (23 for V2.01; 54 for V3.0)
		private const int Z80HDR1_PC = 2;
		private const int Z80HDR1_HWMODE = 4;   // Hardware mode
		//      Число         Значение в версии 2.01    Значение в версии 3.0
		//
		//        0               48k                     48k
		//        1               48k + If.1              48k + If.1
		//        2               SamRam                  48k + M.G.T.
		//        3               128k                    SamRam
		//        4               128k + If.1             128k
		//        5               -                       128k + If.1
		//        6               -                       128k + M.G.T.
		private const int Z80HDR1_SR7FFD = 5;
		private const int Z80HDR1_IF1PAGED = 6;
		private const int Z80HDR1_STUFF = 7;   //D0: R emulation on; D1: LDIR emulation on
		private const int Z80HDR1_7FFD = 8;    //???
		private const int Z80HDR1_AYSTATE = 9;
		#endregion

		#region compression
		private int CompressZ80(byte[] dest, byte[] src, int SrcSize)
		{
			int DestSize = dest.Length;
			// code was taken from SPCONV
			uint NO = 0;
			uint YES = 1;

			uint i, j;
			uint num;
			byte c, n;
			uint ed;

			i = 0;
			j = 0;
			/* ensure 'ed' is not set */
			ed = NO;
			while (i < SrcSize)
			{
				c = src[i];
				i++;
				if (i < SrcSize)
				{
					n = src[i];
				}
				else
				{
					/* force 'n' to be unequal to 'c' */
					n = c;
					n++;
				}

				if (c != n)
				{
					dest[j] = c;
					j++;
					if (c == 0xed) ed = YES;
					else ed = NO;
				}
				else
				{
					if (c == 0xed)
					{
						/* two times 0xed - special care */
						dest[j] = 0xed;
						j++;
						dest[j] = 0xed;
						j++;
						dest[j] = 0x02;
						j++;
						dest[j] = 0xed;
						j++;
						i++; /* skip second ED */

						/* because 0xed is valid compressed we don't
						   have to watch it! */
						ed = NO;
					}
					else if (ed == YES)
					{
						/* can't compress now, skip this double pair */
						dest[j] = c;
						j++;
						ed = NO;  /* 'c' can't be 0xed */
					}
					else
					{
						num = 1;
						while (i < SrcSize)
						{
							if (c != src[i]) break;
							num++;
							i++;
							if (num == 255) break;
						}
						if (num <= 4)
						{
							/* no use to compress */
							while (num != 0)
							{
								dest[j] = c;
								j++;
								num--;
							}
						}
						else
						{
							dest[j] = 0xed;
							j++;
							dest[j] = 0xed;
							j++;
							dest[j] = (byte)num;
							j++;
							dest[j] = c;
							j++;
						}
					}
				}

				if (j >= DestSize)
				{
                    Locator.Resolve<IUserMessage>()
                        .Warning("Z80 loader\n\nCompression error: buffer overflow,\nfile can contain invalid data!");

					/* compressed image bigger or same than dest buffer */
					for (int k = 0; k < SrcSize; k++)
						dest[k] = src[k];
					return SrcSize;
				}
			}
			return (int)j;
		}
		private int DecompressZ80(byte[] dest, byte[] src, int size)
		{
			uint c, j;
			uint k;
			byte l;
			byte im;

			uint i = 0;

			j = 0;
			while (j < size)
			{
				c = src[i++];
				//      if(c == -1) return;
				im = (byte)c;

				if (im != 0xed)
				{
					dest[j++] = im;
				}
				else
				{
					c = src[i++];
					//         if(c == -1) return;
					im = (byte)c;
					if (im != 0xed)
					{
						dest[j++] = 0xed;
						i--;
					}
					else
					{
						/* fetch count */
						k = src[i++];
						//            if(k == -1) return;

						/* fetch character */
						c = src[i++];
						//            if(c == -1) return;
						l = (byte)c;
						while (k != 0)
						{
							dest[j++] = l;
							k--;
						}
					}
				}
			}

			if (j != size)
			{
                string msg = string.Format(
                    "Decompression error - file corrupt? (j={0}, size={1})", 
                    j, 
                    size);
                Logger.Error("Z80Serializer: {0}", msg);
                Locator.Resolve<IUserMessage>()
                    .Error("Z80 loader\n\n{0}", msg);
				return 1;
			}
			return size;
		}
		#endregion
	}

    #region New Z80 Loader

    //public class Z80StreamHelper
    //{
    //    public int Version;

    //    public ushort AF;
    //    public ushort BC;
    //    public ushort DE;
    //    public ushort HL;
    //    public ushort IX;
    //    public ushort IY;
    //    public ushort eAF;
    //    public ushort eBC;
    //    public ushort eDE;
    //    public ushort eHL;

    //    public ushort PC;
    //    public ushort SP;
    //    public ushort IR;
    //    public bool IFF1;
    //    public bool IFF2;
    //    public byte IM;
        
    //    public byte PFE;
    //    public byte P7FFD;
    //    public byte P1FFD;
    //    public bool IsCompressed;
    //    public bool Is128;
    //    public bool IsSamRom;

    //    public byte HwMode;

    //    // ext hdr
    //    public byte[] AyData = new byte[16];
    //    public byte AyIndex = 0;
    //    public int Tact;
        
    //    public void Deserialize(Stream stream)
    //    {
    //        byte tmp1, tmp2, tmp3;
    //        Version = -1;
            
    //        // read main header...
    //        StreamHelper.Read(stream, out tmp1);    // A
    //        StreamHelper.Read(stream, out tmp2);    // F
    //        AF = (ushort)(tmp1 + 0x100 * tmp2);
            
    //        StreamHelper.Read(stream, out BC);
    //        StreamHelper.Read(stream, out HL);
    //        StreamHelper.Read(stream, out PC);
    //        StreamHelper.Read(stream, out SP);
            
    //        StreamHelper.Read(stream, out tmp1);    // Interrupt register
    //        StreamHelper.Read(stream, out tmp2);    // Refresh register (Bit 7 is not significant!)
    //        StreamHelper.Read(stream, out tmp3);
    //        // offset 12:
    //        // Because of compatibility, if byte 12 is 255, it has to be regarded as being 1.
    //        if (tmp1 == 255)
    //            tmp1 = 1;
    //        //Bit 0  : Bit 7 of the R-register
    //        //Bit 1-3: Border colour
    //        //Bit 4  : 1=Basic SamRom switched in
    //        //Bit 5  : 1=Block of data is compressed
    //        //Bit 6-7: No meaning
    //        IR = (ushort)(tmp1 + 0x100 * ((tmp2 & 0x7F) | ((tmp3 & 0x01)<<7)));
    //        PFE = (byte)((tmp3 >> 1) & 7);
    //        IsSamRom = (tmp3 & 0x10) != 0;
    //        IsCompressed = (tmp3 & 0x20) != 0;
            
    //        StreamHelper.Read(stream, out DE);
    //        StreamHelper.Read(stream, out eBC);
    //        StreamHelper.Read(stream, out eDE);
    //        StreamHelper.Read(stream, out eHL);
            
    //        StreamHelper.Read(stream, out tmp1);    // A'
    //        StreamHelper.Read(stream, out tmp2);    // F'
    //        eAF = (ushort)(tmp1 + 0x100 * tmp2);
            
    //        StreamHelper.Read(stream, out IY);
    //        StreamHelper.Read(stream, out IX);

    //        StreamHelper.Read(stream, out tmp1);    // IFF1
    //        StreamHelper.Read(stream, out tmp2);    // IFF2
    //        IFF1 = tmp1 != 0;
    //        IFF2 = tmp2 != 0;

    //        StreamHelper.Read(stream, out tmp1);
    //        //Bit 0-1: Interrupt mode (0, 1 or 2)
    //        //Bit 2  : 1=Issue 2 emulation
    //        //Bit 3  : 1=Double interrupt frequency
    //        //Bit 4-5: 1=High video synchronisation
    //        //         3=Low video synchronisation
    //        //         0,2=Normal
    //        //Bit 6-7: 0=Cursor/Protek/AGF joystick
    //        //         1=Kempston joystick
    //        //         2=Sinclair 2 Left joystick (or user defined, for version 3 .z80 files)
    //        //         3=Sinclair 2 Right joystick
    //        IM = (byte)(tmp1 & 0x03);

    //        // Version < 2?
    //        if (PC == 0)
    //        {
    //            Version = 1;
    //            Is128 = false;
    //            return;
    //        }

    //        ushort extLen;
    //        StreamHelper.Read(stream, out extLen);    // Length of additional header block
    //        if (extLen < 23)
    //        {
    //            LogAgent.Error(
    //                "Z80StreamReader: not supported extended header ({0} bytes)",
    //                extLen);
    //            return;
    //        }
    //        StreamHelper.Read(stream, out PC);
    //        byte[] extHdr = new byte[extLen - 2];
    //        StreamHelper.Read(stream, extHdr);

    //        if (extLen == 23)
    //        {
    //            Version = 2;
    //        }
    //        else if (extLen == 54 || extLen == 55)
    //        {
    //            Version = 3;
    //        }

    //        HwMode = extHdr[0];             // offset 34
    //        P7FFD = extHdr[1];              // offset 35
    //        AyIndex = extHdr[4];            // offset 38
    //        for (int i = 0; i < 16; i++)
    //            AyData[i] = extHdr[i + 5];  // offset 39-54
            
    //        Tact = 0;
    //        P1FFD = 0;
    //        if (Version == 3)
    //        {
                
    //            Tact = extHdr[21] + 0x100 * extHdr[22] + 0x10000 * extHdr[23]; // offset 55,56,57
    //            if (extLen == 55)
    //            {
    //                P1FFD = extHdr[52]; // offset 86
    //            }
    //        }
    //        Is128 = checkMode128(Version, HwMode);
    //    }

    //    public Z80Block ReadNext(Stream stream)
    //    {
    //        if (Version == 1)
    //            return readNext1(stream);
    //        else if (Version == 2)
    //            return readNext2n3(stream);
    //        else return null;
    //    }

    //    private Z80Block readNext1(Stream stream)
    //    {
    //        int len = IsCompressed ? (int)(stream.Length - stream.Position) :
    //            0xC000;
    //        byte[] buffer = new byte[len];
    //        stream.Read(buffer, 0, buffer.Length);

    //        if (IsCompressed)
    //        {
    //            byte[] data = new byte[0xC000];
    //            decompress(data, buffer, data.Length);
    //            buffer = data;
    //        }
    //        return new Z80Block(buffer);
    //    }

    //    private Z80Block readNext2n3(Stream stream)
    //    {
    //        byte[] buffer = new byte[3];
    //        int read = stream.Read(buffer, 0, 3);
    //        if (read != 3)
    //            return null;
    //        int len = BitConverter.ToUInt16(buffer, 0);
    //        int pageId = buffer[2];
    //        if (len == 0xFFFF)
    //        {
    //            // not compressed
    //            len = 0x4000;
    //            buffer = new byte[len];
    //            read = stream.Read(buffer, 0, len);
    //            if (read != len)
    //            {
    //                LogAgent.Warn(
    //                    "Z80StreamHelper: unexpected end of stream (read {0} of {1} bytes)",
    //                    read,
    //                    len);
    //            }
    //        }
    //        else
    //        {
    //            // compressed
    //            buffer = new byte[len];
    //            read = stream.Read(buffer, 0, len);
    //            if (read != len)
    //            {
    //                LogAgent.Warn(
    //                    "Z80StreamHelper: unexpected end of stream (read {0} of {1} bytes)",
    //                    read,
    //                    len);
    //            }
    //            byte[] data = new byte[0x4000];
    //            decompress(data, buffer, data.Length);
    //            buffer = data;
    //        }
    //        return new Z80Block(Is128, pageId, buffer); 
    //    }

    //    private bool decompress(byte[] dest, byte[] src, int size)
    //    {
    //        try
    //        {
    //            uint c, j;
    //            uint k;
    //            byte l;
    //            byte im;

    //            uint i = 0;

    //            j = 0;
    //            while (j < size)
    //            {
    //                c = src[i++];
    //                //      if(c == -1) return;
    //                im = (byte)c;

    //                if (im != 0xed)
    //                {
    //                    dest[j++] = im;
    //                }
    //                else
    //                {
    //                    c = src[i++];
    //                    //         if(c == -1) return;
    //                    im = (byte)c;
    //                    if (im != 0xed)
    //                    {
    //                        dest[j++] = 0xed;
    //                        i--;
    //                    }
    //                    else
    //                    {
    //                        /* fetch count */
    //                        k = src[i++];
    //                        //            if(k == -1) return;

    //                        /* fetch character */
    //                        c = src[i++];
    //                        //            if(c == -1) return;
    //                        l = (byte)c;
    //                        while (k != 0)
    //                        {
    //                            dest[j++] = l;
    //                            k--;
    //                        }
    //                    }
    //                }
    //            }

    //            if (j != size)
    //            {
    //                LogAgent.Error(
    //                    "Z80StreamHelper: Decompression error - file corrupt? (j={0}, size={1})",
    //                    j,
    //                    size);
    //                return false;
    //            }
    //            return true;
    //        }
    //        catch (Exception ex)
    //        {
    //            LogAgent.Error(ex);
    //            return false;
    //        }
    //    }

    //    private static bool checkMode128(int version, byte hwMode)
    //    {
    //        if (version == 2)
    //        {
    //            switch (hwMode)
    //            {
    //                case 0:  // 48k
    //                case 1:  // 48k + If.1
    //                case 2:  // SamRam
    //                    return false;
    //                case 3:  // 128k
    //                case 4:  // 128k + If.1
    //                case 9:  // [ext] Pentagon (128K)
    //                case 10: // [ext] Scorpion (256K)
    //                    return true;
    //                default:
    //                    LogAgent.Warn(
    //                        "Z80StreamSerializer.checkMode128: Unrecognized ZX Spectrum config! (ver={0}, hwMode={1})",
    //                        version,
    //                        hwMode);
    //                    break;
    //            }
    //        }
    //        if (version == 3)
    //        {
    //            switch (hwMode)
    //            {
    //                case 0:  // 48k
    //                case 1:  // 48k + If.1
    //                case 2:  // SamRam
    //                case 3:  // 48k + M.G.T.
    //                    return false;
    //                case 4:  // 128k
    //                case 5:  // 128k + If.1
    //                case 6:  // 128k + M.G.T.
    //                case 9:  // [ext] Pentagon (128K)
    //                case 10: // [ext] Scorpion (256K)
    //                    return true;
    //                default:
    //                    LogAgent.Warn(
    //                        "Z80StreamHelper.checkMode128: Unrecognized ZX Spectrum config! (ver={0}, hwMode={1})",
    //                        version,
    //                        hwMode);
    //                    break;
    //            }
    //        }
    //        return true;
    //    }
    //}

    //public class Z80Block
    //{
    //    private Z80BlockId m_id;
    //    private int m_page;
    //    private byte[] m_data;

    //    public Z80Block(byte[] data)
    //    {
    //        m_id = Z80BlockId.RamFull48;
    //        m_page = 0;
    //        m_data = data;
    //    }

    //    public Z80Block(bool is128, int pageId, byte[] data)
    //    {
    //        parsePageId(is128, pageId);
    //        m_data = data;
    //    }

    //    public Z80BlockId Id { get { return m_id; } }
    //    public int Page { get { return m_page; } }
    //    public byte[] Data { get { return m_data; } }

    //    private void parsePageId(bool is128, int pageId)
    //    {
    //        if (is128)
    //        {
    //            if (pageId >= 3 && pageId <= 10)
    //            {
    //                m_id = Z80BlockId.RamPage;
    //                m_page = (pageId - 3) & 7;
    //                return;
    //            }
    //        }
    //        else
    //        {
    //            if (pageId == 4 || pageId == 5 || pageId == 8)
    //            {
    //                int page48 = 0;
    //                if (pageId == 8) 
    //                    page48 = 5;
    //                if (pageId == 4) 
    //                    page48 = 2;
    //                m_id = Z80BlockId.RamPage;
    //                m_page = page48;
    //                return;
    //            }                
    //        }
    //        if (pageId == 0)
    //        {
    //            m_id = Z80BlockId.Rom48;
    //            m_page = 0;
    //            return;
    //        }
    //        if (pageId == 1)
    //        {
    //            m_id = Z80BlockId.RomPlusD;
    //            m_page = 0;
    //            return;
    //        }
    //        if (pageId == 2)
    //        {
    //            m_id = Z80BlockId.Rom128;
    //            m_page = 0;
    //            return;
    //        }
    //        if (pageId == 11)
    //        {
    //            m_id = Z80BlockId.RomMultiface;
    //            m_page = 0;
    //            return;
    //        }
    //        m_id = Z80BlockId.Unknown;
    //        m_page = 0;
    //        LogAgent.Error("Z80Block.parsePageId: unknown blockId={0}", pageId);
    //    }
    //}
    
    //public enum Z80BlockId
    //{
    //    RamFull48,
    //    RamPage,
    //    Rom48,
    //    Rom128,
    //    RomPlusD,
    //    RomMultiface,
    //    Unknown,
    //}

    #endregion
}
