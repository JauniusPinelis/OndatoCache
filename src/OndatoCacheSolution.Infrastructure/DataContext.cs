using Microsoft.EntityFrameworkCore;
using OndatoCacheSolution.Domain.Models;
using OndatoCacheSolution.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OndatoCacheSolution.Infrastructure
{
    public class DataContext: DbContext
    {
        public DbSet<DbCacheKey> Keys { get; set; }

        public DbSet<DbCacheValue> Values { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbCacheKey>()
                .HasKey(k => k.Key);

            modelBuilder.Entity<DbCacheKey>()
                 .HasMany(k => k.Values)
                 .WithOne(v => v.Key)
                 .HasForeignKey(v => v.KeyId);
        }
    }
}
