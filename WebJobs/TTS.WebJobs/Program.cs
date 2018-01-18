using CUWebinars.Azure.Common;
using Microsoft.Azure.WebJobs;

namespace TTS.WebJobs
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Configure Log4net
            LogConfigurer.ConfigureLogging(typeof(Program), logSettingsAtSpinup: true);

            // configure and run the JobHost
            var jobHostConfiguration = new JobHostConfiguration
            {
                NameResolver = new ConfigNameResolver()
            };

            var jobHost = new JobHost(jobHostConfiguration);

            jobHost.RunAndBlock();
        }

    }
}
