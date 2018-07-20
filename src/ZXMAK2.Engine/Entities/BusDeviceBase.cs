using System;
using System.Xml;
using ZXMAK2.Engine.Interfaces;


namespace ZXMAK2.Engine.Entities
{
    public abstract class BusDeviceBase
    {
        private bool m_isConfigLoading;


        protected BusDeviceBase()
        {
            Category = BusDeviceCategory.Other;
            Name = GetType().Name;
            Description = null;
        }

        public BusDeviceCategory Category { get; protected set; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public int BusOrder { get; set; }
        public event EventHandler ConfigChanged;

        /// <summary>
        /// Collect information about device. Add handlers & serializers here.
        /// </summary>
        public abstract void BusInit(IBusManager bmgr);

        /// <summary>
        /// Called after Init, before device will be used. Add load files here
        /// </summary>
        public abstract void BusConnect();

        /// <summary>
        /// Called when device using finished. Add flush & close files here
        /// </summary>
        public abstract void BusDisconnect();

        /// <summary>
        /// Called to reset device to initial state (before load snapshot)
        /// </summary>
        public virtual void ResetState()
        {
        }

        protected virtual void OnConfigLoad(XmlNode itemNode)
        {
        }

        protected virtual void OnConfigSave(XmlNode itemNode)
        {
        }

        /// <summary>
        /// Called when device configuration was changed.
        /// </summary>
        protected void OnConfigChanged()
        {
            if (!m_isConfigLoading)
            {
                OnProcessConfigChange();
                var handler = ConfigChanged;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Save device configuration to XML node
        /// </summary>
        public void LoadConfigXml(XmlNode itemNode)
        {
            m_isConfigLoading = true;
            try
            {
                OnConfigLoad(itemNode);
            }
            finally
            {
                m_isConfigLoading = false;
                OnConfigChanged();
            }
        }

        /// <summary>
        /// Load device configuration from XML node
        /// </summary>
        public void SaveConfigXml(XmlNode itemNode)
        {
            m_isConfigLoading = true;
            try
            {
                OnConfigSave(itemNode);
            }
            finally
            {
                m_isConfigLoading = false;
            }
        }

        protected virtual void OnProcessConfigChange()
        {
        }
    }
}
