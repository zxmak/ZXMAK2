using System;
using System.IO;
using System.Globalization;
using log4net;


namespace ZXMAK2
{
    public static class Logger
    {
        private static readonly ILog _logger = LogManager.GetLogger("ZXMAK2");
        
        public static void Start()
        {
        }

        public static void Finish()
        {
            LogManager.Shutdown();
        }


        #region Redirect

        public static void Debug(string fmt, params object[] args)
        {
            try
            {
                _logger.DebugFormat(CultureInfo.InvariantCulture, fmt, args);
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        public static void Info(string fmt, params object[] args)
        {
            try
            {
                _logger.InfoFormat(CultureInfo.InvariantCulture, fmt, args);
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        public static void Warn(string fmt, params object[] args)
        {
            try
            {
                _logger.WarnFormat(CultureInfo.InvariantCulture, fmt, args);
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        public static void Error(string fmt, params object[] args)
        {
            try
            {
                _logger.ErrorFormat(CultureInfo.InvariantCulture, fmt, args);
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        public static void Fatal(string fmt, params object[] args)
        {
            try
            {
                _logger.FatalFormat(CultureInfo.InvariantCulture, fmt, args);
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        public static void Debug(Exception exception, string fmt, params object[] args)
        {
            try
            {
                var msg = fmt != null ? string.Format(fmt, args) : null;
                if (exception != null)
                {
                    msg = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}: {1}",
                        exception.GetType(),
                        msg);
                }
                _logger.Debug(msg, exception);
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        public static void Info(Exception exception, string fmt, params object[] args)
        {
            try
            {
                var msg = fmt != null ? string.Format(fmt, args) : null;
                if (exception != null)
                {
                    msg = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}: {1}",
                        exception.GetType(),
                        msg);
                }
                _logger.Info(msg, exception);
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        public static void Warn(Exception exception, string fmt, params object[] args)
        {
            try
            {
                var msg = fmt != null ? string.Format(fmt, args) : null;
                if (exception != null)
                {
                    msg = string.Format( 
                        CultureInfo.InvariantCulture,
                        "{0}: {1}", 
                        exception.GetType(), 
                        msg);
                }
                _logger.Warn(msg, exception);
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        public static void Error(Exception exception, string fmt, params object[] args)
        {
            try
            {
                var msg = fmt != null ? string.Format(fmt, args) : null;
                if (exception != null)
                {
                    msg = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}: {1}",
                        exception.GetType(),
                        msg);
                }
                _logger.Error(msg, exception);
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        public static void Fatal(Exception exception, string fmt, params object[] args)
        {
            try
            {
                var msg = fmt != null ? string.Format(fmt, args) : null;
                if (exception != null)
                {
                    msg = string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}: {1}",
                        exception.GetType(),
                        msg);
                }
                _logger.Fatal(msg, exception);
            }
            catch (Exception ex)
            {
                Error(ex);
            }
        }

        #endregion Redirect


        public static void Debug(Exception exception)
        {
            Debug(exception, null);
        }

        public static void Info(Exception exception)
        {
            Info(exception, null);
        }

        public static void Warn(Exception exception)
        {
            Warn(exception, null);
        }

        public static void Error(Exception exception)
        {
            Error(exception, null);
        }

        public static void Fatal(Exception exception)
        {
            Fatal(exception, null);
        }


        [Obsolete("remove call to LogAgent.DumpArray")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static void DumpArray<T>(string fileName, T[] array)
        {
            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var wr = new StreamWriter(fs))
            {
                for (var i = 0; i < array.Length; i++)
                {
                    wr.WriteLine("{0} = {1}", i, array[i]);
                }
            }
        }

        [Obsolete("remove call to LogAgent.DumpAppend")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static void DumpAppend(
            string fileName, 
            string format, 
            params object[] args)
        {
            using (var fs = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.Read))
            using (var wr = new StreamWriter(fs))
            {
                wr.WriteLine(format, args);
            }
        }
    }
}
