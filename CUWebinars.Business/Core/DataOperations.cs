using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using CUWebinars.Business.Constants;
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
                    
                    if (int.TryParse(timeZoneAsString, NumberStyles.AllowLeadingSign, CultureInfo.CurrentCulture,  out timeZoneAsInt))
                    {
                        timeZoneAsInt += 10;
                        timeZone = (USTimeZone) timeZoneAsInt;
                    }

                    return timeZone;
                }
            }
        }
    }
}
