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
using System.Text;
using System.Text.RegularExpressions;

namespace ZXMAK2.Hardware.GdbServer.Gdb
{
	public class GDBPacket
	{
		private readonly byte[] _message;
		private readonly int _length;
		private string _text;

        private char _commandName;
        private readonly string[] _parameters;
		
		private static readonly Regex RemovePrefix = new Regex(@"^[\+\$]+", RegexOptions.Compiled);
		
		public GDBPacket(byte[] message, int length)
		{
			_message = message;
			_length = length;
			
			var encoder = new ASCIIEncoding();
			_text = encoder.GetString(message, 0, length);
			
			var request = RemovePrefix.Replace(_text, "");
            if (String.IsNullOrEmpty(request))
            {
                _commandName = '\0';
            }
            else
            {
                _commandName = request[0];
                _parameters = request.Substring(1).Split(new char[] { ',', '#', ':', ';' });
            }
		}
		
		public override string ToString ()
		{
			return _text;
		}
		
		public byte[] GetBytes()
		{
			return _message;
		}
		
		public int Length
		{
			get { return _length; }
		}
		
		public char CommandName
		{
			get { return _commandName; }
		}
		
		public string[] GetCommandParameters()
		{
			return _parameters;
		}

        public static string CalculateCRC(string str)
		{
			var encoder = new ASCIIEncoding();
			var bytes = encoder.GetBytes(str);
			var crc = 0;
            for (var i = 0; i < bytes.Length; i++)
            {
                crc += bytes[i];
            }
			return ((byte)crc).ToLowEndianHexString().ToLowerInvariant();
		}
	}
}

