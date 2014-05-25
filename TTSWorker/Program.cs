using System;
using System.Collections.Generic;
using System.Data.RSSBus.Google;
using System.IO;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Jobs;
using RSSBus.GoogleadoOps;

namespace TTSWorker
{
    class Program
    {
        static void Main()
        {
            JobHost host = new JobHost();
            host.RunAndBlock();
        }

        public static void ProcessQueueMessage([QueueInput("webjobsqueue")] string inputText,
                                              [BlobOutput("containername/blobname")]TextWriter writer)
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
                        GoogleCommand cmd = new GoogleCommand("SELECT * FROM MailMessages WHERE Id="+ readerID, connection);
                        cmd.CommandType = System.Data.CommandType.Text;
                        //cmd.Parameters.Clear();
                        //cmd.Parameters.Add(new GoogleParameter("@id", readerID));
                        GoogleDataReader mReader = cmd.ExecuteReader();

                        while (mReader.Read())
                        {

                            try
                            {
                                msgThreadID = mReader["ThreadID"].ToString();
                                msgDate = mReader["Date"].ToString();
                                GoogleCommand cmd2Unseen = new GoogleCommand("UPDATE MailMessages SET Flags='\\Unseen' WHERE Id=" + readerID, connection);
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
                writer.WriteLine(inputText);
            }
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
