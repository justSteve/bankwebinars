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
        public static IEnumerable<BlobFileInfo> ListFolderBlobs(string containerName, string directoryName)
        {
            var blobContainer = GetBlobContainer(containerName);
            var blobDirectory = blobContainer.GetDirectoryReference(directoryName);
            var blobInfos = new List<BlobFileInfo>();
            var blobs = blobDirectory.ListBlobs().ToList();
            foreach (var blob in blobs)
            {
                if (blob is CloudBlockBlob)
                {
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
                }
                if (blob is CloudBlobDirectory)
                {
                    var blobDir = blob.Uri.OriginalString.Replace(blob.Container.Uri.OriginalString + "/", "");
                    blobDir = blobDir.Remove(blobDir.Length - 1);
                    var subBlobs = ListFolderBlobs(containerName, blobDir);
                    blobInfos.AddRange(subBlobs);
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
            ICloudBlob blockBlob = container.GetBlockBlobReference(theFileName);
            //var readPolicy = blockBlob.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            //{

            //    SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-10),
            //    Permissions = SharedAccessBlobPermissions.Read,
            //    SharedAccessExpiryTime = DateTime.UtcNow.AddDays(1),
            //});

            //return new Uri(blockBlob.Uri.AbsoluteUri + readPolicy);
            return new Uri(blockBlob.Uri.AbsoluteUri);
            // returns the correct URI with a SIG value
            // but when attempting to follow the link:
            //<Error><Code>AuthenticationFailed</Code><Message>Server failed to authenticate the request. Make sure the value of Authorization header is formed correctly including the signature.
            // RequestId:bd8a2812-0001-0042-7670-384c80000000
            // Time:2016-11-06T21:00:44.8375295Z</Message><AuthenticationErrorDetail>Signature did not match. String to sign used was r
            // 
            // 2016-11-07T21:00:30Z
            // /storeforbw/invoicesprivate/10-31-2016/45-2016-62.pdf
            // </AuthenticationErrorDetail></Error>
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
    }
}