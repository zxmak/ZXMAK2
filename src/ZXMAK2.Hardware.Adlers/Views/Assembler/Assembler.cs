using System;
using System.Windows.Forms;
using System.Drawing;
using System.Text.RegularExpressions;
using System.IO;
using FastColoredTextBoxNS;
using System.Diagnostics;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Engine;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ZXMAK2.Hardware.Adlers.Views.CustomControls;
using ZXMAK2.Hardware.Adlers.Core;
using System.Text;

namespace ZXMAK2.Hardware.Adlers.Views.AssemblerView
{
    public partial class Assembler : Form
    {
        private byte _tabSpace = 16; //how many characters on tab

        //assembler sources array
        private int _actualAssemblerNodeIndex = 0;
        private Dictionary<int, AssemblerSourceInfo> _assemblerSources = new Dictionary<int, AssemblerSourceInfo>();
        private readonly string _strDefaultAsmSourceCode = "org 40000\n\nret\n";

        //private IDebuggable m_spectrum;
        private FormCpu m_debugger;
       
        //instance
        private static Assembler m_instance = null;

        //Z80 assembler resources(libscreen.asm, fonts, sounds, graphics, ...)
        Z80AsmResources _Z80AsmResourcesForm;

        private Assembler(FormCpu spectrum)
        {
            m_debugger = spectrum;

            InitializeComponent();

            //Init colors object
            AssemblerColorConfig.Init(this);

            //Symbols list view
            listViewSymbols.View = View.Details;
            listViewSymbols.Columns.Add("Name      ", 100, HorizontalAlignment.Center);
            listViewSymbols.Columns.Add("Addr ", 60, HorizontalAlignment.Left);

            txtAsm.DoCaretVisible();
            txtAsm.IsChanged = false;
            txtAsm.ClearUndo();

            txtAsm.SelectionLength = 0;
            txtAsm.SelectionStart = txtAsm.Text.Length + 1;

            //init syntax highlightning
            AssemblerColorConfig.GetInstance();

            //register assembler source(noname.asm), will have Id = 0
            treeViewFiles.Nodes[0].Tag = (int)0;
            txtAsm.InsertText(_strDefaultAsmSourceCode); //must be here; will be used as source code for the new added source code
            AddNewSource(new AssemblerSourceInfo("noname.asm", false), _strDefaultAsmSourceCode);

            this.KeyPreview = true;
            this.BringToFront();

            //print compiler version to form title
            double compilerVersion;
            if (Compiler.GetVersion(out compilerVersion) == 0)
                this.Text = "Assembler, " + String.Format("ver {0:0.##}", compilerVersion).Replace(',', '.');

            //progress bar
            this.btnStopBackgroundProcess.Image = global::ZXMAK2.Resources.ResourceImages.Stop_real_16x16;
            
            //Z80 assembler resources
            _Z80AsmResourcesForm = new Z80AsmResources(ref this.txtAsm);
        }

        public static void Show(FormCpu i_formCpu)
        {
            if (m_instance == null || m_instance.IsDisposed)
            {
                m_instance = new Assembler(i_formCpu);
                m_instance.LoadConfig();
                m_instance.ShowInTaskbar = true;
                m_instance.Show();
            }
            else
                m_instance.Show();

            m_instance.txtAsm.Focus();
        }

        public static Assembler GetInstance()
        {
            return m_instance;
        }

