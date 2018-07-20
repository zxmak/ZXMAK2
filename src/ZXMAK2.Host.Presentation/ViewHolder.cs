using System;
using System.ComponentModel;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Presentation.Interfaces;
using ZXMAK2.Mvvm;


namespace ZXMAK2.Host.Presentation
{
    public class ViewHolder<T> : IViewHolder
        where T : IView
    {
        private readonly string m_name;
        private Argument[] m_args;
        private IMainView m_hostView;
        private ICommand m_command;
        private T m_view;
        private bool m_canClose;


        public ViewHolder(
            string name, 
            params Argument[] args)
        {
            m_name = name;
            m_args = args;
        }

        public Argument[] Arguments
        {
            get { return m_args; }
            set { m_args = value; }
        }

        public ICommand CommandOpen
        {
            get
            {
                if (m_command != null)
                {
                    return m_command;
                }
                m_command = new CommandDelegate(Command_OnExecute, Command_OnCanExecute, m_name);
                return m_command;
            }
        }

        public void Close()
        {
            if (m_view != null)
            {
                m_canClose = true;
                m_view.Close();
                m_view = default(T);
                m_command = null;
            }
        }

        private void CreateTargetForm()
        {
            m_canClose = false;
            var viewResolver = Locator.Resolve<IResolver>("View");
            if (m_args != null && m_args.Length > 0)
            {
                m_view = viewResolver.Resolve<T>(m_args);
            }
            else
            {
                m_view = viewResolver.Resolve<T>();
            }
            m_view.ViewClosed += (s, e) =>
            {
                m_view = default(T);
            };
            m_view.ViewClosing += (s, e) =>
            {
                if (m_view != null && !m_canClose)
                {
                    e.Cancel = true;
                    m_view.Hide();
                    if (m_hostView!=null)
                    {
                        m_hostView.Activate();
                    }
                }
            };
        }

        private bool Command_OnCanExecute(Object arg)
        {
            var viewResolver = Locator.Resolve<IResolver>("View");
            if (!viewResolver.CheckAvailable<T>())
            {
                return false;
            }
            return arg is IMainView;
        }

        private void Command_OnExecute(Object arg)
        {
            var hostView = arg as IMainView;
            m_hostView = hostView ?? m_hostView;
            if (m_hostView == null)
            {
                return;
            }
            var action = new Action(ExecuteSync);
            var hostSync = m_hostView as ISynchronizeInvoke;
            if (hostSync.InvokeRequired)
            {
                hostSync.BeginInvoke(action, null);
            }
            else
            {
                action();
            }
        }

        public void Show()
        {
            Command_OnExecute(m_hostView);
        }

        private void ExecuteSync()
        {
            try
            {
                if (m_view == null)
                {
                    CreateTargetForm();
                }
                m_view.Show(m_hostView);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
