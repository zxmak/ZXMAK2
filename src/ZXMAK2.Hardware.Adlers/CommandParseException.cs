using System;


namespace ZXMAK2.Hardware.Adlers
{
    public class CommandParseException : ApplicationException
    {
        public CommandParseException(string message)
            : base(message)
        {
        }

        public CommandParseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
