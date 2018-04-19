using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MailChimp.Net;
using MailChimp.Net.Interfaces;
using Microsoft.Azure.WebJobs;
using Ninject.Extensions.Logging;
using WinSCP;

namespace LogMaintenance
{
    class Program
    {
        public static String MapBase;
        public static String MapUtcToYear;
        public static String MapUtcToMonth;
        public static String MapUtcToDay;
        public static String MapUtcToHour;
        private static readonly ILogger _logger;
        public static string db;
        public static string dbNative;

        //public static string localDb;



        static void Main(string[] args)
        {

            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            var host = new JobHost(config);
            // The following code ensures that the WebJob will be running continuously

            Functions.RunLogMaint();

        }
    }
}

