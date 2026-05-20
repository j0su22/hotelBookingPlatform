using HotelBookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingPlatform.Infrastructure.Persistence.Configurations;

public sealed class GuestConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(g => g.LastName).IsRequired().HasMaxLength(100);
        builder.Property(g => g.Email).IsRequired().HasMaxLength(200);
        builder.Property(g => g.PhoneNumber).IsRequired().HasMaxLength(20);

        builder.HasIndex(g => g.Email).IsUnique();
    }
}
