using System;

namespace CUWebinars.Business.Core.Exceptions
{
    public class DataIntegrityException :TTSException
    {
        public DataIntegrityException() :
            base()
        {
        }

        public DataIntegrityException(string message)
            : base(message)
        {
        }

        public DataIntegrityException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
