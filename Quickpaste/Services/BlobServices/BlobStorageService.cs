using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Services.BlobServices
{
    /// <summary>
    /// Service to upload blobs. Uses an IBlobStorageStrategy to determine where are the blobs stored (i.e. Locally, Cloud)
    /// </summary>
    public class BlobStorageService
    {
        private readonly IBlobStorageStrategy _blobStorageStrategy;
        private readonly ILogger _logger;
        private readonly IOptions<BlobStorageSettings> _storageSettings;


        public BlobStorageService(IBlobStorageStrategy blobStorageStrategy, IOptions<BlobStorageSettings> storageSettings, ILogger<BlobStorageService> logger)
        {
            _blobStorageStrategy = blobStorageStrategy;
            _storageSettings = storageSettings;
            _logger = logger;

        }

        private static string GetBlobNameFromUrl(string url)
        {

            string[] delimiters = { "/" };
            string[] urlParts = url.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            return urlParts[urlParts.Length - 1];

        }

        /// <summary>
        /// Uploads a given file to private or public storage
        /// </summary>
        /// <param name="fileToUpload"></param>
        /// <param name="isPublic"></param>
        /// <returns>The URL corresponding to the uploaded file on success, null on failure</returns>
        public async Task<string> UploadFile(IFormFile fileToUpload, bool isPublic)
        {
            if (fileToUpload == null || fileToUpload.Length == 0 || fileToUpload.Length > _storageSettings.Value.MaxFileBytes)
            {
                _logger.LogInformation("Invalid file size", fileToUpload?.Name, fileToUpload?.Length);
                return null;
            }

            string fullPath = null;


            // Create a unique name for the file, based on a UNIX timestamp and Guid
            int epoch = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

            string fileExtension = Path.GetExtension(fileToUpload.FileName);
            string fileName = String.Format("{1}-{0}{2}",
                Guid.NewGuid().ToString(),
                epoch,
                fileExtension);
            string fileContentType = null;

            if(!new FileExtensionContentTypeProvider().Mappings.TryGetValue(fileExtension, out fileContentType)) // if couldn't find content type for given extension
            {
                fileContentType = "application/octet-stream";
            }
            

            fullPath = await _blobStorageStrategy.UploadFile(fileName, fileContentType, fileToUpload, isPublic);

            return fullPath;

        }

        /// <summary>
        /// Deletes the file with the corresponding URL
        /// </summary>
        /// <param name="url"></param>
        /// <returns>true on success, false on failure</returns>
        public async Task<bool> DeleteFile(string url)
        {
            if (url == null)
            {
                return true;
            }

            string blobName = GetBlobNameFromUrl(url);
            bool result = await _blobStorageStrategy.DeleteFile(blobName);
            return result;
        }

        /// <summary>
        /// Generates a URL which gives access to a blob (file) that is normally private for the duration specified in durationInMinutes
        /// </summary>
        /// <param name="blobUrl">The URL of the blob to create a storage access token for</param>
        /// <param name="durationInMinutes">How long should the storage token be valid for</param>
        /// <returns>string URL which gives access to the blob for durationInMinutes</returns>
        public string GenerateSharedBlobUrl(string blobUrl, int durationInMinutes)
        {
            string blobName = GetBlobNameFromUrl(blobUrl);

            return _blobStorageStrategy.GenerateSharedBlobUrl(blobName, durationInMinutes);
        }


    }
}
