using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MailChimp.Net;
using MailChimp.Net.Interfaces;
using Ninject.Extensions.Logging;
using WinSCP;

using Microsoft.Azure.WebJobs;

namespace LogMaintenance
{
    class Functions

    {
        public static String MapBase;
        public static String MapUtcToYear;
        public static String MapUtcToMonth;
        public static String MapUtcToDay;
        public static String MapUtcToHour;
        private static readonly ILogger _logger;
        public static string db =
            "Driver={ODBC Driver 13 for SQL Server};Server=tcp:nt2j4x3hvq.database.windows.net,1433;Database=BW33;Uid=TTSOp@nt2j4x3hvq;Pwd=HXm88WIX;Encrypt=yes;TrustServerCertificate=no;Connection Timeout=30;";
        public static string dbNative =
            "Server = tcp:nt2j4x3hvq.database.windows.net,1433; Database = BW33; User ID = TTSOp@nt2j4x3hvq; Password = HXm88WIX; Trusted_Connection = False; Encrypt = True; Connection Timeout = 30; ";




        public static void RunLogMaint()
        {
            MapBase = "BankWebinars";

            //GetLog4Net();

            //GetIISLogsAll();
            DownloadIISLog();

            //UploadIIS();
            //while (true) // Loop indefinitely
            //{
            //    Console.WriteLine("Enter input:"); // Prompt
            //    string line = Console.ReadLine(); // Get string from user
            //    if (line == "exit") // Check string
            //    {
            //        break;
            //    }
            //    if (line == "log4net")
            //        GetLog4Net();
            //    if (line == "iis")
            //        DownloadIISLog();
            //    if (line == "upload")
            //        Upload();
            //}

        }

        private static void UploadLog4Net()
        {
            var dataOperations = new DataOperations(dbNative);
            var uploadLog4Net = dataOperations.UploadLog4Net();
            var myTimeStamp = DateTime.Now.ToShortDateString() + "_"
                              + DateTime.Now.ToShortTimeString();

            var outputFile = @"d:\home\logfiles\" + MapBase + "\\UploadLog4Net_" + myTimeStamp.ToString().Replace(" / ", "_").Replace(":", "_") + ".txt";
            File.WriteAllText(outputFile, uploadLog4Net, Encoding.ASCII);
        }

