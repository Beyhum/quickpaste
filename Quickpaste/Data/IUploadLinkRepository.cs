using Quickpaste.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Data
{
    public interface IUploadLinkRepository
    {
        /// <summary>
        /// Creates a new upload link
        /// </summary>
        /// <param name="uploadLinkToCreate"></param>
        /// <returns>The created upload link on success, otherwise null</returns>
        Task<UploadLink> Create(UploadLink uploadLinkToCreate);

        /// <summary>
        /// Deletes an UploadLink with the given QuickLink
        /// </summary>
        /// <param name="quickLink"></param>
        /// <returns>true on success, false on failure</returns>
        Task<bool> Delete(string quickLink);

        /// <summary>
        /// Gets the UploadLink with the given QuickLink
        /// </summary>
        /// <param name="quickLink"></param>
        /// <returns>UploadLink if found, null otherwise</returns>
        Task<UploadLink> Get(string quickLink);

        /// <summary>
        /// Gets all upload link
        /// </summary>
        /// <returns>List of upload links</returns>
        Task<List<UploadLink>> GetAll();
    }
}
