using System;
using System.Data;
using System.Data.SqlClient;
using CUWebinars.Web.Core;

namespace CUWebinars.Web.Membership
{
    public class DataOperations
    {
        private readonly string _connectionString;

        public DataOperations()
        {
            GlobalConfig globalConfig = GlobalConfig.GlobalConfigSingleton;
            _connectionString = globalConfig.MembershipConnectionString;
        }

        public bool SetNewAccountToVerified(Guid id)
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
            {
                sqlConnection.Open();
                using (var updateUserCommand = new SqlCommand())
                {
                    var idParamater = new SqlParameter
                    {
                        Value = id,
                        SqlDbType = SqlDbType.UniqueIdentifier,
                        Direction = ParameterDirection.Input,
                        ParameterName = "@ID"
                    };

                    updateUserCommand.Connection = sqlConnection;
                    updateUserCommand.Parameters.Add(idParamater);
                    updateUserCommand.CommandText = "UPDATE UserAccounts SET IsAccountVerified = 1 WHERE ID = @ID";
                    updateUserCommand.CommandType = CommandType.Text;

                    var numRows = updateUserCommand.ExecuteNonQuery();

                    return numRows.Equals(1);
                }
            }
        }
    }
}