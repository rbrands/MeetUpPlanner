using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using MeetUpPlanner.Shared;


namespace MeetUpPlanner.Functions
{
    /// <summary>
    /// Helper class to access blob storage
    /// </summary>
    public class BlobStorageRepository
    {
        private IConfiguration _config;
        private string _storageAccountName;
        private string _storageAccountKey;
        private string _storageUri;
        private string _container;

        public BlobStorageRepository(IConfiguration config)
        {
            _config = config;
            _storageAccountName = _config["BLOB_ACCOUNT"];
            _storageAccountKey = _config["BLOB_KEY"];
            _storageUri = _config["BLOB_URI"];
            _container = "tracks";
        }

        /// <summary>
        /// Creates a SAS for uploading a file
        /// </summary>
        /// <returns></returns>
        public BlobAccessSignature GetBlobAccessSignatureForUpload(string blobName, string fileName)
        {
            // Assemble Sas with the required permission to write a file to given container
            BlobSasBuilder sas = new BlobSasBuilder
            {
                BlobContainerName = _container,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10.0)
            };
            sas.SetPermissions(BlobAccountSasPermissions.Create | BlobAccountSasPermissions.Write);

            // Create credentials to sign SAS
            StorageSharedKeyCredential credential = new StorageSharedKeyCredential(_storageAccountName, _storageAccountKey);

            // Build Blob Uri including SAS
            BlobUriBuilder blobUri = new BlobUriBuilder(new Uri(_storageUri));
            blobUri.BlobContainerName = _container;
            blobUri.BlobName = blobName;
            blobUri.Sas = sas.ToSasQueryParameters(credential);

            BlobAccessSignature blobAccess = new BlobAccessSignature(blobName, fileName, blobUri.ToUri());

            return blobAccess;
        }
        public BlobAccessSignature GetBlobAccessSignatureForPNGImageUpload()
        {
            string fileName = $"{System.Guid.NewGuid().ToString()}.png";
            string blobName = $"images/{fileName}";
            return GetBlobAccessSignatureForUpload(blobName, fileName);
        }
        public BlobAccessSignature GetBlobAccessSignatureForGPXUpload()
        {
            string fileName = $"{System.Guid.NewGuid().ToString()}.gpx";
            string blobName = $"gpx/{fileName}";
            return GetBlobAccessSignatureForUpload(blobName, fileName);
        }

        public async Task SetBlobMetadata(Uri blobUri, IDictionary<string, string> metadata)
        {
            StorageSharedKeyCredential credential = new StorageSharedKeyCredential(_storageAccountName, _storageAccountKey);
            BlobClient blobClient = new BlobClient(blobUri, credential);
            await blobClient.SetMetadataAsync(metadata);
        }
        public async Task SetBlobMetadata(Uri blobUri, string key, string value)
        {
            IDictionary<string, string> metadata = new Dictionary<string, string>();
            await SetBlobMetadata(blobUri, metadata);
        }

        public Uri GetUriOfFile(string folder, string name)
        {
            BlobUriBuilder blobUri = new BlobUriBuilder(new Uri(_storageUri));
            blobUri.BlobContainerName = _container;
            blobUri.BlobName = $"{folder}/{name}";
            return blobUri.ToUri();
        }
    }
}
