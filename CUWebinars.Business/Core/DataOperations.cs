using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using CUWebinars.Business.Models;

namespace CUWebinars.Business.Core
{
    public class DataOperations
    {
        private readonly string _connectionString;

        public DataOperations(string connectionString)
        {
            _connectionString = connectionString;
        }


        public IList<Tuple<int, decimal>> GetAdditionalLocationsPricing(int webinarId)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var getPricingsCommand = new SqlCommand())
                {
                    var webinarIdParameter = new SqlParameter
                    {
                        SqlDbType = SqlDbType.Int,
                        ParameterName = "@webinarId",
                        Value = webinarId
                    };

                    getPricingsCommand.Connection = sqlConnection;
                    getPricingsCommand.CommandType = CommandType.Text;
                    getPricingsCommand.Parameters.Add(webinarIdParameter);
                    getPricingsCommand.CommandText = "SELECT id, cost FROM AdditionalLocationsLookupPrice WHERE idWebinar = @webinarId;";

                    IList<Tuple<int, decimal>> pricingInformation = new List<Tuple<int, decimal>>();

                    using (var reader = getPricingsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pricingInformation.Add(new Tuple<int, decimal>(reader.GetInt32(0), reader.GetDecimal(1)));
                        }
                    }

                    return pricingInformation;
                }
            }
        }
        /// <summary>
        /// This method permits affiliates to show friendly label on importer
        /// </summary>
        /// <param name="idWebinar"></param>
        /// <param name="registrationType">Friendly Lable</param>
        
        public int GetRegTypeByLableAndWebinar(string registrationType, int idWebinar)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var command = new SqlCommand())
                {
                    command.Connection = sqlConnection;
                    command.CommandType = CommandType.Text;
                    command.CommandText =
                        " SELECT  regType.idRegtype FROM    dbo.RegType regType INNER JOIN dbo.RegTypesXref rx " +
                        " ON rx.idRegType = regType.idRegType INNER JOIN dbo.RegTypesGroups rtg " +
                        " ON rtg.idRegTypeGroup = rx.idRegTypeGroup INNER JOIN dbo.RegTypesGroupsXref rtgX " +
                        " ON rtgX.idRegTypeGroup = rtg.idRegTypeGroup WHERE   rtgX.idWebinar = " + idWebinar +
                        " AND regType.RegTypeLabel = '" + registrationType + "'";

                    command.CommandType = CommandType.Text;


                    //SqlParameter parameterRT = new SqlParameter();
                    //parameterRT.ParameterName = "@registrationType";
                    //parameterRT.SqlDbType = SqlDbType.;
                    //parameterRT.Direction = ParameterDirection.Input;
                    //parameterRT.Value = registrationType;

                    //SqlParameter parameteridWebinar = new SqlParameter
                    //{
                    //    ParameterName = "@idWebinar",
                    //    SqlDbType = SqlDbType.Int,
                    //    Direction = ParameterDirection.Input,
                    //    Value = idWebinar
                    //};

                    //// Add the parameter to the Parameters collection. 
                    //command.Parameters.Add(parameterRT);
                    //command.Parameters.Add(parameteridWebinar);

                    var message = command.ExecuteScalar();
                    return Convert.ToInt32(message);
                }
            }
        }
        public USTimeZone GetTimeZoneByZipCode(string zip)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();

                using (var getTimeZoneCommand = new SqlCommand())
                {
                    var zipParameter = new SqlParameter
                    {
                        SqlDbType = SqlDbType.VarChar,
                        Size = 50,
                        ParameterName = "@zip",
                        Value = zip
                    };

                    getTimeZoneCommand.Connection = sqlConnection;
                    getTimeZoneCommand.CommandType = CommandType.Text;
                    getTimeZoneCommand.Parameters.Add(zipParameter);
                    getTimeZoneCommand.CommandText = "SELECT timezone FROM TimeZoneByZip WHERE zip = @zip";

                    string timeZoneAsString = string.Empty;

                    using (var reader = getTimeZoneCommand.ExecuteReader())
                    {
                        reader.Read(); //   There should only be 1
                        timeZoneAsString = reader.GetString(0);
                    }

                    int timeZoneAsInt;
                    USTimeZone timeZone = USTimeZone.Central;

                    if (int.TryParse(timeZoneAsString, NumberStyles.AllowLeadingSign, CultureInfo.CurrentCulture, out timeZoneAsInt))
                    {
                        timeZoneAsInt += 10;
                        timeZone = (USTimeZone)timeZoneAsInt;
                    }

                    return timeZone;
                }
            }
        }
    }
}
