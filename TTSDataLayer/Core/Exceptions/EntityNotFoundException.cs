using System;

namespace CUWebinars.Business.Core.Exceptions
{
    public class EntityNotFoundException: TTSException
    {
        public EntityNotFoundException() :
            base()
        {
        }

        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        public EntityNotFoundException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
