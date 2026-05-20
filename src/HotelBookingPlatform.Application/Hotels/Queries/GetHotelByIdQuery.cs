using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Application.Interfaces;
using HotelBookingPlatform.Domain.Common;
using MediatR;

namespace HotelBookingPlatform.Application.Hotels.Queries;

public sealed record GetHotelByIdQuery(Guid Id) : IRequest<Result<HotelDto>>;

public sealed class GetHotelByIdQueryHandler(IHotelReadRepository repo)
    : IRequestHandler<GetHotelByIdQuery, Result<HotelDto>>
{
    public async Task<Result<HotelDto>> Handle(GetHotelByIdQuery request, CancellationToken cancellationToken)
    {
        var hotel = await repo.GetByIdAsync(request.Id, cancellationToken);
        if (hotel is null)
            return Result.Failure<HotelDto>("Hotel no encontrado.", "NOT_FOUND");

        return Result.Success(hotel);
    }
}
