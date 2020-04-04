using System;
using System.Threading.Tasks;
using MAVN.Service.NotificationSystem.Domain.Models;
using MAVN.Service.NotificationSystem.Domain.Repositories;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace MAVN.Service.NotificationSystem.AzureRepositories
{
    public class TemplateContentRepository : ITemplateContentRepository
    {
        private readonly string _blobConnectionString;
        private readonly CloudBlobClient _blobClient;
        private static BlobRequestOptions _blobRequestOptions = new BlobRequestOptions
        {
            MaximumExecutionTime = TimeSpan.FromMinutes(15),
        };

        public TemplateContentRepository(string blobConnectionString)
        {
            _blobConnectionString = blobConnectionString;
            _blobClient = CloudStorageAccount.Parse(blobConnectionString).CreateCloudBlobClient();
        }

        public async Task SaveContentAsync(string templateName, Localization localization, string templateBody)
        {
            if (string.IsNullOrEmpty(templateBody))
                return;

            var blobContainer = _blobClient.GetContainerReference(templateName);
            var isExist = await blobContainer.ExistsAsync();
            if (!isExist)
            {
                await blobContainer.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Off, null, null);
            }

            var blob = blobContainer.GetAppendBlobReference(localization.ToString());

            await blob.CreateOrReplaceAsync();
            blob.Properties.ContentType = "text/plain";
            await blob.SetPropertiesAsync(null, _blobRequestOptions, null);
            await blob.UploadTextAsync(templateBody);
        }

        public async Task<string> GetContentAsync(string templateName, Localization local)
        {
            var blobContainer = _blobClient.GetContainerReference(templateName);
            var isExist = await blobContainer.ExistsAsync();
            if (!isExist)
            {
                return string.Empty;
            }

            var blob = blobContainer.GetAppendBlobReference(local.ToString());
            var isBlobExist = await blob.ExistsAsync();
            if (!isBlobExist)
            {
                return string.Empty;
            }

            var text = await blob.DownloadTextAsync();

            return text;
        }

        public async Task<bool> DeleteContentAsync(string templateName, Localization local)
        {
            var blobContainer = _blobClient.GetContainerReference(templateName);
            
            var isExist = await blobContainer.ExistsAsync();
            if (!isExist)
            {
                return false;
            }

            var blob = blobContainer.GetAppendBlobReference(local.ToString());

            var result = await blob.DeleteIfExistsAsync();
            return result;
        }

        public async Task DeleteContentWithAllLocalsAsync(string templateName, Localization local)
        {
            var blobContainer = _blobClient.GetContainerReference(templateName);
            await blobContainer.DeleteIfExistsAsync();
        }
    }
}
