using System;

namespace CUWebinars.Business.Core.Exceptions
{
    public class UserCreatedMembershipException : DataIntegrityException
    {
        public UserCreatedMembershipException()
        {
        }

        public UserCreatedMembershipException(string message) : base(message)
        {
        }

        public UserCreatedMembershipException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
