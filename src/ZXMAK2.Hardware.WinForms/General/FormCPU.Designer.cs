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
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.panelDasm = new System.Windows.Forms.Panel();
            this.contextMenuDasm = new System.Windows.Forms.ContextMenu();
            this.menuItemDasmGotoADDR = new System.Windows.Forms.MenuItem();
            this.menuItemDasmGotoPC = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItemDasmClearBreakpoints = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuDasmLoadBlock = new System.Windows.Forms.MenuItem();
            this.menuDasmSaveBlock = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItemDasmRefresh = new System.Windows.Forms.MenuItem();
            this.contextMenuData = new System.Windows.Forms.ContextMenu();
            this.menuItemDataGotoADDR = new System.Windows.Forms.MenuItem();
            this.menuItemDataSetColumnCount = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuDataLoadBlock = new System.Windows.Forms.MenuItem();
            this.menuDataSaveBlock = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItemDataRefresh = new System.Windows.Forms.MenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusTact = new System.Windows.Forms.ToolStripStatusLabel();
            this.dasmPanel = new ZXMAK2.Hardware.WinForms.General.DasmPanel();
            this.dataPanel = new ZXMAK2.Hardware.WinForms.General.DataPanel();
            this.panelStatus.SuspendLayout();
            this.panelState.SuspendLayout();
            this.panelRegs.SuspendLayout();
            this.panelMem.SuspendLayout();
            this.panelDasm.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelStatus
            // 
            this.panelStatus.Controls.Add(this.panelState);
            this.panelStatus.Controls.Add(this.splitter3);
            this.panelStatus.Controls.Add(this.panelRegs);
            this.panelStatus.Dock = System.Windows.Forms.DockStyle.Right;
            this.panelStatus.Location = new System.Drawing.Point(451, 0);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(168, 378);
            this.panelStatus.TabIndex = 0;
            // 
            // panelState
            // 
            this.panelState.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelState.Controls.Add(this.listState);
            this.panelState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelState.Location = new System.Drawing.Point(0, 224);
            this.panelState.Name = "panelState";
            this.panelState.Size = new System.Drawing.Size(168, 154);
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
            this.listState.Size = new System.Drawing.Size(164, 150);
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
            this.listF.BackColor = System.Drawing.SystemColors.ButtonFace;
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
            this.listREGS.BackColor = System.Drawing.SystemColors.ButtonFace;
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
            this.splitter1.Location = new System.Drawing.Point(448, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 378);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // panelMem
            // 
            this.panelMem.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelMem.Controls.Add(this.dataPanel);
            this.panelMem.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelMem.Location = new System.Drawing.Point(0, 256);
            this.panelMem.Name = "panelMem";
            this.panelMem.Size = new System.Drawing.Size(448, 122);
            this.panelMem.TabIndex = 2;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter2.Location = new System.Drawing.Point(0, 253);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(448, 3);
            this.splitter2.TabIndex = 3;
            this.splitter2.TabStop = false;
            // 
            // panelDasm
            // 
            this.panelDasm.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelDasm.Controls.Add(this.dasmPanel);
            this.panelDasm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelDasm.Location = new System.Drawing.Point(0, 0);
            this.panelDasm.Name = "panelDasm";
            this.panelDasm.Size = new System.Drawing.Size(448, 253);
            this.panelDasm.TabIndex = 4;
            // 
            // contextMenuDasm
            // 
            this.contextMenuDasm.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemDasmGotoADDR,
            this.menuItemDasmGotoPC,
            this.menuItem2,
            this.menuItemDasmClearBreakpoints,
            this.menuItem4,
            this.menuDasmLoadBlock,
            this.menuDasmSaveBlock,
            this.menuItem1,
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
            // menuDasmLoadBlock
            // 
            this.menuDasmLoadBlock.Index = 5;
            this.menuDasmLoadBlock.Text = "Load Block...";
            this.menuDasmLoadBlock.Click += new System.EventHandler(this.menuLoadBlock_Click);
            // 
            // menuDasmSaveBlock
            // 
            this.menuDasmSaveBlock.Index = 6;
            this.menuDasmSaveBlock.Text = "Save Block...";
            this.menuDasmSaveBlock.Click += new System.EventHandler(this.menuSaveBlock_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 7;
            this.menuItem1.Text = "-";
            // 
            // menuItemDasmRefresh
            // 
            this.menuItemDasmRefresh.Index = 8;
            this.menuItemDasmRefresh.Text = "Refresh";
            this.menuItemDasmRefresh.Click += new System.EventHandler(this.menuItemDasmRefresh_Click);
            // 
            // contextMenuData
            // 
            this.contextMenuData.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemDataGotoADDR,
            this.menuItemDataSetColumnCount,
            this.menuItem5,
            this.menuDataLoadBlock,
            this.menuDataSaveBlock,
            this.menuItem3,
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
            // menuDataLoadBlock
            // 
            this.menuDataLoadBlock.Index = 3;
            this.menuDataLoadBlock.Text = "Load Block...";
            this.menuDataLoadBlock.Click += new System.EventHandler(this.menuLoadBlock_Click);
            // 
            // menuDataSaveBlock
            // 
            this.menuDataSaveBlock.Index = 4;
            this.menuDataSaveBlock.Text = "Save Block...";
            this.menuDataSaveBlock.Click += new System.EventHandler(this.menuSaveBlock_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 5;
            this.menuItem3.Text = "-";
            // 
            // menuItemDataRefresh
            // 
            this.menuItemDataRefresh.Index = 6;
            this.menuItemDataRefresh.Text = "Refresh";
            this.menuItemDataRefresh.Click += new System.EventHandler(this.menuItemDataRefresh_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatus,
            this.toolStripStatusTact});
            this.statusStrip.Location = new System.Drawing.Point(0, 378);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(619, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 5;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(513, 17);
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
            this.dasmPanel.Size = new System.Drawing.Size(444, 249);
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
            // dataPanel
            // 
            this.dataPanel.ColCount = 8;
            this.dataPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataPanel.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.dataPanel.Location = new System.Drawing.Point(0, 0);
            this.dataPanel.Name = "dataPanel";
            this.dataPanel.Size = new System.Drawing.Size(444, 118);
            this.dataPanel.TabIndex = 0;
            this.dataPanel.Text = "dataPanel1";
            this.dataPanel.TopAddress = ((ushort)(0));
            this.dataPanel.GetData += new ZXMAK2.Hardware.WinForms.General.DataPanel.ONGETDATACPU(this.dasmPanel_GetData);
            this.dataPanel.DataClick += new ZXMAK2.Hardware.WinForms.General.DataPanel.ONCLICKCPU(this.dataPanel_DataClick);
            this.dataPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataPanel_MouseClick);
            // 
            // FormCpu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(619, 400);
            this.Controls.Add(this.panelDasm);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.panelMem);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panelStatus);
            this.Controls.Add(this.statusStrip);
            this.KeyPreview = true;
            this.Name = "FormCpu";
            this.ShowInTaskbar = false;
            this.Text = "Z80 CPU";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormCPU_FormClosed);
            this.Load += new System.EventHandler(this.FormCPU_Load);
            this.Shown += new System.EventHandler(this.FormCPU_Shown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormCPU_KeyDown);
            this.panelStatus.ResumeLayout(false);
            this.panelState.ResumeLayout(false);
            this.panelRegs.ResumeLayout(false);
            this.panelMem.ResumeLayout(false);
            this.panelDasm.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
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
      private System.Windows.Forms.MenuItem menuDasmLoadBlock;
      private System.Windows.Forms.MenuItem menuDasmSaveBlock;
      private System.Windows.Forms.MenuItem menuItem1;
      private System.Windows.Forms.MenuItem menuDataLoadBlock;
      private System.Windows.Forms.MenuItem menuDataSaveBlock;
      private System.Windows.Forms.MenuItem menuItem3;
      private System.Windows.Forms.StatusStrip statusStrip;
      private System.Windows.Forms.ToolStripStatusLabel toolStripStatus;
      private System.Windows.Forms.ToolStripStatusLabel toolStripStatusTact;
 
   }
}