using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Services.BlobServices.Azure
{
    public class AzureBlobStorageInitializer
    {
        public static async void InitializeStorage(AzureBlobStorageStrategy storageService, IOptions<AzureBlobStorageSettings> azureStorageSettings)
        {
            try
            {
                CloudStorageAccount storageAccount = storageService.StorageAccount;

                // Create a blob client and retrieve reference to containers
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer publicContainer = blobClient.GetContainerReference(azureStorageSettings.Value.PublicContainerName);

                // Create the "public container if it doesn't already exist.
                if (await publicContainer.CreateIfNotExistsAsync())
                {
                    // Enable public access
                    await publicContainer.SetPermissionsAsync(
                        new BlobContainerPermissions
                        {
                            PublicAccess =
                                BlobContainerPublicAccessType.Blob
                        });

                }

                CloudBlobContainer privateContainer = blobClient.GetContainerReference(azureStorageSettings.Value.PrivateContainerName);

                // Create the "public container if it doesn't already exist.
                await privateContainer.CreateIfNotExistsAsync();
            }
            catch (StorageException ex)
            {
                throw new StorageException("Failed to initialize public and private blob containers", ex);
            }

        }
    }
}
