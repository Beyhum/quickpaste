using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quickpaste.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Data
{
    public class EfUploadLinkRepository: IUploadLinkRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<EfUploadLinkRepository> _logger;

        public EfUploadLinkRepository(AppDbContext dbContext, ILogger<EfUploadLinkRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<UploadLink> Create(UploadLink uploadLinkToCreate)
        {
            try
            {
                _dbContext.Add(uploadLinkToCreate);
                int affectedCount = await _dbContext.SaveChangesAsync();

                return affectedCount == 1 ? uploadLinkToCreate : null;
            }
            catch (SqlException ex)
            {
                _logger.LogError("Failed to create upload link", ex);
                return null;
            }
        }

        public async Task<bool> Delete(string quickLink)
        {
            UploadLink uploadLinkToDelete = await Get(quickLink);
            if (uploadLinkToDelete == null)
            {
                return true;
            }
            _dbContext.Remove(uploadLinkToDelete);

            int affectedCount = await _dbContext.SaveChangesAsync();

            return affectedCount == 1;
        }

        public async Task<UploadLink> Get(string quickLink)
        {
            UploadLink uploadLink = await _dbContext.UploadLinks.FindAsync(quickLink);

            return uploadLink;
        }
        

        public async Task<List<UploadLink>> GetAll()
        {
            return await _dbContext.UploadLinks.ToListAsync();
        }
    }
}
