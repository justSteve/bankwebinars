using System.Configuration;
using System.Data;
using System.Data.RSSBus.Gmail;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using CUWebinars.Business.AccountService;
using CUWebinars.Business.Models;
using CUWebinars.Business.Services;
using CUWebinars.Web.Core.Orchestrators;
using CUWebinars.Web.Helpers;
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

        private readonly IOrderControllerOrchestrator _orderControllerOrchestrator;
        private IOrderManagementService _orderManagementService;
        private IWebinarManagementService _webinarManagementService;
        private readonly IAppHelper _appHelper;

        public ACSController(ILogger logger, IOrderControllerOrchestrator orderControllerOrchestrator, IOrderManagementService orderManagementService, IWebinarManagementService webinarManagementService, IAppHelper appHelper)
            : base()
        {
            _logger = logger;
            _orderControllerOrchestrator = orderControllerOrchestrator;
            _orderManagementService = orderManagementService;
            _webinarManagementService = webinarManagementService;
            _appHelper = appHelper;
        }

        // GET: api/ACS
        public IEnumerable<string> Get()
        {
            var mLic = new GmailConnection().RuntimeLicense;
            
            return new string[] { "value1", "value2" };
        }

        // GET: api/ACS/5
        public string Get(int id)
        {
            return "value";
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
