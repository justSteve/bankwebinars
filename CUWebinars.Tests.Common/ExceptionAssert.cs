using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CUWebinars.Tests.Common
{
    public static class ExceptionAssert
    {
        public static void Throws<T>(Action code, string exceptionMessage = null, string message = null, params object[] args) where T : Exception
        {
            try
            {
                code.Invoke();

                Assert.Fail("No exception was thrown by the code under test.");
            }
            catch (Exception exception)
            {
                if (string.IsNullOrWhiteSpace(exceptionMessage))
                {
                    Assert.AreEqual(exception.GetType(), typeof(T), message, args);
                }
                else
                {
                    Assert.AreEqual(exception.GetType(), typeof(T), message, args);
                    Assert.IsTrue(exception.Message.StartsWith(exceptionMessage, StringComparison.OrdinalIgnoreCase));
                }
            }
        }
    }
}
