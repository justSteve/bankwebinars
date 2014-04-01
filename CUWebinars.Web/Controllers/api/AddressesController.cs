using CUWebinars.Business.AccountService;
using Newtonsoft.Json;
using System.Linq;
using System.Web.Mvc;

namespace CUWebinars.Web.Controllers.api
{
    public class AddressesController : ParentApiController
    {
        private readonly IMembershipService _membershipService;

        public AddressesController(IMembershipService membershipService) : base()
        {
            _membershipService = membershipService;
        }

        [HttpGet]
        public object Get(int? id)
        {
            context.Configuration.LazyLoadingEnabled = false;

            var address = _membershipService.GetAddressesForUser(id.Value);

            return JsonConvert.SerializeObject(address, Formatting.None, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            //return JsonConvert.SerializeObject(
            //    context.WebUsers
            //   .Include("Addresses")
            //   .Where(u => u.idUser == id.Value)
            //   .Single()
            //   .Addresses
            //   .Select(a => new { 
            //       City = a.City,
            //       StreetAddress = a.StreetAddress,
            //       StreetAddress2 = a.StreetAddress2,
            //       Country = a.Country,
            //       Phone = a.Phone,
            //       State = a.State,
            //       Zip = a.Zip                   
            //   })
            //   , Formatting.Indented, new JsonSerializerSettings { MaxDepth = 1, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }        
    }
}