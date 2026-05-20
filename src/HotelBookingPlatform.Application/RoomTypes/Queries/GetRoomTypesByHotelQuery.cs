using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Application.Interfaces;
using HotelBookingPlatform.Domain.Common;
using MediatR;

namespace HotelBookingPlatform.Application.RoomTypes.Queries;

public sealed record GetRoomTypesByHotelQuery(Guid HotelId) : IRequest<Result<IReadOnlyList<RoomTypeDto>>>;

public sealed class GetRoomTypesByHotelQueryHandler(IRoomTypeReadRepository repo)
    : IRequestHandler<GetRoomTypesByHotelQuery, Result<IReadOnlyList<RoomTypeDto>>>
{
    public async Task<Result<IReadOnlyList<RoomTypeDto>>> Handle(GetRoomTypesByHotelQuery request, CancellationToken cancellationToken)
    {
        var result = await repo.GetByHotelIdAsync(request.HotelId, cancellationToken);
        return Result.Success(result);
    }
}
