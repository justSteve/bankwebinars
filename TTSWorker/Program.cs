using System;
using System.Collections.Generic;
using System.Data.RSSBus.Google;
using System.IO;
using System.Linq;
using AE.Net.Mail;
using AE.Net.Mail.Imap;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Jobs;
using RSSBus.GoogleadoOps;

namespace TTSWorker
{
    class Program
    {
        private static void Main()
        {
            var host = "imap.gmail.com";
            var username = "registrations@bankwebinars.com";
            var password = "azza123A";
            var port = 993;
            var isSSL = true;

            //using (var imap = new ImapClient(host, username, password, AE.Net.Mail.Imap.Mailbox., port, isSSL))
            //{
            //    //var msgs = imap.SearchMessages(
            //    //  SearchCondition.Undeleted().And(
            //    //    SearchCondition.From("david"),
            //    //    SearchCondition.SentSince(new DateTime(2000, 1, 1))
            //    //  ).Or(SearchCondition.To("andy"))
            //    //);

            //    //Assert.AreEqual(msgs[0].Value.Subject, "This is cool!");
                
            //    imap.NewMessage += (sender, e) =>
            //    {
            //        var msg = imap.GetMessage(e.MessageCount - 1);
            //        Console.WriteLine("here");
            //        //Assert.AreEqual(msg.Subject, "IDLE support?  Yes, please!");
            //    };
            //}

        }

        static void Main4RSSBus()
        {
            //JobHost host = new JobHost();
            //host.RunAndBlock();
            string connectionString = "user=registrations@bankwebinars.com;password=azza123A;Offline=false;";


            using (GoogleConnection connection = new GoogleConnection(connectionString))
            {
                GoogleDataAdapter gda = new GoogleDataAdapter
                {
                    SelectCommand = new GoogleCommand(
                        "SELECT id from MailMessages where [To] = 'registrations+verify@bankwebinars.com' and  SEARCHCRITERIA " +
                        "= 'UNSEEN' ",
                        connection)
                };
                var AreErrors = "";
                var msgDate = "";
                var msgThreadID = "";

                try
                {
                    GoogleDataReader reader = gda.SelectCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        AreErrors = "";
                        //
                        int readerID = Convert.ToInt32(reader["id"]);

                        if (readerID < 1)
                        {
                            break;
                        }
                        GoogleCommand cmd = new GoogleCommand("SELECT * FROM MailMessages WHERE Id=" + readerID, connection);
                        cmd.CommandType = System.Data.CommandType.Text;
                        GoogleDataReader mReader = cmd.ExecuteReader();

                        while (mReader.Read())
                        {

                            try
                            {
                                //GoogleCommand cmd2Unseen = new GoogleCommand("UPDATE MailMessages SET Flags='UNSEEN' WHERE Id=" + readerID, connection);
                                GoogleCommand cmd2Unseen = new GoogleCommand("UPDATE MailMessages SET DestinationMailbox='Errors' WHERE Id=" + readerID, connection);
                                cmd.CommandType = System.Data.CommandType.Text;
                                GoogleDataReader mReader2Unseen = cmd.ExecuteReader();
                            }
                            catch
                            {
                                throw;
                            }
                        }
                    }
                }
                catch
                {
                    throw;
                }
                //writer.WriteLine(inputText);
            }

        }

        public static void ProcessQueueMessage([QueueInput("webjobsqueue")] string inputText,
                                              [BlobOutput("containername/blobname")]TextWriter writer)
        {
        }
        public static void ProcessEmailConfirm()
        {
            string connectionString = "user=registrations@bankwebinars.com;password=azza123A;Offline=false;";


            using (GoogleConnection connection = new GoogleConnection(connectionString))
            {
                GoogleDataAdapter gda = new GoogleDataAdapter
                {
                    SelectCommand = new GoogleCommand(
                        "SELECT id from MailMessages where SEARCHCRITERIA " +
                        "= 'UNSEEN' ",
                        connection)
                };
                GoogleCommand cmd = new GoogleCommand("SELECT * FROM Calendars", connection);

                GoogleDataReader rdr = cmd.ExecuteReader();
                StringWriter sWriter = new StringWriter();
                while (rdr.Read())
                {
                    //sWriter = new StringWriter();
                    sWriter.Write(String.Format("\t{0} --> \t\t{1}", rdr["Id"], rdr["Summary"]));
                    
                }
            }
        }

    }
}
