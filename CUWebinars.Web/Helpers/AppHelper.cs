using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using CUWebinars.Business.Models;

namespace CUWebinars.Web.Helpers
{
    public class AppHelper
    {
        /// <summary>
        /// Retrieves the city state zip via URL parameter and stored in the ASP.NET Session
        /// </summary>
        /// <returns>City | State based on input zipcode</returns>
        public static String GetCityStateFromZip(int zipCode)
        {

            //var _connSproc = new SqlConnection(ConfigurationManager.ConnectionStrings["LoggerConnection"].ConnectionString);

            //_connSproc.Open();
            //var body =
            //    "select City, (select stateAbbreviation from States where StateCode = (SELECT StateCode FROM zipcodes WHERE zipcode = " + zipCode + ")),(SELECT timezone FROM [dbo].[TimeZoneByZip] WHERE zip = '" + zipCode + "') from zipcodes where zipcode = " + zipCode;

            //SqlCommand cmdGetBody = new SqlCommand(
            //    body, _connSproc
            //    );

            //cmdGetBody.CommandType = CommandType.Text;
            //// execute the command
            //SqlDataReader msgReader = cmdGetBody.ExecuteReader();

            //var value = "";

            //while (msgReader.Read())
            //{
            //    return value = msgReader.FieldCount > 0 ? msgReader[0].ToString() + "," + msgReader[1].ToString() + "," + msgReader[2].ToString() : null;
            //}
            return "Omaha,NE,Central";
        }

        public static USTimeZone ComputeTimeZone(string offset)
        {
            var timeZone = USTimeZone.Central;
            switch (offset)
            {
                case "-5":
                    timeZone = USTimeZone.Eastern;
                    break;
                case "-6":
                    timeZone = USTimeZone.Central;
                    break;
                case "-7":
                    timeZone = USTimeZone.Mountain;
                    break;
                case "-8":
                    timeZone = USTimeZone.Pacific;
                    break;
                case "-9":
                    timeZone = USTimeZone.Alaska;
                    break;
                case "-10":
                    timeZone = USTimeZone.Hawaii;
                    break;
            }
            return timeZone;
        }

        public static IEnumerable<char> CharsToTitleCase(string s)
        {
            s = s.ToLower();
            bool newWord = true;
            foreach (char c in s)
            {
                if (newWord)
                {
                    yield return Char.ToUpper(c);
                    newWord = false;
                }
                else yield return Char.ToLower(c);
                if (c == ' ') newWord = true;
            }
        }

        //36632
        /// <summary>
        /// Retrieves the City State info found via FDI Certification code
        /// </summary>
        /// <returns>Affiliate or null</returns>
        public static String GetCityStateFromCert(int Cert)
        {

            var _connSproc = new SqlConnection(ConfigurationManager.ConnectionStrings["Institutions"].ConnectionString);

            _connSproc.Open();

            SqlCommand cmdGetBody = new SqlCommand(
                "select City, (select stateAbbreviation from States where StateName " +
                "= (select STNAME from Institutions where Cert = "
                + Cert + ")), ZIP, ADDRESS, NAME  from Institutions where Cert = " + Cert, _connSproc);


            cmdGetBody.CommandType = CommandType.Text;

            // execute the command
            SqlDataReader msgReader = cmdGetBody.ExecuteReader();

            var value = "";

            while (msgReader.Read())
            {
                value = msgReader.FieldCount > 0 ? msgReader[0].ToString() + "|" + msgReader[1].ToString() + "|" + msgReader[2].ToString() + "|" + msgReader[3].ToString() + "|" + msgReader[4].ToString() : null;
            }
            return value;
        }


        public static List<string> InstitutionAutoComplete(string name, string zip)
        {
            var _connSproc = new SqlConnection(ConfigurationManager.ConnectionStrings["LoggerConnection"].ConnectionString);
            _connSproc.Open();
            SqlCommand cmdGetBody = new SqlCommand(
                "select top 15 Name +'|'+ city + ', ' + STNAME from Institution where NAME like '" + name + "%' " +
                " order by name", _connSproc);

            cmdGetBody.CommandType = CommandType.Text;
            // execute the command
            SqlDataReader msgReader = cmdGetBody.ExecuteReader();

            List<string> value = new List<string>();

            while (msgReader.Read())
            {
                if (msgReader.HasRows)
                {
                    value.Add(msgReader[0].ToString());
                }
            }
            return value;
        }
    }
}