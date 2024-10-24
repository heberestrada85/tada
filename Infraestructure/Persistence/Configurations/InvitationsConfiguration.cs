using Tada.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Tada.Infrastructure.Persistence.Configurations
{
    public class InvitationsConfiguration: IEntityTypeConfiguration<Invitations>
    {
        public void Configure(EntityTypeBuilder<Invitations> entity)
        {
            entity.HasKey(e => e.Id).HasName("Invitation_pkey");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.InvitationName)
                .IsRequired()
                .HasMaxLength(255);
        }
    }
}
