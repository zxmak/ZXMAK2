using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZXMAK2.Mvvm;
using ZXMAK2.Mvvm.BindingTools;

namespace ZXMAK2.Host.WinForms.BindingTools
{
    public class ToolStripItemBindingAdapter : BaseBindingAdapter
    {
        public ToolStripItemBindingAdapter(ToolStripItem control)
            : base(control)
        {
            control.Click += ToolStrip_OnClick;
            CommandParameter = control.Tag;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;
            if (Command != null)
            {
                Command.CanExecuteChanged -= Command_OnCanExecuteChanged;
            }
            var control = (ToolStripItem)Target;
            control.Click -= ToolStrip_OnClick;    
        }

        public override Type GetTargetPropertyType(string name)
        {
            if (name == "Command")  // virtual property
            {
                return typeof(ICommand);
            }
            return base.GetTargetPropertyType(name);
        }

        public override object GetTargetPropertyValue(string name)
        {
            if (name == "Command")  // virtual property
            {
                return Command;
            }
            return base.GetTargetPropertyValue(name);
        }

        public override void SetTargetPropertyValue(string name, object value)
        {
            if (name == "Command")  // virtual property
            {
                Command = (ICommand)value;
                return;
            }
            if (name == "CommandParameter")  // virtual property
            {
                CommandParameter = value;
                return;
            }
            // cache property set to eliminate redundant UI updates
            if (name == "Text")
            {
                var control = (ToolStripItem)Target;
                if (control != null)
                {
                    if (control.Text != (string)value)
                        control.Text = (string)value;
                    return;
                }
            }
            if (name == "Image")
            {
                var control = (ToolStripItem)Target;
                if (control != null)
                {
                    if (control.Image != (Image)value)
                        control.Image = (Image)value;
                    return;
                }
            }
            if (name == "Visible")
            {
                var control = (ToolStripItem)Target;
                if (control != null)
                {
                    // cache not allowed
                    //if (control.Visible != (bool)value)
                        control.Visible = (bool)value;
                    return;
                }
            }
            if (name == "Checked")
            {
                var menuItem = Target as ToolStripMenuItem;
                if (menuItem != null)
                {
                    if (menuItem.Checked != (bool)value)
                        menuItem.Checked = (bool)value;
                    return;
                }
                var button = Target as ToolStripButton;
                if (button != null)
                {
                    if (button.Checked != (bool)value)
                        button.Checked = (bool)value;
                    return;
                }
            }
            base.SetTargetPropertyValue(name, value);
        }

        // Virtual properties
        private object _commandParameter;

        public object CommandParameter
        {
            get { return _commandParameter; }
            set
            {
                if (_commandParameter == value)
                {
                    return;
                }
                _commandParameter = value;
                Command_OnCanExecuteChanged(_command, EventArgs.Empty);
            }
        }

        private ICommand _command;

        public ICommand Command
        {
            get { return _command; }
            set 
            {
                if (_command == value)
                {
                    return;
                }
                if (_command != null)
                {
                    _command.CanExecuteChanged -= Command_OnCanExecuteChanged;
                }
                _command = value;
                if (_command != null)
                {
                    _command.CanExecuteChanged += Command_OnCanExecuteChanged;
                    Command_OnCanExecuteChanged(_command, EventArgs.Empty);
                }
            }
        }

        private void Command_OnCanExecuteChanged(object sender, EventArgs e)
        {
            if (Command != null)
            {
                var canExecute = Command.CanExecute(CommandParameter);
                var control = (ToolStripItem)Target;
                if (control.Enabled != canExecute)
                    control.Enabled = canExecute;
            }
        }

        private void ToolStrip_OnClick(object sender, EventArgs e)
        {
            if (Command != null && Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }
        }
    }
}
