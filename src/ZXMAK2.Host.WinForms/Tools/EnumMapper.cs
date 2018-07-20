using System;
using ZXMAK2.Host.Entities;
using System.Windows.Forms;
using System.Globalization;


namespace ZXMAK2.Host.WinForms.Tools
{
    public static class EnumMapper
    {
        public static DlgResult GetDlgResult(DialogResult value)
        {
            switch (value)
            {
                case DialogResult.Abort: return DlgResult.Abort;
                case DialogResult.Cancel: return DlgResult.Cancel;
                case DialogResult.Ignore: return DlgResult.Ignore;
                case DialogResult.No: return DlgResult.No;
                case DialogResult.None: return DlgResult.None;
                case DialogResult.OK: return DlgResult.OK;
                case DialogResult.Retry: return DlgResult.Retry;
                case DialogResult.Yes: return DlgResult.Yes;
                default: return ThrowArgumentError<DlgResult>(value);
            }
        }

        public static MessageBoxButtons GetMessageBoxButtons(DlgButtonSet value)
        {
            switch (value)
            {
                case DlgButtonSet.AbortRetryIgnore: return MessageBoxButtons.AbortRetryIgnore;
                case DlgButtonSet.OK: return MessageBoxButtons.OK;
                case DlgButtonSet.OKCancel: return MessageBoxButtons.OKCancel;
                case DlgButtonSet.RetryCancel: return MessageBoxButtons.RetryCancel;
                case DlgButtonSet.YesNo: return MessageBoxButtons.YesNo;
                case DlgButtonSet.YesNoCancel: return MessageBoxButtons.YesNoCancel;
                default: return ThrowArgumentError<MessageBoxButtons>(value);
            }
        }

        public static MessageBoxIcon GetMessageBoxIcons(DlgIcon value)
        {
            switch (value)
            {
                case DlgIcon.Error: return MessageBoxIcon.Error;
                case DlgIcon.Information: return MessageBoxIcon.Information;
                case DlgIcon.None: return MessageBoxIcon.None;
                case DlgIcon.Question: return MessageBoxIcon.Question;
                case DlgIcon.Warning: return MessageBoxIcon.Warning;
                default: return ThrowArgumentError<MessageBoxIcon>(value);
            }
        }

        private static T ThrowArgumentError<T>(Enum value)
        {
            throw new ArgumentException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Unexpected enum value: {0}.{1}",
                    typeof(T).Name,
                    value));
        }
    }
}
