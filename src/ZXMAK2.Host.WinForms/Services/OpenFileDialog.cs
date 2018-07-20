using System;
using System.ComponentModel;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.Entities;
using ZXMAK2.Host.WinForms.Tools;


namespace ZXMAK2.Host.WinForms.Services
{
    public class OpenFileDialog : IOpenFileDialog
    {
        private readonly System.Windows.Forms.OpenFileDialog _hostDialog;

        public OpenFileDialog()
        {
            _hostDialog = new System.Windows.Forms.OpenFileDialog();
            _hostDialog.SupportMultiDottedExtensions = true;
            _hostDialog.DefaultExt = string.Empty;
            _hostDialog.FileOk += (s, e) =>
            {
                var handler = FileOk;
                if (handler != null)
                {
                    handler(this, e);
                }
            };
        }

        public void Dispose()
        {
            _hostDialog.Dispose();
        }

        public event CancelEventHandler FileOk;

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

        public string FileName
        {
            get { return _hostDialog.FileName; }
            set { _hostDialog.FileName = value; }
        }

        public bool ShowReadOnly
        {
            get { return _hostDialog.ShowReadOnly; }
            set { _hostDialog.ShowReadOnly = value; }
        }

        public bool ReadOnlyChecked
        {
            get { return _hostDialog.ReadOnlyChecked; }
            set { _hostDialog.ReadOnlyChecked = value; }
        }

        public bool CheckFileExists
        {
            get { return _hostDialog.CheckFileExists; }
            set { _hostDialog.CheckFileExists = value; }
        }

        public bool Multiselect
        {
            get { return _hostDialog.Multiselect; }
            set { _hostDialog.Multiselect = value; }
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
