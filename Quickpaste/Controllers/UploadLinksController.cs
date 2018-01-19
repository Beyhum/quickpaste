using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Quickpaste.Models;
using Quickpaste.Services.UserServices;
using Quickpaste.Services.PasteServices;
using Quickpaste.Data;
using Microsoft.AspNetCore.Authorization;
using Quickpaste.Models.Dtos;

namespace Quickpaste.Controllers
{
    [Authorize("DefaultPolicy")]
    [Route("api/[controller]")]
    public class UploadLinksController : Controller
    {
        private readonly IUploadLinkRepository _uploadLinkRepository;
        private readonly UserService _userService;
        private readonly PasteService _pasteService;

        public UploadLinksController(PasteService pasteService, UserService userService, IUploadLinkRepository uploadLinkRepository)
        {
            _pasteService = pasteService;
            _userService = userService;
            _uploadLinkRepository = uploadLinkRepository;
        }

        /// <summary>
        /// Create an upload link
        /// </summary>
        /// <param name="uploadLink"></param>
        /// <returns>UploadLink on success, ApiError on failure</returns>
        [Produces(typeof(UploadLink))]
        [HttpPost]
        public async Task<IActionResult> CreateUploadLink([FromBody] UploadLink uploadLink)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiError(ModelState));
            }

            if (await _pasteService.GetPasteByLink(uploadLink.QuickLink) != null)
            {
                return BadRequest(new ApiError("QuickLink already exists"));
            }
            

            UploadLink createdUploadLink = await _uploadLinkRepository.Create(uploadLink);

            if (createdUploadLink == null)
            {
                return StatusCode(500, new ApiError("Failed to create new upload link"));
            }

            return CreatedAtAction("GetUploadLink", new { quickLink = createdUploadLink.QuickLink }, createdUploadLink);

        }

        /// <summary>
        /// Get an UploadLink based on its QuickLink property. Useful to know if it exists/allows upload links
        /// </summary>
        /// <param name="quickLink">The UploadLink's QuickLink property</param>
        /// <returns>UploadLink on success, ApiError on failure</returns>
        [AllowAnonymous]
        [Produces(typeof(UploadLink))]
        [HttpGet("{quickLink}")]
        public async Task<IActionResult> GetUploadLink(string quickLink)
        {

            UploadLink uploadLink = await _uploadLinkRepository.Get(quickLink);

            if (uploadLink == null)
            {
                return NotFound(new ApiError("The upload link you've entered does not exist or has been used"));
            }

            return Ok(uploadLink);

        }

        /// <summary>
        /// Get all created upload links. Only available to authenticated user
        /// </summary>
        /// <returns>List of UploadLinks</returns>
        [Produces(typeof(List<UploadLink>))]
        [HttpGet]
        public async Task<IActionResult> GetUploadLinks()
        {

            List<UploadLink> uploadLinks = await _uploadLinkRepository.GetAll();
            return Ok(uploadLinks);
        }

        /// <summary>
        /// Delete an UploadLink based on its QuickLink property
        /// </summary>
        /// <param name="quickLink"></param>
        /// <returns>No Content on success, ApiError on failure</returns>
        [HttpDelete("{quickLink}")]
        public async Task<IActionResult> DeleteUploadLink(string quickLink)
        {

            bool success = await _uploadLinkRepository.Delete(quickLink);
            if (success)
            {
                return NoContent();
            }
            return BadRequest(new ApiError("Failed to delete Upload Link"));

        }


    }
}
