using Tada.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tada.Infrastructure.Persistence.Configurations
{
    public class ApplicationUserConfiguration: IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> entity)
        {
            entity.Property(e => e.Firstname)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.Surname)
                .IsRequired()
                .HasMaxLength(255);
            entity.Property(e => e.SecondSurname)
                .HasMaxLength(255);

        }
    }
}
