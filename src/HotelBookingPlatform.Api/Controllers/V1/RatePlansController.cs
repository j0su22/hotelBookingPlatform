using Asp.Versioning;
using HotelBookingPlatform.Api.Extensions;
using HotelBookingPlatform.Application.RatePlans.Commands;
using HotelBookingPlatform.Application.RatePlans.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingPlatform.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/rate-plans")]
public sealed class RatePlansController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByRoomType([FromQuery] Guid roomTypeId, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetRatePlansQuery(roomTypeId), ct);
        return result.ToActionResult(this);
    }

    [HttpGet("calculate-price")]
    public async Task<IActionResult> CalculatePrice(
        [FromQuery] Guid ratePlanId,
        [FromQuery] DateOnly checkIn,
        [FromQuery] DateOnly checkOut,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new CalculatePriceQuery(ratePlanId, checkIn, checkOut), ct);
        return result.ToActionResult(this);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateRatePlanCommand command, CancellationToken ct = default)
    {
        var result = await mediator.Send(command, ct);
        return result.ToActionResult(this, created: true);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRatePlanRequest request, CancellationToken ct = default)
    {
        var command = new UpdateRatePlanCommand(id, request.Name, request.PricePerNight, request.ValidFrom, request.ValidTo);
        var result = await mediator.Send(command, ct);
        return result.ToActionResult(this);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var result = await mediator.Send(new DeleteRatePlanCommand(id), ct);
        return result.ToActionResult(this);
    }
}

public sealed record UpdateRatePlanRequest(string Name, decimal PricePerNight, DateOnly ValidFrom, DateOnly ValidTo);
