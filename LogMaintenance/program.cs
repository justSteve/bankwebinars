using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
        public static string localDb;



        static void Main(string[] args)
        {
            localDb = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\steve\\Logger.mdf;Integrated Security=True;Connect Timeout=30";
            db = "Driver={ODBC Driver 13 for SQL Server};Server=tcp:nt2j4x3hvq.database.windows.net,1433;Database=BW33;Uid=TTSOp@nt2j4x3hvq;Password=HXm88WIX;Encrypt=yes;TrustServerCertificate=no;Connection Timeout=30";

            GetLog4Net();
            //Upload();
            //GetIISLogsAll();
            //GetIISLogs("19");
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
            var dataOperations = new DataOperations(localDb);
            var uploadLog4Net = dataOperations.UploadLog4Net();
            var myTimeStamp = DateTime.Now.ToShortDateString() + "_"
                              + DateTime.Now.ToShortTimeString();

            var outputFile = @"C:\Users\steve\Desktop\logfiles\UploadLog4Net_" + myTimeStamp.ToString().Replace("/", "_").Replace(":", "_") + ".txt";
            File.WriteAllText(outputFile, uploadLog4Net, Encoding.ASCII);
        }

        private static void UploadIIS()
        {
            var dataOperations = new DataOperations(localDb);
            var updateIIS = dataOperations.UploadIIS();
            var myTimeStamp = DateTime.Now.ToShortDateString() + "_"
                + DateTime.Now.ToShortTimeString();

            var outputFile = @"C:\Users\steve\Desktop\logfiles\UploadIIS_" + myTimeStamp.ToString().Replace("/", "_").Replace(":", "_") + ".txt";
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

            SessionOptions sessionOptions = new SessionOptions();
            Session session = new Session();

            if (MapBase == "BankWebinars")
            {

                sessionOptions.Protocol = Protocol.Ftp;
                sessionOptions.HostName = "waws-prod-ch1-005.ftp.azurewebsites.windows.net";
                sessionOptions.PortNumber = 21;
                sessionOptions.UserName = @"BankWebinars33\$BankWebinars33";
                sessionOptions.Password = "4q4YMhgci87z8HRYq58Y5YrCvBZaexJGrgFNsTxwanLnaxbDlLnuTeTrSaAF";
            }
            else
            {
                sessionOptions.Protocol = Protocol.Ftp;
                sessionOptions.HostName = "waws-prod-ch1-005.ftp.azurewebsites.windows.net";
                sessionOptions.PortNumber = 21;
                sessionOptions.UserName = @"CUWebinars33\$CUWebinars33";
                sessionOptions.Password = "nZvMRHXwuxkPwsygWmpfwwHiEWTak2Dpw6FXdef6sii1Pxo35i17mDzXNdpX";
            }
            using (session)
            {
                // Connect
                session.Open(sessionOptions);
                string timeStamp = DateTime.Now.ToString("MM_dd_yy_h_mm");
                session.MoveFile("/logfiles/log4netCSV.log", "/logfiles/log4netCSV." + timeStamp + ".log");
                session.GetFiles("/logfiles/log4netCSV." + timeStamp + ".log", @"C:\Users\steve\Desktop\logfiles\" + MapBase + "\\*").Check();
            }

            string[] files = Directory.GetFiles("C:\\Users\\steve\\Desktop\\logfiles\\" + MapBase, "*.log",
                SearchOption.AllDirectories);

            // Display all the files.
            foreach (string file in files)
            {

                string str = "DateTime,Thread,Level,Logger,Message,Exception\r\n";
                str += File.ReadAllText(file, Encoding.ASCII);

                File.Move(file, file.ToString().Replace(".log", ".done"));

                string pattern = @"(?m)\r?\n^(?!""2018)";
                string substitution = @"";

                Regex regex = new Regex(pattern);
                string result = regex.Replace(str, substitution);
                var outputFile = @"C:\Users\steve\Desktop\logfiles\output.txt";
                File.WriteAllText(outputFile, "DateTime,Thread,Level,Logger,Message,Exception\r\n", Encoding.ASCII);
                File.WriteAllText(outputFile, result, Encoding.ASCII);
                var lpOutput = RunCmd("logparser \"SELECT DateTime,Thread,Level,Logger,Message,Exception  into Log4Net FROM '" + outputFile + "\"' -i:CSV -e:1 -o:SQL -createTable:ON -oConnString:\"" + db + "\"");
                //File.Delete(outputFile);

                if (lpOutput.ToLower().Contains("aborted"))
                {
                    var a = 1;
                }

                Debug.WriteLine(lpOutput);
                UploadLog4Net();

            }
        }

        private static void DownloadIISLog()
        {
            MapBase = "BankWebinars";
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

            var blobAsFile = BlobHelper.GetBlobAsFile("sitelogbw",
                "BANKWEBINARS33/" + MapUtcToYear + "/" + MapUtcToMonth + "/" + MapUtcToDay + "/" + MapUtcToHour + "/2e933b.log");
            GetIISLog(blobAsFile);
        }

        private static void GetIISLog(string text)
        {

            var myTimeStamp = DateTime.Now.ToShortTimeString();
            var outputFile = @"C:\Users\steve\Desktop\logfiles\iislog.log";
            File.WriteAllText(outputFile, text, Encoding.ASCII);

            var lpOutput = RunCmd("logparser \"select TO_TIMESTAMP(date, time), [time] ,[s-Sitename] ,[cs-Method] ,[cs-Uri-Stem] ,[cs-Uri-Query] ,[s-Port] ,[cs-Username] ,[c-Ip] ,[cs(User-Agent)] ,[cs(Cookie)] ,[cs(Referer)] ,[cs-Host] ,[sc-Status] ,[sc-Substatus] ,[sc-Win32-Status] ,[sc-Bytes] ,[cs-Bytes] ,[time-Taken],1  into SiteLog FROM '" +
                                      outputFile +
                                      "'\" -i:W3C -e:1 -o:SQL -createTable:ON -oConnString:\"" + db + "\"", "");
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
    }

}

