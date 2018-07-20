using FastColoredTextBoxNS;

namespace ZXMAK2.Hardware.Adlers.Views.CustomControls
{
    public partial class SourceCodeEditorBox : FastColoredTextBox
    {
        private bool _isUpdating = false;

        public SourceCodeEditorBox()
        {
            InitializeComponent();
        }

        public void RefreshRtf()
        {
            SuspendLayout();
            _isUpdating = true;
            string textCurr = this.Text;
            ClearStylesBuffer();
            Clear();
            this.Text = textCurr;
            _isUpdating = false;
            ResumeLayout();
        }

        #region overrides
        public override void Clear()
        {
            //_isUpdating = true;
            base.Clear();
            //_isUpdating = false;
        }
        public override void OnTextChanging(ref string text)
        {
            if (_isUpdating)
                return;
            base.OnTextChanging(ref text);
        }
        protected override void OnTextChanged(TextChangedEventArgs args)
        {
            if (_isUpdating)
                return;
            base.OnTextChanged(args);
        }
        #endregion overrides
    }
}
