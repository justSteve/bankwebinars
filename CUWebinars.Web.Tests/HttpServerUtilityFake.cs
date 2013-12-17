using System.Web;

namespace CUWebinars.Web.Tests
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
