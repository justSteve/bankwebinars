using System.Configuration;
using System.Data.RSSBus.Gmail;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using HtmlAgilityPack;
using log4net.Repository.Hierarchy;
using Ninject.Extensions.Logging;

namespace CUWebinars.Web.Controllers.api
{
    public class ACSController : ApiController
    {
                private readonly IMembershipService _membershipService;
        private readonly ILogger _logger;
        private bool _disposed;

private readonly GmailConnection _conn = new GmailConnection(
            ConfigurationManager.ConnectionStrings["RSSBUS_Affiliate"].ConnectionString);

        
        public ACSController(IMembershipService membershipService, ILogger logger) : base()
        {
            _membershipService = membershipService;
            _logger = logger;
        }

        // GET: api/ACS
        public IEnumerable<string> Get()
        {
            ParseEmails_ACS();
            return new string[] { "value1", "value2" };
        }

        // GET: api/ACS/5
        public string Get(int id)
        {
            return "value";
        }

        public virtual int ParseEmails_ACS()
        {

            var gda = new GmailDataAdapter
            {
                SelectCommand = new GmailCommand(
                    "SELECT id from MailMessages where SEARCHCRITERIA " +
                    "= 'UNSEEN SUBJECT \"Webinar Registration\"' ",
                    _conn)
            };
            var AreErrors = "";
            var MyGuid = "";
            try
            {
                var reader = gda.SelectCommand.ExecuteReader();

                while (reader.Read())
                {
                    AreErrors = "";
                    //
                    int readerID = Convert.ToInt32(reader["id"]);

                    if (readerID < 1)
                    {
                        break;
                    }

                    var cmd = new GmailCommand("GetMailMessage", _conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add(new GmailParameter("@id", readerID));
                    var mReader = cmd.ExecuteReader();

                    while (mReader.Read())
                    {

                    }
                }
            }
            catch
            {
                throw;
            }
            return 0;
        }

        // POST: api/ACS
            public
            void Post([FromBody]string value)
        {


        }

        // PUT: api/ACS/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ACS/5
        public void Delete(int id)
        {
        }
    }
}