        private static void UploadIIS()
        {
            var dataOperations = new DataOperations(dbNative);
            var updateIIS = dataOperations.UploadIIS();
            var myTimeStamp = DateTime.Now.ToShortDateString() + "_"
                + DateTime.Now.ToShortTimeString();

            var outputFile = @"d:\home\logfiles\processed\" + MapBase + "\\UploadIIS.txt";
            File.WriteAllText(outputFile, updateIIS, Encoding.ASCII);
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

        private static void GetLog4Net()
        {
            DateTime thisHour = DateTime.UtcNow.AddHours(-1);
            MapUtcToYear = thisHour.Year.ToString();
            MapUtcToMonth = thisHour.Month.ToString();
            if (thisHour.Month < 10)
                MapUtcToMonth = "0" + MapUtcToMonth;

            MapUtcToDay = thisHour.Date.ToString("dd");
            MapUtcToHour = thisHour.Hour.ToString();
            if (thisHour.Hour < 10)
                MapUtcToHour = "0" + thisHour.Hour.ToString();

            SessionOptions sessionOptions = new SessionOptions();
            Session session = new Session();
            sessionOptions.Protocol = Protocol.Ftp;
            sessionOptions.HostName = "waws-prod-ch1-005.ftp.azurewebsites.windows.net";
            sessionOptions.PortNumber = 21;

            if (MapBase == "BankWebinars")
            {
                sessionOptions.UserName = @"BankWebinars33\$BankWebinars33";
                sessionOptions.Password = "4q4YMhgci87z8HRYq58Y5YrCvBZaexJGrgFNsTxwanLnaxbDlLnuTeTrSaAF";
            }

            if (MapBase == "CUWebinars")
            {
                sessionOptions.UserName = @"CUWebinars33\$CUWebinars33";
                sessionOptions.Password = "nZvMRHXwuxkPwsygWmpfwwHiEWTak2Dpw6FXdef6sii1Pxo35i17mDzXNdpX";
            }

            if (MapBase == "ttsCCS")
            {
                sessionOptions.UserName = @"ttsCCS\$ttsCCS";
                sessionOptions.Password = "1BBwKErM6pYYDgcxSKaESTNiGdp7iwN7yNBbXFCaDuKt1LwgaQfEhfCciPTH";
            }

            if (MapBase == "DES33")
            {
                sessionOptions.UserName = @"DES33\$DES33";
                sessionOptions.Password = "ihoFmRdWATPt5YYlFoHDSawWHhk1MkwAjWfgoc0Pl2idD9vtoBejJyxah2l8";
            }
            using (session)
            {
                // Connect
                session.Open(sessionOptions);
                string timeStamp = DateTime.Now.ToString("MM_dd_yy_h_mm");
                session.MoveFile("/logfiles/log4netCSV.log", "/logfiles/log4netCSV." + timeStamp + ".log");
                session.GetFiles("/logfiles/log4netCSV." + timeStamp + ".log", @"d:\home\logfiles\processed\" + MapBase + "\\*").Check();
            }

            string[] files = Directory.GetFiles("d:\\home\\logfiles\\processed\\" + MapBase, "*.log",
                SearchOption.AllDirectories);

            // Display all the files.
            foreach (string file in files)
            {

                string str = "DateTime,Thread,Level,Logger,Message,Exception\r\n";
                str += File.ReadAllText(file, Encoding.ASCII);


                string pattern = @"(?m)\r?\n^(?!""2018)";
                string substitution = @"";

                Regex regex = new Regex(pattern);
                string result = regex.Replace(str, substitution);
                var outputFile = @"d:\home\logfiles\processed\" + MapBase + "\\output.txt";
                File.WriteAllText(outputFile, "DateTime,Thread,Level,Logger,Message,Exception\r\n", Encoding.ASCII);
                File.WriteAllText(outputFile, result, Encoding.ASCII);
                var lpOutput = RunCmd("logparser \"SELECT DateTime,Thread,Level,Logger,Message,Exception  into Log4Net FROM '" + outputFile + "\"' -i:CSV -e:1 -o:SQL -createTable:ON -oConnString:\"" + db + "\"");
                //File.Delete(outputFile);

                if (lpOutput.ToLower().Contains("aborted"))
                {
                    var a = 1;
                }
                File.Move(file, file.ToString().Replace(".log", ".done"));

                Debug.WriteLine(lpOutput);
                UploadLog4Net();
            }
        }

        private static void DownloadIISLog()
        {

            DateTime thisHour = DateTime.UtcNow.AddHours(-1);
            MapUtcToYear = thisHour.Year.ToString();
            MapUtcToMonth = thisHour.Month.ToString();
            if (thisHour.Month < 10)
                MapUtcToMonth = "0" + MapUtcToMonth;

            MapUtcToDay = thisHour.Date.ToString("dd");

            //subtract an hour to prevent downloading partial logs
            MapUtcToHour = thisHour.AddHours(-1).Hour.ToString();
            if (thisHour.Hour < 10)
                MapUtcToHour = "0" + thisHour.Hour.ToString();

            var blob = BlobHelper.GetBlob("sitelogbw",
                "BANKWEBINARS33/" + MapUtcToYear + "/" + MapUtcToMonth + "/" + MapUtcToDay + "/" + MapUtcToHour + "/",
                "2e933b.log");


            var blobAsFile = "";


            if (MapBase == "BankWebinars")
            {
                //blob = BlobHelper.GetBlob("sitelogbw",
                //    "BANKWEBINARS33/" + MapUtcToYear + "/" + MapUtcToMonth + "/" + MapUtcToDay + "/" + MapUtcToHour + "/",
                //    "2e933b.log");

                var blobString = BlobHelper.FindLogFile("sitelogbw",
                    "BANKWEBINARS33/" + MapUtcToYear + "/" + MapUtcToMonth + "/" + MapUtcToDay + "/" + MapUtcToHour).ToString();

                blobAsFile = BlobHelper.GetBlobAsFile("sitelogbw", blobString);

            }

            if (MapBase == "CUWebinars")
            {
                //blob = BlobHelper.GetBlob("logsite",
                //    "CUWEBINARS33/" + MapUtcToYear + "/" + MapUtcToMonth + "/" + MapUtcToDay + "/" + MapUtcToHour + "/",
                //    "2e933b.log");
                blobAsFile = BlobHelper.GetBlobAsFile("sitelogbw",
                    "CUWEBINARS33/" + MapUtcToYear + "/" + MapUtcToMonth + "/" + MapUtcToDay + "/" + MapUtcToHour + "/e7f7ce.log");

            }

            if (MapBase == "ttsCCS")
            {
                //blob = BlobHelper.GetBlob("sitelogbw",
                //    "ttsccs/" + MapUtcToYear + "/" + MapUtcToMonth + "/" + MapUtcToDay + "/" + MapUtcToHour + "/",
                //    "2e933b.log");
                blobAsFile = BlobHelper.GetBlobAsFile("sitelogbw",
                    "ttsccs/" + MapUtcToYear + "/" + MapUtcToMonth + "/" + MapUtcToDay + "/" + MapUtcToHour + "/69052d.log");
            }

            if (MapBase == "DES33")
            {
                blob = BlobHelper.GetBlob("logsite",
                    "DES33/" + MapUtcToYear + "/" + MapUtcToMonth + "/" + MapUtcToDay + "/" + MapUtcToHour + "/",
                    "2e933b.log");
                //https://storefordes1.blob.core.windows.net/logsite/DES33/2018/04/16/21/e7f7ce.log
                blobAsFile = BlobHelper.GetBlobAsFile("logsite",
                    "DES33/" + MapUtcToYear + "/" + MapUtcToMonth + "/" + MapUtcToDay + "/" + MapUtcToHour + "/e7f7ce.log");
            }

            GetIISLog(blobAsFile);
        }

        private static void GetIISLog(string text)
        {

            var myTimeStamp = DateTime.Now.ToShortTimeString();
            var outputFile = @"d:\home\logfiles\processed\" + MapBase + "\\outputIIS.log";
            File.WriteAllText(outputFile, text, Encoding.ASCII);

            var lpOutput = RunCmd("logparser \"select TO_TIMESTAMP(date, time), [time] ,[s-Sitename] ,[cs-Method] ,[cs-Uri-Stem] ,[cs-Uri-Query] ,[s-Port] ,[cs-Username] ,[c-Ip] ,[cs(User-Agent)] ,[cs(Cookie)] ,[cs(Referer)] ,[cs-Host] ,[sc-Status] ,[sc-Substatus] ,[sc-Win32-Status] ,[sc-Bytes] ,[cs-Bytes] ,[time-Taken],1  into SiteLog FROM '" +
                                      outputFile + "'\" -i:W3C -e:1 -o:SQL -createTable:ON -oConnString:\"" + db + "\"", "");
            UploadIIS();
            Debug.WriteLine(lpOutput);
        }
        private static void GetIISLogsAll()
        {
            string[] files = Directory.GetFiles("C:\\Users\\steve\\Desktop\\logfiles\\BANKWEBINARS33\\2018\\03\\",
                "*.log",
                SearchOption.AllDirectories);

            // Display all the files.
            foreach (string file in files)
            {
                var lpOutput = RunCmd("logparser \"select TO_TIMESTAMP(date, time), [time] ,[s-Sitename] ,[cs-Method] ,[cs-Uri-Stem] ,[cs-Uri-Query] ,[s-Port] ,[cs-Username] ,[c-Ip] ,[cs(User-Agent)] ,[cs(Cookie)] ,[cs(Referer)] ,[cs-Host] ,[sc-Status] ,[sc-Substatus] ,[sc-Win32-Status] ,[sc-Bytes] ,[cs-Bytes] ,[time-Taken], 1  into SiteLog FROM '" +
                                      file +
                                      "'\" -i:W3C -e:1 -o:SQL -createTable:ON -oConnString:\"" + db + "\"", "");
                Debug.WriteLine(lpOutput);

                File.Move(file, file.ToString().Replace(".log", ".done"));
            }
            UploadIIS();
        }

        private static void GetIISLogs(string userEnteredDay)
        {
            string[] files = Directory.GetFiles("C:\\Users\\steve\\Desktop\\logfiles\\BANKWEBINARS33\\2018\\03\\" + userEnteredDay + "\\",
                "*.log",
                SearchOption.AllDirectories);

            // Display all the files.
            foreach (string file in files)
            {
                var lpOutput = RunCmd("logparser \"select TO_TIMESTAMP(date, time), [time] ,[s-Sitename] ,[cs-Method] ,[cs-Uri-Stem] ,[cs-Uri-Query] ,[s-Port] ,[cs-Username] ,[c-Ip] ,[cs(User-Agent)] ,[cs(Cookie)] ,[cs(Referer)] ,[cs-Host] ,[sc-Status] ,[sc-Substatus] ,[sc-Win32-Status] ,[sc-Bytes] ,[cs-Bytes] ,[time-Taken], 1  into SiteLog FROM '" +
                                      file +
                                      "'\" -i:W3C -e:1 -o:SQL -createTable:ON -oConnString:\"" + db + "\"", "");
                Debug.WriteLine(lpOutput);

                File.Move(file, file.ToString().Replace(".log", ".done"));
            }
            UploadIIS();
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
                    //var cmd = System.Text.RegularExpressions.Regex.Unescape(command);

                    sw.WriteLine(command);
                }

                sw.Close();
                returnvalue = sr.ReadToEnd();
            }

            return returnvalue;
        }

        public async Task<string> BuildMCList(string messageBody, string name)
        {

            _logger.Info("BuildMCList starting");

            try
            {

                IMailChimpManager manager = new MailChimpManager("b864fb8a5039b1152c7b774b6602a9e9-us10");
                var lists =
                await manager.Lists.GetAllAsync().ConfigureAwait(false);

                //var list = 

                //.GetAsync(_globalConfig.TenantMailChimpList).ConfigureAwait(false);

                return "";

            }
            catch (Exception exception)
            {
                _logger.FatalException("GenerateMailChimpCampaign: ", exception);
                return exception.Message;
            }
        }
    }
}

