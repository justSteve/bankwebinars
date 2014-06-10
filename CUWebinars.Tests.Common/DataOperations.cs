using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CUWebinars.Tests.Common
{
    public class DataOperations
    {
        public string ConnectionString { get; set; }

        public string GetNameOfLatestPersistedNotification()
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

    }
}
