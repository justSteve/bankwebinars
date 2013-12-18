using System;

namespace CUWebinars.Business.Core.Exceptions
{
    public class DuplicateEntityException:TTSException
    {
        public DuplicateEntityException() :
            base()
        {
        }

        public DuplicateEntityException(string message)
            : base(message)
        {
        }

        public DuplicateEntityException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
