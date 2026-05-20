using HotelBookingPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelBookingPlatform.Infrastructure.Persistence.Configurations;

public sealed class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.HasKey(ir => ir.Id);
        builder.Property(ir => ir.Key).IsRequired().HasMaxLength(200);
        builder.Property(ir => ir.RequestHash).HasMaxLength(64);
        builder.Property(ir => ir.ResponseStatus).IsRequired();
        builder.Property(ir => ir.ResponseBody).IsRequired();
        builder.Property(ir => ir.ExpiresAt).IsRequired();

        builder.HasIndex(ir => ir.Key).IsUnique();
        builder.HasIndex(ir => ir.ExpiresAt);
    }
}
