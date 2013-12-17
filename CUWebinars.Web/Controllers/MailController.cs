using System.Collections.Generic;
using System.Linq;
using ActionMailer.Net.Mvc;
using CUWebinars.Web.Data.Repositories.Interfaces;
using CUWebinars.Web.Services;
using Ninject.Extensions.Logging;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Controllers
{
    public class MailController : MailerBase
    {
        private TTSWebinarsContext db = new TTSWebinarsContext();
        private IMailService _mail;
        private readonly IWebinarRepository _repos;
        public ILogger Logger { get; set; }
        public MailController(IMailService mail, IWebinarRepository repos, ILogger logger)
        {
            _mail = mail;
            _repos = repos;
            Logger = logger;
        }


        public EmailResult ConnectionInfoEmail(IList<Order> orders)
        {
                To.Add("steve@ttstrain.com");
                From = "no-reply@mycoolsite.com";
                Subject = "Welcome to My Cool Site!"; 
            return Email("ConnectionInfoEmailsWereSent", orders.SingleOrDefault());
        }
    }
}
