using System;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Engine.Entities
{
	public class Breakpoint
	{
		protected Breakpoint()
		{
		}

		public Breakpoint(string label, Predicate<IMachineState> check)
		{
			Label = label;
			Check = check;
			Address = null;
		}

		public Breakpoint(ushort addr)
			: this(string.Format("PC==#{0:X4}", addr), state => state.CPU.regs.PC == addr)
		{
			Address = addr;
		}

		public ushort? Address { get; protected set; }
		public string Label { get; protected set; }
		public Predicate<IMachineState> Check { get; protected set; }
	}
}
