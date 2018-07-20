using System;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Presentation;
using ZXMAK2.Host.Presentation.Interfaces;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Hardware.General
{
    public class Debugger : BusDeviceBase, IJtagDevice
    {
        public Debugger()
        {
            Category = BusDeviceCategory.Debugger;
            Name = "DEBUGGER";
            Description = "Default Debugger";
            CreateViewHolder();
        }

        
        #region BusDeviceBase

        private IViewHolder m_viewHolder;


        public override void BusInit(IBusManager bmgr)
        {
            if (m_viewHolder != null)
            {
                bmgr.AddCommandUi(m_viewHolder.CommandOpen);
            }
        }

        public override void BusConnect()
        {
        }

        public override void BusDisconnect()
        {
            if (m_viewHolder != null)
            {
                m_viewHolder.Close();
            }
        }

        #endregion

        #region IJtagDevice

        public void Attach(IDebuggable dbg)
        {
            if (m_viewHolder != null && dbg != null)
            {
                m_viewHolder.Arguments = new [] 
                { 
                    new Argument("debugTarget", dbg), 
                };
            }
        }

        public void Detach()
        {
            if (m_viewHolder != null)
            {
                m_viewHolder.Close();
                m_viewHolder.Arguments = null;
            }
        }

        #endregion

        
        #region IGuiExtension

        private void CreateViewHolder()
        {
            try
            {
                m_viewHolder = new ViewHolder<IDebuggerGeneralView>("Debugger");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        #endregion IGuiExtension
    }
}
