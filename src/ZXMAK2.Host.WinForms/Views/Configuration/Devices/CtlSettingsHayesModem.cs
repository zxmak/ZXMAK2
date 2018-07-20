using ZXMAK2.Engine;
using ZXMAK2.Hardware.General;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Host.WinForms.Views.Configuration.Devices
{
    public partial class CtlSettingsHayesModem : ConfigScreenControl
    {
        private BusManager m_bmgr;
        private HayesModem m_modem;

        public CtlSettingsHayesModem()
        {
            InitializeComponent();
        }

        public void Init(BusManager bmgr, IHostService host, HayesModem modem)
        {
            m_bmgr = bmgr;
            m_modem = modem;

            BindComPorts();
        }

        public void BindComPorts()
        {
            cmbPorts.Items.Clear();
            cmbPorts.Items.Add("NONE");

            cmbPorts.SelectedIndex = 0;

            var ports = System.IO.Ports.SerialPort.GetPortNames();
            foreach (var port in ports)
                cmbPorts.Items.Add(port);

            if (!string.IsNullOrEmpty(m_modem.PortName) && cmbPorts.Items.Contains(m_modem.PortName))
                cmbPorts.SelectedIndex = cmbPorts.Items.IndexOf(m_modem.PortName);
        }

        public override void Apply()
        {
            if (cmbPorts.SelectedIndex == 0)
                m_modem.PortName = string.Empty;
            else
                m_modem.PortName = cmbPorts.SelectedItem.ToString();
        }
    }
}
