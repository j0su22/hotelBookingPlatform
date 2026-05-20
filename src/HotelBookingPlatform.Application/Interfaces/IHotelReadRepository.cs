using HotelBookingPlatform.Application.Common.DTOs;
using HotelBookingPlatform.Domain.Common;

namespace HotelBookingPlatform.Application.Interfaces;

public interface IHotelReadRepository
{
    Task<PagedResult<HotelDto>> GetPagedAsync(PagedRequest request, CancellationToken cancellationToken = default);
    Task<HotelDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
