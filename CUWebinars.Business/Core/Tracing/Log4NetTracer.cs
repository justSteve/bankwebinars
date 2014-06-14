using Ninject.Extensions.Logging.Log4net.Infrastructure;
using System.Diagnostics;

namespace CUWebinars.Business.Core.Tracing
{
    public class Log4NetTracer :  DefaultTraceListener
    {
        private readonly Log4NetLogger _log4NetLogger = new Log4NetLogger(typeof(Log4NetTracer));
        
        public override void Write(string message)
        {
            _log4NetLogger.Info(message);
        }

        public override void WriteLine(string message)
        {
            _log4NetLogger.Info(message);
        }
    }
}
