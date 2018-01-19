using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Quickpaste.Services.BlobServices.Local
{
    /// <summary>
    /// Db Context for blobs that are associated with pastes. Required for saving blobs in local storage instead of cloud storage
    /// </summary>
    public class BlobDbContext : DbContext
    {
        public DbSet<LocalBlob> Blobs { get; set; }

        public BlobDbContext(DbContextOptions<BlobDbContext> options) : base(options)
        {
        }

        public BlobDbContext()
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }



    }
}
