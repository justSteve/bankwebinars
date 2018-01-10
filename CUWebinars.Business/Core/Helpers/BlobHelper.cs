using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using CUWebinars.Business.Notification;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace CUWebinars.Business.Core.Helpers
{
    public class BlobFileInfo
    {
        public string FileName { get; set; }
        public string BlobPath { get; set; }
        public string BlobUri { get; set; }
        public string BlobFilePath { get; set; }
        public IListBlobItem Blob { get; set; }
    }

    public static class BlobHelper
    {
        // Load blob container


        public static CloudBlobContainer GetBlobContainer(string containerName)
        {

            TtsConfigHelper _ttsConfig = new TtsConfigHelper();

            var storageCredentials = new StorageCredentials(_ttsConfig.GetStorageAccountName(),
                _ttsConfig.GetStorageAccessKey());

            var storageAccount = new CloudStorageAccount(storageCredentials, false);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            //var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            return container;
        }

        // Get recursive list of files
        public static IEnumerable<BlobFileInfo> ListFolderBlobs(string containerName)
        {
            var blobInfos = new List<BlobFileInfo>();
            //var blobs = blobDirectory.ListBlobs().ToList();

            TtsConfigHelper _ttsConfig = new TtsConfigHelper();

            var storageCredentials = new StorageCredentials(_ttsConfig.GetStorageAccountName(),
                _ttsConfig.GetStorageAccessKey());

            var storageAccount = new CloudStorageAccount(storageCredentials, false);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            //// Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("invoicesprivate");
            var blobs = container.ListBlobs(null, false).ToList();
            // Loop over items within the container and output the length and URI.
            foreach (IListBlobItem item in container.ListBlobs(null, false))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;

                    var blobFileName = blob.Uri.Segments.Last().Replace("%20", " ");
                    var blobFilePath = blob.Uri.AbsolutePath.Replace(blob.Container.Uri.AbsolutePath + "/", "").Replace("%20", " ");
                    var blobUri = blob.StorageUri.PrimaryUri.ToString();
                    var blobPath = blobFilePath.Replace("/" + blobFileName, "");
                    blobInfos.Add(new BlobFileInfo
                    {
                        FileName = blobFileName,
                        BlobPath = blobPath,
                        BlobUri = blobUri,
                        BlobFilePath = blobFilePath,
                        Blob = blob
                    });

                    Console.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);
                }
                else if (item.GetType() == typeof(CloudPageBlob))
                {
                    CloudPageBlob pageBlob = (CloudPageBlob)item;
                    Console.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);
                }
                else if (item.GetType() == typeof(CloudBlobDirectory))
                {
                    CloudBlobDirectory directory = (CloudBlobDirectory)item;
                    CloudBlobContainer container2ndLevel = blobClient.GetContainerReference(item.Uri.ToString().Replace(blobClient.BaseUri.ToString(), "").TrimEnd('/'));
                    ListFolderBlobs(container2ndLevel.ToString());
                    Console.WriteLine("Directory: {0}", directory.Uri);
                }
            }

            return blobInfos;
        }


        public static Uri GetInvoiceForPage(string theFileName)
        {
            //Objective is to retrieve a pdf from azure storage 
            // and to present a link to that file using a 
            // key presented by this 'readPolicy'

            TtsConfigHelper _ttsConfig = new TtsConfigHelper();
            var storageCredentials = new StorageCredentials(_ttsConfig.GetStorageAccountName(), _ttsConfig.GetStorageAccessKey());

            var storageAccount = new CloudStorageAccount(storageCredentials, false);
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = client.GetContainerReference("invoicesprivate/");

            // Retrieve reference to a blob.
            ICloudBlob blockBlob = container.GetBlockBlobReference(theFileName.TrimEnd('/'));
            var readPolicy = blockBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                SharedAccessStartTime = DateTime.UtcNow.AddDays(-1),
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddDays(1),
            });

            //return new Uri(blockBlob.Uri.AbsoluteUri + readPolicy);
            return new Uri(blockBlob.Uri.AbsoluteUri);
        }
        public static Uri GetPromosForPage(string theFileName)
        {
            //Objective is to retrieve a pdf from azure storage 
            // and to present a link to that file using a 
            // key presented by this 'readPolicy'

            TtsConfigHelper _ttsConfig = new TtsConfigHelper();
            var storageCredentials = new StorageCredentials(_ttsConfig.GetStorageAccountName(), _ttsConfig.GetStorageAccessKey());

            var storageAccount = new CloudStorageAccount(storageCredentials, false);
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = client.GetContainerReference("v3generator/");

            // Retrieve reference to a blob.
            ICloudBlob blockBlob = container.GetBlockBlobReference(theFileName.TrimEnd('/'));
            var readPolicy = blockBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {

                SharedAccessStartTime = DateTime.UtcNow.AddDays(-1),
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddDays(1),
            });

            //return new Uri(blockBlob.Uri.AbsoluteUri + readPolicy);
            return new Uri(blockBlob.Uri.AbsoluteUri);
        }


        public static bool BlobExistsOnCloud(string containerName, string dirName, string key)
        {

            TtsConfigHelper _ttsConfig = new TtsConfigHelper();
            var storageCredentials = new StorageCredentials(_ttsConfig.GetStorageAccountName(),
                _ttsConfig.GetStorageAccessKey());

            var storageAccount = new CloudStorageAccount(storageCredentials, false);
            CloudBlobClient client = storageAccount.CreateCloudBlobClient();

            return client.GetContainerReference(containerName)
                .GetDirectoryReference(dirName)
                .GetBlockBlobReference(key)
                .Exists();
        }
        public static BlobFileInfo GetBlob(string containerName, string directoryName, string blobName)
        {
            var blobContainer = GetBlobContainer(containerName);
            if (!BlobExistsOnCloud(containerName, directoryName, blobName))
                return null;

            var blobDirectory = blobContainer.GetDirectoryReference(directoryName);
            var blob = blobDirectory.GetBlockBlobReference(blobName);


            var blobFileName = blob.Uri.Segments.Last().Replace("%20", " ");

            var blobFilePath = blob.Uri.AbsolutePath.Replace(blob.Container.Uri.AbsolutePath + "/", "").Replace("%20", " ");
            var blobUri = blob.StorageUri.PrimaryUri.ToString();
            var blobPath = blobFilePath.Replace("/" + blobFileName, "");
            var thisBlob = (new BlobFileInfo
            {
                FileName = blobFileName,
                BlobPath = blobPath,
                BlobUri = blobUri,
                BlobFilePath = blobFilePath,
                Blob = blob
            });
            return thisBlob;
        }

        internal static List<BlobFileInfo> GetAffiliateBlobs(string containerName, int idAffiliate)
        {
            var returnList = new List<BlobFileInfo>();
            var topFolders = ListFolderBlobs(containerName);


            return null;
        }
    }
}