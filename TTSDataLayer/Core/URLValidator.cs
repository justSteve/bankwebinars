using System.Net;

namespace CUWebinars.Business.Core
{
    public static class UrlValidator
    {
        public static bool CheckURLExists(string Url)
        {
            try
            {
                string requestUrl = Url;
                if (Url.ToUpper().StartsWith("HTTP://") == false)
                {
                    requestUrl = "http://" + Url;
                }

                WebRequest request = HttpWebRequest.Create(requestUrl);
                request.Method = WebRequestMethods.Http.Head;
                request.Timeout = 2000;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
