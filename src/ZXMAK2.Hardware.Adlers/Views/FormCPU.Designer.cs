namespace ZXMAK2.Hardware.Adlers.Views
{
    partial class FormCpu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelStatus = new System.Windows.Forms.Panel();
            this.tabMenus = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panelState = new System.Windows.Forms.Panel();
            this.listState = new System.Windows.Forms.ListBox();
            this.panelRegs = new System.Windows.Forms.Panel();
            this.listREGS = new System.Windows.Forms.ListBox();
            this.listF = new System.Windows.Forms.ListBox();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.tabPageTrace = new System.Windows.Forms.TabPage();
            this.checkBoxTactCount = new System.Windows.Forms.CheckBox();
            this.checkBoxTraceArea = new System.Windows.Forms.CheckBox();
            this.buttonSetTraceFileName = new System.Windows.Forms.Button();
            this.textBoxTraceFileName = new System.Windows.Forms.TextBox();
            this.checkBoxTraceFileOut = new System.Windows.Forms.CheckBox();
            this.checkBoxShowConsole = new System.Windows.Forms.CheckBox();
            this.groupBoxTraceOptions = new System.Windows.Forms.GroupBox();
            this.textBoxJumpToAnAddress = new System.Windows.Forms.TextBox();
            this.checkBoxDetectJumpOnAddress = new System.Windows.Forms.CheckBox();
            this.checkBoxConditionalCalls = new System.Windows.Forms.CheckBox();
            this.textBoxOpcode = new System.Windows.Forms.TextBox();
            this.checkBoxOpcode = new System.Windows.Forms.CheckBox();
            this.checkBoxConditionalJumps = new System.Windows.Forms.CheckBox();
            this.checkBoxAllJumps = new System.Windows.Forms.CheckBox();
            this.listViewAdressRanges = new System.Windows.Forms.ListView();
            this.btnStopTrace = new System.Windows.Forms.Button();
            this.btnStartTrace = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBoxTrace = new System.Windows.Forms.GroupBox();
            this.checkBoxTraceAutoOpenLog = new System.Windows.Forms.CheckBox();
            this.groupBoxStartup = new System.Windows.Forms.GroupBox();
            this.checkBoxLoadConfig = new System.Windows.Forms.CheckBox();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panelMem = new System.Windows.Forms.Panel();
            this.dataPanel = new ZXMAK2.Hardware.Adlers.Views.CustomControls.DataPanel();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panelDasm = new System.Windows.Forms.Panel();
            this.dasmPanel = new ZXMAK2.Hardware.Adlers.Views.CustomControls.DasmPanel();
            this.contextMenuDasm = new System.Windows.Forms.ContextMenu();
            this.menuItemDasmGotoADDR = new System.Windows.Forms.MenuItem();
            this.menuItemDasmGotoPC = new System.Windows.Forms.MenuItem();
            this.menuItemDumpMemory = new System.Windows.Forms.MenuItem();
            this.menuItemDumpMemoryAtCurrentAddress = new System.Windows.Forms.MenuItem();
            this.menuItemFollowInDisassembler = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItemInsertBreakpointHere = new System.Windows.Forms.MenuItem();
            this.menuItemDasmClearBreakpoints = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuDasmLoadBlock = new System.Windows.Forms.MenuItem();
            this.menuDasmSaveBlock = new System.Windows.Forms.MenuItem();
            this.menuItemSaveDisassembly = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.menuItemCommentsSymbols = new System.Windows.Forms.MenuItem();
            this.menuItemInsertComment = new System.Windows.Forms.MenuItem();
            this.menuItem12 = new System.Windows.Forms.MenuItem();
            this.menuItemClearCurrentComment = new System.Windows.Forms.MenuItem();
            this.menuItemClearAllComments = new System.Windows.Forms.MenuItem();
            this.menuItem11 = new System.Windows.Forms.MenuItem();
            this.menuItemInsertNote = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.menuItemClearCurrentNote = new System.Windows.Forms.MenuItem();
            this.menuItemClearAllNotes = new System.Windows.Forms.MenuItem();
            this.menuItem13 = new System.Windows.Forms.MenuItem();
            this.menuItemLoadComments = new System.Windows.Forms.MenuItem();
            this.menuItemSaveComments = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItemDasmRefresh = new System.Windows.Forms.MenuItem();
            this.contextMenuData = new System.Windows.Forms.ContextMenu();
            this.menuItemDataGotoADDR = new System.Windows.Forms.MenuItem();
            this.menuItemFollowInDisassembly = new System.Windows.Forms.MenuItem();
            this.menuItemDataSetColumnCount = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuDataLoadBlock = new System.Windows.Forms.MenuItem();
            this.menuDataSaveBlock = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItemFindBytes = new System.Windows.Forms.MenuItem();
            this.menuItemFindBytesNext = new System.Windows.Forms.MenuItem();
            this.menuItemSaveAsBytes = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItemDataRefresh = new System.Windows.Forms.MenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dbgCmdLine = new ZXMAK2.Hardware.Adlers.Views.CustomControls.DebuggerCommandLine();
            this.contextMenuTraceAddrArea = new System.Windows.Forms.ContextMenu();
            this.menuItemAddNewTraceAddrArea = new System.Windows.Forms.MenuItem();
            this.menuItemUpdateTraceAddrArea = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.panelStatus.SuspendLayout();
            this.tabMenus.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panelState.SuspendLayout();
            this.panelRegs.SuspendLayout();
            this.tabPageTrace.SuspendLayout();
            this.groupBoxTraceOptions.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBoxTrace.SuspendLayout();
            this.groupBoxStartup.SuspendLayout();
            this.panelMem.SuspendLayout();
            this.panelDasm.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelStatus
            // 
            this.panelStatus.Controls.Add(this.tabMenus);
            this.panelStatus.Controls.Add(this.splitter3);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelStatus.Location = new System.Drawing.Point(458, 0);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(168, 415);
            this.panelStatus.TabIndex = 0;
            // 
            // tabMenus
            // 
            this.tabMenus.Controls.Add(this.tabPage1);
            this.tabMenus.Controls.Add(this.tabPageTrace);
            this.tabMenus.Controls.Add(this.tabPage3);
            this.tabMenus.Dock = System.Windows.Forms.DockStyle.Right;
            this.tabMenus.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.tabMenus.Location = new System.Drawing.Point(0, 3);
            this.tabMenus.Name = "tabMenus";
            this.tabMenus.SelectedIndex = 0;
            this.tabMenus.Size = new System.Drawing.Size(168, 412);
            this.tabMenus.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panelState);
            this.tabPage1.Controls.Add(this.panelRegs);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(160, 386);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "CPU";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panelState
            // 
            this.panelState.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelState.Controls.Add(this.listState);
            this.panelState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelState.Location = new System.Drawing.Point(3, 252);
            this.panelState.Name = "panelState";
            this.panelState.Size = new System.Drawing.Size(154, 131);
            this.panelState.TabIndex = 2;
            // 
            // listState
            // 
            this.listState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listState.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listState.FormattingEnabled = true;
            this.listState.IntegralHeight = false;
            this.listState.ItemHeight = 14;
            this.listState.Location = new System.Drawing.Point(0, 0);
            this.listState.Name = "listState";
            this.listState.Size = new System.Drawing.Size(150, 127);
            this.listState.TabIndex = 1;
            this.listState.DoubleClick += new System.EventHandler(this.listState_DoubleClick);
            // 
            // panelRegs
            // 
            this.panelRegs.Controls.Add(this.listREGS);
            this.panelRegs.Controls.Add(this.listF);
            this.panelRegs.Controls.Add(this.splitter4);
            this.panelRegs.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelRegs.Location = new System.Drawing.Point(3, 3);
            this.panelRegs.Name = "panelRegs";
            this.panelRegs.Size = new System.Drawing.Size(154, 249);
            this.panelRegs.TabIndex = 0;
            // 
            // listREGS
            // 
            this.listREGS.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.listREGS.Dock = System.Windows.Forms.DockStyle.Left;
            this.listREGS.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.listREGS.FormattingEnabled = true;
            this.listREGS.IntegralHeight = false;
            this.listREGS.ItemHeight = 15;
            this.listREGS.Items.AddRange(new object[] {
            " PC = 0000",
            " IR = 0000",
            " SP = 0000",
            " AF = 0000",
            " HL = 0000",
            " DE = 0000",
            " BC = 0000",
            " IX = 0000",
            " IY = 0000",
            "AF\' = 0000",
            "HL\' = 0000",
            "DE\' = 0000",
            "BC\' = 0000",
            " MW = 0000"});
            this.listREGS.Location = new System.Drawing.Point(3, 0);
            this.listREGS.Name = "listREGS";
            this.listREGS.Size = new System.Drawing.Size(80, 249);
            this.listREGS.TabIndex = 0;
            this.listREGS.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listREGS_MouseDoubleClick);
            // 
            // listF
            // 
            this.listF.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.listF.Dock = System.Windows.Forms.DockStyle.Right;
            this.listF.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listF.FormattingEnabled = true;
            this.listF.IntegralHeight = false;
            this.listF.ItemHeight = 15;
            this.listF.Items.AddRange(new object[] {
            "  S = 0",
            "  Z = 0",
            " F5 = 0",
            "  H = 1",
            " F3 = 0",
            "P/V = 0",
            "  N = 0",
            "  C = 0"});
            this.listF.Location = new System.Drawing.Point(82, 0);
            this.listF.Name = "listF";
            this.listF.Size = new System.Drawing.Size(72, 249);
            this.listF.TabIndex = 0;
            this.listF.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listF_MouseDoubleClick);
            // 
            // splitter4
            // 
            this.splitter4.Location = new System.Drawing.Point(0, 0);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(3, 249);
            this.splitter4.TabIndex = 1;
            this.splitter4.TabStop = false;
            // 
            // tabPageTrace
            // 
            this.tabPageTrace.Controls.Add(this.checkBoxTactCount);
            this.tabPageTrace.Controls.Add(this.checkBoxTraceArea);
            this.tabPageTrace.Controls.Add(this.buttonSetTraceFileName);
            this.tabPageTrace.Controls.Add(this.textBoxTraceFileName);
            this.tabPageTrace.Controls.Add(this.checkBoxTraceFileOut);
            this.tabPageTrace.Controls.Add(this.checkBoxShowConsole);
            this.tabPageTrace.Controls.Add(this.groupBoxTraceOptions);
            this.tabPageTrace.Controls.Add(this.listViewAdressRanges);
            this.tabPageTrace.Controls.Add(this.btnStopTrace);
            this.tabPageTrace.Controls.Add(this.btnStartTrace);
            this.tabPageTrace.Location = new System.Drawing.Point(4, 22);
            this.tabPageTrace.Name = "tabPageTrace";
            this.tabPageTrace.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTrace.Size = new System.Drawing.Size(160, 386);
            this.tabPageTrace.TabIndex = 1;
            this.tabPageTrace.Text = "Trace";
            this.tabPageTrace.UseVisualStyleBackColor = true;
            // 
            // checkBoxTactCount
            // 
            this.checkBoxTactCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxTactCount.AutoSize = true;
            this.checkBoxTactCount.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkBoxTactCount.Location = new System.Drawing.Point(7, 281);
            this.checkBoxTactCount.Name = "checkBoxTactCount";
            this.checkBoxTactCount.Size = new System.Drawing.Size(108, 16);
            this.checkBoxTactCount.TabIndex = 12;
            this.checkBoxTactCount.Text = "Tact counter";
            this.checkBoxTactCount.UseVisualStyleBackColor = true;
            // 
            // checkBoxTraceArea
            // 
            this.checkBoxTraceArea.AutoSize = true;
            this.checkBoxTraceArea.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkBoxTraceArea.Location = new System.Drawing.Point(8, 163);
            this.checkBoxTraceArea.Name = "checkBoxTraceArea";
            this.checkBoxTraceArea.Size = new System.Drawing.Size(122, 16);
            this.checkBoxTraceArea.TabIndex = 11;
            this.checkBoxTraceArea.Text = "Address range:";
            this.checkBoxTraceArea.UseVisualStyleBackColor = true;
            this.checkBoxTraceArea.CheckedChanged += new System.EventHandler(this.checkBoxTraceAddresses_CheckedChanged);
            // 
            // buttonSetTraceFileName
            // 
            this.buttonSetTraceFileName.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.buttonSetTraceFileName.Location = new System.Drawing.Point(110, 332);
            this.buttonSetTraceFileName.Name = "buttonSetTraceFileName";
            this.buttonSetTraceFileName.Size = new System.Drawing.Size(43, 23);
            this.buttonSetTraceFileName.TabIndex = 10;
            this.buttonSetTraceFileName.Text = "Open";
            this.buttonSetTraceFileName.UseVisualStyleBackColor = true;
            this.buttonSetTraceFileName.Click += new System.EventHandler(this.buttonSetTraceFileName_Click);
            // 
            // textBoxTraceFileName
            // 
            this.textBoxTraceFileName.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxTraceFileName.Location = new System.Drawing.Point(6, 334);
            this.textBoxTraceFileName.Name = "textBoxTraceFileName";
            this.textBoxTraceFileName.Size = new System.Drawing.Size(100, 19);
            this.textBoxTraceFileName.TabIndex = 9;
            this.textBoxTraceFileName.Text = "trace.log";
            // 
            // checkBoxTraceFileOut
            // 
            this.checkBoxTraceFileOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxTraceFileOut.AutoSize = true;
            this.checkBoxTraceFileOut.Checked = true;
            this.checkBoxTraceFileOut.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTraceFileOut.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkBoxTraceFileOut.Location = new System.Drawing.Point(7, 315);
            this.checkBoxTraceFileOut.Name = "checkBoxTraceFileOut";
            this.checkBoxTraceFileOut.Size = new System.Drawing.Size(122, 16);
            this.checkBoxTraceFileOut.TabIndex = 8;
            this.checkBoxTraceFileOut.Text = "Output to file";
            this.checkBoxTraceFileOut.UseVisualStyleBackColor = true;
            this.checkBoxTraceFileOut.CheckedChanged += new System.EventHandler(this.checkBoxTraceFileOut_CheckedChanged);
            // 
            // checkBoxShowConsole
            // 
            this.checkBoxShowConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxShowConsole.AutoSize = true;
            this.checkBoxShowConsole.Checked = true;
            this.checkBoxShowConsole.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxShowConsole.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkBoxShowConsole.Location = new System.Drawing.Point(7, 298);
            this.checkBoxShowConsole.Name = "checkBoxShowConsole";
            this.checkBoxShowConsole.Size = new System.Drawing.Size(122, 16);
            this.checkBoxShowConsole.TabIndex = 7;
            this.checkBoxShowConsole.Text = "Console output";
            this.checkBoxShowConsole.UseVisualStyleBackColor = true;
            // 
            // groupBoxTraceOptions
            // 
            this.groupBoxTraceOptions.Controls.Add(this.textBoxJumpToAnAddress);
            this.groupBoxTraceOptions.Controls.Add(this.checkBoxDetectJumpOnAddress);
            this.groupBoxTraceOptions.Controls.Add(this.checkBoxConditionalCalls);
            this.groupBoxTraceOptions.Controls.Add(this.textBoxOpcode);
            this.groupBoxTraceOptions.Controls.Add(this.checkBoxOpcode);
            this.groupBoxTraceOptions.Controls.Add(this.checkBoxConditionalJumps);
            this.groupBoxTraceOptions.Controls.Add(this.checkBoxAllJumps);
            this.groupBoxTraceOptions.Location = new System.Drawing.Point(5, 0);
            this.groupBoxTraceOptions.Name = "groupBoxTraceOptions";
            this.groupBoxTraceOptions.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBoxTraceOptions.Size = new System.Drawing.Size(150, 157);
            this.groupBoxTraceOptions.TabIndex = 6;
            this.groupBoxTraceOptions.TabStop = false;
            this.groupBoxTraceOptions.Text = "Trace filter";
            // 
            // textBoxJumpToAnAddress
            // 
            this.textBoxJumpToAnAddress.Enabled = false;
            this.textBoxJumpToAnAddress.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxJumpToAnAddress.Location = new System.Drawing.Point(22, 90);
            this.textBoxJumpToAnAddress.Name = "textBoxJumpToAnAddress";
            this.textBoxJumpToAnAddress.Size = new System.Drawing.Size(122, 19);
            this.textBoxJumpToAnAddress.TabIndex = 7;
            // 
            // checkBoxDetectJumpOnAddress
            // 
            this.checkBoxDetectJumpOnAddress.AutoSize = true;
            this.checkBoxDetectJumpOnAddress.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkBoxDetectJumpOnAddress.Location = new System.Drawing.Point(3, 74);
            this.checkBoxDetectJumpOnAddress.Name = "checkBoxDetectJumpOnAddress";
            this.checkBoxDetectJumpOnAddress.Size = new System.Drawing.Size(136, 16);
            this.checkBoxDetectJumpOnAddress.TabIndex = 6;
            this.checkBoxDetectJumpOnAddress.Text = "Jump to an addr.";
            this.checkBoxDetectJumpOnAddress.UseVisualStyleBackColor = true;
            this.checkBoxDetectJumpOnAddress.CheckedChanged += new System.EventHandler(this.checkBoxDetectJumpOnAddress_CheckedChanged);
            // 
            // checkBoxConditionalCalls
            // 
            this.checkBoxConditionalCalls.AutoSize = true;
            this.checkBoxConditionalCalls.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkBoxConditionalCalls.Location = new System.Drawing.Point(3, 55);
            this.checkBoxConditionalCalls.Name = "checkBoxConditionalCalls";
            this.checkBoxConditionalCalls.Size = new System.Drawing.Size(143, 16);
            this.checkBoxConditionalCalls.TabIndex = 5;
            this.checkBoxConditionalCalls.Text = "Conditional calls";
            this.checkBoxConditionalCalls.UseVisualStyleBackColor = true;
            this.checkBoxConditionalCalls.CheckedChanged += new System.EventHandler(this.checkBoxConditionalCalls_CheckedChanged);
            // 
            // textBoxOpcode
            // 
            this.textBoxOpcode.Enabled = false;
            this.textBoxOpcode.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.textBoxOpcode.Location = new System.Drawing.Point(22, 132);
            this.textBoxOpcode.Name = "textBoxOpcode";
            this.textBoxOpcode.Size = new System.Drawing.Size(122, 19);
            this.textBoxOpcode.TabIndex = 4;
            this.textBoxOpcode.Leave += new System.EventHandler(this.textBoxOpcode_Leave);
            // 
            // checkBoxOpcode
            // 
            this.checkBoxOpcode.AutoSize = true;
            this.checkBoxOpcode.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkBoxOpcode.Location = new System.Drawing.Point(3, 115);
            this.checkBoxOpcode.Name = "checkBoxOpcode";
            this.checkBoxOpcode.Size = new System.Drawing.Size(66, 16);
            this.checkBoxOpcode.TabIndex = 3;
            this.checkBoxOpcode.Text = "Opcode";
            this.checkBoxOpcode.UseVisualStyleBackColor = true;
            this.checkBoxOpcode.CheckedChanged += new System.EventHandler(this.checkBoxOpcode_CheckedChanged);
            // 
            // checkBoxConditionalJumps
            // 
            this.checkBoxConditionalJumps.AutoSize = true;
            this.checkBoxConditionalJumps.Checked = true;
            this.checkBoxConditionalJumps.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxConditionalJumps.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkBoxConditionalJumps.Location = new System.Drawing.Point(3, 37);
            this.checkBoxConditionalJumps.Name = "checkBoxConditionalJumps";
            this.checkBoxConditionalJumps.Size = new System.Drawing.Size(143, 16);
            this.checkBoxConditionalJumps.TabIndex = 1;
            this.checkBoxConditionalJumps.Text = "Conditional jumps";
            this.checkBoxConditionalJumps.UseVisualStyleBackColor = true;
            this.checkBoxConditionalJumps.CheckedChanged += new System.EventHandler(this.checkBoxConditionalJumps_CheckedChanged);
            // 
            // checkBoxAllJumps
            // 
            this.checkBoxAllJumps.AutoSize = true;
            this.checkBoxAllJumps.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.checkBoxAllJumps.Location = new System.Drawing.Point(3, 19);
            this.checkBoxAllJumps.Name = "checkBoxAllJumps";
            this.checkBoxAllJumps.Size = new System.Drawing.Size(129, 16);
            this.checkBoxAllJumps.TabIndex = 0;
            this.checkBoxAllJumps.Text = "All jumps/calls";
            this.checkBoxAllJumps.UseVisualStyleBackColor = true;
            this.checkBoxAllJumps.CheckedChanged += new System.EventHandler(this.checkBoxAllJumps_CheckedChanged);
            // 
            // listViewAdressRanges
            // 
            this.listViewAdressRanges.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewAdressRanges.Enabled = false;
            this.listViewAdressRanges.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.listViewAdressRanges.FullRowSelect = true;
            this.listViewAdressRanges.GridLines = true;
            this.listViewAdressRanges.Location = new System.Drawing.Point(5, 179);
            this.listViewAdressRanges.Name = "listViewAdressRanges";
            this.listViewAdressRanges.Size = new System.Drawing.Size(150, 97);
            this.listViewAdressRanges.TabIndex = 5;
            this.listViewAdressRanges.UseCompatibleStateImageBehavior = false;
            this.listViewAdressRanges.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewAdressRanges_KeyDown);
            this.listViewAdressRanges.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewAdressRanges_MouseClick);
            this.listViewAdressRanges.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewAdressRanges_MouseDoubleClick);
            this.listViewAdressRanges.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listViewAdressRanges_MouseDown);
            // 
            // btnStopTrace
            // 
            this.btnStopTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStopTrace.Enabled = false;
            this.btnStopTrace.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnStopTrace.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStopTrace.Location = new System.Drawing.Point(57, 359);
            this.btnStopTrace.Name = "btnStopTrace";
            this.btnStopTrace.Size = new System.Drawing.Size(70, 23);
            this.btnStopTrace.TabIndex = 4;
            this.btnStopTrace.Text = "Finish";
            this.btnStopTrace.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStopTrace.UseVisualStyleBackColor = true;
            this.btnStopTrace.Click += new System.EventHandler(this.btnStopTrace_Click);
            // 
            // btnStartTrace
            // 
            this.btnStartTrace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStartTrace.Font = new System.Drawing.Font("Lucida Console", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.btnStartTrace.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnStartTrace.Location = new System.Drawing.Point(5, 359);
            this.btnStartTrace.Name = "btnStartTrace";
            this.btnStartTrace.Size = new System.Drawing.Size(44, 23);
            this.btnStartTrace.TabIndex = 3;
            this.btnStartTrace.Text = "Go";
            this.btnStartTrace.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnStartTrace.UseVisualStyleBackColor = true;
            this.btnStartTrace.Click += new System.EventHandler(this.btnStartTrace_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBoxTrace);
            this.tabPage3.Controls.Add(this.groupBoxStartup);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(160, 386);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Misc.";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBoxTrace
            // 
            this.groupBoxTrace.Controls.Add(this.checkBoxTraceAutoOpenLog);
            this.groupBoxTrace.Location = new System.Drawing.Point(4, 111);
            this.groupBoxTrace.Name = "groupBoxTrace";
            this.groupBoxTrace.Size = new System.Drawing.Size(153, 100);
            this.groupBoxTrace.TabIndex = 1;
            this.groupBoxTrace.TabStop = false;
            this.groupBoxTrace.Text = "Trace";
            // 
            // checkBoxTraceAutoOpenLog
            // 
            this.checkBoxTraceAutoOpenLog.AutoSize = true;
            this.checkBoxTraceAutoOpenLog.Checked = true;
            this.checkBoxTraceAutoOpenLog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxTraceAutoOpenLog.Location = new System.Drawing.Point(7, 19);
            this.checkBoxTraceAutoOpenLog.Name = "checkBoxTraceAutoOpenLog";
            this.checkBoxTraceAutoOpenLog.Size = new System.Drawing.Size(115, 16);
            this.checkBoxTraceAutoOpenLog.TabIndex = 0;
            this.checkBoxTraceAutoOpenLog.Text = "Auto open log";
            this.checkBoxTraceAutoOpenLog.UseVisualStyleBackColor = true;
            // 
            // groupBoxStartup
            // 
            this.groupBoxStartup.Controls.Add(this.checkBoxLoadConfig);
            this.groupBoxStartup.Location = new System.Drawing.Point(4, 4);
            this.groupBoxStartup.Name = "groupBoxStartup";
            this.groupBoxStartup.Size = new System.Drawing.Size(153, 100);
            this.groupBoxStartup.TabIndex = 0;
            this.groupBoxStartup.TabStop = false;
            this.groupBoxStartup.Text = "Startup";
            // 
            // checkBoxLoadConfig
            // 
            this.checkBoxLoadConfig.AutoSize = true;
            this.checkBoxLoadConfig.Checked = true;
            this.checkBoxLoadConfig.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxLoadConfig.Location = new System.Drawing.Point(7, 19);
            this.checkBoxLoadConfig.Name = "checkBoxLoadConfig";
            this.checkBoxLoadConfig.Size = new System.Drawing.Size(101, 16);
            this.checkBoxLoadConfig.TabIndex = 0;
            this.checkBoxLoadConfig.Text = "Load config";
            this.checkBoxLoadConfig.UseVisualStyleBackColor = true;
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter3.Location = new System.Drawing.Point(0, 0);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(168, 3);
            this.splitter3.TabIndex = 1;
            this.splitter3.TabStop = false;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(455, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 415);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // panelMem
            // 
            this.panelMem.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelMem.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelMem.Controls.Add(this.dataPanel);
            this.panelMem.Location = new System.Drawing.Point(0, 280);
            this.panelMem.Name = "panelMem";
            this.panelMem.Size = new System.Drawing.Size(455, 121);
            this.panelMem.TabIndex = 2;
            // 
            // dataPanel
            // 
            this.dataPanel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.dataPanel.ColCount = 8;
            this.dataPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataPanel.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.dataPanel.Location = new System.Drawing.Point(0, 0);
            this.dataPanel.Name = "dataPanel";
            this.dataPanel.Size = new System.Drawing.Size(451, 117);
            this.dataPanel.TabIndex = 0;
            this.dataPanel.Text = "dataPanel1";
            this.dataPanel.TopAddress = ((ushort)(0));
            this.dataPanel.GetData += new ZXMAK2.Hardware.Adlers.Views.CustomControls.DataPanel.ONGETDATACPU(this.dasmPanel_GetData);
            this.dataPanel.DataClick += new ZXMAK2.Hardware.Adlers.Views.CustomControls.DataPanel.ONCLICKCPU(this.dataPanel_DataClick);
            this.dataPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataPanel_MouseClick);
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 412);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(455, 3);
            this.splitter2.TabIndex = 3;
            this.splitter2.TabStop = false;
            // 
            // panelDasm
            // 
            this.panelDasm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelDasm.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelDasm.Controls.Add(this.dasmPanel);
            this.panelDasm.Location = new System.Drawing.Point(0, 0);
            this.panelDasm.Name = "panelDasm";
            this.panelDasm.Size = new System.Drawing.Size(455, 280);
            this.panelDasm.TabIndex = 4;
            // 
            // dasmPanel
            // 
            this.dasmPanel.ActiveAddress = ((ushort)(0));
            this.dasmPanel.BackColor = System.Drawing.SystemColors.ControlText;
            this.dasmPanel.BreakpointColor = System.Drawing.Color.Red;
            this.dasmPanel.BreakpointForeColor = System.Drawing.Color.Black;
            this.dasmPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dasmPanel.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.dasmPanel.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dasmPanel.Location = new System.Drawing.Point(0, 0);
            this.dasmPanel.Name = "dasmPanel";
            this.dasmPanel.Size = new System.Drawing.Size(451, 276);
            this.dasmPanel.TabIndex = 0;
            this.dasmPanel.Text = "dasmPanel1";
            this.dasmPanel.TopAddress = ((ushort)(0));
            this.dasmPanel.CheckBreakpoint += new ZXMAK2.Hardware.Adlers.Views.CustomControls.DasmPanel.ONCHECKCPU(this.dasmPanel_CheckBreakpoint);
            this.dasmPanel.CheckExecuting += new ZXMAK2.Hardware.Adlers.Views.CustomControls.DasmPanel.ONCHECKCPU(this.dasmPanel_CheckExecuting);
            this.dasmPanel.GetData += new ZXMAK2.Hardware.Adlers.Views.CustomControls.DasmPanel.ONGETDATACPU(this.dasmPanel_GetData);
            this.dasmPanel.GetDasm += new ZXMAK2.Hardware.Adlers.Views.CustomControls.DasmPanel.ONGETDASMCPU(this.dasmPanel_GetDasm);
            this.dasmPanel.BreakpointClick += new ZXMAK2.Hardware.Adlers.Views.CustomControls.DasmPanel.ONCLICKCPU(this.dasmPanel_SetBreakpoint);
            this.dasmPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dasmPanel_MouseClick);
            this.dasmPanel.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dasmPanel_MouseDoubleClick);
            // 
            // contextMenuDasm
            // 
            this.contextMenuDasm.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemDasmGotoADDR,
            this.menuItemDasmGotoPC,
            this.menuItemDumpMemory,
            this.menuItemFollowInDisassembler,
            this.menuItem2,
            this.menuItemInsertBreakpointHere,
            this.menuItemDasmClearBreakpoints,
            this.menuItem4,
            this.menuDasmLoadBlock,
            this.menuDasmSaveBlock,
            this.menuItemSaveDisassembly,
            this.menuItem8,
            this.menuItemCommentsSymbols,
            this.menuItem6,
            this.menuItemDasmRefresh});
            this.contextMenuDasm.Popup += new System.EventHandler(this.contextMenuDasm_Popup);
            // 
            // menuItemDasmGotoADDR
            // 
            this.menuItemDasmGotoADDR.Index = 0;
            this.menuItemDasmGotoADDR.Shortcut = System.Windows.Forms.Shortcut.CtrlG;
            this.menuItemDasmGotoADDR.Text = "Goto address...";
            this.menuItemDasmGotoADDR.Click += new System.EventHandler(this.menuItemDasmGotoADDR_Click);
            // 
            // menuItemDasmGotoPC
            // 
            this.menuItemDasmGotoPC.Index = 1;
            this.menuItemDasmGotoPC.Text = "Goto PC";
            this.menuItemDasmGotoPC.Click += new System.EventHandler(this.menuItemDasmGotoPC_Click);
            // 
            // menuItemDumpMemory
            // 
            this.menuItemDumpMemory.Index = 2;
            this.menuItemDumpMemory.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemDumpMemoryAtCurrentAddress});
            this.menuItemDumpMemory.Text = "Dump memory at";
            // 
            // menuItemDumpMemoryAtCurrentAddress
            // 
            this.menuItemDumpMemoryAtCurrentAddress.Index = 0;
            this.menuItemDumpMemoryAtCurrentAddress.Text = "Current address";
            this.menuItemDumpMemoryAtCurrentAddress.Click += new System.EventHandler(this.menuItemDumpMemoryAtCurrentAddress_Click);
            // 
            // menuItemFollowInDisassembler
            // 
            this.menuItemFollowInDisassembler.Index = 3;
            this.menuItemFollowInDisassembler.Text = "Follow in disassembler";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 4;
            this.menuItem2.Text = "-";
            // 
            // menuItemInsertBreakpointHere
            // 
            this.menuItemInsertBreakpointHere.Index = 5;
            this.menuItemInsertBreakpointHere.Text = "Insert breakpoint here";
            this.menuItemInsertBreakpointHere.Click += new System.EventHandler(this.menuItemInsertBreakpointHere_Click);
            // 
            // menuItemDasmClearBreakpoints
            // 
            this.menuItemDasmClearBreakpoints.Index = 6;
            this.menuItemDasmClearBreakpoints.Text = "Reset panel breakpoints";
            this.menuItemDasmClearBreakpoints.Click += new System.EventHandler(this.menuItemDasmClearBP_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 7;
            this.menuItem4.Text = "-";
            // 
            // menuDasmLoadBlock
            // 
            this.menuDasmLoadBlock.Index = 8;
            this.menuDasmLoadBlock.Text = "Load Block...";
            this.menuDasmLoadBlock.Click += new System.EventHandler(this.menuLoadBlock_Click);
            // 
            // menuDasmSaveBlock
            // 
            this.menuDasmSaveBlock.Index = 9;
            this.menuDasmSaveBlock.Text = "Save Block...";
            this.menuDasmSaveBlock.Click += new System.EventHandler(this.menuSaveBlock_Click);
            // 
            // menuItemSaveDisassembly
            // 
            this.menuItemSaveDisassembly.Index = 10;
            this.menuItemSaveDisassembly.Text = "Save Disassembly";
            this.menuItemSaveDisassembly.Click += new System.EventHandler(this.menuItemSaveDisassembly_Click);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 11;
            this.menuItem8.Text = "-";
            // 
            // menuItemCommentsSymbols
            // 
            this.menuItemCommentsSymbols.Index = 12;
            this.menuItemCommentsSymbols.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemInsertComment,
            this.menuItem12,
            this.menuItem11,
            this.menuItemInsertNote,
            this.menuItem10,
            this.menuItem13,
            this.menuItemLoadComments,
            this.menuItemSaveComments});
            this.menuItemCommentsSymbols.Text = "Comments/Symbols";
            // 
            // menuItemInsertComment
            // 
            this.menuItemInsertComment.Index = 0;
            this.menuItemInsertComment.Shortcut = System.Windows.Forms.Shortcut.Ctrl1;
            this.menuItemInsertComment.Text = "Insert/Update comment";
            this.menuItemInsertComment.Click += new System.EventHandler(this.menuItemInsertComment_Click);
            // 
            // menuItem12
            // 
            this.menuItem12.Index = 1;
            this.menuItem12.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemClearCurrentComment,
            this.menuItemClearAllComments});
            this.menuItem12.Text = "Clear";
            // 
            // menuItemClearCurrentComment
            // 
            this.menuItemClearCurrentComment.Index = 0;
            this.menuItemClearCurrentComment.Text = "Current comment";
            this.menuItemClearCurrentComment.Click += new System.EventHandler(this.menuItemClearCurrentComment_Click);
            // 
            // menuItemClearAllComments
            // 
            this.menuItemClearAllComments.Index = 1;
            this.menuItemClearAllComments.Text = "All comments";
            this.menuItemClearAllComments.Click += new System.EventHandler(this.menuItemClearAllComments_Click);
            // 
            // menuItem11
            // 
            this.menuItem11.Index = 2;
            this.menuItem11.Text = "-";
            // 
            // menuItemInsertNote
            // 
            this.menuItemInsertNote.Index = 3;
            this.menuItemInsertNote.Shortcut = System.Windows.Forms.Shortcut.Ctrl2;
            this.menuItemInsertNote.Text = "Insert/Update note";
            this.menuItemInsertNote.Click += new System.EventHandler(this.menuItemInsertNote_Click);
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 4;
            this.menuItem10.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemClearCurrentNote,
            this.menuItemClearAllNotes});
            this.menuItem10.Text = "Clear";
            // 
            // menuItemClearCurrentNote
            // 
            this.menuItemClearCurrentNote.Index = 0;
            this.menuItemClearCurrentNote.Text = "Current note";
            this.menuItemClearCurrentNote.Click += new System.EventHandler(this.menuItemClearCurrentNote_Click);
            // 
            // menuItemClearAllNotes
            // 
            this.menuItemClearAllNotes.Index = 1;
            this.menuItemClearAllNotes.Text = "All notes";
            this.menuItemClearAllNotes.Click += new System.EventHandler(this.menuItemClearAllNotes_Click);
            // 
            // menuItem13
            // 
            this.menuItem13.Index = 5;
            this.menuItem13.Text = "-";
            // 
            // menuItemLoadComments
            // 
            this.menuItemLoadComments.Index = 6;
            this.menuItemLoadComments.Text = "Load comments/notes";
            this.menuItemLoadComments.Click += new System.EventHandler(this.menuItemLoadComments_Click);
            // 
            // menuItemSaveComments
            // 
            this.menuItemSaveComments.Index = 7;
            this.menuItemSaveComments.Text = "Save comments/notes";
            this.menuItemSaveComments.Click += new System.EventHandler(this.menuItemSaveComments_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 13;
            this.menuItem6.Text = "-";
            // 
            // menuItemDasmRefresh
            // 
            this.menuItemDasmRefresh.Index = 14;
            this.menuItemDasmRefresh.Text = "Refresh";
            this.menuItemDasmRefresh.Click += new System.EventHandler(this.menuItemDasmRefresh_Click);
            // 
            // contextMenuData
            // 
            this.contextMenuData.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemDataGotoADDR,
            this.menuItemFollowInDisassembly,
            this.menuItemDataSetColumnCount,
            this.menuItem5,
            this.menuDataLoadBlock,
            this.menuDataSaveBlock,
            this.menuItem3,
            this.menuItemFindBytes,
            this.menuItemFindBytesNext,
            this.menuItemSaveAsBytes,
            this.menuItem7,
            this.menuItemDataRefresh});
            // 
            // menuItemDataGotoADDR
            // 
            this.menuItemDataGotoADDR.Index = 0;
            this.menuItemDataGotoADDR.Shortcut = System.Windows.Forms.Shortcut.CtrlD;
            this.menuItemDataGotoADDR.Text = "Dump memory address...";
            this.menuItemDataGotoADDR.Click += new System.EventHandler(this.menuItemDataGotoADDR_Click);
            // 
            // menuItemFollowInDisassembly
            // 
            this.menuItemFollowInDisassembly.Index = 1;
            this.menuItemFollowInDisassembly.Text = "Follow in disassembly";
            this.menuItemFollowInDisassembly.Click += new System.EventHandler(this.menuItemFollowInDisassembly_Click);
            // 
            // menuItemDataSetColumnCount
            // 
            this.menuItemDataSetColumnCount.Index = 2;
            this.menuItemDataSetColumnCount.Text = "Set column count...";
            this.menuItemDataSetColumnCount.Click += new System.EventHandler(this.menuItemDataSetColumnCount_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 3;
            this.menuItem5.Text = "-";
            // 
            // menuDataLoadBlock
            // 
            this.menuDataLoadBlock.Index = 4;
            this.menuDataLoadBlock.Text = "Load Block...";
            this.menuDataLoadBlock.Click += new System.EventHandler(this.menuLoadBlock_Click);
            // 
            // menuDataSaveBlock
            // 
            this.menuDataSaveBlock.Index = 5;
            this.menuDataSaveBlock.Text = "Save Block...";
            this.menuDataSaveBlock.Click += new System.EventHandler(this.menuSaveBlock_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 6;
            this.menuItem3.Text = "-";
            // 
            // menuItemFindBytes
            // 
            this.menuItemFindBytes.Index = 7;
            this.menuItemFindBytes.Shortcut = System.Windows.Forms.Shortcut.CtrlF;
            this.menuItemFindBytes.Text = "Find bytes in memory";
            this.menuItemFindBytes.Click += new System.EventHandler(this.menuItemFindBytes_Click);
            // 
            // menuItemFindBytesNext
            // 
            this.menuItemFindBytesNext.Index = 8;
            this.menuItemFindBytesNext.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
            this.menuItemFindBytesNext.Text = "Find next";
            this.menuItemFindBytesNext.Click += new System.EventHandler(this.menuItemFindBytesNext_Click);
            // 
            // menuItemSaveAsBytes
            // 
            this.menuItemSaveAsBytes.Index = 9;
            this.menuItemSaveAsBytes.Text = "Save as bytes";
            this.menuItemSaveAsBytes.Click += new System.EventHandler(this.menuItemSaveAsBytes_Click);
            // 
            // menuItem7
            // 
            this.menuItem7.Index = 10;
            this.menuItem7.Text = "-";
            // 
            // menuItemDataRefresh
            // 
            this.menuItemDataRefresh.Index = 11;
            this.menuItemDataRefresh.Text = "Refresh";
            this.menuItemDataRefresh.Click += new System.EventHandler(this.menuItemDataRefresh_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.dbgCmdLine);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 389);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(455, 23);
            this.panel1.TabIndex = 5;
            // 
            // dbgCmdLine
            // 
            this.dbgCmdLine.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.dbgCmdLine.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.dbgCmdLine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dbgCmdLine.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.dbgCmdLine.Location = new System.Drawing.Point(0, 0);
            this.dbgCmdLine.Name = "dbgCmdLine";
            this.dbgCmdLine.Size = new System.Drawing.Size(451, 24);
            this.dbgCmdLine.TabIndex = 0;
            this.dbgCmdLine.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dbgCmdLine_KeyUp);
            // 
            // contextMenuTraceAddrArea
            // 
            this.contextMenuTraceAddrArea.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemAddNewTraceAddrArea,
            this.menuItemUpdateTraceAddrArea,
            this.menuItem9});
            // 
            // menuItemAddNewTraceAddrArea
            // 
            this.menuItemAddNewTraceAddrArea.Index = 0;
            this.menuItemAddNewTraceAddrArea.Text = "Add new area";
            this.menuItemAddNewTraceAddrArea.Click += new System.EventHandler(this.menuItemAddNewTraceAddrArea_Click);
            // 
            // menuItemUpdateTraceAddrArea
            // 
            this.menuItemUpdateTraceAddrArea.Index = 1;
            this.menuItemUpdateTraceAddrArea.Text = "Update current";
            this.menuItemUpdateTraceAddrArea.Click += new System.EventHandler(this.menuItemUpdateTraceAddrArea_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 2;
            this.menuItem9.Text = "Delete current";
            // 
            // FormCpu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Yellow;
            this.ClientSize = new System.Drawing.Size(626, 415);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelDasm);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.panelMem);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panelStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.KeyPreview = true;
            this.Name = "FormCpu";
            this.ShowInTaskbar = false;
            this.Text = "Z80 CPU";
            this.Activated += new System.EventHandler(this.FormCpu_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCpu_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormCPU_FormClosed);
            this.Load += new System.EventHandler(this.FormCPU_Load);
            this.Shown += new System.EventHandler(this.FormCPU_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormCPU_KeyDown);
            this.panelStatus.ResumeLayout(false);
            this.tabMenus.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panelState.ResumeLayout(false);
            this.panelRegs.ResumeLayout(false);
            this.tabPageTrace.ResumeLayout(false);
            this.tabPageTrace.PerformLayout();
            this.groupBoxTraceOptions.ResumeLayout(false);
            this.groupBoxTraceOptions.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.groupBoxTrace.ResumeLayout(false);
            this.groupBoxTrace.PerformLayout();
            this.groupBoxStartup.ResumeLayout(false);
            this.groupBoxStartup.PerformLayout();
            this.panelMem.ResumeLayout(false);
            this.panelDasm.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panelMem;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Panel panelDasm;
        private System.Windows.Forms.Panel panelRegs;
        private System.Windows.Forms.Panel panelState;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.ListBox listREGS;
        private System.Windows.Forms.ListBox listF;
        private System.Windows.Forms.Splitter splitter4;
        private System.Windows.Forms.ListBox listState;
        private ZXMAK2.Hardware.Adlers.Views.CustomControls.DasmPanel dasmPanel;
        private ZXMAK2.Hardware.Adlers.Views.CustomControls.DataPanel dataPanel;
        private System.Windows.Forms.ContextMenu contextMenuDasm;
        private System.Windows.Forms.MenuItem menuItemDasmGotoADDR;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem menuItemDasmClearBreakpoints;
        private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItemDasmRefresh;
        private System.Windows.Forms.MenuItem menuItemDasmGotoPC;
        private System.Windows.Forms.ContextMenu contextMenuData;
        private System.Windows.Forms.MenuItem menuItemDataGotoADDR;
        private System.Windows.Forms.MenuItem menuItemDataSetColumnCount;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem menuItemDataRefresh;
        private System.Windows.Forms.MenuItem menuDasmLoadBlock;
        private System.Windows.Forms.MenuItem menuDasmSaveBlock;
        private System.Windows.Forms.MenuItem menuDataLoadBlock;
        private System.Windows.Forms.MenuItem menuDataSaveBlock;
        private System.Windows.Forms.MenuItem menuItem3;
        private System.Windows.Forms.Panel panel1;
        private ZXMAK2.Hardware.Adlers.Views.CustomControls.DebuggerCommandLine dbgCmdLine;
        private System.Windows.Forms.MenuItem menuItem8;
        private System.Windows.Forms.MenuItem menuItemSaveDisassembly;
        private System.Windows.Forms.MenuItem menuItemSaveAsBytes;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem menuItemFindBytes;
        private System.Windows.Forms.MenuItem menuItemFindBytesNext;
        private System.Windows.Forms.MenuItem menuItemFollowInDisassembly;
        private System.Windows.Forms.MenuItem menuItemDumpMemory;
        private System.Windows.Forms.MenuItem menuItemDumpMemoryAtCurrentAddress;
        private System.Windows.Forms.TabControl tabMenus;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPageTrace;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button btnStartTrace;
        private System.Windows.Forms.Button btnStopTrace;
        private System.Windows.Forms.GroupBox groupBoxTraceOptions;
        private System.Windows.Forms.CheckBox checkBoxShowConsole;
        private System.Windows.Forms.CheckBox checkBoxTraceFileOut;
        private System.Windows.Forms.Button buttonSetTraceFileName;
        public System.Windows.Forms.CheckBox checkBoxAllJumps;
        public System.Windows.Forms.CheckBox checkBoxConditionalJumps;
        public System.Windows.Forms.ListView listViewAdressRanges;
        public System.Windows.Forms.CheckBox checkBoxTraceArea;
        private System.Windows.Forms.ContextMenu contextMenuTraceAddrArea;
        private System.Windows.Forms.MenuItem menuItemAddNewTraceAddrArea;
        private System.Windows.Forms.MenuItem menuItemUpdateTraceAddrArea;
        private System.Windows.Forms.MenuItem menuItem9;
        public System.Windows.Forms.CheckBox checkBoxConditionalCalls;
        public System.Windows.Forms.TextBox textBoxTraceFileName;
        private System.Windows.Forms.MenuItem menuItemFollowInDisassembler;
        public System.Windows.Forms.TextBox textBoxOpcode;
        public System.Windows.Forms.CheckBox checkBoxOpcode;
        private System.Windows.Forms.GroupBox groupBoxStartup;
        private System.Windows.Forms.CheckBox checkBoxLoadConfig;
        public System.Windows.Forms.CheckBox checkBoxDetectJumpOnAddress;
        public System.Windows.Forms.TextBox textBoxJumpToAnAddress;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.GroupBox groupBoxTrace;
        private System.Windows.Forms.CheckBox checkBoxTraceAutoOpenLog;
        private System.Windows.Forms.MenuItem menuItemCommentsSymbols;
        private System.Windows.Forms.MenuItem menuItemInsertComment;
        private System.Windows.Forms.MenuItem menuItem12;
        private System.Windows.Forms.MenuItem menuItemClearCurrentComment;
        private System.Windows.Forms.MenuItem menuItemClearAllComments;
        private System.Windows.Forms.MenuItem menuItemLoadComments;
        private System.Windows.Forms.MenuItem menuItemSaveComments;
        private System.Windows.Forms.MenuItem menuItem11;
        private System.Windows.Forms.MenuItem menuItemInsertBreakpointHere;
        private System.Windows.Forms.MenuItem menuItemInsertNote;
        private System.Windows.Forms.MenuItem menuItem13;
        private System.Windows.Forms.MenuItem menuItem10;
        private System.Windows.Forms.MenuItem menuItemClearCurrentNote;
        private System.Windows.Forms.MenuItem menuItemClearAllNotes;
        private System.Windows.Forms.CheckBox checkBoxTactCount;
    }
}