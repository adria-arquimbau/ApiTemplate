using System;
using ApiTemplate.Values.Infrastructure.Data.Models;
using Microsoft.EntityFrameworkCore;
using ApiTemplate.Values.Domain.Entities;

namespace ApiTemplate.Values.Infrastructure.Data
{
    public class ValueItemDbContext : DbContext
    {
        public DbSet<ValueItem> ValueItems { get; set; }

        public ValueItemDbContext(DbContextOptions<ValueItemDbContext> contextOptions) : base(contextOptions)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Only for manual testing purposes. With a real database we should do this  migrations.
            modelBuilder.ApplyConfiguration(new ValueItemTypeConfiguration());
        }
    }
}
