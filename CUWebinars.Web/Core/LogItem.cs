using System;

namespace CUWebinars.Web.Core
{
    public sealed class LogItem : Microsoft.WindowsAzure.StorageClient.TableServiceEntity
    {
        public string Exception { get; set; }
        public string Level { get; set; }
        public string LoggerName { get; set; }
        public string Message { get; set; }
        public string RoleInstance { get; set; }

        public LogItem()
        {
            var dateTime = DateTime.Now;
            PartitionKey = string.Format("{0}-{1}", dateTime.Month, dateTime.Year);
            RowKey = Guid.NewGuid().ToString();
        }
    }
}