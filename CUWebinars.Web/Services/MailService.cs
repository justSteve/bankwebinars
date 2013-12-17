using System;
using System.Collections.Generic;
using System.Net.Mail;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Services
{
    public class MailService : IMailService
    {
        public bool SendMail(string from, string to, string subject, string body)
        {
            try
            {
                var msg = new MailMessage(from, to, subject, body);

                var client = new SmtpClient();
                client.Send(msg);
            }
            catch (Exception ex)
            {
                // Add logging
                return false;
            }

            return true;
        }

        public bool SendConnectionInfo(IList<Order> orders)
        {
            return false;
            //    new MailController().VerificationEmail(newUser).Deliver();
        }
    }
}