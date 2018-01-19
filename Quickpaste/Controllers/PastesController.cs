using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Quickpaste.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Quickpaste.Services.PasteServices;
using Quickpaste.Models;
using Quickpaste.Services.UserServices;
using Quickpaste.Data;
using Quickpaste.StartupSetup.Caching;

namespace Quickpaste.Controllers
{
    [Authorize("DefaultPolicy")]
    [Route("api/[controller]")]
    public class PastesController : Controller
    {
        private readonly IUploadLinkRepository _uploadLinkRepository;
        private readonly UserService _userService;
        private readonly PasteService _pasteService;

        public PastesController(PasteService pasteService, UserService userService, IUploadLinkRepository uploadLinkRepository)
        {
            _pasteService = pasteService;
            _userService = userService;
            _uploadLinkRepository = uploadLinkRepository;
        }

        /// <summary>
        /// Create a paste as an authenticated user
        /// </summary>
        /// <param name="pasteCreateModel">The Paste to create's properties</param>
        /// <returns>The created Paste on success, ApiError on failure</returns>
        [Produces(typeof(Paste))]
        [HttpPost]
        public async Task<IActionResult> CreatePaste([FromForm] PastePostDto pasteCreateModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiError(ModelState));
            }

            if (await _pasteService.GetPasteByLink(pasteCreateModel.QuickLink) != null)
            {
                return BadRequest(new ApiError("QuickLink already exists"));
            }

            var createdPaste = await _pasteService.CreatePaste(pasteCreateModel);

            if (createdPaste == null)
            {
                return StatusCode(500, new ApiError("Failed to create new paste"));
            }

            return CreatedAtAction("GetPaste", new { identifier = createdPaste.Id }, createdPaste);

        }

        /// <summary>
        /// Delete a paste given its ID
        /// </summary>
        /// <param name="id">The Paste's ID</param>
        /// <returns>The deleted Paste on success, ApiError on failure</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaste(string id)
        {
            PasteGetDto deletedPaste = await _pasteService.DeletePaste(id);

            if (deletedPaste != null)
            {
                return Ok(deletedPaste);
            }
            return BadRequest(new ApiError("Failed to delete paste"));
        }

        /// <summary>
        /// Gets a paste given its ID or quickLink
        /// </summary>
        /// <param name="identifier">The Id or QuickLink of the Paste</param>
        /// <param name="isLink">When true, searches for the Paste by QuickLink</param>
        /// <returns>Paste on success, no content on failure</returns>
        [AllowAnonymous]
        [Produces(typeof(PasteGetDto))]
        [ETagFilter(200)]
        [HttpGet("{identifier}")]
        public async Task<IActionResult> GetPaste(string identifier, bool isLink = false)
        {
            PasteGetDto paste = isLink ? await _pasteService.GetPasteByLink(identifier) : await _pasteService.GetPaste(identifier);

            if (paste == null)
            {
                return NotFound();
            }
            if (!paste.IsPublic && !User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            return Ok(paste);

        }


        [Produces(typeof(List<Paste>))]
        [ETagFilter(200)]
        [HttpGet]
        public async Task<IActionResult> GetAllPastes([FromQuery]int pageNumber, [FromQuery]int limit = 5)
        {

            return Ok(await _pasteService.GetAllPastes(pageNumber, limit));

        }

        /// <summary>
        /// Create a paste anonymously given a quickLink that matches a created UploadLink
        /// </summary>
        /// <param name="quickLink">The QuickLink of a created UploadLink that will also be the new Paste's QuickLink</param>
        /// <param name="anonPasteCreateModel">The Paste to create's properties</param>
        /// <returns>No Content on success, ApiError on failure</returns>
        [AllowAnonymous]
        [HttpPost("{quickLink}")]
        public async Task<IActionResult> CreatePasteFromLink([FromRoute] string quickLink, [FromForm] PasteAnonPostDto anonPasteCreateModel)
        {
            UploadLink uploadLink = await _uploadLinkRepository.Get(quickLink);
            if(uploadLink == null)
            {
                return NotFound(new ApiError("The upload link you've entered does not exist or has been used"));
            }

            bool success = await _pasteService.CreatePaste(uploadLink, anonPasteCreateModel);

            if (success)
            {
                return NoContent();
            }
            return BadRequest(new ApiError("Failed to create paste"));
        }

        /// <summary>
        /// Create a link which can be used to view a paste's blob that is normally private. After sharedLinkInput.durationInMinutes has passed, the link is no longer valid
        /// Can also be used to generate a temporary link for authenticated users to download their own blobs
        /// </summary>
        /// <param name="pasteId"></param>
        /// <param name="sharedLinkInput">The settings of the created shared link (duration)</param>
        /// <returns>string URL which gives access to the normally private blob on success, otherwise null</returns>
        [Produces(typeof(SharedLinkGetDto))]
        [HttpPost("{pasteId}/SharedLinks")]
        public async Task<IActionResult> CreateSharedLink(string pasteId, [FromBody] SharedLinkPostDto sharedLinkInput)
        {
            SharedLinkGetDto sharedLink = await _pasteService.CreateSharedLink(pasteId, sharedLinkInput);

            if (sharedLink == null)
            {
                return BadRequest();
            }

            return Ok(sharedLink);
        }
    }
}
