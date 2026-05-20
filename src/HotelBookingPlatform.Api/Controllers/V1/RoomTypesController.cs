using Asp.Versioning;
using HotelBookingPlatform.Api.Extensions;
using HotelBookingPlatform.Application.RoomTypes.Commands;
using HotelBookingPlatform.Application.RoomTypes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingPlatform.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/hotels/{hotelId:guid}/room-types")]
public sealed class RoomTypesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetByHotel(Guid hotelId, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetRoomTypesByHotelQuery(hotelId), ct);
        return result.ToActionResult(this);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(Guid hotelId, [FromBody] CreateRoomTypeRequest request, CancellationToken ct = default)
    {
        var command = new CreateRoomTypeCommand(hotelId, request.Name, request.Description, request.MaxCapacity, request.BasePrice);
        var result = await mediator.Send(command, ct);
        return result.ToActionResult(this, created: true);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid hotelId, Guid id, [FromBody] UpdateRoomTypeRequest request, CancellationToken ct = default)
    {
        var command = new UpdateRoomTypeCommand(id, request.Name, request.Description, request.MaxCapacity, request.BasePrice);
        var result = await mediator.Send(command, ct);
        return result.ToActionResult(this);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid hotelId, Guid id, CancellationToken ct = default)
    {
        var result = await mediator.Send(new DeleteRoomTypeCommand(id), ct);
        return result.ToActionResult(this);
    }
}

public sealed record CreateRoomTypeRequest(string Name, string Description, int MaxCapacity, decimal BasePrice);
public sealed record UpdateRoomTypeRequest(string Name, string Description, int MaxCapacity, decimal BasePrice);
