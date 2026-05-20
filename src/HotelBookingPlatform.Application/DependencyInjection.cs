using FluentValidation;
using HotelBookingPlatform.Application.Bookings.Commands;
using HotelBookingPlatform.Application.Common.Behaviors;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HotelBookingPlatform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BookingOptions>(configuration.GetSection("Booking"));

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
