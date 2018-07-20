using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ZXMAK2.Engine.Cpu;
using System.Linq;

namespace ZXMAK2.Hardware.Adlers
{
    #region Debugger enums/structs, ...
    // enum BreakPointConditionType
    // e.g.: 1.) memoryVsValue = left is memory reference, right is number(#9C40, %1100, 6755, ...)
    //       2.) valueVsRegister = left is value, right is register value
    //
    public enum BreakPointConditionType 
    { 
        memoryVsValue,
        valueVsRegister, 
        registryVsValue,
        flagVsValue, //e.g. Z == 1, C == 0
        registryMemoryReferenceVsValue,

        //Memory read/write
        memoryRead,
        memoryReadInRange,
        memoryWrite,
        memoryWriteInRange
    };

    public enum DebuggerCommandType
    {
        memoryOrRegistryManipulation,
        breakpointManipulation,
        gotoAdress,
        removeBreakpoint,
        enableBreakpoint,
        disableBreakpoint,
        loadBreakpointsListFromFile,
        saveBreakpointsListToFile,
        showAssembler,
        showGraphicsEditor,
        traceLog,
        Unidentified
    };

    public enum BreakPointAccessType
    {
        memoryAccess,
        memoryWrite,
        memoryChange,
        memoryRead,
        registryValue,
        All,
        Undefined
    };

    //Information about extended breakpoint
    public class BreakpointInfo
    {
        public BreakPointConditionType AccessType { get; set; }

        //is multiconditional breakpoint, e.g.: pc == #38 && A == #45
        public bool IsMulticonditional {get; set;}

        //condition in string, e.g.: "pc", "(#9C40)"
        public string LeftCondition { get; set; }
        public bool LeftIsFlag { get; set; }
        public string RightCondition { get; set; }

        //value of condition(if relevant), valid for whole values or memory access
        public ushort LeftValue { get; set; }
        public ushort RightValue { get; set; }

        public int LeftRegistryArrayIndex { get; set; }

        //condition type
        public string ConditionTypeSign { get; set; }
        public bool IsConditionEquals { get; set; } // true - if values must be equal

        //is active
        public bool IsOn { get; set; }

        //original breakpoint string(raw data get from dbg command line)
        public string BreakpointString { get; set; }

        //value mask - e.g.: for F registry => 0xFF, for A registry => 0xFF00; for AF => isMasked = false
        public bool Is8Bit { get; set; }

        public Func<bool> CheckBreakpoint { get; set; }
        public Func<bool> CheckSecondCondition { get; set; }

        public void SetBreakpointCheckMethod(Func<bool> i_checkBreakpoint)
        {
            CheckBreakpoint = i_checkBreakpoint;
        }
        public void SetCheckSecondCondition(Func<bool> i_checkSecondCondition)
        {
            CheckSecondCondition = i_checkSecondCondition;
        }

        public BreakpointInfo()
        {
            IsMulticonditional = false;
            IsConditionEquals = false;
            LeftCondition = string.Empty;
            LeftIsFlag = false;
            RightCondition = string.Empty;
        }
    }
    #endregion

    public partial class DebuggerManager
    {
        public static string[] Regs16Bit = new string[] { "AF", "BC", "DE", "HL", "IX", "IY", "SP", "IR", "PC", "AF'", "BC'", "DE'", "HL'" };
        public static char[]   Regs8Bit  = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'H', 'L' };

        public enum CharType { Number = 0, Letter, Other };

        //debugger commands list
        public static string DbgKeywordLD = "ld"; // memory/registers modification(=ld as in assembler)
        public static string DbgKeywordBREAK = "br"; // set breakpoint
        public static string DbgKeywordDissassemble = "ds"; // dasmPanel - goto adress(disassembly panel), (=disassembly)
        public static string DbgRemoveBreakpoint = "del"; // remove breakpoint, e.g.: del 1 - will delete breakpoint nr. 1
        public static string DbgEnableBreakpoint = "on"; // enables breakpoint
        public static string DbgDisableBreakpoint = "off"; // disables breakpoint
        public static string DbgLoadBreakpointsListFromFile = "loadbrs"; // loads breakpoints from file
        public static string DbgSaveBreakpointsListFromFile = "savebrs"; // save actual breakpoints list into file
        public static string DbgOpenAssembler = "asm"; // opens Assembler Form
        public static string DbgOpenGraphicsEditor = "ge"; // opens Graphics editor
        public static string DbgTraceLog = "trace"; // opens Graphics editor

        private static readonly char[] DebugDelimitersOther = new char[] { '(', '=', ')', '!' };

