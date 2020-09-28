using System;
using ApiTemplate.Values.Domain.Entities;
using ApiTemplate.Values.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiTemplate.Values.Infrastructure.Data.Models
{
    internal class DocumentsTypeConfiguration : IEntityTypeConfiguration<DocumentEntity>
    {
        public void Configure(EntityTypeBuilder<DocumentEntity> configuration)
        {
            configuration.ToTable("Documents");
            configuration.HasKey(e => e.Identifier);
            configuration.HasData(new DocumentEntity(new DocumentId(Guid.Empty), "abc", " abc"));
        }
    }
}