using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wanankucha.Api.Domain.Entities;

namespace Wanankucha.Api.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.NormalizedName).IsUnique();

        builder.Property(e => e.Name).HasMaxLength(256);

        // Soft delete query filter
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
