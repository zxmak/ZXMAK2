using System;


namespace ZXMAK2.Host.Interfaces
{
    public interface IUserMessage
    {
        void ErrorDetails(Exception ex);
        
        void Error(Exception ex);
        void Error(string fmt, params object[] args);
        
        void Warning(Exception ex);
        void Warning(string fmt, params object[] args);

        void Info(string fmt, params object[] args);
    }
}
