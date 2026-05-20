using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Application.Interfaces;
using HotelBookingPlatform.Domain.Common;
using MediatR;

namespace HotelBookingPlatform.Application.Hotels.Queries;

public sealed record GetHotelsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string SortBy = "CreatedAt",
    string SortDirection = "desc") : IRequest<Result<PagedResult<HotelDto>>>;

public sealed class GetHotelsQueryHandler(IHotelReadRepository repo)
    : IRequestHandler<GetHotelsQuery, Result<PagedResult<HotelDto>>>
{
    public async Task<Result<PagedResult<HotelDto>>> Handle(GetHotelsQuery request, CancellationToken cancellationToken)
    {
        var pagedRequest = new PagedRequest
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection
        };

        var result = await repo.GetPagedAsync(pagedRequest, cancellationToken);
        return Result.Success(result);
    }
}
