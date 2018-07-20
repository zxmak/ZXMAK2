using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using ZXMAK2.Dependency;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Hardware.Adlers.Views;
using ZXMAK2.Host.Interfaces;

namespace ZXMAK2.Hardware.Adlers.Core
{
    public class DebuggerTrace
    {
        #region members
        public static readonly byte[] ConditionalCalls = new byte[] {
                                                             //CALL`s
                                                             0xC4, //CALL NZ,nn
                                                             0xCC, //CALL Z,nn
                                                             0xD4, //CALL NC,nn
                                                             0xDC, //CALL C,nn
                                                             0xE4, //CALL PO,nn
                                                             0xEC, //CALL PE,nn
                                                             0xF4, //CALL P,nn
                                                             0xFC  //CALL M,nn*
                                                             };
        public static readonly byte[] ConditionalJumps = new byte[] {
                                                             //JR`s
                                                             0x20, //JR NZ,d
                                                             0x28, //JR Z,d
                                                             0x30, //JR NC,d
                                                             0x38, //JR C,d
                                                             //JP`s
                                                             0xC2, //JP NZ,nn
                                                             0xCA, //JP Z,nn
                                                             0xD2, //JP NC,nn
                                                             0xDA, //JP C,nn
                                                             0xE2, //JP PO,nn
                                                             0xEA, //JP PE,nn
                                                             0xF2, //JP P,nn
                                                             0xFA  //JP M,nn
                                                           };
        public static readonly byte[] CommonJumps = new byte[] { 
                                                             0x18, //JR d
                                                             0xC9, //RET
                                                             0xC3, //JP nn
                                                             0xCD, //CALL nn
                                                             0xE9, //JP (HL)
                                                           };

        public byte[] AllJumps;

        private IDebuggable _spectrum;

        private readonly object _sync = new object();

        private int[] _counters = null;

        private bool[] _addrsFlags; //false => address is excluded from tracing
        private byte[] _currentTraceOpcodes = null;

        //Trace filter
        private bool _isTraceFilterDefined = false;
        //Filters
        private bool _isTracingJumps = false;
        private bool _isTraceAreaDefined = false;
        private bool _isDetectingJumpOnAddress = false;
        //Opcode tracing
        private bool _isTracingOpcode = false;
        private byte _tracedOpcode;
        //Detecting jump/call to an address
        private ushort _detectingJumpToAddress;

        //Logging vars
        private string _traceLogFilename;
        private string _traceInfo;

        //Instruction history
        private ushort _prevPC;
        private bool _isPrevInstructionJumpOrCall;

        //Tacts
        private int _tactCount;
        #endregion

        public DebuggerTrace(IDebuggable i_spectrum)
        {
            _spectrum = i_spectrum;

            //make all jumps array(used for detecting jump to an address option)
            AllJumps = CommonJumps.Union(ConditionalJumps).ToArray();
            AllJumps = AllJumps.Union(ConditionalCalls).ToArray();

            _addrsFlags = new bool[65536];
            ResetAddrsFlags();
        }

        public bool StartTrace(FormCpu i_form)
        {
            lock (_sync)
            {
                _tactCount = -1;

                SetTraceOpcodes(i_form);
                SetTraceArea(i_form);
                //Detecting jump to an address
                _isDetectingJumpOnAddress = i_form.checkBoxDetectJumpOnAddress.Checked;

                if (!ValidateTrace(i_form))
                    return false;

                _counters = new int[65536]; //create and clear
                _traceLogFilename = i_form.textBoxTraceFileName.Text.Trim();

                MakeTraceInfo(i_form);

                return true;
            }
        }
        public void StopTrace()
        {
            lock (_sync)
            {
                _isTracingJumps = false;
                _isTraceAreaDefined = false;
                _isTracingOpcode = false;

                //previous instruction vars(used in detecting jump/call to an address
                _prevPC = (ushort)0;
                _isPrevInstructionJumpOrCall = false;

                if (!_isTraceFilterDefined) //without filtering each instruction is written
                    return;

                //save counters to file
                int[] countersOut = _counters.Select((s, index) => new { s, index })
                                             .Where(x => x.s > 0)
                                             .Select(x => x.index)
                                             .ToArray();
                int totalOccurences = 0;
                Array.Sort(countersOut);
                string traceCountersLog = String.Empty;
                foreach (int counterItem in countersOut)
                {
                    traceCountersLog += String.Format("Addr: #{0:X4}   Trace occurences: {1}\r\n", counterItem, _counters[counterItem]);
                    totalOccurences += _counters[counterItem];
                }

                string sumLine = String.Format("Total addresses: {0}   Total occurences: {1}", countersOut.Length, totalOccurences);
                //tact count
                if (_tactCount != -1)
                    sumLine += String.Format("   Tact count: {0}", _tactCount + 1);
                traceCountersLog += new String('=', sumLine.Length) + "\r\n";
                traceCountersLog += sumLine;

                traceCountersLog += "\r\n\r\n\n";

                //log filter info
                traceCountersLog += _traceInfo;

                File.WriteAllText(Path.Combine(Utils.GetAppFolder(), _traceLogFilename), traceCountersLog);

                _counters = null;
            }
        }

