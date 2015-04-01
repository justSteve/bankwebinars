using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Caching;
using BrockAllen.MembershipReboot;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Core
{
    public class MigrationOperations
    {
        private readonly string _connectionString;
        private readonly string _connectionLegacy;

        public MigrationOperations(string connectionString, string connectionLegacy)
        {
            _connectionString = connectionString;
            _connectionLegacy = connectionLegacy;
        }


        public void CopyLegacyWebinars()
        {

            int idWebinar;
            string description;
            string descriptionLong;
            string imageUrl;
            string smallImageUrl;
            int status;
            bool featuredWebinar;
            bool reviewAllow;
            string title;
            DateTime date;
            string learnCaption;
            string learnBody;
            string whoAttend;
            decimal duration;
            string recordingUrl;
            int idPresenter;
            DateTime DateCreated;
            DateTime DateChanged;
            string ceu;

            using (var sqlConnection = new SqlConnection(_connectionLegacy))
            {
                sqlConnection.Open();

                using (var getLegacyWebinars = new SqlCommand())
                {

                    getLegacyWebinars.Connection = sqlConnection;
                    getLegacyWebinars.CommandType = CommandType.Text;
                    getLegacyWebinars.CommandText = "SELECT [idWebinar] ,[description] ,[descriptionLong] ,[imageUrl] ,[smallImageUrl] ,[status] ,[featuredWebinar] ,[reviewAllow] ,[title] ,[date] ,[learnCaption] ,[learnBody] ,[whoAttend] ,[duration] ,[recordingUrl] ,[idPresenter] ,[DateCreated] ,[DateChanged], ceu FROM [dbo].[Webinar]";

                    using (var sqlUpdateConnection = new SqlConnection(_connectionString))
                    {
                        sqlUpdateConnection.Open();

                        using (var reader = getLegacyWebinars.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                //reader.GetInt32(0), reader.GetDecimal(1)));
                                idWebinar = reader.GetInt32(0);
                                description = reader.GetString(1);
                                descriptionLong = reader.GetString(2);
                                imageUrl = reader.GetString(3);
                                smallImageUrl = reader.GetString(4);
                                status = reader.GetByte(5);
                                featuredWebinar = reader.GetBoolean(6);
                                reviewAllow = reader.GetBoolean(7);
                                title = reader.GetString(8);
                                date = reader.GetDateTime(9);
                                learnCaption = reader.GetString(10);
                                learnBody = reader.GetString(11);
                                whoAttend = reader.GetString(12);
                                duration = reader.GetDecimal(13);
                                recordingUrl = reader.GetString(14);
                                idPresenter = reader.GetInt32(15);
                                DateCreated = reader.GetDateTime(16);
                                DateChanged = reader.GetDateTime(17);
                                ceu = reader.GetString(18);


                                using (var updater = new SqlCommand())
                                {

                                    updater.Connection = sqlUpdateConnection;
                                    updater.CommandType = CommandType.Text;
                                    updater.CommandText = "UPDATE [dbo].[Webinar] SET";

                                    updater.CommandText += "      [Description] = '" + description.Replace("'", "''");
                                    updater.CommandText += "'      ,[DescriptionLong] = '" + descriptionLong.Replace("'", "''");
                                    updater.CommandText += "'      ,[ImageUrl] = '" + imageUrl;
                                    updater.CommandText += "'      ,[SmallImageUrl] = '" + smallImageUrl;
                                    updater.CommandText += "'      ,[Status] = " + status;
                                    updater.CommandText += "       ,[Title] = '" + title.Replace("'", "''");
                                    updater.CommandText += "'      ,[Date] = '" + date.ToString();
                                    updater.CommandText += "'      ,[LearnCaption] = '" + learnCaption.Replace("'", "''");
                                    updater.CommandText += "'      ,[LearnBody] = '" + learnBody.Replace("'", "''");
                                    updater.CommandText += "'      ,[WhoAttend] = '" + whoAttend.Replace("'", "''");
                                    updater.CommandText += "'      ,[Duration] = " + duration.ToString();
                                    updater.CommandText += "       ,[RecordingUrl] = '" + recordingUrl;
                                    updater.CommandText += "'      ,[idPresenter] = " + idPresenter.ToString();
                                    updater.CommandText += "       ,[DateCreated] = '" + DateCreated.ToString();
                                    updater.CommandText += "'      ,[DateChanged] = '" + DateChanged.ToString();
                                    updater.CommandText += "'      ,[ceu] = '" + ceu.Replace("'", "''");
                                    updater.CommandText += "'where idWebinar = " + idWebinar.ToString();
                                    try
                                    {
                                        updater.ExecuteNonQuery();

                                    }
                                    catch (Exception)
                                    {
                                        Debug.WriteLine("presenter insert failed: " + idWebinar);
                                    }

                                }
                            }
                        }
                    }
                }
            }

            int idWebinarFile;
            string fileLocation;
            string fileDesc;
            string myFilename;

            using (var sqlConnection = new SqlConnection(_connectionLegacy))
            {
                sqlConnection.Open();

                using (var getLegacyWebinars = new SqlCommand())
                {
                    getLegacyWebinars.Connection = sqlConnection;
                    getLegacyWebinars.CommandType = CommandType.Text;
                    getLegacyWebinars.CommandText = "SELECT [idWebinarFile] ,[idWebinar] ,[fileLocation] ,[fileDesc] ,[myFilename] FROM [dbo].[WebinarFile]";

                    using (var sqlUpdateConnection = new SqlConnection(_connectionString))
                    {
                        sqlUpdateConnection.Open();

                        using (var reader = getLegacyWebinars.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                idWebinarFile = reader.GetInt32(0);
                                idWebinar = reader.GetInt32(1);
                                fileLocation = reader.GetString(2);
                                fileDesc = reader.GetString(3);
                                myFilename = reader.GetString(4);
                                using (var wfUpdater = new SqlCommand())
                                {
                                    wfUpdater.Connection = sqlUpdateConnection;
                                    wfUpdater.CommandType = CommandType.Text;
                                    wfUpdater.CommandText = "select idWebinar from WebinarFile where idWebinar = " + idWebinar;

                                    using (var findWFReader = wfUpdater.ExecuteReader())
                                    {
                                        if (!findWFReader.HasRows)
                                        {
                                            using (var wfInserter = new SqlCommand())
                                            {
                                                {
                                                    wfInserter.Connection = sqlUpdateConnection;

                                                    wfInserter.CommandType = CommandType.Text;
                                                    wfInserter.CommandText = "INSERT INTO [dbo].[WebinarFile] ([idWebinar],[fileLocation],[fileDesc])VALUES(";
                                                    wfInserter.CommandText += idWebinar + ",'" + myFilename + "', '" + fileDesc + "')";
                                                    try
                                                    {

                                                        wfInserter.ExecuteNonQuery();
                                                    }
                                                    catch (Exception)
                                                    {
                                                        Console.WriteLine("failed to insert " + idWebinar);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            int idWebinarTopicXref;
            int idTopic;


            using (var sqlConnection = new SqlConnection(_connectionLegacy))
            {
                sqlConnection.Open();

                using (var getLegacyWebinarsTopics = new SqlCommand())
                {
                    getLegacyWebinarsTopics.Connection = sqlConnection;
                    getLegacyWebinarsTopics.CommandType = CommandType.Text;
                    getLegacyWebinarsTopics.CommandText = "SELECT [idWebinarTopicXref]       ,[idWebinar]      ,[idTopic]  FROM [dbo].[WebinarTopicXref]";

                    using (var sqlUpdateConnection = new SqlConnection(_connectionString))
                    {
                        sqlUpdateConnection.Open();

                        using (var reader = getLegacyWebinarsTopics.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                idWebinarTopicXref = reader.GetInt32(0);
                                idWebinar = reader.GetInt32(1);
                                idTopic = reader.GetInt32(2);

                                using (var wfUpdater = new SqlCommand())
                                {
                                    wfUpdater.Connection = sqlUpdateConnection;
                                    wfUpdater.CommandType = CommandType.Text;
                                    wfUpdater.CommandText = "select idWebinar from [WebinarTopicXref] where idWebinar = " + idWebinar;

                                    using (var findWFReader = wfUpdater.ExecuteReader())
                                    {
                                        if (!findWFReader.HasRows)
                                        {
                                            using (var topicInserter = new SqlCommand())
                                            {
                                                {
                                                    topicInserter.Connection = sqlUpdateConnection;

                                                    topicInserter.CommandType = CommandType.Text;
                                                    topicInserter.CommandText = "INSERT dbo.WebinarTopicXref ( idWebinar, idTopic ) VALUES  (";
                                                    topicInserter.CommandText += idWebinar + "," + idTopic + ")";
                                                    try
                                                    {

                                                        topicInserter.ExecuteNonQuery();
                                                    }
                                                    catch (Exception)
                                                    {
                                                        Debug.WriteLine("failed to insert topic" + idWebinar);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }
    }
}
