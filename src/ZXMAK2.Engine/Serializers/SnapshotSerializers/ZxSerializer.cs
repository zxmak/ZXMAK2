using System;
using System.IO;

using ZXMAK2.Engine.Cpu;
using ZXMAK2.Engine;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Serializers.SnapshotSerializers
{
    public class ZxSerializer : SnapshotSerializerBase
	{
		public ZxSerializer(Spectrum spec)
            : base(spec)
		{
		}


		#region FormatSerializer

		public override string FormatExtension { get { return "ZX"; } }

		public override bool CanDeserialize { get { return true; } }

		public override void Deserialize(Stream stream)
		{
			loadFromStream(stream);
            UpdateState();
        }

		#endregion


		#region private
		private void loadFromStream(Stream stream)
		{
            byte[] memdump = new byte[49284];
			byte[] hdr = new byte[0xCA];
			if (stream.Length != 49486)
			{
                Locator.Resolve<IUserMessage>()
                    .Error("ZX loader\n\nInvalid file size, file corrupt!");
				return;
			}
			stream.Read(memdump, 0, memdump.Length);
			stream.Read(hdr, 0, hdr.Length);

            InitStd128K();

            var memory = _spec.BusManager.FindDevice<IMemoryDevice>();
			memory.CMR0 = 0x30;	// Disable 128K

			//IUlaDevice ula = _spec.BusManager.FindDevice(typeof(IUlaDevice)) as IUlaDevice;
			//ula.PortFE = hdr.border;

			// set memory...
			for (int i = 0; i < 0x4000; i++)
				memory.RamPages[5][i] = memdump[i + 132];
			for (int i = 0; i < 0x4000; i++)
				memory.RamPages[2][i] = memdump[i + 132 + 0x4000];
			for (int i = 0; i < 0x4000; i++)
				memory.RamPages[0][i] = memdump[i + 132 + 0x8000];
			
			// set Z80 registers...
			_spec.CPU.regs._HL = (ushort)(hdr[0xA0] << 8 | hdr[0xA1]);
			_spec.CPU.regs._DE = (ushort)(hdr[0x9C] << 8 | hdr[0x9D]);
			_spec.CPU.regs._BC = (ushort)(hdr[0x98] << 8 | hdr[0x99]);
			_spec.CPU.regs._AF = (ushort)(hdr[0xAB] << 8 | hdr[0xAF]);
			_spec.CPU.regs.HL = (ushort)(hdr[0x9E] << 8 | hdr[0x9F]);
			_spec.CPU.regs.DE = (ushort)(hdr[0x9A] << 8 | hdr[0x9B]);
			_spec.CPU.regs.BC = (ushort)(hdr[0x96] << 8 | hdr[0x97]);
			_spec.CPU.regs.AF = (ushort)(hdr[0xAD] << 8 | hdr[0xB1]);
			_spec.CPU.regs.IR = (ushort)(hdr[0xA6] << 8 | hdr[0xA7]);
			_spec.CPU.regs.IX = (ushort)(hdr[0xA2] << 8 | hdr[0xA3]);
			_spec.CPU.regs.IY = (ushort)(hdr[0xA4] << 8 | hdr[0xA5]);
			_spec.CPU.regs.SP = (ushort)(hdr[0xB8] << 8 | hdr[0xB9]);
			_spec.CPU.regs.PC = (ushort)(hdr[0xB4] << 8 | hdr[0xB5]);

			_spec.CPU.BINT = false;
			_spec.CPU.XFX = CpuModeEx.None;
			_spec.CPU.FX = CpuModeIndex.None;

			_spec.CPU.IFF1 = _spec.CPU.IFF2 = (hdr[0x8E] & 1) != 0;
			_spec.CPU.HALTED = (hdr[0xBD] & 1) != 0;
			switch (getUInt16(hdr, 0xBE))   // IM mode
			{
				case 0:  _spec.CPU.IM = 1; break;
				case 1:  _spec.CPU.IM = 2; break;
				default: _spec.CPU.IM = 0; break;
			}
		}
		#endregion
	}
}
