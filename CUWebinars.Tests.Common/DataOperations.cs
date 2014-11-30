using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using CUWebinars.Business.Core.Tracing;

namespace CUWebinars.Tests.Common
{
    public class DataOperations
    {
        public string ConnectionString { get; set; }

        public Guid GetIdOfTestUser(string email)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var loggerQueryCommand = new SqlCommand())
                {
                    loggerQueryCommand.Connection = sqlConnection;
                    loggerQueryCommand.CommandText =
                        string.Format("SELECT ID FROM UserAccounts WHERE Email = '{0}';", email);
                    loggerQueryCommand.CommandType = CommandType.Text;

                    var message = loggerQueryCommand.ExecuteScalar();

                    return new Guid(message.ToString());
                }
            }
        }


        public
            string GetNameOfLatestPersistedNotification()
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var loggerQueryCommand = new SqlCommand())
                {
                    loggerQueryCommand.Connection = sqlConnection;
                    loggerQueryCommand.CommandText =
                        "SELECT TOP 1 Message FROM CULog WHERE Level = 'INFO' AND LEFT(Message, 25) = 'Persisted Email for Order' ORDER BY [DATE] DESC;";
                    loggerQueryCommand.CommandType = CommandType.Text;

                    var message = loggerQueryCommand.ExecuteScalar();

                    return new string(message.ToString().SkipWhile(l => l != ':').Skip(1).ToArray());
                }
            }
        }

        public bool DeleteOrder(int idOrder)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var loggerQueryCommand = new SqlCommand())
                {
                    loggerQueryCommand.Connection = sqlConnection;
                    loggerQueryCommand.CommandText = string.Format("DELETE FROM [dbo].[Order] WHERE idOrder = {0};", idOrder);
                    
                    loggerQueryCommand.CommandType = CommandType.Text;

                    var numRows = loggerQueryCommand.ExecuteNonQuery();

                    return numRows == 1;
                }
            }
        }
        
        public void DeleteMostRecentOrderOfUser(string email)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                using (var getMostRecentOrderOfUser = new SqlCommand())
                {
                    getMostRecentOrderOfUser.Connection = sqlConnection;
                    getMostRecentOrderOfUser.CommandText = string.Format("SELECT TOP 1 * FROM [Order] WHERE idUser = (select idUser from WebUser where email = '{0}') order by OrderDate desc", email);
                    
                    getMostRecentOrderOfUser.CommandType = CommandType.Text;

                    int? orderId = null;

                    using (var orderReader = getMostRecentOrderOfUser.ExecuteReader())
                    {
                        orderReader.Read();
                        orderId = orderReader.GetInt32(0);
                    }

                    if(orderId.HasValue)
                        DeleteOrder(orderId.Value);
                    else
                        throw new Exception(string.Format("Order for user {0} not found", email));
                }
            }
        }

        public bool DeleteUserAccountAndClaims(string email)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                Guid id = default(Guid);
                sqlConnection.Open();

                using (var getMostRecentWebUserIdCommand = new SqlCommand())
                {
                    getMostRecentWebUserIdCommand.Connection = sqlConnection;
                    getMostRecentWebUserIdCommand.CommandType = CommandType.Text;
                    getMostRecentWebUserIdCommand.CommandText =
                            string.Format("SELECT ID FROM UserAccounts WHERE Email = '{0}'", email);

                    id = (Guid)getMostRecentWebUserIdCommand.ExecuteScalar();
                }

                Trace.WriteLine(string.Format("Deleting useraccount {0}", id));

                //  Need to use parameter because it's a Guid.
                var idParameter = new SqlParameter
                {
                    Value = id,
                    DbType = DbType.Guid,
                    Direction = ParameterDirection.Input,
                    ParameterName = "@ID"
                };

                using (var deleteUserClaimsCommand = new SqlCommand())
                {
                    deleteUserClaimsCommand.Parameters.Add(idParameter);
                    deleteUserClaimsCommand.Connection = sqlConnection;
                    deleteUserClaimsCommand.CommandType = CommandType.Text;
                    deleteUserClaimsCommand.CommandText = "DELETE FROM UserClaims WHERE UserAccountID = @ID;";

                    deleteUserClaimsCommand.ExecuteNonQuery();

                    deleteUserClaimsCommand.Parameters.Remove(idParameter);// Re-use param later
                }

                using (var deleteUserAccountCommand = new SqlCommand())
                {
                    deleteUserAccountCommand.Parameters.Add(idParameter);
                    deleteUserAccountCommand.Connection = sqlConnection;
                    deleteUserAccountCommand.CommandType = CommandType.Text;
                    deleteUserAccountCommand.CommandText = "DELETE FROM UserAccounts WHERE ID = @ID;";

                    var numRows = deleteUserAccountCommand.ExecuteNonQuery();

                    return numRows == 1;
                }
            }
        }

        public bool DeleteWebUser(string email)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();

                int idWebUser = default (int);
              
                using (var getMostRecentWebUserIdCommand = new SqlCommand())
                {
                    getMostRecentWebUserIdCommand.Connection = sqlConnection;
                    getMostRecentWebUserIdCommand.CommandType = CommandType.Text;
                    getMostRecentWebUserIdCommand.CommandText =
                            string.Format("SELECT idUser FROM WebUser WHERE Email = '{0}'", email);

                    idWebUser = (int)getMostRecentWebUserIdCommand.ExecuteScalar();
                }

                Trace.WriteLine(string.Format("Deleting useraccount {0}", idWebUser));

                using (var deleteWebUserCommand = new SqlCommand())
                {
                    deleteWebUserCommand.Connection = sqlConnection;
                    deleteWebUserCommand.CommandType = CommandType.Text;
                    deleteWebUserCommand.CommandText =
                            string.Format("DELETE FROM WebUser WHERE idUser = {0}", idWebUser);

                    var numRows = deleteWebUserCommand.ExecuteNonQuery();

                    return numRows == 1;
                }
            }
        }
    }
}
