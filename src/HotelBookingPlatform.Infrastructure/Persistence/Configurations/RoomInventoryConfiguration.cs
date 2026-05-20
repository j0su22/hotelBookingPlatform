using HotelBookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingPlatform.Infrastructure.Persistence.Configurations;

public sealed class RoomInventoryConfiguration : IEntityTypeConfiguration<RoomInventory>
{
    public void Configure(EntityTypeBuilder<RoomInventory> builder)
    {
        builder.HasKey(ri => ri.Id);
        builder.Property(ri => ri.Date).IsRequired();
        builder.Property(ri => ri.TotalRooms).IsRequired();
        builder.Property(ri => ri.AvailableRooms).IsRequired();
        builder.Property(ri => ri.RowVersion).IsRowVersion();

        builder.HasOne(ri => ri.RoomType)
            .WithMany(rt => rt.RoomInventories)
            .HasForeignKey(ri => ri.RoomTypeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ri => new { ri.RoomTypeId, ri.Date }).IsUnique();
    }
}
