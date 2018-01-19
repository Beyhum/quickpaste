using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Quickpaste.Models;

namespace Quickpaste.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Paste> Pastes { get; set; }
        public DbSet<UploadLink> UploadLinks { get; set; }
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public AppDbContext()
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Paste>().HasIndex(paste => paste.QuickLink).IsUnique(true);
            builder.Entity<Paste>().HasIndex(paste => paste.IsPublic);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }



    }
}
