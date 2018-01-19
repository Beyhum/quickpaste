using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quickpaste.Models;

namespace Quickpaste.Data
{
    public class EfUserRepository : IUserRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<EfUserRepository> _logger;

        public EfUserRepository(AppDbContext dbContext, ILogger<EfUserRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<int> Count()
        {
            return await _dbContext.Users.CountAsync();
        }

        public async Task<User> Create(User userToCreate)
        {
            try
            {
                _dbContext.Add(userToCreate);
                int affectedCount = await _dbContext.SaveChangesAsync();

                return affectedCount == 1 ? userToCreate : null;
            }
            catch (SqlException ex)
            {
                _logger.LogError("Failed to create user", ex);
                return null;
            }
        }

        public async Task<bool> Delete(string username)
        {
            User userToDelete = await Get(username);
            if (userToDelete == null)
            {
                return true;
            }
            _dbContext.Remove(userToDelete);

            int affectedCount = await _dbContext.SaveChangesAsync();

            return affectedCount == 1;
        }

        public async Task<User> Get(string username)
        {
            User user = await _dbContext.Users.FindAsync(username);
            return user;

        }
    }
}
