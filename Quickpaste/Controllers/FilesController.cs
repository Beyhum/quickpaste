using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quickpaste.Services.BlobServices.Local;

namespace Quickpaste.Controllers
{
    /// <summary>
    /// Controller only used for blobs/files that are stored locally when using Local blob storage (with LocalBlobStorageStrategy)
    /// </summary>
    [Route("[controller]")]
    public class FilesController : Controller
    {
        private readonly LocalBlobStorageStrategy _localBlobStorageService;

        public FilesController(LocalBlobStorageStrategy localBlobStorageService)
        {
            _localBlobStorageService = localBlobStorageService;
        }

        [AllowAnonymous]
        [HttpGet("{blobName}")]
        public async Task<IActionResult> Get(string blobName, [FromQuery] string token)
        {
            LocalBlob blob = await _localBlobStorageService.GetBlob(blobName);
            if(blob == null)
            {
                return NotFound();
            }
            if (!blob.IsPublic)
            {
                if(token == null)
                {
                    return Unauthorized();
                }
                bool validToken = _localBlobStorageService.ValidStorageAccessToken(blobName, token);
                if (validToken)
                {
                    return File(blob.BlobData, blob.ContentType);
                }
                return Unauthorized();
            }

            return File(blob.BlobData, blob.ContentType);
        }

    }
}
