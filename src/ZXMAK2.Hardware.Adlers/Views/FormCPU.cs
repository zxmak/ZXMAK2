/// Description: Debugger Adlers Window
/// Author: Adlers
using System;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Presentation.Interfaces;
using ZXMAK2.Host.WinForms.Views;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Cpu;
using ZXMAK2.Engine.Cpu.Tools;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Hardware.Adlers.Core;
using ZXMAK2.Engine;
using System.Xml;
using System.Text;
using System.Collections.Concurrent;
using ZXMAK2.Hardware.Adlers.Views.AssemblerView;
using ZXMAK2.Hardware.Adlers.Views.GraphicsEditorView;
using System.Runtime.InteropServices;
using System.Xml.Schema;

namespace ZXMAK2.Hardware.Adlers.Views
{
    public partial class FormCpu : FormView, IDebuggerAdlersView
    {
        private IDebuggable m_spectrum;
        private DasmTool m_dasmTool;
        private TimingTool m_timingTool;
        private bool m_isTracing;
        private CpuRegs m_cpuRegs;
        private DebuggerTrace m_debuggerTrace;

        private bool m_showStack = true; // show stack or breakpoint list on the form(panel listState)

        private readonly object m_sync = new object();

        //debugger command line history
        //private List<string> m_cmdLineHistory = new List<string>();
        //private int m_cmdLineHistoryPos = 0;
        private string savedCmdLineString = String.Empty;

        public FormCpu(IDebuggable debugTarget, IBusManager bmgr)
        {
            InitializeComponent();
            Init(debugTarget);
            bmgr.Events.SubscribeWrMem(0, 0, CheckWriteMem);
            bmgr.Events.SubscribeRdMem(0, 0, CheckReadMem);
            bmgr.Events.SubscribePreCycle(Bus_OnBeforeCpuCycle);
            m_cpuRegs = bmgr.CPU.regs;
        }

        private void Init(IDebuggable debugTarget)
        {
            if (debugTarget == m_spectrum)
                return;
            if (m_spectrum != null)
            {
                m_spectrum.UpdateState -= spectrum_OnUpdateState;
                m_spectrum.Breakpoint -= spectrum_OnBreakpoint;
            }
            if (debugTarget != null)
            {
                m_spectrum = debugTarget;
                m_dasmTool = new DasmTool(debugTarget.ReadMemory);
                m_timingTool = new TimingTool(m_spectrum.CPU, debugTarget.ReadMemory);
                m_spectrum.UpdateState += spectrum_OnUpdateState;
                m_spectrum.Breakpoint += spectrum_OnBreakpoint;
            }

            //some GUI init
            this.btnStartTrace.Image = global::ZXMAK2.Resources.ResourceImages.HardwareTapePlay;
            this.btnStopTrace.Image = global::ZXMAK2.Resources.ResourceImages.HardwareTapeRecord;

            m_debuggerTrace = new DebuggerTrace(m_spectrum);

            listViewAdressRanges.View = View.Details;
            listViewAdressRanges.Columns.Add("From", -2, HorizontalAlignment.Left);
            listViewAdressRanges.Columns.Add(" To ", -2, HorizontalAlignment.Left);
            listViewAdressRanges.Columns.Add("Trace", -2, HorizontalAlignment.Left );

            LoadConfig();
        }

        private void FormCPU_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_spectrum.UpdateState -= spectrum_OnUpdateState;
            m_spectrum.Breakpoint -= spectrum_OnBreakpoint;