        public bool ValidateTrace(FormCpu i_form) //returns false when trace failed
        {
            //opcode filter
            if(i_form.checkBoxOpcode.Checked)
            {
                //bool error = false;
                if (i_form.textBoxOpcode.Text.Trim() == String.Empty)
                {
                    Locator.Resolve<IUserMessage>().Error("Filtering by opcode, but opcode not defined.");
                    i_form.textBoxOpcode.Focus();
                    return false;
                }
                if( ParseTracedOpcode(i_form, true ) == false )
                {
                    Locator.Resolve<IUserMessage>().Error("Opcode has incorrect number.");
                    i_form.textBoxOpcode.Focus();
                    return false;
                }
            }

            //detecting jump/call to an address
            if( i_form.checkBoxDetectJumpOnAddress.Checked )
            {
                if (i_form.checkBoxDetectJumpOnAddress.Text.Trim() == String.Empty)
                {
                    Locator.Resolve<IUserMessage>().Error("Detecting jump to an address, but\naddress not defined...");
                    i_form.checkBoxDetectJumpOnAddress.Focus();
                    return false;
                }
                if (ParseJumpCallToAnAddressValue(i_form,  false) == false)
                    return false;
            }

            //trace area - no valid address
            if (!_addrsFlags.Contains(true) && i_form.checkBoxTraceArea.Checked)
            {
                Locator.Resolve<IUserMessage>().Error("No valid trace address detected !\n\nHint: Uncheck adress range or define valid address to trace");
                return false;
            }
            //trace area - final check
            if ((!_isTraceAreaDefined && !_isTracingJumps && !_isTracingOpcode)
                &&
                (!_addrsFlags.Contains(true) && i_form.checkBoxTraceArea.Checked) //no valid addr to trace
               )
            {
                _isTraceFilterDefined = false;

                //Trace everything, are you sure ?
                var service = Locator.Resolve<IUserQuery>();
                if (service == null)
                    return true;

                if( service.Show( "Because no trace filter/s is defined the\nemulation will be very slow.\n\nConsole output is allowed only when\nno trace filter is defined.\n\nAre you sure?",
                                  "Trace: Performance warning",
                                  ZXMAK2.Host.Entities.DlgButtonSet.YesNo,
                                  ZXMAK2.Host.Entities.DlgIcon.Warning )
                               != ZXMAK2.Host.Entities.DlgResult.Yes )
                    return false;
            }
            else
                _isTraceFilterDefined = true;

            return true; //ok; if false => do not start tracing
        }

        private void SetTraceOpcodes(FormCpu i_form)
        {
            _currentTraceOpcodes = null;

            if (i_form.checkBoxAllJumps.Checked && i_form.checkBoxAllJumps.Enabled)
            {
                _currentTraceOpcodes = new byte[CommonJumps.Length + ConditionalJumps.Length];
                _currentTraceOpcodes = CommonJumps.Union(ConditionalJumps).ToArray();
                _currentTraceOpcodes = _currentTraceOpcodes.Union(ConditionalCalls).ToArray();

                _isTracingJumps = true;
            }
            else if (i_form.checkBoxConditionalJumps.Checked && i_form.checkBoxConditionalJumps.Enabled)
            {
                _currentTraceOpcodes = ConditionalJumps;
                _isTracingJumps = true;
            }
            else if (i_form.checkBoxConditionalCalls.Checked && i_form.checkBoxConditionalCalls.Enabled)
            {
                _currentTraceOpcodes = ConditionalCalls;
                _isTracingJumps = true;
            }
            else
                _isTracingJumps = false;

            if (i_form.checkBoxOpcode.Checked)
            {
                _isTracingOpcode = true;
            }
            else
                _isTracingOpcode = false;
        }
        private void SetTraceArea(FormCpu i_form)
        {
            if (i_form.listViewAdressRanges.Items.Count == 0 || i_form.checkBoxTraceArea.Checked == false)
            {
                _isTraceAreaDefined = false;
                return;
            }

            Array.Clear(_addrsFlags, 0, _addrsFlags.Length);

            foreach (ListViewItem item in i_form.listViewAdressRanges.Items)
            {
                string[] tags = ((string)item.Tag).Split(new char[] { ';' });
                if (tags.Length != 3)
                    continue;

                //setting
                int addrFrom = int.Parse(tags[0], System.Globalization.NumberStyles.HexNumber);
                int addrTo = int.Parse(tags[1], System.Globalization.NumberStyles.HexNumber);
                for (; addrFrom <= addrTo; addrFrom++)
                    _addrsFlags[addrFrom] = (tags[2] == "Yes");
            }

            _isTraceAreaDefined = true;
        }

