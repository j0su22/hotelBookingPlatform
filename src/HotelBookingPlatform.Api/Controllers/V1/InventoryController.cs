using Asp.Versioning;
using HotelBookingPlatform.Api.Extensions;
using HotelBookingPlatform.Application.Inventory.Commands;
using HotelBookingPlatform.Application.Inventory.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingPlatform.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/room-types/{roomTypeId:guid}/inventory")]
public sealed class InventoryController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Returns all inventory records for a room type within the given date range.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get(
        Guid roomTypeId,
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetInventoryQuery(roomTypeId, from, to), ct);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Creates or updates inventory for every day in [from, to).
    /// Existing bookings are preserved — only TotalRooms changes.
    /// Requires authentication.
    /// </summary>
    [HttpPut]
    [Authorize]
    public async Task<IActionResult> Upsert(
        Guid roomTypeId,
        [FromBody] UpsertInventoryRequest request,
        CancellationToken ct = default)
    {
        var command = new UpsertInventoryCommand(roomTypeId, request.From, request.To, request.TotalRooms);
        var result = await mediator.Send(command, ct);
        return result.ToActionResult(this);
    }
}

public sealed record UpsertInventoryRequest(DateOnly From, DateOnly To, int TotalRooms);
