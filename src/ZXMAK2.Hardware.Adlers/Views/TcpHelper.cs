using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;

namespace ZXMAK2.Hardware.Adlers.Views
{
    public partial class TcpHelper : Form
    {
        #region members
            private static WebClient _client;
            private static string _proxyAddress;
            private static string _proxyPort;
        #endregion

        public TcpHelper()
        {
            InitializeComponent();
            _proxyAddress = _proxyPort = string.Empty;

            _client = new WebClient();
            _client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)");
        }

        #region setters/getters
            public static bool SetProxyAddress(string i_newAddress)
            {
                Match match = Regex.Match(i_newAddress, @"\b\d{2}\.\d{2}\.\d{2}\.\d{2}\b", RegexOptions.IgnoreCase);
                if (!match.Success)
                    return false;
                _proxyAddress = i_newAddress;

                return true;
            }
            public static bool SetProxyPort( string i_newPort )
            {
                int proxyAdept = ConvertRadix.ParseUInt16(i_newPort, 10);
                if (proxyAdept > 0xFFFF)
                    return false;
                _proxyPort = i_newPort;
                return true;
            }
        #endregion

        #region GUI
        private void checkBoxIsProxy_CheckedChanged(object sender, EventArgs e)
        {
            this.textBoxProxyPort.Enabled = this.textBoxProxyAdress.Enabled = checkBoxIsProxy.Checked;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            //download Pasmo2.dll
            try
            {
                _client.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                _client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);

                if(checkBoxIsProxy.Checked)
                {
                    WebProxy proxy = new WebProxy("http://" + this.textBoxProxyAdress.Text.Trim() + ":" + this.textBoxProxyPort.Text.Trim() + "/",true);
                    _client.Proxy = proxy;
                }

                labelStatusText.Text = "Downloading...";
                buttonStart.Enabled = false;

                _client.DownloadFileAsync(new Uri(@"http://download-codeplex.sec.s-msft.com/Download/Release?ProjectName=pasmo2&DownloadId=1448407&FileTime=130734215334470000&Build=21024"), "Pasmo2.dll");
            }
            catch(Exception tcpException)
            {
                Logger.Error(tcpException);
                Locator.Resolve<IUserMessage>()
                    .Error("Error: \n" + tcpException.Message);
            }
        }
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.progressBarDownloadStatus.Value = e.ProgressPercentage;
        }
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            labelStatusText.Text = "Completed!";
            buttonStart.Enabled = true;
        }
        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (_client.IsBusy)
                _client.CancelAsync();

            labelStatusText.Text = "Canceled";
            buttonStart.Enabled = true;

            this.Hide();
        }
        #endregion

        //
        // get ftp file from internet
        // i_filePath: e.g. "code_file.xml"
        public static string GetFtpFileContents(string i_filePath, out string i_errMessage)
        {
            try
            {
                string fileContents = string.Empty;

                WebRequest request = WebRequest.Create(@"http://adlers.host.sk/ZxSpectrum/" + i_filePath);
                request.Credentials = CredentialCache.DefaultCredentials;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                HttpStatusCode status = response.StatusCode;
                if (status != HttpStatusCode.OK)
                    i_errMessage = response.StatusDescription;
                else
                {
                    i_errMessage = string.Empty;
                    fileContents = reader.ReadToEnd();
                }

                reader.Close();
                response.Close();

                return fileContents;
            }
            catch(Exception ex)
            {
                i_errMessage = ex.Message.ToString();
                return string.Empty;
            }
        }

        public static bool DownloadFileSilent()
        {
            //todo: download file in background, e.g.: Pasmo2.dll
            return false;
        }

        public static bool TestTcpConnection(out string o_message, string i_proxyIP = null, string i_proxyPort = null)
        {
            try
            {
                WebRequest request = WebRequest.Create(@"http://adlers.host.sk/");
                request.Credentials = CredentialCache.DefaultCredentials;
                if (i_proxyIP != null && i_proxyPort != null)
                {
                    IPAddress address;
                    if (IPAddress.TryParse(i_proxyIP, out address) == false)
                    {
                        o_message = "Error in parsing IP address";
                        return false;
                    }

                    WebProxy proxy = new WebProxy(String.Format("{0}:{1}", i_proxyIP, i_proxyPort), false);
                    request.Proxy = proxy;
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    o_message = "success";
                    return true;
                }
                Stream responseStream = response.GetResponseStream();

                o_message = String.Format("Error:\n\nResponse code: {0}\nResponse text: {1}");
                return false;
            }
            catch(Exception ex)
            {
                o_message = String.Format("Error: " + ex.Message);
                return false;
            }

            /*bool retResult = false;
            TcpClient client = new TcpClient();
            
            o_message = String.Empty;

            IPAddress address;
            if(IPAddress.TryParse(i_IPAddress, out address) == false )
            {
                o_message = "Error in parsing IP address";
                return false;
            }

            try
            {
                client.Connect(address, i_Port);
                retResult = true;
            } 
            catch (SocketException ex)
            {
                o_message = ex.Message;
            }
            finally
            {
                client.Close();
            }

            return retResult;*/
        }
    }
}
