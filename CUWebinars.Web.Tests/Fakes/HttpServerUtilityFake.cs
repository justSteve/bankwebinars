using System.Web;

namespace CUWebinars.Web.Tests.Fakes
{
    public class HttpServerUtilityFake : HttpServerUtilityBase
    {
        public HttpServerUtilityFake()
        {

        }
        
        public override string UrlDecode(string s)
        {
            return @"/";
        }
    }
}
