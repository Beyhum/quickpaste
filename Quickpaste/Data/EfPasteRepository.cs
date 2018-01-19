using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Quickpaste.Models;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace Quickpaste.Data
{
    public class EfPasteRepository : IPasteRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<EfPasteRepository> _logger;

        public EfPasteRepository(AppDbContext dbContext, ILogger<EfPasteRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Paste> Create(Paste pasteToCreate)
        {
            try
            {
                _dbContext.Add(pasteToCreate);
                int affectedCount = await _dbContext.SaveChangesAsync();

                return affectedCount == 1 ? pasteToCreate : null;
            }
            catch (SqlException ex)
            {
                _logger.LogError("Failed to create paste", ex);
                return null;
            }
        }

        public async Task<bool> Delete(string pasteId)
        {
            Paste pasteToDelete = await GetById(pasteId);
            if(pasteToDelete == null)
            {
                return true;
            }
            _dbContext.Remove(pasteToDelete);

            int affectedCount = await _dbContext.SaveChangesAsync();

            return affectedCount == 1;
        }

        public async Task<Paste> GetById(string id)
        {
            Paste paste = await _dbContext.Pastes.FindAsync(id);

            return paste;
        }

        public async Task<Paste> GetByLink(string link)
        {
            Paste paste = (await _dbContext.Pastes.Where(r => r.QuickLink == link).ToListAsync()).FirstOrDefault();

            return paste;
        }
        public async Task<List<Paste>> GetAll(int pageNumber = 1, int limit = 20)
        {
            var query = _dbContext.Pastes.OrderByDescending(p => p.CreatedAt);
            if (pageNumber == 1)
            {
                return await query.Take(limit).ToListAsync();

            }else
            {
                return await query.Skip((pageNumber-1) * limit).Take(limit).ToListAsync();
            }

        }
    }
}
