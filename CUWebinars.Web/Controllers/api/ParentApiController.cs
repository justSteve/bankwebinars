using System.Web.Http;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Controllers.api
{
    public class ParentApiController : ApiController
    {
        protected TTSWebinarsContext context;
        public ParentApiController()
        {
            context = new TTSWebinarsContext();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}