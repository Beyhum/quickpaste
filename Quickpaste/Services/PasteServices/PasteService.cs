using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Quickpaste.Services.BlobServices;
using Quickpaste.Models;
using Quickpaste.Models.Dtos;
using Quickpaste.Data;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Quickpaste.Services.PasteServices
{
    public class PasteService
    {
        private readonly IUploadLinkRepository _uploadLinkRepository;
        private readonly ILogger _logger;
        private readonly IPasteRepository _pasteRepository;
        private readonly BlobStorageService _storageService;


        public PasteService(BlobStorageService storageService, IPasteRepository pasteRepository, IUploadLinkRepository uploadLinkRepository, ILogger<PasteService> logger)
        {
            _storageService = storageService;
            _pasteRepository = pasteRepository;
            _logger = logger;
            _uploadLinkRepository = uploadLinkRepository;
        }

        /// <summary>
        /// Creates a new Paste given a PastePostDto
        /// </summary>
        /// <param name="pasteCreateModel"></param>
        /// <returns>The created Paste on success, otherwise null</returns>
        public async Task<Paste> CreatePaste(PastePostDto pasteCreateModel)
        {
            return await CreatePasteHelper(pasteCreateModel.QuickLink, pasteCreateModel.Message, pasteCreateModel.FileToUpload, pasteCreateModel.IsPublic);
            
        }

        /// <summary>
        /// Creates a paste based on an upload link
        /// </summary>
        /// <param name="uploadLink"></param>
        /// <param name="anonPasteCreateModel"></param>
        /// <returns>true on success, false on failure</returns>
        public async Task<bool> CreatePaste(UploadLink uploadLink, PasteAnonPostDto anonPasteCreateModel)
        {

            if (!uploadLink.AllowFile)
            {
                anonPasteCreateModel.FileToUpload = null;
            }

            Paste createdPaste = await CreatePasteHelper(uploadLink.QuickLink, anonPasteCreateModel.Message, anonPasteCreateModel.FileToUpload, false);
            if (createdPaste != null)
            {
                await _uploadLinkRepository.Delete(uploadLink.QuickLink);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Helper method that actually creates a new paste and uploads a given file
        /// </summary>
        /// <param name="quickLink">The quick link associated with the paste</param>
        /// <param name="message">The paste's message</param>
        /// <param name="fileToUpload">The paste's file if it exists</param>
        /// <param name="isPublic">Whether the paste's content is publicly visible or not</param>
        /// <returns>Created Paste on success, null on failure</returns>
        private async Task<Paste> CreatePasteHelper(string quickLink, string message, IFormFile fileToUpload, bool isPublic)
        {

            Paste pasteToCreate = Paste.ToCreate(quickLink, message, null, isPublic);

            if (fileToUpload == null)
            {
                return await _pasteRepository.Create(pasteToCreate);
            }

            Paste createdPasteResult = null;
            string fileUrl = null;
            try
            {
                fileUrl = await _storageService.UploadFile(fileToUpload, pasteToCreate.IsPublic);

                if (fileUrl == null)
                {
                    return null;
                }

                pasteToCreate.BlobUrl = fileUrl;

                createdPasteResult = await _pasteRepository.Create(pasteToCreate);
            }
            finally
            {
                if (createdPasteResult == null && fileUrl != null) // if Paste could not be created but file was uploaded, delete the file
                {
                    bool successfulDelete = await _storageService.DeleteFile(fileUrl);
                    if (!successfulDelete)
                    {
                        _logger.LogWarning($"Could not delete blob for paste that couldn't be created: {fileUrl}");
                    }
                }
            }

            return createdPasteResult;
        }

        /// <summary>
        /// Deletes a paste and its associated blob if exists
        /// </summary>
        /// <param name="pasteId">ID of paste to delete</param>
        /// <returns>Deleted paste on success, null on failure</returns>
        public async Task<PasteGetDto> DeletePaste(string pasteId)
        {
            Paste pasteToDelete = await _pasteRepository.GetById(pasteId);

            if (pasteToDelete?.BlobUrl != null)
            {
                bool successfulBlobDelete = await _storageService.DeleteFile(pasteToDelete.BlobUrl);
                if (!successfulBlobDelete)
                {
                    return null;
                }

            }

            bool successfulDelete = await _pasteRepository.Delete(pasteId);
            return successfulDelete ? new PasteGetDto(pasteToDelete) : null;

        }

        /// <summary>
        /// Gets the paste with Id = pasteId
        /// </summary>
        /// <param name="pasteId"></param>
        /// <returns>Paste if found, null otherwise</returns>
        public async Task<PasteGetDto> GetPaste(string pasteId)
        {
            Paste paste = await _pasteRepository.GetById(pasteId);
            if (paste == null)
            {
                return null;
            }

            PasteGetDto pasteDto = new PasteGetDto(paste);
            return pasteDto;
        }

        /// <summary>
        /// Gets the paste with QuickLink = link
        /// </summary>
        /// <param name="link">The paste's quickLink</param>
        /// <returns>Paste if found, null otherwise</returns>
        public async Task<PasteGetDto> GetPasteByLink(string link)
        {
            Paste paste = await _pasteRepository.GetByLink(link);
            if (paste == null)
            {
                return null;
            }
            PasteGetDto pasteDto = new PasteGetDto(paste);
            return pasteDto;
        }

        /// <summary>
        /// Returns all pastes from latest to oldest
        /// </summary>
        /// <returns>List of Pastes</returns>
        public async Task<List<PasteGetDto>> GetAllPastes(int pageNumber = 1, int limit = 5)
        {
            limit = Math.Min(limit, 20);
            return (await _pasteRepository.GetAll(pageNumber, limit)).Select(r => { return new PasteGetDto(r); }).OrderByDescending(r => r.CreatedAt).ToList();
        }

        /// <summary>
        /// Create a link which can be used to view a blob that is normally private. After sharedLinkInput.durationInMinutes has passed, the link is no longer valid
        /// Also used to generate a temporary link for authenticated users to download their own blobs
        /// </summary>
        /// <param name="pasteId"></param>
        /// <param name="sharedLinkInput"></param>
        /// <returns>string URL which gives access to the normally private blob on success, otherwise null</returns>
        public async Task<SharedLinkGetDto> CreateSharedLink(string pasteId, SharedLinkPostDto sharedLinkInput)
        {
            Paste pasteToShare = await _pasteRepository.GetById(pasteId);
            if(pasteToShare == null)
            {
                return null;
            }
            string sharedLinkUrl = _storageService.GenerateSharedBlobUrl(pasteToShare.BlobUrl, sharedLinkInput.DurationInMinutes);
            if(sharedLinkUrl == null)
            {
                return null;
            }
            return new SharedLinkGetDto(sharedLinkInput.DurationInMinutes, sharedLinkUrl);
        }
    }
}
