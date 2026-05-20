using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Application.Interfaces;
using HotelBookingPlatform.Domain.Common;
using MediatR;

namespace HotelBookingPlatform.Application.Bookings.Queries;

public sealed record GetBookingsQuery(
    string? Status = null,
    string? GuestEmail = null,
    Guid? HotelId = null,
    DateOnly? CheckInFrom = null,
    DateOnly? CheckInTo = null,
    int PageNumber = 1,
    int PageSize = 10,
    string SortBy = "CreatedAt",
    string SortDirection = "desc") : IRequest<Result<PagedResult<BookingDto>>>;

public sealed class GetBookingsQueryHandler(IBookingReadRepository repo)
    : IRequestHandler<GetBookingsQuery, Result<PagedResult<BookingDto>>>
{
    public async Task<Result<PagedResult<BookingDto>>> Handle(GetBookingsQuery request, CancellationToken cancellationToken)
    {
        var filters = new BookingFilters(request.Status, request.GuestEmail, request.HotelId, request.CheckInFrom, request.CheckInTo);
        var paged = new PagedRequest
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            SortBy = request.SortBy,
            SortDirection = request.SortDirection
        };

        var result = await repo.GetPagedAsync(filters, paged, cancellationToken);
        return Result.Success(result);
    }
}
