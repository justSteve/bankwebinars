using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using log4net.Appender;
using log4net.Core;
//from http://axilis.hr/log4net-windows-azure/
namespace CUWebinars.Web.Utils
{
    public class AzureTableAppender : AppenderSkeleton
    {
        private TableServiceContext context;

        public override void ActivateOptions()
        {
            base.ActivateOptions();

            var storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(
                    "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString"));

            var tableClient = storageAccount.CreateCloudTableClient();
            tableClient.CreateTableIfNotExist("log4net");
            context = tableClient.GetDataServiceContext();
        }

        protected override void Append(LoggingEvent e)
        {
            context.AddObject("log4net", new LogItem
                                             {
                                                 Exception = e.GetExceptionString(),
                                                 Level = e.Level.Name,
                                                 LoggerName = e.LoggerName,
                                                 Message = e.RenderedMessage,
                                                 RoleInstance = RoleEnvironment.CurrentRoleInstance.Id
                                             });
            context.SaveChanges();
        }
    }
}