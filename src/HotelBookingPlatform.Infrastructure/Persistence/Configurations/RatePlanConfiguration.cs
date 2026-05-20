using HotelBookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingPlatform.Infrastructure.Persistence.Configurations;

public sealed class RatePlanConfiguration : IEntityTypeConfiguration<RatePlan>
{
    public void Configure(EntityTypeBuilder<RatePlan> builder)
    {
        builder.HasKey(rp => rp.Id);
        builder.Property(rp => rp.Name).IsRequired().HasMaxLength(200);
        builder.Property(rp => rp.PricePerNight).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(rp => rp.ValidFrom).IsRequired();
        builder.Property(rp => rp.ValidTo).IsRequired();
        builder.Property(rp => rp.IsActive).IsRequired();

        builder.HasOne(rp => rp.RoomType)
            .WithMany(rt => rt.RatePlans)
            .HasForeignKey(rp => rp.RoomTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(rp => new { rp.RoomTypeId, rp.IsActive });
    }
}
