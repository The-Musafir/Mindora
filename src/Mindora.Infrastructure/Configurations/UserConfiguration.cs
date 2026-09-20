using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Mindora.Domain.Entities;

namespace Mindora.Infrastructure.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(u => u.IsDeleted).HasDefaultValue(false);
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            builder.HasQueryFilter(u => !u.IsDeleted);
        }
    }
}