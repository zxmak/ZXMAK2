namespace ZXMAK2.Hardware.WinForms.General
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
            this.panelState = new System.Windows.Forms.Panel();
            this.listState = new System.Windows.Forms.ListBox();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.panelRegs = new System.Windows.Forms.Panel();
            this.listF = new System.Windows.Forms.ListBox();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.listREGS = new System.Windows.Forms.ListBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panelMem = new System.Windows.Forms.Panel();
            this.dataPanel = new ZXMAK2.Hardware.WinForms.General.DataPanel();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panelDasm = new System.Windows.Forms.Panel();
            this.dasmPanel = new ZXMAK2.Hardware.WinForms.General.DasmPanel();
            this.contextMenuDasm = new System.Windows.Forms.ContextMenu();
            this.menuItemDasmGotoADDR = new System.Windows.Forms.MenuItem();
            this.menuItemDasmGotoPC = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItemDasmClearBreakpoints = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItemDasmRefresh = new System.Windows.Forms.MenuItem();
            this.contextMenuData = new System.Windows.Forms.ContextMenu();
            this.menuItemDataGotoADDR = new System.Windows.Forms.MenuItem();
            this.menuItemDataSetColumnCount = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItemDataRefresh = new System.Windows.Forms.MenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusTact = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.menuFileSplitter = new System.Windows.Forms.ToolStripSeparator();
            this.menuFileClose = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDebug = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDebugContinue = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDebugBreak = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDebugSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuDebugStepInto = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDebugStepOver = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDebugStepOut = new System.Windows.Forms.ToolStripMenuItem();
            this.menuDebugSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.menuDebugShowNext = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new ZXMAK2.Host.WinForms.Controls.ToolStripEx();
            this.toolStripContinue = new System.Windows.Forms.ToolStripButton();
            this.toolStripBreak = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripStepInto = new System.Windows.Forms.ToolStripButton();
            this.toolStripStepOver = new System.Windows.Forms.ToolStripButton();
            this.toolStripStepOut = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripShowNext = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripBreakpoints = new System.Windows.Forms.ToolStripButton();
            this.panelStatus.SuspendLayout();
            this.panelState.SuspendLayout();
            this.panelRegs.SuspendLayout();
            this.panelMem.SuspendLayout();
            this.panelDasm.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelStatus
            // 
            this.panelStatus.Controls.Add(this.panelState);
            this.panelStatus.Controls.Add(this.splitter3);
            this.panelStatus.Controls.Add(this.panelRegs);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelStatus.Location = new System.Drawing.Point(456, 49);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(168, 371);
            this.panelStatus.TabIndex = 0;
            // 
            // panelState
            // 
            this.panelState.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelState.Controls.Add(this.listState);
            this.panelState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelState.Location = new System.Drawing.Point(0, 224);
            this.panelState.Name = "panelState";
            this.panelState.Size = new System.Drawing.Size(168, 147);
            this.panelState.TabIndex = 2;
            // 
            // listState
            // 
            this.listState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listState.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.listState.FormattingEnabled = true;
            this.listState.IntegralHeight = false;
            this.listState.ItemHeight = 14;
            this.listState.Location = new System.Drawing.Point(0, 0);
            this.listState.Name = "listState";
            this.listState.Size = new System.Drawing.Size(164, 143);
            this.listState.TabIndex = 3;
            this.listState.DoubleClick += new System.EventHandler(this.listState_DoubleClick);
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter3.Location = new System.Drawing.Point(0, 221);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(168, 3);
            this.splitter3.TabIndex = 1;
            this.splitter3.TabStop = false;
            // 
            // panelRegs
            // 
            this.panelRegs.BackColor = System.Drawing.Color.White;
            this.panelRegs.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelRegs.Controls.Add(this.listF);
            this.panelRegs.Controls.Add(this.splitter4);
            this.panelRegs.Controls.Add(this.listREGS);
            this.panelRegs.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelRegs.Location = new System.Drawing.Point(0, 0);
            this.panelRegs.Name = "panelRegs";
            this.panelRegs.Size = new System.Drawing.Size(168, 221);
            this.panelRegs.TabIndex = 0;
            // 
            // listF
            // 
            this.listF.Dock = System.Windows.Forms.DockStyle.Fill;
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
            this.listF.Location = new System.Drawing.Point(101, 0);
            this.listF.Name = "listF";
            this.listF.Size = new System.Drawing.Size(63, 217);
            this.listF.TabIndex = 2;
            this.listF.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listF_MouseDoubleClick);
            // 
            // splitter4
            // 
            this.splitter4.Location = new System.Drawing.Point(98, 0);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(3, 217);
            this.splitter4.TabIndex = 1;
            this.splitter4.TabStop = false;
            // 
            // listREGS
            // 
            this.listREGS.Dock = System.Windows.Forms.DockStyle.Left;
            this.listREGS.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
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
            this.listREGS.Location = new System.Drawing.Point(0, 0);
            this.listREGS.Name = "listREGS";
            this.listREGS.Size = new System.Drawing.Size(98, 217);
            this.listREGS.TabIndex = 1;
            this.listREGS.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listREGS_MouseDoubleClick);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(453, 49);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 371);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // panelMem
            // 
            this.panelMem.BackColor = System.Drawing.Color.White;
            this.panelMem.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelMem.Controls.Add(this.dataPanel);
            this.panelMem.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelMem.Location = new System.Drawing.Point(0, 298);
            this.panelMem.Name = "panelMem";
            this.panelMem.Size = new System.Drawing.Size(453, 122);
            this.panelMem.TabIndex = 2;
            // 
            // dataPanel
            // 
            this.dataPanel.ColCount = 8;
            this.dataPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataPanel.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dataPanel.Location = new System.Drawing.Point(0, 0);
            this.dataPanel.Name = "dataPanel";
            this.dataPanel.Size = new System.Drawing.Size(449, 118);
            this.dataPanel.TabIndex = 0;
            this.dataPanel.Text = "dataPanel1";
            this.dataPanel.TopAddress = ((ushort)(0));
            this.dataPanel.GetData += new ZXMAK2.Hardware.WinForms.General.DataPanel.ONGETDATACPU(this.dasmPanel_GetData);
            this.dataPanel.DataClick += new ZXMAK2.Hardware.WinForms.General.DataPanel.ONCLICKCPU(this.dataPanel_DataClick);
            this.dataPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataPanel_MouseClick);
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 295);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(453, 3);
            this.splitter2.TabIndex = 3;
            this.splitter2.TabStop = false;
            // 
            // panelDasm
            // 
            this.panelDasm.BackColor = System.Drawing.Color.White;
            this.panelDasm.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelDasm.Controls.Add(this.dasmPanel);
            this.panelDasm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDasm.Location = new System.Drawing.Point(0, 49);
            this.panelDasm.Name = "panelDasm";
            this.panelDasm.Size = new System.Drawing.Size(453, 246);
            this.panelDasm.TabIndex = 4;
            // 
            // dasmPanel
            // 
            this.dasmPanel.ActiveAddress = ((ushort)(0));
            this.dasmPanel.BreakpointColor = System.Drawing.Color.Red;
            this.dasmPanel.BreakpointForeColor = System.Drawing.Color.Black;
            this.dasmPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dasmPanel.Font = new System.Drawing.Font("Courier New", 9F);
            this.dasmPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.dasmPanel.Location = new System.Drawing.Point(0, 0);
            this.dasmPanel.Name = "dasmPanel";
            this.dasmPanel.Size = new System.Drawing.Size(449, 242);
            this.dasmPanel.TabIndex = 0;
            this.dasmPanel.Text = "dasmPanel1";
            this.dasmPanel.TopAddress = ((ushort)(0));
            this.dasmPanel.CheckBreakpoint += new ZXMAK2.Hardware.WinForms.General.DasmPanel.ONCHECKCPU(this.dasmPanel_CheckBreakpoint);
            this.dasmPanel.CheckExecuting += new ZXMAK2.Hardware.WinForms.General.DasmPanel.ONCHECKCPU(this.dasmPanel_CheckExecuting);
            this.dasmPanel.GetData += new ZXMAK2.Hardware.WinForms.General.DasmPanel.ONGETDATACPU(this.dasmPanel_GetData);
            this.dasmPanel.GetDasm += new ZXMAK2.Hardware.WinForms.General.DasmPanel.ONGETDASMCPU(this.dasmPanel_GetDasm);
            this.dasmPanel.BreakpointClick += new ZXMAK2.Hardware.WinForms.General.DasmPanel.ONCLICKCPU(this.dasmPanel_BreakpointClick);
            this.dasmPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dasmPanel_MouseClick);
            // 
            // contextMenuDasm
            // 
            this.contextMenuDasm.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemDasmGotoADDR,
            this.menuItemDasmGotoPC,
            this.menuItem2,
            this.menuItemDasmClearBreakpoints,
            this.menuItem4,
            this.menuItemDasmRefresh});
            this.contextMenuDasm.Popup += new System.EventHandler(this.contextMenuDasm_Popup);
            // 
            // menuItemDasmGotoADDR
            // 
            this.menuItemDasmGotoADDR.Index = 0;
            this.menuItemDasmGotoADDR.Text = "Goto address...";
            this.menuItemDasmGotoADDR.Click += new System.EventHandler(this.menuItemDasmGotoADDR_Click);
            // 
            // menuItemDasmGotoPC
            // 
            this.menuItemDasmGotoPC.Index = 1;
            this.menuItemDasmGotoPC.Text = "Goto PC";
            this.menuItemDasmGotoPC.Click += new System.EventHandler(this.menuItemDasmGotoPC_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 2;
            this.menuItem2.Text = "-";
            // 
            // menuItemDasmClearBreakpoints
            // 
            this.menuItemDasmClearBreakpoints.Index = 3;
            this.menuItemDasmClearBreakpoints.Text = "Reset breakpoints";
            this.menuItemDasmClearBreakpoints.Click += new System.EventHandler(this.menuItemDasmClearBP_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 4;
            this.menuItem4.Text = "-";
            // 
            // menuItemDasmRefresh
            // 
            this.menuItemDasmRefresh.Index = 5;
            this.menuItemDasmRefresh.Text = "Refresh";
            this.menuItemDasmRefresh.Click += new System.EventHandler(this.menuItemDasmRefresh_Click);
            // 
            // contextMenuData
            // 
            this.contextMenuData.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemDataGotoADDR,
            this.menuItemDataSetColumnCount,
            this.menuItem5,
            this.menuItemDataRefresh});
            // 
            // menuItemDataGotoADDR
            // 
            this.menuItemDataGotoADDR.Index = 0;
            this.menuItemDataGotoADDR.Text = "Goto Address...";
            this.menuItemDataGotoADDR.Click += new System.EventHandler(this.menuItemDataGotoADDR_Click);
            // 
            // menuItemDataSetColumnCount
            // 
            this.menuItemDataSetColumnCount.Index = 1;
            this.menuItemDataSetColumnCount.Text = "Set column count...";
            this.menuItemDataSetColumnCount.Click += new System.EventHandler(this.menuItemDataSetColumnCount_Click);
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 2;
            this.menuItem5.Text = "-";
            // 
            // menuItemDataRefresh
            // 
            this.menuItemDataRefresh.Index = 3;
            this.menuItemDataRefresh.Text = "Refresh";
            this.menuItemDataRefresh.Click += new System.EventHandler(this.menuItemDataRefresh_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatus,
            this.toolStripStatusTact});
            this.statusStrip.Location = new System.Drawing.Point(0, 420);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(624, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 5;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(518, 17);
            this.toolStripStatus.Spring = true;
            this.toolStripStatus.Text = "Ready";
            this.toolStripStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusTact
            // 
            this.toolStripStatusTact.DoubleClickEnabled = true;
            this.toolStripStatusTact.Name = "toolStripStatusTact";
            this.toolStripStatusTact.Size = new System.Drawing.Size(91, 17);
            this.toolStripStatusTact.Text = "T: 71680 / 71680";
            this.toolStripStatusTact.DoubleClick += new System.EventHandler(this.toolStripStatusTact_DoubleClick);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuDebug});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(624, 24);
            this.menuStrip.TabIndex = 6;
            this.menuStrip.Text = "menuStrip1";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFileLoad,
            this.menuFileSave,
            this.menuFileSplitter,
            this.menuFileClose});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(37, 20);
            this.menuFile.Text = "File";
            // 
            // menuFileLoad
            // 
            this.menuFileLoad.Name = "menuFileLoad";
            this.menuFileLoad.Size = new System.Drawing.Size(109, 22);
            this.menuFileLoad.Text = "Load...";
            this.menuFileLoad.Click += new System.EventHandler(this.menuFileLoad_Click);
            // 
            // menuFileSave
            // 
            this.menuFileSave.Name = "menuFileSave";
            this.menuFileSave.Size = new System.Drawing.Size(109, 22);
            this.menuFileSave.Text = "Save...";
            this.menuFileSave.Click += new System.EventHandler(this.menuFileSave_Click);
            // 
            // menuFileSplitter
            // 
            this.menuFileSplitter.Name = "menuFileSplitter";
            this.menuFileSplitter.Size = new System.Drawing.Size(106, 6);
            // 
            // menuFileClose
            // 
            this.menuFileClose.Name = "menuFileClose";
            this.menuFileClose.Size = new System.Drawing.Size(109, 22);
            this.menuFileClose.Text = "Close";
            this.menuFileClose.Click += new System.EventHandler(this.menuFileClose_Click);
            // 
            // menuDebug
            // 
            this.menuDebug.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuDebugContinue,
            this.menuDebugBreak,
            this.menuDebugSeparator1,
            this.menuDebugStepInto,
            this.menuDebugStepOver,
            this.menuDebugStepOut,
            this.menuDebugSeparator2,
            this.menuDebugShowNext});
            this.menuDebug.Name = "menuDebug";
            this.menuDebug.Size = new System.Drawing.Size(54, 20);
            this.menuDebug.Text = "Debug";
            this.menuDebug.Visible = false;
            // 
            // menuDebugContinue
            // 
            this.menuDebugContinue.Name = "menuDebugContinue";
            this.menuDebugContinue.Size = new System.Drawing.Size(190, 22);
            this.menuDebugContinue.Text = "Continue";
            // 
            // menuDebugBreak
            // 
            this.menuDebugBreak.Name = "menuDebugBreak";
            this.menuDebugBreak.Size = new System.Drawing.Size(190, 22);
            this.menuDebugBreak.Text = "Break";
            // 
            // menuDebugSeparator1
            // 
            this.menuDebugSeparator1.Name = "menuDebugSeparator1";
            this.menuDebugSeparator1.Size = new System.Drawing.Size(187, 6);
            // 
            // menuDebugStepInto
            // 
            this.menuDebugStepInto.Name = "menuDebugStepInto";
            this.menuDebugStepInto.Size = new System.Drawing.Size(190, 22);
            this.menuDebugStepInto.Text = "Step Into";
            // 
            // menuDebugStepOver
            // 
            this.menuDebugStepOver.Name = "menuDebugStepOver";
            this.menuDebugStepOver.Size = new System.Drawing.Size(190, 22);
            this.menuDebugStepOver.Text = "Step Over";
            // 
            // menuDebugStepOut
            // 
            this.menuDebugStepOut.Name = "menuDebugStepOut";
            this.menuDebugStepOut.Size = new System.Drawing.Size(190, 22);
            this.menuDebugStepOut.Text = "Step Out";
            // 
            // menuDebugSeparator2
            // 
            this.menuDebugSeparator2.Name = "menuDebugSeparator2";
            this.menuDebugSeparator2.Size = new System.Drawing.Size(187, 6);
            // 
            // menuDebugShowNext
            // 
            this.menuDebugShowNext.Name = "menuDebugShowNext";
            this.menuDebugShowNext.Size = new System.Drawing.Size(190, 22);
            this.menuDebugShowNext.Text = "Show Next Instruction";
            // 
            // toolStrip
            // 
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripContinue,
            this.toolStripBreak,
            this.toolStripSeparator1,
            this.toolStripStepInto,
            this.toolStripStepOver,
            this.toolStripStepOut,
            this.toolStripSeparator2,
            this.toolStripShowNext,
            this.toolStripSeparator3,
            this.toolStripBreakpoints});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(0);
            this.toolStrip.Size = new System.Drawing.Size(624, 25);
            this.toolStrip.TabIndex = 7;
            this.toolStrip.Text = "toolStripEx1";
            // 
            // toolStripContinue
            // 
            this.toolStripContinue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripContinue.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripContinue.Name = "toolStripContinue";
            this.toolStripContinue.Size = new System.Drawing.Size(23, 22);
            this.toolStripContinue.Text = "Continue";
            this.toolStripContinue.Click += new System.EventHandler(this.toolStripContinue_Click);
            // 
            // toolStripBreak
            // 
            this.toolStripBreak.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBreak.Enabled = false;
            this.toolStripBreak.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBreak.Name = "toolStripBreak";
            this.toolStripBreak.Size = new System.Drawing.Size(23, 22);
            this.toolStripBreak.Text = "Break";
            this.toolStripBreak.Click += new System.EventHandler(this.toolStripBreak_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripStepInto
            // 
            this.toolStripStepInto.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripStepInto.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripStepInto.Name = "toolStripStepInto";
            this.toolStripStepInto.Size = new System.Drawing.Size(23, 22);
            this.toolStripStepInto.Text = "Step Into";
            this.toolStripStepInto.Click += new System.EventHandler(this.toolStripStepInto_Click);
            // 
            // toolStripStepOver
            // 
            this.toolStripStepOver.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripStepOver.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripStepOver.Name = "toolStripStepOver";
            this.toolStripStepOver.Size = new System.Drawing.Size(23, 22);
            this.toolStripStepOver.Text = "Step Over";
            this.toolStripStepOver.Click += new System.EventHandler(this.toolStripStepOver_Click);
            // 
            // toolStripStepOut
            // 
            this.toolStripStepOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripStepOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripStepOut.Name = "toolStripStepOut";
            this.toolStripStepOut.Size = new System.Drawing.Size(23, 22);
            this.toolStripStepOut.Text = "Step Out";
            this.toolStripStepOut.Click += new System.EventHandler(this.toolStripStepOut_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripShowNext
            // 
            this.toolStripShowNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripShowNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripShowNext.Name = "toolStripShowNext";
            this.toolStripShowNext.Size = new System.Drawing.Size(23, 22);
            this.toolStripShowNext.Text = "Show Next Statement";
            this.toolStripShowNext.Click += new System.EventHandler(this.toolStripShowNext_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripBreakpoints
            // 
            this.toolStripBreakpoints.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBreakpoints.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripBreakpoints.Name = "toolStripBreakpoints";
            this.toolStripBreakpoints.Size = new System.Drawing.Size(23, 22);
            this.toolStripBreakpoints.Text = "Breakpoints";
            // 
            // FormCpu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 442);
            this.Controls.Add(this.panelDasm);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.panelMem);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.KeyPreview = true;
            this.Name = "FormCpu";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Z80 CPU";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormCPU_FormClosed);
            this.Shown += new System.EventHandler(this.FormCPU_Shown);
            this.VisibleChanged += new System.EventHandler(this.FormCpu_VisibleChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormCPU_KeyDown);
            this.panelStatus.ResumeLayout(false);
            this.panelState.ResumeLayout(false);
            this.panelRegs.ResumeLayout(false);
            this.panelMem.ResumeLayout(false);
            this.panelDasm.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
      private ZXMAK2.Hardware.WinForms.General.DasmPanel dasmPanel;
      private ZXMAK2.Hardware.WinForms.General.DataPanel dataPanel;
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
      private System.Windows.Forms.StatusStrip statusStrip;
      private System.Windows.Forms.ToolStripStatusLabel toolStripStatus;
      private System.Windows.Forms.ToolStripStatusLabel toolStripStatusTact;
      private System.Windows.Forms.MenuStrip menuStrip;
      private System.Windows.Forms.ToolStripMenuItem menuFile;
      private System.Windows.Forms.ToolStripMenuItem menuFileLoad;
      private System.Windows.Forms.ToolStripMenuItem menuFileSave;
      private System.Windows.Forms.ToolStripSeparator menuFileSplitter;
      private System.Windows.Forms.ToolStripMenuItem menuFileClose;
      private System.Windows.Forms.ToolStripMenuItem menuDebug;
      private System.Windows.Forms.ToolStripMenuItem menuDebugContinue;
      private System.Windows.Forms.ToolStripMenuItem menuDebugBreak;
      private System.Windows.Forms.ToolStripSeparator menuDebugSeparator1;
      private System.Windows.Forms.ToolStripMenuItem menuDebugStepInto;
      private System.Windows.Forms.ToolStripMenuItem menuDebugStepOver;
      private System.Windows.Forms.ToolStripMenuItem menuDebugStepOut;
      private System.Windows.Forms.ToolStripSeparator menuDebugSeparator2;
      private System.Windows.Forms.ToolStripMenuItem menuDebugShowNext;
      private Host.WinForms.Controls.ToolStripEx toolStrip;
      private System.Windows.Forms.ToolStripButton toolStripContinue;
      private System.Windows.Forms.ToolStripButton toolStripBreak;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
      private System.Windows.Forms.ToolStripButton toolStripStepInto;
      private System.Windows.Forms.ToolStripButton toolStripStepOver;
      private System.Windows.Forms.ToolStripButton toolStripStepOut;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
      private System.Windows.Forms.ToolStripButton toolStripShowNext;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
      private System.Windows.Forms.ToolStripButton toolStripBreakpoints;
 
   }
}