        // Main method - returns string list with items entered in debug command line, e.g. : 
        //
        // 0. item: br
        // 1. item: (PC)
        // 2. item: ==
        // 3. item: #9C40
        // 
        // Must be working: ld hl,  #4000; br (pc)==#4000; br (pc)   ==#af; br a<#FE; ld ( 16384  ), 255; ...
        public static List<string> ParseCommand(string dbgCommand)
        {
            try
            {
                var pattern = @"(\s+|,|==|!=|<|>)";
                var dbgParsed = new List<string>();
                dbgCommand = TrimCommand(dbgCommand);
                foreach (string result in Regex.Split(dbgCommand, pattern)) 
                {
                    if (!String.IsNullOrEmpty(result) && result.Trim() != "" && result != ",")
                        dbgParsed.Add(result);
                }
                return dbgParsed;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return null;
            }
        }

        private static string TrimCommand(string strIn)
        {
            var strOut = strIn;

            //trim all whitespaces after '(' until next character
            while (strOut.Contains("( "))
                strOut = strOut.Replace("( ", "(");
            //trim all whitespaces before ')'
            while (strOut.Contains(" )"))
                strOut = strOut.Replace(" )", ")");
            //if multiconditional breakpoint type then ensure that '&&' has space before and after
            if (strOut.Contains("&&") && !strOut.Contains(" && "))
                strOut = strOut.Replace("&&", " && ");

            return strOut;
        }

        /*public static void HasDigitsAndLettersInString(string s, ref bool hasLetters, ref bool hasDigits)
        {
            bool parsingDigits = false;

            foreach (char c in s)
            {
                if (Char.IsLetter(c) && !parsingDigits) //parsingDigits - do not consider [A-Fa-f] as letter in case we`re parsing number
                {
                    hasLetters = true;
                    parsingDigits = false;
                    continue;
                }

                if (Char.IsDigit(c) || c == '%' || c == '#' || c == '(' || c == ')')  // % - binary number, # - hex number; '(' and ')' are also digits
                {
                    hasDigits = true;
                    parsingDigits = true;
                    continue;
                }
            }
        }

        public static void HasOtherCharsInString(string s, char[] searchingChars, ref bool hasOtherChars)
        {
            for (byte listCounter = 0; listCounter < searchingChars.Length; listCounter++)
            {
                if (s.IndexOf(searchingChars[listCounter]) >= 0)
                {
                    hasOtherChars = true;
                    return;
                }
            }
        }

        public static CharType getCharType(char inputChar)
        {
            if (Char.IsDigit(inputChar) || inputChar == '%' || inputChar == '#') // % - binary number, # - hex number
                return CharType.Number;

            if (Char.IsLetter(inputChar))
                return CharType.Letter;

            if (DebugDelimitersOther.Contains<char>(inputChar))
                return CharType.Other;

            throw new CommandParseException("Incorrect character found: " + inputChar);
        }*/

        //Method will resolve whether command entered is memory modification or breakpoint setting
        public static DebuggerCommandType getDbgCommandType(List<string> command)
        {
            if (command[0].ToUpper() == DbgKeywordLD.ToString().ToUpper())
            {
                return DebuggerCommandType.memoryOrRegistryManipulation;
            }

            if (command[0].ToUpper() == DbgKeywordBREAK.ToString().ToUpper())
            {
                return DebuggerCommandType.breakpointManipulation;
            }

            if (command[0].ToUpper() == DbgKeywordDissassemble.ToString().ToUpper())
            {
                return DebuggerCommandType.gotoAdress;
            }

            if (command[0].ToUpper() == DbgEnableBreakpoint.ToString().ToUpper())
            {
                return DebuggerCommandType.enableBreakpoint;
            }

            if (command[0].ToUpper() == DbgDisableBreakpoint.ToString().ToUpper())
            {
                return DebuggerCommandType.disableBreakpoint;
            }

            if (command[0].ToUpper() == DbgRemoveBreakpoint.ToString().ToUpper())
            {
                return DebuggerCommandType.removeBreakpoint;
            }

            if (command[0].ToUpper() == DbgLoadBreakpointsListFromFile.ToString().ToUpper())
            {
                return DebuggerCommandType.loadBreakpointsListFromFile;
            }

            if (command[0].ToUpper() == DbgSaveBreakpointsListFromFile.ToString().ToUpper())
            {
                return DebuggerCommandType.saveBreakpointsListToFile;
            }

            if (command[0].ToUpper() == DbgOpenAssembler.ToString().ToUpper())
            {
                return DebuggerCommandType.showAssembler;
            }

            if (command[0].ToUpper() == DbgOpenGraphicsEditor.ToString().ToUpper())
            {
                return DebuggerCommandType.showGraphicsEditor;
            }

            if (command[0].ToUpper() == DbgTraceLog.ToString().ToUpper())
            {
                return DebuggerCommandType.traceLog;
            }

            return DebuggerCommandType.Unidentified;
        }

