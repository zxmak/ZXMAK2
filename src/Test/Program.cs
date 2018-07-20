using System;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Engine.Cpu;
using System.Threading;


namespace Test
{
	public class Program
	{
		static void Main(string[] args)
		{
            if (args.Length >= 1 && args[0].ToLower() == "/zexall")
			{
				runZexall();
				return;
			}
            if (args.Length >= 1 && args[0].ToLower() == "/perf")
            {
                runPerf();
                return;
            }
            if (args.Length >= 1 && args[0].ToLower() == "/cpu")
            {
                runCpu();
                return;
            }

			SanityUla("NOP        ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48_Early(), new byte[] { 0x00 }, s_patternUla48_Early_NOP);
			SanityUla("DJNZ       ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48_Early(), new byte[] { 0x10, 0x00 }, s_patternUla48_Early_DJNZ);
			SanityUla("IN A,(#FE) ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48_Early(), new byte[] { 0xDB, 0xFE }, s_patternUla48_Early_INAFE);
			SanityUla("OUT (#FE),A", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48_Early(), new byte[] { 0xD3, 0xFE }, s_patternUla48_Early_OUTAFE);
			SanityUla("LD A,(HL)  ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48_Early(), new byte[] { 0x7E }, s_patternUla48_Early_LDAHL);
			SanityUla("IN A,(C)   ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48_Early(), new byte[] { 0xED, 0x78 }, s_patternUla48_Early_INAC);
			SanityUla("OUT (C),A  ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48_Early(), new byte[] { 0xED, 0x79 }, s_patternUla48_Early_OUTCA);
			SanityUla("BIT 7,A    ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48_Early(), new byte[] { 0xCB, 0x7F }, s_patternUla48_Early_BIT7A);
			SanityUla("BIT 7,(HL) ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48_Early(), new byte[] { 0xCB, 0x7E }, s_patternUla48_Early_BIT7HL);
			SanityUla("SET 7,(HL) ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48_Early(), new byte[] { 0xCB, 0xFE }, s_patternUla48_Early_SET7HL);

			SanityUla("NOP        ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48(), new byte[] { 0x00 }, s_patternUla48_Late_NOP);
			SanityUla("DJNZ       ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48(), new byte[] { 0x10, 0x00 }, s_patternUla48_Late_DJNZ);
			SanityUla("IN A,(#FE) ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48(), new byte[] { 0xDB, 0xFE }, s_patternUla48_Late_INAFE);
			SanityUla("OUT (#FE),A", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48(), new byte[] { 0xD3, 0xFE }, s_patternUla48_Late_OUTAFE);
			SanityUla("LD A,(HL)  ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48(), new byte[] { 0x7E }, s_patternUla48_Late_LDAHL);
			SanityUla("IN A,(C)   ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48(), new byte[] { 0xED, 0x78 }, s_patternUla48_Late_INAC);
			SanityUla("OUT (C),A  ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48(), new byte[] { 0xED, 0x79 }, s_patternUla48_Late_OUTCA);
			SanityUla("BIT 7,A    ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48(), new byte[] { 0xCB, 0x7F }, s_patternUla48_Late_BIT7A);
			SanityUla("BIT 7,(HL) ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48(), new byte[] { 0xCB, 0x7E }, s_patternUla48_Late_BIT7HL);
			SanityUla("SET 7,(HL) ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum48(), new byte[] { 0xCB, 0xFE }, s_patternUla48_Late_SET7HL);

			SanityUla("NOP        ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum128(), new byte[] { 0x00 }, s_patternUla128_NOP);
			SanityUla("INC HL     ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum128(), new byte[] { 0x23 }, s_patternUla128_INCHL);
			SanityUla("LD A,(HL)  ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum128(), new byte[] { 0x7E }, s_patternUla128_LDA_HL_);
			SanityUla("LD (HL),A  ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum128(), new byte[] { 0x77 }, s_patternUla128_LDA_HL_); // pattern the same as ld a,(hl)
			SanityUla("OUT (C),A  ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum128(), new byte[] { 0xED, 0x79, 0x03 }, s_patternUla128_OUTCA);
			SanityUla("IN A,(C)   ", new ZXMAK2.Hardware.Spectrum.UlaSpectrum128(), new byte[] { 0xED, 0x78, 0x03 }, s_patternUla128_OUTCA); // pattern the same as out (c),a

			Console.WriteLine("=====Engine Performance Benchmark (500 frames rendering time)=====");
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            int frameCount = 50 * 10;
			ExecTests("testVideo.z80", frameCount);
			ExecTests("testVideo.z80", frameCount);
			ExecTests("testVideo.z80", frameCount);
			ExecTests("zexall.sna", frameCount);
			ExecTests("zexall.sna", frameCount);
			ExecTests("zexall.sna", frameCount);
			ExecTests("testOutFe.z80", frameCount);
			ExecTests("testOutFe.z80", frameCount);
			ExecTests("testOutFe.z80", frameCount);
            ExecLightTests("zexall.sna", frameCount);
            ExecLightTests("zexall.sna", frameCount);
            ExecLightTests("zexall.sna", frameCount);
            ExecLightTests("zexall.sna", frameCount);
            ExecLightTests("zexall.sna", frameCount);
            ExecCpuTests("zexall.sna", frameCount);
            ExecCpuTests("zexall.sna", frameCount);
            ExecCpuTests("zexall.sna", frameCount);
            ExecCpuTests("zexall.sna", frameCount);
            ExecCpuTests("zexall.sna", frameCount);
        }

		private static void SanityUla(string name, IUlaDevice ula, byte[] opcode, int[] pattern)
		{
			IMemoryDevice mem = new ZXMAK2.Hardware.Spectrum.MemorySpectrum48();// MemoryPentagon128();
            var p128 = GetTestMachine(Resources.machines_test);
			p128.Init();
			p128.BusManager.Disconnect();
			p128.BusManager.Clear();
			p128.BusManager.Add((BusDeviceBase)mem);
			p128.BusManager.Add((BusDeviceBase)ula);
			p128.BusManager.Connect();
			p128.IsRunning = true;
			p128.DebugReset();
			p128.ExecuteFrame();
			p128.IsRunning = false;

			ushort offset = 0x4000;
			for (int i = 0; i < pattern.Length; i++)
			{
				for (int j = 0; j < opcode.Length; j++)
					mem.WRMEM_DBG(offset++, opcode[j]);
			}
			p128.CPU.regs.PC = 0x4000;
			p128.CPU.regs.IR = 0x4000;
			p128.CPU.regs.SP = 0x4000;
			p128.CPU.regs.AF = 0x4000;
			p128.CPU.regs.HL = 0x4000;
			p128.CPU.regs.DE = 0x4000;
			p128.CPU.regs.BC = 0x4000;
			p128.CPU.regs.IX = 0x4000;
			p128.CPU.regs.IY = 0x4000;
			p128.CPU.regs._AF = 0x4000;
			p128.CPU.regs._HL = 0x4000;
			p128.CPU.regs._DE = 0x4000;
			p128.CPU.regs._BC = 0x4000;
			p128.CPU.regs.MW = 0x4000;
			p128.CPU.IFF1 = p128.CPU.IFF2 = false;
			p128.CPU.IM = 2;
			p128.CPU.BINT = false;
			p128.CPU.FX = CpuModeIndex.None;
			p128.CPU.XFX = CpuModeEx.None;

			long needsTact = pattern[0];
			long frameTact = p128.CPU.Tact % ula.FrameTactCount;
			long deltaTact = needsTact - frameTact;
			if (deltaTact < 0)
				deltaTact += ula.FrameTactCount;
			p128.CPU.Tact += deltaTact;

			//if (pattern == s_patternUla48_Late_LDAHL)
			//    p128.Loader.SaveFileName("TEST-LDAHL-48-LATE.SZX");
			//if (pattern == s_patternUla48_Early_LDAHL)
			//    p128.Loader.SaveFileName("TEST-LDAHL-48-EARLY.SZX");

			for (int i = 0; i < pattern.Length - 1; i++)
			{
				p128.DebugStepInto();
				frameTact = p128.CPU.Tact % ula.FrameTactCount;
				if (frameTact != pattern[i + 1])
				{
					ConsoleColor tmp = Console.ForegroundColor;
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(
						"Sanity ULA {0} [{1}]:\tfailed @ {2}->{3} (should be {2}->{4})",
						ula.GetType().Name,
						name,
						pattern[i],
						frameTact,
						pattern[i + 1]);
					Console.ForegroundColor = tmp;
					return;
				}
			}
			p128.BusManager.Disconnect();
			ConsoleColor tmp2 = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Sanity ULA {0} [{1}]:\tpassed", ula.GetType().Name, name);
			Console.ForegroundColor = tmp2;
		}

        private static Spectrum GetTestMachine(string configXml)
        {
            var config = new XmlDocument();
            config.LoadXml(configXml);
            var machine = new Spectrum();
            try
            {
                machine.BusManager.LoadConfigXml(config.DocumentElement);
                //var sxml = new XmlDocument();
                //var node = sxml.AppendChild(sxml.CreateElement("Bus"));
                //machine.BusManager.SaveConfigXml(node);
                //sxml.Save("AAAA.xml");
                return machine;
            }
            catch
            {
                machine.Dispose();
                throw;
            }
        }

		#region ULA PATTERNS

		private static int[] s_patternUla48_Late_NOP = new int[]
        {
            14300, 14304, 14308, 14312, 14316, 14320, 14324, 14328, 14332, 
            14336, 14346, 14354, 14362, 14370, 14378, 14386, 14394, 14402, 14410, 14418, 14426, 14434, 14442, 14450, 14458, 14466, 14470, 14474, 14478, 14482, 14486, 14490, 14494, 14498, 
            14502, 14506, 14510, 14514, 14518, 14522, 14526, 14530, 14534, 14538, 14542, 14546, 14550, 14554, 14558, 14562, 14570, 14578, 14586, 14594, 14602, 14610, 14618, 14626, 14634,
            14642, 14650, 14658, 14666, 14674, 14682, 14690, 14694, 14698, 14702, 14706, 14710, 14714, 14718, 14722, 14726, 14730, 14734, 14738, 14742, 14746, 14750, 14754, 14758, 14762,
            14766, 14770, 14774, 14778, 14782, 14786, 14794, 14802,
        };

		private static int[] s_patternUla48_Early_NOP = new int[]
        {
            14300, 14304, 14308, 14312, 14316, 14320, 14324, 14328, 14332,
            14336, 14345, 14353, 14361, 14369, 14377, 14385, 14393, 14401, 14409, 14417, 14425, 14433, 14441, 14449, 14457, 14465, 14469, 14473, 14477, 14481, 14485, 14489, 14493, 14497,
            14501, 14505, 14509, 14513, 14517, 14521, 14525, 14529, 14533, 14537, 14541, 14545, 14549, 14553, 14557, 14561, 14569, 14577, 14585, 14593, 14601, 14609, 14617, 14625, 14633,
            14641, 14649, 14657, 14665, 14673, 14681, 14689, 14693, 14697, 14701, 14705, 14709, 14713, 14717, 14721, 14725, 14729, 14733, 
        };

		private static int[] s_patternUla48_Late_DJNZ = new int[]
        {
            14300, 14313, 14326, 14351, 14383, 14415, 14447, 14467, 14480, 14493, 14506, 14519, 14532, 14545, 14558, 14591, 14623, 14655, 14687, 14700, 14713, 14726, 14739, 14752, 14765,
            14778, 14807, 14839, 14871, 14903, 14919, 14932, 14945, 14958, 14971, 14984, 14997, 15016, 15055, 15087, 15119, 15139, 15152, 15165, 15178, 15191, 15204, 15217, 15230, 15263,
            15295, 15327,
        };

		private static int[] s_patternUla48_Early_DJNZ = new int[]
        {
            14300, 14313, 14326, 14351, 14390, 14422, 14454, 14470, 14483, 14496, 14509, 14522, 14535, 14548, 14567, 14606, 14638, 14670, 14690, 14703, 14716, 14729, 14742, 14755, 14768,
            14781, 14814, 14846, 14878, 14910, 14923, 14936, 14949, 14962, 14975, 14988, 15001, 15030, 15062, 15094, 15126, 15142, 15155, 15168, 15181, 15194, 15207, 15220, 15239, 15278,
            15310,
        };

		private static int[] s_patternUla48_Late_INAFE = new int[]
        {
            14300, 14311, 14322, 14333, 14353, 14377, 14401, 14425, 14449, 14469, 14480, 14491, 14502, 14513, 14524, 14535, 14546, 14557, 14577, 14601, 14625, 14649, 14673, 14693, 14704,
            14715, 14726, 14737, 14748, 14759, 14770, 14781, 14801, 14825, 14849, 14873, 14897, 14917, 14928, 14939, 14950, 14961, 14972, 14983, 14994, 15005, 15025, 15049, 15073, 15097,
            15121, 15141, 15152,
        };

		private static int[] s_patternUla48_Early_INAFE = new int[]
        {
            14300, 14311, 14322, 14333, 14352, 14376, 14400, 14424, 14448, 14468, 14479, 14490, 14501, 14512, 14523, 14534, 14545, 14556, 14576, 14600, 14624, 14648, 14672, 14692, 14703,
            14714, 14725, 14736, 14747, 14758, 14769, 14780, 14800, 14824, 14848, 14872, 14896, 14916, 14927, 14938, 14949, 14960, 14971, 14982, 14993, 15004, 15024, 15048, 15072, 15096,
            15120, 15140, 15151, 
        };

		private static int[] s_patternUla48_Late_OUTAFE = new int[]
        {
            14300, 14311, 14322, 14333, 14354, 14378, 14402, 14426, 14450, 14469, 14480, 14491, 14502, 14513, 14524, 14535, 14546, 14557, 14578, 14602, 14626, 14650, 14674, 14693, 14704,
            14715, 14726, 14737, 14748, 14759, 14770, 14781, 14802, 14826, 14850, 14874, 14898, 14917, 14928, 14939, 14950, 14961, 14972, 14983, 14994, 15005, 15026, 15050, 15074, 15098,
            15122, 15141, 15152, 15163,
        };

		private static int[] s_patternUla48_Early_OUTAFE = new int[]
        {
            14300, 14311, 14322, 14333, 14353, 14377, 14401, 14425, 14449, 14468, 14479, 14490, 14501, 14512, 14523, 14534, 14545, 14556, 14577, 14601, 14625, 14649, 14673, 14692, 14703, 
            14714, 14725, 14736, 14747, 14758, 14769, 14780, 14801, 14825, 14849, 14873, 14897, 14916, 14927, 14938, 14949, 14960, 14971, 14982, 14993, 15004, 15025, 15049, 15073, 15097, 
            15121, 15140, 15151, 15162,
        };

		private static int[] s_patternUla48_Late_LDAHL = new int[]
        {
            14300, 14307, 14314, 14321, 14328, 14335, 14345, 14361, 14377, 14393, 14409, 14425, 14441, 14457, 14469, 14476, 14483, 14490, 14497, 14504, 14511, 14518, 14525, 14532, 14539,
            14546, 14553, 14560, 14577, 14593, 14609, 14625, 14641, 14657, 14673, 14689, 14696, 14703, 14710, 14717, 14724, 14731, 14738, 14745, 14752, 14759, 14766, 14773, 14780, 14793,
            14809, 14825, 14841, 14857, 14873,
        };

		private static int[] s_patternUla48_Early_LDAHL = new int[]
        {
            14300, 14307, 14314, 14321, 14328, 14335, 14352, 14368, 14384, 14400, 14416, 14432, 14448, 14464, 14471, 14478, 14485, 14492, 14499, 14506, 14513, 14520, 14527, 14534, 14541,
            14548, 14555, 14568, 14584, 14600, 14616, 14632, 14648, 14664, 14680, 14692, 14699, 14706, 14713, 14720, 14727, 14734, 14741, 14748, 14755, 14762, 14769, 14776, 14783, 14800, 
            14816, 14832, 14848, 14864, 14880,
        };

		private static int[] s_patternUla48_Late_INAC = new int[]
        {
            14300, 14312, 14324, 14336, 14362, 14386, 14410, 14434, 14458, 14474, 14486, 14498, 14510, 14522, 14534, 14546, 14558, 14578, 14602, 14626, 14650, 14674, 14694, 14706, 14718,
            14730, 14742, 14754, 14766, 14778, 14794, 14818, 14842, 14866, 14890, 14914, 14926, 14938, 14950, 14962, 14974, 14986, 14998, 15010,
        };

		private static int[] s_patternUla48_Early_INAC = new int[]
        {
            14300, 14312, 14324, 14336, 14361, 14385, 14409, 14433, 14457, 14473, 14485, 14497, 14509, 14521, 14533, 14545, 14557, 14577, 14601, 14625, 14649, 14673, 14693, 14705, 14717,
            14729, 14741, 14753, 14765, 14777, 14793, 14817, 14841, 14865, 14889, 14913, 14925, 14937, 14949, 14961, 14973, 14985, 14997, 15009,
        };

		private static int[] s_patternUla48_Late_OUTCA = new int[]
        {
            14300, 14312, 14324, 14336, 14362, 14386, 14410, 14434, 14458, 14474, 14486, 14498, 14510, 14522, 14534, 14546, 14558, 14578, 14602, 14626, 14650, 14674, 14694, 14706, 14718,
            14730, 14742, 14754, 14766, 14778, 14794, 14818, 14842, 14866, 14890, 14914, 14926, 14938, 14950, 14962, 14974, 14986, 14998, 15010, 15034,
        };

		private static int[] s_patternUla48_Early_OUTCA = new int[]
        {
            14300, 14312, 14324, 14336, 14361, 14385, 14409, 14433, 14457, 14473, 14485, 14497, 14509, 14521, 14533, 14545, 14557, 14577, 14601, 14625, 14649, 14673, 14693, 14705, 14717,
            14729, 14741, 14753, 14765, 14777, 14793, 14817, 14841, 14865, 14889, 14913, 14925, 14937, 14949, 14961, 14973, 14985, 14997, 15009, 15033,
        };

		private static int[] s_patternUla48_Late_BIT7A = new int[]
        {
            14300, 14308, 14316, 14324, 14332, 14346, 14362, 14378, 14394, 14410, 14426, 14442, 14458, 14470, 14478, 14486, 14494, 14502, 14510, 14518, 14526, 14534, 14542, 14550, 14558,
            14570, 14586, 14602, 14618, 14634, 14650, 14666, 14682, 14694, 14702, 14710, 14718, 14726, 14734, 14742, 14750, 14758, 14766, 14774, 14782, 14794,
        };

		private static int[] s_patternUla48_Early_BIT7A = new int[]
        {
            14300, 14308, 14316, 14324, 14332, 14345, 14361, 14377, 14393, 14409, 14425, 14441, 14457, 14469, 14477, 14485, 14493, 14501, 14509, 14517, 14525, 14533, 14541, 14549, 14557,
            14569, 14585, 14601, 14617, 14633, 14649, 14665, 14681, 14693, 14701, 14709, 14717, 14725, 14733, 14741, 14749, 14757, 14765, 14773, 14781, 14793,
        };

		private static int[] s_patternUla48_Late_BIT7HL = new int[]
        {
            14300, 14312, 14324, 14336, 14362, 14386, 14410, 14434, 14458, 14474, 14486, 14498, 14510, 14522, 14534, 14546, 14558, 14578, 14602, 14626, 14650, 14674, 14694, 14706, 14718,
            14730, 14742, 14754, 14766, 14778, 14794, 14818, 14842, 14866, 14890, 14914, 14926, 14938, 14950, 14962, 14974, 14986, 14998, 15010, 15034
        };

		private static int[] s_patternUla48_Early_BIT7HL = new int[]
        {
            14300, 14312, 14324, 14336, 14361, 14385, 14409, 14433, 14457, 14473, 14485, 14497, 14509, 14521, 14533, 14545, 14557, 14577, 14601, 14625, 14649, 14673, 14693, 14705, 14717,
            14729, 14741, 14753, 14765, 14777, 14793, 14817, 14841, 14865, 14889, 14913, 14925, 14937, 14949, 14961, 14973, 14985, 14997, 15009, 15033,
        };

		private static int[] s_patternUla48_Late_SET7HL = new int[]
        {
            14300, 14315, 14330, 14354, 14386, 14418, 14450, 14473, 14488, 14503, 14518, 14533, 14548, 14569, 14602, 14634, 14666, 14693, 14708, 14723, 14738, 14753, 14768, 14783, 14810,
            14842, 14874, 14906, 14925, 14940, 14955, 14970, 14985, 15000, 15026, 15058, 15090, 15122, 15145, 15160, 15175, 15190, 15205,
        };

		private static int[] s_patternUla48_Early_SET7HL = new int[]
        {
            14300, 14315, 14330, 14353, 14385, 14417, 14449, 14472, 14487, 14502, 14517, 14532, 14547, 14568, 14601, 14633, 14665, 14692, 14707, 14722, 14737, 14752, 14767, 14782, 14809,
            14841, 14873, 14905, 14924, 14939, 14954, 14969, 14984, 14999, 15025, 15057, 15089, 15121,
        };

		private static int[] s_patternUla128_NOP = new int[]
        {
			14362, 14372, 14380, 14388, 14396, 14404, 14412, 14420, 14428, 14436, 14444, 14452, 14460, 14468, 14476, 14484, 14492, 
			14496, 14500, 14504, 14508, 14512, 14516, 14520, 14524, 14528, 14532, 14536, 14540, 14544, 14548, 14552, 14556, 14560, 14564, 14568, 14572, 14576, 14580, 14584, 14588, 14592,
			14600, 14608, 14616, 14624, 14632, 14640, 14648, 14656, 14664, 14672, 14680, 14688, 14696, 14704, 14712, 14720, 14724, 
			14728, 14732, 14736,
        };

		private static int[] s_patternUla128_INCHL = new int[]
        {
			14362, 14378, 14394, 14410, 14426, 14442, 14458, 14474, 14490, 
			14496, 14502, 14508, 14514, 14520, 14526, 14532, 14538, 14544, 14550, 14556, 14562, 14568, 14574, 14580, 14586, 14598,
			14614, 14630, 14646, 14662, 14678, 14694, 14710, 14722, 14728,
			14734, 14740, 14746,
        };

		private static int[] s_patternUla128_LDA_HL_ = new int[]
        {
			14362, 14379, 14395, 14411, 14427, 14443, 14459, 14475, 14491, 
			14498, 14505, 14512, 14519, 14526, 14533, 14540, 14547, 14554, 14561, 14568, 14575, 14582, 14589, 14599,
			14615, 14631, 14647, 14663, 14679, 14695, 14711, 14723, 14730, 
			14737,
        };

		private static int[] s_patternUla128_OUTCA = new int[]
        {
			14362, 14388, 14402, 14434, 14450, 14476, 14490, 
			14502, 14508, 14520, 14526, 14538, 14544, 14556, 14562, 14574, 14580, 14592, 
			14606, 14638, 14654, 14680, 14694, 14720, 
			14726, 14738, 14744, 14756, 14762, 14774, 14780, 14792, 14798, 14810, 14816,
			14842, 14858, 14884, 14898, 14930, 14946, 14958, 14964, 14976, 14982,
        };

		#endregion

		private static void runZexall()
		{
            var p128 = GetTestMachine(Resources.machines_test);
			p128.Init();
			p128.IsRunning = true;
			p128.DebugReset();
			p128.ExecuteFrame();
			p128.IsRunning = false;
            foreach (var kbd in p128.BusManager.FindDevices<IKeyboardDevice>())
            {
                kbd.KeyboardState = new FakeKeyboardState(Key.Y);
            }

            using (Stream testStream = GetTestStream("zexall.sna"))
            {
                p128.BusManager.LoadManager.GetSerializer(Path.GetExtension("zexall.sna")).Deserialize(testStream);
            }
			p128.IsRunning = true;
			int frame;
			for (frame = 0; frame < 700000; frame++)
			{
				p128.ExecuteFrame();
				if (frame % 30000 == 0 || ((frame > 630000 && frame < 660000) && frame % 10000 == 0))
				{
					Console.WriteLine(string.Format("{0:D8}", frame));
                    p128.BusManager.LoadManager.SaveFileName(string.Format("{0:D8}.PNG", frame));
				}
			}
            p128.BusManager.LoadManager.SaveFileName(string.Format("{0:D8}.PNG", frame));
			p128.BusManager.Disconnect();
		}

        private static void runPerf()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            var frameCount = 50 * 10;
            while (true)
            {
                ExecTests("zexall.sna", frameCount);
            }
        }

        private static void runCpu()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            var frameCount = 50 * 10;
            while (true)
            {
                ExecCpuTests("zexall.sna", frameCount);
            }
        }

		private static void ExecTests(string testName, int frameCount)
		{
            var p128 = GetTestMachine(Resources.machines_test);
			p128.Init();
			p128.IsRunning = true;
			p128.DebugReset();
			p128.ExecuteFrame();

			p128.IsRunning = false;
			using (Stream testStream = GetTestStream(testName))
                p128.BusManager.LoadManager.GetSerializer(Path.GetExtension(testName)).Deserialize(testStream);
			p128.IsRunning = true;


			Stopwatch watch = new Stopwatch();
			watch.Start();
			for (int frame = 0; frame < frameCount; frame++)
				p128.ExecuteFrame();
			watch.Stop();
			Console.WriteLine("{0}:\t{1} [ms]", testName, watch.ElapsedMilliseconds);
			//p128.Loader.SaveFileName(testName);
			p128.BusManager.Disconnect();
		}

        private static void ExecLightTests(string testName, int frameCount)
        {
            var p128 = GetTestMachine(Resources.machines_testLight);
            p128.Init();
            p128.IsRunning = true;
            p128.DebugReset();
            p128.ExecuteFrame();

            p128.IsRunning = false;
            using (Stream testStream = GetTestStream(testName))
                p128.BusManager.LoadManager.GetSerializer(Path.GetExtension(testName)).Deserialize(testStream);
            p128.IsRunning = true;


            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (int frame = 0; frame < frameCount; frame++)
                p128.ExecuteFrame();
            watch.Stop();
            Console.WriteLine("{0} [light]:\t{1} [ms]", testName, watch.ElapsedMilliseconds);
            //p128.Loader.SaveFileName(testName);
            p128.BusManager.Disconnect();
        }

        private static void ExecCpuTests(string testName, int frameCount)
        {
            var cpu = new CpuUnit();
            cpu.regs.PC = 0;
            cpu.RESET = () => { };
            cpu.NMIACK_M1 = () => { };
            cpu.INTACK_M1 = () => { };
            cpu.RDMEM_M1 = addr => (byte)addr;
            cpu.RDMEM = addr => (byte)addr;
            cpu.WRMEM = (addr,value) => { };
            cpu.RDPORT = addr => 0xFF;
            cpu.WRPORT = (addr, value) => { };
            cpu.RDNOMREQ = addr => { };
            cpu.WRNOMREQ = addr => { };

            Stopwatch watch = new Stopwatch();
            watch.Start();
            for (var cycle = 0L; cycle < 71980 * frameCount; cycle++)
                cpu.ExecCycle();
            watch.Stop();
            Console.WriteLine("{0} [cpu]:\t{1} [ms]", testName, watch.ElapsedMilliseconds);
        }

		private static Stream GetTestStream(string testName)
		{
			testName = string.Format("Test.{0}", testName);
			return Assembly.GetExecutingAssembly().GetManifestResourceStream(testName);
		}
	}

	public class FakeKeyboardState : IKeyboardState
	{
		private readonly bool m_pressSimulation;
		private readonly Key m_key;

		public FakeKeyboardState()
		{
			m_pressSimulation = false;
		}

		public FakeKeyboardState(Key keyPressed)
		{
			m_key = keyPressed;
			m_pressSimulation = true;
		}

		public bool this[Key key]
		{
			get { return m_pressSimulation ? (key == m_key) : false; }
		}
	}
}
