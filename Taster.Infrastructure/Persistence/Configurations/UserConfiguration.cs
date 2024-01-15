using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Taster.Domain.Entities;

namespace Taster.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Username).HasMaxLength(32);
            builder.Property(x => x.Email).HasMaxLength(256);
            builder.Property(x => x.HashedPassword).HasMaxLength(64);
        }
    }
}
