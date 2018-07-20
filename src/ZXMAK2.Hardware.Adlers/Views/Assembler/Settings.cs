using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using ZXMAK2.Dependency;
using ZXMAK2.Engine;
using ZXMAK2.Host.Interfaces;

namespace ZXMAK2.Hardware.Adlers.Views.AssemblerView
{
    public partial class Settings : Form
    {
        //instance
        private static Settings m_instance = null;

        private Settings()
        {
            InitializeComponent();
        }

        public static void ShowForm()
        {
            if (m_instance == null || m_instance.IsDisposed)
            {
                m_instance = new Settings();
                m_instance.LoadConfig();
                m_instance.ShowInTaskbar = true;
                m_instance.ShowDialog();
            }
            else
                m_instance.Show();
        }
        public static Settings GetInstance()
        {
            return m_instance;
        }

        #region Load/Save settings
        private void LoadConfig()
        {
            if (!File.Exists(Path.Combine(Utils.GetAppFolder(), FormCpu.ConfigXmlFileName)))
                return;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Path.Combine(Utils.GetAppFolder(), FormCpu.ConfigXmlFileName));

            //Tcp->Proxy enabled
            XmlNode node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Assembler/Settings/Tcp");
            if (node != null)
                this.chckbxUseProxy.Checked = (node.Attributes["Enabled"].InnerText == "1");
            //Tcp->Address
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Assembler/Settings/Tcp/Address");
            if (node != null)
                this.txtbxProxyAddress.Text = node.InnerText;
            //Tcp->Port
            node = xmlDoc.DocumentElement.SelectSingleNode("/Root/Assembler/Settings/Tcp/Port");
            if (node != null)
                this.txtbxProxyPort.Text = node.InnerText;
        }
        public static void GetPartialConfig(ref XmlWriter io_writer)
        {
            if (m_instance == null)
                return;
            //Settings root
            io_writer.WriteStartElement("Settings");

                //Tcp
                io_writer.WriteStartElement("Tcp");
                io_writer.WriteAttributeString("Enabled", m_instance.chckbxUseProxy.Checked ? "1" : "0");
                io_writer.WriteElementString("Address", m_instance.txtbxProxyAddress.Text);
                io_writer.WriteElementString("Port", m_instance.txtbxProxyPort.Text);
                io_writer.WriteEndElement();
                //Tcp end

                //Editor
                io_writer.WriteStartElement("Editor");
                io_writer.WriteAttributeString("SyntaxHighlightning", m_instance.chckbxSyntaxHighligtining.Checked ? "1" : "0");
                io_writer.WriteEndElement();
                //Editor end

            io_writer.WriteEndElement(); //Settings end
        }
        #endregion Load/Save settings

        //Test connection
        private void btnTestConnection_Click(object sender, System.EventArgs e)
        {
            if ( chckbxUseProxy.Checked && ( txtbxProxyAddress.Text == String.Empty || txtbxProxyPort.Text == String.Empty))
                return;

            string message;
            bool ret;
            if( chckbxUseProxy.Checked )
                //use proxy
                ret = TcpHelper.TestTcpConnection(out message, txtbxProxyAddress.Text, txtbxProxyPort.Text);
            else
                ret = TcpHelper.TestTcpConnection(out message);
            if( ret == false )
            {
                Locator.Resolve<IUserMessage>().Error("Connection test failed...\n\nMessage: " + message);
            }
            else
            {
                Locator.Resolve<IUserMessage>().Info("Test succeeded !");
            }
        }

        private void chckbxUseProxy_CheckedChanged(object sender, System.EventArgs e)
        {
            txtbxProxyAddress.Enabled = txtbxProxyPort.Enabled = chckbxUseProxy.Checked;
        }

        #region members
        #endregion members

        #region public getters
        public static bool IsSyntaxHighlightningOn()
        {
            if (m_instance == null)
                return true; //default is true
            else
                return m_instance.chckbxSyntaxHighligtining.Checked;
        }
        #endregion public getters

        #region GUI
        //settings icon comes from here: https://www.iconfinder.com/icons/17814/preferences_settings_tools_icon#size=128

        //Save
        private void btnSave_Click(object sender, System.EventArgs e)
        {
            m_instance.Hide(); //ToDo:
        }
        //Cancel
        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            m_instance.Hide();
        }
        #endregion GUI

    }
}
