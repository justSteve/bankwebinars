using System.Collections.Generic;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Services
{
    public interface IMailService
    {
        bool SendMail(string from, string to, string subject, string body);
        bool SendConnectionInfo(IList<Order> orders );
    }
}