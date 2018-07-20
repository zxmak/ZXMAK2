using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZXMAK2.Hardware.Adlers.Core
{
    class Historization
    {
        //Goto address in memory dump panel
        private static ushort _memDumpGotoAddress = 0;
        public static ushort MemDumpGotoAddress
        {
            get { return _memDumpGotoAddress; }
            set { _memDumpGotoAddress = value;}
        }

        //Goto address in disassembly panel
        private static ushort _disassemblyGotoAddress = 0;
        public static ushort DisassemblyGotoAddress
        {
            get { return _disassemblyGotoAddress; }
            set { _disassemblyGotoAddress = value; }
        }

        //find bytes in memory
        private static string _findBytesInMem = "#AFC9, 201";
        public static string FindBytesInMemory
        {
            get { return _findBytesInMem; }
            set { _findBytesInMem = value; }
        }
    }
}
