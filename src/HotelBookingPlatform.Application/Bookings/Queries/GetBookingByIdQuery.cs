using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Application.Interfaces;
using HotelBookingPlatform.Domain.Common;
using MediatR;

namespace HotelBookingPlatform.Application.Bookings.Queries;

public sealed record GetBookingByIdQuery(Guid Id) : IRequest<Result<BookingDetailDto>>;

public sealed class GetBookingByIdQueryHandler(IBookingReadRepository repo)
    : IRequestHandler<GetBookingByIdQuery, Result<BookingDetailDto>>
{
    public async Task<Result<BookingDetailDto>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await repo.GetByIdAsync(request.Id, cancellationToken);
        if (booking is null)
            return Result.Failure<BookingDetailDto>("Reserva no encontrada.", "NOT_FOUND");

        return Result.Success(booking);
    }
}
