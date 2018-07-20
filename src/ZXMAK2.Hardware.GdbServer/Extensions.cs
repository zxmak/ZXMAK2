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
	public static class Extensions
	{
		public static string ToLowEndianHexString(this byte number)
		{
			return number.ToString("X").PadLeft(2, '0');
		}
		
		public static string ToLowEndianHexString(this ushort number)
		{
			return ((byte)(number & 0xFF)).ToLowEndianHexString() + ((byte)(number >> 8)).ToLowEndianHexString();
		}
	}
}