        private void compileToZ80()
        {
            int retCode = -1;

            if (txtAsm.Text.Trim().Length < 2)
            {
                this.richCompileMessages.AppendLog( "Nothing to compile...", LOG_LEVEL.Info, true/*log time*/);
                return;
            }

            if(ValidateCompile() == false)
                return;

            unsafe
            {
                //FixedBuffer fixedBuf = new FixedBuffer();
                bool fIsTapBasMode = radioBtnTAPBASOutput.Checked;

                //init IN data
                COMPILE_IN compileIn = new COMPILE_IN();
                compileIn.ResetValues();
                COMPILED_INFO compiled = new COMPILED_INFO();

                string asmToCompileOrFileName = (GetActualSourceInfo().IsFile() ? GetActualSourceInfo().GetFileNameToSave() : txtAsm.Text);
                byte[] arrSourceCode = ConvertRadix.ASCIIStringToBytes(txtAsm.Text);

                fixed (byte* pointerSourceCode = &arrSourceCode[0])
                {
                    //Source code pointer
                    compileIn.chrSourceCode = pointerSourceCode;
                    //Symbols
                    compileIn.cEmitSymbols = (byte)'Y'; //always generate source code symbols

                    //Compile method: TAP_BAS or BIN
                    if (fIsTapBasMode)
                    {
                        //TAP_BAS - check output file
                        string tapbasFileName = txtbxFileOutputPath.Text;
                        if (FileTools.IsPathCorrect(tapbasFileName) == false || tapbasFileName == String.Empty)
                        {
                            //incorrect output path entered
                            this.richCompileMessages.AppendLog("Output path is incorrect..Error!", LOG_LEVEL.Error, true);
                            return;
                        }
                    }

                    string errStringText = String.Empty;
                    this.richCompileMessages.AppendInfo("Compiling...\n", LOG_OPTIONS.NoHeaderOrFooter | LOG_OPTIONS.AddTime);

                    //COMPILE!
                    //fixed ( ... )
                    {
                        //Input is file ?
                        if (GetActualSourceInfo().IsFile())
                        {
                            byte[] arrFileIn = ConvertRadix.ASCIIStringToBytes(GetActualSourceInfo().GetFileNameToSave());
                            byte[] arrFileOut = fIsTapBasMode ? ConvertRadix.ASCIIStringToBytes(txtbxFileOutputPath.Text) : new byte[1];

                            compileIn.cfInputIsFile = (byte)'Y';

                            fixed(byte* pointerFileIn = &arrFileIn[0])
                            {
                                fixed (byte* pointerFileOut = &arrFileOut[0])
                                {
                                    compileIn.chrFileIn = pointerFileIn;

                                    if (fIsTapBasMode)
                                    {
                                        compileIn.chrFileOut = pointerFileOut;
                                        compileIn.cCompileMode = Compiler.COMPILE_OPTION_TAP_BAS;
                                    }
                                    else
                                        compileIn.cCompileMode = Compiler.COMPILE_OPTION_BIN_FILE;

                                    //COMPILE(input is file) = TAP_BAS(input is file) or BIN_FILE mode!!!
                                    try
                                    {
                                        retCode = Compiler.DoCompile(compileIn, ref compiled);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                        Locator.Resolve<IUserMessage>().Error("Technical error in compilation...\nSorry, compilation cannot be executed.\n\nDetail:\n" + ex.Message);
                                        return;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (fIsTapBasMode == false)
                            {
                                //COMPILE(input is source code) = BIN mode!!!
                                compileIn.cCompileMode = Compiler.COMPILE_OPTION_BIN;
                                try
                                {
                                    retCode = Compiler.DoCompile(compileIn, ref compiled);
                                }
                                catch (Exception ex)
                                {
                                    Logger.Error(ex);
                                    Locator.Resolve<IUserMessage>().Error("Technical error in compilation...\nSorry, compilation cannot be executed.\n\nDetail:\n" + ex.Message);
                                    return;
                                }
                            }
                            else
                            {   
                                //COMPILE TAP_BAS mode with source code as input!!!
                                byte[] arrFileOut = ConvertRadix.ASCIIStringToBytes(txtbxFileOutputPath.Text);
                                fixed (byte* pointerFileOut = &arrFileOut[0])
                                {
                                    compileIn.chrFileOut = pointerFileOut;
                                    compileIn.cCompileMode = Compiler.COMPILE_OPTION_TAP_BAS;

                                    try
                                    {
                                        retCode = Compiler.DoCompile(compileIn, ref compiled);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error(ex);
                                        Locator.Resolve<IUserMessage>().Error("Technical error in compilation...\nSorry, compilation cannot be executed.\n\nDetail:\n" + ex.Message);
                                        return;
                                    }
                                }
                            }
                        }
                    }
                    if (retCode != 0)
                    {
                        //Compile ERROR!
                        if (compileIn.cCompileMode == Compiler.COMPILE_OPTION_BIN_FILE)
                        {
                            if (compiled.iErrFileLine >= 0)
                            {
                                errStringText += "Error on line " + compiled.iErrFileLine.ToString() + ", file: " + Compiler.GetStringFromMemory(compiled.czErrFileName);
                                errStringText += "\n    ";
                            }
                            errStringText += Compiler.GetStringFromMemory(compiled.czErrMessage);
                        }
                        else
                        {
                            string errMessageTrimmed = Compiler.GetStringFromMemory(compiled.czErrMessage);
                            ConvertRadix.RemoveFormattingChars(ref errMessageTrimmed, true/*trim start*/);
                            if (compiled.iErrFileLine >= 0)
                                errStringText += String.Format("Compile error on line {0}!\n    {1}", compiled.iErrFileLine, errMessageTrimmed.TrimStart());
                            else
                                errStringText += String.Format("Compile error: {0}", errMessageTrimmed.TrimStart());
                        }

                        LOG_INFO logInfo = new LOG_INFO();
                        logInfo.ErrorLine = compiled.iErrFileLine; logInfo.SetMessage(errStringText); logInfo.Level = LOG_LEVEL.Error;
                        this.richCompileMessages.AppendLog(logInfo);
                    }
                    else
                    {
                        //we got a assembly
                        this.richCompileMessages.AppendInfo("Compilation OK !", LOG_OPTIONS.NoHeaderOrFooter | LOG_OPTIONS.AddTime);

                        //write to memory ?
                        if (compiled.iCompiledSize > 0)
                        {
                            //get address where to write the code
                            ushort memAdress = (ushort)(compiled.czCompiled[0] + compiled.czCompiled[1] * 256);
                            if (memAdress >= 0x4000) //RAM start
                            {
                                Stopwatch watch = new Stopwatch();
                                watch.Start();
                                if ((memAdress + compiled.iCompiledSize) > 0xFFFF)
                                    compiled.iCompiledSize = 0xFFFF - memAdress; //prevent memory overload
                                byte[] memBytesCompiled = ConvertRadix.PointerDataToManagedType(compiled.czCompiled + 2/*omit memory address*/, compiled.iCompiledSize);
                                if (memBytesCompiled != null)
                                    m_debugger.GetVMKernel().WriteMemory(memAdress, memBytesCompiled, 0, compiled.iCompiledSize);
                                watch.Stop();

                                TimeSpan time = watch.Elapsed;
                                string compileInfo = String.Format("\n    Memory written starting at address: #{0:X04}({1})", memAdress, memAdress);
                                compileInfo += String.Format("\n    Written #{0:X04}({1}) bytes", compiled.iCompiledSize, compiled.iCompiledSize);

                                this.richCompileMessages.AppendLog(compileInfo, LOG_LEVEL.Info);
                            }
                            else
                            {
                                this.richCompileMessages.AppendLog("Cannot write to ROM(address = " + memAdress.ToString() + "). Bail out.", LOG_LEVEL.Error);
                                return;
                            }

                            //Symbols
                            this.listViewSymbols.Items.Clear();
                            if (new IntPtr(compiled.arrSourceSymbols) != IntPtr.Zero)
                            {
                                Dictionary<string, ushort> symbolsParsed = Compiler.ParseSymbols(Compiler.GetStringFromMemory(compiled.arrSourceSymbols));
                                if (symbolsParsed != null && symbolsParsed.Keys.Count > 0)
                                {
                                    foreach (var item in symbolsParsed)
                                    {
                                        ListViewItem itemToAdd = new ListViewItem(new[] { item.Key, String.Format("#{0:X2}", item.Value) });
                                        itemToAdd.Tag = String.Format("#{0:X2}", item.Value); //will be parsed in ListViewCustom
                                        //itemToAdd.Tag = tag;
                                        this.listViewSymbols.Items.Add(itemToAdd);
                                    }

                                    //this.listViewSymbols.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
                                    //this.listViewSymbols.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                                }
                            }
                        }
                        else
                        {
                            if (compileIn.cCompileMode == Compiler.COMPILE_OPTION_TAP_BAS)
                            {
                                string generatedFileName = txtbxFileOutputPath.Text;
                                bool isFileGenerated = File.Exists(generatedFileName);

                                string compilationInfo;
                                if (isFileGenerated)
                                    compilationInfo = String.Format("Compilation finished: File {0} generated!", generatedFileName);
                                else
                                    compilationInfo = String.Format("Compilation finished: File {0} NOT generated(access denied?)!", generatedFileName);

                                this.richCompileMessages.AppendInfo(String.Format("Compilation finished, file {0} generated!", generatedFileName), LOG_OPTIONS.AddFooter | LOG_OPTIONS.AddTime);
                            }
                            else
                                this.richCompileMessages.AppendInfo("Compilation finished: Nothing to write to memory !", LOG_OPTIONS.AddFooter | LOG_OPTIONS.AddTime);
                        }
                    }
                }
            }
        }

        private bool ValidateCompile()
        {
            Directory.SetCurrentDirectory(Utils.GetAppFolder());
            if (!File.Exists(@"Pasmo2.dll"))
            {
                Locator.Resolve<IUserMessage>().Error(
                    "Pasmo2.dll not found in " + Utils.GetAppFolder() + "!\n\nThis file is needed for compilation\n" +
                    "into Z80 code." +
                    "\n\n" +
                    "Now going to try to get it from internet.\nPlease click OK."
                    );

                TcpHelper client = new TcpHelper();
                client.Show();

                return false;
            }

            if (Compiler.IsStartAdressInCode(txtAsm.Lines) == false)
            {
                //start adress for compilation not found
                Locator.Resolve<IUserMessage>().Error("Compile validation failed...\n\nMissing start address(ORG instruction).");
                return false;
            }

            return true;
        }

        #region GUI methods
        private bool _eventsDisabled = false;

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void assemblerForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                e.Handled = true;
                compileToZ80();
                return;
            }

            if( e.KeyCode == Keys.O && e.Control )
            {
                openFileStripButton_Click(null, null);
            }

            if (e.KeyCode == Keys.S && e.Control)
            {
                saveFileStripButton_Click(null, null);
            }
        }

        //Compile Button
        private void btnCompile_Click(object sender, EventArgs e)
        {
            compileToZ80();
        }
        private void compileToolStrip_Click(object sender, EventArgs e)
        {
            compileToZ80();
        }

        //Select font
        /*private void fonttoolStrip_Click(object sender, EventArgs e)
        {
            FontDialog fontDialog = new FontDialog();
            fontDialog.Font = txtAsm.Font;

            DialogResult res = fontDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                Font font = fontDialog.Font;
                txtAsm.Font = font; //FastColoredTextBox supports only monospaced fonts !
            }
        }*/

        //text Color
        private void toolStripColors_Click(object sender, EventArgs e)
        {
            AssemblerColorConfig.ShowForm();
        }
        //refresh assembler text when color styles changed
        public void RefreshAssemblerCodeSyntaxHighlightning()
        {
            AssemblerColorConfig.RefreshControlStyles(this.txtAsm, null);
        }

        //open file
        private void openFileStripButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog loadDialog = new OpenFileDialog())
            {
                loadDialog.InitialDirectory = ".";
                loadDialog.SupportMultiDottedExtensions = true;
                loadDialog.Title = "Load file...";
                loadDialog.Filter = "Assembler files (asm,txt)|*.asm;*.txt|All files (*.*)|*.*";
                loadDialog.DefaultExt = "";
                loadDialog.FileName = "";
                loadDialog.ShowReadOnly = false;
                loadDialog.CheckFileExists = true;
                if (loadDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    return;

                //progressBarBackgroundProcess.Init(4, () => RegisterAsmFile(loadDialog.FileName), null);
                //progressBarBackgroundProcess.Start();
                RegisterAsmFile(loadDialog.FileName);
            }
        }

        private void RegisterAsmFile( string i_fileName )
        {
            //ProgressBarBackgroundProcess.ProcessInvokeRequired(txtAsm, () => txtAsm.Cursor = Cursors.WaitCursor);
            ProgressBarBackgroundProcess.ProcessInvokeRequired(richCompileMessages, () => richCompileMessages.AppendInfo("Registering new file...\n", LOG_OPTIONS.NoHeaderOrFooter | LOG_OPTIONS.AddTime | LOG_OPTIONS.AddLogTitle));

            string fileText;
            bool loadRetCode = LoadAsm(i_fileName, out fileText);
            progressBarBackgroundProcess.IncreaseCounter();
            if (loadRetCode == false)
                return;

            //progressBarBackgroundProcess.RunOnNewThread(this, () =>
            {
                //new source code
                AssemblerSourceInfo sourceInfo = new AssemblerSourceInfo(i_fileName, true);
                sourceInfo.SetIsFile();
                sourceInfo.SetIsSaved(true);
                sourceInfo.SetSourceNameOrFilename(i_fileName);
                sourceInfo.SetSourceCode(fileText);
                _actualAssemblerNodeIndex = AddNewSource(sourceInfo);
                //new node
                TreeNode node = new TreeNode(Path.GetFileName(i_fileName));

                //add subitems(includes)
                List<string> includes = Compiler.ParseIncludes(i_fileName);
                if (includes != null)
                    foreach (string include in includes)
                    {
                        AssemblerSourceInfo includeInfo = new AssemblerSourceInfo(include, true);
                        //includeInfo.SetIsFile();
                        //includeInfo.SetIsSaved(true);
                        includeInfo.SetSourceNameOrFilename(include);
                        includeInfo.IdParent = _actualAssemblerNodeIndex;
                        string includeFileContent;
                        bool retCode = FileTools.ReadFile(include, out includeFileContent);
                        if (retCode)
                            includeInfo.SetSourceCode(includeFileContent);

                        TreeNode incNode = new TreeNode(Path.GetFileName(include));
                        incNode.ToolTipText = include;
                        incNode.Checked = false;
                        incNode.Tag = AddNewSource(includeInfo);
                        node.Nodes.Add(incNode);
                    }
                node.ToolTipText = i_fileName;
                node.Checked = true;
                node.Tag = _actualAssemblerNodeIndex;
                ProgressBarBackgroundProcess.ProcessInvokeRequired(treeViewFiles, () => treeViewFiles.Nodes.Add(node));
                
                //increase progress bar
                ProgressBarBackgroundProcess.ProcessInvokeRequired(progressBarBackgroundProcess, () => progressBarBackgroundProcess.IncreaseCounter());
                //setting GUI
                ProgressBarBackgroundProcess.ProcessInvokeRequired(richCompileMessages, 
                    () => richCompileMessages.AppendInfo("Applying source code...", 
                          LOG_OPTIONS.NoHeaderOrFooter | LOG_OPTIONS.AddTime));
                //Application.DoEvents();
                txtAsm.Clear();
                progressBarBackgroundProcess.Init(1, () => txtAsm.AppendText(sourceInfo.GetSourceCode()), null);
                progressBarBackgroundProcess.SetOnFinishedAction(() =>
                    ProgressBarBackgroundProcess.ProcessInvokeRequired(richCompileMessages, () => richCompileMessages.AppendInfo("Registering finished.\n", LOG_OPTIONS.AddFooter | LOG_OPTIONS.AddTime))
                );
                progressBarBackgroundProcess.Start();

                _eventsDisabled = true;
                ProgressBarBackgroundProcess.ProcessInvokeRequired(treeViewFiles, () => treeViewFiles.SelectedNode = node);
                _eventsDisabled = false;

                //progressBarBackgroundProcess.RunOnNewThread(txtAsm, () => txtAsm.Cursor = Cursors.IBeam);
                //ProgressBarBackgroundProcess.ProcessInvokeRequired(richCompileMessages, () => richCompileMessages.AppendInfo("Registering finished.\n", LOG_OPTIONS.AddFooter | LOG_OPTIONS.AddTime));
            }//);
        }

        private void txtAsm_TextChanged(object sender, TextChangedEventArgs e)
        {
            AssemblerColorConfig.RefreshControlStyles(txtAsm, e);
        }

        //Save button
        private void saveFileStripButton_Click(object sender, EventArgs e)
        {
            AssemblerSourceInfo sourceInfo = _assemblerSources[_actualAssemblerNodeIndex];
            //save as if not a file
            if (!sourceInfo.IsFile())
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                saveDialog.Title = "Save assembler code";
                DialogResult dialogResult = saveDialog.ShowDialog();
                if (saveDialog.FileName != String.Empty && dialogResult == DialogResult.OK)
                {
                    sourceInfo.SetIsFile();
                    sourceInfo.SetSourceNameOrFilename(saveDialog.FileName);

                    //change name of actual source
                    TreeNode node = treeViewFiles.Nodes[_actualAssemblerNodeIndex];
                    node.Text = sourceInfo.GetDisplayName();
                }
                else
                {
                    Locator.Resolve<IUserMessage>().Warning("File NOT saved!");
                    return;
                }
            }

            SaveAsm(sourceInfo.GetFileNameToSave());
            sourceInfo.SetSourceCode( txtAsm.Text );
        }

