using System;
using System.IO;

namespace CUWebinars.Business
{
    public class TTSLogger : Singleton<TTSLogger>
    {
        //private HttpContext CurrentContext
        //{
        //    get
        //    {
        //        if (HttpContext.Current != null)
        //        {
        //            return HttpContext.Current;
        //        }

        //        SimpleWorkerRequest dummyRequest = new SimpleWorkerRequest("NotASP_NET", "", new StringWriter());
        //        HttpContext context = new HttpContext(dummyRequest);
        //        context.ApplicationInstance = new HttpApplication();

        //        return context;
        //    }
        //}

        //public virtual void LogException(Exception ex)
        //{
        //    //SimpleWorkerRequest
        //    ErrorSignal.FromContext(CurrentContext).Raise(ex);
        //}

        //public virtual void LogMessage(string message)
        //{
        //    LoggingException loggingException = new LoggingException(message);
        //    ErrorSignal.FromContext(CurrentContext).Raise(loggingException);
        //}
    }
}