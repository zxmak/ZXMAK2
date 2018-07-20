using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

using ZXMAK2.Dependency;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;
using ZXMAK2.Engine.Cpu.Tools;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Presentation.Interfaces;
using ZXMAK2.Hardware.Sprinter;
using ZXMAK2.Hardware.WinForms.General;
using ZXMAK2.Host.WinForms.Views;


namespace ZXMAK2.Hardware.WinForms.Sprinter
{
    public partial class DebugForm : FormView, IDebuggerSprinterView
    {
        private DasmPanel dasmPanel;
        private DataPanel dataPanel;
        private DasmTool m_dasmTool;
        private TimingTool m_timingTool;
        private IDebuggable m_spectrum;
        private static int s_addr;
        private static int s_len;

        // ZEK +++
        private SprinterMMU sprint_mmu;
        private SprinterULA sprint_ula;
        //private BDI pevo_bdi;
        // ZEK ---


        public DebugForm(IDebuggable debugTarget)
        {
            InitializeComponent();
            Init(debugTarget);
        }

        private void ChangeReg(ref ushort p, string reg)
        {
            int num = p;
            var service = Locator.Resolve<IUserQuery>();
            if (service == null)
            {
                return;
            }
            if (service.QueryValue("Change Register " + reg, "New value:", "#{0:X4}", ref num, 0, 0xffff))
            {
                p = (ushort)num;
                UpdateCPU(false);
            }
        }

        private void ChangeRegByIndex(int index)
        {
            switch (index)
            {
                case 0:
                    ChangeReg(ref m_spectrum.CPU.regs.PC, "PC");
                    return;

                case 1:
                    ChangeReg(ref m_spectrum.CPU.regs.IR, "IR");
                    return;

                case 2:
                    ChangeReg(ref m_spectrum.CPU.regs.SP, "SP");
                    return;

                case 3:
                    ChangeReg(ref m_spectrum.CPU.regs.AF, "AF");
                    return;

                case 4:
                    ChangeReg(ref m_spectrum.CPU.regs.HL, "HL");
                    return;

                case 5:
                    ChangeReg(ref m_spectrum.CPU.regs.DE, "DE");
                    return;

                case 6:
                    ChangeReg(ref m_spectrum.CPU.regs.BC, "BC");
                    return;

                case 7:
                    ChangeReg(ref m_spectrum.CPU.regs.IX, "IX");
                    return;

                case 8:
                    ChangeReg(ref m_spectrum.CPU.regs.IY, "IY");
                    return;

                case 9:
                    ChangeReg(ref m_spectrum.CPU.regs._AF, "AF'");
                    return;

                case 10:
                    ChangeReg(ref m_spectrum.CPU.regs._HL, "HL'");
                    return;

                case 11:
                    ChangeReg(ref m_spectrum.CPU.regs._DE, "DE'");
                    return;

                case 12:
                    ChangeReg(ref m_spectrum.CPU.regs._BC, "BC'");
                    return;

                case 13:
                    ChangeReg(ref m_spectrum.CPU.regs.MW, "MW (Memptr Word)");
                    return;
            }
        }

        private void contextMenuDasm_Popup(object sender, EventArgs e)
        {
        }

        private bool dasmPanel_CheckExecuting(object Sender, ushort ADDR)
        {
            if (m_spectrum.IsRunning)
            {
                return false;
            }
            return (ADDR == m_spectrum.CPU.regs.PC);
        }

        private void dasmPanel_GetDasm(object Sender, ushort ADDR, out string DASM, out int len)
        {
            var str = m_dasmTool.GetMnemonic(ADDR, out len);
            var timingInfo = m_timingTool.GetTimingString(ADDR);
            DASM = string.Format("{0,-24} ; {1}", str, timingInfo);
        }

        private void dasmPanel_GetData(object Sender, ushort ADDR, int len, out byte[] data)
        {
            data = new byte[len];
            for (int i = 0; i < len; i++)
            {
                data[i] = m_spectrum.ReadMemory((ushort)(ADDR + i));
            }
        }

        private void dasmPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuDasm.Show(dasmPanel, e.Location);
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

        private void dasmPanel_SetBreakpoint(object sender, ushort addr)
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