        public ushort GetPrevInstructionAddress()
        {
            return _prevPC;
        }
        public void SavePrevInstruction(ushort i_memPointer)
        {
            _prevPC = i_memPointer;
        }

        public byte[] GetTraceOpcodes()
        {
            return _currentTraceOpcodes;
        }
        public bool[] GetAddressFlags()
        {
            return _addrsFlags;
        }
        public byte[] GetAllJumpsArr()
        {
            return AllJumps;
        }
        public bool IsTraceFilterDefined()
        {
            return _isTraceFilterDefined;
        }
        public bool IsTracingJumps()
        {
            return _isTracingJumps;
        }
        public bool IsDetectingJumpOnAddress()
        {
            return _isDetectingJumpOnAddress;
        }
        public bool IsTraceAreaDefined()
        {
            return _isTraceAreaDefined;
        }
        public bool IsTracingOpcode()
        {
            return _isTracingOpcode;
        }
        public byte GetTracedOpcode()
        {
            return _tracedOpcode;
        }
        public void SetTracedOpcode(byte i_opcode)
        {
            _tracedOpcode = i_opcode;
        }
        public bool GetIsPrevInstructionJumpOrCall()
        {
            return _isPrevInstructionJumpOrCall;
        }
        public void SetIsPrevInstructionJumpOrCall(bool i_isPrevInstructionJumpOrCall)
        {
            _isPrevInstructionJumpOrCall = i_isPrevInstructionJumpOrCall;
        }
        public ushort GetDetectingJumpToAddress()
        {
            return _detectingJumpToAddress;
        }
        public void IncCounter(int i_memPointer)
        {
            _counters[i_memPointer]++;
        }
        public void IncTactCount(int i_tact)
        {
            _tactCount += i_tact;
        }
        public bool ParseTracedOpcode(FormCpu i_form, bool i_justParse = false)
        {
            try
            {
                UInt16 opcode = ConvertRadix.ConvertNumberWithPrefix(i_form.textBoxOpcode.Text);
                if (opcode > 0xFF) //ToDo: only one byte for traced opcode
                    SetTracedOpcode((byte)(opcode % 256));
                else
                    SetTracedOpcode((byte)opcode);
            }
            catch (CommandParseException)
            {
                if (!i_justParse)
                {
                    Locator.Resolve<IUserMessage>().Error("Incorrect opcode number...");
                    i_form.textBoxOpcode.Focus();
                }
                return false;
            }

            return true;
        }
        public bool ParseJumpCallToAnAddressValue(FormCpu i_form, bool i_justParse = false)
        {
            try
            {
                UInt16 detectedJumpAddress = ConvertRadix.ConvertNumberWithPrefix(i_form.textBoxJumpToAnAddress.Text);
                if (detectedJumpAddress > 0xFFFF)
                {
                    Locator.Resolve<IUserMessage>().Error("Value for jump to an address is too big...\n\nMaximum is 2 bytes(0xFFFF).");
                    i_form.textBoxJumpToAnAddress.Focus();
                }
                else
                    _detectingJumpToAddress = detectedJumpAddress;
            }
            catch (CommandParseException)
            {
                if (!i_justParse)
                {
                    Locator.Resolve<IUserMessage>().Error("Incorrect number in field for jump to an address..");
                    i_form.textBoxJumpToAnAddress.Focus();
                }
                return false;
            }

            return true;
        }

