using FastColoredTextBoxNS;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ZXMAK2.Hardware.Adlers.Views.CustomControls;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace ZXMAK2.Hardware.Adlers.Views.AssemblerView
{
    public partial class AssemblerColorConfig : Form
    {
        private static AssemblerColorConfig _instance = null; //this
        private static Assembler _assemblerForm;

        private bool _eventsDisabled = false;

        //set preview assembler text
        private static readonly string _TEST_TEXT = 
                          "       org 40000 ; this is a comment\n" +
                          "label: xor a\n" +
                          "       push bc\n" +
                          "       jp #4455\n\n" +
                          "defb DefinedByte 0\n" +
                          "       ld ix, 0xAF05\n" +
                          "       ld a, %11100010\n" +
                          "       ret\n" +
                          "macroBorderRandom MACRO var1\n" +
                          "ld a, var1: out(#fe),a\nENDM\n" +
                          "ld b, 10: djnz $43";

        private AssemblerColorConfig()
        {
            InitializeComponent();
            InitSyntaxHighlightningStyles();

            fctbxPreview.Text = _TEST_TEXT;
        }
        public static void ShowForm()
        {
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new AssemblerColorConfig();
                //_instance.LoadConfig();
                _instance.ShowInTaskbar = false;
            }
            _instance.ShowDialog();
        }

        public static AssemblerColorConfig GetInstance()
        {
            if (_instance == null)
                _instance = new AssemblerColorConfig();
            return _instance;
        }

        public static void Init(Assembler i_assemblerForm)
        {
            _assemblerForm = i_assemblerForm;
        }

        #region Syntax highlightning styles
            //comments
            public static Style  CommentStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
            public static string regexComment = @";.*";
            //common instructions
            public static Style  CommonInstructionStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
            public static string regexCommonInstruction = @"\bldir\b|\blddr\b|\bld\b|\bim[ ]+\b|\badd\b|\bsub\b|\bsbc\b|\bhalt\b|\bbit\b|" +
                                                          @"\bset\b|xor|[^.a-zA-Z0-9:_\/\[\]](inc|dec)[ ]|\bcp\b|\bcpl\b|\bei\b|\bdi\b|\band\b|\bor\b|\band\b" +
                                                          @"|\brr\b|\bscf\b|\bccf\b|\bneg\b|\bsrl\b|exx|\bex\b|\brla\b|\brra\b|\brr\b|\bout\b|\bin\b|\bsla\b|\brl\b|\brrca\b" + 
                                                          @"|\brlca\b";
            //jumps
            public static Style  JumpInstructionStyle = new TextStyle(Brushes.DarkViolet, null, FontStyle.Regular);
            public static string regexJumpInstruction = @"\breti\b|\bretn\b|\bret\b|\bjp\b|\bjr\b|\bcall\b|\bdjnz\b";
            //stack
            public static Style  StackInstructionStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
            public static string regexStackInstruction = @"\bpush\b|\bpop\b|\bdec sp\b|\binc sp\b";
            //registry
            public static Style  styleRegistry = new TextStyle(Brushes.DarkRed, null, FontStyle.Regular);
            public static string regexRegistry = @"\bhl\b|(?<=[ ,(])BC(?=[ ,)\n;\r])|\bix\b|\biy\b|\bde\b|\bpc\b|\bsp\b|[\( ,]\b(IR|HL)\b|(?<=[ ,(])AF(?=[ ,)\n;\r])";
            //compiler directives
            public static Style  CompilerDirectivesStyle = new TextStyle(Brushes.SaddleBrown, null, FontStyle.Italic);
            public static string regexCompilerDirectives = @"\borg\b|\bdefb\b|\bdefw\b|\bdefl\b|\bdefm\b|\bdefs\b|\bequ\b|\bmacro\b|\bendm\b|include|incbin|" +
                                                           @"\bif\b|\bendif\b|\belse\b";
            //numbers
            public static Style  NumbersStyle = new TextStyle(Brushes.DarkCyan, null, FontStyle.Regular);
            public static string regexNumbers = @"(?:\(|\n|,| |\+|\-|\*|\/)\d{1,5}\b|[^a-zA-Z](?:x|#|\$)[0-9A-Fa-f]{1,4}|%[0-1]{1,16}";
        
        public void InitSyntaxHighlightningStyles()
        {
            CommentStyle = new TextStyle(Brushes.DarkGreen, null, FontStyle.Italic);
            CommonInstructionStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
            JumpInstructionStyle = new TextStyle(Brushes.Salmon, null, FontStyle.Regular);
            StackInstructionStyle = new TextStyle(Brushes.DarkViolet, null, FontStyle.Regular);
            colorPickerStackInstructions.SelectedValue = Color.DarkViolet;
            CompilerDirectivesStyle = new TextStyle(Brushes.SaddleBrown, null, FontStyle.Italic);
            //styleRegistry = new TextStyle(Brushes.DarkRed, null, FontStyle.Regular);
            NumbersStyle = new TextStyle(Brushes.DarkCyan, null, FontStyle.Regular);

            colorPickerCommonInstructions.SelectedValue = Color.Blue;
            colorPickerJumps.SelectedValue = Color.Salmon;
            colorPickerCompilerDirectives.SelectedValue = Color.Fuchsia;
            colorPickerComments.SelectedValue = Color.DarkGreen;
            colorPickerNumbers.SelectedValue = Color.Teal;
        }
        public static void RefreshControlStyles( object i_fctxbBox, TextChangedEventArgs e)
        {
            Range range;
            if (i_fctxbBox is SourceCodeEditorBox)
            {
                (i_fctxbBox as SourceCodeEditorBox).RefreshRtf();
                range = (e == null ? (i_fctxbBox as SourceCodeEditorBox).Range : e.ChangedRange);
            }
            else
                range = (e == null ? (i_fctxbBox as FastColoredTextBox).Range : e.ChangedRange);

            range.ClearStyle(CompilerDirectivesStyle);
            range.ClearStyle(CommentStyle);
            range.ClearStyle(NumbersStyle);
            range.ClearStyle(CommonInstructionStyle);
            range.ClearStyle(JumpInstructionStyle);
            //range.ClearStyle(styleRegistry);
            range.ClearStyle(StackInstructionStyle);
            ( i_fctxbBox as FastColoredTextBox).ClearStylesBuffer();

            if (Settings.IsSyntaxHighlightningOn() && _instance != null)
            {
                //keep style order! highest priority has first style added.
                if (_instance.IsCommentsColorEnabled())
                    range.SetStyle(CommentStyle, regexComment);
                //range.SetStyle(styleRegistry, regexRegistry, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (_instance.IsNumbersStyleEnabled())
                    range.SetStyle(NumbersStyle, regexNumbers);
                if (_instance.IsCompilerDirectivesEnabled())
                    range.SetStyle(CompilerDirectivesStyle, regexCompilerDirectives, RegexOptions.IgnoreCase);
                if( _instance.IsCommonInstructionsStyleEnabled() )
                    range.SetStyle(CommonInstructionStyle, regexCommonInstruction, RegexOptions.IgnoreCase);
                if (_instance.IsJumpStyleEnabled())
                    range.SetStyle(JumpInstructionStyle, regexJumpInstruction, RegexOptions.IgnoreCase);
                if (_instance.IsStackInstructionsStyleEnabled())
                    range.SetStyle(StackInstructionStyle, regexStackInstruction, RegexOptions.IgnoreCase);
            }
        }
        #endregion Syntax highlightning styles

        private void fastColoredPreview_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshControlStyles(sender, e);
        }

        //Comments style modification
        public void ChangeSyntaxStyle(bool i_isEnabled = false, TextStyle i_textStyleDynamic = null, int i_styleId = -1)
        {
            if (i_styleId == 0)
            {
                _eventsDisabled = true;
                this.chckbxCommentsColorEnabled.Checked = i_isEnabled;
                this.colorPickerComments.SelectedValue = ((SolidBrush)i_textStyleDynamic.ForeBrush).Color;
                this.checkBoxCommentsItalic.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Italic);
                this.checkBoxCommentsBold.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Bold);
                this.checkBoxCommentsStrikeOut.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Strikeout);
                this.checkBoxCommentsUnderline.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Underline);
                _eventsDisabled = false;
            }
            if (i_styleId == 1)
            {
                //compiler directive style
                _eventsDisabled = true;
                this.chcbxCompilerDirectivesEnabled.Checked = i_isEnabled;
                this.colorPickerCompilerDirectives.SelectedValue = ((SolidBrush)i_textStyleDynamic.ForeBrush).Color;
                this.checkBoxCompilerDirectivesItalic.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Italic);
                this.checkBoxCompilerDirectivesBold.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Bold);
                this.checkBoxCompilerDirectivesStrikeout.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Strikeout);
                this.checkBoxCompilerDirectivesUnderline.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Underline);
                _eventsDisabled = false;
            }
            if (i_styleId == 2)
            {
                //jumps style
                _eventsDisabled = true;
                this.chcbxJumpsEnabled.Checked = i_isEnabled;
                this.colorPickerJumps.SelectedValue = ((SolidBrush)i_textStyleDynamic.ForeBrush).Color;
                this.checkBoxJumpsItalic.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Italic);
                this.checkBoxJumpsBold.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Bold);
                this.checkBoxJumpsStrikeout.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Strikeout);
                this.checkBoxJumpsUnderline.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Underline);
                _eventsDisabled = false;
            }
            if (i_styleId == 3)
            {
                //common instruction style
                _eventsDisabled = true;
                this.chcbxCommonInstructionsEnabled.Checked = i_isEnabled;
                this.colorPickerCommonInstructions.SelectedValue = ((SolidBrush)i_textStyleDynamic.ForeBrush).Color;
                this.checkBoxCommonInstructionsItalic.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Italic);
                this.checkBoxCommonInstructionsBold.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Bold);
                this.checkBoxCommonInstructionsStrikeout.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Strikeout);
                this.checkBoxCommonInstructionsUnderline.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Underline);
                _eventsDisabled = false;
            }
            if (i_styleId == 4) //numbers style
            {
                _eventsDisabled = true;
                this.chcbxNumbersEnabled.Checked = i_isEnabled;
                this.colorPickerNumbers.SelectedValue = ((SolidBrush)i_textStyleDynamic.ForeBrush).Color;
                this.checkBoxNumbersItalic.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Italic);
                this.checkBoxNumbersBold.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Bold);
                this.checkBoxNumbersStrikeout.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Strikeout);
                this.checkBoxNumbersUnderline.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Underline);
                _eventsDisabled = false;
            }
            if (i_styleId == 5) //stack instructions style
            {
                _eventsDisabled = true;
                this.chcbxStackInstructionsEnabled.Checked = i_isEnabled;
                this.colorPickerStackInstructions.SelectedValue = ((SolidBrush)i_textStyleDynamic.ForeBrush).Color;
                this.checkBoxStackInstructionsItalic.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Italic);
                this.checkBoxStackInstructionsBold.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Bold);
                this.checkBoxStackInstructionsStrikeout.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Strikeout);
                this.checkBoxStackInstructionsUnderline.Checked = i_textStyleDynamic.FontStyle.HasFlag(FontStyle.Underline);
                _eventsDisabled = false;
            }

            //comments
            FontStyle style = new FontStyle();
            if (this.checkBoxCommentsItalic.Checked)
                style |= FontStyle.Italic;
            if (this.checkBoxCommentsBold.Checked)
                style |= FontStyle.Bold;
            if (this.checkBoxCommentsStrikeOut.Checked)
                style |= FontStyle.Strikeout;
            if (this.checkBoxCommentsUnderline.Checked)
                style |= FontStyle.Underline;
            CommentStyle = new TextStyle(new SolidBrush(colorPickerComments.SelectedValue), null, style);
            //compiler directives
            style = new FontStyle();
            if (this.checkBoxCompilerDirectivesItalic.Checked)
                style |= FontStyle.Italic;
            if (this.checkBoxCompilerDirectivesBold.Checked)
                style |= FontStyle.Bold;
            if (this.checkBoxCompilerDirectivesStrikeout.Checked)
                style |= FontStyle.Strikeout;
            if (this.checkBoxCompilerDirectivesUnderline.Checked)
                style |= FontStyle.Underline;
            CompilerDirectivesStyle = new TextStyle(new SolidBrush(colorPickerCompilerDirectives.SelectedValue), null, style);
            //jumps
            style = new FontStyle();
            if (this.checkBoxJumpsItalic.Checked)
                style |= FontStyle.Italic;
            if (this.checkBoxJumpsBold.Checked)
                style |= FontStyle.Bold;
            if (this.checkBoxJumpsStrikeout.Checked)
                style |= FontStyle.Strikeout;
            if (this.checkBoxJumpsUnderline.Checked)
                style |= FontStyle.Underline;
            JumpInstructionStyle = new TextStyle(new SolidBrush(colorPickerJumps.SelectedValue), null, style);
            //common instructions style
            style = new FontStyle();
            if (this.checkBoxCommonInstructionsItalic.Checked)
                style |= FontStyle.Italic;
            if (this.checkBoxCommonInstructionsBold.Checked)
                style |= FontStyle.Bold;
            if (this.checkBoxCommonInstructionsStrikeout.Checked)
                style |= FontStyle.Strikeout;
            if (this.checkBoxCommonInstructionsUnderline.Checked)
                style |= FontStyle.Underline;
            CommonInstructionStyle = new TextStyle(new SolidBrush(colorPickerCommonInstructions.SelectedValue), null, style);
            //numbers style
            style = new FontStyle();
            if (this.checkBoxNumbersItalic.Checked)
                style |= FontStyle.Italic;
            if (this.checkBoxNumbersBold.Checked)
                style |= FontStyle.Bold;
            if (this.checkBoxNumbersStrikeout.Checked)
                style |= FontStyle.Strikeout;
            if (this.checkBoxNumbersUnderline.Checked)
                style |= FontStyle.Underline;
            NumbersStyle = new TextStyle(new SolidBrush(colorPickerNumbers.SelectedValue), null, style);
            //stack instructions style
            style = new FontStyle();
            if (this.checkBoxStackInstructionsItalic.Checked)
                style |= FontStyle.Italic;
            if (this.checkBoxStackInstructionsBold.Checked)
                style |= FontStyle.Bold;
            if (this.checkBoxStackInstructionsStrikeout.Checked)
                style |= FontStyle.Strikeout;
            if (this.checkBoxStackInstructionsUnderline.Checked)
                style |= FontStyle.Underline;
            StackInstructionStyle = new TextStyle(new SolidBrush(colorPickerStackInstructions.SelectedValue), null, style);
            
            RefreshControlStyles(this.fctbxPreview, null);

            if (i_textStyleDynamic != null)
                _assemblerForm.RefreshAssemblerCodeSyntaxHighlightning();
        }

        #region GUI
        //Save
        private void btnSave_Click(object sender, EventArgs e)
        {
            _assemblerForm.RefreshAssemblerCodeSyntaxHighlightning();
            SaveGUIForUndo();
        }

        private Dictionary<Control, object/*old value*/> _undoObjects;

        public IEnumerable<Control> GetAllControls(Control control, Type type = null)
        {
            var controls = control.Controls.Cast<Control>();
            if (type == null)
                return controls.SelectMany(ctrl => GetAllControls(ctrl, type)).Concat(controls);
            else
                return controls.SelectMany(ctrl => GetAllControls(ctrl, type)).Concat(controls).Where(c => c.GetType() == type);
        }

        public void SaveGUIForUndo()
        {
            _undoObjects = new Dictionary<Control, object>();
            foreach( Control ctrl in GetAllControls(this, typeof(CheckBox)))
            {
                _undoObjects.Add(ctrl, ((CheckBox)ctrl).Checked ? true : false);
            }
            foreach (Control ctrl in GetAllControls(this, typeof(ColorPicker)))
            {
                _undoObjects.Add(ctrl, ((ColorPicker)ctrl).SelectedValue);
            }
        }
        public void Undo()
        {
            if (_undoObjects == null)
                return;
            foreach( KeyValuePair<Control, object> control in _undoObjects )
            {
                Control[] foundControls = this.Controls.Find(control.Key.Name, true);
                if( foundControls.Length > 0 )
                {
                    if (foundControls[0] is CheckBox) //checkboxes
                        ((CheckBox)foundControls[0]).Checked = (bool)control.Value;
                    if (foundControls[0] is ColorPicker) //checkboxes
                        ((ColorPicker)foundControls[0]).SelectedValue = (Color)control.Value;
                }
            }
        }

        //Undo
        private void btnUndo_Click(object sender, System.EventArgs e)
        {
            Undo();
        }

        //Exit
        private void AssemblerColorConfig_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Hide();
        }
        private void btnExit_Click(object sender, System.EventArgs e)
        {
            this.Hide();
        }

        //comments controls(combo and checkbox)
        private void colorPickerComments_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        private void checkBoxCommentsItalic_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        private void checkBoxCommentsBold_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        private void checkBoxCommentsStrikeOut_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        private void checkBoxCommentsUnderline_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        //Compiler directive controls(combo and checkbox)
        private void colorPickerCompilerDirectives_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        private void checkBoxCompilerDirectivesItalic_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        private void checkBoxCompilerDirectivesBold_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        private void checkBoxCompilerDirectivesStrikeout_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        private void checkBoxCompilerDirectivesUnderline_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        //Jumps controls(combo and checkbox)
        private void colorPickerJumps_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        private void checkBoxJumpsItalic_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        private void checkBoxJumpsBold_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        private void checkBoxJumpsStrikeout_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        private void checkBoxJumpsUnderline_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        private void colorPickerCommonInstructions_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        private void checkBoxCommonInstructionsItalic_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        private void checkBoxCommonInstructionsBold_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        private void checkBoxCommonInstructionsStrikeout_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        private void checkBoxCommonInstructionsUnderline_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        private void colorPickerNumbers_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        private void checkBoxNumbersItalic_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        private void checkBoxNumbersBold_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        private void checkBoxNumbersStrikeout_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        private void checkBoxNumbersUnderline_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        //comments enabled
        private void chckbxCommentsColorEnabled_CheckedChanged(object sender, System.EventArgs e)
        {
            colorPickerComments.Enabled = checkBoxCommentsItalic.Enabled = checkBoxCommentsBold.Enabled = checkBoxCommentsStrikeOut.Enabled =
                checkBoxCommentsUnderline.Enabled = chckbxCommentsColorEnabled.Checked;
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        public bool IsCommentsColorEnabled()
        {
            return chckbxCommentsColorEnabled.Checked;
        }

        //compiler directives enabled
        private void chcbxCompilerDirectivesEnabled_CheckedChanged(object sender, System.EventArgs e)
        {
            colorPickerCompilerDirectives.Enabled = checkBoxCompilerDirectivesItalic.Enabled = checkBoxCompilerDirectivesBold.Enabled = checkBoxCompilerDirectivesStrikeout.Enabled =
                checkBoxCompilerDirectivesUnderline.Enabled = chcbxCompilerDirectivesEnabled.Checked;
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        public bool IsCompilerDirectivesEnabled()
        {
            return chcbxCompilerDirectivesEnabled.Checked;
        }

        //jumps enabled
        private void chcbxJumpsEnabled_CheckedChanged(object sender, System.EventArgs e)
        {
            colorPickerJumps.Enabled = checkBoxJumpsItalic.Enabled = checkBoxJumpsBold.Enabled = checkBoxJumpsStrikeout.Enabled =
                checkBoxJumpsUnderline.Enabled = chcbxJumpsEnabled.Checked;
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        public bool IsJumpStyleEnabled()
        {
            return chcbxJumpsEnabled.Checked;
        }

        //common instructions enabled
        private void chcbxCommonInstructionsEnabled_CheckedChanged(object sender, System.EventArgs e)
        {
            colorPickerCommonInstructions.Enabled = checkBoxCommonInstructionsItalic.Enabled = checkBoxCommonInstructionsBold.Enabled = checkBoxCommonInstructionsStrikeout.Enabled =
                checkBoxCommonInstructionsUnderline.Enabled = chcbxCommonInstructionsEnabled.Checked;
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        public bool IsCommonInstructionsStyleEnabled()
        {
            return chcbxCommonInstructionsEnabled.Checked;
        }

        //numbers instructions enabled
        private void chcbxNumbersEnabled_CheckedChanged(object sender, System.EventArgs e)
        {
            colorPickerNumbers.Enabled = checkBoxNumbersItalic.Enabled = checkBoxNumbersBold.Enabled = checkBoxNumbersStrikeout.Enabled =
                checkBoxNumbersUnderline.Enabled = chcbxNumbersEnabled.Checked;
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }
        public bool IsNumbersStyleEnabled()
        {
            return chcbxNumbersEnabled.Checked;
        }

        //stack instruction style
        private void chcbxStackInstructionsEnabled_CheckedChanged(object sender, System.EventArgs e)
        {
            colorPickerStackInstructions.Enabled = checkBoxStackInstructionsItalic.Enabled = checkBoxStackInstructionsBold.Enabled = checkBoxStackInstructionsStrikeout.Enabled =
                checkBoxStackInstructionsUnderline.Enabled = chcbxStackInstructionsEnabled.Checked;
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        private void colorPickerStackInstructions_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        private void checkBoxStackInstructionsItalic_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        private void checkBoxStackInstructionsBold_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        private void checkBoxStackInstructionsStrikeout_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        private void checkBoxStackInstructionsUnderline_CheckedChanged(object sender, System.EventArgs e)
        {
            if (!_eventsDisabled)
                ChangeSyntaxStyle();
        }

        public bool IsStackInstructionsStyleEnabled()
        {
            return chcbxStackInstructionsEnabled.Checked;
        }
        #endregion GUI
    }
}
