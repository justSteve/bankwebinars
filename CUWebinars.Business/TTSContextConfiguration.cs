using System.Data.Entity;
using System.Data.Entity.SqlServer;


namespace CUWebinars.Business
{
    public class TtsContextConfiguration : DbConfiguration
    {
        public TtsContextConfiguration() 
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}
