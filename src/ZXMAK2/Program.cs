using System;
using ZXMAK2.Dependency;
using ZXMAK2.Host.Presentation.Interfaces;


namespace ZXMAK2
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Logger.Start();
            try
            {
                AppDomain.CurrentDomain.UnhandledException +=
                    (s, e) => Logger.Fatal(e.ExceptionObject as Exception, "AppDomain.UnhandledException");
                //AppDomain.CurrentDomain.FirstChanceException += 
                //    (s, e) => Logger.Error(e.Exception, "AppDomain.FirstChanceException");
                RunSafe(args);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                Logger.Finish();
            }
        }

        private static void RunSafe(string[] args)
        {
            var launcher = Locator.Resolve<ILauncher>();
            launcher.Run(args);
            Locator.Shutdown();
        }
    }
}
