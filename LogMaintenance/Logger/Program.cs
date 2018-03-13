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
            // Get list of files in the specific directory.
            // ... Please change the first argument.
            //Uri blob = null;

            //try
            //{
            //    //https://storeforbw.blob.core.windows.net/sitelogbw/BANKWEBINARS33/2018/03/10/15/2e933b.log
            //    var blobTest = BlobHelper.GetBlob("sitelogbw", "BANKWEBINARS33/" + MapUtcToYear + "/" + MapUtcToMonth + "/" + MapUtcToDay + "/" + MapUtcToHour, "2e933b.log");
            //    if (blobTest != null)
            //    {
            //        blob = Helpers.BlobHelper.GetInvoiceForPage(blobTest.FileName);
            //     var lpOutput =   RunCmd("logparser select[date] ,[time] ,[s-Sitename] ,[cs-Method] ,[cs-Uri-Stem] ,[cs-Uri-Query] ,[s-Port] ,[cs-Username] ,[c-Ip] ,[cs(User - Agent)] ,[cs(Cookie)] ,[cs(Referer)] ,[cs-Host] ,[sc-Status] ,[sc-Substatus] ,[sc-Win32-Status] ,[sc-Bytes] ,[cs-Bytes] ,[time-Taken], 1  into SiteLog FROM '" +
            //            "http://storeforbw.blob.core.windows.net/sitelogbw/BANKWEBINARS33/2018/03/10/15/2e933b.log" +
            //            "'\" -i:W3C -o:SQL -createTable:ON -oConnString:\"Driver={SQL Server Native Client 11.0}; Server=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=E:\\Users\\Steve\\seeder.mdf;Integrated Security = True; Connect Timeout = 30;", "");
            //        Console.WriteLine(lpOutput);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _logger.FatalException("Get Log: ", ex);
            //}

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
            string outputFile = ("C:\\Users\\steve\\Desktop\\logfiles\\Output1.txt");
            var f = "C:\\Users\\steve\\Desktop\\logfiles\\log4netCSV1.log";
            //var f = "C:\\Users\\steve\\Desktop\\logfiles\\tester.log";
            var totalLines = CountLinesInFile(f);
            var currentLine = 0;
            //using (StreamReader r = new StreamReader(f))
            //{
            //    try
            //    {
            //        string line;
            //        string tempLine = "";
            //        bool IsInACSImported = false;
            //        while ((line = r.ReadLine()) != null)
            //        {
            //            currentLine++;
            //            if (IsInACSImported)
            //            {
            //                if (!line.StartsWith("\"2018"))
            //                {
            //                    var a = 3;
            //                }
            //                else
            //                {
            //                    tempLine = "";
            //                    IsInACSImported = false;
            //                }
            //            }
            //            else
            //            {
            //                IsInACSImported = false;

            //                if (tempLine == "")
            //                    tempLine = line;

            //                if (line != "DateTime,Thread,Level,Logger,Message,Exception")
            //                {
            //                    if (line.Contains("ACS Imported:"))
            //                    {
            //                        IsInACSImported = true;
            //                    }
            //                    else
            //                    {
            //                        Debug.WriteLine(currentLine + " of " + totalLines);
            //                        if (tempLine.StartsWith("\"2018"))
            //                        {
            //                            using (StreamWriter sw = File.AppendText(outputFile))
            //                            {
            //                                sw.WriteLine(tempLine);
            //                            }
            //                            tempLine = "";
            //                        }
            //                        else
            //                        {
            //                            tempLine += line;
            //                        }
            //                    }
            //                }
            //                else
            //                {
            //                    tempLine = "";
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);
            //        throw;
            //    }
            //}
            string str = File.ReadAllText("C:\\Users\\steve\\Desktop\\logfiles\\Output.txt");
            
            Regex rx = new Regex("(?m)\r?\n^(?!\"2018)", RegexOptions.Singleline);
            //Regex rx = new Regex("(?m)\r?\n^(?!\"2018)", RegexOptions.Singleline);

            str = rx.Replace(str, "\"2018");
            
            File.WriteAllText("C:\\Users\\steve\\Desktop\\logfiles\\test1.txt", str);


            //var lpOutput = RunCmd("logparser \"SELECT DateTime,Thread,Level,Logger,Message,Exception,\'\' into z_OrderSynch FROM '" +
            //                      file +
            //                      "'\" -i:W3C -e:1 -o:SQL -createTable:ON -oConnString:\"Driver={SQL Server Native Client 11.0}; Server=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=E:\\Users\\Steve\\seeder.mdf;Integrated Security = True; Connect Timeout = 30;", "");
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
                var lpOutput = RunCmd("logparser \"select[date] ,[time] ,[s-Sitename] ,[cs-Method] ,[cs-Uri-Stem] ,[cs-Uri-Query] ,[s-Port] ,[cs-Username] ,[c-Ip] ,[cs(User-Agent)] ,[cs(Cookie)] ,[cs(Referer)] ,[cs-Host] ,[sc-Status] ,[sc-Substatus] ,[sc-Win32-Status] ,[sc-Bytes] ,[cs-Bytes] ,[time-Taken], 1  into SiteLog FROM '" +
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

