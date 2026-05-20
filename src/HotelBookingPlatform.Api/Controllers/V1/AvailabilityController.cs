using Asp.Versioning;
using HotelBookingPlatform.Api.Extensions;
using HotelBookingPlatform.Application.Availability.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingPlatform.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/availability")]
public sealed class AvailabilityController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] Guid? hotelId,
        [FromQuery] DateOnly checkIn,
        [FromQuery] DateOnly checkOut,
        [FromQuery] int guests = 1,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetAvailabilityQuery(hotelId, checkIn, checkOut, guests), ct);
        return result.ToActionResult(this);
    }
}
