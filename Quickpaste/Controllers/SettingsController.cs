using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Quickpaste.Models.Dtos;
using Quickpaste.Services.BlobServices;

namespace Quickpaste.Controllers
{
    [Route("api/[controller]")]
    public class SettingsController : Controller
    {
        private IOptions<BlobStorageSettings> _blobStorageSettings;

        public SettingsController(IOptions<BlobStorageSettings> blobStorageSettings)
        {
            _blobStorageSettings = blobStorageSettings;
        }

        /// <summary>
        /// Retrieve the most recent settings for the web app
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetSettings()
        {
            var maxFileBytes = _blobStorageSettings.Value.MaxFileBytes;
            return Ok(new SettingsGetDto() { MaxFileBytes = maxFileBytes });
        }
        
    }
}
