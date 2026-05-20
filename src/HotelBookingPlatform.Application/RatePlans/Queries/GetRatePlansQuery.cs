using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Application.Interfaces;
using HotelBookingPlatform.Domain.Common;
using MediatR;

namespace HotelBookingPlatform.Application.RatePlans.Queries;

public sealed record GetRatePlansQuery(Guid RoomTypeId) : IRequest<Result<IReadOnlyList<RatePlanDto>>>;

public sealed class GetRatePlansQueryHandler(IRatePlanReadRepository repo)
    : IRequestHandler<GetRatePlansQuery, Result<IReadOnlyList<RatePlanDto>>>
{
    public async Task<Result<IReadOnlyList<RatePlanDto>>> Handle(GetRatePlansQuery request, CancellationToken cancellationToken)
    {
        var result = await repo.GetByRoomTypeIdAsync(request.RoomTypeId, cancellationToken);
        return Result.Success(result);
    }
}
