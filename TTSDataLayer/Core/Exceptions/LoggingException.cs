using System;

namespace CUWebinars.Business.Core.Exceptions
{
    public class LoggingException : TTSException
    {
        public LoggingException() :
            base()
        {
        }

        public LoggingException(string message)
            : base(message)
        {
        }

        public LoggingException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
