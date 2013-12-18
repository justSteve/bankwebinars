using System;

namespace CUWebinars.Business.Core.Exceptions
{
    public class SecurityException : TTSException
    {
        public SecurityException() :
            base()
        {
        }

        public SecurityException(string message)
            : base(message)
        {
        }

        public SecurityException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
