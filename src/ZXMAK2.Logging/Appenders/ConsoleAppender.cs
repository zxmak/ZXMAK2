using System;
using log4net.Appender;
using log4net.Core;


namespace ZXMAK2.Logging.Appenders
{
    public class ConsoleAppender : ManagedColoredConsoleAppender, IDisposable
    {
        private readonly ConsoleWindow _consoleWindow = new ConsoleWindow();
        
        public ConsoleAllocMode AllocMode { get; set; }
        public Level AutoLevel { get; set; }


        public ConsoleAppender()
        {
            AutoLevel = Level.Debug;
        }

        public void Dispose()
        {
            _consoleWindow.Dispose();
        }


        public override void ActivateOptions()
        {
            if (AllocMode == ConsoleAllocMode.Always)
            {
                _consoleWindow.Show();
            }
            base.ActivateOptions();
        }

        protected override void OnClose()
        {
            base.OnClose();
            _consoleWindow.Dispose();
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (AllocMode == ConsoleAllocMode.Auto &&
                loggingEvent != null && 
                loggingEvent.Level >= AutoLevel)
            {
                _consoleWindow.Show();
            }
            base.Append(loggingEvent);
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            if (loggingEvents == null)
            {
                return;
            }
            foreach (var logEvent in loggingEvents)
            {
                Append(logEvent);
            }
        }
    }
}
