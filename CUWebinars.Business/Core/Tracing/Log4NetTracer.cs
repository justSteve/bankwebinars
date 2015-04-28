using Ninject.Extensions.Logging.Log4net.Infrastructure;
using System.Diagnostics;

namespace CUWebinars.Business.Core.Tracing
{
    public class Log4NetTracer :  DefaultTraceListener
    {
        private readonly Log4NetLogger _log4NetLogger = new Log4NetLogger(typeof(Log4NetTracer));
        private readonly string _tracingLevel = TtsConfig.TracingLevel;
        
        public override void Write(string message)
        {
            ExecuteTraceWrite(message);
        }

        public override void WriteLine(string message)
        {
            ExecuteTraceWrite(message);
        }

        private void ExecuteTraceWrite(string message)
        {
            switch (_tracingLevel)
            {
                case "Error":
                    _log4NetLogger.Error(message);
                    break;
                case "Warning":
                    _log4NetLogger.Warn(message);
                    break;
                case "Info":
                    _log4NetLogger.Info(message);
                    break;
                case "Verbose":
                    _log4NetLogger.Error(message);
                    break;
                default:
                    _log4NetLogger.Warn(message);
                    break;
            }
        }
    }
}
