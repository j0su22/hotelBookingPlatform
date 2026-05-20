using HotelBookingPlatform.Application.Interfaces;
using HotelBookingPlatform.Domain.Interfaces;
using HotelBookingPlatform.Infrastructure.Persistence;
using HotelBookingPlatform.Infrastructure.Persistence.Seed;
using HotelBookingPlatform.Infrastructure.Repositories.Read;
using HotelBookingPlatform.Infrastructure.Repositories.Write;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBookingPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BookingDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("Default"),
                sql => sql.EnableRetryOnFailure(3)));

        // Write repositories (via UnitOfWork)
        services.AddScoped<Domain.Interfaces.IUnitOfWork, UnitOfWork.UnitOfWork>();
        services.AddScoped<IIdempotencyRepository, IdempotencyRepository>();

        // Read repositories (Dapper)
        services.AddScoped<IHotelReadRepository, HotelReadRepository>();
        services.AddScoped<IRoomTypeReadRepository, RoomTypeReadRepository>();
        services.AddScoped<IRatePlanReadRepository, RatePlanReadRepository>();
        services.AddScoped<IAvailabilityReadRepository, AvailabilityReadRepository>();
        services.AddScoped<IBookingReadRepository, BookingReadRepository>();

        // Seeder
        services.AddScoped<DataSeeder>();

        return services;
    }
}
