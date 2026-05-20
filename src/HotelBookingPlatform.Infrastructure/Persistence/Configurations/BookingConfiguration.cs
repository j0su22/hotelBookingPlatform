using HotelBookingPlatform.Domain.Entities;
using HotelBookingPlatform.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingPlatform.Infrastructure.Persistence.Configurations;

public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.CheckIn).IsRequired();
        builder.Property(b => b.CheckOut).IsRequired();
        builder.Property(b => b.Status).IsRequired().HasConversion<int>();
        builder.Property(b => b.TotalPrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(b => b.ConfirmationNumber).IsRequired().HasMaxLength(50);
        builder.Property(b => b.IdempotencyKey).IsRequired().HasMaxLength(200);
        builder.Property(b => b.GuestCount).IsRequired();
        builder.Property(b => b.RowVersion).IsRowVersion();

        builder.HasOne(b => b.Guest)
            .WithMany()
            .HasForeignKey(b => b.GuestId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.RoomType)
            .WithMany()
            .HasForeignKey(b => b.RoomTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(b => b.IdempotencyKey).IsUnique();
        builder.HasIndex(b => b.ConfirmationNumber).IsUnique();
        builder.HasIndex(b => b.Status);
        builder.HasIndex(b => b.CheckIn);
        builder.HasIndex(b => b.GuestId);
    }
}
