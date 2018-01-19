using Quickpaste.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Quickpaste.Data
{
    public interface IPasteRepository
    {
        /// <summary>
        /// Retrieve a paste by its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The paste with Id = id if found, otherwise null</returns>
        Task<Paste> GetById(string id);

        /// <summary>
        /// Retrieve a paste by its QuickLink
        /// </summary>
        /// <param name="link">The QuickLink of a paste</param>
        /// <returns>The paste with QuickLink = link if found, otherwise null</returns>
        Task<Paste> GetByLink(string link);

        /// <summary>
        /// Create a paste
        /// </summary>
        /// <param name="pasteToCreate"></param>
        /// <returns>The created paste on success, otherwise null</returns>
        Task<Paste> Create(Paste pasteToCreate);

        /// <summary>
        /// Delete a paste with Id = pasteId
        /// </summary>
        /// <param name="pasteId"></param>
        /// <returns>true on successful delete, false otherwise</returns>
        Task<bool> Delete(string pasteId);

        /// <summary>
        /// Gets all pastes in ascending order by creation date
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="limit"></param>
        /// <returns>All pastes</returns>
        Task<List<Paste>> GetAll(int pageNumber = 1, int limit = 20);

    }
}
