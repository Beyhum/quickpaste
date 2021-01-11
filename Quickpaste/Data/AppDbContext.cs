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
            HandleSqliteDateTimeOffsetConversion(builder);

        }

        /// <summary>
        ///  EF Core SQLite provider does not support DateTimeOffset, so we convert it to a string before storing it in SQLite. 
        ///  See: https://docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations#query-limitations
        /// </summary>
        /// <param name="builder"></param>
        private void HandleSqliteDateTimeOffsetConversion(ModelBuilder builder)
        {
            if (Database.ProviderName == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                foreach (var entityType in builder.Model.GetEntityTypes())
                {
                    var properties = entityType.ClrType.GetProperties().Where(p => p.PropertyType == typeof(DateTimeOffset)
                                                                                || p.PropertyType == typeof(DateTimeOffset?));
                    foreach (var property in properties)
                    {
                        builder
                            .Entity(entityType.Name)
                            .Property(property.Name)
                            .HasConversion(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.DateTimeOffsetToStringConverter());
                    }
                }
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }



    }
}
