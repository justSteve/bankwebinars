using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

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

            var _connSproc = new SqlConnection(ConfigurationManager.ConnectionStrings["TTSDatabase"].ConnectionString);

            _connSproc.Open();

            SqlCommand cmdGetBody = new SqlCommand(
                "select City, (select stateAbbreviation from States where StateCode " +
                "= TTSDatabase.StateCode) from TTSDatabase where zipcode = " + zipCode, _connSproc);


            cmdGetBody.CommandType = CommandType.Text;
            // execute the command
            SqlDataReader msgReader = cmdGetBody.ExecuteReader();

            var value = "";

            while (msgReader.Read())
            {
                return value = msgReader.FieldCount > 0 ? msgReader[0].ToString() + "," + msgReader[1].ToString() : null;
            }
            return value;
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


    }
}