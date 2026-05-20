using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Domain.Common;

namespace HotelBookingPlatform.Application.Interfaces;

public interface IBookingReadRepository
{
    Task<PagedResult<BookingDto>> GetPagedAsync(BookingFilters filters, PagedRequest request, CancellationToken cancellationToken = default);
    Task<BookingDetailDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

public sealed record BookingFilters(
    string? Status = null,
    string? GuestEmail = null,
    Guid? HotelId = null,
    DateOnly? CheckInFrom = null,
    DateOnly? CheckInTo = null);
