using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wanankucha.Api.Domain.Entities;

namespace Wanankucha.Api.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);

        // Unique indexes for authentication
        builder.HasIndex(e => e.NormalizedEmail).IsUnique();
        builder.HasIndex(e => e.NormalizedUserName).IsUnique();

        // Non-unique indexes for token lookups
        builder.HasIndex(e => e.RefreshToken);
        builder.HasIndex(e => e.PasswordResetToken);

        builder.Property(e => e.UserName).HasMaxLength(256);
        builder.Property(e => e.Email).HasMaxLength(256);

        // Soft delete query filter
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
