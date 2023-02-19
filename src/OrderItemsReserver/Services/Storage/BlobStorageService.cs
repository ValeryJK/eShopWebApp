using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OrderItemsReserver.Configuration.Interfaces;
using OrderItemsReserver.Extensions;
using OrderItemsReserver.Services.Storage.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace OrderItemsReserver.Services.Storage
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly IBlobStorageServiceConfiguration _blobStorageServiceConfiguration;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<BlobStorageService> _logger;

        public BlobStorageService(IBlobStorageServiceConfiguration blobStorageServiceConfiguration,
            BlobServiceClient blobServiceClient, ILogger<BlobStorageService> logger)
        {
            _blobStorageServiceConfiguration = blobStorageServiceConfiguration
                    ?? throw new ArgumentNullException(nameof(blobStorageServiceConfiguration));

            _blobServiceClient = blobServiceClient
                    ?? throw new ArgumentNullException(nameof(blobServiceClient));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> UploadBlobAsync(Stream stream, string name)
        {
            try
            {
                _logger.LogInformation($"Creating document {name} ...");

                Debug.Assert(stream.CanSeek);
                stream.Seek(0, SeekOrigin.Begin);
                var container = await GetBlobContainer();

                BlobClient blob = container.GetBlobClient(name);
                await blob.UploadAsync(stream);

                _logger.LogInformation($"Document {name} uploaded successfully");
                return blob.Uri.AbsoluteUri;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Document {name} was not uploaded successfully - error details: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteBlobIfExistsAsync(string blobName)
        {
            try
            {
                var container = await GetBlobContainer();
                var blockBlob = container.GetBlobClient(blobName);
                await blockBlob.DeleteIfExistsAsync();

                _logger.LogInformation($"Document {blobName} was deleted successfully");
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Document {blobName} was not deleted successfully - error details: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DoesBlobExistAsync(string blobName)
        {
            try
            {
                var container = await GetBlobContainer();
                var blockBlob = container.GetBlobClient(blobName);
                var doesBlobExist = await blockBlob.ExistsAsync();

                return doesBlobExist.Value;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Document {blobName} existence cannot be verified - error details: {ex.Message}");
                throw;
            }
        }

        public async Task DownloadBlobIfExistsAsync(Stream stream, string blobName)
        {
            try
            {
                var container = await GetBlobContainer();
                var blockBlob = container.GetBlobClient(blobName);

                await blockBlob.DownloadToAsync(stream);

                _logger.LogInformation($"Document {blobName} was downloded successfully");

            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Cannot download document {blobName} - error details: {ex.Message}");
                if (ex.ErrorCode != "404")
                {
                    throw;
                }
            }
        }

        public async Task<string> GetBlobUrl(string blobName)
        {
            try
            {
                var container = await GetBlobContainer();
                var blob = container.GetBlobClient(blobName);

                string blobUrl = blob.Uri.AbsoluteUri;
                return blobUrl;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Url for document {blobName} was not found - error details: {ex.Message}");
                throw;
            }
        }

        private async Task<BlobContainerClient> GetBlobContainer()
        {
            try
            {
                BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(_blobStorageServiceConfiguration.ContainerName);

                await container.CreateIfNotExistsAsync();

                return container;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError($"Cannot find blob container: {_blobStorageServiceConfiguration.ContainerName} - error details: {ex.Message}");
                throw;
            }
        }

        public string GenerateSasTokenForContainer()
        {
            BlobSasBuilder builder = new BlobSasBuilder();
            builder.BlobContainerName = _blobStorageServiceConfiguration.ContainerName;
            builder.ContentType = "text";
            builder.SetPermissions(BlobAccountSasPermissions.Read);
            builder.StartsOn = DateTimeOffset.UtcNow;
            builder.ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(90);
            var sasToken = builder.ToSasQueryParameters(new StorageSharedKeyCredential(_blobStorageServiceConfiguration.AccountName,
                _blobStorageServiceConfiguration.Key)).ToString();

            return sasToken;
        }
    }
}
