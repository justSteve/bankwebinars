using System;

namespace CUWebinars.Business.Core
{
    public class TTSException : ApplicationException
    {
        public TTSException() :
            base()
        {
        }

        public TTSException(string message)
            : base(message)
        {
        }

        public TTSException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}