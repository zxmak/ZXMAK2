using System;
using System.IO;

using ZXMAK2.Engine;
using ZXMAK2.Engine.Cpu;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Serializers.SnapshotSerializers
{
    public class SnaSerializer : SnapshotSerializerBase
	{
		public SnaSerializer(Spectrum spec)
            : base(spec)
		{
		}

		#region FormatSerializer

		public override string FormatExtension { get { return "SNA"; } }

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


		private void loadFromStream(Stream stream)
		{
            var memory = _spec.BusManager.FindDevice<IMemoryDevice>();
            var ula = _spec.BusManager.FindDevice<IUlaDevice>();

			if (stream.Length != 49179 && stream.Length != 131103)
			{
                Locator.Resolve<IUserMessage>()
                    .Error("SNA loader\n\nInvalid SNA file size!");
				return;
			}

			byte[] regsData = new byte[0x1B];
			stream.Read(regsData, 0, 0x1B);
			
			InitStd128K();

			memory.CMR0 = 0x30;	// 48K blocked

			byte[] ram = new byte[0x4000 * 3];
			stream.Read(memory.RamPages[5], 0, 0x4000);
			stream.Read(memory.RamPages[2], 0, 0x4000);
			stream.Read(memory.RamPages[0], 0, 0x4000);

			ula.PortFE = regsData[26];

			var regs = _spec.CPU.regs;
			regs.I = regsData[0];
			regs._HL = (ushort)(regsData[1] + 256 * regsData[2]);
			regs._DE = (ushort)(regsData[3] + 256 * regsData[4]);
			regs._BC = (ushort)(regsData[5] + 256 * regsData[6]);
			regs._AF = (ushort)(regsData[7] + 256 * regsData[8]);
			regs.HL = (ushort)(regsData[9] + 256 * regsData[10]);
			regs.DE = (ushort)(regsData[11] + 256 * regsData[12]);
			regs.BC = (ushort)(regsData[13] + 256 * regsData[14]);
			regs.IY = (ushort)(regsData[15] + 256 * regsData[16]);
			regs.IX = (ushort)(regsData[17] + 256 * regsData[18]);
			regs.R = regsData[20];
			regs.AF = (ushort)(regsData[21] + 256 * regsData[22]);
			regs.SP = (ushort)(regsData[23] + 256 * regsData[24]);

			_spec.CPU.BINT = false;
			_spec.CPU.XFX = CpuModeEx.None;
			_spec.CPU.FX = CpuModeIndex.None;
			_spec.CPU.HALTED = false;

			_spec.CPU.IFF1 = _spec.CPU.IFF2 = (regsData[19] & 0x04) == 0x04;
			switch (regsData[25] & 0x03)
			{
				case 0: _spec.CPU.IM = 0; break;
				case 1: _spec.CPU.IM = 1; break;
				case 2: _spec.CPU.IM = 2; break;
				default: _spec.CPU.IM = 2; break;
			}
            
			if (stream.Length > 49179)  // if SNA 128K
			{
				ushort tmp = (ushort)stream.ReadByte();
				tmp |= (ushort)(stream.ReadByte() << 8);
				_spec.CPU.regs.PC = tmp;

				byte cmr0 = (byte)stream.ReadByte();
				bool trdos = stream.ReadByte() != 0;
				
				memory.CMR0 = cmr0;
				memory.DOSEN = trdos;

				if ((memory.CMR0 & 7) != 0)
				{
					for (int i = 0; i < 0x4000; i++)
						memory.RamPages[cmr0 & 7][i] = memory.RamPages[0][i];
				}
				for (int page = 0; page < 8; page++)
				{
					if ((page == 5) || (page == 2) || (page == (cmr0 & 7))) 
						continue;
					stream.Read(memory.RamPages[page], 0, 0x4000);
				}
			}
			else
			{
				ushort tmp = ReadMemory(_spec.CPU.regs.SP++);
                tmp |= (ushort)(ReadMemory(_spec.CPU.regs.SP++) << 8);
				_spec.CPU.regs.PC = tmp;
			}
		}

		private void saveToStream(Stream stream)
		{
			byte[] regsData = new byte[0x1B];

            var memory = _spec.BusManager.FindDevice<IMemoryDevice>();
            var ula = _spec.BusManager.FindDevice<IUlaDevice>();

			ushort tsp = (ushort)(_spec.CPU.regs.SP - 2);
			regsData[0] = _spec.CPU.regs.I;
			regsData[1] = (byte)(_spec.CPU.regs._HL & 0xFF); regsData[2] = (byte)(_spec.CPU.regs._HL >> 8);
			regsData[3] = (byte)(_spec.CPU.regs._DE & 0xFF); regsData[4] = (byte)(_spec.CPU.regs._DE >> 8);
			regsData[5] = (byte)(_spec.CPU.regs._BC & 0xFF); regsData[6] = (byte)(_spec.CPU.regs._BC >> 8);
			regsData[7] = (byte)(_spec.CPU.regs._AF & 0xFF); regsData[8] = (byte)(_spec.CPU.regs._AF >> 8);
			regsData[9] = (byte)(_spec.CPU.regs.HL & 0xFF); regsData[10] = (byte)(_spec.CPU.regs.HL >> 8);
			regsData[11] = (byte)(_spec.CPU.regs.DE & 0xFF); regsData[12] = (byte)(_spec.CPU.regs.DE >> 8);
			regsData[13] = (byte)(_spec.CPU.regs.BC & 0xFF); regsData[14] = (byte)(_spec.CPU.regs.BC >> 8);
			regsData[15] = (byte)(_spec.CPU.regs.IY & 0xFF); regsData[16] = (byte)(_spec.CPU.regs.IY >> 8);
			regsData[17] = (byte)(_spec.CPU.regs.IX & 0xFF); regsData[18] = (byte)(_spec.CPU.regs.IX >> 8);
			regsData[20] = _spec.CPU.regs.R;
			regsData[21] = (byte)(_spec.CPU.regs.AF & 0xFF); regsData[22] = (byte)(_spec.CPU.regs.AF >> 8);
			//regsData[23] = (byte)(tsp & 0xFF); regsData[24] = (byte)(tsp >> 8);

			regsData[25] = (byte)(_spec.CPU.IM & 0x03);
			regsData[19] = (byte)(_spec.CPU.IFF2 ? 0x04 : 0x00);

            regsData[26] = ula.PortFE;


			if (memory.IsMap48)  // blocked?
			{
                regsData[23] = (byte)(tsp & 0xFF); 
				regsData[24] = (byte)(tsp >> 8);
                stream.Write(regsData, 0, 0x1B);

				byte t1 = ReadMemory(tsp);
				WriteMemory(tsp++, (byte)(_spec.CPU.regs.PC & 0xFF));
				byte t2 = ReadMemory(tsp);
				WriteMemory(tsp++, (byte)(_spec.CPU.regs.PC >> 8));
				tsp -= 2;
				stream.Write(memory.RamPages[memory.Map48[1]], 0, 0x4000);		// wr seg 5
				stream.Write(memory.RamPages[memory.Map48[2]], 0, 0x4000);		// wr seg 2
				stream.Write(memory.RamPages[memory.Map48[3]], 0, 0x4000);		// wr seg X
				WriteMemory(tsp++, t1);
				WriteMemory(tsp++, t2);
			}
			else // non blocked = 128K
			{
                regsData[23] = (byte)(_spec.CPU.regs.SP & 0xFF); 
				regsData[24] = (byte)(_spec.CPU.regs.SP >> 8);
                stream.Write(regsData, 0, 0x1B);

				byte cmr0 = memory.CMR0;
				stream.Write(memory.RamPages[5], 0, 0x4000);                // wr seg 5
				stream.Write(memory.RamPages[2], 0, 0x4000);                // wr seg 2
				stream.Write(memory.RamPages[cmr0 & 7], 0, 0x4000);    // wr seg X

				stream.WriteByte((byte)(_spec.CPU.regs.PC & 0xFF));
				stream.WriteByte((byte)(_spec.CPU.regs.PC >> 8));
				stream.WriteByte(cmr0);

				byte trdosPaged = 0x00;  // non use!!!
				stream.WriteByte(trdosPaged);

				for (int page = 0; page < 8; page++)
				{
					if ((page == 5) || (page == 2) || (page == (cmr0 & 7))) continue;
					stream.Write(memory.RamPages[page], 0, 0x4000);
				}
			}
		}
	}
}
