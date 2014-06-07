using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace CUWebinars.WebUi.Tests.Infrastructure
{
    public class DataOperations
    {
        private readonly string _connectionString;

        public DataOperations()
        {
            WebUiTestGlobals globalConfig = WebUiTestGlobals.WebUiTestGlobalsConfigSingleton;
            _connectionString = globalConfig.TtsDatabaseConnectionString;
        }

        public string GetNameOfLatestPersistedNotification()
        {
            using (var sqlConnection = new SqlConnection(_connectionString))
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

    }
}
