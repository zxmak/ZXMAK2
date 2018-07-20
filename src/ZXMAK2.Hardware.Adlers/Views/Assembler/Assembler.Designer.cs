using FastColoredTextBoxNS;

namespace ZXMAK2.Hardware.Adlers.Views.AssemblerView
{
    partial class Assembler
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Assembler));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("noname.asm");
            this.txtAsm = new FastColoredTextBoxNS.FastColoredTextBox();
            this.btnCompile = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.toolMenu = new System.Windows.Forms.ToolStrip();
            this.toolStripNewSource = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.compileToolStrip = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonReloadFile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.openFileStripButton = new System.Windows.Forms.ToolStripButton();
            this.saveFileStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStrip = new System.Windows.Forms.ToolStripButton();
            this.toolStripColors = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolCodeLibrary = new System.Windows.Forms.ToolStripButton();
            this.treeViewFiles = new System.Windows.Forms.TreeView();
            this.buttonClearAssemblerLog = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btnStopBackgroundProcess = new System.Windows.Forms.Button();
            this.progressBarBackgroundProcess = new ZXMAK2.Hardware.Adlers.Views.CustomControls.ProgressBarBackgroundProcess();
            this.richCompileMessages = new ZXMAK2.Hardware.Adlers.Views.CustomControls.EventLogger();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.ctxMenuAssemblerCommands = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnFormatCode = new System.Windows.Forms.ToolStripMenuItem();
            this.btnValidateCode = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.convertNumbersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toHexadecimalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toDecimalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toBinaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listViewSymbols = new ZXMAK2.Hardware.Adlers.Views.CustomControls.ListViewCustom();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtbxFileOutputPath = new System.Windows.Forms.TextBox();
            this.radioBtnMemoryOutput = new System.Windows.Forms.RadioButton();
            this.radioBtnTAPBASOutput = new System.Windows.Forms.RadioButton();
            this.ctxmenuSymbols = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addToDebuggerAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.txtAsm)).BeginInit();
            this.toolMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.ctxMenuAssemblerCommands.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.ctxmenuSymbols.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtAsm
            // 
            this.txtAsm.AutoCompleteBrackets = true;
            this.txtAsm.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.txtAsm.AutoIndentChars = false;
            this.txtAsm.AutoIndentCharsPatterns = "";
            this.txtAsm.AutoIndentExistingLines = false;
            this.txtAsm.AutoScrollMinSize = new System.Drawing.Size(27, 17);
            this.txtAsm.AutoSize = true;
            this.txtAsm.BackBrush = null;
            this.txtAsm.CharHeight = 17;
            this.txtAsm.CharWidth = 8;
            this.txtAsm.CommentPrefix = "";
            this.txtAsm.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtAsm.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.txtAsm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAsm.Font = new System.Drawing.Font("Consolas", 11F);
            this.txtAsm.IsReplaceMode = false;
            this.txtAsm.Location = new System.Drawing.Point(0, 0);
            this.txtAsm.Name = "txtAsm";
            this.txtAsm.Paddings = new System.Windows.Forms.Padding(0);
            this.txtAsm.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.txtAsm.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("txtAsm.ServiceColors")));
            this.txtAsm.Size = new System.Drawing.Size(542, 552);
            this.txtAsm.TabIndex = 0;
            this.txtAsm.WordWrapAutoIndent = false;
            this.txtAsm.Zoom = 100;
            this.txtAsm.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.txtAsm_TextChanged);
            this.txtAsm.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtAsm_MouseClick);
            // 
            // btnCompile
            // 
            this.btnCompile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCompile.Location = new System.Drawing.Point(728, 35);
            this.btnCompile.Name = "btnCompile";
            this.btnCompile.Size = new System.Drawing.Size(85, 23);
            this.btnCompile.TabIndex = 1;
            this.btnCompile.Text = "Compile";
            this.btnCompile.UseVisualStyleBackColor = true;
            this.btnCompile.Click += new System.EventHandler(this.btnCompile_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(728, 64);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(85, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // toolMenu
            // 
            this.toolMenu.AllowItemReorder = true;
            this.toolMenu.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.toolMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripNewSource,
            this.toolStripSeparator2,
            this.compileToolStrip,
            this.toolStripButtonReloadFile,
            this.toolStripSeparator1,
            this.openFileStripButton,
            this.saveFileStripButton,
            this.toolStripSeparator3,
            this.settingsToolStrip,
            this.toolStripColors,
            this.toolStripSeparator5,
            this.toolCodeLibrary});
            this.toolMenu.Location = new System.Drawing.Point(0, 0);
            this.toolMenu.Name = "toolMenu";
            this.toolMenu.Size = new System.Drawing.Size(868, 35);
            this.toolMenu.TabIndex = 7;
            this.toolMenu.Text = "toolStrip1";
            // 
            // toolStripNewSource
            // 
            this.toolStripNewSource.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripNewSource.Image = ((System.Drawing.Image)(resources.GetObject("toolStripNewSource.Image")));
            this.toolStripNewSource.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripNewSource.Name = "toolStripNewSource";
            this.toolStripNewSource.Size = new System.Drawing.Size(32, 32);
            this.toolStripNewSource.ToolTipText = "New assembler source";
            this.toolStripNewSource.Click += new System.EventHandler(this.toolStripNewSource_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 35);
            // 
            // compileToolStrip
            // 
            this.compileToolStrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.compileToolStrip.Image = ((System.Drawing.Image)(resources.GetObject("compileToolStrip.Image")));
            this.compileToolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.compileToolStrip.Name = "compileToolStrip";
            this.compileToolStrip.Size = new System.Drawing.Size(32, 32);
            this.compileToolStrip.Text = "Compile(F5)";
            this.compileToolStrip.Click += new System.EventHandler(this.compileToolStrip_Click);
            // 
            // toolStripButtonReloadFile
            // 
            this.toolStripButtonReloadFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonReloadFile.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonReloadFile.Image")));
            this.toolStripButtonReloadFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonReloadFile.Name = "toolStripButtonReloadFile";
            this.toolStripButtonReloadFile.Size = new System.Drawing.Size(32, 32);
            this.toolStripButtonReloadFile.Text = "Reload file";
            this.toolStripButtonReloadFile.Click += new System.EventHandler(this.toolStripButtonRefresh_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 35);
            // 
            // openFileStripButton
            // 
            this.openFileStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openFileStripButton.Image = ((System.Drawing.Image)(resources.GetObject("openFileStripButton.Image")));
            this.openFileStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openFileStripButton.Name = "openFileStripButton";
            this.openFileStripButton.Size = new System.Drawing.Size(32, 32);
            this.openFileStripButton.Text = "Open File(Ctrl+O)";
            this.openFileStripButton.Click += new System.EventHandler(this.openFileStripButton_Click);
            // 
            // saveFileStripButton
            // 
            this.saveFileStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveFileStripButton.Image = ((System.Drawing.Image)(resources.GetObject("saveFileStripButton.Image")));
            this.saveFileStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveFileStripButton.Name = "saveFileStripButton";
            this.saveFileStripButton.Size = new System.Drawing.Size(32, 32);
            this.saveFileStripButton.Text = "Save File(Ctrl+S)";
            this.saveFileStripButton.Click += new System.EventHandler(this.saveFileStripButton_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 35);
            // 
            // settingsToolStrip
            // 
            this.settingsToolStrip.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.settingsToolStrip.Image = ((System.Drawing.Image)(resources.GetObject("settingsToolStrip.Image")));
            this.settingsToolStrip.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.settingsToolStrip.Name = "settingsToolStrip";
            this.settingsToolStrip.Size = new System.Drawing.Size(32, 32);
            this.settingsToolStrip.Text = "Settings";
            this.settingsToolStrip.Click += new System.EventHandler(this.settingsToolStrip_Click);
            // 
            // toolStripColors
            // 
            this.toolStripColors.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripColors.Image = ((System.Drawing.Image)(resources.GetObject("toolStripColors.Image")));
            this.toolStripColors.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripColors.Name = "toolStripColors";
            this.toolStripColors.Size = new System.Drawing.Size(32, 32);
            this.toolStripColors.Text = "Select text color";
            this.toolStripColors.ToolTipText = "Colors";
            this.toolStripColors.Click += new System.EventHandler(this.toolStripColors_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 35);
            // 
            // toolCodeLibrary
            // 
            this.toolCodeLibrary.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolCodeLibrary.Image = ((System.Drawing.Image)(resources.GetObject("toolCodeLibrary.Image")));
            this.toolCodeLibrary.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolCodeLibrary.Name = "toolCodeLibrary";
            this.toolCodeLibrary.Size = new System.Drawing.Size(32, 32);
            this.toolCodeLibrary.Text = "toolStripButton1";
            this.toolCodeLibrary.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.toolCodeLibrary.ToolTipText = "Code Library(Includes)";
            this.toolCodeLibrary.Click += new System.EventHandler(this.toolCodeLibrary_Click);
            // 
            // treeViewFiles
            // 
            this.treeViewFiles.CheckBoxes = true;
            this.treeViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewFiles.Font = new System.Drawing.Font("Calibri", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.treeViewFiles.HideSelection = false;
            this.treeViewFiles.LabelEdit = true;
            this.treeViewFiles.Location = new System.Drawing.Point(3, 16);
            this.treeViewFiles.Name = "treeViewFiles";
            treeNode1.Name = "Node0";
            treeNode1.Tag = "0";
            treeNode1.Text = "noname.asm";
            treeNode1.ToolTipText = "not save assembler code";
            this.treeViewFiles.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
            this.treeViewFiles.ShowNodeToolTips = true;
            this.treeViewFiles.Size = new System.Drawing.Size(158, 389);
            this.treeViewFiles.TabIndex = 8;
            this.treeViewFiles.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeViewFiles_AfterLabelEdit);
            this.treeViewFiles.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewFiles_BeforeSelect);
            this.treeViewFiles.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewFiles_AfterSelect);
            this.treeViewFiles.KeyUp += new System.Windows.Forms.KeyEventHandler(this.treeViewFiles_KeyUp);
            // 
            // buttonClearAssemblerLog
            // 
            this.buttonClearAssemblerLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClearAssemblerLog.Location = new System.Drawing.Point(728, 597);
            this.buttonClearAssemblerLog.Name = "buttonClearAssemblerLog";
            this.buttonClearAssemblerLog.Size = new System.Drawing.Size(75, 23);
            this.buttonClearAssemblerLog.TabIndex = 10;
            this.buttonClearAssemblerLog.Text = "Clear log";
            this.buttonClearAssemblerLog.UseVisualStyleBackColor = true;
            this.buttonClearAssemblerLog.Click += new System.EventHandler(this.buttonClearAssemblerLog_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtAsm);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnStopBackgroundProcess);
            this.splitContainer1.Panel2.Controls.Add(this.progressBarBackgroundProcess);
            this.splitContainer1.Panel2.Controls.Add(this.richCompileMessages);
            this.splitContainer1.Size = new System.Drawing.Size(542, 692);
            this.splitContainer1.SplitterDistance = 552;
            this.splitContainer1.TabIndex = 11;
            // 
            // btnStopBackgroundProcess
            // 
            this.btnStopBackgroundProcess.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStopBackgroundProcess.Location = new System.Drawing.Point(526, 118);
            this.btnStopBackgroundProcess.Name = "btnStopBackgroundProcess";
            this.btnStopBackgroundProcess.Size = new System.Drawing.Size(16, 16);
            this.btnStopBackgroundProcess.TabIndex = 17;
            this.btnStopBackgroundProcess.UseVisualStyleBackColor = true;
            this.btnStopBackgroundProcess.Click += new System.EventHandler(this.btnStopBackgroundProcess_Click);
            // 
            // progressBarBackgroundProcess
            // 
            this.progressBarBackgroundProcess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarBackgroundProcess.Location = new System.Drawing.Point(1, 119);
            this.progressBarBackgroundProcess.Name = "progressBarBackgroundProcess";
            this.progressBarBackgroundProcess.Size = new System.Drawing.Size(522, 15);
            this.progressBarBackgroundProcess.TabIndex = 6;
            // 
            // richCompileMessages
            // 
            this.richCompileMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richCompileMessages.BackColor = System.Drawing.Color.Black;
            this.richCompileMessages.Font = new System.Drawing.Font("Consolas", 9F);
            this.richCompileMessages.ForeColor = System.Drawing.Color.Green;
            this.richCompileMessages.Location = new System.Drawing.Point(0, 2);
            this.richCompileMessages.Name = "richCompileMessages";
            this.richCompileMessages.ReadOnly = true;
            this.richCompileMessages.Size = new System.Drawing.Size(542, 118);
            this.richCompileMessages.TabIndex = 5;
            this.richCompileMessages.Text = "";
            this.richCompileMessages.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.richCompileMessages_MouseDoubleClick);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(0, 35);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 695);
            this.splitter1.TabIndex = 12;
            this.splitter1.TabStop = false;
            // 
            // ctxMenuAssemblerCommands
            // 
            this.ctxMenuAssemblerCommands.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFormatCode,
            this.btnValidateCode,
            this.toolStripMenuItem1,
            this.convertNumbersToolStripMenuItem});
            this.ctxMenuAssemblerCommands.Name = "ctxMenuAssemblerCommands";
            this.ctxMenuAssemblerCommands.Size = new System.Drawing.Size(158, 76);
            // 
            // btnFormatCode
            // 
            this.btnFormatCode.Name = "btnFormatCode";
            this.btnFormatCode.Size = new System.Drawing.Size(157, 22);
            this.btnFormatCode.Text = "Format Code";
            this.btnFormatCode.Click += new System.EventHandler(this.btnFormatCode_Click);
            // 
            // btnValidateCode
            // 
            this.btnValidateCode.Name = "btnValidateCode";
            this.btnValidateCode.Size = new System.Drawing.Size(157, 22);
            this.btnValidateCode.Text = "Validate Code";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(154, 6);
            // 
            // convertNumbersToolStripMenuItem
            // 
            this.convertNumbersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toHexadecimalToolStripMenuItem,
            this.toDecimalToolStripMenuItem,
            this.toBinaryToolStripMenuItem});
            this.convertNumbersToolStripMenuItem.Name = "convertNumbersToolStripMenuItem";
            this.convertNumbersToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.convertNumbersToolStripMenuItem.Text = "Convert numbers";
            // 
            // toHexadecimalToolStripMenuItem
            // 
            this.toHexadecimalToolStripMenuItem.Name = "toHexadecimalToolStripMenuItem";
            this.toHexadecimalToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.toHexadecimalToolStripMenuItem.Text = "to hexadecimal";
            this.toHexadecimalToolStripMenuItem.Click += new System.EventHandler(this.toHexadecimalToolStripMenuItem_Click);
            // 
            // toDecimalToolStripMenuItem
            // 
            this.toDecimalToolStripMenuItem.Name = "toDecimalToolStripMenuItem";
            this.toDecimalToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.toDecimalToolStripMenuItem.Text = "to decimal";
            // 
            // toBinaryToolStripMenuItem
            // 
            this.toBinaryToolStripMenuItem.Name = "toBinaryToolStripMenuItem";
            this.toBinaryToolStripMenuItem.Size = new System.Drawing.Size(146, 22);
            this.toBinaryToolStripMenuItem.Text = "to binary";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listViewSymbols);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(164, 280);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Symbols";
            // 
            // listViewSymbols
            // 
            this.listViewSymbols.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewSymbols.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.listViewSymbols.FullRowSelect = true;
            this.listViewSymbols.Location = new System.Drawing.Point(3, 16);
            this.listViewSymbols.Name = "listViewSymbols";
            this.listViewSymbols.Size = new System.Drawing.Size(158, 261);
            this.listViewSymbols.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewSymbols.TabIndex = 0;
            this.listViewSymbols.UseCompatibleStateImageBehavior = false;
            this.listViewSymbols.SizeChanged += new System.EventHandler(this.listViewSymbols_SizeChanged);
            this.listViewSymbols.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewSymbols_MouseClick);
            this.listViewSymbols.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewSymbols_MouseDoubleClick);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.Location = new System.Drawing.Point(12, 38);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.splitContainer3);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer2.Size = new System.Drawing.Size(710, 692);
            this.splitContainer2.SplitterDistance = 164;
            this.splitContainer2.TabIndex = 14;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.groupBox3);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer3.Size = new System.Drawing.Size(164, 692);
            this.splitContainer3.SplitterDistance = 408;
            this.splitContainer3.TabIndex = 15;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.treeViewFiles);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(164, 408);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Files:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtbxFileOutputPath);
            this.groupBox1.Controls.Add(this.radioBtnMemoryOutput);
            this.groupBox1.Controls.Add(this.radioBtnTAPBASOutput);
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(728, 93);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(122, 497);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Compile output:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(97, 15);
            this.label1.TabIndex = 19;
            this.label1.Text = "File output path:";
            // 
            // txtbxFileOutputPath
            // 
            this.txtbxFileOutputPath.Enabled = false;
            this.txtbxFileOutputPath.Location = new System.Drawing.Point(8, 79);
            this.txtbxFileOutputPath.Name = "txtbxFileOutputPath";
            this.txtbxFileOutputPath.Size = new System.Drawing.Size(110, 23);
            this.txtbxFileOutputPath.TabIndex = 18;
            // 
            // radioBtnMemoryOutput
            // 
            this.radioBtnMemoryOutput.AutoSize = true;
            this.radioBtnMemoryOutput.Checked = true;
            this.radioBtnMemoryOutput.Location = new System.Drawing.Point(7, 20);
            this.radioBtnMemoryOutput.Name = "radioBtnMemoryOutput";
            this.radioBtnMemoryOutput.Size = new System.Drawing.Size(70, 19);
            this.radioBtnMemoryOutput.TabIndex = 17;
            this.radioBtnMemoryOutput.TabStop = true;
            this.radioBtnMemoryOutput.Text = "Memory";
            this.radioBtnMemoryOutput.UseVisualStyleBackColor = true;
            this.radioBtnMemoryOutput.CheckedChanged += new System.EventHandler(this.radioBtnMemoryOutput_CheckedChanged);
            // 
            // radioBtnTAPBASOutput
            // 
            this.radioBtnTAPBASOutput.AutoSize = true;
            this.radioBtnTAPBASOutput.Location = new System.Drawing.Point(6, 43);
            this.radioBtnTAPBASOutput.Name = "radioBtnTAPBASOutput";
            this.radioBtnTAPBASOutput.Size = new System.Drawing.Size(104, 19);
            this.radioBtnTAPBASOutput.TabIndex = 16;
            this.radioBtnTAPBASOutput.Text = "TAP with Basic";
            this.radioBtnTAPBASOutput.UseVisualStyleBackColor = true;
            this.radioBtnTAPBASOutput.MouseClick += new System.Windows.Forms.MouseEventHandler(this.radioBtnTAPBASOutput_MouseClick);
            // 
            // ctxmenuSymbols
            // 
            this.ctxmenuSymbols.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addToDebuggerAsToolStripMenuItem});
            this.ctxmenuSymbols.Name = "ctxmenuSymbols";
            this.ctxmenuSymbols.Size = new System.Drawing.Size(170, 26);
            // 
            // addToDebuggerAsToolStripMenuItem
            // 
            this.addToDebuggerAsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.commentToolStripMenuItem,
            this.noteToolStripMenuItem});
            this.addToDebuggerAsToolStripMenuItem.Name = "addToDebuggerAsToolStripMenuItem";
            this.addToDebuggerAsToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.addToDebuggerAsToolStripMenuItem.Text = "Add to debugger as";
            // 
            // commentToolStripMenuItem
            // 
            this.commentToolStripMenuItem.Name = "commentToolStripMenuItem";
            this.commentToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.commentToolStripMenuItem.Text = "Comment";
            this.commentToolStripMenuItem.Click += new System.EventHandler(this.commentToolStripMenuItem_Click);
            // 
            // noteToolStripMenuItem
            // 
            this.noteToolStripMenuItem.Name = "noteToolStripMenuItem";
            this.noteToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.noteToolStripMenuItem.Text = "Note";
            this.noteToolStripMenuItem.Click += new System.EventHandler(this.noteToolStripMenuItem_Click);
            // 
            // Assembler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(868, 730);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.toolMenu);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.buttonClearAssemblerLog);
            this.Controls.Add(this.btnCompile);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "Assembler";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Assembler";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Assembler_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.assemblerForm_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.txtAsm)).EndInit();
            this.toolMenu.ResumeLayout(false);
            this.toolMenu.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ctxMenuAssemblerCommands.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ctxmenuSymbols.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FastColoredTextBoxNS.FastColoredTextBox txtAsm;
        private System.Windows.Forms.Button btnCompile;
        private System.Windows.Forms.Button btnClose;
        private ZXMAK2.Hardware.Adlers.Views.CustomControls.EventLogger richCompileMessages;
        private System.Windows.Forms.ToolStrip toolMenu;
        private System.Windows.Forms.ToolStripButton compileToolStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton settingsToolStrip;
        private System.Windows.Forms.ToolStripButton toolStripColors;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton openFileStripButton;
        private System.Windows.Forms.ToolStripButton saveFileStripButton;
        private System.Windows.Forms.ToolStripButton toolCodeLibrary;
        private System.Windows.Forms.TreeView treeViewFiles;
        private System.Windows.Forms.ToolStripButton toolStripButtonReloadFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.Button buttonClearAssemblerLog;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.ToolStripButton toolStripNewSource;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ContextMenuStrip ctxMenuAssemblerCommands;
        private System.Windows.Forms.ToolStripMenuItem btnFormatCode;
        private System.Windows.Forms.GroupBox groupBox2;
        private ZXMAK2.Hardware.Adlers.Views.CustomControls.ListViewCustom listViewSymbols;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.SplitContainer splitContainer3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ContextMenuStrip ctxmenuSymbols;
        private System.Windows.Forms.ToolStripMenuItem addToDebuggerAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem commentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnValidateCode;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private CustomControls.ProgressBarBackgroundProcess progressBarBackgroundProcess;
        private System.Windows.Forms.Button btnStopBackgroundProcess;
        private System.Windows.Forms.ToolStripMenuItem convertNumbersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toHexadecimalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toDecimalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toBinaryToolStripMenuItem;
        private System.Windows.Forms.RadioButton radioBtnMemoryOutput;
        private System.Windows.Forms.RadioButton radioBtnTAPBASOutput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtbxFileOutputPath;
    }
}