using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quickpaste.Services.AuthServices;
using Quickpaste.StartupSetup;

namespace Quickpaste.Services.BlobServices.Local
{
    public class LocalBlobStorageStrategy : IBlobStorageStrategy
    {
        private readonly BlobDbContext _blobContext;
        private readonly HostingSettings _hostingSettings;
        private readonly AuthorizationService _authorizationService;
        private readonly ILogger _logger;

        private Uri baseUrl;
        public const string BlobNameClaim = "blob";

        public LocalBlobStorageStrategy(BlobDbContext blobContext, IOptions<HostingSettings> hostingSettings, AuthorizationService authorizationService, ILogger<LocalBlobStorageStrategy> logger)
        {
            _blobContext = blobContext;
            _hostingSettings = hostingSettings.Value;
            _authorizationService = authorizationService;
            _logger = logger;

            baseUrl = new Uri(new Uri(_hostingSettings.Hostname), "/files/");
        }

        private static string GetBlobNameFromUrl(string url)
        {

            string[] delimiters = { "/" };
            string[] urlParts = url.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            return urlParts[urlParts.Length - 1];

        }

        public async Task<bool> DeleteFile(string blobName)
        {

            LocalBlob blob = await _blobContext.Blobs.FindAsync(blobName);

            if (blob == null) // if not found, just return true
            {
                return true;
            }
            _blobContext.Remove(blob);

            int affectedCount = await _blobContext.SaveChangesAsync();
            return affectedCount == 1;
        }

        /// <summary>
        /// Returns true if the given token is valid and its 'blob' claim matches blobName
        /// </summary>
        /// <param name="blobName">Name of the blob to which the token corresponds</param>
        /// <param name="token"></param>
        /// <returns>true if valid, false otherwise</returns>
        public bool ValidStorageAccessToken(string blobName, string token)
        {
            var claimsPrincipal = _authorizationService.ValidateToken(token);
            if(claimsPrincipal == null)
            {
                return false;
            }

            return claimsPrincipal.Claims.FirstOrDefault(c => c.Type == BlobNameClaim).Value == blobName;
        }

        public string GenerateSharedBlobUrl(string blobName, int durationInMinutes)
        {
            string tokenIdentifier = Guid.NewGuid().ToString();
            Claim[] claims = {
                new Claim(BlobNameClaim, blobName),
                new Claim(JwtRegisteredClaimNames.Jti, tokenIdentifier)
            };
            string fileNameAndToken = blobName + "?token=" + WebUtility.UrlEncode(_authorizationService.CreateCustomToken(claims, durationInMinutes));
            return new Uri(baseUrl, fileNameAndToken).ToString();
        }



        public async Task<string> UploadFile(string fileName, string fileContentType, IFormFile fileToUpload, bool isPublic)
        {

            string fullPath = null;
            try
            {
                LocalBlob blobToUpload = null;
                using (var memoryStream = new MemoryStream())
                {
                    await fileToUpload.CopyToAsync(memoryStream);
                    blobToUpload = new LocalBlob(fileName, memoryStream.ToArray(), fileContentType, isPublic);

                }
                _blobContext.Add(blobToUpload);
                int affectedRows = await _blobContext.SaveChangesAsync();
                if (affectedRows == 0)
                {
                    return null;
                }
                fullPath = new Uri(this.baseUrl, blobToUpload.BlobName).ToString();

            }
            catch (SqlException ex)
            {
                _logger.LogInformation("Failed to upload file " + fileToUpload.FileName, ex);
                return null;
            }

            return fullPath;
        }

        public async Task<LocalBlob> GetBlob(string blobName)
        {
            return await _blobContext.Blobs.FindAsync(blobName);
        }
    }
}