        private void MakeTraceInfo(FormCpu i_form)
        {
            _traceInfo = "Trace filter:\r\n===========================\r\n";
            if (i_form.checkBoxAllJumps.Checked)
                _traceInfo += "- all jumps\r\n";
            if (i_form.checkBoxConditionalJumps.Checked)
                _traceInfo += "- conditional jumps\r\n";
            if (i_form.checkBoxConditionalCalls.Checked)
                _traceInfo += "- conditional calls\r\n";
            if (i_form.checkBoxDetectJumpOnAddress.Checked)
                _traceInfo += String.Format("- detecting jump/call to an address #{0:X4}\r\n", _detectingJumpToAddress);
            if (i_form.checkBoxOpcode.Checked)
                _traceInfo += String.Format("- tracing opcode #{0:X2}\r\n", _tracedOpcode);
            if (i_form.checkBoxTraceArea.Checked)
            {
                int[] tracedAreas = _addrsFlags.Select((s, index) => new { s, index })
                                               .Where(x => x.s == true)
                                               .Select(x => x.index)
                                               .ToArray();
                if (tracedAreas.Length > 0)
                {
                    if (tracedAreas.Length == _addrsFlags.Length)
                    {
                        _traceInfo += "- trace area: whole memory";
                    }
                    else
                    {
                        _traceInfo += "- trace area in: ";
                        int prevMemAddr = tracedAreas[0];
                        int actualStartAddr = tracedAreas[0];
                        string tracedAreaOut = String.Empty;
                        for( int memCounter = 1; memCounter < tracedAreas.Length; memCounter++ )
                        {
                            if (prevMemAddr != tracedAreas[memCounter] - 1 || memCounter+1 >= tracedAreas.Length)
                            {
                                if (tracedAreas[memCounter] - 1 != tracedAreas[memCounter - 1])
                                {
                                    tracedAreaOut += String.Format("#{0:X4}->#{1:X4}; ", actualStartAddr, tracedAreas[memCounter - 1]);
                                }
                                else
                                {
                                    tracedAreaOut += String.Format("#{0:X4}->#{1:X4}; ", actualStartAddr, tracedAreas[memCounter]);
                                }
                                actualStartAddr = prevMemAddr = tracedAreas[memCounter];
                                continue;
                            }
                            prevMemAddr = tracedAreas[memCounter];
                        }
                        _traceInfo += tracedAreaOut;
                    }
                    _traceInfo += "\r\n";
                }
                else
                    _traceInfo += "- trace area: no valid address => nothing will be traced"; //this should not happen
            }
                
        }

        #region GUI handlers/methods
        public void AddNewAddrArea(FormCpu i_form)
        {
            int FromAddr = 0;
            int ToAddr = 0;
            var service = Locator.Resolve<IUserQuery>();
            if (service == null)
            {
                return;
            }
            if (!service.QueryValue("Address area", "From:", "#{0:X4}", ref FromAddr, 0, 0xFFFF)) return;
            ToAddr = FromAddr;
            if (!service.QueryValue("Address area", "To:", "#{0:X4}", ref ToAddr, FromAddr, 0xFFFF)) return;

            ListViewItem item = new ListViewItem(new[] { String.Format("#{0:X4}", FromAddr), String.Format("#{0:X4}", ToAddr), "Yes" });
            item.Tag = String.Format("{0:X4};{1:X4};Yes", FromAddr, ToAddr);
            i_form.listViewAdressRanges.Items.Add(item);

            i_form.listViewAdressRanges.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
            i_form.listViewAdressRanges.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
            i_form.listViewAdressRanges.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        public void UpdateNewAddrArea(FormCpu i_form)
        {
            //this will only toggle Yes/No
            if (i_form.listViewAdressRanges.FocusedItem.Index < 0)
                return;

            ListViewItem itemToUpdate = i_form.listViewAdressRanges.Items[i_form.listViewAdressRanges.FocusedItem.Index];
            string strNewTraceStatus;
            string[] tags = ((string)itemToUpdate.Tag).Split(new char[] { ';' });
            if (tags.Length != 3)
                return;

            strNewTraceStatus = (tags[2] == "Yes" ? "No" : "Yes");
            ListViewItem item = new ListViewItem(new[] { "#" + tags[0], "#" + tags[1], strNewTraceStatus });
            item.Tag = tags[0] + ";" + tags[1] + ";" + strNewTraceStatus;
            i_form.listViewAdressRanges.Items[i_form.listViewAdressRanges.FocusedItem.Index] = item;
        }
        #endregion

        private void ResetAddrsFlags()
        {
            Array.Clear(_addrsFlags, 0, _addrsFlags.Length);
        }
    }
}
