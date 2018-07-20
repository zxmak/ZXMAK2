/* 
 *  Copyright 2007 - 2018 Alex Makeev
 * 
 *  This file is part of ZXMAK2 (ZX Spectrum virtual machine).
 *
 *  ZXMAK2 is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  ZXMAK2 is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with ZXMAK2.  If not, see <http://www.gnu.org/licenses/>.
 *  
 * 
 */
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Serializers.SnapshotSerializers;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine;

namespace ZXMAK2.Serializers.SnapshotSerializers
{
    public class IntelHexSerializer : SnapshotSerializerBase
    {
        public IntelHexSerializer(Spectrum spec)
            : base(spec)
        {
        }

        public override string FormatExtension { get { return "HEX"; } }

        public override bool CanDeserialize { get { return true; } }

        public override void Deserialize(Stream stream)
        {
            var reader = new StreamReader(stream);
            var list = new List<HexItem>();
            var lineNumber = 0;
            while (!reader.EndOfStream)
            {
                lineNumber++;
                var line = reader.ReadLine();
                if (!line.StartsWith(":"))
                {
                    Locator.Resolve<IUserMessage>()
                        .Error("Intel HEX loader\n\nInvalid Intel HEX file at line" + lineNumber.ToString());
                }
                line = line.Substring(1).Trim().Replace(" ", "");
                if ((line.Length & 1)!=0)
                {
                    Locator.Resolve<IUserMessage>()
                        .Error("Intel HEX loader\n\nIncorrect data at line" + lineNumber.ToString());
                }
                var data = new byte[line.Length / 2];
                for (var i=0; i < data.Length; i++)
                {
                    var hex = line.Substring(i*2, 2).ToLowerInvariant();
                    if (!hex.All(arg=>char.IsDigit(arg)||(arg >= 'a' && arg <= 'f')))
                    {
                        Locator.Resolve<IUserMessage>()
                            .Error("Intel HEX loader\n\nIncorrect data at line" + lineNumber.ToString() +
                            ", position "+(i*2+1).ToString());
                    }
                    data[i] = Convert.ToByte(hex, 16);
                }
                var leng = data[0];
                if (data.Length != leng + 4+1)
                {
                    Locator.Resolve<IUserMessage>()
                        .Error("Intel HEX loader\n\nInvalid data length at line" + lineNumber.ToString());
                }
                byte checkSum = 0;
                for (var i=0; i < data.Length - 1;i++)
                {
                    checkSum += data[i];
                }
                checkSum = (byte)(~checkSum + 1); // get two's complement
                if (checkSum != data[data.Length-1])
                {
                    Locator.Resolve<IUserMessage>()
                        .Error("Intel HEX loader\n\nChecksum error at line" + lineNumber.ToString());
                }
                var item = new HexItem();
                item.Addr = (ushort)((data[1] << 8) | data[2]);
                item.Type = data[3];
                item.Data = new byte[leng];
                Array.Copy(data, 4, item.Data, 0, leng);
                if (item.Type == 1) break; // End Of File
                list.Add(item);
            }
            var memory = _spec.BusManager.FindDevice<IMemoryDevice>();
            foreach (var item in list)
            {
                if (item.Type != 0)
                {
                    Logger.Warn("IntelHex: skip type: {0}, addr: {1}, data: {2}",
                        item.Type, item.Addr, 
                        string.Join("", item.Data.Select(arg=>string.Format(CultureInfo.InvariantCulture, "{0:x02}")).ToArray()));
                    continue;
                }
                if (item.Addr < 0x4000)
                {
                    Logger.Warn("IntelHex: skip rom addr: {1}",
                        item.Type, item.Addr, 
                        string.Join("", item.Data.Select(arg=>string.Format(CultureInfo.InvariantCulture, "{0:x02}")).ToArray()));
                    continue;
                }
                for (var i=0; i < item.Data.Length; i++)
                {
                    ushort locAddr = (ushort)(item.Addr + i);
                    var page = locAddr>>14;
                    if (page == 0) continue;
                    memory.WRMEM_DBG(locAddr, item.Data[i]);
                }
            }
            UpdateState();
        }

        private class HexItem
        {
            public ushort Addr;
            public byte[] Data;
            public byte Type;
        }
    }
}
