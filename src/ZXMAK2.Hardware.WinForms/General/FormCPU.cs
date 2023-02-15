/// Description: CPU Debug Window
/// Author: Alex Makeev
/// Date: 18.03.2008
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using ZXMAK2.Engine.Cpu.Tools;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Presentation.Interfaces;
using ZXMAK2.Host.WinForms.Views;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Hardware.WinForms.General
{
    public partial class FormCpu : FormView, IDebuggerGeneralView
    {
        private IDebuggable m_spectrum;
        private DasmTool m_dasmTool;
        private TimingTool m_timingTool;

        public FormCpu(IDebuggable debugTarget)
        {
            InitializeComponent();
            Init(debugTarget);
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
        }

        private void FormCPU_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_spectrum.UpdateState -= spectrum_OnUpdateState;
            m_spectrum.Breakpoint -= spectrum_OnBreakpoint;
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
            Select();
        }


        private void spectrum_OnUpdateState(object sender, EventArgs args)
        {
            if (!Created)
                return;
            BeginInvoke(new Action(() => UpdateCPU(true)), null);
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

        private void UpdateCPU(bool updatePC)
        {
            var isRunning = m_spectrum.IsRunning;

            statusStrip.BackColor = isRunning ? ColorTranslator.FromHtml("#cc6600") : ColorTranslator.FromHtml("#0077cc");
            statusStrip.ForeColor = isRunning ? ColorTranslator.FromHtml("#ffffff") : ColorTranslator.FromHtml("#ffffff");
            toolStripStatus.Text = isRunning ? "Running" : "Ready";
            toolStripStatusTact.Text = isRunning ? string.Format("T: - / {0}", m_spectrum.FrameTactCount) :
                string.Format("T: {0} / {1}", m_spectrum.GetFrameTact(), m_spectrum.FrameTactCount);
            toolStripStatusTact.Enabled = !isRunning;

            if (isRunning)
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

            listState.Items.Clear();
            listState.Items.Add("IFF1=" + (m_spectrum.CPU.IFF1 ? "1" : "0") + " IFF2=" + (m_spectrum.CPU.IFF2 ? "1" : "0"));
            listState.Items.Add("HALT=" + (m_spectrum.CPU.HALTED ? "1" : "0"));
            listState.Items.Add("BINT=" + (m_spectrum.CPU.BINT ? "1" : "0"));
            listState.Items.Add("  IM=" + m_spectrum.CPU.IM.ToString());
            listState.Items.Add("  FX=" + m_spectrum.CPU.FX.ToString());
            listState.Items.Add(" XFX=" + m_spectrum.CPU.XFX.ToString());
            listState.Items.Add(" LPC=#" + m_spectrum.CPU.LPC.ToString("X4"));
            listState.Items.Add("Tact=" + m_spectrum.CPU.Tact.ToString());
            if (m_spectrum.RzxState.IsPlayback)
            {
                listState.Items.Add(string.Format("rzxm={0}/{1}", m_spectrum.RzxState.Fetch, m_spectrum.RzxState.FetchCount));
                listState.Items.Add(string.Format("rzxi={0}/{1}", m_spectrum.RzxState.Input, m_spectrum.RzxState.InputCount));
                listState.Items.Add(string.Format("rzff={0}/{1}", m_spectrum.RzxState.Frame, m_spectrum.RzxState.FrameCount));
            }
        }

        private void UpdateDASM(bool updatePC)
        {
            if (!m_spectrum.IsRunning && updatePC)
            {
                dasmPanel.ActiveAddress = m_spectrum.CPU.regs.PC;
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
            {
                data[i] = m_spectrum.ReadMemory((ushort)(ADDR + i));
            }
        }

        private bool dasmPanel_CheckBreakpoint(object sender, ushort addr)
        {
            foreach (var bp in m_spectrum.GetBreakpointList())
            {
                if (bp.Address.HasValue && bp.Address == addr)
                    return true;
            }
            return false;
        }

        private void dasmPanel_BreakpointClick(object sender, ushort addr)
        {
            bool found = false;
            foreach (var bp in m_spectrum.GetBreakpointList())
            {
                if (bp.Address.HasValue && bp.Address == addr)
                {
                    m_spectrum.RemoveBreakpoint(bp);
                    found = true;
                }
            }
            if (!found)
            {
                var bp = new Breakpoint(addr);
                m_spectrum.AddBreakpoint(bp);
            }
        }

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
                case Keys.F9:              // Run
                    m_spectrum.DoRun();
                    UpdateCPU(false);
                    break;
                case Keys.F5:              // Stop
                    m_spectrum.DoStop();
                    UpdateCPU(true);
                    break;
            }
        }

        private void menuItemDasmGotoADDR_Click(object sender, EventArgs e)
        {
            int ToAddr = 0;
            var service = Locator.Resolve<IUserQuery>();
            if (service == null)
            {
                return;
            }
            if (!service.QueryValue("Disassembly Address", "New Address:", "#{0:X4}", ref ToAddr, 0, 0xFFFF))
            {
                return;
            }
            dasmPanel.TopAddress = (ushort)ToAddr;
        }

        private void menuItemDasmGotoPC_Click(object sender, EventArgs e)
        {
            dasmPanel.ActiveAddress = m_spectrum.CPU.regs.PC;
            dasmPanel.UpdateLines();
            Refresh();
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
            m_spectrum.CPU.regs.F ^= (byte)(0x80 >> listF.SelectedIndex);
            UpdateREGS();
        }

        private void listREGS_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listREGS.SelectedIndex < 0) return;
            if (m_spectrum.IsRunning) return;
            ChangeRegByIndex(listREGS.SelectedIndex);
        }

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
            if (service == null)
            {
                return;
            }
            if (!service.QueryValue("Change Register " + reg, "New value:", "#{0:X4}", ref val, 0, 0xFFFF)) return;
            p = (ushort)val;
            UpdateCPU(false);
        }


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
            if (service == null)
            {
                return;
            }
            if (!service.QueryValue("POKE #" + Addr.ToString("X4"), "Value:", "#{0:X2}", ref poked, 0, 0xFF)) return;
            m_spectrum.WriteMemory((ushort)Addr, (byte)poked);
            UpdateCPU(false);
        }

        private void menuItemDataGotoADDR_Click(object sender, EventArgs e)
        {
            int adr = dataPanel.TopAddress;
            var service = Locator.Resolve<IUserQuery>();
            if (service == null)
            {
                return;
            }
            if (!service.QueryValue("Data Panel Address", "New Address:", "#{0:X4}", ref adr, 0, 0xFFFF)) return;
            dataPanel.TopAddress = (ushort)adr;
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
            if (service == null)
            {
                return;
            }
            if (!service.QueryValue("Data Panel Columns", "Column Count:", "{0}", ref cols, 1, 32)) return;
            dataPanel.ColCount = cols;
        }

        private void dasmPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuDasm.Show(dasmPanel, e.Location);
        }

        private void dataPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                contextMenuData.Show(dataPanel, e.Location);
        }

        private void listState_DoubleClick(object sender, EventArgs e)
        {
            if (listState.SelectedIndex < 0) return;
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
            }
            UpdateCPU(false);
        }

        private static int s_addr = 0x4000;
        private static int s_len = 6912;

        private void menuLoadBlock_Click(object sender, EventArgs e)
        {
            using (var loadDialog = new OpenFileDialog())
            {
                loadDialog.SupportMultiDottedExtensions = true;
                loadDialog.Title = "Load Block...";
                loadDialog.Filter = "All files (*.*)|*.*";
                loadDialog.DefaultExt = string.Empty;
                loadDialog.FileName = null;
                loadDialog.ShowReadOnly = false;
                loadDialog.CheckFileExists = true;
                if (loadDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                FileInfo fileInfo = new FileInfo(loadDialog.FileName);
                s_len = (int)fileInfo.Length;

                if (s_len < 1)
                    return;
                var service = Locator.Resolve<IUserQuery>();
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
            if (!service.QueryValue("Save Block", "Memory Address:", "#{0:X4}", ref s_addr, 0, 0xFFFF))
                return;
            if (!service.QueryValue("Save Block", "Block Length:", "#{0:X4}", ref s_len, 0, 0x10000))
                return;

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.SupportMultiDottedExtensions = true;
                saveDialog.Title = "Save Block...";
                saveDialog.Filter = "Binary Files (*.bin)|*.bin|All files (*.*)|*.*";
                saveDialog.DefaultExt = "";
                saveDialog.FileName = "";
                saveDialog.OverwritePrompt = true;
                if (saveDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                byte[] data = new byte[s_len];
                m_spectrum.ReadMemory((ushort)s_addr, data, 0, s_len);

                using (FileStream fs = new FileStream(saveDialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                    fs.Write(data, 0, data.Length);
            }
        }

        private void toolStripStatusTact_DoubleClick(object sender, EventArgs e)
        {
            var frameTact = m_spectrum.GetFrameTact();
            var service = Locator.Resolve<IUserQuery>();
            if (service.QueryValue("Frame Tact", "New Frame Tact:", "{0}", ref frameTact, 0, m_spectrum.FrameTactCount))
            {
                var delta = frameTact - m_spectrum.GetFrameTact();
                if (delta < 0)
                    delta += m_spectrum.FrameTactCount;
                m_spectrum.CPU.Tact += delta;
            }
            UpdateCPU(false);
        }
    }
}