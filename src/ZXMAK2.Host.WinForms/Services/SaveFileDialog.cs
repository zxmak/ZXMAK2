using System;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;
using ZXMAK2.Host.WinForms.Tools;


namespace ZXMAK2.Host.WinForms.Services
{
    public class SaveFileDialog : ISaveFileDialog
    {
        private readonly System.Windows.Forms.SaveFileDialog _hostDialog;

        public SaveFileDialog()
        {
            _hostDialog = new System.Windows.Forms.SaveFileDialog();
            _hostDialog.SupportMultiDottedExtensions = true;
        }

        public void Dispose()
        {
            //_hostDialog.Dispose();
        }

        public string Title
        {
            get { return _hostDialog.Title; }
            set { _hostDialog.Title = value; }
        }

        public string Filter
        {
            get { return _hostDialog.Filter; }
            set { _hostDialog.Filter = value; }
        }

        public string DefaultExt
        {
            get { return _hostDialog.DefaultExt; }
            set { _hostDialog.DefaultExt = value; }
        }

        public string FileName
        {
            get { return _hostDialog.FileName; }
            set { _hostDialog.FileName = value; }
        }

        public bool OverwritePrompt
        {
            get { return _hostDialog.OverwritePrompt; }
            set { _hostDialog.OverwritePrompt = value; }
        }

        public DlgResult ShowDialog(object owner)
        {
            var win32owner = owner as System.Windows.Forms.IWin32Window;
            if (owner != null)
            {
                return EnumMapper.GetDlgResult(_hostDialog.ShowDialog(win32owner));
            }
            else
            {
                return EnumMapper.GetDlgResult(_hostDialog.ShowDialog());
            }
        }
    }
}
