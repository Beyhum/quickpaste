using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Services.BlobServices.Azure
{
    public class AzureBlobStorageStrategy : IBlobStorageStrategy
    {
        private readonly ILogger _logger;
        private readonly IOptions<AzureBlobStorageSettings> _azureStorageSettings;

        private readonly string _publicContainerName;
        private readonly string _privateContainerName;

        private CloudBlobContainer GetPrivateContainer(CloudBlobClient existingClient = null)
        {
            CloudBlobClient blobClient = existingClient ?? StorageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(_privateContainerName);

        }
        private CloudBlobContainer GetPublicContainer(CloudBlobClient existingClient = null)
        {
            CloudBlobClient blobClient = existingClient ?? StorageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(_publicContainerName);

        }

        public CloudStorageAccount StorageAccount
        {
            get
            {
                string account = _azureStorageSettings.Value.Account;
                string key = _azureStorageSettings.Value.Key;
                string connectionString = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", account, key);
                return CloudStorageAccount.Parse(connectionString);
            }
        }

        public AzureBlobStorageStrategy(IOptions<AzureBlobStorageSettings> storageSettings, ILogger<AzureBlobStorageStrategy> logger)
        {
            _azureStorageSettings = storageSettings;
            _publicContainerName = storageSettings.Value.PublicContainerName;
            _privateContainerName = storageSettings.Value.PrivateContainerName;
            _logger = logger;
        }
        

        private static string GetBlobNameFromUrl(string url)
        {

            string[] delimiters = { "/" };
            string[] urlParts = url.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            return urlParts[urlParts.Length - 1];

        }

        


        public string GenerateSharedBlobUrl(string blockBlobName, int durationInMinutes)
        {
            CloudBlobContainer container = GetPrivateContainer();
            
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blockBlobName);

            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();
            sasConstraints.SharedAccessStartTime = DateTimeOffset.UtcNow.AddMinutes(-3); // set start time 3 mins in past to compensate for clock skew
            sasConstraints.SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(durationInMinutes);
            sasConstraints.Permissions = SharedAccessBlobPermissions.Read;

            //Generate a shared access signature for the blob with the above constraints
            string sasBlobToken = blockBlob.GetSharedAccessSignature(sasConstraints);
            return blockBlob.Uri + sasBlobToken;


        }

        


        public async Task<bool> DeleteFile(string blockBlobName)
        {

            // Create the blob client and reference the container
            CloudBlobClient blobClient = StorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_publicContainerName);


            CloudBlockBlob blockBlob = container.GetBlockBlobReference(blockBlobName);
            if (!await blockBlob.ExistsAsync()) // if not found in public container, check in private container
            {
                container = blobClient.GetContainerReference(_privateContainerName);
                blockBlob = container.GetBlockBlobReference(blockBlobName);
                // blob doesn't exist in either public nor private container
                if (!await blockBlob.ExistsAsync())
                {
                    return true;
                }
            }

            bool result = await blockBlob.DeleteIfExistsAsync();

            return result;


        }
        public async Task<string> UploadFile(string fileName, string fileContentType, IFormFile fileToUpload, bool isPublic)
        {

            string fullPath = null;

            try
            {

                // Create the blob client and reference the public/private container
                CloudBlobClient blobClient = StorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(isPublic ? _publicContainerName : _privateContainerName);

                // Upload file to Blob Storage
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                blockBlob.Properties.ContentType = fileContentType;
                await blockBlob.UploadFromStreamAsync(fileToUpload.OpenReadStream());
                
                var uriBuilder = new UriBuilder(blockBlob.Uri);
                fullPath = uriBuilder.ToString();

            }
            catch (StorageException ex)
            {
                _logger.LogInformation("Failed to upload file " + fileToUpload.FileName, ex);
                return null;
            }

            return fullPath;
        }


        #region For future use
        /// <summary>
        /// Currently not used
        /// Create a shared access policy through which SAS tokens can be created. If the policy is deleted, all SAS tokens linked to it are invalid
        /// </summary>
        /// <param name="policyName">Name of the policy to be created</param>
        /// <param name="durationInMinutes">How long should the policy be valid for in minutes</param>
        /// <returns></returns>
        private async Task CreateSharedAccessPolicy(string policyName, int durationInMinutes)
        {

            DateTimeOffset expiryTime = new DateTimeOffset(DateTime.UtcNow.AddMinutes(durationInMinutes));

            CloudBlobContainer container = GetPrivateContainer();

            //Get the container's existing permissions.
            BlobContainerPermissions permissions = await container.GetPermissionsAsync();

            // if policy already exists, simply update it
            if (permissions.SharedAccessPolicies.ContainsKey(policyName))
            {
                permissions.SharedAccessPolicies[policyName].SharedAccessExpiryTime = expiryTime;
            }
            else
            {
                //Create a new shared access policy and define its constraints.
                SharedAccessBlobPolicy sharedPolicy = new SharedAccessBlobPolicy()
                {
                    Permissions = SharedAccessBlobPermissions.Read,
                    SharedAccessExpiryTime = expiryTime
                };

                //Add the new policy to the container's permissions, and set the container's permissions.
                permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
                await container.SetPermissionsAsync(permissions);
            }

        }


        /// <summary>
        /// Currently not used
        /// Deletes a policy, invalidating storage access tokens that are associated with it
        /// </summary>
        /// <param name="policyName"></param>
        /// <returns>true on success, false on failure</returns>
        private async Task<bool> DeleteSharedAccessPolicy(string policyName)
        {
            try
            {
                CloudBlobContainer privateContainer = GetPrivateContainer();
                BlobContainerPermissions permissions = await privateContainer.GetPermissionsAsync();
                if (!permissions.SharedAccessPolicies.ContainsKey(policyName))
                {
                    return true;
                }
                bool success = permissions.SharedAccessPolicies.Remove(policyName);
                await privateContainer.SetPermissionsAsync(permissions);

                return success;
            }
            catch (StorageException ex)
            {
                _logger.LogError("Failed to delete policy: " + policyName, ex);
                return false;
            }
        }
        #endregion


    }
}
