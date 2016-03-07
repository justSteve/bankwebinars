using System;
using System.Diagnostics;
using CUWebinars.Business.AccountService;
using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;
using Ninject.Extensions.Logging;

namespace CUWebinars.Web.Controllers.api
{
    public class AddressesController : ParentApiController
    {
        private readonly IMembershipService _membershipService;
        private readonly ILogger _logger;
        private bool _disposed;

        public AddressesController(IMembershipService membershipService, ILogger logger) : base()
        {
            _membershipService = membershipService;
            _logger = logger;
        }

        [HttpGet]
        public object Get(int? id)
        {
            context.Configuration.LazyLoadingEnabled = false;

            Debug.Assert(id != null, "id != null");
            var address = _membershipService.GetAddressesForUser(id.Value);

            return JsonConvert.SerializeObject(address, Formatting.None, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                var logger = _logger as IDisposable;
                if (logger != null)
                    logger.Dispose();

                _membershipService.Dispose();

                base.Dispose(disposing);
            }
            _disposed = true;
        }
    }
}