using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LogMaintenance.Logger.Helpers;
using Microsoft.Azure.Storage;
using Ninject.Extensions.Logging;
using WinSCP;

namespace LogMaintenance.Logger
{
    class Program
    {
        public static String MapBase;
        public static String MapUtcToYear;
        public static String MapUtcToMonth;
        public static String MapUtcToDay;
        public static String MapUtcToHour;
        private static readonly ILogger _logger;



        static void Main(string[] args)
        {
            MapBase = "BankWebinars";
            DateTime thisHour = DateTime.UtcNow.AddHours(-1);
            MapUtcToYear = thisHour.Year.ToString();
            MapUtcToMonth = thisHour.Month.ToString();
            if (thisHour.Month < 10)
                MapUtcToMonth = "0" + MapUtcToMonth;

            MapUtcToDay = thisHour.Date.ToString("dd");
            MapUtcToHour = thisHour.Hour.ToString();
            if (thisHour.Hour < 10)
                MapUtcToHour = "0" + thisHour.Hour.ToString();

            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = "waws-prod-ch1-005.ftp.azurewebsites.windows.net",
                PortNumber = 21,
                UserName = @"BankWebinars33\$BankWebinars33",
                Password = "4q4YMhgci87z8HRYq58Y5YrCvBZaexJGrgFNsTxwanLnaxbDlLnuTeTrSaAF",
            };

            if (MapBase == "BankWebinars")
            {

            }
            else
            {

                // Set up session options
                sessionOptions = new SessionOptions
                {
                    Protocol = Protocol.Ftp,
                    HostName = "waws-prod-ch1-005.ftp.azurewebsites.windows.net",
                    PortNumber = 21,
                    UserName = @"CUWebinars33\$CUWebinars33",
                    Password = "nZvMRHXwuxkPwsygWmpfwwHiEWTak2Dpw6FXdef6sii1Pxo35i17mDzXNdpX",
                };

                using (Session session = new Session())
                {
                    // Connect
                    session.Open(sessionOptions);

                    // Your code
                }


            }
            //ProcessLogs();
            var returnLable = "";
            // Set up session options

            using (Session session = new Session())
            {
                // Connect
                session.Open(sessionOptions);

                //session.PutFiles(@"C:\")
            }

            //BuildIISLog();
            BuildLog4Net();
        }
        static long CountLinesInFile(string f)
        {
            long count = 0;
            using (StreamReader r = new StreamReader(f))
            {
                string line;
                while ((line = r.ReadLine()) != null)
                {
                    count++;
                }
            }
            return count;
        }
        static private void BuildLog4Net()
        {
            //string[] files = Directory.GetFiles
            //File.Delete("C:\\Users\\steve\\Desktop\\logfiles\\Output.txt");
            string outputFile = ("C:\\Users\\steve\\Desktop\\logfiles\\Output.txt");
            var f = "C:\\Users\\steve\\Desktop\\logfiles\\log4netCSV.log";

            string str = File.ReadAllText("C:\\Users\\steve\\Desktop\\logfiles\\Output.txt", Encoding.ASCII);
            var filePath = "C:\\Users\\steve\\Desktop\\logfiles\\Output1.txt";

            string pattern = @"(?m)\r?\n^(?!""2018)";
            string substitution = @"";
            //Regex rx = new Regex("(?m)\r?\n^(?!\"2018)", RegexOptions.Singleline);

            Regex regex = new Regex(pattern);
            string result = regex.Replace(str.ToString(), substitution);

            File.WriteAllText("C:\\Users\\steve\\Desktop\\logfiles\\test1.txt", result);

            var lpOutput = RunCmd("logparser \"SELECT DateTime,Thread,Level,Logger,Message,Exception,\'\' into SiteLog FROM 'C:\\Users\\steve\\Desktop\\logfiles\\Output.txt'\" -i:W3C -e:1 -o:SQL -createTable:ON -oConnString:\"Driver={SQL Server Native Client 11.0}; Server=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=E:\\Users\\Steve\\seeder.mdf;Integrated Security = True; Connect Timeout = 30;", "");
            //Debug.WriteLine(lpOutput);

        }

        private static void BuildIISLog()
        {
            string[] files = Directory.GetFiles("C:\\Users\\steve\\Desktop\\logfiles\\BANKWEBINARS33\\2018\\03\\",
                "*.log",
                SearchOption.AllDirectories);

            // Display all the files.
            foreach (string file in files)
            {
                var lpOutput = RunCmd("logparser \"select TO_TIMESTAMP(date, time), [time] ,[s-Sitename] ,[cs-Method] ,[cs-Uri-Stem] ,[cs-Uri-Query] ,[s-Port] ,[cs-Username] ,[c-Ip] ,[cs(User-Agent)] ,[cs(Cookie)] ,[cs(Referer)] ,[cs-Host] ,[sc-Status] ,[sc-Substatus] ,[sc-Win32-Status] ,[sc-Bytes] ,[cs-Bytes] ,[time-Taken], 1  into SiteLog FROM '" +
                                      file +
                                      "'\" -i:W3C -e:1 -o:SQL -createTable:ON -oConnString:\"Driver={SQL Server Native Client 11.0}; Server=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=E:\\Users\\Steve\\seeder.mdf;Integrated Security = True; Connect Timeout = 30;", "");
                Debug.WriteLine(lpOutput);
            }

        }

        public static string RunCmd(params string[] commands)
        {
            string returnvalue = string.Empty;

            ProcessStartInfo info = new ProcessStartInfo("cmd");
            info.UseShellExecute = false;
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = true;
            info.CreateNoWindow = true;

            using (Process process = Process.Start(info))
            {
                StreamWriter sw = process.StandardInput;
                StreamReader sr = process.StandardOutput;

                foreach (string command in commands)
                {
                    sw.WriteLine(command);
                }

                sw.Close();
                returnvalue = sr.ReadToEnd();
            }

            return returnvalue;
        }
    }

}

