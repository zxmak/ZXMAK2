/* 
 *  Copyright 2015 Alex Makeev
 * 
 *  This file is part of ZXMAK2 (ZX Spectrum virtual machine).
 *
 *  ZXMAK2 is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  ZXMAK2 is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with ZXMAK2.  If not, see <http://www.gnu.org/licenses/>.
 *  
 * 
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using ZXMAK2.Mvvm.BindingTools;
using System.ComponentModel;


namespace ZXMAK2.Host.WinForms.BindingTools
{
    public class ControlBindingAdapter : BaseBindingAdapter
    {
        public ControlBindingAdapter(Control control)
            : base(control)
        {
            control.Validating += Control_OnValidating;
            var textBox = control as TextBox;
            if (textBox != null)
            {
                textBox.TextChanged += Control_OnTextChanged;
            }
            var checkBox = control as CheckBox;
            if (checkBox != null)
            {
                checkBox.CheckedChanged += CheckBox_OnCheckedChanged;
                checkBox.CheckStateChanged += CheckBox_OnCheckStateChanged;
            }
            var notify = control as INotifyPropertyChanged;
            if (notify != null)
            {
                notify.PropertyChanged += Control_OnPropertyChanged;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;
            var control = (Control)Target;
            
            control.Validating -= Control_OnValidating;
            var textBox = control as TextBox;
            if (textBox != null)
            {
                textBox.TextChanged -= Control_OnTextChanged;
            }
            var checkBox = control as CheckBox;
            if (checkBox != null)
            {
                checkBox.CheckedChanged -= CheckBox_OnCheckedChanged;
                checkBox.CheckStateChanged -= CheckBox_OnCheckStateChanged;
            }
            var notify = control as INotifyPropertyChanged;
            if (notify != null)
            {
                notify.PropertyChanged -= Control_OnPropertyChanged;
            }
        }


        public override BindingTrigger GetDefaultPropertyTrigger(string name)
        {
            if (name == "Text" &&
                typeof(TextBox).IsAssignableFrom(TargetType))
            {
                return BindingTrigger.LostFocus;
            }
            return base.GetDefaultPropertyTrigger(name);
        }

        public override void SetTargetPropertyValue(string name, object value)
        {
            // cache property set to eliminate redundant UI updates
            if (name == "Text")
            {
                var control = Target as Control;
                if (control != null)
                {
                    if (control.Text != (string)value)
                        control.Text = (string)value;
                    return;
                }
            }
            if (name == "Checked")
            {
                var control = Target as CheckBox;
                if (control != null)
                {
                    if (control.Checked != (bool)value)
                        control.Checked = (bool)value;
                    return;
                }
            }
            if (name == "CheckState")
            {
                var control = Target as CheckBox;
                if (control != null)
                {
                    if (control.CheckState != (CheckState)value)
                        control.CheckState = (CheckState)value;
                    return;
                }
            }
            base.SetTargetPropertyValue(name, value);
        }

        private void Control_OnValidating(object sender, CancelEventArgs e)
        {
            try
            {
                var textBox = Target as TextBox;
                if (textBox != null)
                {
                    OnPropertyChanged("Text", BindingTrigger.LostFocus);
                }
                var checkBox = Target as CheckBox;
                if (checkBox != null)
                {
                    OnPropertyChanged("Checked", BindingTrigger.LostFocus);
                    OnPropertyChanged("CheckState", BindingTrigger.LostFocus);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                e.Cancel = true;    // invalid value
            }
        }

        private void Control_OnTextChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Text", BindingTrigger.PropertyChanged);
        }

        private void CheckBox_OnCheckedChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("Checked", BindingTrigger.PropertyChanged);
        }

        private void CheckBox_OnCheckStateChanged(object sender, EventArgs e)
        {
            OnPropertyChanged("CheckState", BindingTrigger.PropertyChanged);
        }

        private void Control_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName, BindingTrigger.PropertyChanged);
        }
    }
}