        public static bool isRegistry(string i_expr)
        {
            try
            {
                if (i_expr == null || i_expr.Length < 1)
                    return false;

                string registry = i_expr.ToUpper().Trim();
                if (Regs16Bit.ToArray().Contains<string>(registry))
                    return true;

                //now only low(8bit) registry are allowed, such as A, B, C, L, D, ...
                if (registry.Length > 1)
                    return false;

                if (Regs8Bit.Contains<char>(Convert.ToChar(registry)))
                    return true;

                /*for (byte counter = 0; counter < Regs16Bit.Length; counter++)
                {
                    if (Regs16Bit[counter] == registry)
                        return true;
                }

                //now only low(8bit) registry are allowed, such as A, B, C, L, D, ...
                if (registry.Length > 1)
                    return false;

                for (byte counter = 0; counter < Regs8Bit.Length; counter++)
                {
                    if ( Regs8Bit[counter].ToString() == registry)
                        return true;
                }*/
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return false;
        }

        public static bool isFlag(string i_expr)
        {
            //flags from command line will start with 'f' prefix, e.g.: fZ, fC, fPV, fS, ...
            if( i_expr == null || i_expr == String.Empty || i_expr.Length < 2 )
                return false;

            //F3 and F5 not supported - Z80 does not set/unset them basically
            return i_expr.Equals("fZ") || i_expr.Equals("fC") || i_expr.Equals("fPV") || i_expr.Equals("fH") || i_expr.Equals("fS") || i_expr.Equals("fN");
        }

        public static bool IsHex(char c)
        {
            return (new Regex("[A-Fa-f0-9]").IsMatch(c.ToString()));
        }

        public static bool isRegistryMemoryReference(string registryMemoryReference)
        {
            string registry = getRegistryFromReference(registryMemoryReference);
            if (isRegistry(registry))
                return true;

            return false;
        }

        public static string getRegistryFromReference( string registryMemoryRef )
        {
            if (registryMemoryRef.Length < 4 || !registryMemoryRef.StartsWith("(") || !registryMemoryRef.EndsWith(")")) // (PC), (DE), (hl), ...
                return String.Empty;

            return registryMemoryRef.Substring(1, registryMemoryRef.Length - 2);
        }

        public static bool isMemoryReference(string input)
        {
            if (input.StartsWith("(") && input.EndsWith(")"))
                return true;

            return false;
        }

        public static UInt16 getReferencedMemoryPointer(string input)
        {
            if (!isMemoryReference(input))
                throw new CommandParseException("Incorrect memory reference: " + input);

            return ConvertRadix.ConvertNumberWithPrefix(input.Substring(1, input.Length - 2));
        }

        public static BreakPointAccessType getBreakpointType( List<string> breakpoint )
        {
            try
            {
                var left  = breakpoint[1];
                var right = breakpoint[3];

                if (isMemoryReference(left) || isMemoryReference(right))
                {
                    return BreakPointAccessType.memoryChange;
                }
            }
            catch(Exception ex)
            {
                Logger.Error(ex);
            }
            return BreakPointAccessType.Undefined;
        }

        public static ushort getRegistryValueByName(CpuRegs regs, string registryName)
        {
            registryName = registryName.ToUpperInvariant();

            switch (registryName)
            {
                case "PC":
                    return regs.PC;
                case "IR":
                    return regs.IR;
                case "SP":
                    return regs.SP;
                case "AF":
                    return regs.AF;
                case "A":
                    return (ushort)(regs.AF >> 8);
                case "HL":
                    return regs.HL;
                case "DE":
                    return regs.DE;
                case "BC":
                    return regs.BC;
                case "IX":
                    return regs.IX;
                case "IY":
                    return regs.IY;
                case "AF'":
                    return regs._AF;
                case "HL'":
                    return regs._HL;
                case "DE'":
                    return regs._DE;
                case "BC'":
                    return regs._BC;
                case "MW (Memptr Word)":
                    return regs.MW;
                default:
                    throw new CommandParseException("Bad registry name: " + registryName);
            }
        }

        public static int getRegistryArrayIndex(string registry)
        {
            switch (registry.ToUpperInvariant())
            {
                case "AF":
                case "F":
                    return 0;
                case "A":
                case "BC":
                    return 1;
                case "C":
                case "DE":
                    return 2;
                case "B":
                case "HL":
                    return 3;
                case "E":
                    return 4;
                case "D":
                    return 5;
                case "L":
                case "IX":
                    return 6;
                case "H":
                case "IY":
                    return 7;
                case "SP":
                    return 12;
                case "PC":
                    return 11;
            }
               
            return -1;
        }
    }
}
