using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CUWebinars.Business.Core.Exceptions;

namespace CUWebinars.Web.Core
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