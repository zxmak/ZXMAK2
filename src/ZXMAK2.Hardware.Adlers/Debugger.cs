using System;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Presentation;
using ZXMAK2.Host.Presentation.Interfaces;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Hardware.Adlers
{
    public class Debugger : BusDeviceBase, IJtagDevice
    {
        private IBusManager m_bmgr;
        private IViewHolder m_viewHolder;

        
        public Debugger()
        {
            Category = BusDeviceCategory.Debugger;
            Name = "DEBUGGER ADLERS";
            Description = "Extended debugger written by Adlers";
            CreateViewHolder();
        }

        
        #region BusDeviceBase

        public override void BusInit(IBusManager bmgr)
        {
            if (m_viewHolder != null)
            {
                bmgr.AddCommandUi(m_viewHolder.CommandOpen);
            }
            m_bmgr = bmgr;
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
                m_viewHolder.Arguments = new[] 
                { 
                    new Argument("debugTarget", dbg),
                    new Argument("bmgr", m_bmgr),
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

        private void CreateViewHolder()
        {
            try
            {
                m_viewHolder = new ViewHolder<IDebuggerAdlersView>("Debugger");
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
