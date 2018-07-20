using System;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;

namespace ZXMAK2.Mvvm
{
    public class CommandDelegate : ICommand
    {
        private readonly Action<object> m_action;
        private readonly Func<object, bool> m_canExecute;
        private string m_text;
        private bool m_checked;

        public CommandDelegate(Action<object> action, Func<object, bool> canExecute, string text)
        {
            m_action = action;
            m_canExecute = canExecute;
            m_text = text;
        }

        public CommandDelegate(Action<object> action, Func<object, bool> canExecute)
            : this(action, canExecute, null)
        {
        }

        public CommandDelegate(Action<object> action)
            : this(action, (arg) => true)
        {
        }

        public CommandDelegate(Action action, Func<bool> canExecute, string text)
            : this((arg) => action(), (arg) => canExecute(), text)
        {
        }

        public CommandDelegate(Action action, Func<bool> canExecute)
            : this((arg) => action(), (arg) => canExecute(), null)
        {
        }

        public CommandDelegate(Action action)
            : this((arg) => action(), (arg) => true)
        {
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(Object parameter)
        {
            if (m_canExecute != null)
            {
                return m_canExecute(parameter);
            }
            return false;
        }

        public void Execute(Object parameter)
        {
            if (m_action != null)
            {
                m_action(parameter);
            }
        }

        public void Update()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        public string Text
        {
            get { return m_text; }
            set
            {
                if (m_text == value)
                {
                    return;
                }
                m_text = value;
                OnPropertyChanged("Text");
            }
        }

        public bool Checked
        {
            get { return m_checked; }
            set
            {
                if (m_checked == value)
                {
                    return;
                }
                m_checked = value;
                OnPropertyChanged("Checked");
            }
        }

        private void OnPropertyChanged(string propName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                var arg = new PropertyChangedEventArgs(propName);
                handler(this, arg);
            }
        }
    }
}
