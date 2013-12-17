using System.Collections.Generic;
using System.Diagnostics;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Services
{
    public class MailServiceMock : IMailService
    {
        public bool SendMail(string from, string to, string subject, string body)
        {
            Debug.WriteLine(string.Concat("SendMail: ", subject));
            return true;
        }

        public bool SendConnectionInfo(IList<Order> orders)
        {
            throw new System.NotImplementedException();
        }
    }
}