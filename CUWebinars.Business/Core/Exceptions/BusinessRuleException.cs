using System;

namespace CUWebinars.Business.Core.Exceptions
{
    public class BusinessRuleException:TTSException
    {
        public BusinessRuleException() :
            base()
        {
        }

        public BusinessRuleException(string message)
            : base(message)
        {
        }

        public BusinessRuleException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    }
}
