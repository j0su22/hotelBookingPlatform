using HotelBookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingPlatform.Infrastructure.Persistence.Configurations;

public sealed class RoomTypeConfiguration : IEntityTypeConfiguration<RoomType>
{
    public void Configure(EntityTypeBuilder<RoomType> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(200);
        builder.Property(r => r.Description).IsRequired().HasMaxLength(1000);
        builder.Property(r => r.MaxCapacity).IsRequired();
        builder.Property(r => r.BasePrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(r => r.RowVersion).IsRowVersion();

        builder.HasOne(r => r.Hotel)
            .WithMany(h => h.RoomTypes)
            .HasForeignKey(r => r.HotelId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(r => r.HotelId);
    }
}
