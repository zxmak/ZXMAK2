/*
 * Copyright 2011 Alexander Tsidaev
 * 
 * This file is part of z80gdbserver.
 * z80gdbserver is free software: you can redistribute it and/or modify it under the
 * terms of the GNU General Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 * 
 * z80gdbserver is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 * without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with z80gdbserver. 
 * If not, see http://www.gnu.org/licenses/.
 */

using System;

namespace ZXMAK2.Hardware.GdbServer
{
	public class Breakpoint
	{
		public enum BreakpointType { Execution, Read, Write, Access };
		public delegate void BreakPointEventHandler(Breakpoint breakpoint);
		
		public BreakpointType Type { get; set; }
		public ushort Address { get; set; }
		
		static public BreakpointType GetBreakpointType(int type)
		{
			switch(type)
			{
			case 0:
			case 1:
				return BreakpointType.Execution;
			case 2:
				return BreakpointType.Write;
			case 3:
				return BreakpointType.Read;
			case 4:
				return BreakpointType.Access;
			}
			
			throw new Exception("Incorrect parameter passed");
		}
		
		public Breakpoint(BreakpointType type, ushort address)
		{
			this.Type = type;
			this.Address = address;
		}
	}
}