            SaveConfig();
        }
        private void FormCpu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Owner != null)
                this.Owner.Focus();
        }

        private void FormCPU_Load(object sender, EventArgs e)
        {
            UpdateCPU(true);
        }

        private void FormCPU_Shown(object sender, EventArgs e)
        {
            Show();
            UpdateCPU(false);
            dasmPanel.Focus();
            //dbgCmdLine.Focus();
            Select();
        }

        private void FormCpu_Activated(object sender, EventArgs e)
        {
            if (!m_spectrum.IsRunning)
                dasmPanel.UpdateLines();
        }

        private void spectrum_OnUpdateState(object sender, EventArgs args)
        {
            if (!Created)
                return;
            BeginInvoke(new Action(()=>UpdateCPU(true)), null);
        }
        private void spectrum_OnBreakpoint(object sender, EventArgs args)
        {
            //LogAgent.Info("spectrum_OnBreakpoint {0}", sender);
            if (!Created)
                return;
            BeginInvoke(new Action(() =>
                {
                    Show();
                    UpdateCPU(true);
                    dasmPanel.Focus();
                    Select();
                }), null);
        }

        private void UpdateStack()
        {
            listState.Items.Clear();
            if (m_showStack) // toggle by F12 key
            {
                //set font - it is different for stack and breakpoint list(must be smaller due to big strings)
                FontFamily fontFamily = new FontFamily("Courier New");
                Font fontStack = new Font(
                   fontFamily,
                   12,
                   FontStyle.Regular,
                   GraphicsUnit.Pixel);
                listState.Font = fontStack;

                // show stack on listState panel
                int localStack = m_spectrum.CPU.regs.SP;
                byte counter = 0;
                do
                {
                    //the stack pointer can be set too low(SP=65535), e.g. Dizzy1
                    if (localStack + 1 > 0xFFFF)
                        break;

                    UInt16 stackAdressLo = m_spectrum.ReadMemory(Convert.ToUInt16(localStack++));
                    UInt16 stackAdressHi = m_spectrum.ReadMemory(Convert.ToUInt16(localStack++));

                    listState.Items.Add((localStack - 2).ToString("X4") + ":   " + (stackAdressLo + stackAdressHi * 256).ToString("X4"));

                    counter += 2;
                    if (counter >= 20)
                        break;

                } while (true);
            }
            else
            {
                //set font - it is different for stack and breakpoint list(must be smaller due to big strings)
                FontFamily fontFamily = new FontFamily("Arial");
                Font fontStack = new Font(
                   fontFamily,
                   10,
                   FontStyle.Regular,
                   GraphicsUnit.Pixel);
                listState.Font = fontStack;

                if (GetExtBreakpointsList().Count <= 0)
                {
                    listState.Items.Add("No breakpoints entered!");
                }
                else
                {
                    // show conditional breakpoints list on listState panel
                    foreach (KeyValuePair<byte, BreakpointAdlers> item in GetExtBreakpointsList())
                    {
                        string brDesc = String.Empty;

                        brDesc += item.Key.ToString() + ":";
                        if (!item.Value.Info.IsOn)
                            brDesc += "(off)";
                        else
                            brDesc += " ";
                        if (item.Value.Info.AccessType == BreakPointConditionType.memoryRead || item.Value.Info.AccessType == BreakPointConditionType.memoryReadInRange ||
                             item.Value.Info.AccessType == BreakPointConditionType.memoryWrite || item.Value.Info.AccessType == BreakPointConditionType.memoryWriteInRange
                           )
                        {
                            bool fMemWrite = item.Value.Info.AccessType == BreakPointConditionType.memoryWrite || item.Value.Info.AccessType == BreakPointConditionType.memoryWriteInRange;
                            //desc of memory read breakpoint type
                            if (fMemWrite)
                                brDesc += String.Format("mem write #{0:X2}", item.Value.Info.LeftValue);
                            else
                                brDesc += String.Format("mem read #{0:X2}", item.Value.Info.LeftValue);
                            //in range ?
                            if (item.Value.Info.AccessType == BreakPointConditionType.memoryReadInRange || item.Value.Info.AccessType == BreakPointConditionType.memoryWriteInRange)
                                brDesc += String.Format("-#{0:X2}", item.Value.Info.RightValue);
                        }
                        else
                        {
                            brDesc += item.Value.Info.BreakpointString;
                        }

                        listState.Items.Add(brDesc);
                    }
                }
            }
        }

        private void UpdateCPU(bool updatePC)
        {
            if (m_spectrum.IsRunning)
                updatePC = false;
            dasmPanel.ForeColor = m_spectrum.IsRunning ? SystemColors.ControlDarkDark : SystemColors.ControlText;
            UpdateREGS();
            UpdateDASM(updatePC);
            UpdateDATA();
        }

        private void UpdateREGS()
        {
            listREGS.Items.Clear();
            listREGS.Items.Add(" PC = " + m_spectrum.CPU.regs.PC.ToString("X4"));
            listREGS.Items.Add(" IR = " + m_spectrum.CPU.regs.IR.ToString("X4"));
            listREGS.Items.Add(" SP = " + m_spectrum.CPU.regs.SP.ToString("X4"));
            listREGS.Items.Add(" AF = " + m_spectrum.CPU.regs.AF.ToString("X4"));
            listREGS.Items.Add(" HL = " + m_spectrum.CPU.regs.HL.ToString("X4"));
            listREGS.Items.Add(" DE = " + m_spectrum.CPU.regs.DE.ToString("X4"));
            listREGS.Items.Add(" BC = " + m_spectrum.CPU.regs.BC.ToString("X4"));
            listREGS.Items.Add(" IX = " + m_spectrum.CPU.regs.IX.ToString("X4"));
            listREGS.Items.Add(" IY = " + m_spectrum.CPU.regs.IY.ToString("X4"));
            listREGS.Items.Add(" AF'= " + m_spectrum.CPU.regs._AF.ToString("X4"));
            listREGS.Items.Add(" HL'= " + m_spectrum.CPU.regs._HL.ToString("X4"));
            listREGS.Items.Add(" DE'= " + m_spectrum.CPU.regs._DE.ToString("X4"));
            listREGS.Items.Add(" BC'= " + m_spectrum.CPU.regs._BC.ToString("X4"));
            listREGS.Items.Add(" MW = " + m_spectrum.CPU.regs.MW.ToString("X4"));
            listF.Items.Clear();
            listF.Items.Add("  S = " + (((m_spectrum.CPU.regs.F & 0x80) != 0) ? "1" : "0"));
            listF.Items.Add("  Z = " + (((m_spectrum.CPU.regs.F & 0x40) != 0) ? "1" : "0"));
            listF.Items.Add(" F5 = " + (((m_spectrum.CPU.regs.F & 0x20) != 0) ? "1" : "0"));
            listF.Items.Add("  H = " + (((m_spectrum.CPU.regs.F & 0x10) != 0) ? "1" : "0"));
            listF.Items.Add(" F3 = " + (((m_spectrum.CPU.regs.F & 0x08) != 0) ? "1" : "0"));
            listF.Items.Add("P/V = " + (((m_spectrum.CPU.regs.F & 0x04) != 0) ? "1" : "0"));
            listF.Items.Add("  N = " + (((m_spectrum.CPU.regs.F & 0x02) != 0) ? "1" : "0"));
            listF.Items.Add("  C = " + (((m_spectrum.CPU.regs.F & 0x01) != 0) ? "1" : "0"));
            listF.Items.Add("========");
            listF.Items.Add("IFF1=" + (m_spectrum.CPU.IFF1 ? "1" : "0"));
            listF.Items.Add("IFF2=" + (m_spectrum.CPU.IFF2 ? "1" : "0"));
            listF.Items.Add("HALT=" + (m_spectrum.CPU.HALTED ? "1" : "0"));
            listF.Items.Add("BINT=" + (m_spectrum.CPU.BINT ? "1" : "0"));
            listF.Items.Add("  IM=" + m_spectrum.CPU.IM.ToString());
            listF.Items.Add("  FX=" + m_spectrum.CPU.FX.ToString());
            listF.Items.Add(" XFX=" + m_spectrum.CPU.XFX.ToString());

            /*listState.Items.Clear();
            listState.Items.Add("IFF1=" + (m_spectrum.CPU.IFF1 ? "1" : "0") + " IFF2=" + (m_spectrum.CPU.IFF2 ? "1" : "0"));
            listState.Items.Add("HALT=" + (m_spectrum.CPU.HALTED ? "1" : "0"));
            listState.Items.Add("BINT=" + (m_spectrum.CPU.BINT ? "1" : "0"));
            listState.Items.Add("  IM=" + m_spectrum.CPU.IM.ToString());
            listState.Items.Add("  FX=" + m_spectrum.CPU.FX.ToString());
            listState.Items.Add(" XFX=" + m_spectrum.CPU.XFX.ToString());*/

            UpdateStack();

            //Window text
            this.Text = "Z80 CPU(";
            this.Text += "Tact=" + m_spectrum.CPU.Tact.ToString() + " ";
            this.Text += "frmT=" + m_spectrum.GetFrameTact().ToString() + ")";

            /*listState.Items.Add("Tact=" + m_spectrum.CPU.Tact.ToString());
            listState.Items.Add("frmT=" + m_spectrum.GetFrameTact().ToString());*/
        }

        private void UpdateDASM(bool updatePC)
        {
            if (!m_spectrum.IsRunning && updatePC)
            {
                dasmPanel.ActiveAddress = m_spectrum.CPU.regs.PC;
                //UpdateREGS();
            }
            else
            {
                dasmPanel.UpdateLines();
                dasmPanel.Refresh();
            }
        }

        private void UpdateDATA()
        {
            dataPanel.UpdateLines();
            dataPanel.Refresh();
        }

        public int? GetOpcodeTiming(int i_addr)
        {
            return m_timingTool.GetTiming(i_addr);
        }

        private bool dasmPanel_CheckExecuting(object Sender, ushort ADDR)
        {
            if (m_spectrum.IsRunning) return false;
            if (ADDR == m_spectrum.CPU.regs.PC) return true;
            return false;
        }

        private void dasmPanel_GetDasm(object Sender, ushort ADDR, out string DASM, out int len)
        {
            var mnemonic = m_dasmTool.GetMnemonic(ADDR, out len);
            var timing = m_timingTool.GetTimingString(ADDR);

            DASM = string.Format("{0,-24} ; {1}", mnemonic, timing);
        }

        private void dasmPanel_GetData(object Sender, ushort ADDR, int len, out byte[] data)
        {
            data = new byte[len];
            for (int i = 0; i < len; i++)
                data[i] = m_spectrum.ReadMemory((ushort)(ADDR + i));
        }

        private bool dasmPanel_CheckBreakpoint(object sender, ushort addr)
        {
            foreach (Breakpoint bp in m_spectrum.GetBreakpointList())
                if (bp.Address.HasValue && bp.Address == addr)
                    return true;
            return false;
        }

        private void dasmPanel_SetBreakpoint(object sender, ushort addr)
        {
            bool found = false;
            foreach (Breakpoint bp in m_spectrum.GetBreakpointList())
            {
                if (bp.Address.HasValue && bp.Address == addr)
                {
                    m_spectrum.RemoveBreakpoint(bp);
                    found = true;
                }
            }
            if (!found)
            {
                Breakpoint bp = new Breakpoint(addr);
                m_spectrum.AddBreakpoint(bp);
            }
        }

        private void menuItemDasmGotoPC_Click(object sender, EventArgs e)
        {
            dasmPanel.ActiveAddress = m_spectrum.CPU.regs.PC;
            dasmPanel.UpdateLines();
            Refresh();
        }

        private void menuItemDumpMemoryAtCurrentAddress_Click(object sender, EventArgs e)
        {
            dataPanel.TopAddress = dasmPanel.ActiveAddress;
        }

        private void menuItemFollowInDisassembly_Click(object sender, EventArgs e)
        {
            dasmPanel.TopAddress = dataPanel.ActiveAddress;
        }

        private void menuItemDasmClearBP_Click(object sender, EventArgs e)
        {
            m_spectrum.ClearBreakpoints();
            UpdateCPU(false);
        }

        private void menuItemDasmRefresh_Click(object sender, EventArgs e)
        {
            dasmPanel.UpdateLines();
            Refresh();
        }

        private void listF_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listF.SelectedIndex < 0) return;
            if (m_spectrum.IsRunning) return;

            switch (listF.SelectedIndex)
            {
                case 9:     //iff
                    m_spectrum.CPU.IFF1 = m_spectrum.CPU.IFF2 = !m_spectrum.CPU.IFF1;
                    UpdateCPU(false);
                    return;
                case 11:     //halt
                    m_spectrum.CPU.HALTED = !m_spectrum.CPU.HALTED;
                    UpdateCPU(false);
                    return;
                case 13:     //im
                    m_spectrum.CPU.IM++;
                    if (m_spectrum.CPU.IM > 2)
                        m_spectrum.CPU.IM = 0;
                    UpdateCPU(false);
                    return;
                /*ToDo:
                case 16:     //frmT
                    int frameTact = m_spectrum.GetFrameTact();
                    if (InputBox.InputValue("Frame Tact", "New Frame Tact:", "", "D", ref frameTact, 0, m_spectrum.FrameTactCount))
                    {
                        int delta = frameTact - m_spectrum.GetFrameTact();
                        if (delta < 0)
                            delta += m_spectrum.FrameTactCount;
                        m_spectrum.CPU.Tact += delta;
                    }
                    UpdateCPU(false);
                    return;*/
            }

            m_spectrum.CPU.regs.F ^= (byte)(0x80 >> listF.SelectedIndex);
            UpdateREGS();
        }

        private void listREGS_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listREGS.SelectedIndex < 0) return;
            if (m_spectrum.IsRunning) return;
            ChangeRegByIndex(listREGS.SelectedIndex);
        }

        #region registry setters/getters by name/index
        private void ChangeRegByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    ChangeReg(ref m_spectrum.CPU.regs.PC, "PC");
                    break;
                case 1:
                    ChangeReg(ref m_spectrum.CPU.regs.IR, "IR");
                    break;
                case 2:
                    ChangeReg(ref m_spectrum.CPU.regs.SP, "SP");
                    break;
                case 3:
                    ChangeReg(ref m_spectrum.CPU.regs.AF, "AF");
                    break;
                case 4:
                    ChangeReg(ref m_spectrum.CPU.regs.HL, "HL");
                    break;
                case 5:
                    ChangeReg(ref m_spectrum.CPU.regs.DE, "DE");
                    break;
                case 6:
                    ChangeReg(ref m_spectrum.CPU.regs.BC, "BC");
                    break;
                case 7:
                    ChangeReg(ref m_spectrum.CPU.regs.IX, "IX");
                    break;
                case 8:
                    ChangeReg(ref m_spectrum.CPU.regs.IY, "IY");
                    break;
                case 9:
                    ChangeReg(ref m_spectrum.CPU.regs._AF, "AF'");
                    break;
                case 10:
                    ChangeReg(ref m_spectrum.CPU.regs._HL, "HL'");
                    break;
                case 11:
                    ChangeReg(ref m_spectrum.CPU.regs._DE, "DE'");
                    break;
                case 12:
                    ChangeReg(ref m_spectrum.CPU.regs._BC, "BC'");
                    break;
                case 13:
                    ChangeReg(ref m_spectrum.CPU.regs.MW, "MW (Memptr Word)");
                    break;
            }
        }

        private void ChangeReg(ref ushort p, string reg)
        {
            int val = p;
            var service = Locator.Resolve<IUserQuery>();
            if (!service.QueryValue("Change Register " + reg, "New value:", "#{0:X4}", ref val, 0, 0xFFFF)) return;
            p = (ushort)val;
            UpdateCPU(false);
        }

        private void ChangeRegByName(string i_registryName, ushort newRegistryValue)
        {
            string registryName = i_registryName.ToUpper();

            switch (registryName)
            {
                case "PC":
                    m_spectrum.CPU.regs.PC = newRegistryValue;
                    break;
                case "IR":
                    m_spectrum.CPU.regs.IR = newRegistryValue;
                    break;
                case "SP":
                    m_spectrum.CPU.regs.SP = newRegistryValue;
                    break;
                case "AF":
                    m_spectrum.CPU.regs.AF = newRegistryValue;
                    break;
                case "A":
                    m_spectrum.CPU.regs.AF = (ushort)((newRegistryValue * 256) + (m_spectrum.CPU.regs.AF & 0xFF));
                    break;
                case "HL":
                    m_spectrum.CPU.regs.HL = newRegistryValue;
                    break;
                case "DE":
                    m_spectrum.CPU.regs.DE = newRegistryValue;
                    break;
                case "BC":
                    m_spectrum.CPU.regs.BC = newRegistryValue;
                    break;
                case "IX":
                    m_spectrum.CPU.regs.IX = newRegistryValue;
                    break;
                case "IY":
                    m_spectrum.CPU.regs.IY = newRegistryValue;
                    break;
                case "AF'":
                    m_spectrum.CPU.regs._AF = newRegistryValue;
                    break;
                case "HL'":
                    m_spectrum.CPU.regs._HL = newRegistryValue;
                    break;
                case "DE'":
                    m_spectrum.CPU.regs._DE = newRegistryValue;
                    break;
                case "BC'":
                    m_spectrum.CPU.regs._BC = newRegistryValue;
                    break;
                case "MW (Memptr Word)":
                    m_spectrum.CPU.regs.MW = newRegistryValue;
                    break;
                default:
                    throw new Exception("Bad registry name: " + i_registryName);
            }
        }

        public IDebuggable GetVMKernel()
        {
            return m_spectrum;
        }
        #endregion

        private void contextMenuDasm_Popup(object sender, EventArgs e)
        {
            //if (m_spectrum.IsRunning) menuItemDasmClearBreakpoints.Enabled = false;
            //else menuItemDasmClearBreakpoints.Enabled = true;
        }

        // dbg funs
        private void dataPanel_DataClick(object Sender, ushort Addr)
        {
            int poked;
            poked = m_spectrum.ReadMemory((ushort)Addr);
            var service = Locator.Resolve<IUserQuery>();
            if (!service.QueryValue("POKE #" + Addr.ToString("X4"), "Value:", "#{0:X2}", ref poked, 0, 0xFF)) return;
            m_spectrum.WriteMemory((ushort)Addr, (byte)poked);
            UpdateCPU(false);
        }

        private void menuItemDataGotoADDR_Click(object sender, EventArgs e)
        {
            int adr = Historization.MemDumpGotoAddress;
            var service = Locator.Resolve<IUserQuery>();
            if (!service.QueryValue("Dump memory", "Address:", "#{0:X4}", ref adr, 0, 0xFFFF)) return;
            dataPanel.TopAddress = Historization.MemDumpGotoAddress = (ushort)adr;
            dataPanel.Focus();
        }

        private void menuItemDataRefresh_Click(object sender, EventArgs e)
        {
            dataPanel.UpdateLines();
            Refresh();
        }

        private void menuItemDataSetColumnCount_Click(object sender, EventArgs e)
        {
            int cols = dataPanel.ColCount;
            var service = Locator.Resolve<IUserQuery>();
            if (!service.QueryValue("Data Panel Columns", "Column Count:", "{0}", ref cols, 1, 32)) return;
            dataPanel.ColCount = cols;
        }

        //Save disassembly
        private void menuItemSaveDisassembly_Click(object sender, EventArgs e)
        {
            string dissassembly = String.Empty;
            int addressFrom = dasmPanel.ActiveAddress;
            int addressTo = 0;
            var service = Locator.Resolve<IUserQuery>();
            if (!service.QueryValue("Save disassembly", "Address from:", "#{0:X2}", ref addressFrom, 0, 0xFFFF)) return;
            if (!service.QueryValue("Save disassembly", "Address to:", "#{0:X2}", ref addressTo, 0, 0xFFFF)) return;

            for( int counter = 0; ; )
            {
                int actualAddress = addressFrom + counter;
                if( actualAddress > addressTo )
                    break;

                int len;
                dissassembly += m_dasmTool.GetMnemonic(actualAddress, out len) + System.Environment.NewLine;
                counter += len;
            }

            if (dissassembly != String.Empty)
            {
                File.WriteAllText(Path.Combine(Utils.GetAppFolder(), "dis.asm"), dissassembly);
                Locator.Resolve<IUserMessage>().Info("File dis.asm saved!");
            }
            else
            {
                Locator.Resolve<IUserMessage>().Error("Nothing to save...!");
            }
        }

        //Save memory block as bytes(DEFB)
        private void menuItemSaveAsBytes_Click(object sender, EventArgs e)
        {
            string memBytes = String.Empty;
            int addressFrom = dasmPanel.ActiveAddress;
            int addressTo = 0;
            var service = Locator.Resolve<IUserQuery>();
            if (!service.QueryValue("Save memory bytes(DEFB)", "Address from:", "#{0:X2}", ref addressFrom, 0, 0xFFFF)) return;
            if (!service.QueryValue("Save memory bytes(DEFB)", "Address to:", "#{0:X2}", ref addressTo, 0, 0xFFFF)) return;

            for (int counter = 0; ; )
            {
                int actualAddress = addressFrom + counter;
                if (actualAddress >= addressTo)
                    break;

                if (counter % 8 == 0 || counter == 0)
                {
                    memBytes += "DEFB ";
                }

                memBytes += String.Format("#{0:X2}", m_spectrum.ReadMemory((ushort)actualAddress));

                if ((counter + 1) % 8 != 0 || counter == 0)
                {
                    if (actualAddress+1 < addressTo)
                        memBytes += ", ";
                }
                else
                    memBytes += System.Environment.NewLine;

                counter++;
            }

            if (memBytes != String.Empty)
            {
                File.WriteAllText("membytes.asm", memBytes);
                Locator.Resolve<IUserMessage>().Info("File membytes.asm saved!");
            }
            else
            {
                Locator.Resolve<IUserMessage>().Error("Nothing to save...!");
            }
        }

        //find bytes in memory
        private void menuItemFindBytes_Click(object sender, EventArgs e)
        {
            List<UInt16> bytesToFindInput = new List<UInt16>();
            var service = Locator.Resolve<IUserQuery>();

            string strBytesToFind = Historization.FindBytesInMemory;

            if (sender != null) //null => Find next
            {
                if (!service.QueryText("Find bytes in memory", "Bytes(comma delimited):", ref strBytesToFind))
                    return;
            }
            if (strBytesToFind.Trim() == String.Empty || strBytesToFind.Trim().Length == 0)
                return;
            else
                Historization.FindBytesInMemory = strBytesToFind;

            bytesToFindInput.Clear();
            foreach (string byteCandidate in Regex.Split(strBytesToFind, ","))
            {
                if (!String.IsNullOrEmpty(byteCandidate) && byteCandidate.Trim() != String.Empty && byteCandidate != ",")
                {
                    try
                    {
                        bytesToFindInput.Add(ConvertRadix.ConvertNumberWithPrefix(byteCandidate));
                    }
                    catch
                    {
                        Locator.Resolve<IUserMessage>().Error("Error in parsing the entered values!");
                    }
                }
            }

            if (bytesToFindInput.Count == 0)
                return;

            //finding the memory
            List<byte> bytesToFind = new List<byte>();
            bytesToFind.Clear();
            foreach( UInt16 word in bytesToFindInput )
            {
                if (word > 0xFFFF)
                {
                    Locator.Resolve<IUserMessage>().Error("Input value " + word.ToString() + " exceeded.");
                    return;
                }

                if (word > 0xFF)
                {
                    bytesToFind.Add((byte)(word / 256));
                    bytesToFind.Add((byte)(word % 256));
                }
                else
                    bytesToFind.Add((byte)word);
            }

            //search from actual address(dataPanel.TopAddress) until memory top(0xFFFF)
            for (ushort counter = (ushort)(dataPanel.TopAddress + 1); counter != 0; counter++)
            {
                if (m_spectrum.ReadMemory(counter) == bytesToFind[0]) //check 1. byte
                {
                    //check next bytes
                    bool bFound = true;
                    ushort actualAdress = (ushort)(counter + 1);

                    for (ushort counterNextBytes = 1; counterNextBytes < bytesToFind.Count; counterNextBytes++, actualAdress++)
                    {
                        if (m_spectrum.ReadMemory(actualAdress) != bytesToFind[counterNextBytes])
                        {
                            bFound = false;
                            break;
                        }
                    }

                    if (bFound)
                    {
                        dataPanel.TopAddress = counter;
                        return;
                    }
                }
            }

            //search from address 0 until actual address(dataPanel.TopAddress)
            for (ushort counter = 0; counter < (ushort)(dataPanel.TopAddress + 1); counter++)
            {
                if (m_spectrum.ReadMemory(counter) == bytesToFind[0]) //check 1. byte
                {
                    //check next bytes
                    bool bFound = true;
                    ushort actualAdress = (ushort)(counter + 1);

                    for (ushort counterNextBytes = 1; counterNextBytes < bytesToFind.Count; counterNextBytes++, actualAdress++)
                    {
                        if (m_spectrum.ReadMemory(actualAdress) != bytesToFind[counterNextBytes])
                        {
                            bFound = false;
                            break;
                        }
                    }

                    if (bFound)
                    {
                        dataPanel.TopAddress = counter;
                        return;
                    }
                }
            }
        }

        private void menuItemFindBytesNext_Click(object sender, EventArgs e)
        {
            menuItemFindBytes_Click(null, null);
        }
        private void menuItemNewAdress_Clicked(object sender, EventArgs e)
        {
            if (sender is MenuItem) //must be a MenuItem
            {
                string hex = ((MenuItem)sender).Text;
                dataPanel.TopAddress = ushort.Parse(hex.Substring(1, hex.Length - 1), System.Globalization.NumberStyles.HexNumber);
            }
        }
        private void menuItemNewAdressFollowInDisassembly_Clicked(object sender, EventArgs e)
        {
            if (sender is MenuItem) //must be a MenuItem
            {
                string hex = ((MenuItem)sender).Text;
                dasmPanel.AddAddrToDisassemblyHistory(dasmPanel.TopAddress);
                dasmPanel.TopAddress = ushort.Parse(hex.Substring(1, hex.Length - 1), System.Globalization.NumberStyles.HexNumber);
                dasmPanel.AddAddrToDisassemblyHistory(dasmPanel.TopAddress);
            }
        }

        private void dasmPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.X > dasmPanel.GetGutterWidth() && e.Button == MouseButtons.Left)
            {
                //fill command line with current doubleclicked address on disassembly panel
                dbgCmdLine.Text = dbgCmdLine.Text.Insert(dbgCmdLine.SelectionStart, String.Format("#{0:X4}", dasmPanel.ActiveAddress));
                dbgCmdLine.Focus();
            }
        }

        private void dataPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuData.Show(dataPanel, e.Location);
        }

        private void listState_DoubleClick(object sender, EventArgs e)
        {
            if (!m_showStack) // if we are in breakpoint mode only
            {
                int selectedIndex = listState.SelectedIndex;
                if (selectedIndex < 0 || GetExtBreakpointsList().Count == 0) return;
                if (selectedIndex + 1 > GetExtBreakpointsList().Count) return;

                string strTemp = listState.Items[selectedIndex].ToString();
                int index = strTemp.IndexOf(':');
                string key = String.Empty;
                if (index > 0)
                    key = strTemp.Substring(0, index);

                bool isBreakpointIsOn = GetExtBreakpointsList()[Convert.ToByte(key)].Info.IsOn;
                EnableOrDisableBreakpointStatus(Convert.ToByte(key), !isBreakpointIsOn);
                UpdateREGS();
            }
            /*if (listState.SelectedIndex < 0) return;
            if (m_spectrum.IsRunning)
                return;
            switch (listState.SelectedIndex)
            {
                case 0:     //iff
                    m_spectrum.CPU.IFF1 = m_spectrum.CPU.IFF2 = !m_spectrum.CPU.IFF1;
                    break;
                case 1:     //halt
                    m_spectrum.CPU.HALTED = !m_spectrum.CPU.HALTED;
                    break;
                case 3:     //im
                    m_spectrum.CPU.IM++;
                    if (m_spectrum.CPU.IM > 2)
                        m_spectrum.CPU.IM = 0;
                    break;
                case 7:     //frmT
                    int frameTact = m_spectrum.GetFrameTact();
                    if (InputBox.InputValue("Frame Tact", "New Frame Tact:", "", "D", ref frameTact, 0, m_spectrum.FrameTactCount))
                    {
                        int delta = frameTact - m_spectrum.GetFrameTact();
                        if(delta < 0)
                            delta += m_spectrum.FrameTactCount;
                        m_spectrum.CPU.Tact += delta;
                    }
                    break;
            }
            UpdateCPU(false);*/
        }

        private static int s_addr = 0x4000;
        private static int s_len = 6912;

        private void menuLoadBlock_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog loadDialog = new OpenFileDialog())
            {
                loadDialog.InitialDirectory = ".";
                loadDialog.SupportMultiDottedExtensions = true;
                loadDialog.Title = "Load Block...";
                loadDialog.Filter = "All files (*.*)|*.*";
                loadDialog.DefaultExt = "";
                loadDialog.FileName = "";
                loadDialog.ShowReadOnly = false;
                loadDialog.CheckFileExists = true;
                if (loadDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;

                FileInfo fileInfo = new FileInfo(loadDialog.FileName);
                s_len = (int)fileInfo.Length;

                if (s_len < 1)
                    return;
                var service = Locator.Resolve<IUserQuery>();
                if (service == null)
                {
                    return;
                }
                s_addr = dasmPanel.ActiveAddress;
                if (!service.QueryValue("Load Block", "Memory Address:", "#{0:X4}", ref s_addr, 0, 0xFFFF))
                    return;
                if (!service.QueryValue("Load Block", "Block Length:", "#{0:X4}", ref s_len, 0, 0x10000))
                    return;

                byte[] data = new byte[s_len];
                using (FileStream fs = new FileStream(loadDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    fs.Read(data, 0, data.Length);
                m_spectrum.WriteMemory((ushort)s_addr, data, 0, s_len);
            }
        }

        private void menuSaveBlock_Click(object sender, EventArgs e)
        {
            var service = Locator.Resolve<IUserQuery>();
            s_addr = dasmPanel.ActiveAddress;
            if (!service.QueryValue("Save Block", "Memory Address:", "#{0:X4}", ref s_addr, 0, 0xFFFF))
                return;
            if (!service.QueryValue("Save Block", "Block Length:", "#{0:X4}", ref s_len, 0, 0x10000))
                return;

            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.InitialDirectory = ".";
                saveDialog.SupportMultiDottedExtensions = true;
                saveDialog.Title = "Save Block...";
                saveDialog.Filter = "Binary Files (*.bin)|*.bin|All files (*.*)|*.*";
                saveDialog.DefaultExt = "";
                saveDialog.FileName = "";
                saveDialog.OverwritePrompt = true;
                if (saveDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;

                byte[] data = new byte[s_len];
                m_spectrum.ReadMemory((ushort)s_addr, data, 0, s_len);

                using (FileStream fs = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                    fs.Write(data, 0, data.Length);
            }
        }

        private void Bus_OnBeforeCpuCycle()
        {
            lock(m_sync)
            {
                if (!m_isTracing /*|| m_cpuRegs.PC < 0x4000*/) //no need to trace ROM
                    return;

                //Trace area
                if (m_debuggerTrace.IsTraceAreaDefined())
                {
                    if (!m_debuggerTrace.GetAddressFlags()[m_cpuRegs.PC])
                        return;
                }
                //Jumps/Calls
                if (m_debuggerTrace.IsTracingJumps())
                {
                    if (Array.Exists(m_debuggerTrace.GetTraceOpcodes(), p => p == m_spectrum.ReadMemory(m_cpuRegs.PC)))
                    {
                        m_debuggerTrace.IncCounter(m_cpuRegs.PC);
                        if (checkBoxShowConsole.Checked)
                        {
                            var len = 0;
                            var mnemonic = m_dasmTool.GetMnemonic(m_cpuRegs.PC, out len);
                            Logger.Debug("#{0:X4}   {1}", m_cpuRegs.PC, mnemonic);
                        }
                        return;
                    }
                }
                //Opcode
                if (m_debuggerTrace.IsTracingOpcode())
                {
                    if (m_spectrum.ReadMemory(m_cpuRegs.PC) == m_debuggerTrace.GetTracedOpcode())
                    {
                        m_debuggerTrace.IncCounter(m_cpuRegs.PC);
                        if (checkBoxShowConsole.Checked)
                        {
                            var len = 0;
                            var mnemonic = m_dasmTool.GetMnemonic(m_cpuRegs.PC, out len);
                            Logger.Debug("#{0:X4}   {1}", m_cpuRegs.PC, mnemonic);
                        }
                        return;
                    }
                }
                //Detecting jump to an address
                if( m_debuggerTrace.IsDetectingJumpOnAddress() )
                {
                    if( m_debuggerTrace.GetIsPrevInstructionJumpOrCall() )
                    {
                        if (m_debuggerTrace.GetIsPrevInstructionJumpOrCall() && m_debuggerTrace.GetDetectingJumpToAddress() == m_cpuRegs.PC)
                        {
                            m_debuggerTrace.IncCounter(m_debuggerTrace.GetPrevInstructionAddress());
                            if (checkBoxShowConsole.Checked)
                            {
                                var len = 0;
                                var mnemonic = m_dasmTool.GetMnemonic(m_cpuRegs.PC, out len);
                                Logger.Debug("#{0:X4}   {1}  ; detected jump to #{2:X4}", m_cpuRegs.PC, mnemonic, m_debuggerTrace.GetPrevInstructionAddress());
                            }
                            if (m_debuggerTrace.GetAllJumpsArr().Contains(m_spectrum.ReadMemory(m_cpuRegs.PC)))
                            {
                                m_debuggerTrace.SavePrevInstruction(m_cpuRegs.PC);
                                m_debuggerTrace.SetIsPrevInstructionJumpOrCall(true);
                            }
                            else
                                m_debuggerTrace.SetIsPrevInstructionJumpOrCall(false);
                            return;
                        }
                    }

                    if (m_debuggerTrace.GetAllJumpsArr().Contains(m_spectrum.ReadMemory(m_cpuRegs.PC)))
                    {
                        m_debuggerTrace.SavePrevInstruction(m_cpuRegs.PC);
                        m_debuggerTrace.SetIsPrevInstructionJumpOrCall(true);
                    }
                    else
                        m_debuggerTrace.SetIsPrevInstructionJumpOrCall(false);
                }

                //if Opcode or Jump/Calls criteria selected => return
                if (m_debuggerTrace.IsTracingOpcode() || m_debuggerTrace.IsTracingJumps() || m_debuggerTrace.IsDetectingJumpOnAddress())
                    return;

                //Trace everything
                if (!m_debuggerTrace.IsTraceFilterDefined())
                {
                    //if (checkBoxShowConsole.Checked)
                    {
                        var len = 0;
                        var mnemonic = m_dasmTool.GetMnemonic(m_cpuRegs.PC, out len);
                        Logger.Debug("#{0:X4}   {1}", m_cpuRegs.PC, mnemonic);
                    }
                }
                else
                {
                    //trace filter is defined, but does not contain any Jumps/Calls, Opcode, ... criteria
                    m_debuggerTrace.IncCounter(m_cpuRegs.PC);
                }

                //Tacts
                if (checkBoxTactCount.Checked)
                {
                    int? tactCount = GetOpcodeTiming(m_cpuRegs.PC);
                    if (tactCount!=null)
                        m_debuggerTrace.IncTactCount((int)tactCount);
                }
            }
        }

        private void ClearCommandLineErrorText()
        {
            //display debugger command line content before error occurred
            dbgCmdLine.BackColor = Color.White;
            dbgCmdLine.ForeColor = Color.Black;
            dbgCmdLine.Text = savedCmdLineString;
            savedCmdLineString = String.Empty;
        }

        private void dbgCmdLine_KeyUp(object sender, KeyEventArgs e)
        {
            if (savedCmdLineString != String.Empty)
            {
                ClearCommandLineErrorText();
                return;
            }

            //Debug command entered ?
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    string actualCommand = dbgCmdLine.Text;

                    List<string> parsedCommand = DebuggerManager.ParseCommand(actualCommand);
                    if (parsedCommand == null || parsedCommand.Count == 0)
                        return;

                    DebuggerCommandType commandType = DebuggerManager.getDbgCommandType(parsedCommand);

                    if (commandType == DebuggerCommandType.Unidentified)
                        throw new Exception("unknown debugger command"); // unknown cmd line type

                    //breakpoint manipulation ?
                    if (commandType == DebuggerCommandType.breakpointManipulation)
                    {
                        // add new enhanced breakpoint
                        string left = parsedCommand[1];

                        //left side must be registry or memory reference
                        if (   !DebuggerManager.isRegistry(left)
                            && !DebuggerManager.isFlag(left)
                            && !DebuggerManager.isMemoryReference(left)
                            && left != "memread"
                            && left != "memwrite"
                           )
                            throw new CommandParseException("bad condition !");

                        AddExtBreakpoint(parsedCommand); // add breakpoint, send parsed command e.g.: br pc == #0000

                        m_showStack = false; // show breakpoint list on listState panel
                    }
                    else if (commandType == DebuggerCommandType.gotoAdress)
                    {
                        // goto adress to dissasembly
                        dasmPanel.TopAddress = ConvertRadix.ConvertNumberWithPrefix(parsedCommand[1]);
                    }
                    else if (commandType == DebuggerCommandType.removeBreakpoint)
                    {
                        // remove breakpoint
                        if (parsedCommand.Count > 1)
                            RemoveExtBreakpoint(parsedCommand[1]);
                    }
                    else if (commandType == DebuggerCommandType.enableBreakpoint)
                    {
                        //enable breakpoint
                        EnableOrDisableBreakpointStatus(Convert.ToByte(ConvertRadix.ConvertNumberWithPrefix(parsedCommand[1])), true);
                    }
                    else if (commandType == DebuggerCommandType.disableBreakpoint)
                    {
                        //disable breakpoint
                        EnableOrDisableBreakpointStatus(Convert.ToByte(ConvertRadix.ConvertNumberWithPrefix(parsedCommand[1])), false);
                    }
                    else if (commandType == DebuggerCommandType.loadBreakpointsListFromFile)
                    {
                        //load breakpoints list into debugger
                        LoadBreakpointsListFromFile(parsedCommand[1]);

                        m_showStack = false;
                    }
                    else if (commandType == DebuggerCommandType.saveBreakpointsListToFile)
                    {
                        //save breakpoints list into debugger
                        SaveBreakpointsListToFile(parsedCommand[1]);
                    }
                    else if (commandType == DebuggerCommandType.showAssembler)
                    {
                        m_spectrum.DoStop();
                        UpdateCPU(true);

                        Assembler.Show(this);
                        Assembler.ActiveForm.Focus();

                        return;
                    }
                    else if (commandType == DebuggerCommandType.showGraphicsEditor)
                    {
                        m_spectrum.DoStop();
                        UpdateCPU(true);

                        GraphicsEditor.Show(ref m_spectrum);
                        GraphicsEditor.ActiveForm.Focus();

                        return;
                    }
                    else if (commandType == DebuggerCommandType.traceLog)
                    {
                        if (parsedCommand.Count < 2)
                            throw new Exception("Incorrect trace command syntax! Missing On or Off.");

                        if (parsedCommand[1].ToUpper() == "ON")
                        {
                            m_isTracing = true;
                            //Logger.Start();
                        }
                        else
                        {
                            m_isTracing = false;
                            //Logger.Finish();
                        }
                    }
                    else if (commandType == DebuggerCommandType.memoryOrRegistryManipulation)
                    {
                        if (parsedCommand.Count < 3)
                            throw new CommandParseException("Incorrect left or right expression.");

                        // memory/registry manipulation(LD instruction)
                        string left = parsedCommand[1];
                        UInt16 leftNum = 0;
                        bool isLeftMemoryReference = false;
                        bool isLeftRegistry = false;

                        string right = parsedCommand[2];
                        UInt16 rightNum = 0;
                        bool isRightMemoryReference = false;
                        bool isRightRegistry = false;

                        //Reading values - left side of statement
                        if (DebuggerManager.isMemoryReference(left))
                        {
                            leftNum = DebuggerManager.getReferencedMemoryPointer(left);

                            isLeftMemoryReference = true;
                        }
                        else
                        {
                            // is it register ?
                            if (DebuggerManager.isRegistry(left))
                            {
                                //leftNum = DebuggerManager.getRegistryValueByName(m_spectrum.CPU.regs, left);
                                isLeftRegistry = true;
                            }
                            else
                                leftNum = ConvertRadix.ConvertNumberWithPrefix(left);
                        }

                        //Reading values - right side of statement
                        if (DebuggerManager.isMemoryReference(right))
                        {
                            rightNum = DebuggerManager.getReferencedMemoryPointer(right);
                            isRightMemoryReference = true;
                        }
                        else
                        {
                            // is it register ?
                            if (DebuggerManager.isRegistry(right))
                            {
                                rightNum = DebuggerManager.getRegistryValueByName(m_spectrum.CPU.regs, right);
                                isRightRegistry = true;
                            }
                            else if( right[0] != '\"' ) //text will be placed into memory, will parsed later
                            {
                                //must be a value
                                rightNum = ConvertRadix.ConvertNumberWithPrefix(right);
                                isRightRegistry = false;
                            }
                        }

                        //Writing Memory/Registry
                        if (isLeftMemoryReference && isRightMemoryReference)
                        {
                            // memcpy e.g.: ld (#4000), (#3000)
                            m_spectrum.WriteMemory(leftNum, m_spectrum.ReadMemory(rightNum));
                            m_spectrum.WriteMemory((ushort)(leftNum + 1), m_spectrum.ReadMemory((ushort)(rightNum + 1)));
                        }
                        else if (isLeftMemoryReference)
                        {
                            // write registry or memory ?
                            if (isRightRegistry)
                            {
                                // e.g.: ld (#9C40), hl
                                UInt16 regValue = DebuggerManager.getRegistryValueByName(m_spectrum.CPU.regs, right);
                                if (regValue <= Byte.MaxValue)
                                    m_spectrum.WriteMemory(leftNum, Convert.ToByte(regValue));
                                else
                                {
                                    //2 bytes will be written; ToDo: check on adress if it is not > 65535
                                    byte hiBits = Convert.ToByte(regValue / 256);
                                    byte loBits = Convert.ToByte(regValue - hiBits * 256);

                                    m_spectrum.WriteMemory(leftNum, loBits);
                                    leftNum++;
                                    m_spectrum.WriteMemory(leftNum, hiBits);
                                }
                            }
                            else
                            {
                                //write text or bytes to memory with "ld" directive
                                bool isWritingCharacters = false;
                                for (int counter = 2; parsedCommand.Count > counter; counter++)
                                {
                                    string currExpr = parsedCommand[counter];
                                    if (currExpr[0] == '"')
                                        isWritingCharacters = true;
                                    if (isWritingCharacters)
                                    {
                                        // e.g.: ld (<memAddr>), "something to write to mem"
                                        byte[] arrToWriteToMem = ConvertRadix.GetBytesInStringBetweenCharacter(actualCommand, '\"');
                                        m_spectrum.WriteMemory(leftNum, arrToWriteToMem, 0, arrToWriteToMem.Length);
                                        break;
                                    }
                                    else
                                    {
                                        // e.g.: ld (#9C40), #21 #33 3344 .. .. .. -> x
                                        rightNum = ConvertRadix.ConvertNumberWithPrefix(currExpr);
                                        if (rightNum <= Byte.MaxValue)
                                        {
                                            m_spectrum.WriteMemory(leftNum, Convert.ToByte(rightNum));
                                            leftNum++;
                                        }
                                        else
                                        {
                                            //2 bytes will be written; ToDo: check on adress if it is not > 65535
                                            byte hiBits = Convert.ToByte(rightNum / 256);
                                            byte loBits = Convert.ToByte(rightNum % 256);

                                            m_spectrum.WriteMemory(leftNum, hiBits);
                                            leftNum++;
                                            m_spectrum.WriteMemory(leftNum, loBits);
                                            leftNum++;
                                        }
                                    }
                                }
                            }
                        }
                        else if (isRightMemoryReference)
                        {
                            // e.g.: ld hl, (#9C40)
                            if (isLeftRegistry)
                            {
                                byte LByte = m_spectrum.ReadMemory(rightNum);
                                byte HByte = m_spectrum.ReadMemory((ushort)(rightNum + 1));

                                ChangeRegByName(left, (ushort)(HByte * 256 + LByte));
                            }
                            else
                            {
                                m_spectrum.WriteMemory(m_spectrum.ReadMemory(leftNum), Convert.ToByte(rightNum));
                            }
                        }
                        else
                        {
                            // no, so registry change
                            ChangeRegByName(left, rightNum);
                        }
                    }

                    //command line history
                    /*m_cmdLineHistory.Add(actualCommand);
                    this.m_cmdLineHistoryPos++;*/
                    this.dbgCmdLine.AutoCompleteCustomSource.Add(actualCommand);

                    UpdateREGS();
                    UpdateCPU(false);

                    dbgCmdLine.SelectAll();
                    dbgCmdLine.Focus();
                }
                catch (Exception exc)
                {
                    //Logger.Error(exc);
                    savedCmdLineString = dbgCmdLine.Text;
                    dbgCmdLine.BackColor = Color.Red;
                    dbgCmdLine.ForeColor = Color.Black;
                    dbgCmdLine.Text = exc.Message;
                    dbgCmdLine.Refresh();
                    //System.Threading.Thread.Sleep(140);
                }
            }
            /*else if (e.KeyCode == Keys.Up && this.m_cmdLineHistory.Count != 0) //arrow up - history of command line
            {
                if (this.m_cmdLineHistoryPos < (this.m_cmdLineHistory.Count - 1))
                {
                    this.dbgCmdLine.Text = this.m_cmdLineHistory[++m_cmdLineHistoryPos];
                }
                else
                {
                    this.m_cmdLineHistoryPos = 0;
                    this.dbgCmdLine.Text = this.m_cmdLineHistory[this.m_cmdLineHistoryPos];
                }
                dbgCmdLine.Select(this.dbgCmdLine.Text.Length, 0);
                dbgCmdLine.Focus();
                e.Handled = true;
                return;
            }
            else if (e.KeyCode == Keys.Down && this.m_cmdLineHistory.Count != 0) //arrow down - history of command line
            {
                if (this.m_cmdLineHistoryPos != 0)
                {
                    this.dbgCmdLine.Text = this.m_cmdLineHistory[--m_cmdLineHistoryPos];
                }
                else
                {
                    this.m_cmdLineHistoryPos = this.m_cmdLineHistory.Count - 1;
                    this.dbgCmdLine.Text = this.m_cmdLineHistory[this.m_cmdLineHistoryPos];
                }
                dbgCmdLine.Select(this.dbgCmdLine.Text.Length, 0);
                dbgCmdLine.Focus();
                e.Handled = true;
                return;
            }*/
            else if (this.dbgCmdLine.Text == "lo") //shortcut
            {
                this.dbgCmdLine.Text = "loadbrs ";
                dbgCmdLine.Select(8, 0);
                dbgCmdLine.Focus();
                return;
            }
            else if (this.dbgCmdLine.Text == "sa") //shortcut
            {
                this.dbgCmdLine.Text = "savebrs ";
                dbgCmdLine.Select(8, 0);
                dbgCmdLine.Focus();
                return;
            }
            else if (this.dbgCmdLine.Text == "a") //shortcut - asm
            {
                this.dbgCmdLine.Text = "asm";
                dbgCmdLine.Select(3, 0);
                dbgCmdLine.Focus();
                return;
            }
        }

        #region ContextMenus
        ////Dasm context: Show
        private void dasmPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                //Dump memory at ...
                this.menuItemDumpMemory.MenuItems.Clear();
                this.menuItemFollowInDisassembler.MenuItems.Clear();
                this.menuItemDumpMemory.MenuItems.Add(menuItemDumpMemoryAtCurrentAddress);

                ushort[] numbers = dasmPanel.GetNumberFromCpuInstruction_ActiveLine();
                if (numbers != null)
                {
                    //ToDo: numbers[] - question is whether there can be more numbers in one cpu instruction.
                    MenuItem menuItemNewAdress = new MenuItem();

                    menuItemNewAdress.Index = 0;
                    menuItemNewAdress.Text = String.Format("#{0:X4}", numbers[0]);
                    menuItemNewAdress.Click += new EventHandler(menuItemNewAdress_Clicked);

                    this.menuItemDumpMemory.MenuItems.Add(menuItemNewAdress);

                    //Follow in disassembler(if instrucion contain number)
                    menuItemNewAdress = new MenuItem();

                    menuItemNewAdress.Index = 0;
                    menuItemNewAdress.Text = String.Format("#{0:X4}", numbers[0]);
                    menuItemNewAdress.Click += new EventHandler(menuItemNewAdressFollowInDisassembly_Clicked);

                    this.menuItemFollowInDisassembler.MenuItems.Add(menuItemNewAdress);
                    this.menuItemFollowInDisassembler.Enabled = true;
                }
                else
                    this.menuItemFollowInDisassembler.Enabled = false;

                contextMenuDasm.Show(dasmPanel, e.Location);
            }
        }

        //Dasm context menu: Goto adress
        private void menuItemDasmGotoADDR_Click(object sender, EventArgs e)
        {
            int ToAddr = Historization.DisassemblyGotoAddress;
            var service = Locator.Resolve<IUserQuery>();
            if (service == null)
            {
                return;
            }
            if (!service.QueryValue("Disassembly Address", "Address:", "#{0:X4}", ref ToAddr, 0, 0xFFFF)) return;
            dasmPanel.TopAddress = Historization.DisassemblyGotoAddress = (ushort)ToAddr;
            dasmPanel.Focus();
        }

        //Dasm context menu: Insert breakpoint(extended) here
        private void menuItemInsertBreakpointHere_Click(object sender, EventArgs e)
        {
            List<string> newBreakpoint 
                = new List<string>(String.Format("br pc == #{0:X4}", dasmPanel.ActiveAddress).Split(new char[] { ' ' }).ToArray());
            this.AddExtBreakpoint(newBreakpoint);
            m_showStack = false;
            this.UpdateREGS();
        }

        //Dasm context menu: Insert code comment
        public void InsertCodeComment(string i_newComment, ushort i_addr)
        {
            string strCommentText = String.Empty;
            if (i_newComment == null) //manually inserting by user
            {
                dasmPanel.IsCodeNoteAtAddress(dasmPanel.ActiveAddress, ref strCommentText);
                string strAddressToComment = String.Format("#{0:X4}", dasmPanel.ActiveAddress);
                if (!Locator.Resolve<IUserQuery>().QueryText("Note to add", "Enter note for address " + strAddressToComment + ":", ref strCommentText))
                    return;
            }
            else
                strCommentText = i_newComment;

            dasmPanel.InsertCodeComment(i_addr, strCommentText);
        }

        private void menuItemInsertComment_Click(object sender, EventArgs e)
        {
            string strCommentText = String.Empty;
            dasmPanel.IsCodeCommentAtAddress(dasmPanel.ActiveAddress, ref strCommentText);
            string strAddressToComment = String.Format("#{0:X4}", dasmPanel.ActiveAddress);
            if (!Locator.Resolve<IUserQuery>().QueryText("Comment code address", "Enter comment for address " + strAddressToComment + ":", ref strCommentText))
                return;
            dasmPanel.InsertCodeComment(dasmPanel.ActiveAddress, strCommentText);
        }

        //Dasm context menu: Clear current code comment
        private void menuItemClearCurrentComment_Click(object sender, EventArgs e)
        {
            dasmPanel.ClearCodeComment(dasmPanel.ActiveAddress);
        }

        //Dasm context menu: Clear all code comment
        private void menuItemClearAllComments_Click(object sender, EventArgs e)
        {
            dasmPanel.ClearCodeComments();
        }

        //Dasm context menu: Load comments from file(xml)
        private void menuItemLoadComments_Click(object sender, EventArgs e)
        {
            OpenFileDialog loadDialog = new OpenFileDialog();
            loadDialog.InitialDirectory = ".";
            loadDialog.SupportMultiDottedExtensions = true;
            loadDialog.Title = "Load code comments from file...";
            loadDialog.Filter = "Xml Files (*.xml)|*.xml|All files (*.*)|*.*";
            loadDialog.DefaultExt = "";
            loadDialog.FileName = "";
            loadDialog.ShowReadOnly = false;
            loadDialog.CheckFileExists = true;
            if (loadDialog.ShowDialog() != DialogResult.OK)
                return;
            if (loadDialog.FileName == null || loadDialog.FileName.Length == 0)
                return;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(loadDialog.FileName);

            //parsing comments
            XmlNodeList xmlNodes = xmlDoc.SelectNodes("/Root/Comments/AddressComment");
            ConcurrentDictionary<ushort, string> loadedCodeComments = new ConcurrentDictionary<ushort, string>();
            bool fParseFailed = false;
            foreach(XmlNode node in xmlNodes)
            {
                if (node.Attributes["AddressAt"] == null || node.InnerText == null)
                {
                    fParseFailed = true;
                    break;
                }
                if (node.InnerText == null)
                {
                    fParseFailed = true;
                    break;
                }
                ushort addressAt = ConvertRadix.ConvertNumberWithPrefix(node.Attributes["AddressAt"].InnerText);
                loadedCodeComments.AddOrUpdate(addressAt, node.InnerText, (key, oldValue) => node.InnerText);
            }
            ConcurrentDictionary<ushort, string> loadedCodeNotes = new ConcurrentDictionary<ushort, string>();
            if (!fParseFailed)
            {
                //parsing notes
                xmlNodes = xmlDoc.SelectNodes("/Root/Notes/AddressNote");
                foreach (XmlNode node in xmlNodes)
                {
                    if (node.Attributes["AddressAt"] == null || node.InnerText == null)
                    {
                        fParseFailed = true;
                        break;
                    }
                    if (node.InnerText == null)
                    {
                        fParseFailed = true;
                        break;
                    }
                    ushort addressAt = ConvertRadix.ConvertNumberWithPrefix(node.Attributes["AddressAt"].InnerText);
                    loadedCodeNotes.AddOrUpdate(addressAt, node.InnerText, (key, oldValue) => node.InnerText);
                }
            }

            if( fParseFailed )
            {
                Locator.Resolve<IUserMessage>().Error("Error parsing file...\n\nNothing has been done.");
                return;
            }
            else
            {
                //register loaded code comments
                Locator.Resolve<IUserMessage>().Info("Comments succesfully loaded...");

                dasmPanel.SetCodeComments(loadedCodeComments);
                dasmPanel.SetCodeNotes(loadedCodeNotes);
            }
        }

        //Dasm context menu: Save comments to file(xml)
        private void menuItemSaveComments_Click(object sender, EventArgs e)
        {
            //save dialog
            SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.InitialDirectory = ".";
                saveDialog.SupportMultiDottedExtensions = true;
                saveDialog.Title = "Save code comments to file...";
                saveDialog.Filter = "Xml Files (*.xml)|*.xml|All files (*.*)|*.*";
                saveDialog.DefaultExt = "";
                saveDialog.FileName = "";
                saveDialog.OverwritePrompt = true;
                if (saveDialog.ShowDialog() != DialogResult.OK)
                    return;
                if (saveDialog.FileName == null || saveDialog.FileName.Length == 0)
                    return;

            using (XmlWriter writer = XmlWriter.Create(saveDialog.FileName))
            {
                writer.WriteStartElement("Root");

                //Comments
                if (dasmPanel.GetCodeComments() != null && dasmPanel.GetCodeComments().Count > 0)
                {
                    writer.WriteStartElement("Comments");
                    foreach (var item in dasmPanel.GetCodeComments())
                    {
                        writer.WriteStartElement("AddressComment");
                        writer.WriteAttributeString("AddressAt", String.Format("#{0:X4}", item.Key));
                        writer.WriteElementString("Text", item.Value.ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement(); //Root(CommentsRoot)
                }

                //Notes
                if (dasmPanel.GetCodeNotes() != null && dasmPanel.GetCodeNotes().Count > 0)
                {
                    writer.WriteStartElement("Notes");
                    foreach (var item in dasmPanel.GetCodeNotes())
                    {
                        writer.WriteStartElement("AddressNote");
                        writer.WriteAttributeString("AddressAt", String.Format("#{0:X4}", item.Key));
                        writer.WriteElementString("Text", item.Value.ToString());
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement(); //Root(CommentsRoot)
                }

                writer.WriteEndElement(); //Root

                writer.Flush();
                Locator.Resolve<IUserMessage>().Info("Comments succesfully saved...");
                writer.Close();
            }
        }

        //Dasm context menu: Insert note at address
        private void menuItemInsertNote_Click(object sender, EventArgs e)
        {
            InsertCodeNote(null/*note entering manually by user*/, dasmPanel.ActiveAddress);
        }
        public void InsertCodeNote(string i_newNote, ushort i_addr)
        {
            string strNoteText = String.Empty;
            if (i_newNote == null) //manually inserting by user
            {
                dasmPanel.IsCodeNoteAtAddress(dasmPanel.ActiveAddress, ref strNoteText);
                string strAddressToComment = String.Format("#{0:X4}", dasmPanel.ActiveAddress);
                if (!Locator.Resolve<IUserQuery>().QueryText("Note to add", "Enter note for address " + strAddressToComment + ":", ref strNoteText))
                    return;
            }
            else
                strNoteText = i_newNote;

            dasmPanel.InsertCodeNote(i_addr, strNoteText);
        }
        //Dasm context menu: Clear current note
        private void menuItemClearCurrentNote_Click(object sender, EventArgs e)
        {
            dasmPanel.ClearCodeNote(dasmPanel.ActiveAddress);
        }
        //Dasm context menu: Clear all notes
        private void menuItemClearAllNotes_Click(object sender, EventArgs e)
        {
            dasmPanel.ClearCodeNotes();
        }

        //Trace context menu
        private void listViewAdressRanges_MouseClick(object sender, MouseEventArgs e)
        {
            /*if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuTraceAddrArea.Show(this.listViewAdressRanges, e.Location);
            }*/
        }
        private void listViewAdressRanges_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuTraceAddrArea.Show(this.listViewAdressRanges, e.Location);
            }
        }
        private void listViewAdressRanges_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            m_debuggerTrace.UpdateNewAddrArea(this);

            if (m_isTracing)
                m_debuggerTrace.StartTrace(this); //refresh tracing
        }
        private void listViewAdressRanges_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (listViewAdressRanges.FocusedItem.Index != -1)
                    listViewAdressRanges.Items[listViewAdressRanges.FocusedItem.Index].Remove();
                e.Handled = true;

                if( m_isTracing )
                    m_debuggerTrace.StartTrace(this); //refresh tracing
            }
        }
        #endregion

        #region Conditional breakpoints(memory change, write, registry change, ...)
        //conditional breakpoints
        private ConcurrentDictionary<byte, BreakpointAdlers> _breakpointsExt = null;

        public bool CheckIsBrkMulticonditional(List<string> newBreakpointDesc)
        {
            if (newBreakpointDesc.Count != 8) //multicondional breakpoint; example: br pc == <number> && a == 5
                return false;

            return newBreakpointDesc[4] == "&&";
        }

        public void FillBreakpointConditionData(List<string> i_newBreakpointDesc, int i_brkIndex, ref BreakpointInfo o_breakpointInfo)
        {
            //1.LEFT condition
            bool leftIsMemoryReference = false;
            string leftExpr = i_newBreakpointDesc[i_brkIndex];
            if (DebuggerManager.isMemoryReference(leftExpr))
            {
                o_breakpointInfo.LeftCondition = leftExpr.ToUpper();

                // it can be memory reference by registry value, e.g.: (PC), (DE), ...
                if (DebuggerManager.isRegistryMemoryReference(leftExpr))
                    o_breakpointInfo.LeftRegistryArrayIndex = DebuggerManager.getRegistryArrayIndex(DebuggerManager.getRegistryFromReference(leftExpr));
                else
                    o_breakpointInfo.LeftValue = DebuggerManager.getReferencedMemoryPointer(leftExpr);

                leftIsMemoryReference = true;
            }
            else
            {
                //must be a registry or flag
                if (DebuggerManager.isRegistry(leftExpr))
                {
                    o_breakpointInfo.LeftCondition = leftExpr.ToUpper();
                    o_breakpointInfo.LeftRegistryArrayIndex = DebuggerManager.getRegistryArrayIndex(o_breakpointInfo.LeftCondition);
                    if (leftExpr.Length == 1) //8 bit registry
                        o_breakpointInfo.Is8Bit = true;
                    else
                        o_breakpointInfo.Is8Bit = false;
                }
                else if( DebuggerManager.isFlag(leftExpr) )
                {
                    o_breakpointInfo.LeftCondition = leftExpr.ToUpper();
                    o_breakpointInfo.LeftIsFlag = true;
                }
                else
                {
                    throw new CommandParseException("Incorrect syntax(left expression)!");
                }
            }

            //2.CONDITION type
            o_breakpointInfo.ConditionTypeSign = i_newBreakpointDesc[i_brkIndex+1]; // ==, !=, <, >, ...
            if (o_breakpointInfo.ConditionTypeSign == "==")
                o_breakpointInfo.IsConditionEquals = true;

            //3.RIGHT condition
            byte rightType = 0xFF; // 0 - memory reference, 1 - registry value, 2 - common value

            string rightExpr = i_newBreakpointDesc[i_brkIndex+2];
            if (DebuggerManager.isMemoryReference(rightExpr))
            {
                o_breakpointInfo.RightCondition = rightExpr.ToUpper(); // because of breakpoint panel
                o_breakpointInfo.RightValue = m_spectrum.ReadMemory(DebuggerManager.getReferencedMemoryPointer(rightExpr));

                rightType = 0;
            }
            else
            {
                if (DebuggerManager.isRegistry(rightExpr))
                {
                    o_breakpointInfo.RightCondition = rightExpr;

                    rightType = 1;
                }
                else
                {
                    //it has to be a common value, e.g.: #4000, %111010101, ...
                    o_breakpointInfo.RightCondition = rightExpr.ToUpper(); // because of breakpoint panel
                    o_breakpointInfo.RightValue = ConvertRadix.ConvertNumberWithPrefix(rightExpr); // last chance

                    rightType = 2;
                }
            }

            if (rightType == 0xFF)
                throw new CommandParseException("Incorrect right expression!");
            if (o_breakpointInfo.LeftIsFlag && rightType != 2)
                throw new CommandParseException("Flags allows only true/false right expression...");

            //4. finish
            if (leftIsMemoryReference)
            {
                if (DebuggerManager.isRegistryMemoryReference(o_breakpointInfo.LeftCondition)) // left condition is e.g.: (PC), (HL), (DE), ...
                {
                    if (rightType == 2) // right is number
                        o_breakpointInfo.AccessType = BreakPointConditionType.registryMemoryReferenceVsValue;
                }
            }
            else
            {
                if (rightType == 2)
                {
                    o_breakpointInfo.AccessType = o_breakpointInfo.LeftIsFlag ?  BreakPointConditionType.flagVsValue : BreakPointConditionType.registryVsValue;
                }
            }
        } //end FillBreakpointData()

        public void AddExtBreakpoint(List<string> newBreakpointDesc)
        {
            if (_breakpointsExt == null)
                _breakpointsExt = new ConcurrentDictionary<byte, BreakpointAdlers>();

            BreakpointInfo breakpointInfo = new BreakpointInfo();
            breakpointInfo.IsMulticonditional = CheckIsBrkMulticonditional(newBreakpointDesc);

            //0. memory read/write breakpoint ?
            if (newBreakpointDesc[1].ToUpper() == "MEMREAD" || newBreakpointDesc[1].ToUpper() == "MEMWRITE")
            {
                bool fMemWrite = (newBreakpointDesc[1].ToUpper() == "MEMWRITE");

                if (newBreakpointDesc.Count >= 3) //e.g.: "br memread #4000" or "br memread #4000 #5B00"
                {
                    breakpointInfo.IsOn = true;
                    breakpointInfo.LeftValue = ConvertRadix.ConvertNumberWithPrefix(newBreakpointDesc[2]); // set "start" memory checkpoint
                    if (newBreakpointDesc.Count > 3)
                    {
                        //memory range
                        breakpointInfo.RightValue = ConvertRadix.ConvertNumberWithPrefix(newBreakpointDesc[3]); // set "stop" memory checkpoint
                        breakpointInfo.AccessType = (fMemWrite ? BreakPointConditionType.memoryWriteInRange : BreakPointConditionType.memoryReadInRange);
                    }
                    else
                        breakpointInfo.AccessType = (fMemWrite ?  BreakPointConditionType.memoryWrite : BreakPointConditionType.memoryRead);

                    InsertNewBreakpoint(breakpointInfo);
                }
                else
                    throw new CommandParseException("Incorrect memory check breakpoint syntax...");

                return;
            }

            //fill first condition
            FillBreakpointConditionData(newBreakpointDesc, 1, ref breakpointInfo);

            //let emit CIL code to check the breakpoint
            Func<bool> checkBreakpoint = ILProcessor.EmitCondition(ref m_spectrum, breakpointInfo);
            if (checkBreakpoint != null)
                breakpointInfo.SetBreakpointCheckMethod(checkBreakpoint);

            //multiconditional breakpoint ?
            if (breakpointInfo.IsMulticonditional)
            {
                //second condition
                FillBreakpointConditionData(newBreakpointDesc, 5, ref breakpointInfo);
                checkBreakpoint = ILProcessor.EmitCondition(ref m_spectrum, breakpointInfo);
                breakpointInfo.SetCheckSecondCondition(checkBreakpoint);
            }

            breakpointInfo.IsOn = true; // activate the breakpoint

            //save breakpoint command line string
            breakpointInfo.BreakpointString = String.Empty;
            for (byte counter = 1; counter < newBreakpointDesc.Count; counter++)
            {
                breakpointInfo.BreakpointString += newBreakpointDesc[counter];
            }

            InsertNewBreakpoint(breakpointInfo);

            //when breakpoint is succesfully inserted => switch tab to CPU(show breakpoints)
            this.tabMenus.SelectedIndex = 0;
        }
        public void RemoveExtBreakpoint(string brkIndex)
        {
            if (_breakpointsExt == null || _breakpointsExt.Count == 0)
                throw new Exception("No breakpoints...!");

            if (brkIndex.ToUpper() == "ALL")
            {
                _breakpointsExt.Clear();
                m_spectrum.ClearBreakpoints();
                UpdateREGS();
            }
            else
            {
                byte index = Convert.ToByte(brkIndex);
                BreakpointAdlers bpAdlers;
                if (_breakpointsExt.TryRemove(index, out bpAdlers))
                    m_spectrum.RemoveBreakpoint(bpAdlers);
                else
                    throw new Exception(String.Format("No breakpoint with index {0} !", index));
            }
        }
        public ConcurrentDictionary<byte, BreakpointAdlers> GetExtBreakpointsList()
        {
            if (_breakpointsExt != null)
                return _breakpointsExt;

            _breakpointsExt = new ConcurrentDictionary<byte, BreakpointAdlers>();

            return _breakpointsExt;
        }
        private void InsertNewBreakpoint(BreakpointInfo info)
        {
            //prevent duplicity in breakpoints so that the same breakpoint could not be inserted more than once
            {
                BreakpointAdlers[] breakpointCandidates = _breakpointsExt.Values.Where(p => p.Info.AccessType == info.AccessType).ToArray();
                if( breakpointCandidates.Length != 0 )
                {
                    foreach(BreakpointAdlers item in breakpointCandidates)
                    {
                        if ( item.Info.BreakpointString == info.BreakpointString //ToDo: must be checked by RightValue, RightCondition, ...but for both conditions if multiconditional breakpoint type
                          && item.Info.AccessType != BreakPointConditionType.memoryWrite
                          && item.Info.AccessType != BreakPointConditionType.memoryWriteInRange
                          && item.Info.AccessType != BreakPointConditionType.memoryRead
                          && item.Info.AccessType != BreakPointConditionType.memoryReadInRange
                          )
                        {
                            item.Info.IsOn = true;
                            return;
                        }
                    }
                }
            }

            // ADD breakpoint into list
            // Here will be the breakpoint key assigned by searching keys starting with key 0
            // Maximum 255 breakpoints is allowed
            if (_breakpointsExt.Count < 255)
            {
                var bp = new BreakpointAdlers(info);
                _breakpointsExt.TryAdd(GetNewBreakpointPosition(), bp);
                m_spectrum.AddBreakpoint(bp);
            }
            else
                throw new CommandParseException("Maximum breakpoints count(255) exceeded...");
        }
        private byte GetNewBreakpointPosition()
        {
            if( _breakpointsExt.Count == 0 )
                return 0;

            for (byte positionCounter = 0; positionCounter < _breakpointsExt.Count; positionCounter++)
                if (!_breakpointsExt.ContainsKey(positionCounter))
                    return positionCounter;

            return Convert.ToByte(_breakpointsExt.Count);
        }

        public void CheckWriteMem(ushort addr, byte value)
        {
            if (_breakpointsExt == null)
                return;

            foreach (BreakpointAdlers brk in _breakpointsExt.Values.Where(p => p.Info.IsOn))
            {
                if (brk.Info.AccessType == BreakPointConditionType.memoryVsValue || brk.Info.AccessType == BreakPointConditionType.registryMemoryReferenceVsValue)
                    brk.IsNeedWriteMemoryCheck = true;

                if (brk.Info.LeftValue == addr)
                {
                    if (brk.Info.AccessType == BreakPointConditionType.memoryWrite)
                        brk.IsForceStop = true;
                }

                //memory in range
                if (brk.Info.AccessType == BreakPointConditionType.memoryWriteInRange && brk.Info.LeftValue <= addr && brk.Info.RightValue >= addr)
                    brk.IsNeedWriteMemoryCheck = true;
            }

            return;
        }
        public void CheckReadMem(ushort addr, ref byte value)
        {
            if (_breakpointsExt == null)
                return;

            foreach (BreakpointAdlers brk in _breakpointsExt.Values.Where(p => p.Info.IsOn && 
                                                                               ( p.Info.AccessType == BreakPointConditionType.memoryRead 
                                                                                || 
                                                                                 p.Info.AccessType == BreakPointConditionType.memoryReadInRange)
                                                                               )
                                                                         )
            {
                if (brk.Info.LeftValue == addr && brk.Info.AccessType == BreakPointConditionType.memoryRead)
                    brk.IsForceStop = true;

                //memory in range
                if(brk.Info.AccessType == BreakPointConditionType.memoryReadInRange && brk.Info.LeftValue <= addr && brk.Info.RightValue >= addr )
                    brk.IsForceStop = true;

                //IsForceStop: raise force stop at the end of the currect CPU cycle
                //            (this flag will be checked from BreakpointAdlers.Check at the end of CPU cycle)
            }

            return;
        }

        public void EnableOrDisableBreakpointStatus(byte whichBpToEnableOrDisable, bool setOn) //enables/disables breakpoint, command "on" or "off"
        {
            if (_breakpointsExt == null || _breakpointsExt.Count == 0)
                return;

            if (!_breakpointsExt.ContainsKey(whichBpToEnableOrDisable))
                return;

            BreakpointAdlers temp = _breakpointsExt[whichBpToEnableOrDisable];
            temp.Info.IsOn = setOn;
            return;
        }

        public void LoadBreakpointsListFromFile(string fileName)
        {
            System.IO.StreamReader file = null;
            try
            {
                if (!File.Exists(Path.Combine(Utils.GetAppFolder(), fileName)))
                    throw new Exception("file " + fileName + " does not exists...");

                string dbgCommandFromFile = String.Empty;
                file = new System.IO.StreamReader(Path.Combine(Utils.GetAppFolder(), fileName));
                while ((dbgCommandFromFile = file.ReadLine()) != null)
                {
                    if (dbgCommandFromFile.Trim() == String.Empty || dbgCommandFromFile[0] == ';')
                        continue;

                    List<string> parsedCommand = DebuggerManager.ParseCommand("br " + dbgCommandFromFile);
                    if (parsedCommand == null)
                        throw new Exception("unknown debugger command");

                    AddExtBreakpoint(parsedCommand);
                }
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }

        public void SaveBreakpointsListToFile(string fileName)
        {
            ConcurrentDictionary<byte, BreakpointAdlers> localBreakpointsList = GetExtBreakpointsList();
            if (localBreakpointsList.Count == 0)
                return;

            System.IO.StreamWriter file = null;
            try
            {
                file = new System.IO.StreamWriter(Path.Combine(Utils.GetAppFolder(), fileName));

                foreach (KeyValuePair<byte, BreakpointAdlers> breakpoint in localBreakpointsList)
                {
                    file.WriteLine(breakpoint.Value.Info.BreakpointString);
                }
            }
            finally
            {
                file.Close();
            }
        }

        #endregion

        #region GUI shortcuts
        private void FormCPU_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F3:              // reset
                    if (m_spectrum.IsRunning)
                        break;
                    m_spectrum.DoReset();
                    UpdateCPU(true);
                    break;
                case Keys.F7:              // StepInto
                    if (m_spectrum.IsRunning)
                        break;
                    try
                    {
                        m_spectrum.DoStepInto();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        Locator.Resolve<IUserMessage>().ErrorDetails(ex);
                    }
                    UpdateCPU(true);
                    break;
                case Keys.F8:              // StepOver
                    if (m_spectrum.IsRunning)
                        break;
                    try
                    {
                        m_spectrum.DoStepOver();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        Locator.Resolve<IUserMessage>().ErrorDetails(ex);
                    }
                    UpdateCPU(true);
                    break;
                case Keys.F10:             // Run
                    m_spectrum.DoRun();
                    UpdateCPU(false);
                    break;
                case Keys.F5:              // Stop
                    m_spectrum.DoStop();
                    UpdateCPU(true);
                    e.Handled = true;
                    break;
                case Keys.F12:  // toggle Stack/Breakpoints on the panel
                    m_showStack = !m_showStack;
                    UpdateStack();
                    break;
                case Keys.F: //find bytes in memory(memory dump panel)
                    if (e.Control)
                    {
                        menuItemFindBytes_Click(new object(), null);
                    }
                    break;
                case Keys.N: //find next bytes
                    if (e.Control)
                    {
                        menuItemFindBytesNext_Click(null, null);
                    }
                    break;
                case Keys.Escape:
                    if (savedCmdLineString != String.Empty)
                    {
                        ClearCommandLineErrorText();
                        e.Handled = true;
                    }
                    else
                    {
                        this.Hide();
                        if (this.Owner != null)
                            this.Owner.Focus();
                    }
                    break;
                case Keys.G: //Goto address(disassembly)
                    if (e.Control)
                    {
                        menuItemDasmGotoADDR_Click(null, null);
                    }
                    break;
                case Keys.D: //Dump memory at address
                    if (e.Control)
                    {
                        menuItemDataGotoADDR_Click(null, null);
                    }
                    break;
                case Keys.D1: //Insert new comment
                    if (e.Control)
                    {
                        this.menuItemInsertComment_Click(null, null);
                        e.Handled = true;
                    }
                    break;
                case Keys.D2: //Insert new note
                    if (e.Control)
                    {
                        this.menuItemInsertNote_Click(null, null);
                        e.Handled = true;
                    }
                    break;
            }
        }
        #endregion

        #region Trace GUI methods
        private void checkBoxOpcode_CheckedChanged(object sender, EventArgs e)
        {
            textBoxOpcode.Enabled = checkBoxOpcode.Checked;
        }
        private void checkBoxAllJumps_CheckedChanged(object sender, EventArgs e)
        {
            checkBoxConditionalCalls.Enabled = checkBoxConditionalJumps.Enabled = !checkBoxAllJumps.Checked;
            /*if (m_isTracing)
                m_debuggerTrace.StartTrace(this);*/ //refresh trace settings
        }
        private void checkBoxConditionalJumps_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxConditionalJumps.Checked)
                checkBoxAllJumps.Checked = false;
            /*if (m_isTracing)
                m_debuggerTrace.StartTrace(this);*/ //refresh trace settings
        }
        private void checkBoxConditionalCalls_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxConditionalCalls.Checked)
                checkBoxAllJumps.Checked = false;
            /*if (m_isTracing)
                m_debuggerTrace.StartTrace(this);*/ //refresh trace settings
        }
        private void checkBoxDetectJumpOnAddress_CheckedChanged(object sender, EventArgs e)
        {
            textBoxJumpToAnAddress.Enabled = checkBoxDetectJumpOnAddress.Checked;
        }
        private void checkBoxTraceFileOut_CheckedChanged(object sender, EventArgs e)
        {
            buttonSetTraceFileName.Enabled = textBoxTraceFileName.Enabled = checkBoxTraceFileOut.Checked;
        }
        private void checkBoxTraceAddresses_CheckedChanged(object sender, EventArgs e)
        {
            listViewAdressRanges.Enabled = checkBoxTraceArea.Checked;
            /*if (m_isTracing)
                m_debuggerTrace.StartTrace(this);*/ //refresh trace settings
        }
        private void btnStartTrace_Click(object sender, EventArgs e)
        {
            lock( m_sync )
            {
                //Trace start
                if (!m_debuggerTrace.StartTrace(this))
                    return;

                btnStartTrace.Enabled = false;
                btnStopTrace.Enabled = true;
                m_isTracing = true;
            }
        }
        private void btnStopTrace_Click(object sender, EventArgs e)
        {
            lock (m_sync)
            {
                //Trace stop
                btnStartTrace.Enabled = true;
                btnStopTrace.Enabled = false;

                m_debuggerTrace.StopTrace();
                if (checkBoxTraceAutoOpenLog.Checked)
                    ShowTraceLogFile();

                m_isTracing = false;
            }
        }
        private void menuItemAddNewTraceAddrArea_Click(object sender, EventArgs e)
        {
            m_debuggerTrace.AddNewAddrArea(this);
            if (m_isTracing)
                m_debuggerTrace.StartTrace(this); //refresh tracing
        }
        private void menuItemUpdateTraceAddrArea_Click(object sender, EventArgs e)
        {
            m_debuggerTrace.UpdateNewAddrArea(this);
            if (m_isTracing)
                m_debuggerTrace.StartTrace(this); //refresh tracing
        }
        private void textBoxOpcode_Leave(object sender, EventArgs e)
        {
            /*try
            {
                UInt16 opcode = DebuggerManager.convertNumberWithPrefix(textBoxOpcode.Text);
                if (opcode > 0xFF) //ToDo: only one byte for traced opcode
                    m_debuggerTrace.SetTracedOpcode((byte)(opcode%256));
                else
                    m_debuggerTrace.SetTracedOpcode((byte)opcode);
            }
            catch(CommandParseException)
            {
                Locator.Resolve<IUserMessage>().Error("Incorrect opcode number...");
                textBoxOpcode.Focus();
            }*/
        }
        #endregion

        #region Config
        public static string ConfigXmlFileName = "debugger_config.xml";
        private XmlNode _xmlNode_SavedAssemblerSettings = null;
        private void SaveConfig()
        {
            XmlWriter writer = XmlWriter.Create(Path.Combine(Utils.GetAppFolder(), ConfigXmlFileName));
            //using (XmlWriter writer = XmlWriter.Create(Path.Combine(Utils.GetAppFolder(), configXmlFileName)))
            {
                writer.WriteStartElement("Root");
                //Load on startup
                writer.WriteElementString("LoadConfigOnStartup", this.checkBoxLoadConfig.Checked ? "1" : "0");

                //Debugger
                writer.WriteStartElement("Debugger");
                writer.WriteElementString("Breakpoints", "ToDo");
                writer.WriteEndElement(); //Debugger

                //Trace
                writer.WriteStartElement("Trace");
                writer.WriteElementString("AllJumpsCalls", this.checkBoxAllJumps.Checked ? "1" : "0");
                writer.WriteElementString("ConditionalJumps", this.checkBoxConditionalJumps.Checked ? "1" : "0");
                writer.WriteElementString("ConditionalCalls", this.checkBoxConditionalCalls.Checked ? "1" : "0");
                    //Trace->DetectJumpToAnAdress
                    writer.WriteStartElement("DetectJumpToAnAdress");
                    writer.WriteAttributeString("Checked", this.checkBoxDetectJumpOnAddress.Checked ? "1" : "0");
                    writer.WriteElementString("Value", this.textBoxJumpToAnAddress.Text);
                    writer.WriteEndElement();
                    //Trace->Opcode
                    writer.WriteStartElement("Opcode");
                    writer.WriteAttributeString("Checked", this.checkBoxOpcode.Checked ? "1" : "0");
                    writer.WriteElementString("Value", this.textBoxOpcode.Text);
                    writer.WriteEndElement();
                    //Trace->AddressRange
                    writer.WriteStartElement("AddressRange");
                    writer.WriteAttributeString("Checked", this.checkBoxTraceArea.Checked ? "1" : "0");
                    string tagArr = String.Empty;
                    foreach( ListViewItem item in this.listViewAdressRanges.Items )
                    {
                        tagArr += item.Tag + "|";
                    }
                    writer.WriteElementString("TagArray", tagArr);
                writer.WriteEndElement(); //Trace(end)

                writer.WriteElementString("TraceCount", this.checkBoxTactCount.Checked ? "1" : "0");
                writer.WriteElementString("ConsoleOutput", this.checkBoxShowConsole.Checked ? "1" : "0");
                writer.WriteElementString("OutputToFile", this.checkBoxTraceFileOut.Checked ? "1" : "0");
                writer.WriteElementString("OutputFileName", this.textBoxTraceFileName.Text);
                writer.WriteEndElement(); //Trace

                //Misc.
                writer.WriteStartElement("Misc");
                writer.WriteElementString("AutoOpenTraceLog", this.checkBoxTraceAutoOpenLog.Checked ? "1" : "0");
                //Misc(end)
                writer.WriteEndElement();

                //write assembler config
                if (Assembler.GetInstance() != null)
                    Assembler.GetInstance().GetPartialConfig(ref writer);
                else
                {
                    if (_xmlNode_SavedAssemblerSettings != null)
                        writer.WriteRaw(_xmlNode_SavedAssemblerSettings.OuterXml);
                }

                writer.WriteEndElement(); //Root
                writer.Flush();

                writer.Close();
            }
        }

        private void LoadConfig()
        {           
            if (!File.Exists(Path.Combine(Utils.GetAppFolder(), ConfigXmlFileName)))
                return;

            XmlDocument xmlDoc = new System.Xml.XmlDocument();
            try
            {
                xmlDoc.Load(Path.Combine(Utils.GetAppFolder(), ConfigXmlFileName));
            }
            catch(XmlException)
            {
                Locator.Resolve<IUserMessage>().Error("Configuration file is corrupted and will be deleted.");
                File.Delete(Path.Combine(Utils.GetAppFolder(), ConfigXmlFileName));
                return;
            }

            XmlNode node;

            //LoadConfigOnStartup
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/LoadConfigOnStartup");
            if (node != null)
            {
                bool loadConfig = (node.InnerText == "1");
                this.checkBoxLoadConfig.Checked = loadConfig;
                if (!loadConfig)
                    return;
            }
            else
                return;

            //Set tooltips
            /*ToolTip toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 300;
            toolTip1.ReshowDelay = 500;
            toolTip1.ShowAlways = true;
            toolTip1.SetToolTip(this.checkBoxAllJumps, "Tracing all jumps(JP, JR, ...)");*/

            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Trace/AllJumpsCalls");
            if (node != null)
                this.checkBoxAllJumps.Checked = (node.InnerText == "1");
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Trace/ConditionalJumps");
            if (node != null)
                this.checkBoxConditionalJumps.Checked = (node.InnerText == "1");
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Trace/ConditionalCalls");
            if (node != null)
                this.checkBoxConditionalCalls.Checked = (node.InnerText == "1");

                //Trace->DetectJumpToAnAdress
                node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Trace/DetectJumpToAnAdress");
                if (node != null)
                {
                    this.checkBoxDetectJumpOnAddress.Checked = (node.Attributes["Checked"].InnerText == "1");
                    this.textBoxJumpToAnAddress.Text = node.InnerText;
                }

                //Trace->Opcode
                node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Trace/Opcode");
                if (node != null)
                {
                    this.checkBoxOpcode.Checked = (node.Attributes["Checked"].InnerText == "1");
                    this.textBoxOpcode.Text = node.InnerText;
                }

                //Trace->AddressRange
                node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Trace/AddressRange");
                if (node != null)
                    this.checkBoxTraceArea.Checked = (node.Attributes["Checked"].InnerText == "1");
                node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Trace/AddressRange/TagArray");
                if( node != null )
                {
                    string[] tags = ((string)node.InnerText).Split(new char[] { '|' });
                    tags = tags.Where(x => !string.IsNullOrEmpty(x)).ToArray();
                    if (tags != null)
                    {
                        foreach (string tag in tags)
                        {
                            string[] tagsCurr = tag.Split(new char[] { ';' });
                            if (tagsCurr.Length != 3)
                                continue;

                            ListViewItem itemToAdd = new ListViewItem(new[] { String.Format("#{0:X4}", tagsCurr[0]), String.Format("#{0:X4}", tagsCurr[1]), tagsCurr[2] });
                            itemToAdd.Tag = tag;
                            this.listViewAdressRanges.Items.Add(itemToAdd);
                        }
                    }
                }

            //Misc.->Auto open trace log check box
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Misc/AutoOpenTraceLog");
            if (node != null)
                this.checkBoxTraceAutoOpenLog.Checked = (node.InnerText == "1");

            //TraceCount
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Trace/TraceCount");
            if (node != null)
                this.checkBoxTactCount.Checked = (node.InnerText == "1");
            //ConsoleOutput
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Trace/ConsoleOutput");
            if( node != null )
                this.checkBoxShowConsole.Checked = (node.InnerText == "1");
            //OutputToFile(yes, no)
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Trace/OutputToFile");
            if (node != null)
                this.checkBoxTraceFileOut.Checked = (node.InnerText == "1");
            //OutputFileName
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Trace/OutputFileName");
            if (node != null)
                this.textBoxTraceFileName.Text = node.InnerText;

            //Assembler
            _xmlNode_SavedAssemblerSettings = xmlDoc.SelectSingleNode("Root/Assembler");
        }
        #endregion

        private void buttonSetTraceFileName_Click(object sender, EventArgs e)
        {
            ShowTraceLogFile();
        }

        private void ShowTraceLogFile()
        {
            string strFileName = textBoxTraceFileName.Text;
            if (strFileName.Length == 0)
                return;
            string logFileName = Path.Combine(Utils.GetAppFolder(), strFileName);
            if (!File.Exists(logFileName))
                return;
            System.Diagnostics.Process.Start(logFileName);
        }
    }
}