using System;
using System.Windows.Forms;
using ZXMAK2.Host.Entities;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Host.WinForms.Tools;


namespace ZXMAK2.Host.WinForms.Services
{
    public class UserQuery : IUserQuery
    {
        public DlgResult Show(
            String message,
            String caption,
            DlgButtonSet buttonSet,
            DlgIcon icon)
        {
            return EnumMapper.GetDlgResult(MessageBox.Show(
                message, caption,
                EnumMapper.GetMessageBoxButtons(buttonSet),
                EnumMapper.GetMessageBoxIcons(icon)));
        }

        public object ObjectSelector(object[] objArray, string caption)
        {
            if (objArray.Length < 1)
            {
                return null;
            }
            if (objArray.Length == 1)
            {
                return objArray[0];
            }
            using (var form = new ObjectSelectorDialog())
            {
                form.ItemArray = objArray;
                form.ItemSelected = objArray[0];
                form.Text = caption;
                form.ShowDialog();
                if (form.DialogResult == DialogResult.OK)
                {
                    return form.ItemSelected;
                }
                return null;
            }
        }

        public bool QueryText(string caption, string text, ref string value)
        {
            return InputBox.Query(caption, text, ref value);
        }

        public bool QueryValue(string caption, string text, string format, ref int value, int min, int max)
        {
            return InputBox.InputValue(caption, text, format, ref value, min, max);
        }


        #region InputBox

        private class InputBox : Form
        {
            private InputBox(string Caption, string Text)
            {
                this.label = new System.Windows.Forms.Label();
                this.textValue = new System.Windows.Forms.TextBox();
                this.buttonOK = new System.Windows.Forms.Button();
                this.buttonCancel = new System.Windows.Forms.Button();
                this.SuspendLayout();

                // 
                // label
                // 
                this.label.AutoSize = true;
                this.label.Location = new System.Drawing.Point(9, 13);
                this.label.Name = "label";
                this.label.Size = new System.Drawing.Size(31, 13);
                this.label.TabIndex = 1;
                this.label.Text = Text;
                // 
                // textValue
                // 
                this.textValue.Location = new System.Drawing.Point(12, 31);
                this.textValue.Name = "textValue";
                this.textValue.Size = new System.Drawing.Size(245, 20);
                this.textValue.TabIndex = 2;
                this.textValue.WordWrap = false;
                // 
                // buttonOK
                // 
                this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.buttonOK.Location = new System.Drawing.Point(57, 67);
                this.buttonOK.Name = "buttonOK";
                this.buttonOK.Size = new System.Drawing.Size(75, 23);
                this.buttonOK.TabIndex = 3;
                this.buttonOK.Text = "OK";
                this.buttonOK.UseVisualStyleBackColor = true;
                // 
                // buttonCancel
                // 
                this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.buttonCancel.Location = new System.Drawing.Point(138, 67);
                this.buttonCancel.Name = "buttonCancel";
                this.buttonCancel.Size = new System.Drawing.Size(75, 23);
                this.buttonCancel.TabIndex = 4;
                this.buttonCancel.Text = "Cancel";
                this.buttonCancel.UseVisualStyleBackColor = true;
                // 
                // Form
                // 
                this.AcceptButton = this.buttonOK;
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                this.CancelButton = this.buttonCancel;
                this.ClientSize = new System.Drawing.Size(270, 103);
                this.Controls.Add(this.buttonCancel);
                this.Controls.Add(this.buttonOK);
                this.Controls.Add(this.textValue);
                this.Controls.Add(this.label);
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
                this.MaximizeBox = false;
                this.MinimizeBox = false;
                this.Name = "InputBox";
                this.ShowIcon = false;
                this.ShowInTaskbar = false;
                this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                this.Text = Caption;
                this.ResumeLayout(false);
                this.PerformLayout();
            }

            public static bool Query(
                //IWin32Window owner,
                string Caption,
                string Text,
                ref string s_val)
            {
                using (var ib = new InputBox(Caption, Text))
                {
                    ib.textValue.Text = s_val;
                    if (ib.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        return false;
                    }
                    s_val = ib.textValue.Text;
                }
                return true;
            }

            public static bool InputValue(
                //IWin32Window owner,
                string Caption,
                string Text,
                string format,
                ref int value,
                int min,
                int max)
            {
                int val = value;
                string s_val = string.Format(format, value);
                bool OKVal;
                do
                {
                    OKVal = true;
                    if (!Query(Caption, Text, ref s_val))
                    {
                        return false;
                    }
                    try
                    {
                        var sTr = s_val.Trim();
                        if ((sTr.Length > 0) && (sTr[0] == '#'))
                        {
                            sTr = sTr.Remove(0, 1);
                            val = Convert.ToInt32(sTr, 16);
                            //                  s_val = "0x" + s_val;
                        }
                        else if ((sTr.Length > 1) && ((sTr[1] == 'x') && (sTr[0] == '0')))
                        {
                            sTr = sTr.Remove(0, 2);
                            val = Convert.ToInt32(sTr, 16);
                        }
                        else
                        {
                            val = Convert.ToInt32(sTr, 10);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Numeric value required!");
                        OKVal = false;
                    }
                    if ((val < min) || (val > max))
                    {
                        MessageBox.Show("Numeric value should be int the following range: " + min.ToString() + "..." + max.ToString() + " !");
                        OKVal = false;
                    }
                } while (!OKVal);
                value = val;
                return true;
            }

            private System.Windows.Forms.Label label;
            private System.Windows.Forms.TextBox textValue;
            private System.Windows.Forms.Button buttonOK;
            private System.Windows.Forms.Button buttonCancel;
        }

        #endregion InputBox
    }
}
