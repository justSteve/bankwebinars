using log4net.Config;
using Ninject.Extensions.Logging.Log4net.Infrastructure;
using System;
using System.Configuration;
using System.Net.Configuration;
using System.Text;

namespace CUWebinars.Azure.Common
{
    public class LogConfigurer
    {
        public static string WebjobName;

        public static void ConfigureLogging(Type typeOfLogger, bool logSettingsAtSpinup)
        {
            // reflect to get the name of the exe
            var executingAssembly = typeOfLogger.Assembly;
            string fullNameApp = executingAssembly.FullName.Substring(0, executingAssembly.FullName.IndexOf(','));
            
            string xmlFileNAme = GetXmlFileNAme();

            string log4NetConfigFile = string.Concat(fullNameApp, ".Config.", xmlFileNAme);
            WebjobName = fullNameApp.Substring(fullNameApp.LastIndexOf('.') + 1);

            var log4NetConfigAsStream = executingAssembly.GetManifestResourceStream(log4NetConfigFile);

            if (!ReferenceEquals(log4NetConfigAsStream, null))
            {
                XmlConfigurator.Configure(log4NetConfigAsStream);

                log4NetConfigAsStream.Close();
                log4NetConfigAsStream.Dispose();

                if (logSettingsAtSpinup)
                    LogSpinupDetails(typeOfLogger);
            }
            else
            {
                var a = "not here";
            }
        }

        private static string GetXmlFileNAme()
        {
            string xmlFileNAme;
            string tenant = ConfigurationManager.AppSettings["Tenant"];

            switch (tenant)
            {
                case "BankWebinars":
                    xmlFileNAme = "BWLog4net.xml";
                    break;
                case "CUWebinars":
                    xmlFileNAme = "CULog4net.xml";
                    break;
                case "DirectorSeries":
                    xmlFileNAme = "DESLog4net.xml";
                    break;
                default:
                    throw new NotSupportedException(string.Format("There is no log4net configuration for {0}", tenant));
            }
            return xmlFileNAme;
        }

        public static DateTime UtcNowAsCts
        {
            get
            {
                DateTime timeUtc = DateTime.UtcNow;
                return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
            }
        }

        private static void LogSpinupDetails(Type typeOfLogger)
        {
            const string keyValuePattern = "Key - {0}, Value - {1}{2}";
            var logger = new Log4NetLogger(typeOfLogger);

            try
            {
                logger.Info(string.Format("SPINUP: {0} Starts ", WebjobName));

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            /************* enter any logging statements here e.g. AppSettings *************/
            var stringBuilder = new StringBuilder(string.Format("SPINUP: {0}: \n", WebjobName));

            foreach (string key in ConfigurationManager.AppSettings)
            {
                var value = ConfigurationManager.AppSettings[key];
                stringBuilder.AppendFormat(keyValuePattern, key, value, Environment.NewLine);
            }

            //var smtpDetails = GetSmtpDetails();

            //stringBuilder.AppendFormat(keyValuePattern, "UserName", smtpDetails.UserName, Environment.NewLine);
            //stringBuilder.AppendFormat(keyValuePattern, "Password", smtpDetails.Password, Environment.NewLine);
            //stringBuilder.AppendFormat(keyValuePattern, "Host", smtpDetails.Host, Environment.NewLine);
            //stringBuilder.AppendFormat(keyValuePattern, "DefaultCredentials", smtpDetails.DefaultCredentials,
            //    Environment.NewLine);
            //stringBuilder.AppendFormat(keyValuePattern, "ClientDomain", smtpDetails.ClientDomain, Environment.NewLine);
            //stringBuilder.AppendFormat(keyValuePattern, "Ssl", smtpDetails.EnableSsl, Environment.NewLine);
            //stringBuilder.AppendFormat(keyValuePattern, "Port", smtpDetails.Port, Environment.NewLine);

            logger.Info(stringBuilder.ToString());
        }

        //private static SmtpNetworkElement GetSmtpDetails()
        //{
        //    var smtpSection = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;

        //    if (smtpSection == null)
        //        throw new NullReferenceException("There is no SMTP section in this App.config file.");

        //    var mailSettings = smtpSection.Network;

        //    return mailSettings;
        //} 
    }
}
