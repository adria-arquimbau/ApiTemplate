using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ApiTemplate.Values.Domain.Entities;

namespace ApiTemplate.Values.Infrastructure.Data.Models
{
    internal class ValueItemTypeConfiguration : IEntityTypeConfiguration<ValueItemEntity>
    {
        public void Configure(EntityTypeBuilder<ValueItemEntity> configuration)
        {
            configuration.ToTable("ValueItems");
            configuration.HasKey(property => property.Id);
            configuration.HasData(new ValueItemEntity("asdf", 42) {Id = new Guid("10000000-0000-0000-0000-000000000000")});
        }
    }
}