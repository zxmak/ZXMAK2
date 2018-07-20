using System;
using System.IO;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Cpu;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Serializers.SnapshotSerializers
{
    public class SitSerializer : SnapshotSerializerBase
	{
		public SitSerializer(Spectrum spec)
            : base(spec)
		{
		}


		#region FormatSerializer Members

		public override string FormatExtension { get { return "SIT"; } }

		public override bool CanDeserialize { get { return true; } }

		public override void Deserialize(Stream stream)
		{
			loadFromStream(stream);
            UpdateState();
        }

		#endregion


		private void loadFromStream(Stream stream)
		{
			byte[] hdr = new byte[28];
			byte[] romdump = new byte[0x4000];
			byte[] ramdump = new byte[0xC000];

			if (stream.Length != 65564)
			{
                Locator.Resolve<IUserMessage>()
                    .Error("SIT loader\n\nInvalid data, file corrupt!");
				return;
			}
			stream.Read(hdr, 0, hdr.Length);
			stream.Read(romdump, 0, 0x4000);
			stream.Read(ramdump, 0, 0xC000);

            InitStd128K();

            var memory = _spec.BusManager.FindDevice<IMemoryDevice>();
			memory.CMR0 = 0x30;	// Disable 128K

            var ula = _spec.BusManager.FindDevice<IUlaDevice>();
			ula.PortFE = hdr[27];

            int rom48index = memory.GetRomIndex(RomId.ROM_SOS);
            for (int i = 0; i < 0x4000; i++)
                memory.RomPages[rom48index][i] = romdump[i];
			for (int i = 0; i < 0x4000; i++)
				memory.RamPages[5][i] = ramdump[i];
			for (int i = 0; i < 0x4000; i++)
				memory.RamPages[2][i] = ramdump[i+0x4000];
			for (int i = 0; i < 0x4000; i++)
				memory.RamPages[0][i] = ramdump[i+0x8000];

			_spec.CPU.regs.BC = getUInt16(hdr, 0);
			_spec.CPU.regs.DE = getUInt16(hdr, 2);
			_spec.CPU.regs.HL = getUInt16(hdr, 4);
			_spec.CPU.regs.AF = getUInt16(hdr, 6);
			_spec.CPU.regs.IX = getUInt16(hdr, 8);
			_spec.CPU.regs.IY = getUInt16(hdr, 10);
			_spec.CPU.regs.SP = getUInt16(hdr, 12);
			_spec.CPU.regs.PC = getUInt16(hdr, 14);
			_spec.CPU.regs.IR = getUInt16(hdr, 16);
			_spec.CPU.regs._BC = getUInt16(hdr, 18);
			_spec.CPU.regs._DE = getUInt16(hdr, 20);
			_spec.CPU.regs._HL = getUInt16(hdr, 22);
			_spec.CPU.regs._AF = getUInt16(hdr, 24);

			_spec.CPU.BINT = false;
			_spec.CPU.XFX = CpuModeEx.None;
			_spec.CPU.FX = CpuModeIndex.None;
			_spec.CPU.HALTED = false;

			_spec.CPU.IFF1 = _spec.CPU.IFF2 = (hdr[26] & 1) != 0;    // ?
			switch (hdr[26] & 2)   // IM mode
			{
				case 0: _spec.CPU.IM = 1; break;
				case 1: _spec.CPU.IM = 1; break;
				case 2: _spec.CPU.IM = 2; break;
				default: _spec.CPU.IM = 1; break;
			}
		}
	}
}