        //Refresh button
        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            AssemblerSourceInfo info;
            _assemblerSources.TryGetValue(_actualAssemblerNodeIndex, out info);

            if (info != null)
            {
                string dummy;
                if (info.IsFile())
                    if(LoadAsm(info.GetFileNameToSave(), out dummy))
                    {
                        info.SetSourceCode(dummy);
                        info.SetIsSaved(true);
                        txtAsm.Text = dummy;
                    }

                RefreshFileList();
            }
        }
        //Refresh file list treenode
        private void RefreshFileList()
        {
            _eventsDisabled = true;
            if (_assemblerSources.Count == treeViewFiles.Nodes.Count)
            {
                foreach (TreeNode node in treeViewFiles.Nodes)
                {
                    node.Text = _assemblerSources[(int)node.Tag].GetDisplayName();
                }
            }
            _eventsDisabled = false;
        }

        //Form close
        private void Assembler_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        //Clear compilation log
        private void buttonClearAssemblerLog_Click(object sender, EventArgs e)
        {
            this.richCompileMessages.ClearLog();
        }

        //Sources treeview before node changed
        private void treeViewFiles_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (_eventsDisabled)
                return;
            if (GetActualSourceInfo() != null)
            {
                AssemblerSourceInfo info = GetActualSourceInfo();
                if (info.IsSourceEqual(txtAsm.Text) == false)
                {
                    info.SetIsSaved(false);
                }
                info.SetSourceCode( txtAsm.Text );
            }
            if (treeViewFiles.SelectedNode != null)
                treeViewFiles.SelectedNode.Text = GetActualSourceInfo().GetDisplayName();
        }
        //Sources treeview after node changed
        private void treeViewFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (_eventsDisabled)
                return;

            if (e.Node.Tag != null)
            {
                _actualAssemblerNodeIndex = ConvertRadix.ParseUInt16(e.Node.Tag.ToString(), 10);
                this.txtAsm.Text = _assemblerSources[_actualAssemblerNodeIndex].GetSourceCode();
            }
        }

        //Z80 source code(Libs)
        private void toolCodeLibrary_Click(object sender, EventArgs e)
        {
            _Z80AsmResourcesForm.ShowDialog();
        }

        //Context menu - Assembler commands(visual)
        private void txtAsm_MouseClick(object sender, MouseEventArgs e)
        {
            if( e.Button == System.Windows.Forms.MouseButtons.Right )
                ctxMenuAssemblerCommands.Show(txtAsm, e.Location);
        }

        //Context menu - Symbols(compiled by Pasmo2.dll)
        private void listViewSymbols_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                ctxmenuSymbols.Show(listViewSymbols, e.Location);
        }
        private void noteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewSymbols.SelectedItems.Count >= 1)
            {
                ListView.SelectedListViewItemCollection items = listViewSymbols.SelectedItems;
                foreach (ListViewItem item in items)
                    m_debugger.InsertCodeNote("; " + item.Text, ConvertRadix.ConvertNumberWithPrefix(item.Tag.ToString()));
            }
        }
        private void commentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewSymbols.SelectedItems.Count >= 1)
            {
                ListView.SelectedListViewItemCollection items = listViewSymbols.SelectedItems;
                foreach(ListViewItem item in items )
                    m_debugger.InsertCodeComment(item.Text, ConvertRadix.ConvertNumberWithPrefix(item.Tag.ToString()));
            }
        }

        //Context menu - Convert numbers to hexadecimal format
        private void toHexadecimalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //send selection from assembler code
            txtAsm.Text = ConvertRadix.NumbersInTextToHex(txtAsm.Text);
        }

        //Format code
        private void btnFormatCode_Click(object sender, EventArgs e)
        {
            progressBarBackgroundProcess.Init(txtAsm.Lines.Count, () => FormatCode(), () => FormatCodeFinish());
            progressBarBackgroundProcess.Start();
        }
        private void btnStopBackgroundProcess_Click(object sender, EventArgs e)
        {
            if (progressBarBackgroundProcess.IsWorking())
            {
                progressBarBackgroundProcess.CancelProcessManual();
                richCompileMessages.AppendInfo("Code formatting canceled by user!\n", LOG_OPTIONS.AddFooter | LOG_OPTIONS.AddTime);
            }
        }
        private void FormatCodeFinish()
        {
            //LOG_INFO loginfo = new LOG_INFO("Code formatting finished...\n", LOG_OPTIONS.AddFooter);
            //richCompileMessages.AppendLog(loginfo);
        }
        private void FormatCode()
        {
            //ToDo: we need a list of all assembler commands here, but cannot use AssemblerConfig regex patterns
            string[] opcodes = new string[] { "ld", "org", "push", "ex", "call", "inc", "pop", "sla", "ldir", "djnz", "ret", "add", "adc", "and", "sub", "xor", "jr", "jp", "exx",
                                              "dec", "srl", "scf", "ccf", "di", "ei", "im", "or", "cpl", "out", "in", "cp", "reti", "retn", "rra", "rla", "sbc", "rst",
                                              "rlca", "rrc", "res", "set", "bit", "halt", "cpd", "cpdr", "cpi", "cpir", "cpl", "daa", "rrca", "rr"};
            string[] strAsmLines = txtAsm.Lines.ToArray<string>();
            //Range actLineSave = new Range(txtAsm, txtAsm.Selection.Start, txtAsm.Selection.End);
            //Place actLineSave = ...

            //progressBarBackgroundProcess.RunOnNewThread(txtAsm, () => txtAsm.Cursor = Cursors.WaitCursor);
            ProgressBarBackgroundProcess.ProcessInvokeRequired(richCompileMessages, () => richCompileMessages.AppendInfo("Code formatting started...\n", LOG_OPTIONS.NoHeaderOrFooter | LOG_OPTIONS.AddTime));

            //step 1: add whitespace after each ',' and '('
            for (int lineCounter = 0; lineCounter < strAsmLines.Length; lineCounter++)
            {
                //while (Regex.IsMatch(strAsmLines[lineCounter], @"[,\(]\S+"))
                {
                    int index = strAsmLines[lineCounter].IndexOf(",");
                    while (index >= 0)
                    {
                        if (index < strAsmLines[lineCounter].Length - 1)
                        {
                            if (strAsmLines[lineCounter][index + 1] != ' ')
                                strAsmLines[lineCounter] = strAsmLines[lineCounter].Substring(0, index) + ", " + strAsmLines[lineCounter].Substring(index + 1, strAsmLines[lineCounter].Length - index - 1);
                        }
                        index = strAsmLines[lineCounter].IndexOf(",", index + 1);
                    }

                    //whitespace after each bracket "("
                    index = strAsmLines[lineCounter].IndexOf("(");
                    while (index >= 0)
                    {
                        if (index > 0)
                        {
                            if (strAsmLines[lineCounter][index - 1] != ' ')
                                strAsmLines[lineCounter] = strAsmLines[lineCounter].Substring(0, index) + " (" + strAsmLines[lineCounter].Substring(index + 1, strAsmLines[lineCounter].Length - index - 1);
                        }
                        index = strAsmLines[lineCounter].IndexOf("(", index + 1);
                    }

                    //whitespace after ":"
                    index = strAsmLines[lineCounter].IndexOf(":");
                    while (index >= 0)
                    {
                        if (index > 0)
                        {
                            if (strAsmLines[lineCounter][index - 1] != ' ')
                                strAsmLines[lineCounter] = strAsmLines[lineCounter].Substring(0, index) + ": " + strAsmLines[lineCounter].Substring(index + 1, strAsmLines[lineCounter].Length - index - 1);
                        }
                        index = strAsmLines[lineCounter].IndexOf(":", index + 1);
                    }
                }
            }

            //Application.DoEvents();

            StringBuilder codeFormatted = new StringBuilder();
            foreach (string line in strAsmLines)
            {
                bool isNewLine = true; bool isInComment = false; bool addNewlineAtLineEnd = true; bool bIsInCompilerDirective = false; int lineTokeCounter = 0;

                //canceled ?
                if (progressBarBackgroundProcess.IsTerminateSignalReceived())
                    return;

                string[] lineSplitted = Regex.Split(line, @"\s+", RegexOptions.IgnoreCase);
                string prevToken = String.Empty;
                foreach (string token in lineSplitted)
                {
                    lineTokeCounter++;

                    //compiler directives
                    if (Regex.IsMatch(token, AssemblerColorConfig.regexCompilerDirectives))
                    {
                        if (prevToken != String.Empty)
                            codeFormatted.Append(" ");
                        codeFormatted.Append( token); //"include" has 7 chars
                        bIsInCompilerDirective = true;
                        prevToken = token;
                        continue;
                    }
                    if (bIsInCompilerDirective)
                    {
                        if (token == String.Empty) //newline?
                            bIsInCompilerDirective = false;
                        else
                            codeFormatted.Append(" " + token);
                        prevToken = token;
                        continue;
                    }

                    if (token == String.Empty)
                    {
                        if (lineSplitted.Length == 1)
                        {
                            codeFormatted.AppendLine();
                            addNewlineAtLineEnd = false;
                        }
                        prevToken = token;
                        continue;
                    }
                    if (token.StartsWith(";"))
                    {
                        if (!isInComment)
                        {
                            isInComment = true;
                            if( prevToken == String.Empty )
                                codeFormatted.Append(token);
                            else
                                codeFormatted.Append(" " + token);
                            prevToken = token;
                            continue;
                        }
                        isInComment = false;
                    }
                    if (isInComment)
                    {
                        codeFormatted.Append(" " + token);
                        continue;
                    }

                    if (opcodes.Contains(token.ToLower()))
                    {
                        if (isNewLine)
                        {
                            codeFormatted.Append(new String(' ', _tabSpace));
                            bIsInCompilerDirective = false;
                        }
                        else if (prevToken != String.Empty)
                        {
                            string spaces = new String(' ', Math.Max(_tabSpace - prevToken.Length, 1));
                            codeFormatted.Append(spaces);
                        }
                        codeFormatted.Append(token);
                    }
                    else
                    {
                        //rest(numbers, ...)
                        if ((_tabSpace > token.Length && isNewLine))
                        {
                            int spacesAfter = _tabSpace - token.Length;
                            codeFormatted.Append(token);
                        }
                        else
                        {
                            if (prevToken != String.Empty)
                            {
                                string spaces;
                                
                                if( prevToken.EndsWith(",") )
                                    spaces = new String(' ', Math.Max(4 - prevToken.Length, 1));
                                else
                                    spaces = new String(' ', Math.Max(6 - prevToken.Length, 1));
                                codeFormatted.Append(spaces + token);
                            }
                            else
                                codeFormatted.Append(token);
                        }
                    }

                    isNewLine = false;
                    prevToken = token;
                }
                if (addNewlineAtLineEnd)
                    codeFormatted.AppendLine();
                isInComment = false;

                //increase progress bar counter;
                Invoke(new Action(() =>
                {
                    //codeFormatted = codeFormatted.ToString().TrimEnd(' ');
                    progressBarBackgroundProcess.IncreaseCounter();
                }));
            }

            Invoke(new Action(() =>
            {
                richCompileMessages.AppendInfo("Applying formatted code...", LOG_OPTIONS.NoHeaderOrFooter);
                string code = codeFormatted.ToString();
                while (code.EndsWith("\n") || code.EndsWith("\r"))
                {
                    code = code.TrimEnd('\n');
                    code = code.TrimEnd('\r');
                }
                txtAsm.Text = code + Environment.NewLine;
                /*if (txtAsm.Range.End.iLine > code.Length)
                    txtAsm.Selection = actLineSave;
                else
                    txtAsm.Selection = new Range(code.Length, code.Length);
                txtAsm.DoSelectionVisible();*/

                //txtAsm.Cursor = Cursors.IBeam;
                richCompileMessages.AppendInfo("Code formatting finished...\n", LOG_OPTIONS.AddFooter | LOG_OPTIONS.AddTime);
            }));
        }

        private void richCompileMessages_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            LOG_INFO logInfo = richCompileMessages.GetCurrentMessage();
            if (logInfo != null && logInfo.ErrorLine != -1 && logInfo.ErrorLine-1 < txtAsm.LinesCount)
            {
                int lineIndex = logInfo.ErrorLine - 1;
                txtAsm.Selection = new Range(txtAsm, lineIndex);
                //txtAsm.SelectionColor = Color.DarkRed;
                txtAsm.DoSelectionVisible();
            }
            //Locator.Resolve<IUserMessage>().Info(String.Format("Current error line: {0}", logInfo.ErrorLine));
        }

        private TreeNode GetActualNode()
        {
            if (treeViewFiles.SelectedNode != null)
                return treeViewFiles.SelectedNode;
            else
                return treeViewFiles.Nodes[0];
        }
        private AssemblerSourceInfo GetActualSourceInfo()
        {
            //TreeNode nodeAct = GetActualNode();
            AssemblerSourceInfo info;
            _assemblerSources.TryGetValue(_actualAssemblerNodeIndex, out info);
            return info;
        }

        private void listViewSymbols_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listViewSymbols.SelectedItems.Count == 1)
            {
                ListView.SelectedListViewItemCollection items = listViewSymbols.SelectedItems;

                ListViewItem lvItem = items[0];
                string label = lvItem.Text;

                int labelLineIndex = -1;
                for( int counter = 0; counter < txtAsm.Lines.Count; counter++ )
                {
                    string actLine = txtAsm.Lines[counter];
                    string[] lineTokens = Regex.Split(actLine, @"\s+");
                    if( lineTokens.Length > 0 && lineTokens != null )
                    {
                        if (lineTokens[0] == label || lineTokens[0] == label + ":")
                        {
                            labelLineIndex = counter;
                            txtAsm.Focus();
                            break;
                        }
                    }
                }
                if (labelLineIndex >= 0)
                {
                    txtAsm.Selection = new Range(txtAsm, labelLineIndex);
                    txtAsm.DoSelectionVisible();
                }
            }
        }

        //Compiled output options
        private void radioBtnMemoryOutput_CheckedChanged(object sender, EventArgs e)
        {
            //Memory compiled output
            txtbxFileOutputPath.Enabled = false;
        }
        private void radioBtnTAPBASOutput_MouseClick(object sender, MouseEventArgs e)
        {
            //TAP_BAS compiled output
            txtbxFileOutputPath.Enabled = true;
        }

        //Settings
        private void settingsToolStrip_Click(object sender, EventArgs e)
        {
            Adlers.Views.AssemblerView.Settings.ShowForm();
        }

        //Symbols list view size changed
        private void listViewSymbols_SizeChanged(object sender, EventArgs e)
        {
            listViewSymbols.Columns[0].Width = Math.Max(listViewSymbols.Width - 50, 100);
        }
        #endregion GUI

        #region File operations
            private bool LoadAsm(string i_fileName, out string o_strFileText)
            {
                    Func<string> delegateLoadAsm = delegate()
                    {
                        string lambdaOut = string.Empty;

                        if (i_fileName == String.Empty || i_fileName == null)
                            return string.Empty;

                        try
                        {
                            bool retCode = FileTools.ReadFile(i_fileName, out lambdaOut);
                            if (!retCode)
                                return lambdaOut;

                            Invoke(new Action(() =>
                            {
                                this.richCompileMessages.AppendInfo("File " + i_fileName + " read successfully..", LOG_OPTIONS.NoHeaderOrFooter | LOG_OPTIONS.AddTime);
                            }));
                            return lambdaOut;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                            this.richCompileMessages.Text += "\n\nFile " + i_fileName + " read ERROR!";
                            return lambdaOut;
                        }
                    };
                    o_strFileText = delegateLoadAsm.Invoke();
                    return o_strFileText != string.Empty;
            }

            private bool SaveAsm(string i_fileName)
            {
                if (i_fileName == String.Empty)
                    i_fileName = "code_save.asm";
                File.WriteAllText(Path.Combine(Utils.GetAppFolder(),i_fileName), this.txtAsm.Text);

                //Locator.Resolve<IUserMessage>().Info("File " + i_fileName + " saved!");
                this.richCompileMessages.AppendInfo("File " + i_fileName + " saved!");

                AssemblerSourceInfo info;
                _assemblerSources.TryGetValue(_actualAssemblerNodeIndex, out info);
                if (info != null)
                {
                    _assemblerSources[_actualAssemblerNodeIndex].SetIsSaved(true);
                    GetActualNode().Text = _assemblerSources[_actualAssemblerNodeIndex].GetDisplayName();
                }
                return true;
            }
        #endregion File operations

        #region Source management(add/delete/save/refresh)
        class AssemblerSourceInfo
        {
            private int _id;
            public int Id{get { return this._id; } set{ if(value >= 0) this._id = value; }}
            public int IdParent;

            private string _sourceCode;
            public string GetSourceCode()
            {
                return _sourceCode;
            }
            public void SetSourceCode(string i_newSourceCode)
            {
                if (i_newSourceCode != null)
                    _sourceCode = i_newSourceCode;
                else
                    _sourceCode = String.Empty;
            }

            private  bool _isFile { get; set; }
            private bool IsSaved { get; set; }
            private string _sourceName { get; set; } //empty when it is a file

            private string _fileName;

            public AssemblerSourceInfo(string i_sourceName, bool i_isFile)
            {
                _sourceName = i_sourceName;
                _isFile = i_isFile;
                IsSaved = true;
                _fileName = _sourceCode = string.Empty;
                IdParent = -1;
            }

            public string GetFileNameToSave()
            {
                if (_fileName == string.Empty || !_isFile)
                    return _sourceName;
                else
                    return _fileName;
            }
            public string GetSourceName()
            {
                return _sourceName;
            }
            public string GetDisplayName()
            {
                string fileName = _sourceName;

                if (_sourceName.IndexOf(Path.DirectorySeparatorChar) != -1)
                {
                    string[] splitted = _sourceName.Split(Path.DirectorySeparatorChar);
                    fileName = splitted[splitted.Length-1];
                }
                if (!IsSaved)
                    return "*" + fileName;
                else
                    return fileName;
            }
            public bool IsFile()
            {
                return _isFile;
            }
            public void SetIsFile()
            {
                _isFile = true;
            }
            public void SetIsSaved(bool i_isSaved)
            {
                IsSaved = i_isSaved;
            }
            public bool IsSourceEqual(string i_sourceActual)
            {
                return i_sourceActual == this._sourceCode;
            }
            public void SetSourceNameOrFilename(string i_newName)
            {
                if (!_isFile) //if it is not file
                    _sourceName = i_newName;
                else
                {
                    int lastIndex = _fileName.LastIndexOf(Path.DirectorySeparatorChar);
                    //if it is a file then we remember the file path
                    if (lastIndex != -1)
                    {
                        _fileName = _fileName.Substring(0, lastIndex) + Path.DirectorySeparatorChar + i_newName;
                        _sourceName = i_newName;
                    }
                    else
                        _sourceName = _fileName = i_newName;
                }
            }
            public string GetFileDirectory()
            {
                if (IsFile() && _fileName.Length > 1)
                    return _fileName.Substring(0, _fileName.LastIndexOf(Path.DirectorySeparatorChar)) + Path.DirectorySeparatorChar;
                else
                    return String.Empty;
            }
            public void RenameFile(string i_newName)
            {
                if (IsFile())
                {
                    string newFileWithFullPath = GetFileDirectory() + i_newName;
                    if (File.Exists(_fileName))
                        File.Move(_fileName, newFileWithFullPath);
                    else
                        SetIsSaved(false);
                    _sourceName = i_newName;
                    _fileName = newFileWithFullPath;
                }
            }
        }

        private int AddNewSource(AssemblerSourceInfo i_sourceCandidate, string i_sourceCode = null)
        {
            i_sourceCandidate.Id = SourceInfo_GetNextId();
            if (i_sourceCandidate.Id == 0) //there were none
                i_sourceCandidate.SetSourceCode( txtAsm.Text );
            if (i_sourceCode!=null)
                i_sourceCandidate.SetSourceCode(_strDefaultAsmSourceCode);
            _assemblerSources.Add(i_sourceCandidate.Id, i_sourceCandidate);

            return i_sourceCandidate.Id;
        }
        private void RemoveSource(int i_sourceIndex)
        {
            if (_assemblerSources != null && _assemblerSources.Count > 0)
            {
                _assemblerSources.Remove(i_sourceIndex);
                //remove child nodes(includes) too
                foreach (var idToRemove in _assemblerSources.Where(p => p.Value.IdParent == i_sourceIndex).ToList())
                    _assemblerSources.Remove(idToRemove.Key);
            }
            if (_assemblerSources.Count > 0)
            {
                _actualAssemblerNodeIndex = 0;
                this.txtAsm.Text = _assemblerSources.FirstOrDefault().Value.GetSourceCode(); //setting to first in list
                treeViewFiles.SelectedNode = treeViewFiles.Nodes[0];
            }
            else
                this.txtAsm.Text = String.Empty;
        }
        private int SourceInfo_GetNextId()
        {
            if (_assemblerSources.Count == 0)
                return 0;
            return _assemblerSources.Max(p => p.Key) + 1;
        }
        private string GetNewSourceName()
        {
            for (int counter = 1; counter < int.MaxValue; counter++ )
            {
                string sourceNameCandidate = String.Format("noname{0}.asm", counter);
                if( _assemblerSources.Values.FirstOrDefault( p => p.GetSourceName() == sourceNameCandidate ) == null )
                    return sourceNameCandidate;
            }
            
            return string.Empty;
        }

        //treeViewFiles: KeyUp
        private void treeViewFiles_KeyUp(object sender, KeyEventArgs e)
        {
            if( e.KeyCode == Keys.Delete && treeViewFiles.Nodes.Count > 0 )
            {
                var node = treeViewFiles.SelectedNode;
                if (node != null)
                {
                    RemoveSource((int)node.Tag);
                    treeViewFiles.Nodes.Remove(node);
                }
            }
        }
        //treeViewFiles: AfterLabelEdit
        private void treeViewFiles_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            TreeNode node = treeViewFiles.SelectedNode;
            if( node != null )
            {
                if ( e.Label == null || e.Label == node.Text)
                    return;

                int index = (int)node.Tag;
                AssemblerSourceInfo sourceInfo;
                if( _assemblerSources != null && _assemblerSources.TryGetValue(index, out sourceInfo) )
                {
                    sourceInfo.RenameFile(e.Label);
                }

                _actualAssemblerNodeIndex = index;
            }
        }

        //Create new source code
        private void toolStripNewSource_Click(object sender, EventArgs e)
        {
            string newSourceName = GetNewSourceName();
            
            //register node + source
            AssemblerSourceInfo newSource = new AssemblerSourceInfo(newSourceName, false);
            TreeNode node = new TreeNode();
            node.Text = newSource.GetSourceName();
            node.Tag = AddNewSource(newSource);
            treeViewFiles.Nodes.Add(node);
        }
        #endregion

        #region Config
        public void GetPartialConfig(ref XmlWriter io_writer)
        {
            if (m_instance == null)
                return;

            //Assembler root
            io_writer.WriteStartElement("Assembler");
                io_writer.WriteStartElement("Colors");

                    //Colors->Comments
                    io_writer.WriteStartElement("CommentStyle");
                    io_writer.WriteAttributeString("Enabled", AssemblerColorConfig.GetInstance().IsCommentsColorEnabled() ? "1" : "0");
                    io_writer.WriteAttributeString("TextColor", AssemblerColorConfig.CommentStyle.GetCSS());
                    io_writer.WriteEndElement();  //Colors->Comments end

                    //Colors->CompilerDirectivesStyle
                    io_writer.WriteStartElement("CompilerDirectivesStyle");
                    io_writer.WriteAttributeString("Enabled", AssemblerColorConfig.GetInstance().IsCompilerDirectivesEnabled() ? "1" : "0");
                    io_writer.WriteAttributeString("TextColor", AssemblerColorConfig.CompilerDirectivesStyle.GetCSS());
                    io_writer.WriteEndElement();  //Colors->Compiler directives end

                    //Colors->JumpsStyle
                    io_writer.WriteStartElement("JumpsStyle");
                    io_writer.WriteAttributeString("Enabled", AssemblerColorConfig.GetInstance().IsJumpStyleEnabled() ? "1" : "0");
                    io_writer.WriteAttributeString("TextColor", AssemblerColorConfig.JumpInstructionStyle.GetCSS());
                    io_writer.WriteEndElement();  //Colors->Jumps end

                    //Colors->CommonInstructionStyle
                    io_writer.WriteStartElement("CommonInstructionsStyle");
                    io_writer.WriteAttributeString("Enabled", AssemblerColorConfig.GetInstance().IsCommonInstructionsStyleEnabled() ? "1" : "0");
                    io_writer.WriteAttributeString("TextColor", AssemblerColorConfig.CommonInstructionStyle.GetCSS());
                    io_writer.WriteEndElement();  //Colors->CommonInstructionStyle end

                    //Colors->NumbersStyle
                    io_writer.WriteStartElement("NumbersStyle");
                    io_writer.WriteAttributeString("Enabled", AssemblerColorConfig.GetInstance().IsNumbersStyleEnabled() ? "1" : "0");
                    io_writer.WriteAttributeString("TextColor", AssemblerColorConfig.NumbersStyle.GetCSS());
                    io_writer.WriteEndElement();  //Colors->NumbersStyle end

                    //Colors->StackInstructionsStyle
                    io_writer.WriteStartElement("StackInstructionsStyle");
                    io_writer.WriteAttributeString("Enabled", AssemblerColorConfig.GetInstance().IsStackInstructionsStyleEnabled() ? "1" : "0");
                    io_writer.WriteAttributeString("TextColor", AssemblerColorConfig.StackInstructionStyle.GetCSS());
                    io_writer.WriteEndElement();  //Colors->StackInstructionsStyle end

                io_writer.WriteEndElement(); //Colors end

                //Compile mode
                io_writer.WriteStartElement("Compiler");
                io_writer.WriteAttributeString("MinCompilerVersion", "1.03");
                    io_writer.WriteStartElement("OutputOptions");
                        io_writer.WriteElementString("OutputMode", radioBtnTAPBASOutput.Checked ? "2" : "1"); //TAP_BAS = 2; BIN = 1
                        io_writer.WriteElementString("TapBasOutputPath", txtbxFileOutputPath.Text);
                    io_writer.WriteEndElement(); //OutputOptions end
                io_writer.WriteEndElement(); //Compiler end

            //save settings
            Settings.GetPartialConfig(ref io_writer);

            io_writer.WriteEndElement(); //Assembler end
        }
        public void LoadConfig()
        {
            if (!File.Exists(Path.Combine(Utils.GetAppFolder(), FormCpu.ConfigXmlFileName)))
                return;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Path.Combine(Utils.GetAppFolder(), FormCpu.ConfigXmlFileName));

            //comments
            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Assembler/Colors/CommentStyle");
            if (node != null)
            {
                bool isStyleEnabled = false;
                if (node.Attributes["Enabled"] != null)
                    isStyleEnabled = node.Attributes["Enabled"].InnerText == "1";
                string css = node.Attributes["TextColor"].InnerText;
                Color commentColor = ParseCss_GetColor(css);
                FontStyle fontStyle = new FontStyle();
                if (ParseCss_IsItalic(css))
                    fontStyle |= FontStyle.Italic;
                if (ParseCss_IsBold(css))
                    fontStyle |= FontStyle.Bold;
                if (ParseCss_IsUnderline(css))
                    fontStyle |= FontStyle.Underline;
                if (ParseCss_IsStrikeout(css))
                    fontStyle |= FontStyle.Strikeout;
                AssemblerColorConfig.GetInstance().ChangeSyntaxStyle(isStyleEnabled, new TextStyle(new SolidBrush(commentColor), null, fontStyle), 0);
            }

            //compiler directive style
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Assembler/Colors/CompilerDirectivesStyle");
            if (node != null)
            {
                bool isStyleEnabled = false;
                if (node.Attributes["Enabled"] != null)
                    isStyleEnabled = node.Attributes["Enabled"].InnerText == "1";
                string css = node.Attributes["TextColor"].InnerText;
                Color compilerDirectivesColor = ParseCss_GetColor(css);
                FontStyle fontStyle = new FontStyle();
                if (ParseCss_IsItalic(css))
                    fontStyle |= FontStyle.Italic;
                if (ParseCss_IsBold(css))
                    fontStyle |= FontStyle.Bold;
                if (ParseCss_IsUnderline(css))
                    fontStyle |= FontStyle.Underline;
                if (ParseCss_IsStrikeout(css))
                    fontStyle |= FontStyle.Strikeout;
                AssemblerColorConfig.GetInstance().ChangeSyntaxStyle(isStyleEnabled, new TextStyle(new SolidBrush(compilerDirectivesColor), null, fontStyle), 1);
            }

            //jumps style
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Assembler/Colors/JumpsStyle");
            if (node != null)
            {
                bool isStyleEnabled = false;
                if (node.Attributes["Enabled"] != null)
                    isStyleEnabled = node.Attributes["Enabled"].InnerText == "1";
                string css = node.Attributes["TextColor"].InnerText;
                Color jumpsStyleColor = ParseCss_GetColor(css);
                FontStyle fontStyle = new FontStyle();
                if (ParseCss_IsItalic(css))
                    fontStyle |= FontStyle.Italic;
                if (ParseCss_IsBold(css))
                    fontStyle |= FontStyle.Bold;
                if (ParseCss_IsUnderline(css))
                    fontStyle |= FontStyle.Underline;
                if (ParseCss_IsStrikeout(css))
                    fontStyle |= FontStyle.Strikeout;
                AssemblerColorConfig.GetInstance().ChangeSyntaxStyle(isStyleEnabled, new TextStyle(new SolidBrush(jumpsStyleColor), null, fontStyle), 2);
            }

            //common instruction style
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Assembler/Colors/CommonInstructionsStyle");
            if (node != null)
            {
                bool isStyleEnabled = false;
                if (node.Attributes["Enabled"] != null)
                    isStyleEnabled = node.Attributes["Enabled"].InnerText == "1";
                string css = node.Attributes["TextColor"].InnerText;
                Color commonInstructionsStyleColor = ParseCss_GetColor(css);
                FontStyle fontStyle = new FontStyle();
                if (ParseCss_IsItalic(css))
                    fontStyle |= FontStyle.Italic;
                if (ParseCss_IsBold(css))
                    fontStyle |= FontStyle.Bold;
                if (ParseCss_IsUnderline(css))
                    fontStyle |= FontStyle.Underline;
                if (ParseCss_IsStrikeout(css))
                    fontStyle |= FontStyle.Strikeout;
                AssemblerColorConfig.GetInstance().ChangeSyntaxStyle(isStyleEnabled, new TextStyle(new SolidBrush(commonInstructionsStyleColor), null, fontStyle), 3);
            }

            //numbers style
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Assembler/Colors/NumbersStyle");
            if (node != null)
            {
                bool isStyleEnabled = false;
                if (node.Attributes["Enabled"] != null)
                    isStyleEnabled = node.Attributes["Enabled"].InnerText == "1";
                string css = node.Attributes["TextColor"].InnerText;
                Color numbersStyleColor = ParseCss_GetColor(css);
                FontStyle fontStyle = new FontStyle();
                if (ParseCss_IsItalic(css))
                    fontStyle |= FontStyle.Italic;
                if (ParseCss_IsBold(css))
                    fontStyle |= FontStyle.Bold;
                if (ParseCss_IsUnderline(css))
                    fontStyle |= FontStyle.Underline;
                if (ParseCss_IsStrikeout(css))
                    fontStyle |= FontStyle.Strikeout;
                AssemblerColorConfig.GetInstance().ChangeSyntaxStyle(isStyleEnabled, new TextStyle(new SolidBrush(numbersStyleColor), null, fontStyle), 4);
            }

            //stack instructions style
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Assembler/Colors/StackInstructionsStyle");
            if (node != null)
            {
                bool isStyleEnabled = false;
                if (node.Attributes["Enabled"] != null)
                    isStyleEnabled = node.Attributes["Enabled"].InnerText == "1";
                string css = node.Attributes["TextColor"].InnerText;
                Color stackInstrucionsStyleColor = ParseCss_GetColor(css);
                FontStyle fontStyle = new FontStyle();
                if (ParseCss_IsItalic(css))
                    fontStyle |= FontStyle.Italic;
                if (ParseCss_IsBold(css))
                    fontStyle |= FontStyle.Bold;
                if (ParseCss_IsUnderline(css))
                    fontStyle |= FontStyle.Underline;
                if (ParseCss_IsStrikeout(css))
                    fontStyle |= FontStyle.Strikeout;
                AssemblerColorConfig.GetInstance().ChangeSyntaxStyle(isStyleEnabled, new TextStyle(new SolidBrush(stackInstrucionsStyleColor), null, fontStyle), 5);
            }

            //Compile options
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Assembler/Compiler/OutputOptions/OutputMode");
            if (node != null)
            {
                this.SuspendLayout();
                if (node.InnerText == "1")
                    radioBtnMemoryOutput.Checked = true; //1
                else
                {
                    radioBtnTAPBASOutput.Checked = true; //2
                    txtbxFileOutputPath.Enabled = true;
                }
                this.ResumeLayout();
            }
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Assembler/Compiler/OutputOptions/TapBasOutputPath");
            if (node != null)
            {
                this.SuspendLayout();
                txtbxFileOutputPath.Text = node.InnerText;
                this.ResumeLayout();
            }

            AssemblerColorConfig.GetInstance().SaveGUIForUndo(); //save color settings
        }
        private Color ParseCss_GetColor(string i_cssString)
        {
            Regex regex = new Regex(@"(?!color:)#[0-9a-fA-F]{6}");
            Match match = regex.Match(i_cssString);
            if (match.Success)
            {
                return ColorTranslator.FromHtml(match.Value);
            }
            return Color.Black;
        }
        private bool ParseCss_IsItalic(string i_cssString)
        {
            //;font-style:oblique;
            return i_cssString.Contains(";font-style:oblique;");
        }
        private bool ParseCss_IsBold(string i_cssString)
        {
            //;font-weight:bold;
            return i_cssString.Contains(";font-weight:bold;");
        }
        private bool ParseCss_IsUnderline(string i_cssString)
        {
            //;text-decoration:underline;
            return i_cssString.Contains(";text-decoration:underline;");
        }
        private bool ParseCss_IsStrikeout(string i_cssString)
        {
            //;text-decoration:line-through;
            return i_cssString.Contains(";text-decoration:line-through;");
        }
        #endregion Config
    }
}
