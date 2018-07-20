using System;
using ZXMAK2.Engine;
using ZXMAK2.Host.Interfaces;
using ZXMAK2.Engine.Interfaces;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Host.WinForms.Views.Configuration.Devices
{
    public partial class CtlSettingsJoystick : ConfigScreenControl
    {
        private BusManager m_bmgr;
        private IHostService m_host;
        private IJoystickDevice m_device;

        public CtlSettingsJoystick()
        {
            InitializeComponent();
        }

        public void Init(BusManager bmgr, IHostService host, IJoystickDevice device)
        {
            m_bmgr = bmgr;
            m_host = host;
            m_device = device;
            var busDevice = (BusDeviceBase)device;
            //txtDevice.Text = busDevice.Name;
            txtDescription.Text = busDevice.Description.Replace("\n", Environment.NewLine);

            cbxType.Items.Clear();
            if (m_host != null && m_host.Joystick != null)
            {
                foreach (var hdi in m_host.Joystick.GetAvailableJoysticks())
                {
                    cbxType.Items.Add(hdi);
                }
            }
            //cbxType.Sorted = true;

            cbxType.SelectedIndex = -1;
            for (var i = 0; i < cbxType.Items.Count; i++)
            {
                var hdi = (IHostDeviceInfo)cbxType.Items[i];
                if (m_device.HostId == hdi.HostId)
                {
                    cbxType.SelectedIndex = i;
                    break;
                }
            }
            cbxType_SelectedIndexChanged(this, EventArgs.Empty);
        }

        public override void Apply()
        {
            var hdi = (IHostDeviceInfo)cbxType.SelectedItem;
            if (hdi != null)
            {
                m_device.HostId = hdi.HostId;
            }
            else
            {
                m_device.HostId = string.Empty;
            }
            Init(m_bmgr, m_host, m_device);
        }

        private void cbxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var hdi = (HostDeviceInfo)cbxType.SelectedItem;
        }
    }
}
