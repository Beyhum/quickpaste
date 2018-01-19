using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Services.BlobServices
{
    /// <summary>
    /// Interface used by BlobStorageService to manipulate blobs
    /// Register the appropriate implementation in Startup.cs
    /// </summary>
    public interface IBlobStorageStrategy
    {
        /// <summary>
        /// Uploads a given file to private or public storage
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileContentType"></param>
        /// <param name="fileToUpload"></param>
        /// <param name="isPublic"></param>
        /// <returns>The URL corresponding to the uploaded file on success, null on failure</returns>
        Task<string> UploadFile(string fileName, string fileContentType, IFormFile fileToUpload, bool isPublic);

        /// <summary>
        /// Deletes the file with the corresponding blobName
        /// </summary>
        /// <param name="blobName"></param>
        /// <returns>true on success, false on failure</returns>
        Task<bool> DeleteFile(string blobName);

        /// <summary>
        /// Generates a URL which gives access to a blob (file) that is normally private for the duration specified in durationInMinutes
        /// </summary>
        /// <param name="blobName">The name of the blob to create a shared link for</param>
        /// <param name="durationInMinutes">How long should the shared link be valid for</param>
        /// <returns>string URL which gives access to the blob for durationInMinutes</returns>
        string GenerateSharedBlobUrl(string blobName, int durationInMinutes);



    }
}