        private void dataPanel_DataClick(object Sender, ushort Addr)
        {
            int num = m_spectrum.ReadMemory(Addr);
            var service = Locator.Resolve<IUserQuery>();
            if (service == null)
            {
                return;
            }
            if (service.QueryValue("POKE #" + Addr.ToString("X4"), "Value:", "#{0:X2}", ref num, 0, 0xff))
            {
                m_spectrum.WriteMemory(Addr, (byte)num);
                UpdateCPU(false);
            }
        }

        private void dataPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuData.Show(dataPanel, e.Location);
            }
        }

        private void FormCPU_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_spectrum.UpdateState -= new EventHandler(spectrum_OnUpdateState);
            m_spectrum.Breakpoint -= new EventHandler(spectrum_OnBreakpoint);
        }

        private void FormCPU_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F3:
                    if (!m_spectrum.IsRunning)
                    {
                        m_spectrum.DoReset();
                        UpdateCPU(true);
                        return;
                    }
                    return;

                case Keys.F4:
                case Keys.F6:
                    break;

                case Keys.F5:
                    m_spectrum.DoStop();
                    UpdateCPU(true);
                    break;

                case Keys.F7:
                    if (!m_spectrum.IsRunning)
                    {
                        try
                        {
                            m_spectrum.DoStepInto();
                        }
                        catch (Exception exception)
                        {
                            Logger.Error(exception);
                            Locator.Resolve<IUserMessage>().ErrorDetails(exception);
                        }
                        UpdateCPU(true);
                        return;
                    }
                    return;

                case Keys.F8:
                    if (!m_spectrum.IsRunning)
                    {
                        try
                        {
                            m_spectrum.DoStepOver();
                        }
                        catch (Exception exception2)
                        {
                            Logger.Error(exception2);
                            Locator.Resolve<IUserMessage>().ErrorDetails(exception2);
                        }
                        UpdateCPU(true);
                        return;
                    }
                    return;

                case Keys.F9:
                    m_spectrum.DoRun();
                    UpdateCPU(false);
                    return;
                case Keys.F10:
                    sprint_ula.UpdateFrame();
                    return;

                default:
                    return;
            }
        }

        private void FormCPU_Load(object sender, EventArgs e)
        {
            UpdateCPU(true);
            PentEvoRegs.DoubleClick += new EventHandler(PentEvoRegs_OnDBLClick);
        }

        private void FormCPU_Shown(object sender, EventArgs e)
        {
            base.Show();
            UpdateCPU(false);
            dasmPanel.Focus();
            base.Select();
        }

        private void Init(IDebuggable debugTarget)
        {
            if (debugTarget != m_spectrum)
            {
                if (m_spectrum != null)
                {
                    m_spectrum.UpdateState -= new EventHandler(spectrum_OnUpdateState);
                    m_spectrum.Breakpoint -= new EventHandler(spectrum_OnBreakpoint);
                }
                if (debugTarget != null)
                {
                    m_spectrum = debugTarget;
                    // ZEK +++
                    sprint_mmu = m_spectrum.Bus.FindDevice<SprinterMMU>();
                    sprint_ula = m_spectrum.Bus.FindDevice<SprinterULA>();
                    //pevo_bdi = m_spectrum.Bus.FindDevice(typeof(BDI)) as BDI;
                    // ZEK ---

                    m_dasmTool = new DasmTool(debugTarget.ReadMemory);
                    m_timingTool = new TimingTool(m_spectrum.CPU, debugTarget.ReadMemory);
                    m_spectrum.UpdateState += new EventHandler(spectrum_OnUpdateState);
                    m_spectrum.Breakpoint += new EventHandler(spectrum_OnBreakpoint);
                }
            }
        }

        private void listF_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if ((listF.SelectedIndex >= 0) && !m_spectrum.IsRunning)
            {
                m_spectrum.CPU.regs.F = (byte)(m_spectrum.CPU.regs.F ^ ((byte)(((int)0x80) >> listF.SelectedIndex)));
                UpdateREGS();
            }
        }

        private void listREGS_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if ((listREGS.SelectedIndex >= 0) && !m_spectrum.IsRunning)
            {
                ChangeRegByIndex(listREGS.SelectedIndex);
            }
        }

        private void listState_DoubleClick(object sender, EventArgs e)
        {
            if ((listState.SelectedIndex >= 0) && !m_spectrum.IsRunning)
            {
                switch (listState.SelectedIndex)
                {
                    case 0:
                        m_spectrum.CPU.IFF1 = m_spectrum.CPU.IFF2 = !m_spectrum.CPU.IFF1;
                        break;

                    case 1:
                        m_spectrum.CPU.HALTED = !m_spectrum.CPU.HALTED;
                        break;

                    case 3:
                        {
                            var cpu = m_spectrum.CPU;
                            cpu.IM = (byte)(cpu.IM + 1);
                            if (m_spectrum.CPU.IM > 2)
                            {
                                m_spectrum.CPU.IM = 0;
                            }
                            break;
                        }
                    case 8:
                        {
                            int frameTact = m_spectrum.GetFrameTact();
                            var service = Locator.Resolve<IUserQuery>();
                            if (service == null)
                            {
                                break;
                            }
                            if (service.QueryValue("Frame Tact", "New Frame Tact:", "{0}", ref frameTact, 0, m_spectrum.FrameTactCount))
                            {
                                int num2 = frameTact - m_spectrum.GetFrameTact();
                                if (num2 < 0)
                                {
                                    num2 += m_spectrum.FrameTactCount;
                                }
                                var zcpu2 = m_spectrum.CPU;
                                zcpu2.Tact += num2;
                            }
                            break;
                        }
                }
                UpdateCPU(false);
            }
        }

        private void menuItemDasmClearBP_Click(object sender, EventArgs e)
        {
            m_spectrum.ClearBreakpoints();
            UpdateCPU(false);
        }

        private void menuItemDasmGotoADDR_Click(object sender, EventArgs e)
        {
            int num = 0;
            var service = Locator.Resolve<IUserQuery>();
            if (service == null)
            {
                return;
            }
            if (service.QueryValue("Disassembly Address", "New Address:", "#{0:X4}", ref num, 0, 0xffff))
            {
                dasmPanel.TopAddress = (ushort)num;
            }
        }

        private void menuItemDasmGotoPC_Click(object sender, EventArgs e)
        {
            dasmPanel.ActiveAddress = m_spectrum.CPU.regs.PC;
            dasmPanel.UpdateLines();
            Refresh();
        }

        private void menuItemDasmRefresh_Click(object sender, EventArgs e)
        {
            dasmPanel.UpdateLines();
            Refresh();
        }

        private void menuItemDataGotoADDR_Click(object sender, EventArgs e)
        {
            int topAddress = dataPanel.TopAddress;
            var service = Locator.Resolve<IUserQuery>();
            if (service == null)
            {
                return;
            }
            if (service.QueryValue("Data Panel Address", "New Address:", "#{0:X4}", ref topAddress, 0, 0xffff))
            {
                dataPanel.TopAddress = (ushort)topAddress;
            }
        }

        private void menuItemDataRefresh_Click(object sender, EventArgs e)
        {
            dataPanel.UpdateLines();
            Refresh();
        }

        private void menuItemDataSetColumnCount_Click(object sender, EventArgs e)
        {
            int colCount = dataPanel.ColCount;
            var service = Locator.Resolve<IUserQuery>();
            if (service == null)
            {
                return;
            }
            if (service.QueryValue("Data Panel Columns", "Column Count:", "{0}", ref colCount, 1, 0x20))
            {
                dataPanel.ColCount = colCount;
            }
        }

        private void menuLoadBlock_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = ".";
            dialog.SupportMultiDottedExtensions = true;
            dialog.Title = "Load Block...";
            dialog.Filter = "All files (*.*)|*.*";
            dialog.DefaultExt = "";
            dialog.FileName = "";
            dialog.ShowReadOnly = false;
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FileInfo info = new FileInfo(dialog.FileName);
                s_len = (int)info.Length;
                var service = Locator.Resolve<IUserQuery>();
                if (((s_len >= 1) && 
                    service.QueryValue("Load Block", "Memory Address:", "#{0:X4}", ref s_addr, 0, 0xffff)) &&
                    service.QueryValue("Load Block", "Block Length:", "#{0:X4}", ref s_len, 0, 0x10000))
                {
                    byte[] buffer = new byte[s_len];
                    using (FileStream stream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        stream.Read(buffer, 0, buffer.Length);
                    }
                    m_spectrum.WriteMemory((ushort)s_addr, buffer, 0, s_len);
                }
            }
        }

        private void menuSaveBlock_Click(object sender, EventArgs e)
        {
            var service = Locator.Resolve<IUserQuery>();
            if (service.QueryValue("Save Block", "Memory Address:", "#{0:X4}", ref s_addr, 0, 0xffff) &&
                service.QueryValue("Save Block", "Block Length:", "#{0:X4}", ref s_len, 0, 0x10000))
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.InitialDirectory = ".";
                dialog.SupportMultiDottedExtensions = true;
                dialog.Title = "Save Block...";
                dialog.Filter = "Binary Files (*.bin)|*.bin|All files (*.*)|*.*";
                dialog.DefaultExt = "";
                dialog.FileName = "";
                dialog.OverwritePrompt = true;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    byte[] data = new byte[s_len];
                    m_spectrum.ReadMemory((ushort)s_addr, data, 0, s_len);
                    using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
            }

            /*      Save Video RAM
                        SaveFileDialog dialog = new SaveFileDialog();
                        dialog.InitialDirectory = ".";
                        dialog.SupportMultiDottedExtensions = true;
                        dialog.Title = "Save Block...";
                        dialog.Filter = "Binary Files (*.bin)|*.bin|All files (*.*)|*.*";
                        dialog.DefaultExt = "";
                        dialog.FileName = "";
                        dialog.OverwritePrompt = true;
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create, FileAccess.Write, FileShare.Read))
                            {
                                for (int i = 0; i < sprint_mmu.VRamPages.Length; i++)
                                stream.Write(sprint_mmu.VRamPages[i], 0, 0x4000);
                            }
                        }*/
        }

        private void spectrum_OnBreakpoint(object sender, EventArgs args)
        {
            if (!base.Created)
                return;
            BeginInvoke(new Action(() =>
            {
                base.Show();
                UpdateCPU(true);
                dasmPanel.Focus();
                base.Select();
            }), null);
        }

        private void spectrum_OnUpdateState(object sender, EventArgs args)
        {
            if (!base.Created)
                return;
            BeginInvoke(new Action(() => UpdateCPU(true)), null);
        }

        private void UpdateCPU(bool updatePC)
        {
            if (m_spectrum.IsRunning)
            {
                updatePC = false;
            }
            dasmPanel.ForeColor = m_spectrum.IsRunning ? SystemColors.ControlDarkDark : SystemColors.ControlText;
            UpdateREGS();
            UpdateDASM(updatePC);
            UpdateDATA();
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
            listF.Items.Add(" F3 = " + (((m_spectrum.CPU.regs.F & 8) != 0) ? "1" : "0"));
            listF.Items.Add("P/V = " + (((m_spectrum.CPU.regs.F & 4) != 0) ? "1" : "0"));
            listF.Items.Add("  N = " + (((m_spectrum.CPU.regs.F & 2) != 0) ? "1" : "0"));
            listF.Items.Add("  C = " + (((m_spectrum.CPU.regs.F & 1) != 0) ? "1" : "0"));
            listState.Items.Clear();
            listState.Items.Add("IFF1=" + (m_spectrum.CPU.IFF1 ? "1" : "0") + " IFF2=" + (m_spectrum.CPU.IFF2 ? "1" : "0"));
            listState.Items.Add("HALT=" + (m_spectrum.CPU.HALTED ? "1" : "0"));
            listState.Items.Add("BINT=" + (m_spectrum.CPU.BINT ? "1" : "0"));
            listState.Items.Add("  IM=" + m_spectrum.CPU.IM.ToString());
            listState.Items.Add("  FX=" + m_spectrum.CPU.FX.ToString());
            listState.Items.Add(" XFX=" + m_spectrum.CPU.XFX.ToString());
            listState.Items.Add(" LPC=#" + m_spectrum.CPU.LPC.ToString("X4"));
            listState.Items.Add("Tact=" + m_spectrum.CPU.Tact.ToString());
            listState.Items.Add("frmT=" + m_spectrum.GetFrameTact().ToString());


            // ZEK +++
            PentEvoRegs.Items.Clear();
            OutInfo("Variables----------");
            OutInfo("  SYS:         " + ((sprint_mmu.SYS) ? " On" : "Off"));
            OutInfo("  RA16:        " + ((sprint_mmu.RA16) ? " On" : "Off"));

            OutInfo("");

            OutInfo("Port XX7C/XX3C-----");
            OutBit("D0(ON/OFF)", sprint_mmu.SYSPORT, 0);
            OutBit("D1(TRB/ExpROM)", sprint_mmu.SYSPORT, 1);
            OutBit("D2(DCP0)", sprint_mmu.SYSPORT, 2);
            OutBit("D3(DCP1)", sprint_mmu.SYSPORT, 3);
            OutBit("D4(DCP2)", sprint_mmu.SYSPORT, 4);
            OutBit("D7(P128 ON)", sprint_mmu.SYSPORT, 7);

            OutInfo("");

            OutInfo("Port 89(RGADR): " + String.Format("#{0:X2}", sprint_mmu.RGADR));
            OutInfo("Port C9(RGMOD): " + String.Format("#{0:X2}", sprint_mmu.RGMOD));
            OutInfo("");
            OutInfo("Port 82(PAGE0): " + String.Format("#{0:X2}", sprint_mmu.PAGE0));
            OutInfo("Port A2(PAGE1): " + String.Format("#{0:X2}", sprint_mmu.PAGE1));
            OutInfo("Port C2(PAGE2): " + String.Format("#{0:X2}", sprint_mmu.PAGE2));
            OutInfo("Port E2(PAGE3): " + String.Format("#{0:X2}", sprint_mmu.PAGE3));

            OutInfo("Port 7FFD:      " + String.Format("#{0:X2}", sprint_mmu.CMR0));
            OutInfo("Port 1FFD:      " + String.Format("#{0:X2}", sprint_mmu.CMR1));

            OutInfo("");


            //            OutBit("D0(ON/OFF)", sprint_mmu.SYSPORT, 0);

            /*            // xx77
                        OutInfo("Port XX77---------");
                        OutBit("A14(ENA_PAL)", sprint_mmu.AFF77, 6);
                        OutBit("A8(DIS_MMU)", sprint_mmu.AFF77, 0);
                        OutBit("A9(CPM)", sprint_mmu.AFF77, 1);
                        OutBit("D3(TURBO14)", sprint_mmu.PFF77, 3);

                        // xxBF
                        OutInfo("Port XXBF---------");
                        OutBit("D0(SHADOW)", sprint_mmu.PXXBF, 0);
                        OutBit("D1(WRFLASH)", sprint_mmu.PXXBF, 1);
                        OutBit("D2(FONTROM)", sprint_mmu.PXXBF, 2);

                        //7FFD
                        OutInfo("Port 7FFD---------");
                        OutBit("D3(SCREEN)", sprint_mmu.P7FFD, 3);
                        OutBit("D4(MEMMAP)", sprint_mmu.P7FFD, 4);

                        if (sprint_mmu.PEFF7_M128)
                            OutBit("D5(LOCK48)", sprint_mmu.P7FFD, 5);
                        */
            // ZEK ---
        }

        // ZEK +++

        void OutInfo(string msg)
        {
            PentEvoRegs.Items.Add(msg);
        }

        void OutBit(string name, byte val, byte bit)
        {
            OutInfo("  " + name + "=" + (((val & (1 << bit)) != 0) ? "1" : "0"));
        }

        // ZEK ---

        private void Update1FFD()
        {
            int num = sprint_mmu.CMR1;
            var service = Locator.Resolve<IUserQuery>();
            if (service.QueryValue("Value of 1FFD port", "New value:", "#{0:X2}", ref num, 0, 0xff))
            {
                sprint_mmu.CMR1 = (byte)num;
                //                dasmPanel.TopAddress = (ushort)num;
            }
        }

        private void Update7FFD()
        {
            int num = sprint_mmu.CMR0;
            var service = Locator.Resolve<IUserQuery>();
            if (service.QueryValue("Value of 7FFD port", "New value:", "#{0:X2}", ref num, 0, 0xff))
            {
                sprint_mmu.CMR0 = (byte)num;
                //                dasmPanel.TopAddress = (ushort)num;
            }
        }

        private void PentEvoRegs_OnDBLClick(object sender, EventArgs args)
        {
            switch (PentEvoRegs.SelectedIndex)
            {
                //SYS
                case 1: sprint_mmu.SYS = !sprint_mmu.SYS; break;
                //RA16
                case 2: sprint_mmu.RA16 = !sprint_mmu.RA16; break;
                //7FFD
                case 19: Update7FFD(); break;
                //1FFD
                case 20: Update1FFD(); break;
            }
            UpdateREGS();
            UpdateDASM(false);
            UpdateDATA();
        }

    }
}
