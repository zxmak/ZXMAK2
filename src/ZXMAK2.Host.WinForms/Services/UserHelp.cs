using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Host.WinForms.Services
{
    public class UserHelp : IUserHelp
    {
        private static string s_url;

        public bool CanShow(object uiControl)
        {
            return uiControl is Control;
        }
        
        public void ShowHelp(object uiControl)
        {
            var control = uiControl as Control;
            if (control == null || !CheckUrl())
            {
                return;
            }
            Help.ShowHelp(control, s_url);
        }

        public void ShowHelp(object uiControl, HelpNavigator navigator)
        {
            var control = uiControl as Control;
            if (control == null || !CheckUrl())
            {
                return;
            }
            Help.ShowHelp(control, s_url, navigator);
        }

        public void ShowHelp(object uiControl, string keyword)
        {
            var control = uiControl as Control;
            if (control == null || !CheckUrl())
            {
                return;
            }
            Help.ShowHelp(control, s_url, keyword);
        }

        public void ShowHelp(Object uiControl, HelpNavigator command, object parameter)
        {
            var control = uiControl as Control;
            if (control == null || !CheckUrl())
            {
                return;
            }
            Help.ShowHelp(control, s_url, command, parameter);
        }

        private static bool CheckUrl()
        {
            if (s_url != null)
            {
                return true;
            }
            var appName = Path.GetFullPath(Assembly.GetEntryAssembly().Location);
            var helpFile = Path.ChangeExtension(appName, ".chm");
            if (!File.Exists(helpFile))
            {
                Locator.Resolve<IUserMessage>()
                    .Error("Help file is missing");
                return false;
            }
            if (helpFile.Contains("#")) //Path to .chm file must not contain # - Microsoft bug
            {
                var fileName = Path.GetRandomFileName() + ".chm";
                var tmpFile = Path.Combine(Path.GetTempPath(), fileName);
                File.Copy(helpFile, tmpFile);
                s_url = tmpFile;
                Application.ApplicationExit += (s, e) => File.Delete(tmpFile);
                return true;
            }
            s_url = helpFile;
            return true;
        }
    }
}
